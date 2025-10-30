using BeerCraftMVC.Models.Entities;
using BeerCraftMVC.Models.ViewModels.Account;
using BeerCraftMVC.Models.ViewModels.Inventory;
using BeerCraftMVC.Models.ViewModels.Profile;
using BeerCraftMVC.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;

namespace BeerCraftMVC.Controllers
{
    /// <summary>
    /// Контролер за управление на Login/Register страницата със съответната логика. Показва страницата Access.cshtml
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            // Dependency Injection на UserRepository, за задачи като "Дай ми потребител по име" и "Запази този потребител в базата".
        }

        [HttpGet]
        public IActionResult Access(string panel = "", string returnUrl = null)
        {
            //първо отваряне на страницата, по подразбиране се отваря Login панела
            ViewBag.ReturnUrl = returnUrl;

            string activePanel = string.IsNullOrEmpty(panel) ? "login" : panel.ToLower();
            ViewBag.ActivePanel = activePanel;

           var model = new LoginRegisterViewModel
            {
                Login = new LoginViewModel(),
                Register = new RegisterViewModel()
            };
            return View(model);

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind(Prefix = "Register")] RegisterViewModel registerModel)
        {
           

            if (!ModelState.IsValid)
            {
                var wrapperModel = new LoginRegisterViewModel
                {
                    Register = registerModel,
                    Login = new LoginViewModel()
                };
                ViewBag.ActivePanel = "register";
                return View("Access", wrapperModel);
                //връща потребителя към същата страница, ако има грешки в модела (неспазени Required полета и т.н.)
            }

            //проверка дали потребителското име/имейлът вече съществуват със съответния error handling message
            if (await _userRepository.GetByUsernameAsync(registerModel.Username) != null)
            {
                ModelState.AddModelError("Register.Username", "Username is already taken.");
                var wrapperModel = new LoginRegisterViewModel
                {
                    Register = registerModel,
                    Login = new LoginViewModel()
                };
                ViewBag.ActivePanel = "register";
                return View("Access", wrapperModel);
            }

            if (await _userRepository.GetByEmailAsync(registerModel.Email) != null)
            {
                ModelState.AddModelError("Register.Email", "Email is already in use.");
                var wrapperModel = new LoginRegisterViewModel
                {
                    Register = registerModel,
                    Login = new LoginViewModel()
                };
                ViewBag.ActivePanel = "register";
                return View("Access", wrapperModel);
            }

            //при минаване на всички проверки се създава нов потребител и се записва в базата
            var user = new User
            {
                Username = registerModel.Username,
                Name = registerModel.Name,
                Email = registerModel.Email,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(registerModel.Password),//паролата се хешира за сигурност
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user); //запазване в базата данни
            await SignInUser(user); //автоматично логване след регистрация
            return RedirectToAction("Index", "Home"); //user-ът се пренасочва към началната страница
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind(Prefix = "Login")] LoginViewModel loginModel, string returnUrl = null) 
        { 
            if (!ModelState.IsValid)
            {
                var wrapperModel = new LoginRegisterViewModel
                {
                    Login = loginModel,
                    Register = new RegisterViewModel()
                };
                ViewBag.ActivePanel = "login";
                return View("Access", wrapperModel);
            }

            var user = await _userRepository.GetByUsernameAsync(loginModel.Username);

            //проверка дали потребителското име съществува и дали паролата съвпада
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.HashedPassword))
            {
                var wrapperModel = new LoginRegisterViewModel
                {
                    Login = loginModel,
                    Register = new RegisterViewModel()
                };
                ViewBag.ActivePanel = "login";
                return View("Access", wrapperModel);
            }

            await SignInUser(user); //login на потребителя

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //при натискане на Logout бутона се изтриват всички cookies и сесии и се връща към началната страница
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(User user)
        {
            //при вписване се създават claims (идентификационни данни) за потребителя и се създава сесия
            var claims = new List<Claim>
              {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
              };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); 
            //създаване на claims identity с помощта на cookies

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, //"запомни ме..."
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)//"...за следващите 7 дни"
            };
            await HttpContext.SignInAsync(
            //казва на ASP.NET Core да създаде сесия за вписания потребител и я изпраща към браузъра му
                      CookieAuthenticationDefaults.AuthenticationScheme,
              new ClaimsPrincipal(claimsIdentity),
              authProperties);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); //взима ID-то на вписания потребител от claims
            if(string.IsNullOrEmpty(userIdString)|| !int.TryParse(userIdString, out int userId))
            {
                
                return RedirectToAction("Access", "Account");
            }
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return RedirectToAction("Access", "Account");
                }
            var viewModel = new ProfileViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                InventoryItems = user.Inventory 
         .OrderBy(inv => inv.Ingredient.Name) 
         .Select(inv => new InventoryItemViewModel
         {
             IngredientId = inv.IngredientId,
             IngredientName = inv.Ingredient.Name, 
             Quantity = inv.Quantity
         }).ToList()
            };
            //попълва информацията за потребителя и неговия инвентар
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel viewModel)
        {
            // Проверяваме дали ID-то от формата съвпада с логнатия потребител 
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int currentUserId) || viewModel.UserId != currentUserId)
            {
                return Forbid(); 
            }

            if (ModelState.IsValid)
            {
                
                var userToUpdate = await _userRepository.GetByIdAsync(viewModel.UserId);
                if (userToUpdate == null)
                {
                    return NotFound();
                }
                userToUpdate.Name = viewModel.Name;

                try
                {
                    await _userRepository.UpdateAsync(userToUpdate);
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("Profile"); 
                }
                catch (DbUpdateConcurrencyException) 
                {
                  
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, see system administrator.");
                }
            }

          
            viewModel.Username = User.Identity.Name;
            var originalUser = await _userRepository.GetByIdAsync(viewModel.UserId);
            if (originalUser != null) viewModel.Email = originalUser.Email;


            return View(viewModel); 
        }
    }
}
