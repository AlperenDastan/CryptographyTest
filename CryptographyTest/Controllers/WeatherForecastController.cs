using CryptographyTest.Models;
using CryptographyTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CryptographyTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly DetectiveApiDbContext _context;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, DetectiveApiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet, Route("/Get/All/Cases"),]
        public async Task<ICollection<Case>> GetCases()
        {
            var cases = await _context.Cases
                            .Include(x => x.Detective)
                            .Include(x => x.Supervisor)
                            .Include(x => x.Tips)
                            .ThenInclude(y => y.ContactPerson)
                            .ToListAsync();

            return cases;
        }
        [HttpGet, Route("/Get/All/Tips"),]
        public async Task<ICollection<Tip>> GetTips()
        {
            var tips = await _context.Tips
                .Include(x => x.ContactPerson)
                .Select(x => new Tip 
                { 
                    Id = x.Id, 
                    Level = x.Level, 
                    LogDate = x.LogDate,
                    Description = x.Description, 
                    ContactPerson = new ContactPerson
                    {
                        Id = x.ContactPerson.Id,
                        Address = x.ContactPerson.Address,
                        City = x.ContactPerson.City,
                        Name = x.ContactPerson.Name,
                        Notes = x.ContactPerson.Notes,
                        Phone = x.ContactPerson.Phone,
                    }, 
                })
                .ToListAsync();
            return tips;
        }


        [HttpGet, Route("/Get/All/Persons"),]
        public async Task<ICollection<ContactPerson>> GetContactPersons()
        {
            var persons = await _context.ContactPersons.ToListAsync();
            return persons;
        }
        [HttpGet, Route("/Get/All/Users"),]
        public async Task<ICollection<User>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        [HttpGet, Route("/Post/VerifyPassword"),]
        public async Task<IActionResult> VerifyPassword(Guid userId, string rawPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is not null)
            {
                var result = HashingService.VerifyPassword(rawPassword, user.Password);

                if (result)
                {
                    return Ok($"Success. Raw={rawPassword}, StoredHash={user?.Password}");
                }

                return Unauthorized("Failed to verify hash");
            }

            return BadRequest("User not found");
        }

        [HttpPost, Route("/Post/Encrypt/Cases/Name")]
        public async Task<ActionResult<string>> EncryptCaseNames()
        {
            var cases = await _context.Cases.ToListAsync();
            var casesNames = "";
            var casesEncryptedNames = "";
            foreach (var cas in cases)
            {
                if (!string.IsNullOrEmpty(cas.Name))
                {
                    casesNames += cas.Name;
                    cas.Name = RsaService.Encrypt(cas.Name);
                    casesEncryptedNames += cas.Name;
                }
            }

            _context.UpdateRange(cases);
            var result = _context.SaveChanges();
            return Ok($"Updated number cases:{result}. Orginal names:{casesNames}. Encrypted names: {casesEncryptedNames}");
        }


        [HttpPost, Route("/Post/Decrypt/Cases/Name")]
        public async Task<ActionResult<string>> DecryptCaseNames()
        {
            var cases = await _context.Cases.ToListAsync();
            var casesNames = "";
            var casesEncryptedNames = "";
            foreach (var cas in cases)
            {
                if (!string.IsNullOrEmpty(cas.Name))
                {
                    casesEncryptedNames += cas.Name;
                    cas.Name = RsaService.Decrypt(cas.Name);
                    casesNames += cas.Name;
                }
            }

            _context.UpdateRange(cases);
            var result = _context.SaveChanges();
            return Ok($"Updated number cases:{result}. Orginal names:{casesNames}. Encrypted names: {casesEncryptedNames}");
        }


        [HttpPost, Route("/Post/Case/Tip")]
        public async Task<ActionResult<string>> PostTip(Guid caseId, TipDto tip)
        {
            var cas = await _context.Cases.Include(x => x.Tips)
                                          .ThenInclude(x => x.ContactPerson)
                                          .FirstOrDefaultAsync(c => c.Id == caseId);

            if (cas is not null)
            {
                var newTip = new Tip
                {
                    Description = tip.Description,  // Consider encrypting if needed outside EF
                    Level = TipLevel.Classified,
                    LogDate = DateTime.Now,
                    ContactPerson = new ContactPerson
                    {
                        Name = tip.PersonName,   // Consider moving encryption to the getter/setter or adjust model setup
                        Address = tip.Address,
                        City = tip.City,
                        Notes = tip.Notes,
                        Phone = tip.Phone,
                    }
                };
                _context.Tips.Add(newTip);
                cas.Tips.Add(newTip);  // EF tracks this change automatically

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok($"Saved tip! Thank you for helping us, please come again.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Log the error
                    return BadRequest("Failed to save due to a data conflict. Please try again.");
                }
            }

            return BadRequest("Case not found");
        }


    }

    public class TipDto
    {
        public string? Description { get; set; }
        public string? PersonName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Notes { get; set; }
    }
}
