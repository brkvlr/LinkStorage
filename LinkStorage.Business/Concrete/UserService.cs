using LinkStorage.Utility;
using LinkStorage.Business.Abscract;
using LinkStorage.Business.Shared.Concrete;
using LinkStorage.Models;
using LinkStorage.Repository.Shared.Abstract;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace LinkStorage.Business.Concrete
{
    public class UserService : Service<AppUser>, IUserService
    {
        private readonly IRepository<AppUser> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IRepository<AppUser> repo, IHttpContextAccessor httpContextAccessor) : base(repo)
        {
            _repository = repo;
            _httpContextAccessor = httpContextAccessor;
        }


        public AppUser CheckLogin(AppUser user)
        {
            return _repository.GetAll().Include(p => p.UserType).FirstOrDefault(p => (p.Email == user.UserName || p.UserName == user.UserName) && p.Password == user.Password);
        }

        public async Task<AppUser?> AddAsync(AppUser user)
        {
            return await Task.Run(() => _repository.Add(user));
        }

        public IQueryable<AppUser> GetAll(string emailOrUsername, string password)
        {
            return _repository.GetAll(u => u.IsDeleted == false && (u.Email == emailOrUsername || u.UserName == emailOrUsername) && u.Password == password)
                .Select(x => new AppUser
                {
                    Id = x.Id,
                    Email = x.Email,
                    Password = x.Password,
                    UserType = x.UserType
                });
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
