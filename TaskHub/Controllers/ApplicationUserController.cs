using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskHub.Models;
using TaskHub.Models.ModelViews;

namespace TaskHub.Controllers
{
    public class ApplicationUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LogInView model)
        {
            if (ModelState.IsValid)
            {
                //Uses the SignInManager to sign in if the user passes the authentication.
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid Login attempt");

            }
            return View(model);
        }

        // GET: Users/Create
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUserView userView)
        {
            //Checks if there are roles already in the DB if there is not, this will create two roles Admin and User
            if (_roleManager.Roles.Count() == 0)
            {
                IdentityRole identityRoleAdmin = new IdentityRole()
                {
                    Name = "Admin"
                };

                IdentityRole identityRoleUser = new IdentityRole()
                {
                    Name = "User"
                };

                await _roleManager.CreateAsync(identityRoleAdmin);

                await _roleManager.CreateAsync(identityRoleUser);
            }

            //Checks if the inputs are valid based on the annotations of the model.
            if (ModelState.IsValid)
            {
                //Creates an instance of the ApplicationUser to extact needed data from the model view.
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = userView.Email,
                    FirstName = userView.FirstName,
                    LastName = userView.LastName,
                    UserName = userView.Email
                };

                //Creates the user with the use of the userManager object.
                var result = await _userManager.CreateAsync(user, userView.Password);
                //Assigns the selected role.
                await _userManager.AddToRoleAsync(user, userView.Role);

                if (result.Succeeded)
                {
                    //This will check if there is a user currently logged in it will redirect to login but if there is 
                    //a login session it will go to the index
                    if (User.Identity.IsAuthenticated == true)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction(nameof(Login));
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(userView);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            //Maps the returned users to the ApplicationViewModel before it is passed to the view.
            List<ApplicationUserView> usersView = users.Select(u => new ApplicationUserView
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Id = u.Id
            }).ToList();

            return View(usersView);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "ApplicationUser");
        }

    }
}
