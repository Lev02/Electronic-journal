# Electronic-journal

Technologies used: C#, WPF, SQL
Programs used: MS Visual Studio 2019, MS SQL Server 2019

In order to work with electronic journal locally you have to
connect database "EJournalDB.mdf" to your microsoft sql server.
You can find "EJournalDB.mdf" and "EJournalDB_log.ldf" in the root directory.

Go to MS SQL Management Studio, databases -> connect; than locate and connect "EjournalDB.mdf" file.
After that go to file App.config, which is located in project's root directory, and
change the name of the server from "SQLEXPRESS" to yours. The name of the server can be found
in MS SQL Management Studio.

You can access electronic journal three different ways - as admin, as a pupil, or as a teacher.

WHAT DOES ADMIN DO?
- Adds and deletes pupils, teachers, groups, subjects. 
- Connects a group with teacher and subject.

WHAT DOES TEACHER DO?
- Gives grades to pupils.
- Marks lateness.

WHAT DOES PUPIL DO?
- Views his grades and lateness.
