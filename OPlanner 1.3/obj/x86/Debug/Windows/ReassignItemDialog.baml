   M S B A M L   `   `   `  ����  OPlanner 1.0)    #PlannerNameSpace.ReassignItemDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  New Backlog Item��5   	    � SizeToContent$ WidthAndHeight=�6"    � 
ResizeMode$ NoResize=�6A   .��5      �  � RowDefinitions 5   
   �� 5      $��10 �6   �� 5      $��32 �6   �� 5	      $��32 �6   �� 5
      $��* �6    � ColumnDefinitions 5   
   �� 5      $��20 �6   �� 5      $��* �6   �� 5      $��20 �6   .+�5   
   �� $��1��6   $��1��6!   $?��9Select a team member to assign all selected WorkItems to:��61   $��14(�6r   �� 5   
   $��2��6   $��1��6"   $
��Left=�62   $	��Top=�6M    �� Orientation$ 
Horizontal=�6e   .+�5      �� $��Team Member:��6   $��0,0,5,0q�6,   $��14(�6=   �� 5      -   5         ��Name$ TeamMemberCombo��5      $
��Left=�60   $	��Top=�6K   $	��270��6c   5      �� 5   
   $��1��6   $��3��6%   $ 
Horizontal=�62   $��Right=�6K   $��5q�6g   .+�5      �� -   5         ��Name$ OkButton��5      $��OK��6'   $��24��64   $��50��6@   $��5q�6K   6V   �� 5      -   5        $ CancelButton��5      $��Cancel��6+   $��24��6<   $��50��6H   $��5q�6S   6^   5      5      5      