using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserManagementAPI.Model;

namespace UserManagementAPI.Data
{
    public class PersonSeeder
    {
        public static async Task SeedAsync(UserManager<Person> userManager)
        {
            for (int i = 1; i <= 30; i++)
            {
                var email = $"user{i}@example.com";
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new Person
                    {
                        UserName = email,
                        Email = email,
                        FirstName = $"First{i}",
                        LastName = $"Last{i}",
                        RegisterDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-i)),
                        IsActive = true
                    };
                    await userManager.CreateAsync(user, $"Password{i}!");
                }
            }
        }
    }
}
