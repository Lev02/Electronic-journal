using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    /// Логика взаимодействия для PupilJournalWindow.xaml
    /// </summary>
    public partial class PupilJournalWindow : Window
    {
        public PupilJournalWindow(Dictionary<string,string> subjectsTeachers, string pupil, string pupil_ID, string group, ref SqlConnection connection)
        {
            InitializeComponent();

            this.connection = connection;

            this.Title = "Журнал учащегося группы " + group + " " + pupil;

            PupilJournalWindow_SizeChanged();

            this.SizeChanged += PupilJournalWindow_SizeChanged;

            List<string> subjects = new List<string>();
            foreach (var pair in subjectsTeachers)
                subjects.Add(pair.Key);

            fillMarksTable(subjects, pupil_ID);
            fillLatenessTable(subjects, pupil_ID);

            JournalLateness.Visibility = Visibility.Hidden;

            for (int i = 0; i < JournalMarks.Columns.Count; i++)
            {
                JournalMarks.Columns[i].CanUserSort = false;
                JournalMarks.Columns[i].CanUserReorder = false;
                JournalMarks.Columns[i].CanUserResize = false;
            }

            for (int i = 0; i < JournalLateness.Columns.Count; i++)
            {
                JournalLateness.Columns[i].CanUserSort = false;
                JournalLateness.Columns[i].CanUserReorder = false;
                JournalLateness.Columns[i].CanUserResize = false;
            }

            this.subjects = subjects;
            this.pupil_ID = pupil_ID;
        }

        List<string> subjects;
        string pupil_ID;

        SqlConnection connection;

        

        private void PupilJournalWindow_SizeChanged(object sender = null, SizeChangedEventArgs e = null)
        {
            double buttonWidth = 0.2;
            double buttonHeight = 0.1;
            double datagridWidth = 0.95;
            double datagridHeight = 0.75;

            JournalMarks.Width = this.Width * datagridWidth;
            JournalMarks.Height = this.Height * datagridHeight;

            JournalLateness.Width = this.Width * datagridWidth;
            JournalLateness.Height = this.Height * datagridHeight;

            this.backButton.Height = this.Height * buttonHeight;
            this.switchButton.Height = this.Height * buttonHeight;

            this.backButton.Width = this.Width * buttonWidth;
            this.switchButton.Width = this.Width * buttonWidth;

            double button_margin = this.Width * (1.0 - datagridWidth) / 2.0;
            this.backButton.Margin = new Thickness(button_margin, 0, 0, button_margin);
            this.switchButton.Margin = new Thickness(0, 0, button_margin, button_margin);

        }

        private void fillMarksTable(List<string> subjects, string pupil_ID)
        {
            DataTable dt = new DataTable();

            DateTime start_of_semester = new DateTime(2021, 2, 5);

            dt.Columns.Add("Предмет");
            dt.Columns.Add("Средний");

          
            while (start_of_semester < DateTime.Now)
            {
                string day = start_of_semester.Day.ToString();
                string month = start_of_semester.Month.ToString();

                if (day.Length == 1) day = "0" + day;
                if (month.Length == 1) month = "0" + month;

                dt.Columns.Add(day + "\\" + month);
             
                start_of_semester = start_of_semester.AddDays(1);
            }


            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            for (int i = 0; i < subjects.Count; i++)
            {
                List<string> dates = new List<string>();
                List<string> marks = new List<string>();

                dt.Rows.Add(new string[] { subjects[i] });

                Functions.GetPupilMarks(dates, marks, pupil_ID, subjects[i]);

                double avg = 0;

                for (int j = 0; j < marks.Count; j++)
                {
                    dates[j] = dates[j].Substring(0, 5).Replace(".", "\\");
                    dt.Rows[i][dt.Columns[dates[j]]] = marks[j];
                    avg += Convert.ToInt32(marks[j]);
                }
                
                if (marks.Count > 0)
                {
                    avg /= marks.Count;
                    dt.Rows[i]["Средний"] = Math.Round(avg,2);
                }
                else dt.Rows[i]["Средний"] = "-";

            }
        
            JournalMarks.DataContext = dt;
        }

        private void fillLatenessTable(List<string> subjects, string pupil_ID)
        {
            DataTable dt = new DataTable();

            DateTime start_of_semester = new DateTime(2021, 2, 5);

            dt.Columns.Add("Предмет");
            dt.Columns.Add("Всего минут");

            while (start_of_semester < DateTime.Now)
            {
                string day = start_of_semester.Day.ToString();
                string month = start_of_semester.Month.ToString();

                if (day.Length == 1) day = "0" + day;
                if (month.Length == 1) month = "0" + month;

                dt.Columns.Add(day + "\\" + month);

                start_of_semester = start_of_semester.AddDays(1);
            }


            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            for (int i = 0; i < subjects.Count; i++)
            {
                List<string> dates = new List<string>();
                List<string> lateness = new List<string>();

                dt.Rows.Add(new string[] { subjects[i] });

                Functions.GetPupilLateness(dates, lateness, pupil_ID, subjects[i]);

                int avg = 0;

                for (int j = 0; j < lateness.Count; j++)
                {
                    dates[j] = dates[j].Substring(0, 5).Replace(".", "\\");
                    dt.Rows[i][dt.Columns[dates[j]]] = lateness[j];
                    if (lateness[j] != "н" && lateness[j].Length > 0)
                    {
                        string num = "";
                        if (!Char.IsDigit(lateness[j][0]))
                            continue;
                        if (lateness[j].Length > 1 && !Char.IsDigit(lateness[j][1]))
                            continue;
                        for (int k = 0; k < Math.Min(lateness[j].Length,2); k++)
                        {
                            num += lateness[j][k];
                        }
                        avg += Convert.ToInt32(num);
                    }                                        
                }

                if (lateness.Count > 0)
                {
                    dt.Rows[i]["Всего минут"] = avg;
                }
                else dt.Rows[i]["Всего минут"] = "-";

            }

            JournalLateness.DataContext = dt;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            PupilLoginWindow p = new PupilLoginWindow(connection);
            p.Show();
            Close();
        }

        private void switchButton_Click(object sender, RoutedEventArgs e)
        {
            if (JournalMarks.Visibility == Visibility.Visible)
            {
                JournalMarks.Visibility = Visibility.Hidden;
                JournalLateness.Visibility = Visibility.Visible;

                switchButton.Content = "Перейти к оценкам";

            }
            else
            {
                JournalMarks.Visibility = Visibility.Visible;
                JournalLateness.Visibility = Visibility.Hidden;

                switchButton.Content = "Перейти к опозданиям";
            }
        }
    }
}
