using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

//Add-Migration <migration name> -Context SepesDbContext -StartupProject Sepes.RestApi -Project Sepes.Infrastructure

namespace Sepes.Infrastructure.Model.Context
{
    public class SepesDbContext : DbContext
    {
        public SepesDbContext(DbContextOptions<SepesDbContext> options) : base(options) { }

        public virtual DbSet<Study> Studies { get; set; }

        public virtual DbSet<SandBox> SandBoxes { get; set; }

        public virtual DbSet<DataSet> DataSets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Study>()
                .Property(b => b.Created)
                .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<Study>()
              .Property(b => b.Updated)
              .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<DataSet>()
            .Property(b => b.Created)
            .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<DataSet>()
              .Property(b => b.Updated)
              .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<SandBox>()
               .Property(b => b.Created)
               .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<SandBox>()
              .Property(b => b.Updated)
              .HasDefaultValueSql("getutcdate()");
        }

    }


}
