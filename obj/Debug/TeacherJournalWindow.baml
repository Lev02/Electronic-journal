   M S B A M L   `   `   `  ����C  ?EJournal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:EJournalEJournal  5   	   #    EJournal.TeacherJournalWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:EJournal     5   	     � Title$   ��5   	   $	��450��5	   	   $	��800��6    � WindowStartupLocation$ CenterScreen=�5
   	    � Icon$ Resources/JournalIcon.png��5   	   .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5      &   System.Windows.Controls.DataGrid  -   5        $��JournalMarks��5      $	��750��6,      
IsReadOnly .  $��Center=�5      $	��Top=�5      "   VerticalScrollBarVisibility$
 Auto=�5         FrozenColumnCount$ 1��5      ��5      �� 5        5   
   -   5        $��JournalLateness��5      $	��750��6+    .  $��Center=�5       $	��Top=�5!      $
 Auto=�5"      $ 1��5$      ��5#      �� 5'      �� 5)   
   -   5         ��Name$ updateMarksButton��5)      $%��Сохранить оценки��6,   $��Center=�6e   $��Bottom=�6�   $	��151��6�   �� 5*   
   -   5        $ updateLatenessButton��5*      $+��%Сохранить опоздания��6/   $��Center=�6n   $��Bottom=�6�   $	��151��6�   $��321,0,321.6,24q�6�   �� 5,   
   -   5        $ 
backButton��5,      $��Вернуться��6%   $��0,0,0,0q�69   $
��Left=�6c   $��Bottom=�6~   $	��151��6�   �� 5-   
   -   5        $ switchButton��5-      $,��&Перейти к опозданиям��6'   $��0,0,0,0q�6F   $��Right=�6r   $��Bottom=�6�   $	��151��6�   50      51      