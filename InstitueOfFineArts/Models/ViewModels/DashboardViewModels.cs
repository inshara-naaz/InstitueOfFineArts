using InstituteOfFineArts.Models;

namespace InstituteOfFineArts.Models.ViewModels
{
    public class HomeDashboardViewModel
    {
        public List<Competition> UpcomingCompetitions { get; set; } = new();
        public List<Award> RecentAwards { get; set; } = new();
        public int OngoingCompetitionSubmissionCount { get; set; }
        public Competition? OngoingCompetition { get; set; }
    }

    public class StudentDashboardViewModel
    {
        public List<Competition> UpcomingCompetitions { get; set; } = new();
        public List<Submission> MySubmissions { get; set; } = new();
        public List<Award> MyAwards { get; set; } = new();
        public List<StudentExhibitionStatusViewModel> MyExhibitionPaintings { get; set; } = new();
        public List<Submission> BestSubmissions { get; set; } = new();
    }

    public class ManagerDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalStaff { get; set; }
        public int TotalCompetitions { get; set; }
        public int TotalSubmissions { get; set; }
        public int TotalAwards { get; set; }
        public int TotalExhibitions { get; set; }
        public int TotalExhibitionListings { get; set; }
        public int PaintingsSold { get; set; }
        public int PaymentsPendingToStudents { get; set; }
    }

    public class SubmissionReportViewModel
    {
        public int? CompetitionId { get; set; }
        public string? CompetitionTitle { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public SubmissionGrade? FilterGrade { get; set; }
        public List<Submission> Submissions { get; set; } = new();
        public List<Competition> AllCompetitions { get; set; } = new();
    }
}
