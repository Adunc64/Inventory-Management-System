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
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;   // ✅ this is the GUID Id, not email

            var list = await _context.Inventories
                .Where(i => i.OwnerId == userId)
                .ToListAsync();

            return View(list);
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
    }
}