using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.IO;
using System.Globalization;
using System.Data.Common;

namespace PRELIMSAGAIN
{
   
        public class Requestor
        {
            public string ID { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleInitial { get; set; }
            public string DateOfBirth { get; set; }
            public string ContactNumber { get; set; }
            public string Address { get; set; }

 
            public Requestor(string lastName, string firstName, string middleInitial, string dateOfBirth, string contactNumber, string address)
            {
                ID = GenerateRequestorID();  
                LastName = lastName;
                FirstName = firstName;
                MiddleInitial = middleInitial;
                DateOfBirth = dateOfBirth;
                ContactNumber = contactNumber;
                Address = address;
              
            }
        private string GenerateRequestorID()
            {
                Random random = new Random();
                string randomNumber = random.Next(1000, 9999).ToString(); 
                DateTime currentDate = DateTime.Now;
                return $"{currentDate.Year}-{randomNumber}"; 
            }
        }
    public class RequestorManager
    {

        private List<Requestor> requestors = new List<Requestor>();
        public void AddRequestor(Requestor requestor)
        {
            try
            {
                requestors.Add(requestor);
                SaveRequestorsToFile();
                Console.WriteLine("Requestor added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while adding: " + ex.Message);
            }
        }
        public void ViewRequestor(string requestorID)
        {
            try
            {
                var requestor = requestors.FirstOrDefault(s => s.ID == requestorID);
                if (requestor != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    PrintLine();
                    PrintRow("Requestor ID", " Last Name", "First Name", "Middle Initial", " Date of Birth", "Contact No. ", "Address");
                    PrintLine();
                  
                    PrintRow(requestor.ID, requestor.LastName, requestor.FirstName, requestor.MiddleInitial,
                        requestor.DateOfBirth, requestor.ContactNumber, requestor.Address);
                    PrintLine();
                    Console.ResetColor();

                }
                else
                {
                    Console.WriteLine("Requestor not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while viewing: " + ex.Message);
            }
        }

        public void UpdateRequestor(string requestorID, string newLastName, string newFirstName, string newMiddleInitial, string newDob, string newContact, string newAddress)
        {
            try
            {
                var requestor = requestors.FirstOrDefault(s => s.ID == requestorID);
                if (requestor != null)
                {
                    requestor.LastName = newLastName;
                    requestor.FirstName = newFirstName;
                    requestor.MiddleInitial = newMiddleInitial;
                    requestor.DateOfBirth = newDob;
                    requestor.ContactNumber = newContact;
                    requestor.Address = newAddress;
                    
                    SaveRequestorsToFile();
                    Console.WriteLine("Requestor updated successfully.");
                }
                else
                {
                    Console.WriteLine("Requestor not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating: " + ex.Message);
            }
        }

        public void DeleteRequestor(string requestorID)
        {
            try
            {
                var requestor = requestors.FirstOrDefault(s => s.ID == requestorID);
                if (requestor != null)
                {
                    requestors.Remove(requestor);
                    SaveRequestorsToFile();
                    Console.WriteLine("Requestor deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Requestor not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting: " + ex.Message);
            }
        }
        public void DisplayAllRequestors()
        {
            try
            {
                if (requestors.Count == 0)
                {
                    Console.WriteLine("No requestors to display.");
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                PrintLine();
                PrintRow("Requestor ID", "Last Name", "First Name", "Middle Initial", " Date of Birth", "Contact No.", "Address");
                PrintLine();
                foreach (var requestor in requestors)
                {
                    PrintRow(requestor.ID, requestor.LastName, requestor.FirstName, requestor.MiddleInitial, requestor.DateOfBirth, requestor.ContactNumber, requestor.Address);
                    PrintLine();
                }
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while displaying: " + ex.Message);
            }
        }
        private void SaveRequestorsToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("requestors.txt"))
                {
                    foreach (var requestor in requestors)
                    {
                        writer.WriteLine($"{requestor.ID},{requestor.LastName},{requestor.FirstName},{requestor.MiddleInitial},{requestor.DateOfBirth},{requestor.ContactNumber},{requestor.Address}");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("An error occurred while saving data to the file: " + ex.Message);
            }
        }
        public void LoadRequestorsFromFile()
        {
            try
            {
                if (File.Exists("requestors.txt"))
                {
                    using (StreamReader reader = new StreamReader("requestors.txt"))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split(',');
                            string id = parts[0];
                            string lastName = parts[1];
                            string firstName = parts[2];
                            string middleInitial = parts[3];
                            string dateOfBirth = parts[4];
                            string contactNumber = parts[5];
                            string address = parts[6];
                            requestors.Add(new Requestor(lastName, firstName, middleInitial, dateOfBirth, contactNumber, address) { ID = id }); // Load existing ID 
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("An error occurred while loading data from the file: " + ex.Message);
            }
        }
        static int tableWidth = 200;

        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth-3));
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

        public void SearchRequestorGoods()
        {
            Console.Write("ID of requestor to search: ");
            string requestorID = Console.ReadLine();

            var lines = File.ReadAllLines("summary.txt")
             .Select(line =>
              {
                  var parts = line.Split(',');
                  if (parts.Length >= 4)
                  {
                      return new
                      {
                          Name = parts[0],
                          ID = parts[1],
                          ProductName = parts[2],
                          Requested = parts[3],
                          Updated = parts[4]
                      };
                  }
                  return null;
              })
                .Where(item => item != null)
                .ToList();

            if (lines.Count == 0)
            {
                Console.WriteLine("No valid data to display.");
                return;
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintLine();
            PrintRow("Requestor Name ", "Requestor ID", "GoodsName", "Requested Quantity", "Updated Quantity");
            PrintLine();

            foreach (var item in lines)
            {
                if (item.ID.Contains(requestorID)) 
                {
                    PrintRow(item.Name, item.ID, item.ProductName, item.Requested, item.Updated);
                    PrintLine();
                }
            }
            Console.ResetColor();
        }

    } 
}
