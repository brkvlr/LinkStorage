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
            return _repository.GetAll().Include(p => p.UserType).Where(p => (p.Email == user.UserName || p.UserName == user.UserName) && p.Password == user.Password).FirstOrDefault();

        }

        public async Task<AppUser?> AddAsync(AppUser user)
        {
            return await Task.Run(() => _repository.Add(user));
        }

        public async Task<AppUser?> Register(AppUser user)
        {
            _repository.Add(user);
            return user;

        }

        public bool IsEmailUnique(string email)
        {
            return _repository.GetFirstOrDefault(u => u.Email == email) == null;
        }

        public bool IsUsernameUnique(string username)
        {
            return _repository.GetFirstOrDefault(u => u.UserName == username) == null;
        }


        public IQueryable<AppUser> GetAll()
        {
            return _repository.GetAll()
                .Select(x => new AppUser
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    Password = x.Password,
                    UserType = x.UserType,
                    IsDeleted = x.IsDeleted
                });
        }

        public AppUser Profile()
        {
            throw new NotImplementedException("Profil bilgilerini almak için mevcut kullanıcı tanımlanmalıdır.");
        }

        public async Task<bool> NewUserPassword(string mail)
        {
            var appUser = GetFirstOrDefault(u => u.Email == mail);
            if (appUser is not null)
            {
                string newPassword = Helper.RandomPassword(); 
                appUser.Password = newPassword; 
                string message = "Merhaba,<br>" +
                    "Şifreniz yenilenmiştir.<br>" +
                   $"Şifreniz: {newPassword}";
                _repository.Update(appUser);

                if (await HelperMail.SendMail(appUser.UserName, "Şifre Yenileme", message))
                {

                    return true;

                }

                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
