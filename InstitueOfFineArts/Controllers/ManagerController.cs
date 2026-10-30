using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstituteOfFineArts.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var submissions = await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Competition)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            ViewBag.TotalSubmissions = submissions.Count;
            ViewBag.PendingReviews = submissions.Count(s => s.Grade == SubmissionGrade.Pending);
            ViewBag.ActiveCompetitions = await _context.Competitions.CountAsync(c => c.Status == CompetitionStatus.Ongoing);
            ViewBag.RegisteredStudents = await _context.Students.CountAsync();

            return View(submissions);
        }

        public async Task<IActionResult> ViewPostings()
        {
            var submissions = await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Competition)
                .Include(s => s.Evaluation)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            return View(submissions);
        }

        public async Task<IActionResult> Reports()
        {
            var submissions = await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Competition)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            ViewBag.TotalSubmissions = submissions.Count;
            ViewBag.ApprovedArtworks = submissions.Count(s => s.Grade != SubmissionGrade.Pending && s.Grade != SubmissionGrade.Disqualified);
            ViewBag.PendingReviews = submissions.Count(s => s.Grade == SubmissionGrade.Pending);

            var total = submissions.Count;
            var approved = submissions.Count(s => s.Grade != SubmissionGrade.Pending && s.Grade != SubmissionGrade.Disqualified);
            ViewBag.ApprovalRate = total == 0 ? "0%" : (approved * 100 / total) + "%";

            return View(submissions);
        }
    }
}