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

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge(); // Redirect to login if user is not authenticated
            }

            var myInevntories = await _context.Inventories.Where(i => i.OwnerId == user.Id).ToListAsync();

            return View(myInevntories);
        }
    }
}