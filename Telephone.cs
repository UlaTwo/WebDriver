namespace WindowsFormsApp1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Telephone")]
    public partial class Telephone
    {
        public int TelephoneId { get; set; }

        [StringLength(200)]
        public string Number { get; set; }

        public int WebsiteId { get; set; }

        public virtual Website Website { get; set; }
    }
}
