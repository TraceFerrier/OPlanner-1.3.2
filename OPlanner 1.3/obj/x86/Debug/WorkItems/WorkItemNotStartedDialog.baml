   M S B A M L   `   `   `  ����  OPlanner 1.09    3PlannerNameSpace.WorkItems.WorkItemNotStartedDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$(  "Move Work Item back to Not Started��5   	    � SizeToContent$ WidthAndHeight=�64    � 
ResizeMode$ NoResize=�6S   .��5      � -   5         �Name$ DialogContext��5      ���	#DDE53D10 � RowDefinitions 5   
   �� 5      $��80 �6   �� 5      $��32 �6   �� 5	      $��32 �6   �� 5
      $��32 �6   �� 5      $��80 �6   �� 5      $��48 �6    � ColumnDefinitions 5   
   �� 5      $	��110 �6   �� 5      $��90 �6   �� 5      $��90 �6   �� 5      $��90 �6   .+�5   
   �� $��0��6   $��4��6"    �� Orientation$ 
Horizontal=�66   $��Stretch=�6O   .+�5      �� $��/Images/Warning.png��6   $��5q�61   $
��None=�6<   �� 5      $ Vertical=�6   $	��329��60   .+�5      �� $��1��6   $��1��6)   $��Title: ��69   $��14(�6I   $
��Wrap=�6W   $��4q�6k   $��	Underline{�6v   �� 5      $��1��6   $��1��6)   $��14(�6Q   $
��Wrap=�6_   $��4q�6s   $	��329��6~   $	��Top=�6�   ��69   �� *Title+6�   5      5      �� 5   
   $��0��6   $��1��6!   $��4��6.   $��2��6B   
���   �$��1q�6g   $��3q�6{   �� 5    
   $��1��6   $��0��6!   $��
Estimate: ��61   $��14(�6C   $��4q�6Q   �� 5!   
   $��1��6   $��1��6!   $��14(�6L   $
��Wrap=�6Z   $��4q�6n   $��60��6y   ��61   �� *
Estimate+6y   �� 5#   
   $��2��6   $��0��6   
�������$��2��6P   $��4q�6d   �� 5$   
   $��2��6   $��0��6!   $��Completed: ��61   $��14(�6D   $��4q�6R   �� 5%   
   $��2��6   $��1��6!   $��14(�6M   $
��Wrap=�6[   $��4q�6o   $��60��6z   ��61   �� *	Completed+6z   �� 5'   
   $��3��6   $��0��6!   $��Remaining: ��61   $��14(�6D   $��4q�6R   �� 5(   
   $��3��6   $��1��6!   $��14(�6Q   $
��Wrap=�6_   $��4q�6s   $��60��6~   ��61   �� *WorkRemaining+6~   �� 5*   
   $��4��6   $��4��6!   
�������$����This work item has completed work associated with it - moving it back to 'Not Started' will set the completed work back to zero.  Click 'Cancel' if you don't want to do this.��5+      $��6q�5,      $��14(�6   $
��Wrap=�5-      �� 5.   
   $ 
Horizontal=�6   $��5��6.   $��2��6;   $��2��6K   $��Right=�6_   .+�5/      �� -   5         ��Name$ OkButton��5/      $��OK��6'   $
��Left=�64   $��5q�6O   $��24��6Z   $��75��6f      RoundCorner#��[  6�   �� 50      -   5        $ CancelButton��50      $��Cancel��6+   $
��Left=�6<   $��5q�6W   $��24��6b   $��75��6n   #��[  6�   51      52      53      