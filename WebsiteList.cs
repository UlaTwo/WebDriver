namespace WindowsFormsApp1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WebsiteList")]
    public partial class WebsiteList
    {
        public int WebsiteListId { get; set; }

        [StringLength(2083)]
        public string Url { get; set; }

        public virtual WebsiteListPointer WebsiteListPointer { get; set; }
    }
}
