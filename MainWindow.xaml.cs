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
using System.IO;
using System.Net;
using System.Data.SqlClient;
using System.Configuration;

namespace EJournal
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                connection = new SqlConnection(connectionString);
                connection.Open();
                Functions.Connection = connection;
            }
            catch
            {
                MessageBox.Show("Не удалось подключиться к базе данных!");
                Close();
            }
                   
        }

        SqlConnection connection;
        private void Pupil_Button(object sender, RoutedEventArgs e)
        {          
            PupilLoginWindow pupilLoginWindow = new PupilLoginWindow();
            pupilLoginWindow.Show();
            Close();
        }

        private void Teacher_Button(object sender, RoutedEventArgs e)
        {         
            TeacherLoginWindow teacherLoginWindow = new TeacherLoginWindow();
            teacherLoginWindow.Show();
            Close();
        }

        private void OpenAdminWindow(object sender, RoutedEventArgs e)
        {
            AdminLoginWindow alw = new AdminLoginWindow();
            alw.Show();
            Close();
        }

        private void ReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Functions.GetReference());
        }
    }
}
