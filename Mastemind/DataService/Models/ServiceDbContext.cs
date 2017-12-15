using System;
using System.Configuration;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DataService.Models
{
    public class ServiceDbContext : DbContext
    {
        public virtual DbSet<Statistic> Statistics { get; set; }

        public ServiceDbContext() : base("name=DBCS")
        {
            //var cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            var xmlPath = $@"{AppDomain.CurrentDomain.BaseDirectory}\config.xml";
            var doc = XDocument.Load(xmlPath);
            var cs = doc.Element("configuration")?.Element("connectionStrings")?.Element("add")?
                .Attribute("connectionString")?.Value;
            var dataSource = Regex.Match(cs, @"Data Source=(.+?);").Groups[1].Value;
            var dataSourceMapped = AppDomain.CurrentDomain.BaseDirectory
                + dataSource.Replace("//", "/").Replace("~/", "").Replace("/", @"\");
            Database.Connection.ConnectionString = cs.Replace(dataSource, dataSourceMapped);
            Database.SetInitializer<ServiceDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Statistics

            modelBuilder.Entity<Statistic>()
                .HasKey(e => e.Id)
                .ToTable("tblStatistics");
        }
    }
}
