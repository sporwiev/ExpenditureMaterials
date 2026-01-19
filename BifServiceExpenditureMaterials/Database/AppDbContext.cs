using System.Diagnostics;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Other = BifServiceExpenditureMaterials.Models.Other;


namespace BifServiceExpenditureMaterials.Database
{
    public class AppDbContext : DbContext
    {
        public static string subip = "123";
        public static string ip = "172.16.0.";
        public DbSet<machine>? machine { get; set; }
        public DbSet<Oil>? Oil { get; set; }
        public DbSet<Antifreeze>? Antifreeze { get; set; }
        public DbSet<Grease>? Grease { get; set; }
        public DbSet<Filter>? Filter { get; set; }
        public DbSet<Other>? Other { get; set; }
        public DbSet<DataEditing> dataediting { get; set; }
        public DbSet<Motors>? Motors { get; set; }
        public DbSet<CountMaterials>? CountMaterials { get; set; }
        public DbSet<Material>? Materials { get; set; }
        public DbSet<Antifreezebrandfields> antifreezebrandfields { get; set; }
        public DbSet<Antifreezecolorfields> antifreezecolorfields { get; set; }
        public DbSet<Antifreezetypefields> antifreezetypefields { get; set; }
        public DbSet<Greasebrandfields> greasebrandfields { get; set; }
        public DbSet<Greasetypefields> greasetypefields { get; set; }
        public DbSet<Greasevilocityfields> greasevilocityfields { get; set; }
        public DbSet<Oilbrandfields> oilbrandfields { get; set; }
        public DbSet<Oiltypefields> oiltypefields { get; set; }
        public DbSet<Oilvilocityfields> oilvilocityfields { get; set; }
        public DbSet<OilCount> oilcount {  get; set; }
        public DbSet<HistoryCountOil> historycountoil { get; set; }
        public DbSet<HistoryCountAntifreeze> historycountantifreeze { get; set; }
        public DbSet<HistoryCountGrease> historycountgrease { get; set; }
        public DbSet<HistoryCountFilters> historycountfilters { get; set; }
        public DbSet<GreaseCount> greasecount { get; set; }
        public DbSet<AntifreezeCount> antifreezecount { get; set; }
        public DbSet<FiltersCount> filterscount { get; set; }


        public DbSet<Files> files {  get; set; }

        public DbSet<PatternMachine> patternMachines { get; set; }

        public DbSet<Subunit> subunit { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                   
                    //    _connectionString = ConfigureWithIpScanning().GetAwaiter().GetResult();
                    //string connStr = $"server={Networks.GetIp()};port=3306;database=expenditurematerials;user=Anton;password=kruglovcss123;";
                optionsBuilder.EnableSensitiveDataLogging()
        .EnableDetailedErrors()
        .LogTo(new Action<string>((action) =>
        {
            Debug.WriteLine(action);
        }), LogLevel.Information)
        .UseSqlite("Data Source=../../../Database/expenditurematerials.sqlite").UseSqliteLolita();
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Material>()
           .HasOne(m => m.Oil).WithMany().HasForeignKey(m => m.oil_id);
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Filter).WithMany().HasForeignKey(m => m.filter_id);
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Grease).WithMany().HasForeignKey(m => m.grease_id);
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Other).WithMany().HasForeignKey(m => m.other_id);
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Antifreeze).WithMany().HasForeignKey(m => m.antifreeze_id);
            modelBuilder.Entity<Material>()
               .HasOne(m => m.CountMaterials).WithMany().HasForeignKey(m => m.countmaterial_id);
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Motor).WithMany().HasForeignKey(m => m.motor_id);
            modelBuilder.Entity<OilCount>()
                .HasOne(m => m.Oil).WithMany().HasForeignKey(m => m.id_oil);
            modelBuilder.Entity<GreaseCount>()
                .HasOne(m => m.Grease).WithMany().HasForeignKey(m => m.id_grease);
            modelBuilder.Entity<AntifreezeCount>()
                .HasOne(m => m.Antifreeze).WithMany().HasForeignKey(m => m.id_antifreeze);
            modelBuilder.Entity<HistoryCountAntifreeze>()
                .HasOne(m => m.Antifreeze).WithMany().HasForeignKey(m => m.antifreeze_id);
            modelBuilder.Entity<HistoryCountFilters>()
                .HasOne(m => m.Filter).WithMany().HasForeignKey(m => m.filter_id);
            modelBuilder.Entity<HistoryCountGrease>()
                .HasOne(m => m.Grease).WithMany().HasForeignKey(m => m.grease_id);
            modelBuilder.Entity<HistoryCountOil>()
                .HasOne(m => m.Oil).WithMany().HasForeignKey(m => m.oil_id);
            modelBuilder.Entity<OilCount>()
                .HasOne(m => m.Oil).WithMany().HasForeignKey(m => m.id_oil);
            modelBuilder.Entity<AntifreezeCount>()
                .HasOne(m => m.Antifreeze).WithMany().HasForeignKey(m => m.id_antifreeze);
            modelBuilder.Entity<GreaseCount>()
                .HasOne(m => m.Grease).WithMany().HasForeignKey(m => m.id_grease);
            modelBuilder.Entity<FiltersCount>()
                .HasOne(m => m.Filter).WithMany().HasForeignKey(m => m.id_filter);
            modelBuilder.Entity<CountMaterials>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

        }
    }
    
}
