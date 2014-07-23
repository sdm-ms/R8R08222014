using System;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Web.DataAccess;
using System.Configuration;
using System.Data.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Data.Linq.Mapping;
////using PredRatings;
using MoreStrings;

using System.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Model
{
    public class RoutineMaintenanceException : Exception
    {
        public RoutineMaintenanceException(string message)
            : base(message)
        {
        }
    }

    public static class DatabaseAndAzureRoleStatus
    {
        public static DatabaseStatus GetStatus(IR8RDataContext theDataContext)
        {
            /* We've disabled caching, because SubmitChanges doesn't work properly on a cached object. */
            var theStatus = theDataContext.GetTable<DatabaseStatus>().Where(x => true).OrderBy(x => x.DatabaseStatusID).FirstOrDefault();
            return theStatus;
        }

        // This method should ordinarily return false. However, it should return true if 
        // we want to be able to run the background process from the development machine
        // and temporarily disable background processing on the deployed machine.
        public static bool ForceBackgroundTaskToProcess()
        {
            return false; // Note: setting to true doesn't work right currently
        }

        public static bool ForceBackgroundTaskNotToProcess()
        {
            return false;
        }

        public static void KillExistingBackgroundProcess(IR8RDataContext theDataContext)
        {
            if (!RoleEnvironment.IsAvailable)
                return;
            theDataContext.SubmitChanges();
            var existingRoles = theDataContext.GetTable<RoleStatus>().Where(x => x.RoleID != RoleEnvironment.CurrentRoleInstance.Id).ToList();
            // delete our record of stale roles
            if (existingRoles.Any())
            {
                theDataContext.GetTable<RoleStatus>().DeleteAllOnSubmit(existingRoles);
                theDataContext.SubmitChanges();
                System.Threading.Thread.Sleep(30000);
            }
        }

        static bool temporarilyAllowChanges = false; // change briefly when calling SetPreventChanges.
        public static bool ShouldPreventChanges(IR8RDataContext theDataContext)
        {
            if (!RoleEnvironment.IsAvailable)
                return false;
            if (temporarilyAllowChanges)
                return false;
            DatabaseStatus theStatus = GetStatus(theDataContext);
            if (theStatus == null)
                return false;
            return theStatus.PreventChanges;
        }

        public static void CheckPreventChanges(IR8RDataContext theDataContext)
        {
            if (ShouldPreventChanges(theDataContext))
                throw new RoutineMaintenanceException("Sorry, the database is currently undergoing routine maintenance. Please try again later.");
        }

        public static void SetPreventChanges(IR8RDataContext theDataContext, bool doPrevent)
        {
            temporarilyAllowChanges = true; // otherwise, we won't be able to submit this change!
            DatabaseStatus theStatus = GetStatus(theDataContext);
            if (theStatus == null)
            {
                theStatus = new DatabaseStatus { DatabaseStatusID = Guid.NewGuid(), PreventChanges = doPrevent };
                theDataContext.GetTable<DatabaseStatus>().InsertOnSubmit(theStatus);
            }
            theStatus.PreventChanges = doPrevent;
            theDataContext.SubmitChanges();
            temporarilyAllowChanges = false;
        }

        public static List<RoleStatus> UpdateRoleStatusInfo(IR8RDataContext theDataContext)
        {
            if (!RoleEnvironment.IsAvailable)
                return new List<RoleStatus>();
            var existingRoles = theDataContext.GetTable<RoleStatus>().ToList();
            // delete our record of stale roles
            var staleRoles = existingRoles.Where(x => x.LastCheckIn < TestableDateTime.Now - maximumTimeForBackgroundTasks || x.LastCheckIn > TestableDateTime.Now + maximumTimeForBackgroundTasks); // also count as stale any role that mistakenly has a check in in the future (for example, if the system clock has gone backwards)
            if (DatabaseAndAzureRoleStatus.ForceBackgroundTaskToProcess())
            {
                foreach (var role in existingRoles)
                {
                    if (role.RoleID == RoleEnvironment.CurrentRoleInstance.Id)
                        role.IsBackgroundProcessing = true;
                    else
                        role.IsBackgroundProcessing = false;
                }
                theDataContext.SubmitChanges();
                return existingRoles;
            }
            List<RoleStatus> remainingRoles = existingRoles;
            if (staleRoles.Any())
            {
                theDataContext.GetTable<RoleStatus>().DeleteAllOnSubmit(staleRoles);
                theDataContext.SubmitChanges();
                remainingRoles = theDataContext.GetTable<RoleStatus>().ToList();
            }
            // if we don't have a properly designated role for background processing, designate one
            if (!remainingRoles.Any(x => x.IsBackgroundProcessing)
                || (remainingRoles.Where(x => x.IsBackgroundProcessing).Count() > 1)
                || (remainingRoles.Any(x => x.IsWorkerRole && !x.IsBackgroundProcessing)))
            {
                bool turningProcessingOff = false;
                foreach (var remainingRole in remainingRoles)
                {
                    if (remainingRole.IsBackgroundProcessing)
                    {
                        turningProcessingOff = true;
                        remainingRole.IsBackgroundProcessing = false;
                    }
                }
                var theWorkerRole = remainingRoles.FirstOrDefault(x => x.IsWorkerRole);
                if (theWorkerRole != null)
                    theWorkerRole.IsBackgroundProcessing = true;
                else
                {
                    var selectedWebRole = remainingRoles.OrderBy(x => x.WhenCreated).FirstOrDefault(); 
                    if (selectedWebRole != null)
                        selectedWebRole.IsBackgroundProcessing = true;
                }
                theDataContext.SubmitChanges();
                remainingRoles = theDataContext.GetTable<RoleStatus>().ToList();
                if (turningProcessingOff)
                    disableProcessingUntil = TestableDateTime.Now + maximumTimeForBackgroundTasks;
            }
            return remainingRoles;
        }

        public static bool CurrentRoleIsWorkerRole()
        {
            if (!RoleEnvironment.IsAvailable)
                return true;
            return RoleEnvironment.CurrentRoleInstance.Role.Name == workerRoleName;
        }

        //static bool? currentRoleIsBackgroundProcessing;
        //static DateTime lastCurrentRoleIsBackgroundProcessingCheck;
        public static bool CurrentRoleIsBackgroundProcessing()
        {
            return (performingBackgroundWork == true);
            //if (currentRoleIsBackgroundProcessing != null && lastCurrentRoleIsBackgroundProcessingCheck > TestableDateTime.Now - new TimeSpan(0, 1, 0))
            //    return (bool)currentRoleIsBackgroundProcessing;
            //IR8RDataContext theDataContext = GetIR8RDataContext.New(false, false); // make this read-only so that MSDTC is not required if there is already an open data context
            //RoleStatus matchingRole = theDataContext.GetTable<RoleStatus>().FirstOrDefault(x => x.RoleID == RoleEnvironment.CurrentRoleInstance.Id);
            //if (matchingRole == null)
            //    currentRoleIsBackgroundProcessing = false;
            //else
            //    currentRoleIsBackgroundProcessing = matchingRole.IsBackgroundProcessing;
            //lastCurrentRoleIsBackgroundProcessingCheck = TestableDateTime.Now;
            //return (bool) currentRoleIsBackgroundProcessing;
        }

        public static RoleStatus CheckInRole(IR8RDataContext theDataContext)
        {
            if (!RoleEnvironment.IsAvailable)
                return null;
            var existingRoles = UpdateRoleStatusInfo(theDataContext);
            RoleStatus matchingRole = existingRoles.FirstOrDefault(x => x.RoleID == RoleEnvironment.CurrentRoleInstance.Id);
            if (matchingRole == null)
            {
                RoleStatus newStatus = new RoleStatus
                {
                    RoleStatusID = Guid.NewGuid(),
                    IsBackgroundProcessing = false,
                    IsWorkerRole = CurrentRoleIsWorkerRole(),
                    LastCheckIn = TestableDateTime.Now,
                    RoleID = RoleEnvironment.CurrentRoleInstance.Id,
                    WhenCreated = TestableDateTime.Now
                };
                theDataContext.GetTable<RoleStatus>().InsertOnSubmit(newStatus);
                theDataContext.SubmitChanges();
                return newStatus;
            }
            else
            {
                matchingRole.LastCheckIn = TestableDateTime.Now;
                theDataContext.SubmitChanges();
                return matchingRole;
            }
        }

        static DateTime? disableProcessingUntil = null;
        //static DateTime? firstCheckTime = null;
        //static DateTime? mostRecentCheckTime = null;
        //static bool? isWorkerRole = null;
        //static RoleInstance currentRole = null;
        static string workerRoleName = "WorkerRole1";
        // static string webRoleName = "WebRole1";
        static bool? performingBackgroundWork = null;
        static TimeSpan maximumTimeForBackgroundTasks = TimeSpan.FromMinutes((double)2);
        public static bool PerformBackgroundProcess(IR8RDataContext theDataContext)
        {
            if (ForceBackgroundTaskNotToProcess())
                return false;

            if (!RoleEnvironment.IsAvailable)
                return true; // we're unit testing

            DatabaseStatus theStatus = GetStatus(theDataContext);
            if (theStatus == null) /* should occur only when rebuilding the database */
                return false;
            if (theStatus.PreventChanges)
                return false;

            performingBackgroundWork = ShouldPerformBackgroundProcess(theDataContext);

            //Trace.TraceInformation("Perform background process for " + RoleEnvironment.CurrentRoleInstance.Id + ": " + performingBackgroundWork);

            if (ForceBackgroundTaskToProcess())
                return true;

            return (bool)performingBackgroundWork;
        }


        private static bool ShouldPerformBackgroundProcess(IR8RDataContext theDataContext)
        {
            RoleStatus thisRole = CheckInRole(theDataContext);
            if (disableProcessingUntil != null && TestableDateTime.Now < (DateTime)disableProcessingUntil)
                return false;
            return thisRole.IsBackgroundProcessing;


            //// If this role was not started within last two minutes, return false.
            //if (firstCheckTime == null)
            //    firstCheckTime = TestableDateTime.Now;
            //if (!(TestableDateTime.Now - firstCheckTime > maximumTimeForBackgroundTasks))
            //    return false;

            //if (currentRole == null)
            //    currentRole = RoleEnvironment.CurrentRoleInstance;
            //if (isWorkerRole == null)
            //    isWorkerRole = currentRole.Role.Name == workerRoleName;

            //if ((bool)isWorkerRole)
            //    return true;

            //// This is a web role. Determine whether there is any worker role. If not, see if it is lowest
            //// web role alphabetically. If so, then return true. If not, return false, and don't check again for at
            //// least two minutes.

            //if (performingBackgroundWork == false && !(TestableDateTime.Now - mostRecentCheckTime > maximumTimeForBackgroundTasks))
            //{
            //    if (mostRecentCheckTime == null)
            //        mostRecentCheckTime = TestableDateTime.Now;
            //    return false; /* previously not doing background work and checked within past two minutes */
            //}
            //mostRecentCheckTime = TestableDateTime.Now;
            //var theWebRoles = RoleEnvironment.Roles["WebRole1"];
            //var theWorkerRoles = RoleEnvironment.Roles["WorkerRole1"];
            //if (theWorkerRoles.Instances.Any())
            //    return false;
            //var webRoleInChargeId = theWebRoles.Instances.Select(x => x.Id).OrderBy(x => x).First();
            //return (currentRole.Id == webRoleInChargeId);
        }


    }
}