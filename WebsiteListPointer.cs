namespace WindowsFormsApp1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WebsiteListPointer")]
    public partial class WebsiteListPointer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WebsiteListId { get; set; }

        public virtual WebsiteList WebsiteList { get; set; }
    }
}
