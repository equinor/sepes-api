using Microsoft.EntityFrameworkCore;

//Use the command below to create a new migration. 
//Replace <migration name> with a good migration name and run this in Package Manager Console
//Add-Migration <migration name> -Context SepesDbContext -StartupProject Sepes.RestApi -Project Sepes.Infrastructure

namespace Sepes.Infrastructure.Model.Context
{    public class SepesDbContext : DbContext
    {
        public SepesDbContext(DbContextOptions<SepesDbContext> options) : base(options) { }

        public virtual DbSet<Study> Studies { get; set; }

        public virtual DbSet<Sandbox> Sandboxes { get; set; }

        public virtual DbSet<Dataset> Datasets { get; set; }

        public virtual DbSet<StudyDataset> StudyDatasets { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<StudyParticipant> StudyParticipants { get; set; }

        public virtual DbSet<SandboxResource> SandboxResources { get; set; }

        public virtual DbSet<SandboxResourceOperation> SandboxResourceOperations { get; set; }

        public virtual DbSet<Variable> Variables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddPrimaryKeys(modelBuilder);
            AddDefaultValues(modelBuilder);
            AddRelationships(modelBuilder);      
        }

        void AddPrimaryKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Variable>().HasKey(v => v.Id);
            modelBuilder.Entity<User>().HasKey(v => v.Id);
            modelBuilder.Entity<Study>().HasKey(s => s.Id);
            modelBuilder.Entity<Dataset>().HasKey(d => d.Id);
            modelBuilder.Entity<Sandbox>().HasKey(s => s.Id);
            modelBuilder.Entity<StudyDataset>().HasKey(sd => new { sd.StudyId, sd.DatasetId });
            modelBuilder.Entity<StudyParticipant>().HasKey(p => new { p.StudyId, p.UserId, p.RoleName });
            modelBuilder.Entity<SandboxResource>().HasKey(r => r.Id);
            modelBuilder.Entity<SandboxResourceOperation>().HasKey(o => o.Id);
        }

        void AddRelationships(ModelBuilder modelBuilder)
        {
            //STUDY / SANDBOX RELATION
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
                .HasForeignKey(sd => sd.StudyId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudyParticipant>()
                .HasOne(sd => sd.User)
                .WithMany(d => d.StudyParticipants)
                .HasForeignKey(sd => sd.UserId).OnDelete(DeleteBehavior.Restrict);


            //CLOUD RESOURCE / SANDBOX RELATION
            modelBuilder.Entity<SandboxResource>()
                .HasOne(cr => cr.Sandbox)
                .WithMany(d => d.Resources)
                .HasForeignKey(sd => sd.SandboxId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SandboxResourceOperation>()
                .HasOne(cr => cr.Resource)
                .WithMany(d => d.Operations)
                .HasForeignKey(sd => sd.SandboxResourceId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SandboxResourceOperation>()
                .HasOne(o => o.DependsOnOperation)
                .WithMany(o => o.DependantOnThisOperation)
             .HasForeignKey(sd => sd.DependsOnOperationId).OnDelete(DeleteBehavior.Restrict);
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

            modelBuilder.Entity<User>()
                .Property(b => b.Created)
                .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<User>()
                .Property(b => b.Updated)
                .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<StudyParticipant>()
                .Property(b => b.Created)
                .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<SandboxResource>()
                .Property(sr => sr.Created)
                .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<SandboxResource>()
                .Property(sr => sr.Updated)
                .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<SandboxResourceOperation>()
                .Property(sro => sro.Created)
                .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<SandboxResourceOperation>()
                .Property(sro => sro.Updated)
                .HasDefaultValueSql("getutcdate()");
        }
    }
}
