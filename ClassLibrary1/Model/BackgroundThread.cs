using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Threading;
using System.Diagnostics;
using System.Transactions;

using Microsoft.WindowsAzure.ServiceRuntime;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;


namespace ClassLibrary1.Model
{
    public class R8RBackgroundTask
    {
        public bool RepeatIndefinitely { get; set; }
        public bool MoreWorkToDo { get; internal set; }
        public bool CurrentlyInBriefPause { get; internal set; }
        public DateTime? ThisWebServerLastUpdateTime;
        public long LoopSetCompletedCount = 0;

        public R8RBackgroundTask()
        {
            MoreWorkToDo = true;
            RepeatIndefinitely = true;
            CurrentlyInBriefPause = false;
            BackgroundThreadManager.CurrentlyPaused = false;
        }

        /// <summary>
        /// Perform idle tasks (such as checking for a rating that needs resolution or updating)
        /// </summary>
        public void BackgroundTasksRunner(R8RDataManipulation dataManipulation)
        {
            lock (BackgroundThreadManager.padlock)
            {
                //if (BackgroundThread.ExitRequested)
                //{
                //    BackgroundThread.ExitGranted = true;
                //    return;
                //}
                dataManipulation.ResetDataContexts();


                //if (BackgroundThread.IsPauseRequested())
                //Trace.TraceInformation("Pause is requested.");
                MoreWorkToDo = true; // note that this may be relied on by external components, so until we've gone through all tasks with no more work to do, we must keep this at true
                const int numTasks = 20;
                const int numLoops = 10;
                for (int loop = 1; loop <= numLoops; loop++)
                {
                    if (loop != 1 && BackgroundThreadManager.PauseRequestedWhenWorkIsComplete && !MoreWorkToDo)
                    {
                        BackgroundThreadManager.PauseRequestedWhenWorkIsComplete = false;
                        BackgroundThreadManager.CurrentlyPaused = true;
                    }
                    MoreWorkToDo = CompleteSingleLoop(dataManipulation, numTasks, numLoops, loop);
                }
                LoopSetCompletedCount++;
                if (!MoreWorkToDo)
                {
                    if (!RoleEnvironment.IsAvailable)
                        Thread.Sleep(100); // sleep only long enough for unit tests to realize that the idle tasks have completed.
                    else
                        Thread.Sleep(3000);
                }
            }
            // Trace.TraceInformation("Exiting IdleTasks moreWorkToDo: " + moreWorkToDo.ToString());
        }

        private bool CompleteSingleLoop(R8RDataManipulation dataManipulation, int numTasks, int numLoops, int loop)
        {
            bool moreWorkToDo = true;
            dataManipulation.ResetDataContexts();
            bool[] moreWorkToDoThisTask = new bool[numTasks];
            for (int i = 1; i <= numTasks; i++)
            {
                if (BackgroundThreadManager.PauseRequestedImmediately)
                {
                    BackgroundThreadManager.PauseRequestedImmediately = false;
                    BackgroundThreadManager.CurrentlyPaused = true;
                }
                while (BackgroundThreadManager.CurrentlyPaused)
                {
                    Thread.Sleep(1); // very minimal pause until we check again to see if the pause request has been turned off
                }
                if (loop == 1 || loop == numLoops) // we try everything on first loop, as well as on last loop, so moreWorkToDo will be accurate
                    moreWorkToDoThisTask[i - 1] = true;
                if (!BackgroundThreadManager.IsBriefPauseRequested() && ((i == 1 && moreWorkToDoThisTask.Any(x => x == true)) || moreWorkToDoThisTask[i - 1]))
                {
                    // Trace.TraceInformation("Task " + i);
                    // ProfileSimple.Start("TaskNum" + i);
                    dataManipulation.ResetDataContexts();
                    try
                    {
                        CompleteSingleTask(dataManipulation, moreWorkToDoThisTask, i);
                        dataManipulation.DataContext.SubmitChanges();
                        dataManipulation.ResetDataContexts();
                    }
                    catch (Exception ex)
                    {
                        moreWorkToDoThisTask[i - 1] = false; // we don't want a single exception to stop all other background processing
                        Trace.TraceError("Idle task failed: " + ex.Message);
                    }
                    finally
                    {
                        dataManipulation.ResetDataContexts();
                        DatabaseAndAzureRoleStatus.CheckInRole(dataManipulation.DataContext);
                        WeakReferenceTracker.CheckUncollected();
                    }
                    //Trace.TraceInformation("TaskNum " + i);
                    // ProfileSimple.End("TaskNum" + i);

                    if (i != numTasks)
                        moreWorkToDo = true;
                    else
                        moreWorkToDo = moreWorkToDoThisTask.Any(x => x == true);
                }
            }
            return moreWorkToDo;
        }

        private static void CompleteSingleTask(R8RDataManipulation dataManipulation, bool[] moreWorkToDoThisTask, int i)
        {
            switch (i)
            {

                // Table as a whole

                case 1:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskCashOutPointsManagers();
                    break;

                // Table data

                case 2:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.RespondToResetTblRowFieldDisplays();
                    break;

                case 3:
                    moreWorkToDoThisTask[i - 1] = GeocodeUpdate.DoUpdate(dataManipulation.DataContext); // updating addresses that couldn't be geocoded before
                    break;

                case 4:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.ContinueLongProcess(); // currently, create missing ratings and upload table rows
                    break;

                // Rating status (note that long process above also can include ratings)

                case 5:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.FixStatusInconsistencies(); // From the domain level down to the Rating level, ensures that we can stop further short-term resolution activity and entering of new user ratings when it should be inactive/deleted
                    break;

                case 6:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.CompleteMultipleAddUserRatings(); // loads the UserRatingsToAdd and completes the process of adding UserRating to the database
                    break;

                case 7:
                    moreWorkToDoThisTask[i - 1] = StatusRecords.DeleteOldStatusRecords(dataManipulation.DataContext); // we are deleting old status records (which are used to ensure consistent sorting even after user ratings have been changed)
                    break;

                case 8:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskImplementResolutions(); // completes the ratinggroupresolutions in the proposed state
                    break;

                case 9:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskShortTermResolve(); // Sets the ShortTermResolutionValue for the RatingPhaseStatus
                    break;

                case 10:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskMakeHighStakesKnown();
                    break;

                case 11:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskConsiderDemotingHighStakesPrematurely();
                    break;

                case 12:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.AdvanceRatingGroupsNeedingAdvancing();
                    break;

                // Points and user ratings

                case 13:
                    moreWorkToDoThisTask[i - 1] = VolatilityTracking.UpdateTrackers(dataManipulation.DataContext); // updates volatility both for user ratings and for the TblRows that they are in
                    break;

                case 14:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskUpdatePointsBecauseOfSomethingOtherThanNewUserRating();
                    break;

                case 15:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskUpdatePointsAndUserInteractionsInResponseToRatingPhaseStatusTrigger();
                    break;

                case 16:
                    moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskReviewRecentUserRatings();
                    break;

                // Trust

                case 17:
                    // This also affects Ratings. Only the TrustTracker is consulted. So, if we moved trust tracking to a separate database, we would do this with the Ratings.
                    moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskFlagRatingsNeedingReviewBasedOnChangeInTrust(); // when a user's overall trust level has changed sufficiently, we set the ReviewRecentUserRatingsAfter field of the Ratings so that the UserRatings will be reviewed soon.
                    break;

                case 18:
                    // These are internal to trust tracking, but one is triggered by the adjust user interaction process.
                    moreWorkToDoThisTask[i - 1] = TrustTrackingBackgroundTasks.DoTrustTrackingBackgroundTasks(dataManipulation.DataContext);
                    break;

                case 19:
                    // As user ratings get old, we update how recent they are, and chnage the user interaction stats accordingly.
                    moreWorkToDoThisTask[i - 1] = RecencyUpdates.UpdateRecency(dataManipulation.DataContext);
                    break;

                // Data sync

                case 20:
                    moreWorkToDoThisTask[i - 1] = FastAccessTablesMaintenance.ContinueFastAccessMaintenance(dataManipulation.DataContext, new DenormalizedTableAccess(1)); // DEBUG: Must change this once we have denormalized tables stored in multiple locations
                    break;

            }
        }

        //public Thread GetBackgroundThread()
        //{
        //    return BackgroundThread.Instance.GetThread();
        //}

        public void Run()
        {
            MoreWorkToDo = true;
            CurrentlyInBriefPause = false;
            DateTime expireThread = TestableDateTime.Now + new TimeSpan(1, 0, 0);
            R8RDataManipulation theDataAccessModule = new R8RDataManipulation();
            while (TestableDateTime.Now < expireThread || RepeatIndefinitely)
            {
                //Trace.TraceInformation("Background task main run loop.");
                // Trace.TraceInformation("Running background task.");
                while (BackgroundThreadManager.IsBriefPauseRequested())
                {
                    CurrentlyInBriefPause = true;
                    //Trace.TraceInformation("Background task 5 second pause.");
                    Thread.Sleep(5000); // pause
                }

                CurrentlyInBriefPause = false;
                //Trace.TraceInformation("IdleTasksOnce");
                BackgroundTasksRunner(theDataAccessModule);
                if (!MoreWorkToDo)
                {
                    CurrentlyInBriefPause = true;
                    //Trace.TraceInformation("No more work to do. Pausing background task 0.5 seconds.");
                    Thread.Sleep(500);
                }
                CurrentlyInBriefPause = false;
            }
            //Trace.TraceInformation("Background task main run loop complete.");

        }
    }

    /// <summary>
    /// Summary description for BackgroundThreadManager
    /// </summary>
    public sealed class BackgroundThreadManager
    {
        bool useSeparateThread = true; // set to false for debugging, if you want all operations to be sequential.
        static volatile BackgroundThreadManager instance = null;
        public static readonly object padlock = new object();
        static Thread myThread = null;
        static R8RBackgroundTask theTask = null;
        static DateTime BriefPauseRequestedTime;
        public static int BriefPauseRequestNumberSeconds = 10;
        static bool _briefPauseRequested;
        internal static int numberPauseRequests = 0;
        public static bool PauseRequestedImmediately { get; set; }
        public static bool PauseRequestedWhenWorkIsComplete { get; set; }
        public static bool CurrentlyPaused { get; set; }

        internal static bool BriefPauseRequested
        {
            get
            {
                if (BriefPauseRequestedTime < TestableDateTime.Now - TimeSpan.FromSeconds(BriefPauseRequestNumberSeconds))
                    _briefPauseRequested = false;
                return _briefPauseRequested;
            }
            
            set
            {
                if (value == true)
                {
                    BriefPauseRequestedTime = TestableDateTime.Now;
                    if (!_briefPauseRequested)
                        _briefPauseRequested = true;
                }
            }
        }

        BackgroundThreadManager()
        {
            _briefPauseRequested = false;
        }

        public static bool IsBriefPauseRequested()
        {
            // Trace.TraceInformation("Pause requested: " + PauseRequested);
            return BriefPauseRequested;
        }

        public void RequestBriefPause()
        {
            BriefPauseRequested = true;
            //try
            //{
            //    PauseRequested = true; // We've requested a pause -- wait for the main background task to recognize this and to pause
            //    Monitor.TryEnter(padlock, TimeSpan.FromMilliseconds(10000));
            //    Monitor.Exit(padlock);
            //    EnsureBackgroundTaskIsRunning();
            //}
            //catch
            //{
            //}
        }

        public void RequestBriefPauseAndWaitForPauseToBegin()
        {
            EnsureBackgroundTaskIsRunning(true);
            RequestBriefPause();
            while (theTask != null && !theTask.CurrentlyInBriefPause)
            {
                Thread.Sleep(25);
                EnsureBackgroundTaskIsRunning(true);
            }
        }

        public void RequestPauseAndWaitForPauseToBegin()
        {
            PauseRequestedWhenWorkIsComplete = true;
            EnsureBackgroundTaskIsRunning(true);
            while (theTask != null && !CurrentlyPaused)
            {
                Thread.Sleep(1);
                EnsureBackgroundTaskIsRunning(true);
            }
        }

        public bool IsBackgroundTaskBusy()
        {
            if (theTask == null)
                return false;
            else
                return theTask.MoreWorkToDo;
        }

        public long? LoopSetsCompleted()
        {
            if (theTask == null)
                return null;
            else
                return theTask.LoopSetCompletedCount;
        }

        public static bool RunBackgroundTaskFromWebRole = false;

        public void EnsureBackgroundTaskIsRunning(bool repeatIndefinitely)
        {
            //Trace.TraceInformation("Entering EnsureBackgroundTaskIsRunning");
            //if (!Monitor.TryEnter(padlock, TimeSpan.FromMilliseconds(1)))
            //{
            //    return true; // Seems busy; must be more work to do
            //}
            //try
            //{
            if (useSeparateThread)
            {
                System.Threading.ThreadState threadState = System.Threading.ThreadState.Unstarted;
                if (myThread != null)
                    threadState = myThread.ThreadState;
                if (myThread == null || (threadState != System.Threading.ThreadState.Running && threadState != System.Threading.ThreadState.WaitSleepJoin))
                {
                    Trace.TraceInformation("About to reset thread, which was in state " + ((myThread == null) ? "null" : threadState.ToString()));
                    ResetThread(repeatIndefinitely);
                }
                //else
                //    Trace.TraceInformation("Background task already running. Will not reset.");
            }
            else
            {
                //Trace.TraceInformation("Using integrated idle tasks.");
                theTask = new R8RBackgroundTask();
                R8RDataManipulation theDataAccessModule = new R8RDataManipulation();
                theTask.BackgroundTasksRunner(theDataAccessModule);
            }
            //}
            //finally
            //{
            //    Monitor.Exit(padlock);
            //}
            //Trace.TraceInformation("Exiting EnsureBackgroundTaskIsRunning");
        }

        internal void ResetThread(bool repeatIndefinitely)
        {
            try
            {
                if (myThread != null)
                    myThread.Abort();
                theTask = new R8RBackgroundTask();
                theTask.RepeatIndefinitely = repeatIndefinitely;
                myThread = new Thread(theTask.Run);
                myThread.Name = "R8R " + TestableDateTime.Now.ToString();
                myThread.Start();
            }
            catch
            {
            }
        }

        public void StopThread()
        {
            if (myThread != null)
                myThread.Abort();
        }


        //public Thread GetThread()
        //{
        //    //lock (padlock)
        //    //{
        //        return myThread;
        //    //}
        //}


        public static BackgroundThreadManager Instance
        {
            get
            {
                //lock (padlock)
                //{
                    if (instance == null)
                    {
                        instance = new BackgroundThreadManager();
                    }
                    return instance;
                //}
            }
        }
    }

}