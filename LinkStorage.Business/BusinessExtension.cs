using LinkStorage.Business.Shared.Abstract;
using LinkStorage.Business.Shared.Concrete;
using LinkStorage.Repository.Shared.Abstract;
using LinkStorage.Repository.Shared.Concrete;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business
{
    public static class BusinessExtension
    {
        public static void AddBusinessDI(this IServiceCollection services)
        {
            services.AddScoped(typeof (IService<>), typeof (Service<>));
        }

        public static void AddRepositoryDI(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}
