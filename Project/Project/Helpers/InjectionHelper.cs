using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Entities;

namespace Project.Helpers
{
    public static class InjectionHelper
    {
        public static void AddAppServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
            }).AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            builder.Services.AddFluentValidationAutoValidation().
                AddFluentValidationClientsideAdapters().
                AddValidatorsFromAssemblyContaining<Program>();
        }
    }
}
