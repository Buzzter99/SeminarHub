﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SeminarHub.Data
{
    public class SeminarHubDbContext : IdentityDbContext
    {
        public SeminarHubDbContext(DbContextOptions<SeminarHubDbContext> options)
            : base(options)
        {
        }
        public DbSet<Seminar> Seminars { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SeminarParticipant> SeminarsParticipants { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SeminarParticipant>().HasKey(x => new
            {
                x.ParticipantId,x.SeminarId
            });
            builder.Entity<SeminarParticipant>()
                .HasOne(e => e.Seminar)
                .WithMany(x => x.SeminarParticipants)
                .OnDelete(DeleteBehavior.Restrict);
            builder
               .Entity<Category>()
               .HasData(new Category()
               {
                   Id = 1,
                   Name = "Technology & Innovation"
               },
               new Category()
               {
                   Id = 2,
                   Name = "Business & Entrepreneurship"
               },
               new Category()
               {
                   Id = 3,
                   Name = "Science & Research"
               },
               new Category()
               {
                   Id = 4,
                   Name = "Arts & Culture"
               });
            base.OnModelCreating(builder);
        }
    }
}