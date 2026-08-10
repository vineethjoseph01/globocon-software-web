using System;
using System.Linq;
using GloboconSoftwareWeb.Models;
using GloboconSoftwareWeb.Services;
using Microsoft.EntityFrameworkCore;

namespace GloboconSoftwareWeb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch
            {
                context.Database.EnsureCreated();
            }

            try
            {
                context.Database.ExecuteSqlRaw("IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Videos') AND name = 'IsMalayalam') ALTER TABLE Videos ADD IsMalayalam BIT NOT NULL DEFAULT 0;");
            }
            catch { }

            // Seed Admin User if none exists
            if (!context.AdminUsers.Any())
            {
                var (hash, salt) = PasswordHasher.HashPassword("GloboconAdmin2026!");
                context.AdminUsers.Add(new AdminUser
                {
                    Username = "admin",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    CreatedAt = DateTime.UtcNow
                });
                context.SaveChanges();
            }

            // Seed 4 English Showcase Videos if none exist
            if (!context.Videos.Any(v => !v.IsMalayalam))
            {
                var initialEnglishVideos = new[]
                {
                    new Video
                    {
                        Title = "SmartRoster AI - Intelligent Workforce Rostering Platform",
                        FilePath = "/uploads/videos/work-sample-1.mp4",
                        ThumbnailPath = "/images/globocon-logo.jpg",
                        UploadDate = DateTime.UtcNow.AddDays(-10),
                        DisplayOrder = 1,
                        IsPortrait = true,
                        IsMalayalam = false
                    },
                    new Video
                    {
                        Title = "Virtual Hospital AI - Smarter Healthcare Management Solution",
                        FilePath = "/uploads/videos/work-sample-2.mp4",
                        ThumbnailPath = "/images/globocon-logo.jpg",
                        UploadDate = DateTime.UtcNow.AddDays(-8),
                        DisplayOrder = 2,
                        IsPortrait = true,
                        IsMalayalam = false
                    },
                    new Video
                    {
                        Title = "Smart Tax Booking AI - Automated Accounting Assistance Portal",
                        FilePath = "/uploads/videos/work-sample-3.mp4",
                        ThumbnailPath = "/images/globocon-logo.jpg",
                        UploadDate = DateTime.UtcNow.AddDays(-5),
                        DisplayOrder = 3,
                        IsPortrait = true,
                        IsMalayalam = false
                    },
                    new Video
                    {
                        Title = "SmartAI Agent - Enterprise Autonomous Workflow Showcase",
                        FilePath = "/uploads/videos/work-sample-4.mp4",
                        ThumbnailPath = "/images/globocon-logo.jpg",
                        UploadDate = DateTime.UtcNow.AddDays(-2),
                        DisplayOrder = 4,
                        IsPortrait = true,
                        IsMalayalam = false
                    }
                };

                context.Videos.AddRange(initialEnglishVideos);
                context.SaveChanges();
            }

            // Sync/Seed Real Malayalam Showcase Videos (Darsan video removed as requested)
            var realMalayalamFiles = new[]
            {
                new { File = "Challengers Gramolsavam.mp4", Title = "Challengers Gramolsavam - Community Event Platform Showcase", Order = 1 },
                new { File = "Miracle Energy Solutions.mp4", Title = "Miracle Energy Solutions - Industrial Energy Platform", Order = 2 },
                new { File = "Moksha Travels - Mahabelipuram and Kanchipuram Package.mp4", Title = "Moksha Travels - Temple Tour Package System", Order = 3 },
                new { File = "Moksha Travels - Vaishnava Devi Package.mp4", Title = "Moksha Travels - Pilgrimage Booking Platform", Order = 4 },
                new { File = "Nima Elevators.mp4", Title = "Nima Elevators - Field Service & Maintenance Application", Order = 5 },
                new { File = "The Science Tutoring.mp4", Title = "The Science Tutoring - EdTech Learning Platform", Order = 6 },
                new { File = "Vande Bharat Holidays.mp4", Title = "Vande Bharat Holidays - Tour Management Portal", Order = 7 }
            };

            foreach (var item in realMalayalamFiles)
            {
                string relPath = $"/uploads/videos/{item.File}";
                if (!context.Videos.Any(v => v.FilePath == relPath))
                {
                    context.Videos.Add(new Video
                    {
                        Title = item.Title,
                        FilePath = relPath,
                        ThumbnailPath = "/images/globocon-logo.jpg",
                        UploadDate = DateTime.UtcNow.AddHours(-item.Order),
                        DisplayOrder = item.Order,
                        IsPortrait = false,
                        IsMalayalam = true
                    });
                }
            }

            // Explicitly remove Darsan video if present in DB
            var darsanVideos = context.Videos.Where(v => v.Title.Contains("Darsan") || v.FilePath.Contains("Darsan")).ToList();
            if (darsanVideos.Any())
            {
                context.Videos.RemoveRange(darsanVideos);
            }

            // Clean out sample placeholders from Malayalam showcase if real files are registered
            var placeholderMalayalams = context.Videos.Where(v => v.IsMalayalam && v.FilePath.Contains("work-sample")).ToList();
            if (placeholderMalayalams.Any())
            {
                context.Videos.RemoveRange(placeholderMalayalams);
            }

            context.SaveChanges();
        }
    }
}
