using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace EJournal
{
    /// <summary>
    /// Логика взаимодействия для AdminLoginWindow.xaml
    /// </summary>
    public partial class AdminLoginWindow : Window
    {
        public AdminLoginWindow(SqlConnection connection)
        {
            InitializeComponent();

            this.connection = connection;

        }

        SqlConnection connection;

        private void Back_Button(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void Enter_Admin(object sender, RoutedEventArgs e)
        {
            string login = AdminTextBox.Text;
            string password = PasswordTextBox.Password;
            if (Functions.ValidateAdmin(login, password))
            {
                AdminWindow aw = new AdminWindow(connection);
                aw.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Введены неверные данные!");
            }
        }

        private void ReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Functions.GetReference());
        }
    }
}
