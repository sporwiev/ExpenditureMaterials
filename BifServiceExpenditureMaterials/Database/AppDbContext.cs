using System.Diagnostics;
using BifServiceExpenditureMaterials.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Other = BifServiceExpenditureMaterials.Models.Other;

namespace BifServiceExpenditureMaterials.Database
{
    /// <summary>
    /// Контекст базы данных приложения (SQLite через Entity Framework Core).
    /// Содержит все DbSet-коллекции и настройки связей между моделями.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>Последний октет IP-адреса сервера для подключения.</summary>
        public static string SubIp = "123";

        /// <summary>Базовый IP-адрес подсети (первые три октета).</summary>
        public static string Ip = "172.16.0.";

        // ─── Справочники (простые таблицы) ───────────────────────────────────────

        public DbSet<machine>?    machine    { get; set; }
        public DbSet<Oil>?        Oil        { get; set; }
        public DbSet<Antifreeze>? Antifreeze { get; set; }
        public DbSet<Grease>?     Grease     { get; set; }
        public DbSet<Filter>?     Filter     { get; set; }
        public DbSet<Other>?      Other      { get; set; }
        public DbSet<Motors>?     Motors     { get; set; }
        public DbSet<Subunit>     subunit    { get; set; }

        // ─── Основные данные ──────────────────────────────────────────────────────

        public DbSet<Material>?       Materials       { get; set; }
        public DbSet<CountMaterials>? CountMaterials  { get; set; }
        public DbSet<DataEditing>     dataediting     { get; set; }
        public DbSet<Files>           files           { get; set; }
        public DbSet<PatternMachine>  patternMachines { get; set; }

        // ─── Характеристики масел ─────────────────────────────────────────────────

        public DbSet<Oilbrandfields>    oilbrandfields    { get; set; }
        public DbSet<Oiltypefields>     oiltypefields     { get; set; }
        public DbSet<Oilvilocityfields> oilvilocityfields { get; set; }

        // ─── Характеристики смазок ────────────────────────────────────────────────

        public DbSet<Greasebrandfields>    greasebrandfields    { get; set; }
        public DbSet<Greasetypefields>     greasetypefields     { get; set; }
        public DbSet<Greasevilocityfields> greasevilocityfields { get; set; }

        // ─── Характеристики антифризов ────────────────────────────────────────────

        public DbSet<Antifreezebrandfields> antifreezebrandfields { get; set; }
        public DbSet<Antifreezecolorfields> antifreezecolorfields { get; set; }
        public DbSet<Antifreezetypefields>  antifreezetypefields  { get; set; }

        // ─── Остатки на складе ────────────────────────────────────────────────────

        public DbSet<OilCount>        oilcount        { get; set; }
        public DbSet<GreaseCount>     greasecount     { get; set; }
        public DbSet<AntifreezeCount> antifreezecount { get; set; }
        public DbSet<FiltersCount>    filterscount    { get; set; }

        // ─── История списаний ─────────────────────────────────────────────────────

        public DbSet<HistoryCountOil>        historycountoil        { get; set; }
        public DbSet<HistoryCountAntifreeze> historycountantifreeze { get; set; }
        public DbSet<HistoryCountGrease>     historycountgrease     { get; set; }
        public DbSet<HistoryCountFilters>    historycountfilters    { get; set; }

        // ─── Конфигурация подключения ─────────────────────────────────────────────

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            optionsBuilder
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                // LogLevel.Warning — менее шумно, чем Information
                .LogTo(msg => Debug.WriteLine(msg), LogLevel.Warning)
                .UseSqlite("Data Source=../../../Database/expenditurematerials.sqlite")
                .UseSqliteLolita();
        }

        // ─── Настройка связей между моделями ─────────────────────────────────────

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Связи Material с зависимыми справочниками
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

            // Связи остатков на складе
            modelBuilder.Entity<OilCount>()
                .HasOne(m => m.Oil).WithMany().HasForeignKey(m => m.id_oil);

            modelBuilder.Entity<GreaseCount>()
                .HasOne(m => m.Grease).WithMany().HasForeignKey(m => m.id_grease);

            modelBuilder.Entity<AntifreezeCount>()
                .HasOne(m => m.Antifreeze).WithMany().HasForeignKey(m => m.id_antifreeze);

            modelBuilder.Entity<FiltersCount>()
                .HasOne(m => m.Filter).WithMany().HasForeignKey(m => m.id_filter);

            // Связи таблиц истории списаний
            modelBuilder.Entity<HistoryCountOil>()
                .HasOne(m => m.Oil).WithMany().HasForeignKey(m => m.oil_id);

            modelBuilder.Entity<HistoryCountAntifreeze>()
                .HasOne(m => m.Antifreeze).WithMany().HasForeignKey(m => m.antifreeze_id);

            modelBuilder.Entity<HistoryCountGrease>()
                .HasOne(m => m.Grease).WithMany().HasForeignKey(m => m.grease_id);

            modelBuilder.Entity<HistoryCountFilters>()
                .HasOne(m => m.Filter).WithMany().HasForeignKey(m => m.filter_id);

            // Идентификатор CountMaterials генерируется автоматически БД
            modelBuilder.Entity<CountMaterials>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
