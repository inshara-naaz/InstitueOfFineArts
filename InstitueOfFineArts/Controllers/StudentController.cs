using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstituteOfFineArts.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public StudentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Dashboard()
        {
            var student = await GetCurrentStudent();

            if (student == null)
            {
                TempData["Error"] = "Student profile not found. Please contact admin.";
                return RedirectToAction("Login", "Account");
            }

            ViewBag.MySubmissionsCount = await _context.Submissions
                .CountAsync(s => s.StudentId == student.StudentId);

            ViewBag.MyAwardsCount = await _context.Awards
                .CountAsync(a => a.StudentId == student.StudentId);

            ViewBag.ActiveCompetitions = await _context.Competitions
                .CountAsync(c => c.Status == CompetitionStatus.Ongoing);

            ViewBag.TotalArtworks = await _context.Submissions.CountAsync();

            ViewBag.RecentSubmissions = await _context.Submissions
                .Include(s => s.Competition)
                .Where(s => s.StudentId == student.StudentId)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(5)
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> MyPostings()
        {
            var student = await GetCurrentStudent();

            if (student == null)
            {
                TempData["Error"] = "Student profile not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            var submissions = await _context.Submissions
                .Include(s => s.Competition)
                .Where(s => s.StudentId == student.StudentId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            return View(submissions);
        }

        [HttpGet]
        public async Task<IActionResult> UploadPosting()
        {
            var competitions = await _context.Competitions
                .Where(c => c.Status == CompetitionStatus.Ongoing && c.EndDate >= DateTime.Now)
                .OrderBy(c => c.EndDate)
                .ToListAsync();

            return View(competitions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSubmission(
            int CompetitionId,
            string PaintingTitle,
            string AccompanyingText,
            string TextType,
            string? CreativeDescription,
            IFormFile ImageFile)
        {
            var student = await GetCurrentStudent();

            if (student == null)
            {
                TempData["Error"] = "Student profile not found. Please contact admin.";
                return RedirectToAction(nameof(Dashboard));
            }

            if (CompetitionId <= 0)
            {
                TempData["Error"] = "Please select a competition.";
                return RedirectToAction(nameof(UploadPosting));
            }

            if (string.IsNullOrWhiteSpace(PaintingTitle))
            {
                TempData["Error"] = "Painting title is required.";
                return RedirectToAction(nameof(UploadPosting));
            }

            if (string.IsNullOrWhiteSpace(AccompanyingText))
            {
                TempData["Error"] = "Poem / quotation / story / description is required.";
                return RedirectToAction(nameof(UploadPosting));
            }

            if (ImageFile == null || ImageFile.Length == 0)
            {
                TempData["Error"] = "Please upload a JPG/JPEG image.";
                return RedirectToAction(nameof(UploadPosting));
            }

            var extension = Path.GetExtension(ImageFile.FileName).ToLower();

            if (extension != ".jpg" && extension != ".jpeg")
            {
                TempData["Error"] = "Only JPG/JPEG files are allowed.";
                return RedirectToAction(nameof(UploadPosting));
            }

            if (ImageFile.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "File size must be less than 5 MB.";
                return RedirectToAction(nameof(UploadPosting));
            }

            var competition = await _context.Competitions
                .FirstOrDefaultAsync(c => c.CompetitionId == CompetitionId);

            if (competition == null)
            {
                TempData["Error"] = "Competition not found.";
                return RedirectToAction(nameof(UploadPosting));
            }

            var alreadySubmitted = await _context.Submissions
                .AnyAsync(s => s.StudentId == student.StudentId && s.CompetitionId == competition.CompetitionId);

            if (alreadySubmitted)
            {
                TempData["Error"] = "You have already submitted artwork for this competition. Please edit your existing posting instead.";
                return RedirectToAction(nameof(MyPostings));
            }

            var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "submissions");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }

            var submission = new Submission
            {
                StudentId = student.StudentId,
                CompetitionId = competition.CompetitionId,
                ImageFilePath = $"/uploads/submissions/{uniqueFileName}",
                OriginalFileName = ImageFile.FileName,
                FileSizeInBytes = ImageFile.Length,
                PaintingTitle = PaintingTitle,
                AccompanyingText = AccompanyingText,
                TextType = string.IsNullOrWhiteSpace(TextType) ? "Description" : TextType,
                CreativeDescription = CreativeDescription,
                SubmittedAt = DateTime.UtcNow,
                IsLateSubmission = DateTime.Now > competition.EndDate,
                Grade = DateTime.Now > competition.EndDate ? SubmissionGrade.Disqualified : SubmissionGrade.Pending
            };

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Artwork submitted successfully.";
            return RedirectToAction(nameof(MyPostings));
        }

        public async Task<IActionResult> EditPosting()
        {
            var student = await GetCurrentStudent();

            if (student == null)
            {
                TempData["Error"] = "Student profile not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            var submissions = await _context.Submissions
                .Include(s => s.Competition)
                .Where(s => s.StudentId == student.StudentId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            return View(submissions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePosting(
            int SubmissionId,
            string PaintingTitle,
            string AccompanyingText,
            string TextType,
            string? CreativeDescription,
            IFormFile? ImageFile)
        {
            var student = await GetCurrentStudent();

            if (student == null)
            {
                TempData["Error"] = "Student profile not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            var submission = await _context.Submissions
                .Include(s => s.Competition)
                .FirstOrDefaultAsync(s => s.SubmissionId == SubmissionId && s.StudentId == student.StudentId);

            if (submission == null)
            {
                TempData["Error"] = "Posting not found.";
                return RedirectToAction(nameof(EditPosting));
            }

            if (submission.Competition != null && submission.Competition.EndDate < DateTime.Now)
            {
                TempData["Error"] = "Deadline passed. You cannot edit this posting now.";
                return RedirectToAction(nameof(EditPosting));
            }

            if (submission.Grade != SubmissionGrade.Pending)
            {
                TempData["Error"] = "This posting is already evaluated. You cannot edit it now.";
                return RedirectToAction(nameof(EditPosting));
            }

            if (string.IsNullOrWhiteSpace(PaintingTitle))
            {
                TempData["Error"] = "Painting title is required.";
                return RedirectToAction(nameof(EditPosting));
            }

            if (string.IsNullOrWhiteSpace(AccompanyingText))
            {
                TempData["Error"] = "Text is required.";
                return RedirectToAction(nameof(EditPosting));
            }

            submission.PaintingTitle = PaintingTitle;
            submission.AccompanyingText = AccompanyingText;
            submission.TextType = string.IsNullOrWhiteSpace(TextType) ? "Description" : TextType;
            submission.CreativeDescription = CreativeDescription;
            submission.LastUpdatedAt = DateTime.UtcNow;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var extension = Path.GetExtension(ImageFile.FileName).ToLower();

                if (extension != ".jpg" && extension != ".jpeg")
                {
                    TempData["Error"] = "Only JPG/JPEG files are allowed.";
                    return RedirectToAction(nameof(EditPosting));
                }

                if (ImageFile.Length > 5 * 1024 * 1024)
                {
                    TempData["Error"] = "File size must be less than 5 MB.";
                    return RedirectToAction(nameof(EditPosting));
                }

                var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "submissions");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                submission.ImageFilePath = $"/uploads/submissions/{uniqueFileName}";
                submission.OriginalFileName = ImageFile.FileName;
                submission.FileSizeInBytes = ImageFile.Length;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Posting updated successfully.";
            return RedirectToAction(nameof(MyPostings));
        }

        public async Task<IActionResult> MyAwards()
        {
            var student = await GetCurrentStudent();

            if (student == null)
            {
                TempData["Error"] = "Student profile not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            var awards = await _context.Awards
                .Include(a => a.Competition)
                .Include(a => a.CompetitionAward)
                .Where(a => a.StudentId == student.StudentId)
                .OrderByDescending(a => a.DateIssued)
                .ToListAsync();

            return View(awards);
        }

        public async Task<IActionResult> BestPostings()
        {
            var bestPostings = await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Competition)
                .Where(s => s.Grade == SubmissionGrade.Best)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            return View(bestPostings);
        }

        public async Task<IActionResult> ExhibitionStatus()
        {
            var student = await GetCurrentStudent();

            if (student == null)
            {
                TempData["Error"] = "Student profile not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            var listings = await _context.ExhibitionListings
                .Include(e => e.Submission)
                .ThenInclude(s => s.Competition)
                .Include(e => e.Exhibition)
                .Where(e => e.Submission.StudentId == student.StudentId)
                .OrderByDescending(e => e.ExhibitionListingId)
                .ToListAsync();

            return View(listings);
        }

        private async Task<Student?> GetCurrentStudent()
        {
            var userId = _userManager.GetUserId(User);

            return await _context.Students
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId && s.IsActive);
        }
    }
}