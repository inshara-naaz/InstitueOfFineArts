using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace InstituteOfFineArts.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public StaffController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // DASHBOARD

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.StudentCount = await _context.Students.CountAsync(s => s.IsActive);
            ViewBag.CompetitionCount = await _context.Competitions.CountAsync();
            ViewBag.SubmissionCount = await _context.Submissions.CountAsync();
            ViewBag.ExhibitionCount = await _context.Exhibitions.CountAsync();

            ViewBag.PendingSubmissionCount = await _context.Submissions
                .CountAsync(s => s.Grade == SubmissionGrade.Pending);

            ViewBag.ReviewedSubmissionCount = await _context.Submissions
                .CountAsync(s => s.Grade != SubmissionGrade.Pending);

            ViewBag.BestSubmissionCount = await _context.Submissions
                .CountAsync(s => s.Grade == SubmissionGrade.Best);

            return View();
        }

        // VIEW STUDENTS

        public async Task<IActionResult> ViewStudents()
        {
            var students = await _context.Students
                .Include(s => s.ApplicationUser)
                .Include(s => s.Submissions)
                    .ThenInclude(sub => sub.Competition)
                .Include(s => s.Awards)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return View(students);
        }


        // COMPETITIONS CRUD

        public IActionResult AddCompetition()
        {
            return View();
        }

        public async Task<IActionResult> ManageCompetitions()
        {
            var competitions = await _context.Competitions
                .Include(c => c.ManagedByStaff)
                .Include(c => c.Submissions)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.TotalCompetitions = competitions.Count;
            ViewBag.OngoingCount = competitions.Count(c => c.Status == CompetitionStatus.Ongoing);
            ViewBag.UpcomingCount = competitions.Count(c => c.Status == CompetitionStatus.Upcoming);
            ViewBag.CompletedCount = competitions.Count(c => c.Status == CompetitionStatus.Concluded);

            return View(competitions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompetition(
            string Title,
            string? Description,
            DateTime StartDate,
            DateTime EndDate,
            string? Conditions,
            CompetitionStatus Status,
            IFormFile? ImageFile)
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                TempData["Error"] = "Competition title is required.";
                return RedirectToAction(nameof(ManageCompetitions));
            }

            if (EndDate < StartDate)
            {
                TempData["Error"] = "End date cannot be before start date.";
                return RedirectToAction(nameof(ManageCompetitions));
            }

            var staff = await GetCurrentStaff();

            if (staff == null)
            {
                TempData["Error"] = "Staff profile not found. Please contact admin.";
                return RedirectToAction(nameof(Dashboard));
            }

            //  IMAGE SAVE CODE 
            string? imagePath = null;
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "competitions");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                imagePath = $"/uploads/competitions/{uniqueFileName}";
            }

            var competition = new Competition
            {
                Title = Title,
                Description = Description,
                StartDate = StartDate,
                EndDate = EndDate,
                Conditions = Conditions,
                Status = Status,
                StaffId = staff.StaffId,
                CreatedAt = DateTime.UtcNow,
                ImagePath = imagePath
            };

            _context.Competitions.Add(competition);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Competition created successfully.";
            return RedirectToAction(nameof(ManageCompetitions));
        }

        [HttpGet]
        public async Task<IActionResult> EditCompetition(int id)
        {
            var competition = await _context.Competitions
                .Include(c => c.ManagedByStaff)
                .FirstOrDefaultAsync(c => c.CompetitionId == id);

            if (competition == null)
            {
                TempData["Error"] = "Competition not found.";
                return RedirectToAction(nameof(ManageCompetitions));
            }

            return View(competition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompetition(
            int CompetitionId,
            string Title,
            string? Description,
            DateTime StartDate,
            DateTime EndDate,
            string? Conditions,
            CompetitionStatus Status,
            IFormFile? ImageFile)
        {
            var competition = await _context.Competitions
                .FirstOrDefaultAsync(c => c.CompetitionId == CompetitionId);

            if (competition == null)
            {
                TempData["Error"] = "Competition not found.";
                return RedirectToAction(nameof(ManageCompetitions));
            }

            if (string.IsNullOrWhiteSpace(Title))
            {
                TempData["Error"] = "Competition title is required.";
                return RedirectToAction(nameof(EditCompetition), new { id = CompetitionId });
            }

            if (EndDate < StartDate)
            {
                TempData["Error"] = "End date cannot be before start date.";
                return RedirectToAction(nameof(EditCompetition), new { id = CompetitionId });
            }

            competition.Title = Title;
            competition.Description = Description;
            competition.StartDate = StartDate;
            competition.EndDate = EndDate;
            competition.Conditions = Conditions;
            competition.Status = Status;
            competition.UpdatedAt = DateTime.UtcNow;

            //  IMAGE UPDATE CODE 
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "competitions");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                competition.ImagePath = $"/uploads/competitions/{uniqueFileName}";
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Competition updated successfully.";
            return RedirectToAction(nameof(ManageCompetitions));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompetition(int id)
        {
            var competition = await _context.Competitions
                .Include(c => c.Submissions)
                .FirstOrDefaultAsync(c => c.CompetitionId == id);

            if (competition == null)
            {
                TempData["Error"] = "Competition not found.";
                return RedirectToAction(nameof(ManageCompetitions));
            }

            if (competition.Submissions.Any())
            {
                TempData["Error"] = "This competition has submissions, so it cannot be deleted.";
                return RedirectToAction(nameof(ManageCompetitions));
            }

            _context.Competitions.Remove(competition);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Competition deleted successfully.";
            return RedirectToAction(nameof(ManageCompetitions));
        }

        // EXHIBITIONS CRUD

        public async Task<IActionResult> ManageExhibitions()
        {
            var exhibitions = await _context.Exhibitions
                .Include(e => e.ManagedByStaff)
                .Include(e => e.ExhibitionListings)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            ViewBag.TotalExhibitions = exhibitions.Count;
            ViewBag.OngoingExhibitions = exhibitions.Count(e => e.Status == InstituteOfFineArts.Models.ExhibitionStatus.Ongoing);
            ViewBag.UpcomingExhibitions = exhibitions.Count(e => e.Status == InstituteOfFineArts.Models.ExhibitionStatus.Upcoming);
            ViewBag.CompletedExhibitions = exhibitions.Count(e => e.Status == InstituteOfFineArts.Models.ExhibitionStatus.Concluded);

            return View(exhibitions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExhibition(
            string ExhibitionName,
            string? Description,
            string Venue,
            string? City,
            string? Country,
            DateTime ExhibitionStartDate,
            DateTime ExhibitionEndDate,
            InstituteOfFineArts.Models.ExhibitionStatus Status,
            string? AdditionalNotes,
            IFormFile? ImageFile)  // ← IMAGE PARAMETER ADD KIYA
        {
            if (string.IsNullOrWhiteSpace(ExhibitionName))
            {
                TempData["Error"] = "Exhibition name is required.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            if (string.IsNullOrWhiteSpace(Venue))
            {
                TempData["Error"] = "Venue is required.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            if (ExhibitionEndDate < ExhibitionStartDate)
            {
                TempData["Error"] = "End date cannot be before start date.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            var staff = await GetCurrentStaff();

            if (staff == null)
            {
                TempData["Error"] = "Staff profile not found. Please contact admin.";
                return RedirectToAction(nameof(Dashboard));
            }

            var exhibition = new Exhibition
            {
                ExhibitionName = ExhibitionName,
                Description = Description,
                Venue = Venue,
                City = City ?? "",
                Country = string.IsNullOrWhiteSpace(Country) ? "Pakistan" : Country,
                ExhibitionStartDate = ExhibitionStartDate,
                ExhibitionEndDate = ExhibitionEndDate,
                Status = Status,
                StaffId = staff.StaffId,
                AdditionalNotes = AdditionalNotes,
                CreatedAt = DateTime.UtcNow
            };

            // 🔥 IMAGE SAVE CODE ADD KIYA 🔥
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "exhibitions");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                exhibition.ImagePath = $"/uploads/exhibitions/{uniqueFileName}";
            }

            _context.Exhibitions.Add(exhibition);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Exhibition created successfully.";
            return RedirectToAction(nameof(ManageExhibitions));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExhibition(
            int ExhibitionId,
            string ExhibitionName,
            string? Description,
            string Venue,
            string? City,
            string? Country,
            DateTime ExhibitionStartDate,
            DateTime ExhibitionEndDate,
            InstituteOfFineArts.Models.ExhibitionStatus Status,
            string? AdditionalNotes)
        {
            var exhibition = await _context.Exhibitions
                .FirstOrDefaultAsync(e => e.ExhibitionId == ExhibitionId);

            if (exhibition == null)
            {
                TempData["Error"] = "Exhibition not found.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            if (string.IsNullOrWhiteSpace(ExhibitionName))
            {
                TempData["Error"] = "Exhibition name is required.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            if (string.IsNullOrWhiteSpace(Venue))
            {
                TempData["Error"] = "Venue is required.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            if (ExhibitionEndDate < ExhibitionStartDate)
            {
                TempData["Error"] = "End date cannot be before start date.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            exhibition.ExhibitionName = ExhibitionName;
            exhibition.Description = Description;
            exhibition.Venue = Venue;
            exhibition.City = City ?? "";
            exhibition.Country = string.IsNullOrWhiteSpace(Country) ? "Pakistan" : Country;
            exhibition.ExhibitionStartDate = ExhibitionStartDate;
            exhibition.ExhibitionEndDate = ExhibitionEndDate;
            exhibition.Status = Status;
            exhibition.AdditionalNotes = AdditionalNotes;
            exhibition.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Exhibition updated successfully.";
            return RedirectToAction(nameof(ManageExhibitions));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExhibition(int id)
        {
            var exhibition = await _context.Exhibitions
                .Include(e => e.ExhibitionListings)
                .FirstOrDefaultAsync(e => e.ExhibitionId == id);

            if (exhibition == null)
            {
                TempData["Error"] = "Exhibition not found.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            if (exhibition.ExhibitionListings.Any())
            {
                TempData["Error"] = "This exhibition has artwork listings, so it cannot be deleted.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            _context.Exhibitions.Remove(exhibition);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Exhibition deleted successfully.";
            return RedirectToAction(nameof(ManageExhibitions));
        }

        // VIEW POSTINGS

        public async Task<IActionResult> ViewPostings()
        {
            var submissions = await _context.Submissions
                .Include(s => s.Student)
                    .ThenInclude(st => st.ApplicationUser)
                .Include(s => s.Competition)
                .Include(s => s.Evaluation)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            ViewBag.TotalSubmissions = submissions.Count;
            ViewBag.PendingSubmissions = submissions.Count(s => s.Grade == SubmissionGrade.Pending);
            ViewBag.BestSubmissions = submissions.Count(s => s.Grade == SubmissionGrade.Best);
            ViewBag.LateSubmissions = submissions.Count(s => s.IsLateSubmission);

            return View(submissions);
        }

        // MARK / EVALUATE POSTINGS

        public async Task<IActionResult> MarkPostings()
        {
            var submissions = await _context.Submissions
                .Include(s => s.Student)
                    .ThenInclude(st => st.ApplicationUser)
                .Include(s => s.Competition)
                .Include(s => s.Evaluation)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            ViewBag.TotalSubmissions = submissions.Count;
            ViewBag.PendingCount = submissions.Count(s => s.Grade == SubmissionGrade.Pending);
            ViewBag.ReviewedCount = submissions.Count(s => s.Grade != SubmissionGrade.Pending);
            ViewBag.BestCount = submissions.Count(s => s.Grade == SubmissionGrade.Best);

            return View(submissions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEvaluation(
            int SubmissionId,
            SubmissionGrade Grade,
            string? PositivePoints,
            string? NegativePoints,
            string? AreasOfImprovement,
            string? GeneralRemarks)
        {
            var staff = await GetCurrentStaff();

            if (staff == null)
            {
                TempData["Error"] = "Staff profile not found. Please contact admin.";
                return RedirectToAction(nameof(MarkPostings));
            }

            var submission = await _context.Submissions
                .Include(s => s.Evaluation)
                .FirstOrDefaultAsync(s => s.SubmissionId == SubmissionId);

            if (submission == null)
            {
                TempData["Error"] = "Submission not found.";
                return RedirectToAction(nameof(MarkPostings));
            }

            if (submission.Evaluation == null)
            {
                var evaluation = new Evaluation
                {
                    SubmissionId = submission.SubmissionId,
                    StaffId = staff.StaffId,
                    Grade = Grade,
                    PositivePoints = PositivePoints,
                    NegativePoints = NegativePoints,
                    AreasOfImprovement = AreasOfImprovement,
                    GeneralRemarks = GeneralRemarks,
                    EvaluatedAt = DateTime.UtcNow
                };

                _context.Evaluations.Add(evaluation);
            }
            else
            {
                submission.Evaluation.StaffId = staff.StaffId;
                submission.Evaluation.Grade = Grade;
                submission.Evaluation.PositivePoints = PositivePoints;
                submission.Evaluation.NegativePoints = NegativePoints;
                submission.Evaluation.AreasOfImprovement = AreasOfImprovement;
                submission.Evaluation.GeneralRemarks = GeneralRemarks;
                submission.Evaluation.UpdatedAt = DateTime.UtcNow;
            }

            submission.Grade = Grade;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Submission evaluated successfully.";
            return RedirectToAction(nameof(MarkPostings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EvaluateSubmission(
            int SubmissionId,
            SubmissionGrade Grade,
            string? PositivePoints,
            string? NegativePoints,
            string? AreasOfImprovement,
            string? GeneralRemarks)
        {
            return await SaveEvaluation(
                SubmissionId,
                Grade,
                PositivePoints,
                NegativePoints,
                AreasOfImprovement,
                GeneralRemarks
            );
        }

        // SEND TO EXHIBITION

        public async Task<IActionResult> SendtoExhibition()
        {
            var submissions = await _context.Submissions
                .Include(s => s.Student)
                    .ThenInclude(st => st.ApplicationUser)
                .Include(s => s.Competition)
                .Include(s => s.ExhibitionListings)
                .Where(s => s.Grade == SubmissionGrade.Best)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            var exhibitions = await _context.Exhibitions
                .Where(e => e.Status == InstituteOfFineArts.Models.ExhibitionStatus.Upcoming ||
                            e.Status == InstituteOfFineArts.Models.ExhibitionStatus.Ongoing)
                .OrderBy(e => e.ExhibitionStartDate)
                .ToListAsync();

            ViewBag.Exhibitions = exhibitions;

            ViewBag.TotalAvailable = submissions.Count(s => !s.ExhibitionListings.Any());
            ViewBag.TotalExhibitions = exhibitions.Count;
            ViewBag.SentCount = submissions.Count(s => s.ExhibitionListings.Any());
            ViewBag.PendingCount = submissions.Count(s => !s.ExhibitionListings.Any());

            ViewBag.AvailableArtworks = ViewBag.TotalAvailable;
            ViewBag.ActiveExhibitions = ViewBag.TotalExhibitions;
            ViewBag.PendingSelection = ViewBag.PendingCount;

            return View(submissions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendArtworkToExhibition(
            int SubmissionId,
            int ExhibitionId,
            decimal StudentQuotedPrice,
            string? Notes)
        {
            var staff = await GetCurrentStaff();

            if (staff == null)
            {
                TempData["Error"] = "Staff profile not found. Please contact admin.";
                return RedirectToAction(nameof(SendtoExhibition));
            }

            if (SubmissionId <= 0)
            {
                TempData["Error"] = "Please select valid artwork.";
                return RedirectToAction(nameof(SendtoExhibition));
            }

            if (ExhibitionId <= 0)
            {
                TempData["Error"] = "Please select valid exhibition.";
                return RedirectToAction(nameof(SendtoExhibition));
            }

            if (StudentQuotedPrice <= 0)
            {
                TempData["Error"] = "Please enter valid price.";
                return RedirectToAction(nameof(SendtoExhibition));
            }

            var submission = await _context.Submissions
                .Include(s => s.ExhibitionListings)
                .FirstOrDefaultAsync(s => s.SubmissionId == SubmissionId);

            if (submission == null)
            {
                TempData["Error"] = "Submission not found.";
                return RedirectToAction(nameof(SendtoExhibition));
            }

            if (submission.Grade != SubmissionGrade.Best)
            {
                TempData["Error"] = "Only Best graded artworks can be sent to exhibition.";
                return RedirectToAction(nameof(SendtoExhibition));
            }

            var exhibition = await _context.Exhibitions
                .FirstOrDefaultAsync(e => e.ExhibitionId == ExhibitionId);

            if (exhibition == null)
            {
                TempData["Error"] = "Exhibition not found.";
                return RedirectToAction(nameof(SendtoExhibition));
            }

            var alreadySent = await _context.ExhibitionListings
                .AnyAsync(e => e.SubmissionId == SubmissionId && e.ExhibitionId == ExhibitionId);

            if (alreadySent)
            {
                TempData["Error"] = "This artwork is already sent to this exhibition.";
                return RedirectToAction(nameof(SendtoExhibition));
            }

            var listing = new ExhibitionListing
            {
                ExhibitionId = ExhibitionId,
                SubmissionId = SubmissionId,
                ForwardedByStaffId = staff.StaffId,
                StudentQuotedPrice = StudentQuotedPrice,
                SaleStatus = SaleStatus.Available,
                PaymentStatus = PaymentStatus.NotApplicable,
                Notes = Notes,
                ListedAt = DateTime.UtcNow
            };

            _context.ExhibitionListings.Add(listing);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Artwork sent to exhibition successfully.";
            return RedirectToAction(nameof(SendtoExhibition));
        }

        // =========================
        // EXHIBITION STATUS / SALE UPDATE
        // =========================

        public async Task<IActionResult> ExhibitionStatus()
        {
            var listings = await _context.ExhibitionListings
                .Include(e => e.Exhibition)
                .Include(e => e.Submission)
                    .ThenInclude(s => s.Student)
                        .ThenInclude(st => st.ApplicationUser)
                .Include(e => e.Submission)
                    .ThenInclude(s => s.Competition)
                .OrderByDescending(e => e.ListedAt)
                .ToListAsync();

            return View(listings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateExhibitionListingStatus(
            int ExhibitionListingId,
            SaleStatus SaleStatus,
            decimal? FinalSalePrice,
            DateTime? DateSold,
            string? BuyerName,
            string? BuyerPhone,
            string? BuyerEmail,
            string? BuyerAddress,
            PaymentStatus PaymentStatus,
            decimal? AmountPaidToStudent,
            DateTime? PaymentDate,
            string? PaymentRemarks)
        {
            var listing = await _context.ExhibitionListings
                .FirstOrDefaultAsync(e => e.ExhibitionListingId == ExhibitionListingId);

            if (listing == null)
            {
                TempData["Error"] = "Exhibition listing not found.";
                return RedirectToAction(nameof(ExhibitionStatus));
            }

            listing.SaleStatus = SaleStatus;
            listing.FinalSalePrice = FinalSalePrice;
            listing.DateSold = SaleStatus == SaleStatus.Sold
                ? (DateSold ?? DateTime.UtcNow)
                : DateSold;

            listing.BuyerName = BuyerName;
            listing.BuyerPhone = BuyerPhone;
            listing.BuyerEmail = BuyerEmail;
            listing.BuyerAddress = BuyerAddress;

            listing.PaymentStatus = PaymentStatus;
            listing.AmountPaidToStudent = AmountPaidToStudent;
            listing.PaymentDate = PaymentStatus == PaymentStatus.Paid
                ? (PaymentDate ?? DateTime.UtcNow)
                : PaymentDate;

            listing.PaymentRemarks = PaymentRemarks;
            listing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Exhibition listing status updated successfully.";
            return RedirectToAction(nameof(ExhibitionStatus));
        }

        // HELPER

        private async Task<Staff?> GetCurrentStaff()
        {
            var userId = _userManager.GetUserId(User);

            return await _context.Staff
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId && s.IsActive);
        }
    }
}