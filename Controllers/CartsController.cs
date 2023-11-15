using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldDominion.Models;
using WorldDominion.Services;

namespace WorldDominion.Controllers 
{
	public class CartsController : Controller 
	{
		private readonly ApplicationDbContext _context;
		private readonly CartService _cartService;

		public CartsController(CartService cartService, ApplicationDbContext context)
		{
			_context = context;
			_cartService = cartService;
		}

		public async Task<IActionResult> Index()
		{
			//Get cart (either existing or generate new one)
			var cart = _cartService.GetCart();

			if(cart == null)
			{
				return NotFound();
			}

			// If the cart has items, we need to add the product reference for those items
			if (cart.CartItems.Count > 0)
			{
				foreach(var cartItem in cart.CartItems)
				{
					var product = await _context.Products
						.Include(p => p.Department)
						.FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);

					if (product != null)
					{
						cartItem.Product = product;
					}
				}
			}

			return View(cart);
		}

		[HttpPost]
		public async Task<IActionResult> AddToCart(int productId, int quantity)
		{
			// Getting the active cart
			var cart = _cartService.GetCart();

			if(cart == null)
			{
				return NotFound();
			}

			// Checking if the item is already in the cart
			var cartItem = cart.CartItems.Find(cartItem => cartItem.ProductId == productId);

			if(cartItem != null && cartItem.Product != null)
			{
				cartItem.Quantity += quantity;	
			} else {
				var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

				if (product == null)
				{
					return NotFound();
				}

				cartItem = new CartItem
				{
					ProductId = productId,
					Quantity = quantity,
					Product = product
				};

				cart.CartItems.Add(cartItem);
			}

			_cartService.SaveCart(cart);
		
			return RedirectToAction("Index");
		}

		public IActionResult RemoveFromCart(int productId)
		{
			// Getting the active cart
			var cart = _cartService.GetCart();

			if(cart == null)
			{
				return NotFound();
			}

			// Checking if the item is already in the cart
			var cartItem = cart.CartItems.Find(cartItem => cartItem.ProductId == productId);

			if(cartItem != null)
			{
				cart.CartItems.Remove(cartItem);
				_cartService.SaveCart(cart);
			}

			return RedirectToAction("Index");
		}

	}
}