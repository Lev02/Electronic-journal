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
using System.IO;
using System.Data.SqlClient;

namespace EJournal
{
    /// <summary>
    /// Логика взаимодействия для TeacherLoginWindow.xaml
    /// </summary>
    /// 

    public partial class TeacherLoginWindow : Window
    {
        public TeacherLoginWindow(SqlConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            updateTeachers();     
        }

        SqlConnection connection;

        List<string> teachers;
        List<string> passwords;

        private void updateTeachers(bool use_internet = false)
        {
            teachers = new List<string>();
            if (use_internet && Functions.InternetIsAvailable())
            {
                teachers = Functions.GetTeachersFromInternet();  
            }
            else
            {
                teachers = Functions.GetTeachersFromSqlServer(passwords);        
            }

            foreach (string teacher in teachers)
                TeacherList.Items.Add(teacher);
        }

        private void Back_Button(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void Enter_Teacher(object sender, RoutedEventArgs e)
        {
            string fio = TeacherList.Text;
            string password = PasswordTextBox.Password;
            if (Functions.ValidateTeacher(fio,password))
            {
                TeacherLoginSuccessWindow t = new TeacherLoginSuccessWindow(connection, fio);
                t.Show();
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
