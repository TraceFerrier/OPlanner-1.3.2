   M S B A M L   `   `   `  ����  OPlanner 1.0.    (PlannerNameSpace.DeleteBacklogItemDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Delete Backlog Item��5   	    � SizeToContent$ WidthAndHeight=�6%    � 
ResizeMode$ NoResize=�6D   .��5      � ���	#DDE53D10 � RowDefinitions 5   
   �� 5      $��10 �6   �� 5      $��30 �6   �� 5	      $��30 �6   �� 5
      $��24 �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��20 �6   �� 5      $��* �6   �� 5      $��20 �6   .+�5   
   �� $��1��6   $��1��6"    �� Orientation$ 
Horizontal=�62   .+�5      �� $��Backlog Item:��6   $��16(�6-   $��0,0,6,0q�6;   $��SemiBold"�6L   �� 5      -   5         ��Name$ BacklogTitleBlock��5      $	��foo��63   $��16(�6>   $��SemiBold"�6L   5      �� 5   
   $��2��6   $��1��6"   $ 
Horizontal=�62   .+�5      �� $��Pillar:��6   $��16(�6'   $��0,0,6,0q�65   $��SemiBold"�6F   �� 5      -   5        $ BacklogPillarBlock��5      $��1��64   $��1��6A   $	��bar��6Q   $��16(�6\   $��SemiBold"�6j   5      �� 5    
   $��2��6   $��1��6!   $��Stretch=�61   $��3��6O   $��Bottom=�6Z   �� 5!   
   $��3��6   $��1��6!   
��� ���$����This Backlog item is the parent of the work items shown below.  If you delete the Backlog Item, all of these work items will be deleted as well.��6E   $��14(�6�   $
��Wrap=�6�   �� 5"   
   $��4��6   $��1��6!   $��Stretch=�61   $��1��6O   $��Bottom=�6Z   �� 5#   
   $��5��6   $��1��6"   $
��Left=�62   $	��Top=�6M   $ Vertical=�6e   .+�5$      �� $��
WorkItems:��6   $��0,0,5,0q�6*   $��14(�6;   &   System.Windows.Controls.DataGrid  5%      -   5        $��WorkItemGrid��5%      $��1��6+   $��4��68      ItemGridStyle#��[  6L      SelectionMode$ Single=�6s      SelectionUnit$ FullRow=�6�   $	��360��6�   	   Columns	 5&      0  *System.Windows.Controls.DataGridTextColumn  5'      ,  &System.Windows.Controls.DataGridColumn
   Header$
 ID��6)      MinWidth$ 50Q�65      Width5  /System.Windows.Controls.DataGridLengthConverter$ 55 6C      CanUserSort .    SortMemberPath$ ID��6�     DataGridTextColumnStyle1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 6�    " DataGridTextColumnEditingStyle   EditingElementStyle# [ 6�      Binding 6N   �� *ID+ �� Mode$ OneWay=�6�     5(      $
 Title��6)   $	 120Q�68   $	 300 6G    . $ Title��6�   # [ 6�   # [ 6�    6S   �� *Title+$ OneWay=�6�     5)      $
 Train��6)   $	 100Q�68   $	 160 6G      
IsReadOnly .  . $ 	TrainName��6�   # [ 6�   # [ 6�    6S   �� *	TrainName+$ OneWay=�6�     5*      $
 
Scrum Team��6)   $	 100Q�6=   $	 190 6L    .  . $ ScrumTeamName��6�   # [ 6�   # [ 6   6X   �� *ScrumTeamName+$ OneWay=�6    5+      $
 	Completed��6)   $ 60Q�6<   $ 80 6J   # [ 6�   # [ 6�   $ 	Completed��6   6U   �� *	Completed+$ TwoWay=� �� UpdateSourceTrigger$ 	LostFocus=�6    5,      $
 Work Remaining��6)   $ 60Q�6A   $ 80 6O   # [ 6�   # [ 6�   $ 	Completed��6   6Z   �� *WorkRemaining+$ OneWay=�6  5.      5/      �� 51   
   $��1��6   $��6��6%   $ 
Horizontal=�62   $��Right=�6K   $��5q�6g   .+�52      �� -   5         ��Name$ OkButton��52      $��OK��6'   $��24��64   $��50��6@   $��5q�6K     RoundCorner#��[ 6m   �� 53      -   5        $ CancelButton��53      $��Cancel��6+   $��24��6<   $��50��6H   $��5q�6S   #��[ 6y   54      56      57      