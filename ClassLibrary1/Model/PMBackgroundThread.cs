﻿using System;
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
    public class MyBackgroundTask
    {
        public bool RepeatIndefinitely { get; set; }
        public bool MoreWorkToDo { get; internal set; }
        public bool CurrentlyInBriefPause { get; internal set; }
        public DateTime? ThisWebServerLastUpdateTime;

        public MyBackgroundTask()
        {
            MoreWorkToDo = true;
            RepeatIndefinitely = false;
            CurrentlyInBriefPause = false;
            BackgroundThread.CurrentlyPaused = false;
        }

        /// <summary>
        /// Perform idle tasks (such as checking for a rating that needs resolution or updating)
        /// </summary>
        public void IdleTasksLoop(RaterooDataManipulation dataManipulation)
        {
            bool performBackgroundProcess = false;
            try
            {
                lock (BackgroundThread.padlock)
                {
                    
                    dataManipulation.ResetDataContexts();
                    performBackgroundProcess = PMDatabaseAndAzureRoleStatus.PerformBackgroundProcess(dataManipulation.DataContext); 
                    if (!performBackgroundProcess)
                    {
                        if (!PMDatabaseAndAzureRoleStatus.CurrentRoleIsWorkerRole())
                            CacheInvalidityNotification.ProcessNewNotifications(); // web roles should do this periodically (regardless of whether it's also performing worker role functions)
                        Thread.Sleep(5000); // wait a bit
                        return;
                    }
                    dataManipulation.ResetDataContexts();

                    //if (BackgroundThread.IsPauseRequested())
                    //Trace.TraceInformation("Pause is requested.");
                    MoreWorkToDo = true; // note that this may be relied on by external components, so until we've gone through all tasks with no more work to do, we must keep this at true
                    const int numTasks = 19;
                    const int numLoops = 20;
                    bool[] moreWorkToDoThisTask = new bool[numTasks];
                    for (int loop = 1; loop <= numLoops; loop++)
                    {
                        if (loop != 1 && BackgroundThread.PauseRequestedWhenWorkIsComplete && !MoreWorkToDo)
                        {
                            BackgroundThread.PauseRequestedWhenWorkIsComplete = false;
                            BackgroundThread.CurrentlyPaused = true;
                        }
                        for (int i = 1; i <= numTasks; i++)
                        {
                            if (BackgroundThread.PauseRequestedImmediately)
                            {
                                BackgroundThread.PauseRequestedImmediately = false;
                                BackgroundThread.CurrentlyPaused = true;
                            }
                            while (BackgroundThread.CurrentlyPaused)
                            {
                                Thread.Sleep(1); // very minimal pause until we check again to see if the pause request has been turned off
                            }
                            if (loop == 1 || loop == numLoops) // we try everything on first loop, as well as on last loop, so moreWorkToDo will be accurate
                                moreWorkToDoThisTask[i - 1] = true;
                            if (!BackgroundThread.IsBriefPauseRequested() && ((i == 1 && moreWorkToDoThisTask.Any(x => x == true)) || moreWorkToDoThisTask[i - 1]))
                            {
                                // Trace.TraceInformation("Task " + i);
                               // ProfileSimple.Start("TaskNum" + i);
                                dataManipulation.ResetDataContexts();
                                switch (i)
                                {

                                    case 1:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.CompleteMultipleAddUserRatings();
                                        break;

                                    case 2:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskImplementResolutions();
                                        break;

                                    case 3:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.FixStatusInconsistencies();
                                        break;

                                    case 4:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskShortTermResolve();
                                        break;

                                    case 5:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskCheckPointsManagers();
                                        break;

                                    case 6:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.RespondToResetTblRowFieldDisplays();
                                        break;

                                    case 7:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.ContinueLongProcess();
                                        break;

                                    case 8:
                                        moreWorkToDoThisTask[i - 1] = GeocodeUpdate.DoUpdate(dataManipulation.DataContext);
                                        break;

                                    case 9:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskUpdatePoints();
                                        break;

                                    case 10:
                                        moreWorkToDoThisTask[i - 1] = PMTrustTrackingBackgroundTasks.DoTrustTrackingBackgroundTasks(dataManipulation.DataContext);
                                        break;

                                    case 11:
                                        moreWorkToDoThisTask[i - 1] = StatusRecords.DeleteOldStatusRecords(dataManipulation.DataContext);
                                        break;

                                    case 12:
                                        moreWorkToDoThisTask[i - 1] = VolatilityTracking.UpdateTrackers(dataManipulation.DataContext);
                                        break;

                                    case 13:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskRevertLongUntrustedRatings();
                                        break;

                                    case 14:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskMakeHighStakesKnown();
                                        break;

                                    case 15:
                                        CacheInvalidityNotification.DeleteOldNotifications();
                                        moreWorkToDoThisTask[i - 1] = false;
                                        break;

                                    case 16:
                                        moreWorkToDoThisTask[i - 1] = SQLFastAccess.ContinueFastAccessMaintenance(dataManipulation.DataContext);
                                        break;

                                    case 17:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.UpdatePointsTrustRulesBackgroundTask();
                                        break;

                                    case 18:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.IdleTaskConsiderDemotingHighStakesPrematurely();
                                        break;

                                    case 19:
                                        moreWorkToDoThisTask[i - 1] = dataManipulation.AdvanceRatingGroupsNeedingAdvancing();
                                        break;

                                }
                                dataManipulation.DataContext.SubmitChanges();
                                dataManipulation.ResetDataContexts();
                                PMDatabaseAndAzureRoleStatus.CheckInRole(dataManipulation.DataContext);
                                dataManipulation.ResetDataContexts();
                                //Trace.TraceInformation("TaskNum " + i);
                               // ProfileSimple.End("TaskNum" + i);

                                MoreWorkToDo = i != numTasks || moreWorkToDoThisTask.Any(x => x == true);
                            }
                        }
                    }
                    if (!MoreWorkToDo)
                    {
                        if (!RoleEnvironment.IsAvailable)
                            Thread.Sleep(100); // sleep only long enough for unit tests to realize that the idle tasks have completed.
                        else
                            Thread.Sleep(3000);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Idle task failed: " + ex.Message);
                // I cant' remember if this reset should happen or not.  At the least I think I should report that there was an error.
                //dataManipulation.ResetDataContexts();
            }
            finally
            {
                if (performBackgroundProcess && !PMDatabaseAndAzureRoleStatus.ShouldPreventChanges(dataManipulation.DataContext))
                    PMDatabaseAndAzureRoleStatus.CheckInRole(dataManipulation.DataContext);
            }
            // Trace.TraceInformation("Exiting IdleTasks moreWorkToDo: " + moreWorkToDo.ToString());
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
            RaterooDataManipulation theDataAccessModule = new RaterooDataManipulation();
            while (TestableDateTime.Now < expireThread || RepeatIndefinitely)
            {
                //Trace.TraceInformation("Background task main run loop.");
                // Trace.TraceInformation("Running background task.");
                while (BackgroundThread.IsBriefPauseRequested())
                {
                    CurrentlyInBriefPause = true;
                    //Trace.TraceInformation("Background task 5 second pause.");
                    Thread.Sleep(5000); // pause
                }

                CurrentlyInBriefPause = false;
                //Trace.TraceInformation("IdleTasksOnce");
                IdleTasksLoop(theDataAccessModule);
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
    /// Summary description for BackgroundThread
    /// </summary>
    public sealed class BackgroundThread
    {
        bool useSeparateThread = true; // set to false for debugging, if you want all operations to be sequential.
        static volatile BackgroundThread instance = null;
        public static readonly object padlock = new object();
        static Thread myThread = null;
        static MyBackgroundTask theTask = null;
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

        BackgroundThread()
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
                    //Trace.TraceInformation("About to reset thread, which was in state " + ((myThread == null) ? "null" : threadState.ToString()));
                    ResetThread(repeatIndefinitely);
                }
                //else
                //    Trace.TraceInformation("Background task already running. Will not reset.");
            }
            else
            {
                //Trace.TraceInformation("Using integrated idle tasks.");
                theTask = new MyBackgroundTask();
                RaterooDataManipulation theDataAccessModule = new RaterooDataManipulation();
                theTask.IdleTasksLoop(theDataAccessModule);
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
                theTask = new MyBackgroundTask();
                theTask.RepeatIndefinitely = repeatIndefinitely;
                myThread = new Thread(theTask.Run);
                myThread.Name = "Rateroo " + TestableDateTime.Now.ToString();
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


        public static BackgroundThread Instance
        {
            get
            {
                //lock (padlock)
                //{
                    if (instance == null)
                    {
                        instance = new BackgroundThread();
                    }
                    return instance;
                //}
            }
        }
    }

}