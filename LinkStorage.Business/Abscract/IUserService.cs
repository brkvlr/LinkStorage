using LinkStorage.Business.Shared.Abstract;
using LinkStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business.Abscract
{
    public interface IUserService : IService<AppUser>
    {
        AppUser CheckLogin(AppUser user);
        Task<AppUser?> AddAsync(AppUser user);
        AppUser Profile();
        Task<bool> NewUserPassword(string mail);
        Task<AppUser?> Register(AppUser user);
        bool IsEmailUnique(string email);
        bool IsUsernameUnique(string username);
    }
}
