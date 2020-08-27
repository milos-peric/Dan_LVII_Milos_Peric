using Dan_LVII_Milos_Peric.ProductServiceReference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dan_LVII_Milos_Peric
{
    class Utility
    {
        public static void StartMenu()
        {
            Utility utility = new Utility();
            bool isCorrectChoice = true;
            while (isCorrectChoice)
            {
                Console.WriteLine();
                Console.WriteLine("\n|----------------------------------------------------------------|");
                Console.WriteLine("|                         Product Stock                          |");
                Console.WriteLine("|----------------------------------------------------------------|");
                Console.WriteLine("|            Please choose one of the options below:             |");
                Console.WriteLine("|                                                                |");
                Console.WriteLine("|            (1)Show available products                          |");
                Console.WriteLine("|            (2)Select item to buy                               |");
                Console.WriteLine("|            (3)Add new product                                  |");
                Console.WriteLine("|            (4)Modify price                                     |");
                Console.WriteLine("|                                                                |");
                Console.WriteLine("|----------------------------------------------------------------|");
                Console.WriteLine("|            (q)Quit program                                     |");
                Console.WriteLine("|----------------------------------------------------------------|\n");
                Console.WriteLine();
                string menu = Console.ReadLine();
                switch (menu)
                {
                    case "1":
                        utility.ShowAllProducts();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        break;
                    case "2":
                        utility.PurchaseProduct();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        break;
                    case "3":
                        utility.AddProduct();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        break;
                    case "4":
                        utility.ModifyPrice();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        break;
                    case "q":
                        Console.WriteLine("Are you sure you want to quit?");
                        Console.WriteLine("Press 'q' to quit or any other key to continue.");
                        if (Console.ReadKey().KeyChar == 'q' || Console.ReadKey().KeyChar == 'Q')
                        {
                            Environment.Exit(0);
                        }
                        break;
                    default:
                        Console.WriteLine("Wrong choice, please try again");
                        break;
                }
            }
        }

        public void ShowAllProducts()
        {
            using (ProductServiceClient wcf = new ProductServiceClient())
            {
                foreach (var item in wcf.GetAllProducts())
                {
                    Console.WriteLine($"{item.ID}. {item.Name} - ${item.Price}, Amount: {item.ItemsRemaining}");
                }
            }
        }

        public void PurchaseProduct()
        {
            Console.WriteLine("Displaying available products:");
            ShowAllProducts();
            Console.WriteLine("Please select one of the available products by typing product number:");
            int selectedProduct;
            List<Product> productList = new List<Product>();
            Product productToFind = null;
            bool isValidId = false;
            do
            {
                selectedProduct = Convert.ToInt32(Console.ReadLine());
                using (ProductServiceClient wcf = new ProductServiceClient())
                {
                    foreach (var item in wcf.GetAllProducts())
                    {
                        if (item.ID == selectedProduct)
                        {
                            productToFind = item;
                        }
                    }
                    if (productToFind != null)
                    {
                        Console.WriteLine($"Product number {productToFind.ID} selected.");
                        Console.WriteLine("Please select product amount:");
                        int amount = Convert.ToInt32(Console.ReadLine());
                        if (amount > productToFind.ItemsRemaining)
                        {
                            Console.WriteLine("Amount selected exeeds currently available items in stock. Please choose another amount");
                        }
                        else
                        {
                            wcf.ModifyProductStock(productToFind.ID, amount);
                            productToFind.ItemsRemaining = amount;
                            productList.Add(productToFind);
                            int choice;
                            Console.WriteLine("Do you wish to purchase another product?");
                            Console.WriteLine("1 - Yes");
                            Console.WriteLine("2 - No");
                            choice = Convert.ToInt32(Console.ReadLine());
                            if (choice == 1)
                            {
                                Console.WriteLine("Returning to item selection:");
                                ShowAllProducts();
                                Console.WriteLine("Please select one of the available products by typing product number:");
                            }
                            else if (choice == 2)
                            {
                                Console.WriteLine("Concluding shopping and generating receipt:");
                                PrintReceipt(productList);
                                isValidId = true;
                            }
                            else
                            {
                                Console.WriteLine("Wrong choice.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Product you selected doesn't exist. Please choose another product");
                    }
                }
            } while (isValidId == false);
        }

        public void AddProduct()
        {

            bool isValidProductName = false;
            string productName;
            do
            {
                Console.WriteLine("Please enter product name:");
                productName = Console.ReadLine();
                if (ValidateForLetters(productName) == false)
                {
                    Console.WriteLine("Product name can only contain letters and spaces.");
                }
                else
                {
                    isValidProductName = true;
                }
            } while (isValidProductName == false);
            bool isValidAmount = false;
            string amountInStock;
            int amountNumber;
            do
            {
                Console.WriteLine("Please enter product stock amount:");
                amountInStock = Console.ReadLine();
                bool isNumber = int.TryParse(amountInStock, out amountNumber);
                if (isNumber == false || amountNumber < 1)
                {
                    Console.WriteLine("Please enter a positive integer.");
                }
                else
                {
                    isValidAmount = true;
                }
            } while (isValidAmount == false);
            bool isValidPrice = false;
            string priceString;
            double price;
            do
            {
                Console.WriteLine("Please enter product price:");
                priceString = Console.ReadLine();
                bool isDouble = double.TryParse(priceString, out price);
                if (isDouble == false || price <= 0.0)
                {
                    Console.WriteLine("Please enter a positive price.");
                }
                else
                {
                    isValidPrice = true;
                }
            } while (isValidPrice == false);

            Product product = new Product()
            {
                Name = productName,
                ItemsRemaining = amountNumber,
                Price = price
            };
            using (ProductServiceClient wcf = new ProductServiceClient())
            {
                wcf.AddNewProduct(product);
            }
            Console.WriteLine();
            Console.WriteLine("New product added successfully.");
        }

        public void ModifyPrice()
        {
            ShowAllProducts();
            int productCount;
            using (ProductServiceClient wcf = new ProductServiceClient())
            {
                productCount = wcf.GetAllProducts().Count();
            }
            bool isValidId = false;
            string IdString;
            int idNumber;
            do
            {
                Console.WriteLine("Please select one of the items which price you wish to modify:");
                IdString = Console.ReadLine();
                bool isNumber = int.TryParse(IdString, out idNumber);
                if (isNumber == false || idNumber < 1 || idNumber > productCount)
                {
                    Console.WriteLine($"Please enter a positive integer less than {productCount + 1}.");
                }
                else
                {
                    Console.WriteLine($"Item {idNumber} selected.");
                    isValidId = true;
                }
            } while (isValidId == false);
            bool isValidPrice = false;
            string priceString;
            double price;
            do
            {
                Console.WriteLine("Please enter new price:");
                priceString = Console.ReadLine();
                bool isDouble = double.TryParse(priceString, out price);
                if (isDouble == false || price <= 0.0)
                {
                    Console.WriteLine("Please enter a positive price.");
                }
                else
                {
                    
                    isValidPrice = true;
                }
            } while (isValidPrice == false);
            using (ProductServiceClient wcf = new ProductServiceClient())
            {
                wcf.ModifyProductPrice(idNumber, price);
            }
            Console.WriteLine("Product price modified successfully, displaying available products:");
            ShowAllProducts();
        }

        public void PrintReceipt(List<Product> productList)
        {
            Console.WriteLine();
            Console.WriteLine("Receipt created:");
            Console.WriteLine(string.Concat(Enumerable.Repeat("*", 20)));
            Console.WriteLine(DateTime.UtcNow);
            Console.WriteLine(string.Concat(Enumerable.Repeat("*", 20)));
            double totalPrice = 0;
            foreach (var product in productList)
            {
                Console.WriteLine($"{product.Name} - {product.ItemsRemaining} x {product.Price}");
                totalPrice += product.ItemsRemaining * product.Price;
            }
            Console.WriteLine(string.Concat(Enumerable.Repeat("*", 20)));
            Console.WriteLine($"Price Total: ${totalPrice:0.00}");
            using (ProductServiceClient wcf = new ProductServiceClient())
            {
                wcf.CreateReceipt(productList.ToArray());
            }
            Console.WriteLine();
            Console.WriteLine("Receipt written to file in service base directory.");
        }

        public static bool ValidateForLetters(string word)
        {
            bool isOnlyLetters = Regex.IsMatch(word, @"^[a-zA-Z ]+$");
            return isOnlyLetters;
        }
    }
}
