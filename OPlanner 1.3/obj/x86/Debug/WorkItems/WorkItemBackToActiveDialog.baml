   M S B A M L   `   `   `  ����  OPlanner 1.0;    5PlannerNameSpace.WorkItems.WorkItemBackToActiveDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$#  Move Work Item back to Active��5   	    � SizeToContent$ WidthAndHeight=�6/    � 
ResizeMode$ NoResize=�6N   .��5      � -   5         �Name$ DialogContext��5       � RowDefinitions 5   
   �� 5      $��96 �6   �� 5      $��* �6   �� 5	      $��* �6   �� 5
      $��* �6   �� 5      $��* �6   �� 5      $��48 �6    � ColumnDefinitions 5   
   �� 5      $	��110 �6   �� 5      $��90 �6   �� 5      $��90 �6   �� 5      $��90 �6   .+�5   
   �� $��0��6   $��4��6"    �� Orientation$ 
Horizontal=�66   $��Stretch=�6O   .+�5      �� $��/Images/Warning.png��6   $��5q�61   $
��None=�6<   �� 5      $��5��6   $��4��6%   
��� ���$����To change from Completed back to 'To-Do' or 'In Progress', please provide new values for Estimate and/or Completed, to indicate how much work is actually left for this work item.��5      $��6q�5      $��14(�6   $
��Wrap=�5      $	��329��5      5      �� 5   
   $��1��6   $��0��6"   $��4��62    !  StandardDialogStackPanelStyle#��[  6F   .+�5       �� $��Title��6    % !StandardDialogTextBlockLabelStyle#��[ 6%   $��SemiBold"�6`   �� 5!       % !StandardDialogTextBoxContentStyle#��[ 6Y   $��64��5"      " �� VerticalScrollBarVisibility$
 Auto=�6&    �� TextWrapping$
 Wrap=�6I   $��3q�6]   ��5!      �� *Title+	 �� Mode$	 TwoWay=�
 �� UpdateSourceTrigger$
 	LostFocus=�5"   ]   5#      �� 5%   
   $��0��6   $��2��6!   
�������$��4��6A   $��2��6U   
���   �$��1q�6z   $��1q�6�   �� 5'   
   $��2��6   $��2��6!   $��
Estimate: ��61   $��14(�6C   $��4q�6Q   �� 5(   
   -   5         ��Name$ EstimateBox��5(      $��2��6)   $��3��66   $��14(�6n   $
��Wrap=�6|   $��4q�6�   $��60��6�   ��6F   �� *
Estimate+$	 TwoWay=�6�   �� 5*   
   $��3��6   $��2��6!   $��Completed: ��61   $��14(�6D   $��4q�6R   �� 5+   
   -   5         ��Name$ CompletedBox��5+      $��3��6(   $��3��65   $��14(�6n   $
 Wrap=�6|   $��4q�6�   $��60��6�   ��6E   �� *	Completed+$	 TwoWay=�6�   �� 5-   
   $��4��6   $��2��6!   $��Remaining: ��61   $��14(�6D   $��4q�6R   �� 5.   
   -   5        $ RemainingBox��5.      $��4��6(   $��3��65   $��14(�6s   $
 Wrap=�6�   $��4q�6�   $��60��6�   
���������6E   �� *WorkRemaining+$	 OneTime=�6�   �� 50   
   $ 
Horizontal=�6   $��5��6.   $��2��6;   $��2��6K   $��Right=�6_   .+�51      �� -   5         ��Name$ OkButton��51      $��OK��6'   $
��Left=�64   $��5q�6O   $��24��6Z   $��75��6f     RoundCorner#��[ 6�   �� 52      -   5        $ CancelButton��52      $��Cancel��6+   $
��Left=�6<   $��5q�6W   $��24��6b   $��75��6n   #��[ 6�   53      54      55      