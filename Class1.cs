using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PRELIMSAGAIN
{

    abstract class Goods
    {
        public abstract string FilePath { get; }
        public virtual bool RequiresExpiry => false;

        private string GenerateProductID()
        {
            Random random = new Random();
            string randomNumber = random.Next(1000, 9999).ToString(); 
            return $"{randomNumber}"; 
        }
        public void AddGoods()
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("File not found. Creating a new one...");
                File.Create(FilePath).Dispose();
            }

            Console.Write("Enter Product Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Product Quantity: ");
            int quantity = int.Parse(Console.ReadLine());

            string expiryDate = "";
            if (RequiresExpiry)
            {
                Console.WriteLine("Is expiration date needed? [y/n]: ");
                string confirm = Console.ReadLine();
                if (confirm == "y")
                {
                    expiryDate = GetValidExpiryDate();
                }
            }
            string code = GenerateProductID();

            using (StreamWriter writer = File.AppendText(FilePath))
            {
                writer.WriteLine($"{name},{expiryDate},{quantity},{code}");
            }
            Console.WriteLine("Product added successfully!");
        }

        public void UpdateGoods()
        {
            Console.Write("Code of product to update: ");
            string productcode = Console.ReadLine();

            var lines = File.ReadAllLines(FilePath).ToList();
            var updatedLines = lines.Where(line => !line.EndsWith("," + productcode)).ToList();

            if (lines.Count == updatedLines.Count)
            {
                Console.WriteLine("Product not found.");
            }
            else
            {
                File.WriteAllLines(FilePath, updatedLines);
                Console.Write("New Product name: ");
                string newName = Console.ReadLine();
                string newEdate = GetValidExpiryDate();
                Console.Write("New Product Quantity: ");
                string newQuantity = Console.ReadLine();
                string code = GenerateProductID();


                using (StreamWriter writer = new StreamWriter(FilePath, true))
                {

                    writer.WriteLine($"{newName},{newEdate},{newQuantity},{code}");

                }
                Console.WriteLine("Product updated succesfully!");
            }

            

        }

        public void RemoveGoods()
        {
            Console.Write("Enter the code of the product to remove: ");
            string nameToRemove = Console.ReadLine();

        var lines = File.ReadAllLines(FilePath).ToList();
        var updatedLines = lines.Where(line => !line.EndsWith("," + nameToRemove)).ToList();

            if (lines.Count == updatedLines.Count)
            {
                Console.WriteLine("Product not found.");
            }
            else
            {
                File.WriteAllLines(FilePath, updatedLines);
                Console.WriteLine("Product removed successfully.");
            }
        }


        public void SortAndDisplayByExpiry()
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("No data available.");
                return;
            }
          

            var lines = File.ReadAllLines(FilePath).Select(line =>
            {
                var parts = line.Split(',');
                if (parts.Length >= 3)
                {
                    DateTime expiryDate;
                    bool hasValidDate = DateTime.TryParseExact(parts[1], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiryDate);
                    return new
                    {
                        Name = parts[0],
                        ExpiryDate = hasValidDate ? expiryDate : DateTime.MaxValue,
                        Quantity = parts.Length > 2 ? parts[2] : "N/A",
                        Code = parts[3]
                    };
                }
                return null;
            })
            .Where(item => item != null)
            .OrderBy(item => item.ExpiryDate)
            .ToList();

            if (lines.Count == 0)
            {
                Console.WriteLine("No valid data to display.");
                return;
            }


            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintLine();
            PrintRow("Product Name", "Expiration", "Quantity", "Product Code");
            PrintLine();

            foreach (var item in lines)
            {
                string expiryText = item.ExpiryDate == DateTime.MaxValue ? "No Expiry" : item.ExpiryDate.ToString("MM/dd/yyyy");
                PrintRow(item.Name, expiryText, item.Quantity, item.Code);
                PrintLine();
            }

            Console.ResetColor();
        }


        private string GetValidExpiryDate()
        {
            string pattern = @"^\d{2}/\d{2}/\d{4}$";
            while (true)
            {
                Console.Write("Enter Expiry Date (MM/DD/YYYY): ");
                string date = Console.ReadLine();
                if (Regex.IsMatch(date, pattern))
                {
                    return date;
                }
                Console.WriteLine("Invalid date format. Try again.");
            }
        }

        public void SearchGoods()
        {
            Console.Write("Name of product to search: ");
            string searchProduct = Console.ReadLine();
            bool condition = false;

            var lines = File.ReadAllLines(FilePath)
                .Select(line =>
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 3)
                    {
                        DateTime expiryDate;
                        bool hasValidDate = DateTime.TryParseExact(parts[1], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiryDate);
                        return new
                        {
                            Name = parts[0],
                            ExpiryDate = hasValidDate ? expiryDate : DateTime.MaxValue,
                            Quantity = parts.Length > 2 ? parts[2] : "N/A",
                            Code = parts[3]
                        };
                    }
                    return null;
                })
                .Where(item => item != null)
                .OrderBy(item => item.ExpiryDate) 
                .ToList();

            if (lines.Count == 0)
            {
                Console.WriteLine("No valid data to display.");
                return;
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintLine();
            PrintRow("Product Name ", "Expiration", "Quantity", "Code");
            PrintLine();

            foreach (var item in lines)
            {
                if (item.Name.Contains(searchProduct, StringComparison.OrdinalIgnoreCase)) 
                {
                    condition = true; 
                    string expiryText = item.ExpiryDate == DateTime.MaxValue ? "No Expiry" : item.ExpiryDate.ToString("MM/dd/yyyy");
                    PrintRow(item.Name, expiryText, item.Quantity, item.Code);
                    PrintLine();
                }
            }
            Console.ResetColor();

            if (!condition) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\tNo Product Found, Try Again!");
                Console.ResetColor();
            }
            Console.ReadKey();
        }

        public void RequestGoods()
        {
            Console.WriteLine("Enter Account ID: ");
            string accountID = Console.ReadLine();

            var accountLines = File.ReadAllLines("requestors.txt");
            var account = accountLines.Select(line => line.Split(','))
                .FirstOrDefault(parts => parts[0] == accountID);

            if (account == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }
            string accountName = account[1];


            Console.WriteLine("Enter Product ID: ");
            string productID = Console.ReadLine();

            var productLines = File.ReadAllLines(FilePath).ToList();
            var product = productLines
                .Select(line => line.Split(','))
                .FirstOrDefault(parts => parts[3] == productID);

            if (product == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }
            string productName = product[0];
            int productQuantity = int.Parse(product[2]);

            Console.WriteLine($"Available quantity of {productName}: {productQuantity}");
            Console.WriteLine("Enter requested quantity: ");
            int requestedQuantity = int.Parse(Console.ReadLine());

            if (requestedQuantity > productQuantity)
            {
                Console.WriteLine("Insufficient stock. Request denied.");
                return;
            }

            int updatedQuantity = productQuantity - requestedQuantity;
            var updatedProductLines = productLines
                .Select(line =>
                {
                    var parts = line.Split(',');
                    if (parts[3] == productID)
                    {
                        return $"{parts[0]},{parts[1]},{updatedQuantity},{parts[3]}";
                    }
                    return line;
                })
                .ToList();
            File.WriteAllLines(FilePath, updatedProductLines);

            Console.WriteLine($"\nRequest Summary:");
            Console.WriteLine($"Account: {accountName}" + " " + $"ID: {accountID}");
            Console.WriteLine($"Product: {productName}");
            Console.WriteLine($"Requested Quantity: {requestedQuantity}");
            Console.WriteLine($"Remaining Quantity: {updatedQuantity}");

            string[] summary = { $"{accountName}", $"{accountID}", $"{productName}", $"{requestedQuantity}", $"{updatedQuantity}" };

            using (StreamWriter sw = File.AppendText("summary.txt"))
            {
                foreach (var line in summary)
                {
                    sw.Write(line + ",");
                }
                sw.WriteLine();
            }

        }

        static int tableWidth = 109;

        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
    }

    class FoodnWater : Goods
    {
        public override string FilePath => "food_and_water.txt";
        public override bool RequiresExpiry => true;
    }

    class Hygiene : Goods
    {
        public override string FilePath => "hygiene_goods.txt";
        public override bool RequiresExpiry => true;

    }

    class Clothing : Goods
    {
        public override string FilePath => "clothing_donations.txt";
        //public override bool RequiresExpiry => true;

    }


    static class GoodsManager
    {
        public static Goods GetGoodsInstance(string choice)
        {

           
                return choice switch
            {
                "1" => new FoodnWater(),
                "2" => new Hygiene(),
                "3" => new Clothing(),
                _ => null,
                 
            };

        }

    }
}
