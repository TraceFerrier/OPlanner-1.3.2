   M S B A M L   `   `   `  ����  OPlanner 1.0!    PlannerNameSpace.HelpWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  OPlanner Help��5   	   $	��800��6   $
��1200��6,    � 	Resources 5         MemberAssignmentRow��%!   &	        �� 5   
   5	      5
      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   � -   5         �Name$ ViewContext��5      $��1��6$    5      �� 5      $��* �6     � ColumnDefinitions 5      �� 5      $��* �6#   .+�5      �� -   5         ��Name$ HelpRichTextBox��5   '   $��1��6    �� AcceptsReturn .  �� 
IsReadOnly .  �� 
AcceptsTab . "	 �� VerticalScrollBarVisibility$
	 Auto=�6y   $
 �� HorizontalScrollBarVisibility$

 Auto=�6�    �� IsDocumentEnabled . $��Stretch=�5      $��Stretch=�66   5       5!      5"      