using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectMvc.Models;
using projectMvc.Models.models;
using projectMvc.Models.viewmodels;

namespace projectMvc.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
    Mycontext _context;
        public AdminController(Mycontext context)
        {
            _context = context;
        }
        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckNameExists(string name, int id)
        {
            bool exists = _context.ManuItems.Any(x => x.Name == name && x.Id != id);
            return Json(!exists);
        }
        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckCategoryNameExists(string name, int id)
        {
            bool exists = _context.Categories.Any(x => x.Name == name && x.Id != id);
            return Json(!exists);
        }




        #region manutiems managment
        public async Task<IActionResult> Menu()
        {
            var menuviewmodel = new Menuviewmodel()
            {
                Categories = await _context.Categories.Where(p=>p.IsDeleted==false).ToListAsync(),
                manuItems = await _context.ManuItems.Where(x=>x.IsDeleted==false).Include(p => p.Category).ToListAsync()
            };
            return View(menuviewmodel);
        }
   
        [HttpGet]
        public async Task<IActionResult> Addmenuitem()
        {
            var model = new Createmanuitemviewmodel
            {
                Categories = await _context.Categories.ToListAsync()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult>Addmenuitem(Createmanuitemviewmodel Createmanuitemviewmodel)
        {
            if(Createmanuitemviewmodel.ImageFile==null)
            {
                ModelState.AddModelError("ImageFile", "Image file is required.");
            }
            if (ModelState.IsValid)
            {
               
                var newMenuItem = new ManuItem
                {
                    Name = Createmanuitemviewmodel.Name,
                    Description = Createmanuitemviewmodel.Description,
                    Price = Createmanuitemviewmodel.Price,
                    CategoryId = Createmanuitemviewmodel.CategoryId
                };
                if (Createmanuitemviewmodel.ImageFile != null && Createmanuitemviewmodel.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Createmanuitemviewmodel.ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Createmanuitemviewmodel.ImageFile.CopyToAsync(fileStream);
                    }
                    newMenuItem.Image = uniqueFileName;
                }
                _context.ManuItems.Add(newMenuItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Menu");
            }
            Createmanuitemviewmodel.Categories = await _context.Categories.Where(p=>p.IsDeleted==false).ToListAsync();
            return View(Createmanuitemviewmodel);
        }
        [HttpGet]
        public async Task<IActionResult>Update(int id)
        {
            var item = await _context.ManuItems.FindAsync(id);
            var model = new Createmanuitemviewmodel
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                CategoryId = item.CategoryId,
                UnAvailable = item.Unavailable,

                Categories = await _context.Categories.ToListAsync()
            };

            ViewBag.Id = id;

            return View("updatemenuitem", model);
        }
        public async Task<IActionResult> Update(Createmanuitemviewmodel model,int id)
        {
            if (ModelState.IsValid)
            {
                var menuItem = await _context.ManuItems.FindAsync(id);
                if (menuItem == null)
                {
                    return NotFound();
                }
                menuItem.Name = model.Name;
                menuItem.Description = model.Description;
                menuItem.Price = model.Price;
                menuItem.CategoryId = model.CategoryId;
                menuItem.Unavailable = model.UnAvailable;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                    menuItem.Image = uniqueFileName;
                }
                _context.ManuItems.Update(menuItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Menu");
            }
            model.Categories = await _context.Categories.ToListAsync();
            return View("updatemenuitem", model);
        }
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.ManuItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
                return NotFound();

            item.IsDeleted = true;
            item.Unavailable = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Menu");
        }

        #endregion
        #region category managment
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.Where(p=>p.IsDeleted==false).ToListAsync();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Addcategory()
        {
            var category = new Category();
            return View("Addcategory", category);
        }
        [HttpPost]
        public async Task<IActionResult>Addcategory(Category category)
        {
            if (ModelState.IsValid)
            {
                 await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Categories");
            }
            return View(category);
        }
        [HttpGet]
        public async Task<IActionResult> Updatecategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Updatecategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Categories");
            }
            return View(category);
        }
        
        public async Task<IActionResult> Deletecategory(int id)
        {
           var x=await _context.Categories.FindAsync(id);
            if(x==null)
            {
                return NotFound();
            }
            x.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Categories");

        }
        #endregion
        #region Orders Management

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ManuItem)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusModel model)
        {
            try
            {
                var order = await _context.Orders.FindAsync(model.OrderId);

                if (order == null)
                {
                    return Json(new { success = false, message = "Order not found" });
                }

                // Parse the status string to enum
                if (Enum.TryParse<OrderStatus>(model.Status, out var newStatus))
                {
                    order.Status = newStatus;
                    order.UpdatedAt = DateTime.Now;

                    // If status is Ready or Delivered, update the estimated time
                    if (newStatus == OrderStatus.Ready || newStatus == OrderStatus.Delivered)
                    {
                        order.EstimatedDeliveryTime = DateTime.Now;
                    }

                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Status updated successfully" });
                }

                return Json(new { success = false, message = "Invalid status" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ManuItem)
                        .ThenInclude(m => m.Category)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return PartialView("_OrderDetailsPartial", order);
        }

        [HttpGet]
        public async Task<IActionResult> PrintOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ManuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View("PrintOrder", order);
        }

        #endregion

        #region Statistics

        [HttpGet]
        public async Task<IActionResult> OrderStatistics()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            var stats = new
            {
                TodayOrders = await _context.Orders
                    .Where(o => o.CreatedAt.Date == today)
                    .CountAsync(),

                TodayRevenue = await _context.Orders
                    .Where(o => o.CreatedAt.Date == today)
                    .SumAsync(o => o.Total),

                WeekOrders = await _context.Orders
                    .Where(o => o.CreatedAt >= startOfWeek)
                    .CountAsync(),

                WeekRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= startOfWeek)
                    .SumAsync(o => o.Total),

                MonthOrders = await _context.Orders
                    .Where(o => o.CreatedAt >= startOfMonth)
                    .CountAsync(),

                MonthRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= startOfMonth)
                    .SumAsync(o => o.Total),

                PendingOrders = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Pending)
                    .CountAsync(),

                PreparingOrders = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Preparing)
                    .CountAsync()
            };

            return Json(stats);
        }

        #endregion
    }

    // Model for updating order status
   

public class UpdateOrderStatusModel
{
    public int OrderId { get; set; }
    public string Status { get; set; }
}
}
