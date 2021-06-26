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
    public static class Functions
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
            if (date.Length != 10) return false;

            foreach (char s in date)
            {
                if (!Char.IsDigit(s) && (s != '.' && s != '-')) return false;
            }     
            
            return true;
        }

        public static bool InternetIsAvailable()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204")) return true;
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

                if (String.IsNullOrWhiteSpace(response.CharacterSet)) readStream = new StreamReader(receiveStream);
                else readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

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
                string regex_template = "group&amp;id=\\d+\">.+?(?=<)";
                Regex regex = new Regex(regex_template);
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
                    if (IsValidGroup(html.Substring(i, 5))) groups.Add(html.Substring(i, 5));
                }
                WriteGroupsToSqlServer(groups);
            }          

            return groups;
        }

        public static List<string> GetGroupsFromSqlServer(string teacher = null)
        {
            List<string> groups = new List<string>();
            string sqlExpression = null;

            if (teacher != null) sqlExpression = "SELECT DISTINCT Группа FROM ГруппыПредметы WHERE Учитель = '" + teacher + "'";
            else sqlExpression = "SELECT * FROM Группы";

            var reader = ExecuteQuery(sqlExpression);
            while (reader.Read()) groups.Add(reader.GetValue(0).ToString());

            reader.Close();
            
            return groups;
        }

        public static void DropGroupsSubjects( )
        {
            try
            {
                ExecuteNonQuery("DROP TABLE ГруппыПредметы");
            }
            catch { }        
        }

        public static void DropGroupsTeachers( )
        {
            try
            {
                ExecuteNonQuery("DROP TABLE ГруппыУчителя");
            }
            catch { }
        }

        public static void DropPupilMarks( )
        {
            try
            {
                ExecuteNonQuery("DROP TABLE УчащиесяОценки");
            }
            catch { }
        }

        public static void DropPupilLateness( )
        {
            try
            {
                ExecuteNonQuery("DROP TABLE УчащиесяОпозданияОтсутствия");
            }
            catch { }
        }

        public static void DropPupils( )
        {
            try
            {
                ExecuteNonQuery("DROP TABLE Учащиеся");
            }
            catch { }
        }

        public static void CreateGroupsSubjects( )
        {
            try
            {
                string sqlExpression = 
                    $@"CREATE TABLE ГруппыПредметы (
                        Группа nvarchar(5),
                        FOREIGN KEY(Группа)
                        REFERENCES Группы(Номер)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE,
                        Предмет nvarchar(50),
                        FOREIGN KEY(Предмет)
                        REFERENCES Предметы(Название)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE,
                        Учитель nvarchar(50) NOT NULL
                    )";
                ExecuteNonQuery(sqlExpression);
            }
            catch { }
        }

        public static void CreateGroupsTeachers( )
        {
            try
            {
                string sqlExpression = 
                    $@"
                    CREATE TABLE ГруппыУчителя ( +
                       Группа nvarchar(5),
                       FOREIGN KEY(Группа)
                       REFERENCES Группы(Номер)
                       ON UPDATE CASCADE
                       ON DELETE CASCADE,
                       Учитель nvarchar(50) NOT NULL
                    )";
                ExecuteNonQuery(sqlExpression);
            }
            catch { }
        }

        public static void CreateTeacher( )
        {
            try
            {
                string sqlExpression = 
                    $@"
                        CREATE TABLE Преподаватели (
                            ID int PRIMARY KEY IDENTITY(1,1),
                            ФИО nvarchar(50) NOT NULL,
                            Пароль nvarchar(50) NOT NULL
                    )";
                ExecuteNonQuery(sqlExpression);
            }
            catch { }
        }

        public static void DropTeacher( )
        {
            try
            {
                ExecuteNonQuery("DROP TABLE Преподаватели");
            }
            catch { }
        }

        public static void CreatePupils( )
        {
            try
            {
                string sqlExpression =
                    $@"
                        CREATE TABLE Учащиеся (
                            ID int PRIMARY KEY IDENTITY(1,1),
                            Фамилия nvarchar(50) NOT NULL,
                            ФИО nvarchar(50),
                            ДатаРождения date NOT NULL,
                            НомерГруппы nvarchar(5) NOT NULL,
                            FOREIGN KEY(НомерГруппы)
                            REFERENCES Группы(Номер)
                            ON UPDATE CASCADE
                            ON DELETE CASCADE
                    )";
                ExecuteNonQuery(sqlExpression);
            }
            catch { }
        }

        public static void CreatePupilsMarks( )
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
                ExecuteNonQuery(createPupilsMarksSqlExpression);
            }
            catch { }
        }

        public static void CreatePupilsLateness( )
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
                ExecuteNonQuery(createPupilsLatenessSqlExpression);
            }
            catch { }
        }

        public static void WriteGroupsToSqlServer(List<string> groups)
        {
            string clearGroupsSqlExpression = "TRUNCATE TABLE Группы";
            string insertGroupSqlExpression = "INSERT INTO Группы(Номер) VALUES ";

            DropGroupsSubjects();
            DropGroupsTeachers();
            DropPupilMarks();
            DropPupilLateness();
            DropPupils();

            ExecuteQuery(clearGroupsSqlExpression);

            CreateGroupsSubjects();
            CreateGroupsTeachers();
            CreatePupils();
            CreatePupilsMarks();
            CreatePupilsLateness();

            foreach (string group in groups) ExecuteNonQuery(insertGroupSqlExpression + "('" + group + "');");

        }

        public static void WriteTeachersToSqlServer(List<string> teachers)
        {
            DropTeacher();
            CreateTeacher();

            string insertTeacherSqlExpression = "INSERT INTO Преподаватели(ФИО, Пароль) VALUES ";

            foreach (string teacher in teachers)
            {
                //пароль по умолчанию - 1111
                ExecuteNonQuery(insertTeacherSqlExpression + "('" + teacher + "'," + "'1111');");
            }       
        }

        public static List<string> GetSubjectsFromInternet( )
        {
            List<string> subjects = new List<string>();
            string html = GetHtml("https://kbp.by/rasp/timetable/view_beta_kbp/?q=");

            string regex_template = "subject&amp;id=\\d+\">.+?(?=<)";

            Regex regex = new Regex(regex_template);
            MatchCollection matches = regex.Matches(html);

            // записываем то, что начинается после символа >
            foreach (Match match in matches) subjects.Add(match.Value.Substring(match.Value.IndexOf('>') + 1));
                
            WriteSubjectsToSqlServer(subjects);

            return subjects;
        }

        //при вызове функции учесть, что пересоздасться таблица ПредметыГруппы
        public static void WriteSubjectsToSqlServer(List<string> subjects)
        {
            string clearSubjectsSqlExpression = "TRUNCATE TABLE Предметы";
            string insertTeacherSqlExpression = "INSERT INTO Предметы(Название) VALUES ";

            //очистка таблицы
            DropGroupsSubjects();
            DropPupilMarks();
            DropPupilLateness();

            ExecuteQuery(clearSubjectsSqlExpression);

            CreateGroupsSubjects();
            CreatePupilsMarks();
            CreatePupilsLateness();

            foreach (string subject in subjects) ExecuteQuery(insertTeacherSqlExpression + "('" + subject + "');");
        }

        public static void MatchGroupsWithTeachers(List<string> groupUrls, List<string> groups, List<string> teachers)
        {
            if (groupUrls.Count != groups.Count) throw new Exception("Количество ссылок на группы должно быть равным количеству групп");

            ExecuteNonQuery("TRUNCATE TABLE ГруппыУчителя");

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
            foreach (string teacher in matchingTeachers) ExecuteNonQuery(insertTeacherSqlExpression + "('" + group + "'" + ",'" + teacher + "');");

        }

        public static void MatchGroupsWithSubjects(List<string> groupUrls, List<string> groups, List<string> subjects)
        {
            if (groupUrls.Count != groups.Count) throw new Exception("Количество ссылок на группы должно быть равным количеству групп");

            ExecuteNonQuery("TRUNCATE TABLE ГруппыПредметы");

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
                        for (int k = 1; part[k] != '<'; k++) teacher += part[k];
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

        public static List<string> GetTeachersFromSqlServer()
        {
            List<string> teachers = new List<string>();

            string sqlExpression = "SELECT ФИО FROM Преподаватели";

            var reader = ExecuteQuery(sqlExpression);
            while (reader.Read())
            {
                teachers.Add(reader.GetValue(0).ToString());
            }
            reader.Close();
            
            return teachers;
        }

       
        public static bool ValidateAdmin(string login, string password)
        {
            string sqlExpression = $@"
                SELECT * FROM Администраторы WHERE
                    Логин='{login}' AND 
                    Пароль='{password}'
            "; 

            var reader = ExecuteQuery(sqlExpression);
            bool valid = reader.Read();
            reader.Close();
            return valid;
        }

        public static bool ValidatePupil(string secondName, string birthday, string group)
        {
            string sqlExpression = $@"
                SELECT * FROM Учащиеся
                WHERE Фамилия='{secondName}'
                AND ДатаРождения='{birthday}'
                AND НомерГруппы='{group}'
            ";

            var reader = ExecuteQuery(sqlExpression);

            bool valid = reader.Read();
            reader.Close();
            return valid;
        }

        public static bool ValidateTeacher(string fio, string password)
        {
            string sqlExpression = $@"
                SELECT * FROM Преподаватели
                WHERE ФИО='{fio}' 
                AND Пароль='{password}'
            ";

            var reader = ExecuteQuery(sqlExpression);

            bool valid = reader.Read();
            reader.Close();
            return valid;
        }

        public static Dictionary<string,string> GetGroupSubjectsTeachers(string group)
        {
            Dictionary<string, string> subjectsTeachers = new Dictionary<string, string>();

            string sqlExpression = $@"
                SELECT Предмет,Учитель FROM ГруппыПредметы
                WHERE Группа='{group}'
            ";

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

            string sqlExpression = $@"
                SELECT ID, Фамилия, ИмяОтчество FROM Учащиеся 
                WHERE НомерГруппы='{group}'
            ";

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

        public static void DeleteOldGrades( DataTable dt, string group, string subject,  List<string> pupil_IDs)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString() == "") continue;
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
                            string sqlDeleteExpression = $@"
                                DELETE FROM УчащиесяОценки WHERE
                                    Дата ='{date}' AND
                                    Предмет ='{subject}' AND 
                                    ID_учащегося ='{pupil_ID}' AND 
                                    Оценка ='{mark}'
                            ";
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
        public static void UpdateNewGrades(DataTable dt, string group, string subject, List<string> pupil_IDs)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString() == "") continue;
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
                            string sqlInsertExpression = $@"
                                INSERT INTO УчащиесяОценки(ID_учащегося, Дата, Предмет, Оценка)
                                VALUES('{pupil_ID}','{date}','{subject}', '{mark}')
                            ";
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

        public static void DeleteOldLateness(DataTable dt, string group, string subject, List<string> pupil_IDs)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString() == "") continue;

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
        public static void UpdateNewLateness(DataTable dt, string group, string subject, List<string> pupil_IDs)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString() == "") continue;
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
                            if (!ok || (Convert.ToInt32(mark) > 90 || Convert.ToInt32(mark) < 0)) continue;
                        }
                        
                        string sqlInsertExpression = $@"
                            INSERT INTO УчащиесяОпозданияОтсутствия(ID_учащегося, Дата, Предмет, МинутОпоздания)
                            VALUES('{pupil_ID}','{date}','{subject}', '{mark}')
                        ";

                        ExecuteNonQuery(sqlInsertExpression);
                    }
                }
            }
        }
 

        public static void GetPupilMarks(List<string> dates, List<string> marks, string pupil_ID, string subject)
        {
            string sqlExpression = $@"
                SELECT Дата, Оценка FROM УчащиесяОценки
                WHERE ID_учащегося='{pupil_ID}' AND Предмет='{subject}'
            ";

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
            string sqlExpression = $@"
                SELECT Дата, МинутОпоздания FROM УчащиесяОпозданияОтсутствия 
                WHERE ID_учащегося='{pupil_ID}' AND Предмет='{subject}'
            ";

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
                sqlExpression = $@"
                    SELECT ID FROM Учащиеся WHERE
                    НомерГруппы='{group}' AND Фамилия='{secondName}' AND ДатаРождения='{birthday}'
                ";
            }
            else
            {
                sqlExpression = $@"
                    SELECT ID FROM Учащиеся WHERE
                        НомерГруппы='{group}' AND 
                        Фамилия='{secondName}' AND 
                        ДатаРождения='{birthday}' AND 
                        ИмяОтчество='{io}'
                ";
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

        public static int AddTeacher(string fio, string password)
        {
            string sqlExpression = $@"INSERT INTO Преподаватели VALUES('{fio}','{password}')";
            return ExecuteNonQuery(sqlExpression);
        }

        public static int AddPupil(string family, string birthday, string group, string name_patronymic)
        {
            string sqlExpression = $@"
                INSERT INTO Учащиеся VALUES(
                    '{family}','{birthday}','{group}','{name_patronymic}'
                )";
            return ExecuteNonQuery(sqlExpression);
        }

        public static int AddSubject(string subject)
        {
            string sqlExpression = $@"INSERT INTO Предметы VALUES('{subject}')";
            return ExecuteNonQuery(sqlExpression);
        }

        public static int AddGroup(string group)
        {
            string sqlExpression = $@"INSERT INTO Группы VALUES('{group}')";
            return ExecuteNonQuery(sqlExpression);
        }

        public static int DeleteTeacher(string fio)
        {
            string sqlExpression = $@"
                DELETE FROM Преподаватели WHERE
                    ФИО='{fio}'
                ";
            return ExecuteNonQuery(sqlExpression);
        }

        public static int DeleteSubject(string subject)
        {
            string sqlExpression = $@"DELETE FROM Предметы WHERE Название='{subject}'";
            return ExecuteNonQuery(sqlExpression);
        }

        public static int DeleteGroup(string group)
        {
            string sqlExpression = $@"DELETE FROM Группы WHERE Номер='{group}'";
            return ExecuteNonQuery(sqlExpression);
        }

        public static int DeletePupil(string family, string birthday, string group, string name_patronymic)
        {
            string sqlExpression = $@"
                DELETE FROM Учащиеся WHERE
                    Фамилия='{family}' AND 
                    ДатаРождения='{birthday}' AND 
                    НомерГруппы='{group}' AND 
                    ИмяОтчество='{name_patronymic}'
                ";
            return ExecuteNonQuery(sqlExpression);
        }

        public static int AddSubjectTeacherToGroup(string subject, string teacher, string group)
        {
            string sqlExpression = $@"INSERT INTO ГруппыПредметы VALUES('{group}','{subject}','{teacher}')";
            return ExecuteNonQuery(sqlExpression);
        }

        public static SqlDataReader ExecuteQuery(string sqlExpression)
        {
            SqlCommand command = new SqlCommand(sqlExpression, Connection);
            var reader = command.ExecuteReader();
            return reader;
        }
        public static int ExecuteNonQuery(string sqlExpression)
        {
            SqlCommand command = new SqlCommand(sqlExpression, Connection);
            return command.ExecuteNonQuery();
        }
    }
}
