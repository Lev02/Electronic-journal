using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;


namespace EJournal
{
    /// <summary>
    /// Логика взаимодействия для TeacherJournalWindow.xaml
    /// </summary>
    public partial class TeacherJournalWindow : Window
    {
        public TeacherJournalWindow(string group, string subject, string teacher, SqlConnection connection)
        {
            InitializeComponent();

            this.connection = connection;

            this.group = group;
            this.subject = subject;
            this.teacher = teacher;

            this.Title = "Журнал группы " + group + " по " + subject;

            TeacherJournalWindow_SizeChanged();
            this.SizeChanged += TeacherJournalWindow_SizeChanged;

            JournalMarks.Visibility = Visibility.Visible;
            JournalLateness.Visibility = Visibility.Hidden;

            updateMarksButton.Visibility = Visibility.Visible;
            updateLatenessButton.Visibility = Visibility.Hidden;

            fillTables(group, subject);

            for (int i = 0; i < JournalMarks.Columns.Count; i++)
            {
                JournalMarks.Columns[i].CanUserResize = false;
                JournalMarks.Columns[i].CanUserReorder = false;
                JournalMarks.Columns[i].CanUserSort = false;
            }

            for (int i = 0; i < JournalLateness.Columns.Count; i++)
            {
                JournalLateness.Columns[i].CanUserResize = false;
                JournalLateness.Columns[i].CanUserReorder = false;
                JournalLateness.Columns[i].CanUserSort = false;
            }
        }

        SqlConnection connection;

        private void TeacherJournalWindow_SizeChanged(object sender = null, SizeChangedEventArgs e = null)
        {
            double buttonWidth = 0.2;
            double buttonHeight = 0.1;
            double datagridWidth = 0.95;
            double datagridHeight = 0.75;

            JournalMarks.Width = this.Width * datagridWidth;
            JournalMarks.Height = this.Height * datagridHeight;

            JournalLateness.Width = this.Width * datagridWidth;
            JournalLateness.Height = this.Height * datagridHeight;

            this.updateMarksButton.Height = this.Height * buttonHeight;
            this.updateLatenessButton.Height = this.Height * buttonHeight;
            this.backButton.Height = this.Height * buttonHeight;
            this.switchButton.Height = this.Height * buttonHeight;

            this.updateMarksButton.Width = this.Width * buttonWidth;
            this.updateLatenessButton.Width = this.Width * buttonWidth;
            this.backButton.Width = this.Width * buttonWidth;
            this.switchButton.Width = this.Width * buttonWidth;

            double button_margin = this.Width * (1.0 - datagridWidth) / 2.0;
            this.backButton.Margin = new Thickness(button_margin, 0, 0, button_margin);
            this.updateMarksButton.Margin = new Thickness(0, 0, 0, button_margin);
            this.updateLatenessButton.Margin = new Thickness(0, 0, 0, button_margin);
            this.switchButton.Margin = new Thickness(0, 0, button_margin, button_margin);

        }

        DataTable dt_Marks;
        DataTable dt_Lateness;

        int columns_amount;
        int rows_amount;
        string group;
        string subject;
        string teacher;
        List<string> pupil_IDs;
        List<string> IoS;
        List<string> pupils;

        private void fillTables(string group, string subject)
        {
            dt_Marks = new DataTable();
            dt_Lateness = new DataTable();
            
            DateTime start_of_semester = new DateTime(2021, 2, 5);

            dt_Marks.Columns.Add(new DataColumn("Фамилия",typeof(string)));
            dt_Marks.Columns[0].ReadOnly = true;

            dt_Lateness.Columns.Add(new DataColumn("Фамилия", typeof(string)));
            dt_Lateness.Columns[0].ReadOnly = true;

            columns_amount = 0;

            while (start_of_semester < DateTime.Now)
            {
                string day = start_of_semester.Day.ToString();
                string month = start_of_semester.Month.ToString();

                if (day.Length == 1) day = "0" + day;
                if (month.Length == 1) month = "0" + month;

                dt_Marks.Columns.Add(day + "\\" + month);
                dt_Lateness.Columns.Add(day + "\\" + month);

                start_of_semester = start_of_semester.AddDays(1);
                columns_amount++;
            }

            pupil_IDs = new List<string>();
            IoS = new List<string>();

            pupils = Functions.GetGroupPupils(group, ref pupil_IDs, ref IoS);
            rows_amount = pupils.Count;
      
            for (int i = 0; i < pupils.Count; i++)
            {
                List<string> marks_dates = new List<string>();
                List<string> marks = new List<string>();

                List<string> lates_dates = new List<string>();
                List<string> lates = new List<string>();

                string fio = pupils[i] + " " + IoS[i].Split()[0][0] + ". " + IoS[i].Split()[1][0] + ".";

                dt_Marks.Rows.Add(new string[] { fio });
                dt_Lateness.Rows.Add(new string[] { fio });

                Functions.GetPupilMarks(marks_dates, marks, pupil_IDs[i], subject);
                Functions.GetPupilLateness(lates_dates, lates, pupil_IDs[i], subject);

                for (int j = 0; j < marks.Count; j++)
                {
                    marks_dates[j] = marks_dates[j].Substring(0, 5).Replace(".", @"\");
                    dt_Marks.Rows[i][dt_Marks.Columns[marks_dates[j]]] = marks[j];
                }

                for (int j = 0; j < lates.Count; j++)
                {
                    lates_dates[j] = lates_dates[j].Substring(0, 5).Replace(".", @"\");

                    if (lates[j] != "н")
                    {
                        if (!Char.IsDigit(lates[j][0]))
                            continue;
                        if (lates[j].Length > 1 && !Char.IsDigit(lates[j][1]))
                            continue;
                    }

                    dt_Lateness.Rows[i][dt_Lateness.Columns[lates_dates[j]]] = lates[j];
                }
            }
               
            JournalMarks.ItemsSource = dt_Marks.DefaultView;
            JournalLateness.ItemsSource = dt_Lateness.DefaultView;  
        }

        private void saveMarksButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.deleteOldGrades(dt_Marks, group, subject, pupil_IDs);

            dt_Marks = ((DataView)JournalMarks.ItemsSource).ToTable();

            Functions.updateNewGrades(dt_Marks, group, subject, pupil_IDs);

            MessageBox.Show("Оценки были успешно сохранены в базу данных!");
        }

        private void saveLatenessButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.deleteOldLateness(dt_Lateness, group, subject, pupil_IDs);

            dt_Lateness = ((DataView)JournalLateness.ItemsSource).ToTable();

            Functions.updateNewLateness(dt_Lateness, group, subject, pupil_IDs);
            MessageBox.Show("Опоздания были успешно сохранены в базу данных!");
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            TeacherLoginSuccessWindow teacherLoginSuccessWindow = new TeacherLoginSuccessWindow(connection, teacher);
            teacherLoginSuccessWindow.Show();
            Close();
        }

        private void switchButton_Click(object sender, RoutedEventArgs e)
        {
            if (JournalMarks.Visibility == Visibility.Visible)
            {
                JournalMarks.Visibility = Visibility.Hidden;
                JournalLateness.Visibility = Visibility.Visible;

                switchButton.Content = "Перейти к оценкам";
                updateLatenessButton.Visibility = Visibility.Visible;
                updateMarksButton.Visibility = Visibility.Hidden;

            }
            else
            {
                JournalMarks.Visibility = Visibility.Visible;
                JournalLateness.Visibility = Visibility.Hidden;

                switchButton.Content = "Перейти к опозданиям";
                updateLatenessButton.Visibility = Visibility.Hidden;
                updateMarksButton.Visibility = Visibility.Visible;
            }
        }
    }
}
