using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using ClassLibrary1.Misc;
using System.Diagnostics;

namespace TestProject1
{
    class HeterogeneousUserPool
    {
        public TestHelper TestHelper { get; private set; }
        /// <summary>
        /// The number of HeterogeneousUsers in this pool
        /// </summary>
        public int UserCount 
        {
            get
            {
                return HeterogeneousUsers.Count();
            }
        }
        /// <summary>
        /// The percentage of users that are subversive.  Subversive
        /// users purposefully aim for a user rating value that is
        /// not the correct one.
        /// </summary>
        public float PercentageSubversive { get; private set;}

        /// <summary>
        /// UserCount * PercentageSubversive, rounded up.
        /// </summary>
        public int SubversiveUserCount 
        { 
            get 
            { 
                return (int)Math.Ceiling(UserCount * PercentageSubversive); 
            } 
        }

        public int DominantUserCount
        {
            get 
            {
                return UserCount - SubversiveUserCount;
            }
        }

        public Action AfterEachRatingAction { get; private set; }
        public Action AfterEachUserRatingAction { get; private set; }

        public IEnumerable<HeterogeneousUser> HeterogeneousUsers { get; private set; }

        public HeterogeneousUserPool(TestHelper testHelper, double quality, int userRatingEstimateWeight,
            float subversivePercentage, Action afterEachRatingAction = null, Action afterEachUserRatingAction = null)
        {
            TestHelper = testHelper;
            PercentageSubversive = subversivePercentage;
            AfterEachRatingAction = afterEachRatingAction;
            AfterEachUserRatingAction = afterEachUserRatingAction;
            
            var heterogeneousUsers = new List<HeterogeneousUser>();
            int subversiveUserCount = (int)Math.Ceiling(TestHelper.UserIds.Count() * subversivePercentage);
            int dominantUserCount = TestHelper.UserIds.Count() - subversiveUserCount;
            foreach (Guid userId in TestHelper.UserIds)
            {
                HeterogeneousUserType type = heterogeneousUsers.Count < dominantUserCount ?
                    HeterogeneousUserType.Dominant : HeterogeneousUserType.Subversive;
                var heterogeneousUser = new HeterogeneousUser(TestHelper, userId, type, quality, 
                    userRatingEstimateWeight);
                heterogeneousUsers.Add(heterogeneousUser);
            }
            HeterogeneousUsers = heterogeneousUsers;
        }

        public void PrintOutUsersInfo()
        {
            foreach (HeterogeneousUser user in HeterogeneousUsers)
                user.PrintOutInfo();
        }

        /// <summary>
        /// Picks <paramref name="usersRatingEachRatingCount"/> users at random to rate each rating.
        /// </summary>
        /// <param name="correctRatingValue"></param>
        /// <param name="subversiveUserRatingValue"></param>
        /// <param name="tbl"></param>
        /// <param name="usersRatingEachRatingCount"></param>
        public void PerformRatings(decimal correctRatingValue, decimal subversiveUserRatingValue, Tbl tbl,
            int userRatingsPerRating, bool subversiveUserIgnoresPreviousRatings)
        {
            List<Rating> ratings = TestHelper.ActionProcessor.DataContext.GetTable<Rating>()
                .Where(r => r.RatingGroup.TblRow.Tbl.TblID == tbl.TblID).ToList();
            PerformRatings(ratings, correctRatingValue, subversiveUserRatingValue, userRatingsPerRating, subversiveUserIgnoresPreviousRatings);
        }

        /// <summary>
        /// Picks <paramref name="usersRatingEachRatingCount"/> users at random to rate each rating in 
        /// <paramref name="ratings"/>.
        /// </summary>
        /// <param name="correctRatingValue"></param>
        /// <param name="subversiveUserRatingValue"></param>
        /// <param name="tbl"></param>
        /// <param name="usersRatingEachRatingCount"></param>
        public void PerformRatings(List<Rating> ratings,
            decimal correctRatingValue, decimal subversiveUserRatingValue,
            int userRatingsPerRating, bool subversiveUserIgnoresPreviousRatings)
        {
            GC.Collect();
            long memoryInit = GC.GetTotalMemory(false);
            Debug.WriteLine("Pre rating: " + memoryInit);
            int numberUserRatings = 0;
            foreach (Rating rating in ratings)
            {
                var randomUsers = HeterogeneousUsers.OrderBy(u => Guid.NewGuid()).Take(userRatingsPerRating);
                foreach (var user in randomUsers)
                {
                    if (user.Type == HeterogeneousUserType.Dominant)
                        user.Rate(rating, correctRatingValue, subversiveUserIgnoresPreviousRatings);
                    else
                        user.Rate(rating, subversiveUserRatingValue, subversiveUserIgnoresPreviousRatings);
                    numberUserRatings++;
                    GC.Collect(); // DEBUG
                    //Debug.WriteLine("DEBUG1 average usage by rate: " + (((double)(GC.GetTotalMemory(false) - memoryInit)) / ((double)(numberUserRatings))));
                    TestHelper.ActionProcessor.ResetDataContexts(); // DEBUG -- delete this
                    GC.Collect(); // DEBUG
                   // Debug.WriteLine("DEBUG average usage by rate: " + (((double)(GC.GetTotalMemory(false) - memoryInit)) / ((double)(numberUserRatings))));
                }
            }
        }
    }
}
