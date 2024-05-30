using CryptographyTest.Services;
using System.Collections.Generic;
using System.Linq;

namespace CryptographyTest.Models
{
    public static class DbContextExtensions
    {
        public static void EnsureSeedData(this DetectiveApiDbContext context)
        {
            // Check if there are any entries already in the database for the Cases table
            
            if (!context.ContactPersons.Any())
            {
                context.ContactPersons.AddRange(
                    new ContactPerson { Name = "Alice Johnson", Phone = "555-0100", Address = "123 Elm St", City = "Springfield", Notes = "Frequent updates needed" },
                    new ContactPerson { Name = "Bob Smith", Phone = "555-0101", Address = "456 Oak St", City = "Shelbyville", Notes = "Prefers email contact" },
                    new ContactPerson { Name = "Carol White", Phone = "555-0102", Address = "789 Pine St", City = "Capital City", Notes = "Evening calls only" }
                );
                context.SaveChanges();
            }
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { Name = "Detective John Doe", BadgeNumber = "BD123", Email = "john.doe@example.com", Role = UserRole.Detective, Password = HashingService.HashPassword("password1") },
                    new User { Name = "Detective Jane Smith", BadgeNumber = "BD456", Email = "jane.smith@example.com", Role = UserRole.Detective, Password = HashingService.HashPassword("password2") },
                    new User { Name = "Supervisor Anne Clark", BadgeNumber = "BS789", Email = "anne.clark@example.com", Role = UserRole.Supervisor, Password = HashingService.HashPassword("password3") }
                );
                context.SaveChanges();
            }

            if (!context.Cases.Any())
            {
                var detective = context.Users.FirstOrDefault(u => u.Role == UserRole.Detective);
                var supervisor = context.Users.FirstOrDefault(u => u.Role == UserRole.Supervisor);
                var tips = context.Tips.ToList();

                var caseLocal = new Case
                {

                    Name = "High Profile Burglary",
                    SerialNumber = "CP-112233",
                    Notes = new List<string?> { "Urgent", "High value items stolen" },
                    Description = "Investigation of a high profile burglary in downtown district.",
                    Detective = detective,
                    Supervisor = supervisor,
                    Status = CaseStatus.Active,
                    Tips = tips
                };

                context.Cases.Add(caseLocal);

                context.SaveChanges();
            }


            if (!context.Tips.Any())
            {
                // Assume some cases and contact persons exist; otherwise, adjust accordingly.
                var firstContact = context.ContactPersons.ToList();
                context.Tips.AddRange(
                    new Tip { Description = "Anonymous tip about suspicious activity.", ContactPerson = firstContact[0], LogDate = DateTime.Now, Level = TipLevel.Public },
                    new Tip { Description = "Reported sighting of missing person.", ContactPerson = firstContact[1], LogDate = DateTime.Now.AddDays(-1), Level = TipLevel.Classified },
                    new Tip { Description = "Possible fraud activity at local bank.", ContactPerson = firstContact[1], LogDate = DateTime.Now.AddDays(-2), Level = TipLevel.Public },
                    new Tip { Description = "Unusual car parked near crime scene.", ContactPerson = firstContact[2], LogDate = DateTime.Now.AddDays(-3), Level = TipLevel.Classified },
                    new Tip { Description = "Important evidence found in nearby trash bin.", ContactPerson = firstContact[2], LogDate = DateTime.Now.AddDays(-4), Level = TipLevel.Public }
                );

                context.SaveChanges();
            }

            context.SaveChanges();
        }
    }

}
