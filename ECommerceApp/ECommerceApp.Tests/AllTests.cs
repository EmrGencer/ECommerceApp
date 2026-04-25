using NUnit.Framework;
using ECommerceApp.Core;
using System;

namespace ECommerceApp.Tests
{
    [TestFixture]
    public class ECommerceTests
    {
        private Cart _cart;
        private OrderService _orderService;
        private Product _testProduct;

        [SetUp]
        public void Setup()
        {
            _cart = new Cart();
            _orderService = new OrderService();
            _testProduct = new Product { Id = 1, Name = "Laptop", Price = 50m, Stock = 5 };
        }

        [Test]
        public void TC01_WhiteBox_GetTotalPrice_Under100_CalculatesCorrectly()
        {
            _cart.AddProduct(_testProduct, 1); 
            Assert.That(_cart.GetTotalPrice(), Is.EqualTo(50m)); 
        }

        [Test]
        public void TC02_WhiteBox_GetTotalPrice_Over100_AppliesDiscountCorrectly()
        {
            _cart.AddProduct(_testProduct, 3); 
            Assert.That(_cart.GetTotalPrice(), Is.EqualTo(135m)); 
        }

        [Test]
        public void TC03_WhiteBox_CartClear_EmptiesItemList()
        {
            _cart.AddProduct(_testProduct, 1);
            _cart.Clear();
            Assert.That(_cart.Items.Count, Is.EqualTo(0)); 
        }

        [Test]
        public void TC04_BlackBox_AddProduct_NegativeQuantity_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => _cart.AddProduct(_testProduct, -2));
        }

        [Test]
        public void TC05_BlackBox_PlaceOrder_EmptyCart_ReturnsFalse()
        {
            bool result = _orderService.PlaceOrder(_cart);
            Assert.That(result, Is.False); 
        }

        [Test]
        public void TC06_BlackBox_ProcessPayment_NegativeAmount_ShouldFail()
        {
            bool result = _orderService.ProcessPayment(-50m);
            Assert.That(result, Is.False); 
        }

        [Test]
        public void TC07_GrayBox_PlaceOrder_InsufficientStock_ShouldNotGoBelowZero()
        {
            var limitedProduct = new Product { Id = 2, Name = "Mouse", Price = 10m, Stock = 1 };
            _cart.AddProduct(limitedProduct, 2); 
            _orderService.PlaceOrder(_cart);
            Assert.That(limitedProduct.Stock, Is.GreaterThanOrEqualTo(0)); 
        }

        [Test]
        public void TC08_GrayBox_PlaceOrder_SuccessfulPayment_ShouldClearCart()
        {
            _cart.AddProduct(_testProduct, 1);
            _orderService.PlaceOrder(_cart);
            Assert.That(_cart.Items.Count, Is.EqualTo(0)); 
        }

        [Test]
        public void TC09_Integration_EndToEnd_ValidOrderFlow_ReducesStockCorrectly()
        {
            int initialStock = _testProduct.Stock; 
            _cart.AddProduct(_testProduct, 2); 
            bool isSuccess = _orderService.PlaceOrder(_cart);
            Assert.That(isSuccess, Is.True);
            Assert.That(_testProduct.Stock, Is.EqualTo(initialStock - 2)); 
        }

        [Test]
        public void TC10_Integration_EndToEnd_OrderWithDiscount_ShouldProcessProperly()
        {
            _cart.AddProduct(_testProduct, 3); 
            bool isSuccess = _orderService.PlaceOrder(_cart);
            Assert.That(isSuccess, Is.True); 
        }
    }
}