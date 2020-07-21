namespace WindowsFormsApp1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Address")]
    public partial class Address
    {
        public int AddressId { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        public int WebsiteId { get; set; }

        public virtual Website Website { get; set; }
    }
}
