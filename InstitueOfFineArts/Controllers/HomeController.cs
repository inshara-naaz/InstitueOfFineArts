using InstituteOfFineArts.Models;
using InstituteOfFineArts.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstituteOfFineArts.Controllers
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
            var model = new HomeDashboardViewModel
            {
                UpcomingCompetitions = await _context.Competitions
                    .Where(c => c.Status == CompetitionStatus.Upcoming)
                    .OrderBy(c => c.StartDate)
                    .Take(5)
                    .ToListAsync(),

                OngoingCompetition = await _context.Competitions
                    .FirstOrDefaultAsync(c => c.Status == CompetitionStatus.Ongoing),

                OngoingCompetitionSubmissionCount = await _context.Submissions
                    .CountAsync(s => s.Competition.Status == CompetitionStatus.Ongoing),

                RecentAwards = await _context.Awards
                    .Include(a => a.Student)
                    .Include(a => a.Competition)
                    .Include(a => a.CompetitionAward)
                    .OrderByDescending(a => a.DateIssued)
                    .Take(10)
                    .ToListAsync()
            };

            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        // 🔥 YEH UPDATE KARO 🔥
        public async Task<IActionResult> Competitions()
        {
            var competitions = await _context.Competitions
                .Where(c => c.Status == CompetitionStatus.Ongoing || c.Status == CompetitionStatus.Upcoming)
                .OrderByDescending(c => c.Status)
                .ThenBy(c => c.StartDate)
                .ToListAsync();

            // Stats ke liye ViewBag
            ViewBag.TotalCompetitions = await _context.Competitions.CountAsync();
            ViewBag.TotalParticipants = await _context.Submissions.CountAsync();
            ViewBag.TotalPrizeAmount = "5M+";
            ViewBag.TotalWinners = await _context.Awards.CountAsync();

            return View(competitions);
        }

        public async Task<IActionResult> Exhibitions()
        {
            // Sabse pehle wali upcoming exhibition (timer ke liye)
            var nextExhibition = await _context.Exhibitions
                .Where(e => e.ExhibitionStartDate > DateTime.Now && e.Status == ExhibitionStatus.Upcoming)
                .OrderBy(e => e.ExhibitionStartDate)
                .FirstOrDefaultAsync();

            // 🔥 SAB EXHIBITIONS (cards ke liye)
            var allExhibitions = await _context.Exhibitions
                .Where(e => e.Status == ExhibitionStatus.Upcoming || e.Status == ExhibitionStatus.Ongoing)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            ViewBag.NextExhibition = nextExhibition;
            ViewBag.NextExhibitionDate = nextExhibition?.ExhibitionStartDate.ToString("yyyy-MM-dd HH:mm:ss");

            // Stats ke liye
            ViewBag.TotalExhibitions = await _context.Exhibitions.CountAsync();
            ViewBag.TotalArtworksSold = await _context.ExhibitionListings.CountAsync(l => l.SaleStatus == SaleStatus.Sold);
            ViewBag.TotalSales = await _context.ExhibitionListings
                .Where(l => l.SaleStatus == SaleStatus.Sold)
                .SumAsync(l => l.FinalSalePrice ?? 0);

            return View(allExhibitions);  
        }

        public async Task<IActionResult> Gallery()
        {
            // ExhibitionListing se woh submissions jo exhibition me bheji gayi hain
            var exhibitionListings = await _context.ExhibitionListings
                .Include(el => el.Submission)
                    .ThenInclude(s => s.Student)
                .Include(el => el.Submission)
                    .ThenInclude(s => s.Competition)
                .Where(el => el.SaleStatus == SaleStatus.Available || el.SaleStatus == SaleStatus.Sold)
                .OrderByDescending(el => el.ListedAt)
                .ToListAsync();

            ViewBag.ExhibitionListings = exhibitionListings;

            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }




        [Authorize]
        public IActionResult RedirectToDashboard()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else if (User.IsInRole("Manager"))
            {
                return RedirectToAction("Dashboard", "Manager");
            }
            else if (User.IsInRole("Staff"))
            {
                return RedirectToAction("Dashboard", "Staff");
            }
            else if (User.IsInRole("Student"))
            {
                return RedirectToAction("Dashboard", "Student");
            }

            return RedirectToAction("Index", "Home");
        }


    }
}
