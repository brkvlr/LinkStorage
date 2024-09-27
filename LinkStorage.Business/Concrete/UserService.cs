using LinkStorage.Utility;
using LinkStorage.Business.Abscract;
using LinkStorage.Business.Shared.Concrete;
using LinkStorage.Models;
using LinkStorage.Repository.Shared.Abstract;
using System.Linq;
using System.Threading.Tasks;

namespace LinkStorage.Business.Concrete
{
    public class UserService : Service<AppUser>, IUserService
    {
        private readonly IRepository<AppUser> _repository;

        public UserService(IRepository<AppUser> repo) : base(repo)
        {
            _repository = repo;
        }

        public AppUser CheckLogin(AppUser user)
        {
            // Giriş yapmış kullanıcıyı kontrol et
            return _repository.GetFirstOrDefault(u => (u.UserName == user.UserName || u.Email == user.Email) && u.Password == user.Password);
        }

        public async Task<AppUser?> AddAsync(AppUser user)
        {
            return await Task.Run(() => _repository.Add(user));
        }

        public AppUser Profile()
        {
            throw new NotImplementedException("Profil bilgilerini almak için mevcut kullanıcı tanımlanmalıdır.");
        }

        public async Task<bool> NewUserPassword(string mail)
        {
            var appUser = _repository.GetFirstOrDefault(u => u.Email == mail);
            if (appUser != null)
            {
                string newPassword = Helper.RandomPassword(); 
                appUser.Password = newPassword; 
                string message = "Merhaba,<br>" +
                                 "Şifreniz yenilenmiştir.<br>" +
                                 $"Şifreniz: {newPassword}";
                _repository.Update(appUser);

                return await HelperMail.SendMail(appUser.Email, "Şifre Yenileme", message);
            }
            else
            {
                return false;
            }
        }
    }
}
