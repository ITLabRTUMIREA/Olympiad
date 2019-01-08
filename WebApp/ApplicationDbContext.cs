﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Models.Exercises;
using Models.Links;
using Models.Solutions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{

    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserToChallenge>()
                .HasKey(utc => new { utc.ChallengeId, utc.UserId });
            builder.Entity<UserToChallenge>()
                .HasOne<Challenge>()
                .WithMany(c => c.UserToChallenges)
                .HasForeignKey(utc => utc.ChallengeId);
            builder.Entity<UserToChallenge>()
                .HasOne<User>()
                .WithMany(u => u.UserToChallenges)
                .HasForeignKey(utc => utc.UserId);
        }
        public DbSet<User> Students { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<UserToChallenge> UserToChallenges { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<ExerciseData> TestData { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
