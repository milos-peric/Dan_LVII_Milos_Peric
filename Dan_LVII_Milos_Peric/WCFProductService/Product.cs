using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCFProductService
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ItemsRemaining { get; set; }
        public double Price { get; set; }
        public Product()
        {
        }

        public Product(int id, string name, int remainingItems, double price)
        {
            ID = id;
            Name = name;
            ItemsRemaining = remainingItems;
            Price = price;
        }
    }
}