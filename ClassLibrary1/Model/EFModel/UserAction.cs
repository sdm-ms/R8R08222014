namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserAction
    {
        public UserAction()
        {
            AdministrationRights = new HashSet<AdministrationRight>();
            ProposalEvaluationRatingSettings = new HashSet<ProposalEvaluationRatingSetting>();
            RewardRatingSettings = new HashSet<RewardRatingSetting>();
        }

        public Guid UserActionID { get; set; }

        public string Text { get; set; }

        public bool SuperUser { get; set; }

        public virtual ICollection<AdministrationRight> AdministrationRights { get; set; }

        public virtual ICollection<ProposalEvaluationRatingSetting> ProposalEvaluationRatingSettings { get; set; }

        public virtual ICollection<RewardRatingSetting> RewardRatingSettings { get; set; }
    }
}
