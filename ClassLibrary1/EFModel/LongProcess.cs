namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LongProcess
    {
        public int LongProcessID { get; set; }

        public int TypeOfProcess { get; set; }

        public int? Object1ID { get; set; }

        public int? Object2ID { get; set; }

        public int Priority { get; set; }

        public byte[] AdditionalInfo { get; set; }

        public int? ProgressInfo { get; set; }

        public bool Started { get; set; }

        public bool Complete { get; set; }

        public bool ResetWhenComplete { get; set; }

        public int? DelayBeforeRestart { get; set; }

        public DateTime? EarliestRestart { get; set; }
    }
}
