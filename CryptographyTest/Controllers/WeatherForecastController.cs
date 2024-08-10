using CryptographyTest.Models;
using CryptographyTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CryptographyTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly DetectiveApiDbContext _context;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly UserManager<User> _userManager;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, DetectiveApiDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;  // Initialize it here
        }

        [Authorize(Roles = "Supervisor, Detective")]
        [HttpGet, Route("/api/Get/All/Cases")]
        public async Task<IActionResult> GetCases()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (userId == null)
            {
                return NotFound("Could not find user");
            }
            var parsed = Guid.TryParse(userId, out var parseduserid);
            if (!parsed)
            {
                return BadRequest("Could not parse userId");
            }
            if (userRole == "Supervisor")
            {
                // Supervisor: return all cases
                var cases = await _context.Cases
                    .Include(x => x.Detective)
                    .Include(x => x.Supervisor)
                    .Include(x => x.Tips)
                    .ThenInclude(y => y.ContactPerson)
                    .Where(c => c.SupervisorId == parseduserid)
                    .ToListAsync();

                // Log the number of cases returned
                Console.WriteLine($"Supervisor - Number of cases returned: {cases.Count}");
                return Ok(cases);
            }
            else if (userRole == "Detective")
            {
                // Detective: return only their own cases

                var cases = await _context.Cases
                    .Include(x => x.Detective)
                    .Include(x => x.Supervisor)
                    .Include(x => x.Tips)
                    .ThenInclude(y => y.ContactPerson)
                    .Where(c => c.DetectiveId == parseduserid)
                    .ToListAsync();

                // Log the number of cases returned
                Console.WriteLine($"Detective - Number of cases returned: {cases.Count}");
                return Ok(cases);
            }

            // If the role is neither Supervisor nor Detective, return Unauthorized
            return Unauthorized("You do not have permission to access these cases.");
        }


        [Authorize(Roles = "Detective")]
        [HttpGet, Route("/api/Get/MyCases")]
        public async Task<ICollection<Case>> GetMyCases()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cases = await _context.Cases
                            .Where(c => c.Detective.Id.ToString() == userId)
                            .Include(x => x.Supervisor)
                            .Include(x => x.Tips)
                            .ThenInclude(y => y.ContactPerson)
                            .ToListAsync();

            return cases;
        }

        [Authorize(Roles = "Supervisor, Admin")]
        [HttpPost, Route("/api/Post/CreateCase")]
        public async Task<IActionResult> CreateCase([FromBody] CreateCaseRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var detective = await _context.Users.FindAsync(model.DetectiveId);
            var supervisor = await _context.Users.FindAsync(model.SupervisorId);

            if (detective == null || supervisor == null)
            {
                return BadRequest("Invalid Detective or Supervisor ID.");
            }

            var newCase = new Case
            {
                Name = model.Name,
                Description = model.Description,
                SerialNumber = model.SerialNumber,
                Status = model.Status,
                Detective = detective,
                Supervisor = supervisor,
                Notes = model.Notes?.ToList()
            };

            _context.Cases.Add(newCase);
            await _context.SaveChangesAsync();

            return Ok(newCase);
        }

        [HttpGet, Route("/api/Get/All/Tips"),]
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


        [HttpGet, Route("/api/Get/All/Persons"),]
        public async Task<ICollection<ContactPerson>> GetContactPersons()
        {
            var persons = await _context.ContactPersons.ToListAsync();
            return persons;
        }
        [HttpGet, Route("/api/Get/All/Users/{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet, Route("/api/Post/VerifyPassword")]
        public async Task<IActionResult> VerifyPassword(Guid userId, string rawPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is not null)
            {
                var result = await _userManager.CheckPasswordAsync(user, rawPassword);  // Use CheckPasswordAsync

                if (result)
                {
                    return Ok($"Success. Raw={rawPassword}, StoredHash={user.PasswordHash}");
                }

                return Unauthorized("Failed to verify hash");
            }

            return BadRequest("User not found");
        }

        [HttpPost, Route("/api/Post/Encrypt/Cases/Name")]
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


        [HttpPost, Route("/api/Post/Decrypt/Cases/Name")]
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


        [HttpPost, Route("/api/Post/Case/Tip")]
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
    public class CreateCaseRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SerialNumber { get; set; }
        public CaseStatus Status { get; set; }
        public Guid DetectiveId { get; set; }
        public Guid SupervisorId { get; set; }
        public List<string>? Notes { get; set; }
    }
}
