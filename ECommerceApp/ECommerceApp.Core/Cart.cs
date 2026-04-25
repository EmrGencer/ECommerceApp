using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.Core
{
    public class Cart
    {
        private readonly List<Product> _items = new List<Product>();
        public IReadOnlyList<Product> Items => _items;

        public void AddProduct(Product product, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                _items.Add(product);
            }
        }

        public decimal GetTotalPrice()
        {
            decimal total = _items.Sum(item => item.Price);
            if (total > 100)
            {
                return total - 100; 
            }
            return total;
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}