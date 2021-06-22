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
using System.Net;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;

namespace EJournal
{
    /// <summary>
    /// Логика взаимодействия для PupilLoginWindow.xaml
    /// </summary>
    /// 

 
    public partial class PupilLoginWindow : Window
    {
        public PupilLoginWindow(SqlConnection connection)
        {
            InitializeComponent();

            this.connection = connection;

            try
            {
                updateGroups();
                GroupList.SelectedItem = GroupList.Items[0];
                FioList.SelectedItem = FioList.Items[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }        


        }

        SqlConnection connection;

        private void updateGroups()
        {
            var groups = Functions.GetGroupsFromSqlServer();

            foreach (string group in groups)
            {
                GroupList.Items.Add(group);
                updatePupils(group);
            } 
                  
        }    

        private void updatePupils(string group)
        {
            List<string> pupil_IDs = new List<string>();
            List<string> IoS = new List<string>();

            var pupils = Functions.GetGroupPupils(group, ref pupil_IDs, ref IoS);

            for (int i = 0; i < pupils.Count; i++)
            {
                FioList.Items.Add(pupils[i] + " " + IoS[i]);
            }
        }

        private void Back_Button(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private bool IsValidDate(string date)
        {
            bool valid = true;
            if (date.Length != 10)
            {
                valid = false;
            }
            else
            {
                try
                {
                    var parts = date.Split('.');
                    if (parts.Length == 3)
                    {
                        var isNumericDay = int.TryParse(parts[0], out _);
                        var isNumericMonth = int.TryParse(parts[1], out _);
                        var isNumericYear = int.TryParse(parts[2], out _);
                        valid = isNumericDay && isNumericMonth && isNumericYear;
                    }
                    else if (date.Split('-').Length == 3)
                    {
                        parts = date.Split('-');
                        var isNumericDay = int.TryParse(parts[0], out _);
                        var isNumericMonth = int.TryParse(parts[1], out _);
                        var isNumericYear = int.TryParse(parts[2], out _);
                        valid = isNumericDay && isNumericMonth && isNumericYear;
                    }
                    else if (date.Split('/').Length == 3)
                    {
                        parts = date.Split('/');
                        var isNumericDay = int.TryParse(parts[0], out _);
                        var isNumericMonth = int.TryParse(parts[1], out _);
                        var isNumericYear = int.TryParse(parts[2], out _);
                        valid = isNumericDay && isNumericMonth && isNumericYear;
                    }
                    else
                    {
                        valid = false;
                    }           
                }
                catch (Exception ex)
                {
                    valid = false;
                }
            }
            return valid;
        }

        private void Enter_Pupil(object sender, RoutedEventArgs e)
        {
            string secondName = FioList.SelectedItem.ToString().Split()[0];
            string io = FioList.SelectedItem.ToString().Split()[1] + " " + FioList.SelectedItem.ToString().Split()[2];
            string group = GroupList.Text;
            string birthday = BirthdayTextBox.Text;

            if (!IsValidDate(birthday))
            {
                MessageBox.Show("Дата имела неверный формат!");
                return;
            }

            if (Functions.ValidatePupil(secondName,birthday,group))
            {
                string pupil_ID = Functions.GetPupilID(group, secondName, birthday, io);
                PupilJournalWindow pupilJournalWindow = new PupilJournalWindow(Functions.GetGroupSubjectsTeachers(group),secondName, pupil_ID, group, ref connection);
                pupilJournalWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Введены неверные данные");
            }
        }

        private void ReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Functions.GetReference());
        }
    }
}
