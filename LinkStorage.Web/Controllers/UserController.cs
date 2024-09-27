using LinkStorage.Business.Abscract;
using LinkStorage.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkStorage.Web.Controllers
{
    [Authorize] // Tüm controller için yetkilendirme
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
            var users = _userService.GetAll();
            return View(users);
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
            // Model state kontrolü
            if (ModelState.IsValid)
            {
                // Kullanıcıyı kontrol et
                AppUser appUser = _userService.CheckLogin(user);
                if (appUser != null)
                {
                    // Claim'leri oluştur
                    List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new Claim(ClaimTypes.Email, appUser.Email),
                new Claim(ClaimTypes.Role, appUser.UserType.Name)
            };

                    // Claim kimliğini oluştur
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Oturumu kapat ve yeni oturum aç
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = true });

                    // Kullanıcı türüne göre yönlendirme yap
                    if (appUser.UserType.Name == "Admin")
                    {
                        TempData["success"] = "Hoş geldiniz";
                        return RedirectToAction("Index", "User");
                    }

                    TempData["success"] = "Hoş geldiniz";
                    return RedirectToAction("Index", "Home");
                }
            }

            // Hatalı giriş durumunda mesajı ayarla
            TempData["error"] = "Kullanıcı adı ya da şifreniz yanlıştır!";
            return RedirectToAction("Login"); // Hatalı giriş
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
            if (_userService.Add(user) != null)
                return Ok(new { result = true, message = "Kullanıcı Başarılı Bir Şekilde Oluşturulmuştur.", userId = user.Id });

            return Ok(new { result = false, message = "Kullanıcı oluşturulamadı." });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(AppUser user)
        {
            if (ModelState.IsValid)
            {
                await _userService.AddAsync(user);
                return RedirectToAction("Index");
            }
            return View(user);
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

            return Ok(_userService.Update(user));
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

        [AllowAnonymous] // Tüm kullanıcılara açık
        [HttpPost]
        public async Task<IActionResult> Password(string email)
        {
            // Şifre yenileme isteği
            if (await _userService.NewUserPassword(email))
            {
                TempData["success"] = "Şifreniz mail adresinize gönderilmiştir.";
                return RedirectToAction("Index"); // Başarılı ise ana sayfaya yönlendir
            }
            else
            {
                TempData["error"] = "Şifre yenileme işlemi başarısızdır.";
                return RedirectToAction("Password"); // Hata durumunda tekrar form sayfasına yönlendir
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
