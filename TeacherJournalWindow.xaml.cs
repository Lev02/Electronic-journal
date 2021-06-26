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
        private DataTable _dt_Marks;
        private DataTable _dt_Lateness;

        private int _columns_amount;
        private int _rows_amount;

        private string _group;
        private string _subject;
        private string _teacher;

        private List<string> _pupil_IDs;
        private List<string> _IoS;
        private List<string> _pupils;

        public TeacherJournalWindow(string group, string subject, string teacher)
        {
            InitializeComponent();

            _group = group;
            _subject = subject;
            _teacher = teacher;

            Title = "Журнал группы " + group + " по " + subject;

            TeacherJournalWindow_SizeChanged();
            SizeChanged += TeacherJournalWindow_SizeChanged;

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

        private void TeacherJournalWindow_SizeChanged(object sender = null, SizeChangedEventArgs e = null)
        {
            double buttonWidth = 0.2;
            double buttonHeight = 0.1;
            double datagridWidth = 0.95;
            double datagridHeight = 0.75;

            JournalMarks.Width = Width * datagridWidth;
            JournalMarks.Height = Height * datagridHeight;

            JournalLateness.Width = Width * datagridWidth;
            JournalLateness.Height = Height * datagridHeight;

            updateMarksButton.Height = Height * buttonHeight;
            updateLatenessButton.Height = Height * buttonHeight;
            backButton.Height = Height * buttonHeight;
            switchButton.Height = Height * buttonHeight;

            updateMarksButton.Width = Width * buttonWidth;
            updateLatenessButton.Width = Width * buttonWidth;
            backButton.Width = Width * buttonWidth;
            switchButton.Width = Width * buttonWidth;

            double button_margin = Width * (1.0 - datagridWidth) / 2.0;
            backButton.Margin = new Thickness(button_margin, 0, 0, button_margin);
            updateMarksButton.Margin = new Thickness(0, 0, 0, button_margin);
            updateLatenessButton.Margin = new Thickness(0, 0, 0, button_margin);
            switchButton.Margin = new Thickness(0, 0, button_margin, button_margin);

        }

        
        private void fillTables(string group, string subject)
        {
            _dt_Marks = new DataTable();
            _dt_Lateness = new DataTable();
            
            DateTime start_of_semester = new DateTime(2021, 2, 5);

            _dt_Marks.Columns.Add(new DataColumn("Фамилия",typeof(string)));
            _dt_Marks.Columns[0].ReadOnly = true;

            _dt_Lateness.Columns.Add(new DataColumn("Фамилия", typeof(string)));
            _dt_Lateness.Columns[0].ReadOnly = true;

            _columns_amount = 0;

            while (start_of_semester < DateTime.Now)
            {
                string day = start_of_semester.Day.ToString();
                string month = start_of_semester.Month.ToString();

                if (day.Length == 1) day = "0" + day;
                if (month.Length == 1) month = "0" + month;

                _dt_Marks.Columns.Add(day + "\\" + month);
                _dt_Lateness.Columns.Add(day + "\\" + month);

                start_of_semester = start_of_semester.AddDays(1);
                _columns_amount++;
            }

            _pupil_IDs = new List<string>();
            _IoS = new List<string>();

            _pupils = Functions.GetGroupPupils(group, ref _pupil_IDs, ref _IoS);
            _rows_amount = _pupils.Count;
      
            for (int i = 0; i < _pupils.Count; i++)
            {
                List<string> marks_dates = new List<string>();
                List<string> marks = new List<string>();

                List<string> lates_dates = new List<string>();
                List<string> lates = new List<string>();

                string fio = _pupils[i] + " " + _IoS[i].Split()[0][0] + ". " + _IoS[i].Split()[1][0] + ".";

                _dt_Marks.Rows.Add(new string[] { fio });
                _dt_Lateness.Rows.Add(new string[] { fio });

                Functions.GetPupilMarks(marks_dates, marks, _pupil_IDs[i], subject);
                Functions.GetPupilLateness(lates_dates, lates, _pupil_IDs[i], subject);

                for (int j = 0; j < marks.Count; j++)
                {
                    marks_dates[j] = marks_dates[j].Substring(0, 5).Replace(".", @"\");
                    _dt_Marks.Rows[i][_dt_Marks.Columns[marks_dates[j]]] = marks[j];
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

                    _dt_Lateness.Rows[i][_dt_Lateness.Columns[lates_dates[j]]] = lates[j];
                }
            }
               
            JournalMarks.ItemsSource = _dt_Marks.DefaultView;
            JournalLateness.ItemsSource = _dt_Lateness.DefaultView;  
        }

        private void saveMarksButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.DeleteOldGrades(_dt_Marks, _group, _subject, _pupil_IDs);

            _dt_Marks = ((DataView)JournalMarks.ItemsSource).ToTable();

            Functions.UpdateNewGrades(_dt_Marks, _group, _subject, _pupil_IDs);
            MessageBox.Show("Оценки были успешно сохранены в базу данных!");
        }

        private void saveLatenessButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.DeleteOldLateness(_dt_Lateness, _group, _subject, _pupil_IDs);

            _dt_Lateness = ((DataView)JournalLateness.ItemsSource).ToTable();

            Functions.UpdateNewLateness(_dt_Lateness, _group, _subject, _pupil_IDs);
            MessageBox.Show("Опоздания были успешно сохранены в базу данных!");
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            TeacherLoginSuccessWindow teacherLoginSuccessWindow = new TeacherLoginSuccessWindow(_teacher);
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
