using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using projectMvc.Models.models;
using projectMvc.Models.viewmodels;

namespace projectMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewmodel registerViewmodel)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = registerViewmodel.Email,
                    Email = registerViewmodel.Email,
                    FullName = registerViewmodel.Name
                };

                IdentityResult result = await _userManager.CreateAsync(user, registerViewmodel.Password);

                if (result.Succeeded)
                {
                    // تحقق من وجود الرولز (Admin, Customer) ولو مش موجودة أنشئها
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!await _roleManager.RoleExistsAsync("Customer"))
                        await _roleManager.CreateAsync(new IdentityRole("Customer"));

                    // تعيين رول افتراضي للمستخدم الجديد
                    await _userManager.AddToRoleAsync(user, "Customer");

                    // بعد التسجيل الناجح، يروح صفحة اللوجن
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(registerViewmodel);
        }

        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Loginviewmodel loginViewmodel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginViewmodel.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewmodel.Password, loginViewmodel.RememberMe, false);

                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                   
                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Menu", "Admin");
                        }
                      
                            return RedirectToAction("Index", "User");
                       
                       
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }

            return View(loginViewmodel);
        }
    }
}
