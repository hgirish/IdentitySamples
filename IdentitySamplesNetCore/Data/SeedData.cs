using IdentitySamplesNetCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitySamplesNetCore.Data
{
    public class SeedData
    {
       
        public static async Task EnsureSeedDataAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var configuration = scope.ServiceProvider.GetService<IConfiguration>();
                var seedDataFile = configuration["IdentityCore:SeedDataJsonFile"];
                IList<UserDetailInfo> userDetails = GetUserDetails(seedDataFile);

                context.Database.Migrate();

                var usrMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                foreach (var detail in userDetails)
                {
                    var user  = await usrMgr.FindByNameAsync(detail.UserName);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = detail.UserName,
                            Email = detail.Email,
                            EmailConfirmed = true                           
                        };
                        var result = await usrMgr.CreateAsync(user, detail.Password);
                        if (!result.Succeeded)
                        {
                            Console.WriteLine($"{detail.UserName} creation failed");
                            Console.WriteLine(result.Errors.First().Description);
                            continue;
                        }
                        result = await usrMgr.AddClaimsAsync(user, new Claim[]
                        {
                            new Claim(ClaimTypes.Email, detail.Email)
                        });
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine($"{user.UserName} created.");
                        foreach (var role in detail.Roles)
                        {
                            var roleExists = await roleMgr.RoleExistsAsync(role);
                            if (!roleExists)
                            {
                                result = await roleMgr.CreateAsync(new IdentityRole(role));
                                if (!result.Succeeded)
                                {
                                    throw new Exception(result.Errors.First().Description);
                                }
                            }
                            
                        }
                     result = await  usrMgr.AddToRolesAsync(user, detail.Roles);
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("Roles added");
                    }
                    else
                    {
                        //Console.WriteLine($"{detail.UserName} already exists");
                    }
                }

            }
        }

        private static IList<UserDetailInfo> GetUserDetails(string seedDataFile)
        {
            var content = System.IO.File.ReadAllText(seedDataFile);
          var userDetails =  JsonConvert.DeserializeObject<List<UserDetailInfo>>(content);
            return userDetails;
        }

        public class UserDetailInfo
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string[] Roles { get; set; }
        }
    }
}
