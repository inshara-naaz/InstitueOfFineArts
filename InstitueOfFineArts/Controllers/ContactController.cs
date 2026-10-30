using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstituteOfFineArts.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ContactMessage model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedAt = DateTime.Now;
                    model.IsRead = false;

                    _context.ContactMessages.Add(model);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Message sent successfully! We'll get back to you soon.";
                    return RedirectToAction("Contact", "Home");
                }
                else
                {
                    // Debug: ModelState errors
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    TempData["Error"] = "Validation failed: " + string.Join(", ", errors);
                    return RedirectToAction("Contact", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Database error: " + ex.Message;
                return RedirectToAction("Contact", "Home");
            }
        }
    }
}