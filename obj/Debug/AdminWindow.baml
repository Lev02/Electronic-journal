   M S B A M L   `   `   `  ����C  ?EJournal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:EJournalEJournal  5   	       EJournal.AdminWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:EJournal     5   	     � Title$/  )Панель администратора��5   	   $	��480��6'   $	��710��64    � WindowStartupLocation$ CenterScreen=�5	   	    � Icon$ Resources/JournalIcon.png��5
   	   .��5      � .+�5   
   �� -   5        $)��#Добавить учащегося��5      $
��Left=�6.   $��	10,10,0,0q�6I   $	��Top=�6\   $	��150��6t   6�   �� 5   
   -   5        $'��!Удалить учащегося��5      $
��Left=�6-   $��
180,10,0,0q�6H   $	��Top=�6\   $	��150��6t   6�   �� 5   
   -   5        $1��+Добавить преподавателя��5      $
��Left=�62   $��
370,10,0,0q�6M   $	��Top=�6a   $	��150��6y   6�   �� 5   
   -   5        $/��)Удалить преподавателя��5      $
��Left=�61   $��
540,10,0,0q�6L   $	��Top=�6`   $	��150��6x   6�   �� 5   
   $
��Left=�6   $��	74,51,0,0q�6/   $
��Wrap=�6B   $��ФИО��6V   $	��Top=�6a   �� 5   
   -   5         ��Name$ pupilFamilyAdd��5      $
��Left=�6*   $��23��6E   $��	10,70,0,0q�6Q    �� TextWrapping$
 Wrap=�6d   $�� ��6x   $	��Top=�6�   $	��150��6�   �� 5   
   $
��Left=�6   $��
46,104,0,0q�6/   $
��Wrap=�6C   $��Дата рождения��6W   $	��Top=�6l   �� 5   
   -   5        $ PupilBirthdayAdd��5      $
��Left=�6,   $��23��6G   $��
10,125,0,0q�6S   $
 Wrap=�6g   $�� ��6{   $	��Top=�6�   $	��150��6�    �� 	MaxLength$ 10��6�   �� 5   
   $
��Left=�6   $��
62,160,0,0q�6/   $
��Wrap=�6C   $��Группа��6W   $	��Top=�6e   �� 5   
   -   5         ��Name$ pupilGroupAdd��5      $
��Left=�6*   $��
10,180,0,0q�6E   $	��Top=�6Y   $	��150��6q    �� 
IsEditable .  6}   �� 5   
   $
��Left=�6   $��
244,49,0,0q�6/   $
��Wrap=�6C   $��ФИО��6W   $	��Top=�6b   �� 5   
   -   5        $ pupilFamilyDelete��5      $
��Left=�6-   $��23��6H   $��
180,70,0,0q�6T   $
 Wrap=�6h   $�� ��6|   $	��Top=�6�   $	��150��6�   �� 5   
   $
��Left=�6   $��218,104,0,0q�6/   $
��Wrap=�6D   $��Дата рождения��6X   $	��Top=�6m   �� 5    
   -	   5        $ pupilBirthdayDelete��5       $
��Left=�6/   $��23��6J   $��180,125,0,0q�6V   $
 Wrap=�6k   $�� ��6   $	��Top=�6�   $	��150��6�   �� 5"   
   $
��Left=�6   $��232,160,0,0q�6/   $
��Wrap=�6D   $��Группа��6X   $	��Top=�6f   �� 5#   
   -
   5        $ pupilGroupDelete��5#      $
��Left=�6-   $��180,180,0,0q�6H   $	��Top=�6]   $	��150��6u    .  6�   �� 5&   
   $
��Left=�6   $��
422,50,0,0q�6/   $
��Wrap=�6C   $��ФИО��6W   $	��Top=�6b   �� 5'   
   -   5        $ teacherFioAdd��5'      $
��Left=�6)   $��23��6D   $��
370,70,0,0q�6P   $
 Wrap=�6d   $�� ��6x   $	��Top=�6�   $	��150��6�   �� 5(   
   $
��Left=�6   $��408,104,0,0q�6/   $
��Wrap=�6D   $��Пароль��6X   $	��Top=�6f   �� 5)   
   -   5        $ teacherPasswordAdd��5)      $
��Left=�6.   $��23��6I   $��370,125,0,0q�6U   $
 Wrap=�6j   $�� ��6~   $	��Top=�6�   $	��150��6�   �� 5,   
   $
��Left=�6   $��
590,51,0,0q�6/   $
��Wrap=�6C   $��ФИО��6W   $	��Top=�6b   �� 5-   
   -   5        $ teacherListToDelete��5-      $
��Left=�60   $��
538,71,0,0q�6K   $	��Top=�6_   $	��150��6w    .  6�   �� 50   
   $
��Left=�6   $��28��6/   $��	0,207,0,0q�6;   $	��Top=�6N   $	��698��6f   �� 53   
   -   5        $5��/Добавить предмет в группу��53      $
��Left=�65   $��
10,240,0,0q�6P   $	��Top=�6d   $	��160��6|   6�   �� 55   
   $
��Left=�6   $��
62,280,0,0q�6/   $
��Wrap=�6C   $��Группа��6W   $	��Top=�6e   �� 56   
   $
��Left=�6   $��
46,334,0,0q�6/   $
��Wrap=�6C   $ ��Преподаватель��6W   $	��Top=�6l   �� 57   
   -   5        $ teacherListToAddSubject��57      $
��Left=�64   $��
10,355,0,0q�6O   $	��Top=�6c   $	��150��6{    .  6�   �� 59   
   -   5        $ groupsListToAddSubject��59      $
��Left=�63   $��
10,301,0,0q�6N   $	��Top=�6b   $	��150��6z    .  6�   �� 5:   
   $
��Left=�6   $��
62,390,0,0q�6/   $
��Wrap=�6C   $��Предмет��6W   $	��Top=�6f   �� 5;   
   -   5        $ subjectListToAddSubject��5;      $
��Left=�64   $��
10,411,0,0q�6O   $	��Top=�6c   $	��150��6{    .  6�   �� 5>   
   -   5        $%��Добавить предмет��5>      $
��Left=�6,   $��180,240,0,0q�6G   $	��Top=�6\   $	��160��6t   6�   �� 5?   
   $
��Left=�6   $��232,280,0,0q�6/   $
��Wrap=�6D   $��Предмет��6X   $	��Top=�6g   �� 5@   
   -   5        $ 
subjectAdd��5@      $
��Left=�6&   $��23��6A   $��180,301,0,0q�6M   $
 Wrap=�6b   $�� ��6v   $	��Top=�6~   $	��150��6�   �� 5A   
   $
��Left=�6   $��28��6/   $��	0,207,0,0q�6;   $	��Top=�6N   $	��698��6f   �� 5B   
   -   5        $%��Добавить предмет��5B      $
��Left=�6,   $��180,240,0,0q�6G   $	��Top=�6\   $	��160��6t   6�   �� 5C   
   $
��Left=�6   $��232,280,0,0q�6/   $
��Wrap=�6D   $��Предмет��6X   $	��Top=�6g   �� 5F   
   -   5        $#��Удалить предмет��5F      $
��Left=�6+   $��180,349,0,0q�6F   $	��Top=�6[   $	��160��6s   6   �� 5G   
   $
��Left=�6   $��232,389,0,0q�6/   $
��Wrap=�6D   $��Предмет��6X   $	��Top=�6g   �� 5I   
   -   5        $#��Удалить предмет��5I      $
��Left=�6+   $��180,349,0,0q�6F   $	��Top=�6[   $	��160��6s   6   �� 5J   
   $
��Left=�6   $��232,389,0,0q�6/   $
��Wrap=�6D   $��Предмет��6X   $	��Top=�6g   �� 5M   
   -   5        $#��Добавить группу��5M      $
��Left=�6+   $��370,240,0,0q�6F   $	��Top=�6[   $	��160��6s   6   �� 5N   
   $
��Left=�6   $��422,280,0,0q�6/   $
��Wrap=�6D   $��Группа��6X   $	��Top=�6f   �� 5O   
   -   5        $ groupAdd��5O      $
��Left=�6$   $��23��6?   $��370,301,0,0q�6K   $
 Wrap=�6`   $�� ��6t   $	��Top=�6|   $	��150��6�   �� 5Q   
   -   5        $!��Удалить группу��5Q      $
��Left=�6*   $��370,349,0,0q�6E   $	��Top=�6Z   $	��160��6r   6~   �� 5R   
   $
��Left=�6   $��422,389,0,0q�6/   $
��Wrap=�6D   $��Группа��6X   $	��Top=�6f   �� 5S   
   -   5        $ subjectListToDelete��5S      $
��Left=�60   $��180,411,0,0q�6K   $	��Top=�6`   $	��150��6x    .  6�   �� 5T   
   -   5        $ groupListToDelte��5T      $
��Left=�6-   $��370,411,0,0q�6H   $	��Top=�6]   $	��150��6u    .  6�   �� 5V   
   -   5        $,��&Обновить базу данных��5V      $
��Left=�60   $��540,240,0,0q�6K   $	��Top=�6`   $	��141��6x   $	��128��6�   �� 5W   
   -   5         ��Name$ 
backButton��5W      $��Вернуться��6%   $��543,0,0,17.4q�69   $
��Left=�6h   $��Bottom=�6�   $	��151��6�   $��44��6�   	 J� RenderTransformOrigin$	 
0.494,0.38&�6�   5Y      5Z      