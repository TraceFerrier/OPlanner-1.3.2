   M S B A M L   `   `   `  ����G  COPlanner 1.0, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:PlannerNameSpacePlannerNameSpace  5   	   *    $PlannerNameSpace.MoveWorkItemsDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ,cclr-namespace:PlannerNameSpace     5   	     � Title$  Move Work Items��5   	   $	��800��6!   $
��1100��6.   .��5      �  � ColumnDefinitions 5   
   �� 5      $
��947* �6   �� 5	      $
��145* �6   ��5   
   �� 5       �� ImageSource$  /Images/MoveBackground.jpg��6    j� Stretch$ UniformToFill=�6B    �� Opacity$	 0.1Q�6Z    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��50 �6   .+�5   
   � $��0��6   $��6q�6   $��2��6'    5      �� 5      $��24 �6    �� 5      $	��300 �6    �� 5      $
��auto �6    �� 5      $��24 �6    �� 5      $��32 �6    �� 5      $��* �6    .+�5      �� $��0��6   $��Work Items to move��6%   $
��Left=�6?   $��10,0,0,0q�6Z   $��14(�6l   $
��Bold"�6z   &   System.Windows.Controls.DataGrid  5       -   5        $��WorkItemsGrid��5       $��1��6,      ItemGridStyle#��[  69      SelectionMode$ Single=�6`      SelectionUnit$ FullRow=�6w      Columns 5!      0  *System.Windows.Controls.DataGridTextColumn  5"      ,  &System.Windows.Controls.DataGridColumn	   Header$	 ID��6)   
   MinWidth$
 50Q�65      Width5  /System.Windows.Controls.DataGridLengthConverter$ 60 6C     DataGridTextColumnStyle1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 6r    " DataGridTextColumnEditingStyle   EditingElementStyle# [ 6�      Binding 6N   �� *ID+ �� Mode$ OneWay=�6�     5#      $	 Title��6)   $	
 120Q�68   $	 260 6G   # [ 6�   # [ 6�    6S   �� *Title+$ TwoWay=� �� UpdateSourceTrigger$ 	LostFocus=�6�     5$      $	 Assigned To��6)   $
 80Q�6>   $ 80 6L   # [ 6�   # [ 6�    6W   �� *
AssignedTo+$ OneWay=�6�     5%      $	 Estimate (Low)��6)   $
 60Q�6A   $ 80 6O   # [ 6�   # [ 6�      SortMemberPath$ EstimateLow��6$   6Z   �� *EstimateLow+$ TwoWay=�$ 	LostFocus=�6$    5&      $	 Estimate (High)��6)   $
 60Q�6B   $ 80 6P   # [ 6�   # [ 6�   $ EstimateHigh��6&   6[   �� *EstimateHigh+$ TwoWay=�$ 	LostFocus=�6&    5'      $	 Estimate��6)   $
 60Q�6;   $ 80 6I      
IsReadOnly .  # DataGridReadOnlyTextColumnStyle# [ 6�   $ Estimate��6�    6T   �� *
Estimate+$ OneWay=�6�     5(      $	 	Completed��6)   $
 60Q�6<   $ 80 6J   # [ 6�   # [ 6�   $ 	Completed��6   6U   �� *	Completed+$ TwoWay=�$ 	LostFocus=�6    5)      $	 Work Remaining��6)   $
 60Q�6A   $ 80 6O    . # [ 6�   $ WorkRemaining��6�    6Z   �� *WorkRemaining+$ OneWay=�6�   5+      �� 5-      $��2��6   $��Stretch=�6(   $��Center=�6F   
���   � �� ShowsPreview . $��3��6�   �� 5.      $��3��6   $8��2Select a Backlog Item to move these work items to:��6%   $
��Left=�6_   $��10,0,0,0q�6z   $��14(�6�   $
��Bold"�6�   �� 50      $��4��6    �� Orientation$ 
Horizontal=�6&   .+�52      �� $��Pillar:��6     FilterComboLabelStyle#��[ 6*   $��10,0,0,0q�6Y   �� 53      -   5         ��Name$ BacklogPillarCombo��53        FilterComboStyle#��[ 6�    �� DisplayMemberPath$ Title��6�   $	��225��6�   ��67   �� *PillarsWithAllNone+  PillarManager �� Source# [ 6�   �� 55      $��Train:��6   #��[ 6)   $��15,0,0,0q�6X   �� 56      -   5        $ BacklogTrainCombo��56      #��[ 6�   $ Title��6�   $	��225��6�   ��66   �� *TrainsWithAllNone+  TrainManager# [ 6�   �� 58      $��Scrum Team:��6   #��[ 6.   $��15,0,0,0q�6]   �� 59      -   5        $ ScrumTeamCombo��59      #��[ 63   $ Title��6]   $	��225��6w   5:        5<      -   5        $��BacklogGrid��5<      $��5��6*   $��0��67   #��[  6G   $ Single=�6n   $ FullRow=�6�      CanUserSortColumns .   5=        5>      $	 ID��6)   $
 50Q�65   $ 60 6C   # [ 6r   # [ 6�    6N   �� *ID+$ OneWay=�6�     5?      $	 Title��6)   $	
 120Q�68   $	 240 6G   # [ 6�   # [ 6�    6S   �� *Title+$ TwoWay=�$ 	LostFocus=�6�     5@      $	 Description��6)   $	
 100Q�6>   $	 240 6M   # [ 6�    6Y   �� *ShortDescription+$ OneWay=�6�     5A      %   PlannerNameSpace.PlannerContent   ColumnHeaderBusinessRank#	 Z 6)   $
 60Q�6g   $ 60 6u   # [ 6�   # [ 6  $ BusinessRank��6K   6�   �� *BusinessRank+$ TwoWay=�$ 	LostFocus=�6K    5B      $	 Work Scheduled��6)   $
 80Q�6A   $ 80 6O   # [ 6�    6Z   �� *TotalWorkScheduled+$ OneWay=�6�     5C      $	 Work Completed��6)   $
 80Q�6A   $ 80 6O   # [ 6�    6Z   �� *TotalWorkCompleted+$ OneWay=�6�     5D      $	 Work Remaining��6)   $
 80Q�6A   $ 80 6O   # [ 6�    6Z   �� *TotalWorkRemaining+$ OneWay=�6�   5F      5G      �� 5H   
   $��1��6   $��0��6"   $��2��62   $ 
Horizontal=�6F    ! StandardDialogStackPanelStyle#��[ 6_   $��Right=�6�   $��0,8,5,8q�6�   .+�5I      �� -   5         ��Name$ OkButton��5I      $��OK��6'   $��5q�64   $��60��6?    	 RoundCorner#��[	 6a   �� 5J      -   5        $ CancelButton��5J      $��Cancel��6+   $��5q�6<   $��60��6G   #��[	 6m   5K      5L      5M      