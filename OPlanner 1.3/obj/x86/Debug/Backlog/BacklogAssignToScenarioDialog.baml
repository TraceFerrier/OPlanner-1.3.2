   M S B A M L   `   `   `  ����G  COPlanner 1.0, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:PlannerNameSpacePlannerNameSpace  5   	   4    .PlannerNameSpace.BacklogAssignToScenarioDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ,cclr-namespace:PlannerNameSpace     5   	     � Title$(  "Assign Backlog Items to a Scenario��5   	   $	��800��64   $
��1100��6A   .��5      �  � ColumnDefinitions 5   
   �� 5      $
��947* �6   �� 5	      $
��145* �6   ��5   
   �� 5       �� ImageSource$  /Images/MoveBackground.jpg��6    j� Stretch$ UniformToFill=�6B    �� Opacity$	 0.1Q�6Z    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��50 �6   .+�5   
   � $��0��6   $��6q�6   $��2��6'    5      �� 5      $��24 �6    �� 5      $	��300 �6    �� 5      $
��auto �6    �� 5      $��24 �6    �� 5      $��32 �6    �� 5      $��* �6    .+�5      �� $��0��6   $��Backlog Items To Assign��6%   $
��Left=�6D   $��10,0,0,0q�6_   $��14(�6q   $
��Bold"�6   &   System.Windows.Controls.DataGrid  5       -   5        $��BacklogGrid��5       $��1��6*      ItemGridStyle#��[  67      SelectionMode$ Single=�6^      SelectionUnit$ FullRow=�6u      CanUserSortColumns . 	   Columns	 5!      0  *System.Windows.Controls.DataGridTextColumn  5"      ,  &System.Windows.Controls.DataGridColumn
   Header$
 ID��6)      MinWidth$ 50Q�65      Width5  /System.Windows.Controls.DataGridLengthConverter$ 55 6C      SortMemberPath$ ID��6r     DataGridTextColumnStyle1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 6�    " DataGridTextColumnEditingStyle   EditingElementStyle# [ 6�      Binding 6N   �� *ID+ �� Mode$ OneWay=�6�     5#      $
 Title��6)   $	 120Q�68   $	 170 6G   $ Title��6�   # [ 6�   # [ 6�    6S   �� *Title+$ TwoWay=� �� UpdateSourceTrigger$ 	LostFocus=�6�   4  .System.Windows.Controls.DataGridTemplateColumn  5$      $	 140Q�6-   $	 150 6<   $
 Pillar��6H      CanUserSort . $ 
PillarName��6k      CellTemplate 5%      �� 5&      .R�5'   "   ��   DataGridComboStyle#��[ 6z    �� DisplayMemberPath$ Title��6�    �� SelectedValuePath$ StoreKey��6�   ��6+   �� *PillarsWithNone+  PillarManager �� Source# [  �� SelectedItem 6�   ��  �� Path$ ParentPillarItem�$ TwoWay=�$ PropertyChanged=�5(      5*        5,      -   5        TrainColumn��$	 140Q�5,   B   $	 160 6Q   $
 Train��6]    . $ 	TrainName��6    5-      �� 5.      .R�5/   "   �� #��[ 6x   $ Title��6�   $ StoreKey��6�   ��6+   �� *TrainsWithNone+  TrainManager# [  6�   �� $ ParentTrainItem�$ TwoWay=�$ PropertyChanged=�50      52        54      $	 140Q�6-   $	 160 6<   $
 Scenario��6H    . $ ParentScenarioName��6m    55      �� 56      .R�57   "   ��  ��Name$ ScenarioComboBox��6+   #��[ 6u   $ QualifiedTitle��6�   ��6E   �� $ AvailableScenarios� 6�   �� $ ParentScenarioItem�$ TwoWay=�$ PropertyChanged=�58      5:        5<      $	 100Q�6-   $	 180 6<   $
 
Scrum Team��6H    .  $ ScrumTeamName��6p    5=      �� 5>      .R�5?   "   �� #��[ 6X   $ Title��6�   ��6+   �� $ ValidScrumTeams� 6�   �� $ FeatureTeamItem�$ TwoWay=�$ PropertyChanged=�5@      5B        5D      %   PlannerNameSpace.PlannerContent   ColumnHeaderBusinessRank#
 Z 6)   $ 60Q�6g   $ 60 6u   $ BusinessRank��6�   # [ 6�   # [ 6#   6�   �� *BusinessRank+$ TwoWay=�$ 	LostFocus=�6#    5E      $
 Story Points��6)   $ 60Q�6?   $ 60 6M   $ StoryPoints��6�   # [ 6�   # [ 6�    6X   �� *StoryPoints+$ TwoWay=�$ 	LostFocus=�6�     5F      $
 Work Scheduled��6)   $ 80Q�6A   $ 80 6O   $ TotalWorkScheduled��6�    # DataGridReadOnlyTextColumnStyle# [ 6�    6Z   �� *TotalWorkScheduled+$ OneWay=�6�     5G      $
 Work Completed��6)   $ 80Q�6A   $ 80 6O   $ TotalWorkCompleted��6�   # [ 6�    6Z   �� *TotalWorkCompleted+$ OneWay=�6�     5H      $
 Work Remaining��6)   $ 80Q�6A   $ 80 6O   $ TotalWorkRemaining��6�   # [ 6�    6Z   �� *TotalWorkRemaining+$ OneWay=�6�     5I      $ 60Q�6-   $ 90 6;   $
 Completion Status��6F    .  $ ItemStatusText��6u    5J      �� 5K      .R�5L   "   �� $
��Wrap=�6�   ��6,   �� $ ItemStatusText�$ OneWay=���6^   �� *StatusColor+��6�   �� *StatusTextColor+5M      5O        5P      $ 60Q�6-   $ 80 6;   $
 Resolution Status��6F    .  $ Status��6u    5Q      �� 5R      .R�5S   "   �� $
��Wrap=�6�   ��6,   �� $ Status�$ OneWay=���6V   �� *ResolutionStatusColor+6�   5T      5V      5X      �� 5Z      $��2��6   $��Stretch=�6(   $��Center=�6F   
���   � �� ShowsPreview . $��3��6�   �� 5[      $��3��6   $9��3Select a Scenario to assign these Backlog Items to:��6%   $
��Left=�6`   $��10,0,0,0q�6{   $��14(�6�   $
��Bold"�6�   �� 5]      $��4��6    �� Orientation$ 
Horizontal=�6&   .+�5_      �� $��Pillar:��6     FilterComboLabelStyle#��[ 6*   $��10,0,0,0q�6Y   �� 5`      -   5        $ PillarCombo��5`        FilterComboStyle#��[ 60   $ Title��6Z   $	��225��6t   �� 5b      $��Quarter:��6   #��[ 6+   $��15,0,0,0q�6Z   �� 5c      -   5        $ QuarterCombo��5c      #��[ 61   $ Title��6[   $	��225��6u   5e        5g      -   5        $��ScenarioGrid��5g      $��5��6+   $��0��68   #��[  6H   $ Single=�6o   $ FullRow=�6�    . 	 5h        5i      $
 ID��6)   $ 50Q�65   $ 60 6C   # [ 6r   # [ 6�    6N   �� *ID+$ OneWay=�6�     5j      $
 Title��6)   $	 120Q�68   $	 240 6G   # [ 6�   # [ 6�    6S   �� *Title+$ TwoWay=�$ 	LostFocus=�6�     5k      $
 Pillar��6)   $	 100Q�69   $	 240 6H   # [ 6z    6T   �� *ParentPillarTitle+6z     5l      $
 Parent Experience��6)   $	 100Q�6D   $	 240 6S   # [ 6�    6_   �� *ParentExperienceTitle+6�     5m      #
 Z 6)   $ 60Q�6g   $ 60 6u   # [ 6�   # [ 6  $ BusinessRank��6K   6�   �� *BusinessRank+$ TwoWay=�$ 	LostFocus=�6K    5n      $
 Spec Status��6)   $ 80Q�6>   $	 100 6L   # [ 6�    6X   �� *SpecStatusText+$ OneWay=�6�     5o      $
 Design Status��6)   $ 80Q�6@   $	 100 6N   # [ 6�    6Z   �� *DesignStatusText+$ OneWay=�6�     5p      $
 Backlog Item Count��6)   $ 80Q�6E   $ 80 6S   # [ 6�    6^   �� *BacklogCount+$ OneWay=�6�   5r      5s      �� 5t   
   $��1��6   $��0��6"   $��2��62   $ 
Horizontal=�6F    !	 StandardDialogStackPanelStyle#��[	 6_   $��Right=�6�   $��0,8,5,8q�6�   .+�5u      �� -   5         ��Name$ OkButton��5u      $��OK��6'   $��5q�64   $��60��6?    
 RoundCorner#��[
 6a   �� 5v      -   5        $ CancelButton��5v      $��Cancel��6+   $��5q�6<   $��60��6G   #��[
 6m   5w      5x      5y      