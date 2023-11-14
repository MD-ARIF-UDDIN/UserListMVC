using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserListMVC.Models;

namespace UserListMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public User User { get; set; }

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            User = new User();
            if (id == null)
            {
                return View(User);
            }

            User = _db.Users.FirstOrDefault(u => u.Id == id);
            if (User == null)
            {
                return NotFound();
            }
            return View(User);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (User.Id == 0)
                {
                    _db.Users.Add(User);
                }
                else
                {
                    _db.Users.Update(User);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(User);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Users.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var userFromDb = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (userFromDb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _db.Users.Remove(userFromDb);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful" });
        }







    }
}
