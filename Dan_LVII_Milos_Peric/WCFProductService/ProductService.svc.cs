using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFProductService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class ProductService : IProductService
    {
        private readonly string _productStockPath = AppDomain.CurrentDomain.BaseDirectory + @"\ProductStock.txt";
        private List<Product> _productList;
        private static int _recieptNumber = 1;

        public ProductService()
        {
            _productList = GetAllProducts();
        }

        public List<Product> GetAllProducts()
        {
            List<Product> list = new List<Product>();
            try
            {
                using (StreamReader streamReader = new StreamReader(_productStockPath))
                {
                    int productId = 1;
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] lines = line.Split(',');
                        string name = lines.ElementAt(0);
                        int remainingItems = Convert.ToInt32(lines.ElementAt(1));
                        double price = Convert.ToDouble(lines.ElementAt(2));
                        list.Add(new Product(productId++, name, remainingItems, price));
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                Console.WriteLine("Not possible to read from file {0} or file doesn't exist.", _productStockPath);
                Console.WriteLine(e.Message);
                return list;
            }
        }

        public void CreateReceipt(List<Product> list)
        {
            DateTime receiptCreationTime = DateTime.UtcNow;
            StringBuilder fileName = new StringBuilder();
            fileName.Append(AppDomain.CurrentDomain.BaseDirectory);
            fileName.Append("\\");
            fileName.Append("Racun_");
            if (_recieptNumber < 10)
            {
                fileName.Append(0);
                fileName.Append(_recieptNumber++);
            }
            else
            {
                fileName.Append(_recieptNumber++);
            }
            fileName.Append("_");
            fileName.Append(DateTime.UtcNow.ToString("ddMMyyyyHHmmss"));
            fileName.Append(".txt");

            using (StreamWriter streamWriter = new StreamWriter(fileName.ToString(), append: false))
            {
                streamWriter.WriteLine(receiptCreationTime);
                streamWriter.WriteLine(string.Concat(Enumerable.Repeat("*", 20)));
                double totalPrice = 0;
                foreach (var product in list)
                {
                    streamWriter.WriteLine($"{product.Name} - {product.ItemsRemaining} x {product.Price}");
                    totalPrice += product.ItemsRemaining * product.Price;
                }
                streamWriter.WriteLine(string.Concat(Enumerable.Repeat("*", 20)));
                streamWriter.WriteLine($"Price Total: ${totalPrice:0.00}");
            }
        }

        public void ModifyProductStock(int id, int amount)
        {
            Product productToModify = (from p in _productList where p.ID == id select p).First();
            productToModify.ItemsRemaining -= amount;
            for (int i = 0; i < _productList.Count; i++)
            {
                if (productToModify.ID == _productList.ElementAt(i).ID)
                {
                    _productList[i] = productToModify;
                }
            }
            using (StreamWriter streamWriter = new StreamWriter(_productStockPath, append: false))
            {
                foreach (var product in _productList)
                {
                    streamWriter.Write(product.Name + "," + product.ItemsRemaining + "," + product.Price + "\n");
                }
            }
            _productList = GetAllProducts();
        }

        public void AddNewProduct(Product product)
        {
            using (StreamWriter streamWriter = new StreamWriter(_productStockPath, append: true))
            {
                    streamWriter.Write(product.Name + "," + product.ItemsRemaining + "," + product.Price + "\n");
            }
            _productList = GetAllProducts();
        }

        public void ModifyProductPrice(int id, double price)
        {
            Product productToModify = (from p in _productList where p.ID == id select p).First();
            productToModify.Price = price;
            for (int i = 0; i < _productList.Count; i++)
            {
                if (productToModify.ID == _productList.ElementAt(i).ID)
                {
                    _productList[i] = productToModify;
                }
            }
            using (StreamWriter streamWriter = new StreamWriter(_productStockPath, append: false))
            {
                foreach (var product in _productList)
                {
                    streamWriter.Write(product.Name + "," + product.ItemsRemaining + "," + product.Price + "\n");
                }
            }
            _productList = GetAllProducts();
        }
    }
}
