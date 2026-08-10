using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GloboconSoftwareWeb.Data;
using GloboconSoftwareWeb.Models;

namespace GloboconSoftwareWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredVideos = await _context.Videos
                .Where(v => !v.IsMalayalam)
                .OrderBy(v => v.DisplayOrder)
                .ThenByDescending(v => v.UploadDate)
                .Take(4)
                .ToListAsync();

            ViewBag.FeaturedVideos = featuredVideos;
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }

        public async Task<IActionResult> Work()
        {
            var videos = await _context.Videos
                .Where(v => !v.IsMalayalam)
                .OrderBy(v => v.DisplayOrder)
                .ThenByDescending(v => v.UploadDate)
                .ToListAsync();

            return View(videos);
        }

        // Unlisted Malayalam Work Video Showcase Page (Accessible via specific link only)
        public async Task<IActionResult> MalayalamWork()
        {
            var videos = await _context.Videos
                .Where(v => v.IsMalayalam)
                .OrderBy(v => v.DisplayOrder)
                .ThenByDescending(v => v.UploadDate)
                .ToListAsync();

            return View(videos);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactSubmission());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactSubmission model, [FromForm(Name = "WebsiteUrl")] string? honeypot, [FromForm(Name = "cf-turnstile-response")] string? turnstileToken)
        {
            // 1. Anti-Spam Honeypot Check: if hidden field is filled, reject silently
            if (!string.IsNullOrEmpty(honeypot))
            {
                // Silently pretend success to fool automated spambots
                TempData["SuccessMessage"] = "Thank you! Your message has been received.";
                return RedirectToAction(nameof(Contact));
            }

            if (ModelState.IsValid)
            {
                model.SubmittedAt = DateTime.UtcNow;
                model.IsRead = false;

                _context.ContactSubmissions.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thank you for reaching out! Your message has been received and our team will get back to you shortly.";
                return RedirectToAction(nameof(Contact));
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
