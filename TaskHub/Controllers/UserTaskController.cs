using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskHub.Data;
using TaskHub.Models;

namespace TaskHub.Controllers
{
    public class UserTaskController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserTaskController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserTasks/Create
        public IActionResult Create()
        {
            //ViewData is a dictionary property that can be used to pass additonal data items from controllers to view
            //We pass a list of users to have a dropdown in the view to whom the task will be assigned
            ViewData["AssignedToId"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: UserTasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserTask userTask)
        {
            if (ModelState.IsValid)
            {
                userTask.TaskId = Guid.NewGuid();
                userTask.Status = "Undone";
                userTask.CreatedDate = DateTime.Now;
                _context.Add(userTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssignedToId"] = new SelectList(_context.Users, "Id", "FullName");
            return View(userTask);
        }

        // GET: UserTasks
        public async Task<IActionResult> Index()
        {
            //Gets the current user via the usermanager
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            //Checks the users role if Admin
            if (User.IsInRole("Admin"))
            {
                //Loads all the UserTask
                //.Include loads all the link the related data
                var appDbContext = _context.UserTasks.Include(u => u.AssignedTo);

                return View(await appDbContext.ToListAsync());
            }
            //If not Admin 
            else
            {
                //Loads all the UserTask that is assigned to the logged user.
                var appDbContext = _context.UserTasks.Include(u => u.AssignedTo).Where(x => x.AssignedToId == user.Id);

                return View(await appDbContext.ToListAsync());
            }
        }

        // GET: UserTasks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.UserTasks == null)
            {
                return NotFound();
            }

            var userTask = await _context.UserTasks.Include(x => x.AssignedTo).FirstOrDefaultAsync(x => x.TaskId == id);
            if (userTask == null)
            {
                return NotFound();
            }
            ViewData["AssignedToId"] = new SelectList(_context.Users, "Id", "FullName", userTask.AssignedToId);
            return View(userTask);
        }

        // POST: UserTasks/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UserTask userTask)
        {
            if (id != userTask.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                _context.Update(userTask);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["AssignedToId"] = new SelectList(_context.Users, "Id", "FullName", userTask.AssignedToId);
            return View(userTask);
        }

        // GET: UserTasks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.UserTasks == null)
            {
                return NotFound();
            }

            var userTask = await _context.UserTasks
                .Include(u => u.AssignedTo)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (userTask == null)
            {
                return NotFound();
            }

            return View(userTask);
        }

        // POST: UserTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.UserTasks == null)
            {
                return Problem("Entity set 'AppDbContext.UserTasks'  is null.");
            }
            var userTask = await _context.UserTasks.FindAsync(id);
            if (userTask != null)
            {
                _context.UserTasks.Remove(userTask);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
