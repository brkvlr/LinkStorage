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
            if (user != null)
            {
                AppUser appUser = _userService.CheckLogin(user); // Kullanıcıyı kontrol et
                if (appUser == null) // Kullanıcı bulunduysa
                {
                    List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new Claim(ClaimTypes.Name, appUser.UserName), // Kullanıcı adını al
                new Claim(ClaimTypes.Email, appUser.Email),   // E-posta adresini al
                new Claim(ClaimTypes.Role, appUser.UserType.Name) // Rolü al
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = true });

                    // Yönlendirme işlemleri
                }
                else
                {
                    TempData["error"] = "Kullanıcı Adı ya da Şifreniz Yanlıştır!";
                }
            }
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
