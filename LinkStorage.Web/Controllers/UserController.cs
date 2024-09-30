using LinkStorage.Business.Abscract;
using LinkStorage.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkStorage.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
            if (ModelState.IsValid)
            {
                user.UserTypeId = 2;
                _userService.Add(user);

                // E-posta gönderme işlemi vb.
                TempData["success"] = "Kayıt başarılı!";
                return RedirectToAction("Index", "Home");
            }

            return View(user); // Hatalı durumda kullanıcı bilgilerini geri gönder
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
                AppUser appUser = _userService.CheckLogin(user); // Kullanıcıyı kontrol et
                if (appUser != null) // Kullanıcı bulunduysa
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Email, appUser.Email));
                    claims.Add(new Claim("UserTypeId", appUser.UserTypeId.ToString()));
                    claims.Add(new Claim(ClaimTypes.Role, appUser.UserType.Name));

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

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult GetAll()
        {

            return Json(new { data = _userService.GetAll() });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(AppUser user)
        {
            if (ModelState.IsValid)
            {
                _userService.Add(user); // Kullanıcıyı kaydet
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
                return View(existingUser); // Hata durumunda mevcut kullanıcıyı geri döndür
            }

            var existingUserInDb = _userService.GetById(user.Id);

            if (string.IsNullOrEmpty(user.Password))
            {
                user.Password = existingUserInDb.Password; // Mevcut şifreyi koru
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

            [AllowAnonymous]
        public IActionResult Profile()
        {
            var user = _userService.Profile();
            return View(user);
        }
    }
}
