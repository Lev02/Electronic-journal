   M S B A M L   `   `   `  ����C  ?EJournal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:EJournalEJournal  5   	       EJournal.PupilLoginWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:EJournal     5   	     � Title$(  "Вход для учащегося��5   	   $	��450��5	   	   $	��450��6   $	��450��5
   	   $	��450��6   $	��450��5   	   $	��450��6    � WindowStartupLocation$ CenterScreen=�5   	    � Icon$ Resources/JournalIcon.png��5   	   .��5      � .+�5   
   �� $��Resources/PupilIcon.jpg��6   $��151,10,151.2,300q�61   �� 5   
   $
��Left=�6   $��
31,195,0,0q�6/   $
��Wrap=�6C   $��Дата рождения��6W   $	��Top=�6l   $��31��6�   $	��144��6�    �� TextAlignment$ Center=�6�   $��20(�6�   
������� J� RenderTransformOrigin$ 0.512,0.815&�6�   �� 5   
   -   5        $��
Войти��5      $
��Left=�6!   $��151,312,0,0q�6<   $	��Top=�6Q   $	��158��6i   $��51��6u   $
��Bold"�6�   $ 0.495,1.067&�6�    � 	Resources 5       �� 
TargetType �� Property �� Value��%_   '��       �� 5      " ��6   .�5      ��  �@��CornerRadius$ 15��65   5      5      5      �� 5   
   -   5        $��BirthdayTextBox��5      $
��Left=�6)   $��30��6D   $��198,196,0,0q�6P   	 �� TextWrapping$
	 Wrap=�6e   $�� ��6y   $	��Top=�6�   $	��158��6�   
 �� 	MaxLength$
 10��6�    �� 
CaretIndex$ 0��6�   $��Center=�6�   $
��Bold"�6�   �� 5   
   $
��Left=�6   $��
31,248,0,0q�6/   $
��Wrap=�6C   $��Группа��6W   $	��Top=�6e   $��31��6}   $	��144��6�   $ Center=�6�   $��20(�6�   
�������$ 0.512,0.815&�6�   �� 5   
   -   5        $��Вернуться��5      $
��Left=�6%   $��
10,378,0,0q�6@   $	��Top=�6T   $��92��6l   $��32��6w   $��DemiBold"�6�    5      ��%_   '��       �� 5      " ��6   .�5       ��  �@��CornerRadius$ 15��65   5!      5"      5#      �� 5$   
   -   5        $��	GroupList��5$      $
��Left=�6$   $��198,248,0,0q�6?   $	��Top=�6T   $	��158��6l   $��32��6x    �� 
IsEditable .  5&      �� 5'   
   $
��Left=�6   $��
31,140,0,0q�6/   $
��Wrap=�6C   $��ФИО��6W   $	��Top=�6b   $��31��6z   $	��144��6�   $ Center=�6�   $��20(�6�   
�������$ 0.512,0.815&�6�   �� 5(   
   -   5         ��Name$ ReferenceButton��5(      $��Справка��6*   $��RIght=�6<   $��0,10,287.2,0q�6X   $	��Top=�6n   $	��141��6�   6�   �� 5)   
   -   5         ��Name$ FioList��5)      $
��Left=�6$   $��198,140,0,0q�6?   $	��Top=�6T   $	��158��6l   $��32��6x    .  6�   5*      5+      