using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Models;
using System.Security.Claims;

namespace SeminarHub.Controllers
{
    [Authorize]
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext _context;

        public SeminarController(SeminarHubDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new SeminarFormViewModel()
            {
                Categories = await GetCategoriesAsync()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(SeminarFormViewModel model)
        {
            if (!DateTime.TryParseExact(model.DateAndTime, DataConstants.DateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), "Invalid Date!");
            }
            if (await _context.Categories.FirstOrDefaultAsync(x => x.Id == model.CategoryId) == null)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Invalid Category!");
            }
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategoriesAsync();
                return View(model);
            }
            var entity = new Seminar()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                OrganizerId = GetUserId(),
                Duration = (int)model.Duration!,
                CategoryId = model.CategoryId,
                DateAndTime = result
            };
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await _context.Seminars.AsNoTracking().Select(c => new SeminarAllViewModel()
            {
                Id = c.Id,
                Topic = c.Topic,
                Lecturer = c.Lecturer,
                Category = c.Category.Name,
                DateAndTime = c.DateAndTime.ToString(DataConstants.DateFormat),
                Organizer = c.Organizer.UserName
            }).ToListAsync();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var entity = await _context.Seminars.AsNoTracking().Select(c => new SeminarFormViewModel()
            {
                Id = c.Id,
                Topic = c.Topic,
                DateAndTime = c.DateAndTime.ToString(DataConstants.DateFormat),
                Duration = c.Duration,
                Lecturer = c.Lecturer,
                Category = c.Category.Name,
                Details = c.Details,
                Organizer = c.Organizer.UserName
            }).FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return BadRequest();
            }
            return View(entity);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = GetUserId();
            var entity = await _context.Seminars.AsNoTracking().Select(c => new SeminarFormViewModel()
            {
                Id = c.Id,
                Topic = c.Topic,
                DateAndTime = c.DateAndTime.ToString(DataConstants.DateFormat),
                OrganizerId = c.OrganizerId,
            }).FirstOrDefaultAsync(x => x.Id == id && x.OrganizerId == userId);
            if (entity == null)
            {
                return BadRequest();
            }
            if (userId != entity.OrganizerId)
            {
                return Unauthorized();
            }
            return View(entity);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(SeminarFormViewModel model)
        {
            var entity =  await _context.Seminars.FirstOrDefaultAsync(x => x.Id == model.Id);
            string userId = GetUserId();
            if (entity == null)
            {
                return BadRequest();
            }
            if (entity.OrganizerId != userId)
            {
                return Unauthorized();
            }
            if (await _context.SeminarsParticipants.FirstOrDefaultAsync(x => x.SeminarId == model.Id) == null)
            {
                _context.Seminars.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(All));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Seminars.AsNoTracking().Select(c => new SeminarFormViewModel()
            {
                Id = c.Id,
                Topic = c.Topic,
                Lecturer = c.Lecturer,
                Details = c.Details,
                DateAndTime = c.DateAndTime.ToString(DataConstants.DateFormat),
                Duration = c.Duration,
                OrganizerId = c.OrganizerId
            }).FirstOrDefaultAsync(x => x.Id == id);
            if (model == null)
            {
                return BadRequest();
            }
            model.Categories = await GetCategoriesAsync();
            if (model.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SeminarFormViewModel model)
        {
            var entity = await _context.Seminars.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
            {
                return BadRequest();
            }
            model.OrganizerId = entity.OrganizerId;
            if (model.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }
            if (!DateTime.TryParseExact(model.DateAndTime, DataConstants.DateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), "Invalid Date!");
            }
            if (await _context.Categories.FirstOrDefaultAsync(x => x.Id == model.CategoryId) == null)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Invalid Category!");
            }
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategoriesAsync();
                return View(model);
            }
            entity.Topic = model.Topic;
            entity.Lecturer = model.Lecturer;
            entity.Details = model.Details;
            entity.DateAndTime = result;
            entity.Duration = (int)model.Duration!;
            entity.CategoryId = model.CategoryId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }
        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            string userId = GetUserId();
            var model = await _context.SeminarsParticipants.AsNoTracking().Include(x => x.Seminar).ThenInclude(x => x.Category).Where(x => x.ParticipantId == userId).Select(c => new SeminarAllViewModel()
            {
                Id = c.SeminarId,
                Topic = c.Seminar.Topic,
                Lecturer = c.Seminar.Lecturer,
                Category = c.Seminar.Category.Name,
                DateAndTime = c.Seminar.DateAndTime.ToString(DataConstants.DateFormat)
            }).ToListAsync();
            return View(model);
        }
        [HttpPost]
        public async  Task<IActionResult> Join(int id)
        {
            string userId = GetUserId();
            var model = await _context.Seminars.AsNoTracking().Select(c => new SeminarFormViewModel()
            {
                Id = c.Id,
                Topic = c.Topic,
                Lecturer = c.Lecturer,
                Details = c.Details,
                DateAndTime = c.DateAndTime.ToString(DataConstants.DateFormat),
                Duration = c.Duration,
                OrganizerId = c.OrganizerId
            }).FirstOrDefaultAsync(x => x.Id == id);
            if (model == null || model.OrganizerId == userId)
            {
                return BadRequest();
            }
            if (_context.SeminarsParticipants.Any(x => x.ParticipantId == userId && x.SeminarId == id))
            {
                return RedirectToAction(nameof(All));
            }
            model.Categories = await GetCategoriesAsync();
            var entity = new SeminarParticipant()
            {
                SeminarId = model.Id,
                ParticipantId = userId
            };
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Joined));
        }
        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            string userId = GetUserId();
            var entity = await
                _context.SeminarsParticipants.FirstOrDefaultAsync(x => x.SeminarId == id && x.ParticipantId == userId);
            if (entity == null)
            {
                return BadRequest();
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Joined));
        }
        private async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {

            return await _context.Categories.AsNoTracking().Select(c => new CategoryViewModel()
            {
                Id = c.Id,
                Name = c.Name,
            }).ToListAsync();
        }
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
