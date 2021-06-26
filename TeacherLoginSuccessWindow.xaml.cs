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
    /// Логика взаимодействия для TeacherLoginSuccessWindow.xaml
    /// </summary>
    public partial class TeacherLoginSuccessWindow : Window
    {
        private string _teacher;

        public TeacherLoginSuccessWindow(string teacher)
        {
            InitializeComponent();

            _teacher = teacher;
            updateGroups(teacher);
            GroupList.SelectionChanged += GroupList_SelectionChanged;
        }
        private void GroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateSubjects();
        }

        private void updateGroups(string teacher, bool use_internet = false)
        {
            var groups = Functions.GetGroupsFromSqlServer(teacher);
            foreach (var group in groups) GroupList.Items.Add(group);
        }

        private void updateSubjects()
        {
            var subjects = Functions.GetGroupSubjectsTeachers(GroupList.SelectedItem.ToString());
            SubjectList.Items.Clear();
       
            foreach (var subjectTeacher in subjects)
            {
                if (_teacher == subjectTeacher.Value) SubjectList.Items.Add(subjectTeacher.Key);
            }
            
            SubjectList.SelectedItem = SubjectList.Items[0];
        }

        private void Enter_Journal(object sender, RoutedEventArgs e)
        {
            if (GroupList.SelectedItem == null)
            {               
                MessageBox.Show("Выберите группу");
                return;
            }
            if (SubjectList.SelectedItem == null)
            {
                MessageBox.Show("Выберите предмет группы");
                return;
            }
            string group = GroupList.SelectedItem.ToString();
            string subject = SubjectList.SelectedItem.ToString();

            TeacherJournalWindow t = new TeacherJournalWindow(group, subject, _teacher);
            t.Show();
            Close();
        }

        private void Back_Button(object sender, RoutedEventArgs e)
        {
            TeacherLoginWindow teacherLoginWindow = new TeacherLoginWindow();
            teacherLoginWindow.Show();
            Close();
        }

        private void ReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Functions.GetReference());
        }
    }
}
