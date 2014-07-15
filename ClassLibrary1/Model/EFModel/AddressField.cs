namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AddressField
    {
        public int AddressFieldID { get; set; }

        public int FieldID { get; set; }

        public string AddressString { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public DateTime? LastGeocode { get; set; }

        public byte Status { get; set; }

        public DbGeography Geo { get; set; }

        public virtual Field Field { get; set; }
    }
}
