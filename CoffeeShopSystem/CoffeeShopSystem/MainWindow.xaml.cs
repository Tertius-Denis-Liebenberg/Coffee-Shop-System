using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace CoffeeShopSystem
{
    // Interface declares method
    interface Interface1
    {
        void TotalOrderSummary(string passQuery, Label summary);
    }

    // Base class implements Interface
    public class Order : Interface1
    {
        public virtual void TotalOrderSummary(string passQuery, Label summary)
        {
            // Display all summary data
        }
    }

    // Subclass extends base class
    public class Manager : Order
    {
        // Overide TotalOrderSummary
        public override void TotalOrderSummary(string passQuery, Label summary)
        {
            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-57CKL3L\MSSQLSERVER01;Initial Catalog=CoffeeShopDB;Integrated Security=True");

            // Create file path
            string dir = Environment.CurrentDirectory;
            string path = Regex.Replace(dir, @"\\bin.*", @"\Files\BackUpSale.txt");
            try
            {
                //Data to upload to txt 
                con.Open();
                SqlCommand cmd = new SqlCommand(passQuery, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Adds to summary
                    summary.Content += string.Format("{0, -15} {1, -15} {2, -10} {3, -10} {4, -10} {5, -15} {6, -20} {7, -10}\n",
                    reader["CustomerID"].ToString(),
                    reader["CoffeeType"].ToString(),
                    reader["Size"].ToString(),
                    reader["Sugar"].ToString(),
                    reader["Cream"].ToString(),
                    reader["Quantity"].ToString(),
                    reader["Dates"].ToString(),
                    reader["Price"].ToString());
                }
                con.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occured" + e);
            }
        }

        /* New Methods */

        public void DisplayInfos()
        {
            
        }

        // Save data to text
        public void SaveToTXT(string passQuery)
        {
            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-57CKL3L\MSSQLSERVER01;Initial Catalog=CoffeeShopDB;Integrated Security=True");

            // Creates file path
            string dir = Environment.CurrentDirectory;
            string path = Regex.Replace(dir, @"\\bin.*", @"\Files\BackUpSale.txt");
            try
            {
                // Checks if file exists
                bool Exists = File.Exists(path);
                if (!Exists)
                {
                    File.CreateText(path);
                }

                // If file is empty, add headers
                if (new FileInfo(path).Length == 0)
                {
                    File.AppendAllText(path, String.Format("{0, -15} {1, -15} {2, -10} {3, -10} {4, -10} {5, -15} {6, -20} {7, -10}\n", "CustomerID", "Coffee Type", "Size", "Sugar", "Cream", "Qtity", "Date", "Price"));
                }

                //Data to upload to txt 
                con.Open();
                SqlCommand cmd = new SqlCommand(passQuery, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    File.AppendAllText(path, String.Format("{0, -15} {1, -15} {2, -10} {3, -10} {4, -10} {5, -15} {6, -20} {7, -10}\n", 
                        reader["CustomerID"].ToString(), 
                        reader["CoffeeType"].ToString(), 
                        reader["Size"].ToString(), 
                        reader["Sugar"].ToString(), 
                        reader["Cream"].ToString(), 
                        reader["Quantity"].ToString(), 
                        reader["Dates"].ToString(), 
                        reader["Price"].ToString()));
                }
                con.Close();
            }
            catch (Exception e )
            {
                MessageBox.Show("An error has occured" + e);
            }
        }

        // Save data to database
        public void SaveToDB(List<Orders> orders)
        {
            MainWindow obj = new MainWindow();
            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-57CKL3L\MSSQLSERVER01;Initial Catalog=CoffeeShopDB;Integrated Security=True");
            DateTime Date = DateTime.Now;
            string query;

            foreach (var order in orders)
            {
                // for each order placed
                query = "Insert into SalesTable (CustomerID, CoffeeType, Size, Sugar, Cream, Quantity, Dates, Price) values ("
                + order.CustomerID + ", '" + order.CoffeeType + "', '" + order.Size + "', '" + order.Sugar + "', '" + order.Cream + "', " + order.Quantity + ", '" + order.Dates
                + "', 'R " + order.Price + "')";

                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
                obj.CID++;
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Public variables
        public SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-57CKL3L\MSSQLSERVER01;Initial Catalog=CoffeeShopDB;Integrated Security=True");
        public string File = @"D:\CTU\2022\PRG521\SA\CoffeeShopSystem\CoffeeShopSystem\Files\BackUpSale.txt";
        public Manager obj = new Manager();
        public int CID = 1;
        public int StartCID = 0;
        public string SizeChecked = "";
        public double price = 0;
        public int counter = 0;
        public string Checked = "";
        public string Date = "";
        public string Day = "";
        public string Month = "";
        public string Year = "";
        public DateTime Date1 = DateTime.Now;
        public string Sugar = "", Cream = "";
        public string CoffeeType = "";
        public int QuantitySelected = 0;
        public double Total = 0;
        public List<Orders> orders = new List<Orders>();

        // Gets query for managment
        public string query()
        {
            if (Monthly.IsChecked == true)
            {
                return "select * from SalesTable where MONTH('"+DatePicker.Text+"') = MONTH(Dates)";
            }
            else if (Yearly.IsChecked == true)
            {
                return "select * from SalesTable  where YEAR('" + DatePicker.Text + "') = YEAR(Dates)";
            }
            else
            {
                return "select * from SalesTable  where DAY('" + DatePicker.Text + "') = DAY(Dates)";
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            GetCID(); // Gets last Customer ID

        }

        public void GetCID()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select CustomerID from SalesTable", con);
            SqlDataReader reader = cmd.ExecuteReader();
            
            // Sets Customer ID
            while (reader.Read() == true)
            {
                CID++;
                StartCID++;
            }
        }

        /* Manager Tab */
        /* Buttons call validation */
        private void b1Click(object sender, RoutedEventArgs e)
        {
            UserValidation validate = new UserValidation(1, query(), Summary);
            validate.Show();
        }

        private void b2Click(object sender, RoutedEventArgs e)
        {

            UserValidation validate = new UserValidation(2, query(), Summary);
            validate.Show();
        }

        private void b3Click(object sender, RoutedEventArgs e)
        {
            UserValidation validate = new UserValidation(3, query(), Summary);
            Summary.Content += "\n";
            validate.Show();
        }

        // Displays date selected on Managment tab
        private void DatePicked(object sender, RoutedEventArgs e)
        {
            UpdateManagementSummary();
        }

        // Updates date on Managment summary
        public void UpdateManagementSummary()
        {
            Date = DatePicker.ToString();
            string SplitDate = "";
            Month = "";
            Day = "";
            Year = "";
            int counter = 0;

            // Formats date
            foreach (char i in Date)
            {
                if (i == '/')
                {
                    SplitDate += "-";
                    counter++;
                }
                else if (i == ' ')
                {
                    break;
                }
                else
                {
                    SplitDate += i;

                    switch (counter)
                    {
                        case 0:
                            Month += i;
                            break;
                        case 1:
                            Day += i;
                            break;
                        case 2:
                            Year += i;
                            break;
                    }
                }

                Date = SplitDate;
            }

            // Displays date
            Summary.Content = "";
            if (Date != "")
            {
                DateTime Date1 = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day));
                Summary.Content = Date1.ToString("dddd, MMMM dd, yyyy");
            }

        }

        // Prints to file
        public void PrintFile(string passQuery)
        {

            // Create a PrintDialog  
            PrintDialog printDlg = new PrintDialog();
            // Create a FlowDocument dynamically.  
            FlowDocument doc = CreateFlowDocument(passQuery);
            doc.Name = "FlowDoc";
            // Create IDocumentPaginatorSource from FlowDocument  
            IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer  
            printDlg.PrintDocument(idpSource.DocumentPaginator, "Hello WPF Printing.");
        }

        // Creates File format
        private FlowDocument CreateFlowDocument(string passQuery)
        {
            // Create a FlowDocument  
            FlowDocument doc = new FlowDocument();
            // Create a Section  
            Section sec = new Section();
            // Create first Paragraph  
            Paragraph p1 = new Paragraph();
            // Create and add a new Bold, Italic and Underline  
            Bold bld = new Bold();

            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-57CKL3L\MSSQLSERVER01;Initial Catalog=CoffeeShopDB;Integrated Security=True");

            try
            {
                //Data to upload to txt 
                con.Open();
                SqlCommand cmd = new SqlCommand(passQuery, con);
                SqlDataReader reader = cmd.ExecuteReader();

                // Gets data for file
                while (reader.Read())
                {
                    p1.Inlines.Add(new Run(String.Format("\n{0, -15} {1, -15} {2, -10} {3, -10} {4, -10} {5, -15} {6, -20} {7, -10}\n",
                        reader["CustomerID"].ToString(),
                        reader["CoffeeType"].ToString(),
                        reader["Size"].ToString(),
                        reader["Sugar"].ToString(),
                        reader["Cream"].ToString(),
                        reader["Quantity"].ToString(),
                        DateTime.Parse(reader["Dates"].ToString()).ToString("yyyy/MM/dd"),
                        reader["Price"].ToString())));
                }
                con.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occured" + e);
            }

            Italic italicBld = new Italic();
            italicBld.Inlines.Add(bld);
            Underline underlineItalicBld = new Underline();
            underlineItalicBld.Inlines.Add(italicBld);
            // Add Bold, Italic, Underline to Paragraph  
            p1.Inlines.Add(underlineItalicBld);
            // Add Paragraph to Section  
            sec.Blocks.Add(p1);
            // Add Section to FlowDocument  
            doc.Blocks.Add(sec);
            return doc;
        }

        /* Order Tab */
        // Sets formatting and data to Order summary
        private void AddToOrderTable(object sender, RoutedEventArgs e)
        {
            CoffeeType = CoffeeDrop.Text;
            QuantitySelected = Convert.ToInt32(Quantity.Text);
            string description = "";

            // Checks Coffee size
            if (SmallOrder.IsChecked == true)
            {
                SizeChecked = "Small";
                description += "Small";
            }
            else if (MediumOrder.IsChecked == true)
            {
                SizeChecked = "Medium";
                description += "Medium";
            }
            else if (LargeOrder.IsChecked == true)
            {
                SizeChecked = "Large";
                description += "Large";
            }

            // Checks extras
            if (SugarOrder.IsChecked == true)
            {
                description += ", Sugar";
            }
            else if (SugarOrder.IsChecked == false)
            {
                description += "";
            }

            if (CreamOrder.IsChecked == true)
            {
                description += ", Cream";
            }
            else if (CreamOrder.IsChecked == false)
            {
                description += "";
            }

            // Checks and sets Coffee price
            if (CoffeeType == "Late")
            {
                if (SmallOrder.IsChecked == true)
                {
                    price = 28.5;
                }
                else if (MediumOrder.IsChecked == true)
                {
                    price = 33.5;
                }
                else if (LargeOrder.IsChecked == true)
                {
                    price = 37.5;
                }
            }
            else if (CoffeeType == "Capuccino")
            {
                if (SmallOrder.IsChecked == true)
                {
                    price = 28.5;
                }
                else if (MediumOrder.IsChecked == true)
                {
                    price = 33.5;
                }
                else if (LargeOrder.IsChecked == true)
                {
                    price = 37.5;
                }
            }
            else if (CoffeeType == "Americano")
            {
                if (SmallOrder.IsChecked == true)
                {
                    price = 25;
                }
                else if (MediumOrder.IsChecked == true)
                {
                    price = 30;
                }
                else if (LargeOrder.IsChecked == true)
                {
                    price = 33;
                }
            }
            else if (CoffeeType == "Espresso")
            {
                if (SmallOrder.IsChecked == true)
                {
                    price = 22;
                }
                else if (MediumOrder.IsChecked == true)
                {
                    price = 24;
                }
                else if (LargeOrder.IsChecked == true)
                {
                    price = 26;
                }
            }
            else if (CoffeeType == "Machiato")
            {
                if (SmallOrder.IsChecked == true)
                {
                    price = 25;
                }
                else if (MediumOrder.IsChecked == true)
                {
                    price = 27;
                }
                else if (LargeOrder.IsChecked == true)
                {
                    price = 29;
                }
            }
            else
            {
                price = 0;
            }

            if (SugarOrder.IsChecked == true)
            {
                Sugar = "Yes";
            }
            else
            {
                Sugar = "No";
            }

            if (CreamOrder.IsChecked == true)
            {
                Cream = "Yes";
            }
            else
            {
                Cream = "No";
            }

            // Add data to Summary
            try
            {
                if (counter == 0)
                {
                    OrderSummary.Content = Date1.ToString("dddd, MMMM dd, yyyy") + "\n\n" + string.Format("{0, -20} {1, -25} {2, -10} {3, -15} \n", "Qtity", "|Description", "|Price", "|Total     |")
                    + string.Format("{0, -23} {1, -27} {2, -10} {3, -15} \n", Quantity.Text, description, "R " + price, "R " + (price * Convert.ToInt32(Quantity.Text)));

                    counter++;
                }
                else
                {
                    OrderSummary.Content += string.Format("{0, -23} {1, -27} {2, -10} {3, -15} \n", Quantity.Text, description, "R " + price, "R " + (price * Convert.ToDouble(Quantity.Text)));
                    counter++;
                }

                // Data to add to database
                orders.Add(new Orders()
                {
                    CustomerID = CID.ToString(),
                    CoffeeType = CoffeeType,
                    Size = SizeChecked,
                    Sugar = Sugar,
                    Cream = Cream,
                    Quantity = Quantity.Text,
                    Dates = Date,
                    Price = price.ToString()
                });

                // Calculates overall Total
                Total += price * Convert.ToInt32(Quantity.Text);
                Totals.Content =  "Total: R " + Total;
            }
            catch
            {
                MessageBox.Show("Some fields are not selected!");
            }
        }

        // Calls method to save to database
        private void Print(object sender, RoutedEventArgs e)
        {
            obj.SaveToDB(orders);
        }
    }
}
