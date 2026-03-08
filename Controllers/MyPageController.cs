using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;    
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Data;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Controllers
{
    [Authorize]
    public class MyPageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        
        public MyPageController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //My Page
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            ViewData["TitleSort"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DescriptionSort"] = sortOrder == "desc" ? "desc_desc" : "desc";

            ViewData["CurrentFilter"] = searchString;

            var inventories = _context.Inventories
                .Where(i => i.OwnerId == user.Id);

            //Search
            searchString = searchString?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                inventories = inventories.Where(i =>
                    EF.Functions.ILike(i.Title!, $"%{searchString}%") ||
                    (i.Description != null && EF.Functions.ILike(i.Description, $"%{searchString}%"))
                );
            }

            //Sort
            switch (sortOrder)
            {
                case "title_desc":
                    inventories = inventories.OrderByDescending(i => i.Title);
                    break;

                case "desc":
                    inventories = inventories.OrderBy(i => i.Description);
                    break;

                case "desc_desc":
                    inventories = inventories.OrderByDescending(i => i.Description);
                    break;

                default:
                    inventories = inventories.OrderBy(i => i.Title);
                    break;
            }

            return View(await inventories.ToListAsync());
        }

        // GET: MyPage/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MyPage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inventory inventory)
        {
            var userId = _userManager.GetUserId(User)!;

            inventory.OwnerId = userId;                
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: MyPage/DeleteSelected
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSelected(List<int> selectedIds)
        {
            var userId = await _userManager.GetUserAsync(User);

            if (userId == null)
            {
                return Challenge();
            }

            var inventoriesToDelete = await _context.Inventories.Where(i => selectedIds.Contains(i.Id) && i.OwnerId == userId.Id).ToListAsync();

            _context.Inventories.RemoveRange(inventoriesToDelete);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}