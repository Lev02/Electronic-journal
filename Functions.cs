using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using System.Data;

namespace EJournal
{
    public class Functions
    {
        private static SqlConnection _connection;

        public static SqlConnection Connection
        {
            get => _connection;
            set => _connection = value;
        }

        public static bool IsValidGroup(string group)
        {
            string russianAlphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            return
                group.Length == 5 &&
                russianAlphabet.Contains(group[0]) &&
                group[1] == '-' &&
                Char.IsDigit(group[2]) &&
                Char.IsDigit(group[3]) &&
                Char.IsDigit(group[4]);
        }

        public static bool IsValidDate(string date)
        {
            if (date.Length != 10)
                return false;
            foreach (char s in date)
                if (!Char.IsDigit(s) && (s != '.' && s != '-'))
                    return false;
            return true;
        }

        public static bool InternetIsAvailable()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetHtml(string urlAddress)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (String.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                return data;
            }

            return "";

        }

        public static List<string> GetGroupsFromInternet(bool get_urls = false)
        {
            string html = GetHtml("https://kbp.by/rasp/timetable/view_beta_kbp/");
            List<string> groups = new List<string>();

            if (get_urls)
            {
                string template = "group&amp;id=\\d+\">.+?(?=<)";
                Regex regex = new Regex(template);
                MatchCollection matches = regex.Matches(html);

                string val;
                foreach (Match match in matches)
                {
                    val = match.Value;
                    // записываем то, что начинается после символа >
                    groups.Add("?page=stable&cat=group&id=" + val.Substring(13,val.IndexOf("\">")-13));
                }
            }
            else
            {
                for (int i = 0; i < html.Length - 5; i++)
                {
                    if (IsValidGroup(html.Substring(i, 5)))
                    {
                        groups.Add(html.Substring(i, 5));
                    }
                }

                WriteGroupsToSqlServer(groups);
            }          

            return groups;
        }

        public static List<string> GetGroupsFromSqlServer(string teacher = null)
        {
            List<string> groups = new List<string>();
            string sqlExpression = null;

            if (teacher != null)
                sqlExpression = "SELECT DISTINCT Группа FROM ГруппыПредметы WHERE Учитель = '" + teacher + "'";
            else sqlExpression = "SELECT * FROM Группы";

            var reader = ExecuteQuery(sqlExpression);
            while (reader.Read())
            {
                groups.Add(reader.GetValue(0).ToString());
            }
            reader.Close();
            
            return groups;
        }

        //при вызове функции будут пересозданы таблицы Учащиеся и ПредметыГруппы

      
        public static void dropGroupsSubjects( )
        {
            try
            {
                string dropGroupsSubjectsSqlExpression = "DROP TABLE ГруппыПредметы";
                ExecuteQuery(dropGroupsSubjectsSqlExpression);
            }
            catch { }        
        }

        public static void dropGroupsTeachers( )
        {
            try
            {
                string dropGroupsTeachersSqlExpression = "DROP TABLE ГруппыУчителя";
                ExecuteQuery(dropGroupsTeachersSqlExpression);
            }
            catch { }
        }

        public static void dropPupilMarks( )
        {
            try
            {
                string dropPupilsMarksSqlExpression = "DROP TABLE УчащиесяОценки";
                ExecuteQuery(dropPupilsMarksSqlExpression);
            }
            catch { }
        }

        public static void dropPupilLateness( )
        {
            try
            {
                string dropPupilsLatenessSqlExpression = "DROP TABLE УчащиесяОпозданияОтсутствия";
                ExecuteQuery(dropPupilsLatenessSqlExpression);
            }
            catch { }
        }

        public static void dropPupils( )
        {
            try
            {
                string dropPupilsTableSqlExpression = "DROP TABLE Учащиеся";
                ExecuteQuery(dropPupilsTableSqlExpression);
            }
            catch { }
        }

        public static void createGroupsSubjects( )
        {
            try
            {
                string createGroupsSubjectsTableSqlExpression = "CREATE TABLE ГруппыПредметы (" +
                " Группа nvarchar(5)," +
                " FOREIGN KEY(Группа)" +
                " ERENCES Группы(Номер)" +
                " ON UPDATE CASCADE " +
                "ON DELETE CASCADE," +
                " Предмет nvarchar(50)," +
                " FOREIGN KEY(Предмет)" +
                " ERENCES Предметы(Название)" +
                " ON UPDATE CASCADE" +
                " ON DELETE CASCADE, " +
                "Учитель nvarchar(50) NOT NULL," +
                 ")";
                ExecuteQuery(createGroupsSubjectsTableSqlExpression);
            }
            catch { }
        }

        public static void createGroupsTeachers( )
        {
            try
            {
                string createGroupsTeachersTableSqlExpression = "CREATE TABLE ГруппыУчителя (" +
               "Группа nvarchar(5)," +
               "FOREIGN KEY(Группа) " +
               "ERENCES Группы(Номер)" +
               " ON UPDATE CASCADE " +
               "ON DELETE CASCADE," +
               " Учитель nvarchar(50) NOT NULL" +
               ")";
                ExecuteQuery(createGroupsTeachersTableSqlExpression);
            }
            catch { }
        }

        public static void createTeacher( )
        {
            try
            {
                string exp = "CREATE TABLE Преподаватели (  ID int PRIMARY KEY IDENTITY(1,1), ФИО nvarchar(50) NOT NULL, Пароль nvarchar(50) NOT NULL, ) ";
                ExecuteQuery(exp);
            }
            catch { }
        }

        public static void dropTeacher( )
        {
            try
            {
                string exp = "DROP TABLE Преподаватели";
                ExecuteQuery(exp);
            }
            catch { }
        }

        public static void createPupils( )
        {
            try
            {
                string createPupilsTableSqlExpression = "CREATE TABLE Учащиеся (" +
                " ID int PRIMARY KEY IDENTITY(1,1)," +
                " Фамилия nvarchar(50) NOT NULL," +
                " ФИО nvarchar(50)," +
                " ДатаРождения date NOT NULL," +
                " НомерГруппы nvarchar(5) NOT NULL," +
                " FOREIGN KEY(НомерГруппы)" +
                " ERENCES Группы(Номер)" +
                " ON UPDATE CASCADE" +
                " ON DELETE CASCADE," +
                ")"
                ;
                ExecuteQuery(createPupilsTableSqlExpression);
            }
            catch { }
        }

       

        public static void createPupilsMarks( )
        {
            try
            {
                string createPupilsMarksSqlExpression =
                "CREATE TABLE УчащиесяОценки" +
                " (  ID_учащегося int NOT NULL," +
                " FOREIGN KEY(ID_учащегося) " +
                " ERENCES Учащиеся(ID)" +
                "  ON UPDATE CASCADE" +
                " ON DELETE CASCADE," +
                "  Дата date NOT NULL, " +
                " Предмет nvarchar(50) NOT NULL," +
                " FOREIGN KEY(Предмет)  " +
                "ERENCES Предметы(Название)  " +
                "ON UPDATE CASCADE ON DELETE CASCADE, " +
                " Оценка int NOT NULL CHECK(Оценка > 0 AND Оценка< 11) " +
                ")";
                ExecuteQuery(createPupilsMarksSqlExpression);
            }
            catch { }
        }

        public static void createPupilsLateness( )
        {
            try
            {
                string createPupilsLatenessSqlExpression =
                "CREATE TABLE УчащиесяОпозданияОтсутствия (  " +
                "ID_учащегося int NOT NULL," +
                " FOREIGN KEY(ID_учащегося)" +
                "  ERENCES Учащиеся(ID) " +
                " ON UPDATE CASCADE " +
                "ON DELETE CASCADE," +
                "  Дата date NOT NULL, " +
                " Предмет nvarchar(50) NOT NULL, " +
                "FOREIGN KEY(Предмет)  " +
                "ERENCES Предметы(Название)" +
                "  ON UPDATE CASCADE " +
                "ON DELETE CASCADE, " +
                " МинутОпоздания nvarchar(2) NOT NULL" +
                " )";
                ExecuteQuery(createPupilsLatenessSqlExpression);
            }
            catch { }
        }

        public static void WriteGroupsToSqlServer(List<string> groups)
        {
            string clearGroupsSqlExpression = "TRUNCATE TABLE Группы";
            string insertGroupSqlExpression = "INSERT INTO Группы(Номер) VALUES ";

            dropGroupsSubjects();
            dropGroupsTeachers();
            dropPupilMarks();
            dropPupilLateness();
            dropPupils();

            ExecuteQuery(clearGroupsSqlExpression);

            createGroupsSubjects();
            createGroupsTeachers();
            createPupils();
            createPupilsMarks();
            createPupilsLateness();

            foreach (string group in groups) 
                ExecuteQuery(insertGroupSqlExpression + "('" + group + "');");
        }

        public static void WriteTeachersToSqlServer(List<string> teachers)
        {
            dropTeacher();
            createTeacher();

            string insertTeacherSqlExpression = "INSERT INTO Преподаватели(ФИО, Пароль) VALUES ";

            foreach (string teacher in teachers)
            {
                //пароль по умолчанию - MishaMazBelaz
                ExecuteQuery(insertTeacherSqlExpression + "('" + teacher + "'," + "'MishaMazBelaz');");
            }       
        }

        public static List<string> GetSubjectsFromInternet( )
        {
            List<string> subjects = new List<string>();
            string html = GetHtml("https://kbp.by/rasp/timetable/view_beta_kbp/?q=");

            string template = "subject&amp;id=\\d+\">.+?(?=<)";

            Regex regex = new Regex(template);
            MatchCollection matches = regex.Matches(html);

            string val;
            foreach (Match match in matches)
            {
                val = match.Value;
                
                // записываем то, что начинается после символа >
                subjects.Add(val.Substring(val.IndexOf('>') + 1));
                
            }

            WriteSubjectsToSqlServer(subjects);

            return subjects;
        }

        //при вызове функции учесть, что пересоздасться таблица ПредметыГруппы
        public static void WriteSubjectsToSqlServer(List<string> subjects)
        {
            string clearSubjectsSqlExpression = "TRUNCATE TABLE Предметы";
            string insertTeacherSqlExpression = "INSERT INTO Предметы(Название) VALUES ";

            //очистка таблицы
            dropGroupsSubjects();
            dropPupilMarks();
            dropPupilLateness();

            ExecuteQuery(clearSubjectsSqlExpression);

            createGroupsSubjects();
            createPupilsMarks();
            createPupilsLateness();

            foreach (string subject in subjects)
                //пароль по умолчанию - MishaMazBelaz
                ExecuteQuery(insertTeacherSqlExpression + "('" + subject + "');");
        }

        public static void MatchGroupsWithTeachers(List<string> groupUrls, List<string> groups, List<string> teachers)
        {
            if (groupUrls.Count != groups.Count)
                throw new Exception("Количество ссылок на группы должно быть равным количеству групп");

            string clearTableSqlExpression = "TRUNCATE TABLE ГруппыУчителя";
            ExecuteQuery(clearTableSqlExpression);

            for (int i = 0; i < groupUrls.Count; i++)
            {
                string html = GetHtml("https://kbp.by/rasp/timetable/view_beta_kbp/" + groupUrls[i]);
                List<string> matchingTeachers = new List<string>();
                foreach (string teacher in teachers)
                {
                    if (html.Contains(teacher))
                        matchingTeachers.Add(teacher);
                }
                matchingTeachers = matchingTeachers.Distinct().ToList();

                WriteMatchingGroupAndTeacherToSqlServer(groups[i], matchingTeachers);
            }
        }

        public static void WriteMatchingGroupAndTeacherToSqlServer(string group,  List<string> matchingTeachers)
        {
            string insertTeacherSqlExpression = "INSERT INTO ГруппыУчителя(Группа,Учитель) VALUES ";
            foreach (string teacher in matchingTeachers)
                ExecuteQuery(insertTeacherSqlExpression + "('" + group + "'" + ",'" + teacher + "');");
        }

        public static void MatchGroupsWithSubjects(List<string> groupUrls, List<string> groups, List<string> subjects)
        {
            if (groupUrls.Count != groups.Count)
                throw new Exception("Количество ссылок на группы должно быть равным количеству групп");

            string clearTableSqlExpression = "TRUNCATE TABLE ГруппыПредметы";
            ExecuteQuery(clearTableSqlExpression);

            for (int i = 0; i < groupUrls.Count; i++)
            {
                string html = GetHtml("https://kbp.by/rasp/timetable/view_beta_kbp/" + groupUrls[i]);
                Dictionary<string, string> matchingSubjectsTeachers = new Dictionary<string, string>();
                foreach (string subject in subjects)
                {
                    if (html.Contains(subject))
                    {
                        string part = html.Substring(html.IndexOf(subject));
                        part = part.Substring(part.IndexOf("<a h=\"?cat=teacher&amp;id="));
                        part = part.Substring(part.IndexOf(">"));
                        string teacher = "";
                        for (int k = 1; part[k] != '<'; k++)
                            teacher += part[k];

                        matchingSubjectsTeachers[subject] = teacher;
                    }
                        
                }
                matchingSubjectsTeachers = matchingSubjectsTeachers.Distinct().ToDictionary(x=>x.Key,x=>x.Value);

                WriteMatchingGroupAndSubjectToSqlServer(groups[i],  matchingSubjectsTeachers);
            }
        }

        public static void WriteMatchingGroupAndSubjectToSqlServer(string group,  Dictionary<string,string> matchingSubjectsTeachers)
        {  
            string insertTeacherSqlExpression = "INSERT INTO ГруппыПредметы(Группа,Предмет,Учитель) VALUES ";

            foreach (var subjectTeacher in matchingSubjectsTeachers)
                ExecuteQuery(insertTeacherSqlExpression + "('" + group + "'" + ",'" + subjectTeacher.Key + "','" + subjectTeacher.Value + "');");
        }

        public static List<string> GetTeachersFromInternet( )
        {
            List<string> teachers = new List<string>();

            string html = GetHtml("https://kbp.by/rasp/timetable/view_beta_kbp/?q=");
            
            string template = "teacher&amp;id=\\d+\">.+?(?=<)";

            Regex regex = new Regex(template);
            MatchCollection matches = regex.Matches(html);

            string val;
            foreach (Match match in matches)
            {
                val = match.Value;
                // записываем имя, которое начинается после символа >
                teachers.Add(val.Substring(val.IndexOf('>') + 1));
            }
            WriteTeachersToSqlServer(teachers);
                
            return teachers;           
        }

        public static List<string> GetTeachersFromSqlServer(List<string> passwords)
        {
            List<string> teachers = new List<string>();
            passwords = new List<string>();

            string sqlExpression = "SELECT ФИО,Пароль FROM Преподаватели";

            var reader = ExecuteQuery(sqlExpression);
            while (reader.Read())
            {
                teachers.Add(reader.GetValue(0).ToString());
                passwords.Add(reader.GetValue(1).ToString());
            }
            reader.Close();
            
            return teachers;
        }

       

        public static bool ValidateAdmin(string login, string password)
        {
            string sqlExpression =
                $@"SELECT * FROM Администраторы WHERE
                    Логин='{login}' AND 
                    Пароль='{password}'"; 

            var reader = ExecuteQuery(sqlExpression);
            bool valid = reader.Read();
            reader.Close();
            return valid;
        }

        public static bool ValidatePupil(string secondName, string birthday, string group)
        {
            string sqlExpression = "SELECT * FROM Учащиеся" +
                " WHERE Фамилия='" + secondName
                + "' AND ДатаРождения='" + birthday 
                + "' AND НомерГруппы='" + group + "'";


            var reader = ExecuteQuery(sqlExpression);

            bool valid = reader.Read();
            reader.Close();
            return valid;
        }

        public static bool ValidateTeacher(string fio, string password)
        {
            string sqlExpression = "SELECT * FROM Преподаватели" +
                " WHERE ФИО='" + fio
                + "' AND Пароль='" + password + "'";

            var reader = ExecuteQuery(sqlExpression);

            bool valid = reader.Read();
            reader.Close();
            return valid;
        }

        public static Dictionary<string,string> GetGroupSubjectsTeachers(string group)
        {
            Dictionary<string, string> subjectsTeachers = new Dictionary<string, string>();

            string sqlExpression = "SELECT Предмет,Учитель FROM ГруппыПредметы" +
                " WHERE Группа='" + group + "'";

            var reader = ExecuteQuery(sqlExpression);

            while (reader.Read())
            {
                subjectsTeachers[reader.GetValue(0).ToString()] = reader.GetValue(1).ToString();
            }
            reader.Close();
            
            return subjectsTeachers;
        }

        public static List<string> GetSubjectsFromSqlServer()
        {
            List<string> subjects = new List<string>();
            string sqlExpression = "SELECT Название FROM Предметы";

            var reader = ExecuteQuery(sqlExpression);

            while (reader.Read())
            {
                subjects.Add(reader.GetValue(0).ToString());
            }
            reader.Close();
           
            return subjects;
        }

        public static List<string> GetGroupPupils(string group, ref List<string> pupil_IDs, ref List<string> IoS)
        {
            List<string> pupils = new List<string>();
            pupil_IDs = new List<string>();

            string sqlExpression = "SELECT ID, Фамилия, ИмяОтчество FROM Учащиеся WHERE НомерГруппы='" + group + "'";

            var reader = ExecuteQuery(sqlExpression);

            while (reader.Read())
            {
                pupil_IDs.Add(reader.GetValue(0).ToString());
                pupils.Add(reader.GetValue(1).ToString());
                IoS.Add(reader.GetValue(2).ToString());
            }
            reader.Close();

            return pupils;
        }

        public static void deleteOldGrades( DataTable dt, string group, string subject,  List<string> pupil_IDs)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString() == "")
                    continue;
                string secondName = dt.Rows[i][0].ToString();
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString().Replace(" ", "") != "")
                    {
                        string date = dt.Columns[j].ColumnName.Replace("\\",".") + "." + DateTime.Now.Year;
                        string pupil_ID = pupil_IDs[i];
                        string mark = dt.Rows[i][j].ToString().Replace(" ", "");

                        var isNumeric = int.TryParse(mark, out _);

                        if (isNumeric)
                        {
                            string sqlDeleteExpression =
                            "DELETE FROM УчащиесяОценки WHERE " +
                            "Дата ='" + date + "' AND " +
                            "Предмет ='" + subject + "' AND " +
                            "ID_учащегося ='" + pupil_ID + "' AND " +
                            "Оценка ='" + mark + "'";
                            ;

                            ExecuteNonQuery(sqlDeleteExpression);
                        }
                        else
                        {
                            dt.Rows[i][j] = "";
                        }
                    }
                }
            }
        }
        public static void updateNewGrades(DataTable dt, string group, string subject, List<string> pupil_IDs)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //if (dt.Rows[i][0].ToString() == "")
                //    continue;
                string secondName = dt.Rows[i][0].ToString();
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString().Replace(" ", "") != "")
                    {
                        string date = dt.Columns[j].ColumnName.Replace("\\", ".") + "." + DateTime.Now.Year;
                        string pupil_ID = pupil_IDs[i];
                        string mark = dt.Rows[i][dt.Columns[j]].ToString().Replace(" ", "");

                        var isNumeric = int.TryParse(mark, out _);

                        if (isNumeric)
                        {
                            string sqlInsertExpression = $"INSERT INTO УчащиесяОценки(ID_учащегося, Дата, Предмет, Оценка)" +
                            $" VALUES('{pupil_ID}','{date}','{subject}', '{mark}')";

                            ExecuteNonQuery(sqlInsertExpression);
                        }
                        else
                        {
                            dt.Rows[i][j] = "";
                        }                          
                    }
                }
            }
        }

        public static void deleteOldLateness(DataTable dt, string group, string subject, List<string> pupil_IDs)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString() == "")
                    continue;
                string secondName = dt.Rows[i][0].ToString();
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString().Replace(" ", "") != "")
                    {
                        string date = dt.Columns[j].ColumnName.Replace("\\", ".") + "." + DateTime.Now.Year;
                        string pupil_ID = pupil_IDs[i];
                        string mark = dt.Rows[i][j].ToString().Replace(" ", "");

                        string sqlDeleteExpression =
                            "DELETE FROM УчащиесяОпозданияОтсутствия WHERE " +
                            "Дата ='" + date + "' AND " +
                            "Предмет ='" + subject + "' AND " +
                            "ID_учащегося ='" + pupil_ID + "' AND " +
                            "МинутОпоздания ='" + mark + "'";
                        ;

                        ExecuteNonQuery(sqlDeleteExpression);
                    }
                }
            }       
        }
        public static void updateNewLateness(DataTable dt, string group, string subject, List<string> pupil_IDs)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString() == "")
                    continue;
                string secondName = dt.Rows[i][dt.Columns[0]].ToString();
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString().Replace(" ", "") != "")
                    {
                        string date = dt.Columns[j].ColumnName.Replace(@"\", ".") + "." + DateTime.Now.Year;
                        string pupil_ID = pupil_IDs[i];
                        string mark = dt.Rows[i][j].ToString().Replace(" ", "");

                        if (mark.Length > 2)
                            mark = mark.Substring(0, 2);

                        if (mark != "н")
                        {
                            bool ok = true;
                            foreach (char c in mark)
                            {
                                if (!Char.IsDigit(c))
                                {
                                    dt.Rows[i][j] = "";
                                    ok = false;
                                    continue;
                                }
                            }
                            if (!ok) continue;
                            else if (Convert.ToInt32(mark) > 90 || Convert.ToInt32(mark) < 0)
                                continue;
                        }
                        
                        string sqlInsertExpression = $"INSERT INTO УчащиесяОпозданияОтсутствия(ID_учащегося, Дата, Предмет, МинутОпоздания)" +
                            $" VALUES('{pupil_ID}','{date}','{subject}', '{mark}')";

                        ExecuteNonQuery(sqlInsertExpression);
                    }
                }
            }
        }
 

        public static void GetPupilMarks(List<string> dates, List<string> marks, string pupil_ID, string subject)
        {
            string sqlExpression = $"SELECT Дата, Оценка FROM УчащиесяОценки " +
                $"WHERE ID_учащегося='{pupil_ID}' AND Предмет='{subject}'";

            var reader = ExecuteQuery(sqlExpression);
            while (reader.Read())
            {
                dates.Add(reader.GetValue(0).ToString());
                marks.Add(reader.GetValue(1).ToString());
            }
            reader.Close();
        }

        public static void GetPupilLateness(List<string> dates, List<string> lateness, string pupil_ID, string subject)
        {
            string sqlExpression = $"SELECT Дата, МинутОпоздания FROM УчащиесяОпозданияОтсутствия " +
                $"WHERE ID_учащегося='{pupil_ID}' AND Предмет='{subject}'";

            var reader = ExecuteQuery(sqlExpression);
            while (reader.Read())
            {
                dates.Add(reader.GetValue(0).ToString());
                lateness.Add(reader.GetValue(1).ToString());
            }
            reader.Close();
        }

        public static string GetPupilID(string group, string secondName, string birthday, string io=null)
        {
            string ID = "1";
            string sqlExpression;
            if (io == null)
            {
                sqlExpression = "SELECT ID FROM Учащиеся WHERE" +
                $" НомерГруппы='{group}' AND Фамилия='{secondName}' AND ДатаРождения='{birthday}'";
            }
            else
            {
                sqlExpression = "SELECT ID FROM Учащиеся WHERE" +
                $" НомерГруппы='{group}' AND Фамилия='{secondName}' AND ДатаРождения='{birthday}' AND ИмяОтчество='{io}'";
            }

            var reader = ExecuteQuery(sqlExpression);

            if (reader.Read())
                ID = reader.GetValue(0).ToString();
            reader.Close();

            return ID;
        }

        public static string GetReference()
        {
            string reference = $@"
Данное приложение создано для учеников и учащихся ЧУО 'КБиП'

В систему можно войти как администратор, учащийся или преподаватель

При входе как администратор требуется ввсти логин и пароль администратора.
В панели администратора можно всячески изменять базу данных электронного журнала,
то есть добавлять и удалять учащихся, преподавателей, групппы, предметы.

При входе как учащийся требуется выбрать ФИО, группы и ввести дату рождения учащегося
В окне учащегося есть возможность просматривать оценки и опоздания учащегося по каждому предмету

При входе как преподаватель требуется выбрать преподавателя и ввести пароль.
После этого нужно выбрать группу и предмет, оценки которой требуется изменить.
В панели преподавателя есть возможность изменять оценки, опоздания и отсутствия группы
в любую дату.
";
            return reference;
        }

        public static SqlDataReader ExecuteQuery(string sqlExpression)
        {
            SqlCommand command = new SqlCommand(sqlExpression, Connection);
            var reader = command.ExecuteReader();
            return reader;
        }
        public static void ExecuteNonQuery(string sqlExpression)
        {
            SqlCommand command = new SqlCommand(sqlExpression, Connection);
            command.ExecuteNonQuery();
        }
    }
}
