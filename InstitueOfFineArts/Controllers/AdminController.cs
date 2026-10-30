using System.Text.Json;
using InstituteOfFineArts.Models;
using InstituteOfFineArts.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;  // ← YEH ADD KARO

namespace InstituteOfFineArts.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;  // ← YEH ADD KARO

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment environment)  // ← YEH ADD KARO (environment parameter)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _environment = environment;  // ← YEH ADD KARO
        }

        // =========================
        // DASHBOARD
        // =========================
        public async Task<IActionResult> Dashboard()
        {
            await LoadAdminCountsAsync();
            await LoadDashboardChartsAsync();

            ViewBag.RecentSubmissions = await _context.Submissions
                .Include(s => s.Student)
                    .ThenInclude(st => st.ApplicationUser)
                .Include(s => s.Competition)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(5)
                .ToListAsync();

            return View();
        }

        // =========================
        // MANAGE STUDENTS
        // =========================
        public async Task<IActionResult> ManageStudents()
        {
            var students = await _context.Students
                .Include(s => s.ApplicationUser)
                .OrderByDescending(s => s.StudentId)
                .ToListAsync();

            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(StudentCreateViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["Error"] = "Validation failed: " + string.Join(" | ", errorMessages);
                return RedirectToAction(nameof(ManageStudents));
            }

            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
            {
                TempData["Error"] = "This email already exists.";
                return RedirectToAction(nameof(ManageStudents));
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = $"{model.FirstName} {model.LastName}",
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                TempData["Error"] = result.Errors.FirstOrDefault()?.Description ?? "Student user could not be created.";
                return RedirectToAction(nameof(ManageStudents));
            }

            if (!await _roleManager.RoleExistsAsync("Student"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Student"));
            }

            await _userManager.AddToRoleAsync(user, "Student");

            var student = new Student
            {
                ApplicationUserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                ParentOrGuardianName = model.ParentOrGuardianName,
                ParentOrGuardianPhone = model.ParentOrGuardianPhone,
                EnrollmentNumber = model.EnrollmentNumber,
                AdmissionDate = model.AdmissionDate,
                Course = model.Course,
                Class = model.Class,
                Remarks = model.Remarks,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Student created successfully.";
            return RedirectToAction(nameof(ManageStudents));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStudent(
            int StudentId,
            string FirstName,
            string LastName,
            string Email,
            string Gender,
            DateTime DateOfBirth,
            string Address,
            string PhoneNumber,
            string? ParentOrGuardianName,
            string? ParentOrGuardianPhone,
            string EnrollmentNumber,
            DateTime AdmissionDate,
            string Course,
            string? Class,
            string? Remarks,
            bool IsActive)
        {
            var student = await _context.Students
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(s => s.StudentId == StudentId);

            if (student == null)
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction(nameof(ManageStudents));
            }

            var enrollmentExists = await _context.Students
                .AnyAsync(s => s.StudentId != StudentId && s.EnrollmentNumber == EnrollmentNumber);

            if (enrollmentExists)
            {
                TempData["Error"] = "This enrollment number already exists.";
                return RedirectToAction(nameof(ManageStudents));
            }

            student.FirstName = FirstName;
            student.LastName = LastName;
            student.Gender = Gender;
            student.DateOfBirth = DateOfBirth;
            student.Address = Address;
            student.PhoneNumber = PhoneNumber;
            student.ParentOrGuardianName = ParentOrGuardianName;
            student.ParentOrGuardianPhone = ParentOrGuardianPhone;
            student.EnrollmentNumber = EnrollmentNumber;
            student.AdmissionDate = AdmissionDate;
            student.Course = Course;
            student.Class = Class;
            student.Remarks = Remarks;
            student.IsActive = IsActive;

            if (student.ApplicationUser != null)
            {
                var emailOwner = await _userManager.FindByEmailAsync(Email);
                if (emailOwner != null && emailOwner.Id != student.ApplicationUser.Id)
                {
                    TempData["Error"] = "This email is already used by another user.";
                    return RedirectToAction(nameof(ManageStudents));
                }

                student.ApplicationUser.Email = Email;
                student.ApplicationUser.UserName = Email;
                student.ApplicationUser.FullName = $"{FirstName} {LastName}";
                student.ApplicationUser.IsActive = IsActive;

                await _userManager.UpdateAsync(student.ApplicationUser);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Student updated successfully.";
            return RedirectToAction(nameof(ManageStudents));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var student = await _context.Students
                    .Include(s => s.ApplicationUser)
                    .FirstOrDefaultAsync(s => s.StudentId == id);

                if (student == null)
                {
                    TempData["Error"] = "Student not found.";
                    return RedirectToAction(nameof(ManageStudents));
                }

                var userId = student.ApplicationUserId;

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    IF OBJECT_ID('dbo.ExhibitionListings', 'U') IS NOT NULL
                    BEGIN
                        DELETE EL
                        FROM dbo.ExhibitionListings EL
                        INNER JOIN dbo.Submissions S ON EL.SubmissionId = S.SubmissionId
                        WHERE S.StudentId = {id}
                    END

                    IF OBJECT_ID('dbo.Evaluations', 'U') IS NOT NULL
                    BEGIN
                        DELETE EV
                        FROM dbo.Evaluations EV
                        INNER JOIN dbo.Submissions S ON EV.SubmissionId = S.SubmissionId
                        WHERE S.StudentId = {id}
                    END

                    IF OBJECT_ID('dbo.Awards', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM dbo.Awards
                        WHERE StudentId = {id}
                    END

                    DELETE FROM dbo.Submissions
                    WHERE StudentId = {id}

                    DELETE FROM dbo.Students
                    WHERE StudentId = {id}
                ");

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        DELETE FROM dbo.AspNetUserRoles WHERE UserId = {userId}
                        DELETE FROM dbo.AspNetUserClaims WHERE UserId = {userId}
                        DELETE FROM dbo.AspNetUserLogins WHERE UserId = {userId}
                        DELETE FROM dbo.AspNetUserTokens WHERE UserId = {userId}
                        DELETE FROM dbo.AspNetUsers WHERE Id = {userId}
                    ");
                }

                await transaction.CommitAsync();

                TempData["Success"] = "Student deleted from database successfully.";
                return RedirectToAction(nameof(ManageStudents));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Delete failed: " + ex.Message;
                return RedirectToAction(nameof(ManageStudents));
            }
        }

        // =========================
        // MANAGE STAFF
        // =========================
        public async Task<IActionResult> ManageStaff()
        {
            var staff = await _context.Staff
                .Include(s => s.ApplicationUser)
                .OrderByDescending(s => s.StaffId)
                .ToListAsync();

            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaff(StaffCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill all required staff fields correctly.";
                return RedirectToAction(nameof(ManageStaff));
            }

            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
            {
                TempData["Error"] = "This email already exists.";
                return RedirectToAction(nameof(ManageStaff));
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = $"{model.FirstName} {model.LastName}",
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                TempData["Error"] = result.Errors.FirstOrDefault()?.Description ?? "Staff user could not be created.";
                return RedirectToAction(nameof(ManageStaff));
            }

            if (!await _roleManager.RoleExistsAsync("Staff"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Staff"));
            }

            await _userManager.AddToRoleAsync(user, "Staff");

            var staff = new Staff
            {
                ApplicationUserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                EmergencyContact = model.EmergencyContact,
                Qualification = model.Qualification,
                DateJoined = model.DateJoined,
                HandlesClass = model.HandlesClass,
                HandlesSubject = model.HandlesSubject,
                Designation = model.Designation,
                Remarks = model.Remarks,
                IsActive = true
            };

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Staff member created successfully.";
            return RedirectToAction(nameof(ManageStaff));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStaff(
            int StaffId,
            string FirstName,
            string LastName,
            string Email,
            string Gender,
            DateTime DateOfBirth,
            string Address,
            string PhoneNumber,
            string? EmergencyContact,
            string? Qualification,
            DateTime DateJoined,
            string? HandlesClass,
            string? HandlesSubject,
            string? Designation,
            string? Remarks,
            bool IsActive)
        {
            var staff = await _context.Staff
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(s => s.StaffId == StaffId);

            if (staff == null)
            {
                TempData["Error"] = "Staff not found.";
                return RedirectToAction(nameof(ManageStaff));
            }

            staff.FirstName = FirstName;
            staff.LastName = LastName;
            staff.Gender = Gender;
            staff.DateOfBirth = DateOfBirth;
            staff.Address = Address;
            staff.PhoneNumber = PhoneNumber;
            staff.EmergencyContact = EmergencyContact;
            staff.Qualification = Qualification;
            staff.DateJoined = DateJoined;
            staff.HandlesClass = HandlesClass;
            staff.HandlesSubject = HandlesSubject;
            staff.Designation = Designation;
            staff.Remarks = Remarks;
            staff.IsActive = IsActive;

            if (staff.ApplicationUser != null)
            {
                var emailOwner = await _userManager.FindByEmailAsync(Email);
                if (emailOwner != null && emailOwner.Id != staff.ApplicationUser.Id)
                {
                    TempData["Error"] = "This email is already used by another user.";
                    return RedirectToAction(nameof(ManageStaff));
                }

                staff.ApplicationUser.Email = Email;
                staff.ApplicationUser.UserName = Email;
                staff.ApplicationUser.FullName = $"{FirstName} {LastName}";
                staff.ApplicationUser.IsActive = IsActive;

                await _userManager.UpdateAsync(staff.ApplicationUser);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Staff updated successfully.";
            return RedirectToAction(nameof(ManageStaff));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var staff = await _context.Staff
                    .Include(s => s.ApplicationUser)
                    .FirstOrDefaultAsync(s => s.StaffId == id);

                if (staff == null)
                {
                    TempData["Error"] = "Staff not found.";
                    return RedirectToAction(nameof(ManageStaff));
                }

                var userId = staff.ApplicationUserId;

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    IF OBJECT_ID('dbo.ExhibitionListings', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM dbo.ExhibitionListings
                        WHERE ForwardedByStaffId = {id}
                    END

                    IF OBJECT_ID('dbo.Evaluations', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM dbo.Evaluations
                        WHERE StaffId = {id}
                    END

                    IF OBJECT_ID('dbo.Awards', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM dbo.Awards
                        WHERE IssuedByStaffId = {id}
                    END

                    IF OBJECT_ID('dbo.Competitions', 'U') IS NOT NULL
                    BEGIN
                        DELETE EL
                        FROM dbo.ExhibitionListings EL
                        INNER JOIN dbo.Submissions S ON EL.SubmissionId = S.SubmissionId
                        INNER JOIN dbo.Competitions C ON S.CompetitionId = C.CompetitionId
                        WHERE C.StaffId = {id}

                        DELETE EV
                        FROM dbo.Evaluations EV
                        INNER JOIN dbo.Submissions S ON EV.SubmissionId = S.SubmissionId
                        INNER JOIN dbo.Competitions C ON S.CompetitionId = C.CompetitionId
                        WHERE C.StaffId = {id}

                        DELETE S
                        FROM dbo.Submissions S
                        INNER JOIN dbo.Competitions C ON S.CompetitionId = C.CompetitionId
                        WHERE C.StaffId = {id}

                        DELETE FROM dbo.Competitions
                        WHERE StaffId = {id}
                    END

                    IF OBJECT_ID('dbo.Exhibitions', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM dbo.ExhibitionListings
                        WHERE ExhibitionId IN (SELECT ExhibitionId FROM dbo.Exhibitions WHERE StaffId = {id})

                        DELETE FROM dbo.Exhibitions
                        WHERE StaffId = {id}
                    END

                    DELETE FROM dbo.Staff
                    WHERE StaffId = {id}
                ");

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        DELETE FROM dbo.AspNetUserRoles WHERE UserId = {userId}
                        DELETE FROM dbo.AspNetUserClaims WHERE UserId = {userId}
                        DELETE FROM dbo.AspNetUserLogins WHERE UserId = {userId}
                        DELETE FROM dbo.AspNetUserTokens WHERE UserId = {userId}
                        DELETE FROM dbo.AspNetUsers WHERE Id = {userId}
                    ");
                }

                await transaction.CommitAsync();

                TempData["Success"] = "Staff deleted from database successfully.";
                return RedirectToAction(nameof(ManageStaff));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Delete failed: " + ex.Message;
                return RedirectToAction(nameof(ManageStaff));
            }
        }

        // =========================
        // MANAGE COMPETITIONS
        // =========================
        public async Task<IActionResult> ManageCompetitions()
        {
            var competitions = await _context.Competitions
                .Include(c => c.ManagedByStaff)
                .OrderByDescending(c => c.CompetitionId)
                .ToListAsync();

            return View(competitions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompetition(Competition model, IFormFile? ImageFile)
        {
            if (model.EndDate < model.StartDate)
            {
                TempData["Error"] = "End date cannot be before start date.";
                return RedirectToAction(nameof(ManageCompetitions));
            }

            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.IsActive)
                        ?? await _context.Staff.FirstOrDefaultAsync();

            if (staff == null)
            {
                TempData["Error"] = "Please create at least one staff member before creating competition.";
                return RedirectToAction(nameof(ManageCompetitions));
            }

            model.StaffId = staff.StaffId;
            model.CreatedAt = DateTime.Now;

            // Handle Image Upload
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

                model.ImagePath = $"/uploads/competitions/{uniqueFileName}";
            }

            _context.Competitions.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Competition created successfully.";
            return RedirectToAction(nameof(ManageCompetitions));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCompetition(Competition model, IFormFile? ImageFile)
        {
            var competition = await _context.Competitions
                .FirstOrDefaultAsync(c => c.CompetitionId == model.CompetitionId);

            if (competition == null)
            {
                TempData["Error"] = "Competition not found.";
                return RedirectToAction(nameof(ManageCompetitions));
            }

            if (model.EndDate < model.StartDate)
            {
                TempData["Error"] = "End date cannot be before start date.";
                return RedirectToAction(nameof(ManageCompetitions));
            }

            competition.Title = model.Title;
            competition.Description = model.Description;
            competition.StartDate = model.StartDate;
            competition.EndDate = model.EndDate;
            competition.Conditions = model.Conditions;
            competition.Status = model.Status;
            competition.UpdatedAt = DateTime.Now;

            // Handle Image Upload for Edit
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
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var competition = await _context.Competitions
                    .FirstOrDefaultAsync(c => c.CompetitionId == id);

                if (competition == null)
                {
                    TempData["Error"] = "Competition not found.";
                    return RedirectToAction(nameof(ManageCompetitions));
                }

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    IF OBJECT_ID('dbo.ExhibitionListings', 'U') IS NOT NULL
                    BEGIN
                        DELETE EL
                        FROM dbo.ExhibitionListings EL
                        INNER JOIN dbo.Submissions S ON EL.SubmissionId = S.SubmissionId
                        WHERE S.CompetitionId = {id}
                    END

                    IF OBJECT_ID('dbo.Evaluations', 'U') IS NOT NULL
                    BEGIN
                        DELETE EV
                        FROM dbo.Evaluations EV
                        INNER JOIN dbo.Submissions S ON EV.SubmissionId = S.SubmissionId
                        WHERE S.CompetitionId = {id}
                    END

                    IF OBJECT_ID('dbo.Awards', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM dbo.Awards
                        WHERE CompetitionId = {id}
                    END

                    DELETE FROM dbo.Submissions
                    WHERE CompetitionId = {id}

                    DELETE FROM dbo.Competitions
                    WHERE CompetitionId = {id}
                ");

                await transaction.CommitAsync();

                TempData["Success"] = "Competition deleted from database successfully.";
                return RedirectToAction(nameof(ManageCompetitions));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Delete failed: " + ex.Message;
                return RedirectToAction(nameof(ManageCompetitions));
            }
        }

        // =========================
        // MANAGE EXHIBITIONS
        // =========================
        public async Task<IActionResult> ManageExhibitions()
        {
            var exhibitions = await _context.Exhibitions
                .Include(e => e.ManagedByStaff)
                .OrderByDescending(e => e.ExhibitionId)
                .ToListAsync();

            ViewBag.TotalExhibitions = exhibitions.Count;
            ViewBag.OngoingExhibitions = exhibitions.Count(e => e.Status.ToString() == "Ongoing");
            ViewBag.UpcomingExhibitions = exhibitions.Count(e => e.Status.ToString() == "Upcoming");
            ViewBag.CompletedExhibitions = exhibitions.Count(e => e.Status.ToString() == "Concluded");

            return View(exhibitions);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExhibition(Exhibition exhibition, IFormFile? ImageFile)
        {
            if (exhibition.ExhibitionEndDate < exhibition.ExhibitionStartDate)
            {
                TempData["Error"] = "End date cannot be before start date.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.IsActive)
                        ?? await _context.Staff.FirstOrDefaultAsync();

            if (staff == null)
            {
                TempData["Error"] = "Please create at least one staff member before creating exhibition.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            exhibition.StaffId = staff.StaffId;
            exhibition.CreatedAt = DateTime.Now;

            // 🔥 IMAGE SAVE CODE - YEH ADD KIYA 🔥
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
        public async Task<IActionResult> UpdateExhibition(Exhibition model, IFormFile? ImageFile)
        {
            var exhibition = await _context.Exhibitions
                .FirstOrDefaultAsync(e => e.ExhibitionId == model.ExhibitionId);

            if (exhibition == null)
            {
                TempData["Error"] = "Exhibition not found.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            if (model.ExhibitionEndDate < model.ExhibitionStartDate)
            {
                TempData["Error"] = "End date cannot be before start date.";
                return RedirectToAction(nameof(ManageExhibitions));
            }

            exhibition.ExhibitionName = model.ExhibitionName;
            exhibition.Description = model.Description;
            exhibition.Venue = model.Venue;
            exhibition.City = model.City;
            exhibition.Country = model.Country;
            exhibition.ExhibitionStartDate = model.ExhibitionStartDate;
            exhibition.ExhibitionEndDate = model.ExhibitionEndDate;
            exhibition.Status = model.Status;
            exhibition.AdditionalNotes = model.AdditionalNotes;
            exhibition.UpdatedAt = DateTime.Now;

            // 🔥 IMAGE UPDATE CODE - YEH ADD KIYA 🔥
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

            // 🔥 YEH LINE PEHLE SE THI - CHANGE MAT KARO 🔥
            await _context.SaveChangesAsync();

            TempData["Success"] = "Exhibition updated successfully.";
            return RedirectToAction(nameof(ManageExhibitions));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExhibition(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var exhibition = await _context.Exhibitions
                    .FirstOrDefaultAsync(e => e.ExhibitionId == id);

                if (exhibition == null)
                {
                    TempData["Error"] = "Exhibition not found.";
                    return RedirectToAction(nameof(ManageExhibitions));
                }

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    IF OBJECT_ID('dbo.ExhibitionListings', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM dbo.ExhibitionListings
                        WHERE ExhibitionId = {id}
                    END

                    DELETE FROM dbo.Exhibitions
                    WHERE ExhibitionId = {id}
                ");

                await transaction.CommitAsync();

                TempData["Success"] = "Exhibition deleted from database successfully.";
                return RedirectToAction(nameof(ManageExhibitions));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Delete failed: " + ex.Message;
                return RedirectToAction(nameof(ManageExhibitions));
            }
        }

        // =========================
        // ANNOUNCEMENTS
        // =========================
        public IActionResult Announcements()
        {
            return View();
        }

        // =========================
        // REPORTS
        // =========================
        public async Task<IActionResult> Reports()
        {
            await LoadAdminCountsAsync();
            await LoadReportsDataAsync();
            return View();
        }

        // =========================
        // EXHIBITION STATUS
        // =========================
        public IActionResult ExhibitionStatus()
        {
            return View();
        }

        // =========================
        // HELPERS
        // =========================
        private async Task LoadAdminCountsAsync()
        {
            ViewBag.StudentCount = await _context.Students.CountAsync();
            ViewBag.StaffCount = await _context.Staff.CountAsync();
            ViewBag.CompetitionCount = await _context.Competitions.CountAsync();
            ViewBag.SubmissionCount = await _context.Submissions.CountAsync();
            ViewBag.AwardCount = 0;
        }

        private async Task LoadDashboardChartsAsync()
        {
            var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var monthlyCounts = new int[12];
            var currentYear = DateTime.Now.Year;

            var submissions = await _context.Submissions
                .Where(s => s.SubmittedAt.Year == currentYear)
                .Select(s => new { s.SubmittedAt.Month })
                .ToListAsync();

            foreach (var item in submissions)
            {
                if (item.Month >= 1 && item.Month <= 12)
                {
                    monthlyCounts[item.Month - 1]++;
                }
            }

            ViewBag.MonthlyLabels = JsonSerializer.Serialize(monthNames);
            ViewBag.MonthlySubmissions = JsonSerializer.Serialize(monthlyCounts);

            var competitionStatusLabels = new[] { "Ongoing", "Upcoming", "Concluded" };
            var competitionStatusValues = new[]
            {
                await _context.Competitions.CountAsync(c => c.Status.ToString() == "Ongoing"),
                await _context.Competitions.CountAsync(c => c.Status.ToString() == "Upcoming"),
                await _context.Competitions.CountAsync(c => c.Status.ToString() == "Concluded")
            };

            ViewBag.CompetitionStatusLabels = JsonSerializer.Serialize(competitionStatusLabels);
            ViewBag.CompetitionStatusValues = JsonSerializer.Serialize(competitionStatusValues);
        }

        private async Task LoadReportsDataAsync()
        {
            var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var monthlyCounts = new int[12];
            var currentYear = DateTime.Now.Year;

            var submissions = await _context.Submissions
                .Where(s => s.SubmittedAt.Year == currentYear)
                .Select(s => new { s.SubmittedAt.Month })
                .ToListAsync();

            foreach (var item in submissions)
            {
                if (item.Month >= 1 && item.Month <= 12)
                {
                    monthlyCounts[item.Month - 1]++;
                }
            }

            ViewBag.MonthlyLabels = monthNames;
            ViewBag.MonthlySubmissions = monthlyCounts;

            ViewBag.CompetitionStatusLabels = new[] { "Upcoming", "Ongoing", "Concluded" };
            ViewBag.CompetitionStatusCounts = new[]
            {
                await _context.Competitions.CountAsync(c => c.Status.ToString() == "Upcoming"),
                await _context.Competitions.CountAsync(c => c.Status.ToString() == "Ongoing"),
                await _context.Competitions.CountAsync(c => c.Status.ToString() == "Concluded")
            };

            var studentPrograms = await _context.Students
                .GroupBy(s => s.Course)
                .Select(g => new { Course = g.Key ?? "Not Set", Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(6)
                .ToListAsync();

            ViewBag.StudentProgramLabels = studentPrograms.Select(x => x.Course).ToArray();
            ViewBag.StudentProgramCounts = studentPrograms.Select(x => x.Count).ToArray();

            var submissionStatus = await _context.Submissions
                .GroupBy(s => s.Grade)
                .Select(g => new { Grade = g.Key.ToString(), Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            ViewBag.SubmissionStatusLabels = submissionStatus.Select(x => x.Grade).ToArray();
            ViewBag.SubmissionStatusCounts = submissionStatus.Select(x => x.Count).ToArray();

            ViewBag.AwardTypeLabels = new[] { "Awards" };
            ViewBag.AwardTypeCounts = new[] { 0 };
        }
    }
}