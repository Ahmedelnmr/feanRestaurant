using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.EntityFrameworkCore;
using projectMvc.Helpers;
using projectMvc.Models;
using projectMvc.Models.models;
using projectMvc.Models.viewmodels;

namespace projectMvc.Controllers
{
    public class UserController : Controller
    {
        Mycontext _context ;
        private readonly UserManager<User> _userManager;
        public UserController(Mycontext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        public IActionResult Index()
        {
            return View("Home");
        }

        public async Task<IActionResult> Menu(int? categoryId)
        {
            var viewModel = new Menuviewmodel
            {
                Categories = await _context.Categories
         .Where(c =>
             !c.IsDeleted &&
             c.ManuItems.Any(m => !m.IsDeleted )
         )
         .Include(c => c.ManuItems.Where(m => !m.IsDeleted ))
         .ToListAsync(),

                manuItems = categoryId == null
         ? await _context.ManuItems
             .Where(m => !m.IsDeleted )
             .ToListAsync()
         : await _context.ManuItems
             .Where(m => m.CategoryId == categoryId && !m.IsDeleted )
             .ToListAsync(),

                SelectedCategoryId = categoryId
            };

            return View(viewModel);
        }
        public IActionResult About()
        {
            return View();
        }
        public async Task<IActionResult>ItemDetails(int id)
        {
            var item = await _context.ManuItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }
      
       
        public IActionResult OrderConfirmation()
        {
                        return View();

        }

        #region Cart Management (Session-Based)

        [HttpPost]
        public async Task<IActionResult> AddToCart(int manuItemId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You must be logged in to add items to the cart";
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
            }
            var item = await _context.ManuItems.FindAsync(manuItemId);
            if (item == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User); 
            var cartKey = $"Cart_{userId}";
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey) ?? new List<CartItem>();
            var existingItem = cart.FirstOrDefault(c => c.ManuItemId == manuItemId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {

                cart.Add(new CartItem
                {
                    ManuItemId = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = quantity,
                    Description = item.Description,
                    Image = item.Image
                });
            }

            HttpContext.Session.SetObjectAsJson(cartKey, cart);
            TempData["Success"] = $"Successfully added {item.Name} to your cart 🛒";
            return RedirectToAction("Cart");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Cart()
        {
            var userId = _userManager.GetUserId(User);
            var cartKey = $"Cart_{userId}";
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey) ?? new List<CartItem>();
            return View(cart);
        }

        [HttpPost]
        public IActionResult IncreaseQuantity(int id)
        {
            var userId = _userManager.GetUserId(User);
            var cartKey = $"Cart_{userId}";
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey);
            if (cart == null) return Json(new { success = false });

            var item = cart.FirstOrDefault(x => x.ManuItemId == id);
            if (item != null)
            {
                item.Quantity++;
                HttpContext.Session.SetObjectAsJson(cartKey, cart);
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DecreaseQuantity(int id)
        {
            var userId = _userManager.GetUserId(User);
            var cartKey = $"Cart_{userId}";
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey);
            if (cart == null) return Json(new { success = false });

            var item = cart.FirstOrDefault(x => x.ManuItemId == id);
            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
                HttpContext.Session.SetObjectAsJson(cartKey, cart);
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var userId = _userManager.GetUserId(User);
            var cartKey = $"Cart_{userId}";
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey);
            if (cart == null) return Json(new { success = false });

            cart.RemoveAll(x => x.ManuItemId == id);
            HttpContext.Session.SetObjectAsJson(cartKey, cart);

            return Json(new { success = true });
        }

        #endregion

        #region Order Management

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            var userId = _userManager.GetUserId(User);
            var cartKey = $"Cart_{userId}";
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey) ?? new List<CartItem>();

            if (!cart.Any())
            {
                TempData["Error"] = "Cart is Empty 🛒";
                return RedirectToAction("Cart");
            }

            return View(cart);
        }

        [Authorize]
        [HttpPost]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderType orderType, string deliveryAddress)
        {
            var userId = _userManager.GetUserId(User);

            // 1️⃣ Get the cart from Session (linked to user)
            var cartKey = $"Cart_{userId}";
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey);

            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Cart");
            }

            // 2️⃣ Check delivery address if order type is Delivery
            if (orderType == OrderType.Delivery && string.IsNullOrWhiteSpace(deliveryAddress))
            {
                TempData["Error"] = "Please enter a delivery address.";
                return RedirectToAction("Checkout");
            }

            // 3️⃣ Calculate totals
            decimal subtotal = cart.Sum(x => x.SubTotal);

            // ✨ Happy Hour Discount: 20% off from 3-5 PM
            var now = DateTime.Now;
            decimal happyHourDiscount = 0;
            if (now.Hour >= 15 && now.Hour < 17)
            {
                happyHourDiscount = subtotal * 0.2m;
            }

            // ✨ Bulk Discount: 10% off orders over $100
            decimal bulkDiscount = 0;
            if (subtotal > 100)
            {
                bulkDiscount = subtotal * 0.1m;
            }

            decimal totalDiscount = happyHourDiscount + bulkDiscount;

            decimal tax = (subtotal - totalDiscount) * 0.085m;
            decimal delivery = orderType == OrderType.Delivery ? 5 : 0;
            decimal total = (subtotal - totalDiscount) + tax + delivery;

            // 4️⃣ Create new order
            var order = new Order
            {
                UserId = userId,
                OrderType = orderType,
                DeliveryAddress = deliveryAddress,
                Subtotal = subtotal,
                Tax = tax,
                Discount = totalDiscount,
                Total = total,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                EstimatedDeliveryTime = DateTime.Now.AddMinutes(orderType == OrderType.Delivery ? 45 : 30)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 5️⃣ Convert CartItems to OrderItems
            foreach (var cartItem in cart)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ManuItemId = cartItem.ManuItemId,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.OrderItems.Add(orderItem);

                // 6️⃣ Update daily stats
                await UpdateDailyStats(cartItem.ManuItemId, cartItem.Quantity);
            }

            await _context.SaveChangesAsync();

            // 7️⃣ Clear cart after order is placed
            HttpContext.Session.Remove(cartKey);

            TempData["Success"] = $"Your order has been placed successfully! 🎉 Order Number: #{order.Id}";
            return RedirectToAction("OrderConfirmation", new { id = order.Id });
        }



        private async Task UpdateDailyStats(int manuItemId, int quantity)
        {
            var today = DateTime.Today;

            var stats = await _context.DailyMenuItemStats
                .FirstOrDefaultAsync(s => s.ManuItemId == manuItemId && s.Date == today);

            if (stats == null)
            {
                // إنشاء سجل جديد
                stats = new DailyMenuItemStats
                {
                    ManuItemId = manuItemId,
                    Date = today,
                    OrderCount = quantity
                };
                _context.DailyMenuItemStats.Add(stats);
            }
            else
            {
                // تحديث العدد الموجود
                stats.OrderCount += quantity;
            }

            await _context.SaveChangesAsync();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var userId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ManuItem)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ManuItem)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ManuItem)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        #endregion

        #region Statistics (للإدارة)

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> MenuItemStats(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.Today.AddDays(-30);
            endDate ??= DateTime.Today;

            var stats = await _context.DailyMenuItemStats
                .Include(s => s.MenuItem)
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .GroupBy(s => new { s.ManuItemId, s.MenuItem.Name })
                .Select(g => new
                {
                    MenuItemId = g.Key.ManuItemId,
                    MenuItemName = g.Key.Name,
                    TotalOrders = g.Sum(x => x.OrderCount),
                    AverageDailyOrders = g.Average(x => x.OrderCount)
                })
                .OrderByDescending(x => x.TotalOrders)
                .ToListAsync();

            return View(stats);
        }

        #endregion
        public IActionResult Book()
        {
            return View();
        }
    }

}
