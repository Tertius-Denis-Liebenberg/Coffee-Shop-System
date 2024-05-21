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
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace CoffeeShopSystem
{
    /// <summary>
    /// Interaction logic for UserValidation.xaml
    /// </summary>
    public partial class UserValidation : Window
    {
        public SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-57CKL3L\MSSQLSERVER01;Initial Catalog=CoffeeShopDB;Integrated Security=True");
        public string query = "";
        public static bool isValid = true;
        public MainWindow obj = new MainWindow();
        public int button = 0;
        public string passQuery = "";
        public Label summary;

        // sets MainWindow.xaml variables
        public UserValidation(int button, string query, Label summary)
        {
            InitializeComponent();
            this.button = button;
            this.passQuery = query;
            this.summary = summary;
        }

        // Checks user
        private void CheckUser(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager obj = new Manager();
                MainWindow obj2 = new MainWindow();

                // See if user has access
                query = "select * from ManagerTable where Username = '" + UserName.Text + "' and managerPassword = '" + Password.Text + "' and ManagerAccess = 'true'";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = null;

                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    con.Close();
                    UserName.Text = "";
                    Password.Text = "";
                    this.Hide();

                    // Navigates to other methods, according to the button clicked
                    switch (button){
                        case 1:
                            obj.SaveToTXT(passQuery);
                            break;
                        case 2:
                            obj2.PrintFile(passQuery);
                            break;
                        case 3:
                            obj.TotalOrderSummary(passQuery, summary);
                            break;
                    }
                }
                else
                {
                    isValid = false;
                    MessageBox.Show("Only Managers can access this option");
                    this.Hide();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occured");
            }
        }

        private void NewUser(object sender, RoutedEventArgs e)
        {
            // Creates new user
            /* Was not required for the project */
        }

        // returns validation
        public bool ValidCheck()
        {
            return isValid;
        }
    }
}
