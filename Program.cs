using System.Text.RegularExpressions;

namespace PRELIMSAGAIN
{

    internal class Program
    {
        static int tableWidth = 109;

        static void Main(string[] args)
        {
            bool verify;
            do
            {
                Console.WriteLine("No One Left Behind: LGU Donation Inventory Management System");
                Console.Write("Enter username: ");
                string username = Console.ReadLine();
                Console.Write("Enter password: ");
                string password = Console.ReadLine();
                string fPath = "login.txt";

                verify = VerifyLogin(username, password, fPath);

                if (verify == false)
                {
                    Console.WriteLine("Wrong Credentials! Try Again!");
                    Console.ReadKey();
                    Console.Clear();
                }
            } while (verify == false);

        RootMenu:
            string choice1 = RootMenu();
            while (true)
            {
                switch (choice1)
                {
                    case "1":
                        Console.WriteLine("\nGoods Menu:");
                        Console.WriteLine("1. Add Donation Goods");
                        Console.WriteLine("2. Update Donation Goods");
                        Console.WriteLine("3. Remove Donation Goods");
                        Console.WriteLine("4. Display Goods");
                        Console.WriteLine("5. Search Goods");
                        Console.WriteLine("6. Request Goods");
                        Console.WriteLine("7. Return");

                        Console.Write("Choice: ");
                        string choice = Console.ReadLine();
                        Console.Clear();

                        switch (choice)
                        {
                            case "1":
                                AddGoodsMenu();
                                break;
                            case "2":
                                UpdateGoodsMenu();
                                break;
                            case "3":
                                RemoveGoodsMenu();
                                break;
                            case "4":
                                DisplayGoodsMenu();
                                break;
                            case "5":
                                SearchGoodsMenu();
                                break;
                            case "6":
                                RequestGoodsMenu();
                                break;
                            case "7":
                                goto RootMenu;

                            default:
                                Console.WriteLine("Invalid choice. Please try again.");
                                break;
                        }
                        break;

                    case "2":
                        RequestorManager manager = new RequestorManager();

                        manager.LoadRequestorsFromFile();

                        Console.WriteLine("\nRequestor Menu:");
                        Console.WriteLine("1. Add Requestor");
                        Console.WriteLine("2. Search Requestor");
                        Console.WriteLine("3. Update Requestor");
                        Console.WriteLine("4. Delete Requestor");
                        Console.WriteLine("5. Display All Requestors");
                        Console.WriteLine("6. View Requested Goods");
                        Console.WriteLine("7. Return");

                        Console.Write("Choice: ");
                        string choice2 = Console.ReadLine();
                        Console.Clear();


                        switch (choice2)
                        {
                            case "1":
                                Console.Write("Enter Last Name: ");
                                string lastName = Console.ReadLine();
                                Console.Write("Enter First Name: ");
                                string firstName = Console.ReadLine();
                                Console.Write("Enter Middle Initial: ");
                                string middleInitial = Console.ReadLine();
                                string dob = Birthdate();
                                Console.Write("Enter Contact Number: ");
                                string contact = Console.ReadLine();
                                Console.Write("Enter Address to be delivered: ");
                                string address = Console.ReadLine();
                                manager.AddRequestor(new Requestor(lastName, firstName, middleInitial, dob, contact, address));
                                break;

                            case "2":
                                Console.Write("Enter Student ID to search: ");
                                string searchID = Console.ReadLine();
                                manager.ViewRequestor(searchID);
                                break;

                            case "3":
                                try
                                {
                                    Console.Write("Enter Requestor ID to update: ");
                                    string updateID = Console.ReadLine();
                                    Console.Write("Enter New Last Name: ");
                                    string newLastName = Console.ReadLine();
                                    Console.Write("Enter New First Name: ");
                                    string newFirstName = Console.ReadLine();
                                    Console.Write("Enter New Middle Initial: ");
                                    string newMiddleInitial = Console.ReadLine();
                                    string newDob = Birthdate();
                                    Console.Write("Enter Contact Number: ");
                                    string newContact = Console.ReadLine();
                                    Console.Write("Enter Address to be delivered: ");
                                    string newAddress = Console.ReadLine();
                                    manager.UpdateRequestor(updateID, newLastName, newFirstName, newMiddleInitial, newDob, newContact, newAddress);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("An error occured while Entering message: " + e.Message);

                                }
                                break;

                            case "4":
                                Console.Write("Enter Requestor ID to delete: ");
                                string deleteID = Console.ReadLine();
                                manager.DeleteRequestor(deleteID);
                                break;

                            case "5":
                                manager.DisplayAllRequestors();
                                break;

                            case "6":
                                manager.SearchRequestorGoods();
                                break;

                            case "7":
                                goto RootMenu;

                            default:
                                Console.WriteLine("Invalid choice. Please try again.");
                                break;
                        }
                        break;

                    case "3":
                        Console.WriteLine("Exiting program...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        goto RootMenu;
                }
            }
        }


        public static string RootMenu()
        {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Goods");
                Console.WriteLine("2. Requestor");
                Console.WriteLine("3. Exit");

            Console.Write("Choice: ");
            string choice1 = Console.ReadLine();
            Console.Clear();
            return choice1;
        }

        public static bool VerifyLogin(string username, string password, string fPath)
        {
            string[] lines = System.IO.File.ReadAllLines(@fPath);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] field = lines[i].Split(',');
                if (field[0].Equals(username) && field[1].Equals(password))
                {
                    return true;
                }
            }
            return false;
        }

        public static string Birthdate()
        {
            string pattern = @"^\d{2}/\d{2}/\d{4}$";
            while (true)
            {
                Console.Write("Enter Birth Date (MM/DD/YYYY): ");
                string dob = Console.ReadLine();
                if (Regex.IsMatch(dob, pattern))
                {
                    return dob;
                }
                Console.WriteLine("Invalid date format. Try again.");
            }
        }
        private static void AddGoodsMenu()
        {

                Console.WriteLine("Add Donation Goods");
                Console.WriteLine("1. Add Food and Water");
                Console.WriteLine("2. Add Hygiene Goods");
                Console.WriteLine("3. Add Clothing Donations");

                Console.Write("Choice: ");
                string choice = (Console.ReadLine());
                Goods goods = GoodsManager.GetGoodsInstance(choice);
                goods?.AddGoods();
            
        }

        public static void SearchGoodsMenu()
        {
            Console.WriteLine("Search Donation Goods");
            Console.WriteLine("1. Search Food and Water");
            Console.WriteLine("2. Search Hygiene Goods");
            Console.WriteLine("3. Search Clothing Donations");
            Console.Write("Choice: ");

            string choice = Console.ReadLine();
            Goods goods = GoodsManager.GetGoodsInstance(choice);
            goods?.SearchGoods();
        }
        private static void DisplayGoodsMenu()
        {
            Console.WriteLine("Sort and Display Goods by Expiry Date");
            Console.WriteLine("1. Display Food and Water");
            Console.WriteLine("2. Display Hygiene Goods");
            Console.WriteLine("3. Display Clothing Donations");
            Console.Write("Choice: ");
            string sortChoice = Console.ReadLine();
            Goods goods = GoodsManager.GetGoodsInstance(sortChoice);
            goods?.SortAndDisplayByExpiry();       
        }


        private static void UpdateGoodsMenu()
        {
            Console.WriteLine("Update Goods Menu:");
            Console.WriteLine("1. Update Food and Water");
            Console.WriteLine("2. Update Hygiene Goods");
            Console.WriteLine("3. Update Clothing Donations");
            Console.Write("Choice: ");
            string sortChoice = Console.ReadLine();
            Console.Clear();
            Goods goods = GoodsManager.GetGoodsInstance(sortChoice);
            goods?.UpdateGoods();

        }
        private static void RemoveGoodsMenu()
        {
            Console.WriteLine("Remove Goods Menu:");
            Console.WriteLine("1. Remove Food and Water");
            Console.WriteLine("2. Remove Hygiene Goods");
            Console.WriteLine("3. Remove Clothing Donations");
            Console.Write("Choice: ");
            string sortChoice = Console.ReadLine();
            Console.Clear();
            Goods goods = GoodsManager.GetGoodsInstance(sortChoice);
            goods?.RemoveGoods();

        }
        private static void RequestGoodsMenu()
        {
            Console.WriteLine("Request Goods:");
            Console.WriteLine("1. Request Food and Water");
            Console.WriteLine("2. Request Hygiene Goods");
            Console.WriteLine("3. Request Clothing Donations");
            Console.Write("Choice: ");
            string sortChoice = Console.ReadLine();
            Goods goods = GoodsManager.GetGoodsInstance(sortChoice);
            goods?.RequestGoods();
        }

    }
}



