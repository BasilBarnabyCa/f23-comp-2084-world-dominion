using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldDominion.Models;
using WorldDominion.Services;

namespace WorldDominion.Controllers
{
	public class OrdersController : Controller
	{
		private readonly CartService _cartService;
		private ApplicationDbContext _context;

		public OrdersController(CartService cartService, ApplicationDbContext context)
		{
			_cartService = cartService;
			_context = context;
		}

		[Authorize()]
		public IActionResult Checkout()
		{
			// var userId = User?.Identity?.IsAuthenticated;;
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var cart = _cartService.GetCart();

			if (cart == null)
			{
				return NotFound();
			}

			var order = new Order()
			{
				UserId = userId,
				Total = cart.CartItems.Sum(cartItem => cartItem.Quantity * cartItem.Product.MSRP),
				OrderItems = new List<OrderItem>()
			};

			foreach (var cartItem in cart.CartItems)
			{
				order.OrderItems.Add(new OrderItem()
				{
					OrderId = order.Id,
					ProductName = cartItem.Product.Name,
					Quantity = cartItem.Quantity,
					Price = cartItem.Product.MSRP
				});

			}

			return View("Details", order);
		}
	}
}