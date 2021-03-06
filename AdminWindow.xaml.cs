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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();

            fillGroupsList();
            fillTeachersList();
            fillSubjectList();

            pupilGroupAdd.SelectedItem = pupilGroupAdd.Items[0];
            pupilGroupDelete.SelectedItem = pupilGroupDelete.Items[0];

            teacherListToDelete.SelectedItem = teacherListToDelete.Items[0];
        }

        private void fillSubjectList()
        {
            var subjects = Functions.GetSubjectsFromSqlServer();

            foreach (string subject in subjects)
            {
                subjectListToDelete.Items.Add(subject);
                subjectListToAddSubject.Items.Add(subject);
            }
        }

        private void fillTeachersList()
        {
            var passwords = new List<string>();
            var teachers = Functions.GetTeachersFromSqlServer();

            foreach (string teacher in teachers)
            {
                teacherListToDelete.Items.Add(teacher);
                teacherListToAddSubject.Items.Add(teacher);
            }              
        }

        private void fillGroupsList()
        {
            var groups = Functions.GetGroupsFromSqlServer();

            foreach (string group in groups)
            {
                pupilGroupAdd.Items.Add(group);
                pupilGroupDelete.Items.Add(group);
                groupListToDelte.Items.Add(group);
                groupsListToAddSubject.Items.Add(group);
            }
                
        }

        private void addPupil(object sender, RoutedEventArgs e)
        {
            string family = pupilFamilyAdd.Text.Split()[0];
            string name_patronymic = pupilFamilyAdd.Text.Split()[1] + " " + pupilFamilyAdd.Text.Split()[2];
            string birthday = PupilBirthdayAdd.Text;
            string group = pupilGroupAdd.SelectedItem.ToString();

            if (family == "")
            {
                MessageBox.Show("Введите фамилию учащегося");
                return;
            }
            else if (!Functions.IsValidDate(birthday))
            {
                MessageBox.Show("Введите дату рождения в формате dd.mm.yyyy");
                return;
            }
            else if (pupilFamilyAdd.Text.Split().Length != 3)
            {
                MessageBox.Show("ФИО состоит из трех слов!");
                return;
            }

            int result = Functions.AddPupil(family,birthday,group,name_patronymic);
            if (result > 0) MessageBox.Show("Учащийся был успешно добавлен");
            else MessageBox.Show("Не удалось добавить учащегося!");
        }

        private void deletePupil(object sender, RoutedEventArgs e)
        {
            string family = pupilFamilyAdd.Text.Split()[0];
            string name_patronymic = pupilFamilyAdd.Text.Split()[1] + " " + pupilFamilyAdd.Text.Split()[2];
            string birthday = PupilBirthdayAdd.Text;
            string group = pupilGroupAdd.SelectedItem.ToString();

            if (family == "")
            {
                MessageBox.Show("Введите фамилию учащегося");
                return;
            }
            else if (!Functions.IsValidDate(birthday))
            {
                MessageBox.Show("Введите дату рождения в формате dd.mm.yyyy");
                return;
            }

            int result = Functions.DeletePupil(family,birthday,group,name_patronymic);
            if (result > 0) MessageBox.Show("Учащийся был успешно удален");
            else MessageBox.Show("Такого учащегося нет!");  
        }

        private void addTeacher(object sender, RoutedEventArgs e)
        {
            var result = Functions.AddTeacher(teacherFioAdd.Text, teacherPasswordAdd.Text);
 
            if (result > 0)
            {
                MessageBox.Show("Преподаватель был успешно добавлен");
                teacherListToDelete.Items.Add(teacherFioAdd.Text);
                teacherListToAddSubject.Items.Add(teacherFioAdd.Text);
            }
            else MessageBox.Show("Не удалось добавить преподавателя!");
        }

        private void deleteTeacher(object sender, RoutedEventArgs e)
        {
            string fio = teacherListToDelete.Text;
            int result = Functions.DeleteTeacher(fio);

            if (result > 0)
            {
                MessageBox.Show("Преподаватель был успешно удален");
                teacherListToDelete.Items.Remove(fio);
                teacherListToAddSubject.Items.Remove(fio);
            }
            else MessageBox.Show("Преподавателя с такими данными нет!");
        }

        private void addSubjectTeacherToGroup(object sender, RoutedEventArgs e)
        {
            string subject = subjectListToAddSubject.SelectedItem.ToString();
            string teacher = teacherListToAddSubject.SelectedItem.ToString();
            string group = groupsListToAddSubject.SelectedItem.ToString();

            int result = Functions.AddSubjectTeacherToGroup(subject,teacher,group);

            if (result > 0) MessageBox.Show($@"Предмет {subject} был успешно добавлен в группу {group}");
            else MessageBox.Show($@"Не удалось добавить предмет {subject} в группу {group}!");
        }

        private void addSubject(object sender, RoutedEventArgs e)
        {
            string subject = subjectAdd.Text;

            if (subject == "")
            {
                MessageBox.Show("введите предмет!");
                return;
            }

            int result = Functions.AddSubject(subject);
            if (result > 0)
            {
                subjectListToDelete.Items.Add(subject);
                subjectListToAddSubject.Items.Add(subject);
                MessageBox.Show($@"Предмет {subject} был успешно добавлен!");
            }      
            else MessageBox.Show($@"Не удалось добавить предмет {subject}");
        }

        private void deleteSubject(object sender, RoutedEventArgs e)
        {
            string subject = subjectListToDelete.SelectedItem.ToString();

            if (subject == "")
            {
                MessageBox.Show("введите предмет!");
                return;
            }

            int result = Functions.DeleteSubject(subject);

            if (result > 0)
            {
                subjectListToDelete.Items.Remove(subject);
                subjectListToAddSubject.Items.Remove(subject);
                MessageBox.Show($@"Предмет {subject} был успешно удален!");
            }         
            else MessageBox.Show($@"Не удалось удалить предмет {subject}");
        }

        private void addGroup(object sender, RoutedEventArgs e)
        {
            string group = groupAdd.Text;

            if (group == "")
            {
                MessageBox.Show("Введите группу!");
                return;
            }

            int result = Functions.AddGroup(group);
            if (result > 0)
            {
                MessageBox.Show($@"Группа {group} была успешно добавлена!");
                groupListToDelte.Items.Add(group);
                groupsListToAddSubject.Items.Add(group);
                pupilGroupAdd.Items.Add(group);
                pupilGroupDelete.Items.Add(group);
            }

            else MessageBox.Show($@"Не удалось добавить группу {group}");
        }

        private void deleteGroup(object sender, RoutedEventArgs e)
        {
            string group = groupListToDelte.SelectedItem.ToString();

            if (group == "")
            {
                MessageBox.Show("Введите группу!");
                return;
            }

            int result = Functions.DeleteGroup(group);

            if (result > 0)
            {
                groupListToDelte.Items.Remove(group);
                groupsListToAddSubject.Items.Remove(group);
                pupilGroupAdd.Items.Remove(group);
                pupilGroupDelete.Items.Remove(group);
                MessageBox.Show($@"Группа {group} была успешно добавлена!");
            }    
            else MessageBox.Show($@"Не удалось добавить группу {group}");
        }

        private void Update_DataBase(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("При обновлении базы данных данные об учащихся и их оценках будут удалены, продолжить?", "Предупреждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                List<string> groups = Functions.GetGroupsFromInternet();
                List<string> group_urls = Functions.GetGroupsFromInternet(true);
                var subjects = Functions.GetSubjectsFromInternet();
                List<string> teachers = Functions.GetTeachersFromInternet();

                Functions.MatchGroupsWithSubjects(group_urls, groups, subjects);
                //Functions.MatchGroupsWithTeachers(ref group_urls, ref groups, ref teachers, ref connection);
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            AdminLoginWindow alw = new AdminLoginWindow();
            alw.Show();
            Close();
        }
    }
}
