   M S B A M L   `   `   `  ����G  COPlanner 1.0, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:PlannerNameSpacePlannerNameSpace  5   	   6    0PlannerNameSpace.ScenarioMoveToNextQuarterDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ,cclr-namespace:PlannerNameSpace     5   	     � Title$'  !Move Scenario to the Next Quarter��5   	   $	��800��63   $
��1100��6@   .��5      � ���	#FFE95C36 � ColumnDefinitions 5   
   �� 5      $
��947* �6   �� 5	      $
��145* �6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��50 �6   .+�5   
   � $��0��6   $��6q�6   $��2��6'    5      �� 5      $��70 �6    �� 5      $��70 �6    �� 5      $��70 �6    �� 5      $��30 �6    �� 5      $	��210 �6    �� 5      $��40 �6    �� 5      $	��210 �6    �� 5      $��* �6    .+�5      ��  �� Orientation$ Vertical=�6   $��0��60   $��0��6=   .+�5      �� $��Scenario to be moved:��6      WelcomeTextBlockStyle#��[  69   $
��Left=�6h   $��5q�6�   �� 5       -   5         ��Name$ ScenarioNameBox��5        % !StandardDialogTextBoxContentStyle#��[ 63   $��16(�6n   $��5,0,10,0q�6}    �� 
IsReadOnly . 6�   5!      �� 5#      $ Vertical=�6   $��1��60   $��0��6=   .+�5$      �� $?��9Name to be given to the new Scenario in the next Quarter:��6   #��[  6]   $
��Left=�6�   $��5q�6�   �� 5%      -   5        $ ScenarioNewNameBox��5%      #��[ 66   $��16(�6q   $��5,0,10,0q�6�    .  6�   5&      �� 5(      $ Vertical=�6   $��2��60   $��0��6=   .+�5)      �� $D��>Train within the next quarter that all items will be moved to:��6   #��[  6b   $
��Left=�6�   $��5q�6�   �� 5*      -   5        $ NewTrainNameBox��5*      #��[ 63   $��16(�6n   $��5,0,10,0q�6}    . 6�   5+      �� 5-      $��3��6   $>��8Backlog Items owned by this Scenario that will be moved:��6%   #��[  6e   $
��Left=�6�   $��5q�6�   $��Bottom=�6�   &   System.Windows.Controls.DataGrid  5.      -   5        $��BacklogGrid��5.      $��4��6*     ItemGridStyle#��[ 67      SelectionMode$ Single=�6^      SelectionUnit$ FullRow=�6u      CanUserSortColumns . 	   Columns	 5/      0  *System.Windows.Controls.DataGridTextColumn  50      ,  &System.Windows.Controls.DataGridColumn
   Header$
 ID��6)      MinWidth$ 50Q�65      Width5  /System.Windows.Controls.DataGridLengthConverter$ 55 6C      SortMemberPath$ ID��6r     DataGridTextColumnStyle1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 6�    " DataGridTextColumnEditingStyle   EditingElementStyle# [ 6�      Binding 6N   �� *ID+ �� Mode$ OneWay=�6�     51      $
 Title��6)   $	 120Q�68   $	 170 6G   $ Title��6�   # [ 6�   # [ 6�    6S   �� *Title+$ TwoWay=� �� UpdateSourceTrigger$ 	LostFocus=�6�   4  .System.Windows.Controls.DataGridTemplateColumn  52      $	 140Q�6-   $	 150 6<   $
 Pillar��6H      CanUserSort . $ 
PillarName��6k      CellTemplate 53      �� 54      .R�55   "   ��   DataGridComboStyle#��[ 6z    �� DisplayMemberPath$ Title��6�    �� SelectedValuePath$ StoreKey��6�   ��6+   �� *PillarsWithNone+  PillarManager �� Source# [  �� SelectedItem 6�   ��  �� Path$ ParentPillarItem�$ TwoWay=�$ PropertyChanged=�56      58        5:      -   5        TrainColumn��$	 140Q�5:   B   $	 160 6Q   $
 Train��6]    . $ 	TrainName��6    5;      �� 5<      .R�5=   "   �� #��[ 6x   $ Title��6�   $ StoreKey��6�   ��6+   �� *TrainsWithNone+  TrainManager# [  6�   �� $ ParentTrainItem�$ TwoWay=�$ PropertyChanged=�5>      5@        5B      $	 100Q�6-   $	 180 6<   $
 Feature Team��6H    .  $ ScrumTeamName��6r    5C      �� 5D      .R�5E   "   �� #��[ 6X   $ Title��6�   ��6+   �� $ ValidScrumTeams� 6�   �� $ FeatureTeamItem�$ TwoWay=�$ PropertyChanged=�5F      5H        5J      %   PlannerNameSpace.PlannerContent   ColumnHeaderBusinessRank#
 Z 6)   $ 60Q�6g   $ 60 6u   $ BusinessRank��6�   # [ 6�   # [ 6#   6�   �� *BusinessRank+$ TwoWay=�$ 	LostFocus=�6#    5K      $
 Story Points��6)   $ 60Q�6?   $ 60 6M   $ StoryPoints��6�   # [ 6�   # [ 6�    6X   �� *StoryPoints+$ TwoWay=�$ 	LostFocus=�6�     5L      $
 Work Scheduled��6)   $ 80Q�6A   $ 80 6O   $ TotalWorkScheduled��6�    # DataGridReadOnlyTextColumnStyle# [ 6�    6Z   �� *TotalWorkScheduled+$ OneWay=�6�     5M      $
 Work Completed��6)   $ 80Q�6A   $ 80 6O   $ TotalWorkCompleted��6�   # [ 6�    6Z   �� *TotalWorkCompleted+$ OneWay=�6�     5N      $
 Work Remaining��6)   $ 80Q�6A   $ 80 6O   $ TotalWorkRemaining��6�   # [ 6�    6Z   �� *TotalWorkRemaining+$ OneWay=�6�     5O      $ 60Q�6-   $ 90 6;   $
 Completion Status��6F    .  $ ItemStatusText��6u    5P      �� 5Q      .R�5R   "   �� $
��Wrap=�6�   ��6,   �� $ ItemStatusText�$ OneWay=���6^   �� *StatusColor+��6�   �� *StatusTextColor+5S      5U        5V      $ 60Q�6-   $ 80 6;   $
 Resolution Status��6F    .  $ Status��6u    5W      �� 5X      .R�5Y   "   �� $
��Wrap=�6�   ��6,   �� $ Status�$ OneWay=���6V   �� *ResolutionStatusColor+6�   5Z      5\      5^      �� 5`      $��5��6   $4��.Work Items owned by the selected Backlog Item:��6%   #��[  6[   $
��Left=�6�   $��5q�6�   $��Bottom=�6�     5a      -   5        $��WorkItemsGrid��5a      $��6��6,   #��[ 69   $ Single=�6`   $ FullRow=�6w   	 5b        5c      $
 ID��6)   $ 50Q�65   $ 60 6C   # [ 6r   # [ 6�    6N   �� *ID+$ OneWay=�6�     5d      $
 Title��6)   $	 120Q�68   $	 260 6G   # [ 6�   # [ 6�    6S   �� *Title+$ TwoWay=�$ 	LostFocus=�6�     5e      $
 Assigned To��6)   $ 80Q�6>   $ 80 6L   # [ 6�   # [ 6�    6W   �� *
AssignedTo+$ OneWay=�6�     5f      $
 Estimate (Low)��6)   $ 60Q�6A   $ 80 6O   # [ 6�   # [ 6�   $ EstimateLow��6$   6Z   �� *EstimateLow+$ TwoWay=�$ 	LostFocus=�6$    5g      $
 Estimate (High)��6)   $ 60Q�6B   $ 80 6P   # [ 6�   # [ 6�   $ EstimateHigh��6&   6[   �� *EstimateHigh+$ TwoWay=�$ 	LostFocus=�6&    5h      $
 Estimate��6)   $ 60Q�6;   $ 80 6I      
IsReadOnly . # [ 6�   $ Estimate��6�    6T   �� *
Estimate+$ OneWay=�6�     5i      $
 	Completed��6)   $ 60Q�6<   $ 80 6J   # [ 6�   # [ 6�   $ 	Completed��6   6U   �� *	Completed+$ TwoWay=�$ 	LostFocus=�6    5j      $
 Work Remaining��6)   $ 60Q�6A   $ 80 6O    . # [ 6�   $ WorkRemaining��6�    6Z   �� *WorkRemaining+$ OneWay=�6�   5l      5o      �� 5q   
   $��1��6   $��0��6"   $��2��62   $ 
Horizontal=�6F    !	 StandardDialogStackPanelStyle#��[	 6_   $��Right=�6�   $��0,8,5,8q�6�   .+�5r      �� -   5         ��Name$ OkButton��5r      $��OK��6'   $��5q�64   $��60��6?    
 RoundCorner#��[
 6a   �� 5s      -   5        $ CancelButton��5s      $��Cancel��6+   $��5q�6<   $��60��6G   #��[
 6m   5t      5v      5w      