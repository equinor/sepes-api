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

        public virtual DbSet<StudyDataset> StudyDatasets { get; set; }

        public virtual DbSet<Participant> Participants { get; set; }

        public virtual DbSet<StudyParticipant> StudyParticipants { get; set; }

        public virtual DbSet<SandboxResource> SandboxResources { get; set; }

        public virtual DbSet<SandboxResource> SandboxResourceOperations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddPrimaryKeys(modelBuilder);
            AddDefaultValues(modelBuilder);
            AddRelationships(modelBuilder);
        }

        void AddPrimaryKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Study>().HasKey(s => s.Id);
            modelBuilder.Entity<Dataset>().HasKey(d => d.Id);
            modelBuilder.Entity<Sandbox>().HasKey(s => s.Id);
            modelBuilder.Entity<StudyDataset>().HasKey(sd => new { sd.StudyId, sd.DatasetId });
            modelBuilder.Entity<StudyParticipant>().HasKey(sd => new { sd.StudyId, sd.ParticipantId, sd.RoleName });
            modelBuilder.Entity<SandboxResource>().HasKey(cr => cr.Id);
            modelBuilder.Entity<SandboxResourceOperation>().HasKey(cro => cro.Id);
        }

        void AddRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sandbox>()
                .HasOne(s => s.Study)
                .WithMany(s => s.Sandboxes)
                .HasForeignKey(s => s.StudyId);

            //STUDY / DATASET RELATION
            modelBuilder.Entity<StudyDataset>()
                .HasOne(sd => sd.Study)
                .WithMany(s => s.StudyDatasets)
                .HasForeignKey(sd => sd.StudyId);

            modelBuilder.Entity<StudyDataset>()
                .HasOne(sd => sd.Dataset)
                .WithMany(d => d.StudyDatasets)
                .HasForeignKey(sd => sd.DatasetId);


            //STUDY / PARTICIPANT RELATION
            modelBuilder.Entity<StudyParticipant>()
                .HasOne(sd => sd.Study)
                .WithMany(s => s.StudyParticipants)
                .HasForeignKey(sd => sd.StudyId);

            modelBuilder.Entity<StudyParticipant>()
                .HasOne(sd => sd.Participant)
                .WithMany(d => d.StudyParticipants)
                .HasForeignKey(sd => sd.ParticipantId);


            //CLOUD RESOURCE / SANDBOX RELATION
            modelBuilder.Entity<SandboxResource>()
         .HasOne(cr => cr.Sandbox)
         .WithMany(d => d.Resources)
         .HasForeignKey(sd => sd.SandboxId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SandboxResourceOperation>()
    .HasOne(cr => cr.Resource)
    .WithMany(d => d.Operations)
    .HasForeignKey(sd => sd.SandboxResourceId).OnDelete(DeleteBehavior.Restrict);
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

            modelBuilder.Entity<Participant>()
            .Property(b => b.Created)
            .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<Participant>()
              .Property(b => b.Updated)
              .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<StudyParticipant>()
        .Property(b => b.Created)
        .HasDefaultValueSql("getutcdate()");
        }
    }
}
