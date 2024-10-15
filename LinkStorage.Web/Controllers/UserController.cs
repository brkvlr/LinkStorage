using LinkStorage.Business.Abscract;
using LinkStorage.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LinkStorage.Business.Concrete;

namespace LinkStorage.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserTypeService _userTypeService;

        public UserController(IUserService userService, IUserTypeService userTypeService)
        {
            _userService = userService;
            _userTypeService = userTypeService;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = _userService.GetAll();
            return View(users);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View(new AppUser());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(AppUser user)
        {
            if (!_userService.IsEmailUnique(user.Email))
            {
                ModelState.AddModelError("Email", "Bu e-posta zaten kullanılıyor.");
                return View(user);
            }

            if (!_userService.IsUsernameUnique(user.UserName))
            {
                ModelState.AddModelError("UserName", "Bu kullanıcı adı zaten kullanılıyor.");
                return View(user);
            }
            user.UserTypeId = 2;

            var result = await _userService.Register(user);

            if (result != null)
            {
                TempData["success"] = "Kayıt başarılı!";
                return RedirectToAction("Index", "Home");
            }

            TempData["error"] = "Kayıt sırasında bir hata oluştu.";
            return View(user);
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(AppUser user)
        {

            if (user != null)
            {
                AppUser appUser = _userService.CheckLogin(user); 
                if (appUser != null) 
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Email, appUser.Email));
                    claims.Add(new Claim("UserTypeId", appUser.UserTypeId.ToString()));
                    claims.Add(new Claim(ClaimTypes.Role, appUser.UserType.Name));
                    claims.Add(new Claim(ClaimTypes.Name, appUser.UserName));

                    var identity = new ClaimsIdentity(claims, "Login");
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(principal);
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = true });

                    if (appUser.UserType.Name == "Admin")
                    {
                        TempData["success"] = $"Hoşgeldiniz {appUser.UserName}";
                        return RedirectToAction("Index", "User");
                    }
                    if (appUser.UserType.Name == "User")
                    {
                        TempData["success"] = $"Hoşgeldiniz {appUser.UserName}";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        TempData["success"] = $"Hoşgeldiniz {appUser.UserName}";

                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["error"] = "Kullanıcı Adı ya da Şifreniz Yanlış!";
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var users = _userService.GetAll().ToList();
                var userTypes = _userTypeService.GetAllType().ToDictionary(ut => ut.Id, ut => ut.Name);

                var result = users.Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.UserTypeId,
                    UserTypeName = userTypes.ContainsKey(u.UserTypeId) ? userTypes[u.UserTypeId] : "Bilinmeyen"
                });

                return Json(new { data = result });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(AppUser user)
        {
            if (ModelState.IsValid)
            {
                _userService.Add(user); 
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = _userService.GetById(id);
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(AppUser user)
        {
            if (!ModelState.IsValid)
            {
                var existingUser = _userService.GetById(user.Id);
                return View(existingUser); 
            }

            var existingUserInDb = _userService.GetById(user.Id);

            if (string.IsNullOrEmpty(user.Password))
            {
                user.Password = existingUserInDb.Password; 
            }

             _userService.Update(user);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult HardDelete(int id)
        {
            _userService.HardDelete(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(AppUser user)
        {

            return Ok(_userService.Delete(user.Id));
        }

        [HttpPost]
        public IActionResult Update(AppUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { result = false, message = "Geçersiz kullanıcı verisi" });
            }

            var result = _userService.Update(user);

            if (result != null)
            {
                return Ok(new { result = true, message = "Kullanıcı başarıyla güncellendi" });
            }

            return BadRequest(new { result = false, message = "Kullanıcı güncellenemedi" });
        }

        [HttpPost]
        public IActionResult GetById(int id)
        {
            return Json(_userService.GetById(id));
        }

        public IActionResult Profile()
        {
            return View(_userService.Profile());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(AppUser user)
        {
            return RedirectToAction("Profile", "User");
        }

        [AllowAnonymous]
        public IActionResult Password()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Password(string email)
        {
            if (await _userService.NewUserPassword(email))
            {
                TempData["success"] = "Şifreniz Mail Adresinize Gönderilmiştir.";
                return RedirectToAction("index");

            }
            else
            {
                TempData["error"] = "Şifre Yenileme İşlemi Başarısızdır.";
                return RedirectToAction("password");
            }
        }
    }
}
