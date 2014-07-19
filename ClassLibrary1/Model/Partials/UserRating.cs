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
using System.Diagnostics;
////using PredRatings;
using MoreStrings;

using System.Threading;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

namespace ClassLibrary1.EFModel
{
    public partial class UserRating
    {
        public bool Resolved { get { return this.LongTermResolutionReflected; } }

        public bool ResolvedShortTerm { get { return Resolved || this.ShortTermResolutionReflected; } }

        public decimal? ShortTermResolutionValueOrLastTrustedValueIfNotResolved
        {
            get
            {
                if (ResolvedShortTerm || Resolved)
                    return ShortTermResolutionValue;
                return this.Rating.LastTrustedValue;
            }
        }

        public decimal? ShortTermResolutionValue
        {
            get
            {
                if (!ResolvedShortTerm)
                    return null;
                if (!Resolved)
                    return this.RatingPhaseStatus.ShortTermResolutionValue;
                if (this.Rating.RatingGroup.ResolutionTime < this.RatingPhaseStatus.RatingGroupPhaseStatus.ShortTermResolveTime)
                    return LongTermResolutionValue;
                else
                    return this.RatingPhaseStatus.ShortTermResolutionValue;
            }
        }

        public decimal? LongTermResolutionValueOrLastTrustedValueIfNotResolved
        {
            get
            {
                if (Resolved && this.Rating.RatingGroup.ResolutionTime != null && this.Rating.RatingGroup.ResolutionTime < this.UserRatingGroup.WhenCreated)
                    return null;
                return this.Rating.LastTrustedValue;
            }
        }

        public decimal? LongTermResolutionValue
        {
            get
            {
                if (!Resolved)
                    return null;
                if (this.Rating.RatingGroup.ResolutionTime < this.UserRatingGroup.WhenCreated)
                    return null;
                return this.Rating.LastTrustedValue;
            }
        }

        public decimal PointsEarnedShortTerm { get { return GetPointsOrMaxLoss(true, false, false, false, true); } }

        public decimal PointsEarnedLongTerm { get { return GetPointsOrMaxLoss(false, true, false, false, true); } }

        public decimal PointsEarned { get { return GetPointsOrMaxLoss(true, true, false, false, true); } }

        public decimal PendingPointsShortTerm { get { return GetPointsOrMaxLoss(true, false, false, true, false); } }

        public decimal PendingPointsLongTerm { get { return GetPointsOrMaxLoss(false, true, false, true, false); } }

        public decimal PendingPoints { get { return GetPointsOrMaxLoss(true, true, false, true, false); } }

        public decimal PendingOrEarnedPointsShortTerm { get { return GetPointsOrMaxLoss(true, false, false, true, true); } }

        public decimal PendingOrEarnedPointsLongTerm { get { return GetPointsOrMaxLoss(false, true, false, true, true); } }

        public decimal PendingOrEarnedPoints { get { return GetPointsOrMaxLoss(true, true, false, true, true); } }

        public decimal NotYetPendingPointsShortTerm { get { return GetPointsOrMaxLoss(true, false, true, false, false); } }

        public decimal NotYetPendingPointsLongTerm { get { return GetPointsOrMaxLoss(false, true, true, false, false); } }

        public decimal NotYetPendingPoints { get { return GetPointsOrMaxLoss(true, true, true, false, false); } }

        public decimal PendingMaxLossShortTerm { get { return GetPointsOrMaxLoss(true, false, false, true, false, true); } }

        public decimal PendingMaxLossLongTerm { get { return GetPointsOrMaxLoss(false, true, false, true, false, true); } }

        public decimal PendingMaxLoss { get { return GetPointsOrMaxLoss(true, true, false, true, false, true); } }

        public decimal NotYetPendingMaxLossShortTerm { get { return GetPointsOrMaxLoss(true, false, true, false, false, true); } }

        public decimal NotYetPendingMaxLossLongTerm { get { return GetPointsOrMaxLoss(false, true, true, false, false, true); } }

        public decimal NotYetPendingMaxLoss { get { return GetPointsOrMaxLoss(true, true, true, false, false, true); } }
        public decimal PotentialMaxLossShortTerm { get { return GetPointsOrMaxLoss(true, false, true, true, false, true); } }

        public decimal PotentialMaxLossLongTerm { get { return GetPointsOrMaxLoss(false, true, true, true, false, true); } }

        public decimal PotentialMaxLoss { get { return GetPointsOrMaxLoss(true, true, true, true, false, true); } }

        public decimal MaxLossShortTerm { get { return MaxLoss * (1 - LongTermPointsWeight); } }

        public decimal MaxLossLongTerm { get { return MaxLoss * LongTermPointsWeight; } }

        public decimal PointsOrPendingPointsLongTermUnweighted { get { if (!PointsHaveBecomePending) return 0; return PotentialPointsLongTermUnweighted; } }

        public decimal GetPointsOrMaxLoss(bool includeShortTerm, bool includeLongTerm, bool includeNotYetPending, bool includePending, bool includeEarned, bool getMaxLoss = false)
        {
            decimal total = 0;
            if (includeShortTerm && (
                (includeNotYetPending && !this.PointsHaveBecomePending && !this.ResolvedShortTerm) ||
                (includePending && this.PointsHaveBecomePending && !this.ResolvedShortTerm) ||
                (includeEarned && this.ResolvedShortTerm)
                ))
            {
                if (getMaxLoss)
                    total += this.MaxLossShortTerm;
                else
                    total += this.PotentialPointsShortTerm;
            }

            if (includeLongTerm && (
                (includeNotYetPending && !this.PointsHaveBecomePending && !this.ResolvedShortTerm) ||
                (includePending && this.PointsHaveBecomePending && !this.Resolved) ||
                (includeEarned && this.Resolved)
                ))
            {
                if (getMaxLoss)
                    total += this.MaxLossLongTerm;
                else
                    total += this.PotentialPointsLongTerm;
            }
            return total;
        }
    }
}
