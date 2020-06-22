using Microsoft.EntityFrameworkCore;

//Add-Migration <migration name> -Context SepesDbContext -StartupProject Sepes.RestApi -Project Sepes.Infrastructure

namespace Sepes.Infrastructure.Model.Context
{
    public class SepesDbContext : DbContext
    {
        public SepesDbContext(DbContextOptions<SepesDbContext> options) : base(options) { }

        public virtual DbSet<Study> Studies { get; set; }

        public virtual DbSet<Sandbox> Sandboxes { get; set; }

        public virtual DbSet<Dataset> Datasets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddPrimaryKeys(modelBuilder);
            AddDefaultValues(modelBuilder);
            AddRelationships(modelBuilder);
        }

        void AddPrimaryKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Study>().HasKey(s=> s.Id);
            modelBuilder.Entity<Dataset>().HasKey(s => s.Id);
            modelBuilder.Entity<Sandbox>().HasKey(s => s.Id);

            modelBuilder.Entity<StudyDataset>().HasKey(s => new { s.StudyId, s.DatasetId } );          
        }

        void AddRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sandbox>().HasOne(s => s.Study).WithMany(s => s.Sandboxes).HasForeignKey(s => s.StudyId);

            modelBuilder.Entity<StudyDataset>()
                .HasOne(sd => sd.Study)
                .WithMany(s => s.StudyDatasets)
                .HasForeignKey(pt => pt.StudyId);

            modelBuilder.Entity<StudyDataset>()
                .HasOne(pt => pt.Dataset)
                .WithMany(t => t.StudyDatasets)
                .HasForeignKey(pt => pt.DatasetId);
        }


            void AddDefaultValues(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Study>()
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Sandbox>()
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Dataset>()
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Study>()
            .Property(b => b.Created)
            .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<Study>()
              .Property(b => b.Updated)
              .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<Dataset>()
            .Property(b => b.Created)
            .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<Dataset>()
              .Property(b => b.Updated)
              .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<Sandbox>()
               .Property(b => b.Created)
               .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<Sandbox>()
              .Property(b => b.Updated)
              .HasDefaultValueSql("getutcdate()");
        }
    }
}
