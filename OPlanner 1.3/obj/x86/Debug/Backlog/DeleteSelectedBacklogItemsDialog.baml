   M S B A M L   `   `   `  ����  OPlanner 1.07    1PlannerNameSpace.DeleteSelectedBacklogItemsDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$#  Delete Selected Backlog Items��5   	    � SizeToContent$ WidthAndHeight=�6/    � 
ResizeMode$ NoResize=�6N   .��5      � ���	#DDE53D10 � RowDefinitions 5   
   �� 5      $��10 �6   �� 5      $��48 �6   �� 5	      $��30 �6   �� 5
      $��24 �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��20 �6   �� 5      $��* �6   �� 5      $��20 �6   .+�5   
   �� $��1��6   $��1��6"    �� Orientation$ 
Horizontal=�62   $��Center=�6K   .+�5      �� $����Are you sure you want to delete all the selected backlog items? If so, all Work Items assigned to these backlog items will be deleted as well.��6   $
��Wrap=�6�   $	��860��6�   $��16(�6�   $��0,0,6,0q�6�   $��SemiBold"�6�   
�������6  5      �� 5   
   $��2��6   $��1��6"   $ 
Horizontal=�62   .+�5      �� $��Total to delete:��6   $��16(�60   $��0,0,6,0q�6>   $��SemiBold"�6O   �� 5      -   5         ��Name$ DeleteCountBlock��5      $��1��62   $��1��6?   $��0��6O   $��16(�6X   $��SemiBold"�6f   5      �� 5   
   $��3��6   $��1��6"   $ 
Horizontal=�62   .+�5       �� $3��-Total Work Remaining for these Backlog Items:��6   $��16(�6M   $��0,0,6,0q�6[   $��SemiBold"�6l   �� 5!      -   5        $ TotalWorkRemainingBlock��5!      $��1��69   $��1��6F   $��0��6V   $��16(�6_   $��SemiBold"�6m   5"      �� 5$   
   $��4��6   $��1��6"   $
��Left=�62   $	��Top=�6M   $ Vertical=�6e   $��0,6,0,0q�6|   .+�5%      �� $��Backog Items:��6   $��0,0,5,0q�6-   $��16(�6>   &   System.Windows.Controls.DataGrid  5&      -   5        $��BacklogItemGrid��5&      $��1��6.   $��4��6;      ItemGridStyle#��[  6O      SelectionMode$ Single=�6v      SelectionUnit$ FullRow=�6�   $	��360��6�   	   Columns	 5'      0  *System.Windows.Controls.DataGridTextColumn  5(      ,  &System.Windows.Controls.DataGridColumn
   Header$
 ID��6)      MinWidth$ 50Q�65      Width5  /System.Windows.Controls.DataGridLengthConverter$ 55 6C      CanUserSort .    SortMemberPath$ ID��6�     DataGridTextColumnStyle1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 6�    " DataGridTextColumnEditingStyle   EditingElementStyle# [ 6�      Binding 6N   �� *ID+ �� Mode$ OneWay=�6�     5)      $
 Title��6)   $	 120Q�68   $	 300 6G    . $ Title��6�   # [ 6�   # [ 6�    6S   �� *Title+$ OneWay=�6�     5*      $
 Train��6)   $	 100Q�68   $	 160 6G      
IsReadOnly .  . $ 	TrainName��6�   # [ 6�   # [ 6�    6S   �� *	TrainName+$ OneWay=�6�     5+      $
 Pillar��6)   $	 100Q�69   $	 190 6H    . # [ 6�   # [ 6�    6T   �� *
PillarName+$ OneWay=�6�     5,      $
 Work Completed��6)   $ 60Q�6A   $ 80 6O   # [ 6�   # [ 6�   $ 	Completed��6+   6Z   �� *TotalWorkCompleted+$ OneWay=� �� UpdateSourceTrigger$ 	LostFocus=�6+    5-      $
 Work Remaining��6)   $ 60Q�6A   $ 80 6O   # [ 6�   # [ 6�   $ 	Completed��6   6Z   �� *TotalWorkRemaining+$ OneWay=�6  5/      50      �� 52   
   $��1��6   $��6��6%   $ 
Horizontal=�62   $��Right=�6K   $��5q�6g   .+�53      �� -   5         ��Name$ OkButton��53      $��OK��6'   $��24��64   $��50��6@   $��5q�6K     RoundCorner#��[ 6V   �� 54      -   5        $ CancelButton��54      $��Cancel��6+   $��24��6<   $��50��6H   $��5q�6S   #��[ 6^   55      57      58      