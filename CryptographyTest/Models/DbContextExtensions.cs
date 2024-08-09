using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptographyTest.Models
{
    public static class DbContextExtensions
    {
        public static async Task EnsureSeedDataAsync(this DetectiveApiDbContext context, UserManager<User> userManager)
        {
            if (!context.ContactPersons.Any())
            {
                context.ContactPersons.AddRange(
                    new ContactPerson { Name = "Alice Johnson", Phone = "555-0100", Address = "123 Elm St", City = "Springfield", Notes = "Frequent updates needed" },
                    new ContactPerson { Name = "Bob Smith", Phone = "555-0101", Address = "456 Oak St", City = "Shelbyville", Notes = "Prefers email contact" },
                    new ContactPerson { Name = "Carol White", Phone = "555-0102", Address = "789 Pine St", City = "Capital City", Notes = "Evening calls only" }
                );
                await context.SaveChangesAsync();
            }

            if (!userManager.Users.Any())
            {
                // Seed a detective user
                var detectiveUser = new User
                {
                    UserName = "Detective1",
                    Email = "detective1@example.com",
                    Role = UserRole.Detective,
                    BadgeNumber = "11111",
                };
                await userManager.CreateAsync(detectiveUser, "Password123!");

                // Seed a supervisor user
                var supervisorUser = new User
                {
                    UserName = "Supervisor1",
                    Email = "supervisor1@example.com",
                    Role = UserRole.Supervisor,
                    BadgeNumber = "55555",
                };
                await userManager.CreateAsync(supervisorUser, "Password123!");

                // Save changes
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User { UserName = "john.doe@example.com", Email = "john.doe@example.com", Role = UserRole.Detective },
                    new User { UserName = "jane.smith@example.com", Email = "jane.smith@example.com", Role = UserRole.Detective },
                    new User { UserName = "anne.clark@example.com", Email = "anne.clark@example.com", Role = UserRole.Supervisor }
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "password"); // Replace "password" with your desired password
                }
            }

            if (!context.Cases.Any())
            {
                var detectives = context.Users.Where(u => u.Role == UserRole.Detective).ToList();
                var supervisor = context.Users.FirstOrDefault(u => u.Role == UserRole.Supervisor);
                var tips = context.Tips.ToList();

                var cases = new List<Case>
                {
                    new Case
                    {
                        Name = "High Profile Burglary",
                        SerialNumber = "CP-112233",
                        Notes = new List<string?> { "Urgent", "High value items stolen" },
                        Description = "Investigation of a high profile burglary in downtown district.",
                        Detective = detectives.FirstOrDefault(),
                        Supervisor = supervisor,
                        Status = CaseStatus.Active,
                        Tips = tips
                    },
                    new Case
                    {
                        Name = "Cyber Fraud Investigation",
                        SerialNumber = "CP-112244",
                        Notes = new List<string?> { "Complex case involving international fraud" },
                        Description = "Investigation of a complex cyber fraud involving multiple international banks.",
                        Detective = detectives.Skip(1).FirstOrDefault(),  // Assigning to the next detective
                        Supervisor = supervisor,
                        Status = CaseStatus.Active,
                        Tips = tips
                    },
                    new Case
                    {
                        Name = "Missing Person",
                        SerialNumber = "CP-112255",
                        Notes = new List<string?> { "Possible kidnapping" },
                        Description = "Investigation of a missing person case with possible kidnapping.",
                        Detective = detectives.FirstOrDefault(),  // Assigning to the first detective again
                        Supervisor = supervisor,
                        Status = CaseStatus.Active,
                        Tips = tips
                    }
                };

                context.Cases.AddRange(cases);
                await context.SaveChangesAsync();
            }

            if (!context.Tips.Any())
            {
                var firstContact = context.ContactPersons.ToList();
                context.Tips.AddRange(
                    new Tip { Description = "Anonymous tip about suspicious activity.", ContactPerson = firstContact[0], LogDate = DateTime.Now, Level = TipLevel.Public },
                    new Tip { Description = "Reported sighting of missing person.", ContactPerson = firstContact[1], LogDate = DateTime.Now.AddDays(-1), Level = TipLevel.Classified },
                    new Tip { Description = "Possible fraud activity at local bank.", ContactPerson = firstContact[1], LogDate = DateTime.Now.AddDays(-2), Level = TipLevel.Public },
                    new Tip { Description = "Unusual car parked near crime scene.", ContactPerson = firstContact[2], LogDate = DateTime.Now.AddDays(-3), Level = TipLevel.Classified },
                    new Tip { Description = "Important evidence found in nearby trash bin.", ContactPerson = firstContact[2], LogDate = DateTime.Now.AddDays(-4), Level = TipLevel.Public }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}