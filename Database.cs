namespace WindowsFormsApp1
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Database : DbContext
    {
        public Database()
            : base("name=Database")
        {
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Mail> Mail { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Telephone> Telephone { get; set; }
        public virtual DbSet<Website> Website { get; set; }
        public virtual DbSet<WebsiteList> WebsiteList { get; set; }
        public virtual DbSet<WebsiteListPointer> WebsiteListPointer { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebsiteList>()
                .HasOptional(e => e.WebsiteListPointer)
                .WithRequired(e => e.WebsiteList)
                .WillCascadeOnDelete();
        }
    }
}
