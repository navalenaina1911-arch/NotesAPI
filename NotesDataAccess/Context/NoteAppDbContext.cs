using Microsoft.EntityFrameworkCore;
using NotesDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesDataAccess.Context
{
    public class NoteAppDbContext : DbContext
    {
        public NoteAppDbContext(DbContextOptions<NoteAppDbContext> options)
            : base(options) { }

        public DbSet<Note> Notes => Set<Note>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Note>(entity =>
            {
                entity.Property(n => n.Title)
                 .HasMaxLength(50)
                 .IsRequired();


                entity.Property(n => n.Content)
                 .HasMaxLength(500);

                entity.Property(n => n.UpdatedBy)
               .HasMaxLength(50);

                entity.Property(n => n.CreatedBy)
               .HasMaxLength(50);

                entity.Property(n => n.CreatedAt)
                    .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                entity.Property(n => n.UpdatedAt)
                    .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                entity.Property(n => n.IsDeleted)
                    .HasDefaultValue(false);

                modelBuilder.Entity<Note>()
                    .HasQueryFilter(n => !n.IsDeleted);

                modelBuilder.Entity<Note>()
                            .HasGeneratedTsVectorColumn(
                            n => n.SearchVector,
                            "english",
                            n => new { n.Title, n.Content })
                            .HasIndex(n => n.SearchVector)
                            .HasMethod("GIN");
            });
        }
    }
}
