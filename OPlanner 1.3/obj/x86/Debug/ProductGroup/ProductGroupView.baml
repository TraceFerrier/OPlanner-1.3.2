   M S B A M L   `   `   `  ����G  COPlanner 1.0, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:PlannerNameSpacePlannerNameSpace  5      -    'PlannerNameSpace.Views.ProductGroupView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,cclr-namespace:PlannerNameSpace     5      .��5	      � -   5          �Name$  Context��5	       � RowDefinitions 5
   
   �� 5      $��32 �6   �� 5      $	��300 �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $	��130 �6#   �� 5      $	��240 �6#   �� 5      $	��240 �6#   �� 5      $��* �6#   .+�5      �� $��Pillars��6   $��0��6'   $��0��67      OverviewHeadingStyle#��[  6D   %   PlannerNameSpace.PlannerContent   PillarOverviewToolTip ^� ToolTip# Z 6r   �� 5      -   5         ��Name$ UpdateControl��5      $�� ��6/   $��3��67   $��0��6G     ItemUpdateTextStyle#��[ 6T   5      &   System.Windows.Controls.DataGrid  5    
   -   5        $��
PillarGrid��5       $��1��6%     ItemGridStyle#��[ 62      CanUserSortColumns .    Columns 5!      0  *System.Windows.Controls.DataGridTextColumn  5"      ,  &System.Windows.Controls.DataGridColumn   Header$ Pillar Name��6%   	   MinWidth$		 120Q�6:   
   Width5  /System.Windows.Controls.DataGridLengthConverter$	
 240 6I      SortDirection$ 	AscendingX�6�     DataGridTextColumnStyle1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 6�    " DataGridTextColumnEditingStyle   EditingElementStyle# [ 6�      Binding 6U   �� *Title+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ 	LostFocus=�6�     5#      $ Pillar Path��6%   $		 120Q�6:   $	
 360 6I      
IsReadOnly . # [ 6�   # [ 6�      SortMemberPath$ BusinessRank��6   6U   �� *
PillarPath+6  4  .System.Windows.Controls.DataGridTemplateColumn  5%      $		 100Q�6)   $	
 180 68   $ PM Owner��6D      CanUserSort . $ PMOwner��6i      CellTemplate 5&      �� 5'      .R�5(      ��   DataGridComboStyle#��[ 6W    �� DisplayMemberPath$ DisplayName��6�    �� SelectedValuePath$ Alias��6�   ��6'   ��  �� Path$ AvailablePMMembers� �� SelectedValue 6�   �� $ PMOwner�$ TwoWay=�$ PropertyChanged=�5)      5+        5-      $		 100Q�6)   $	
 180 68   $ 	Dev Owner��6D    . $ DevOwner��6j    5.      �� 5/      .R�50      �� #��[ 6X   $ DisplayName��6�   $ Alias��6�   ��6'   �� $ AvailableDevMembers� 6�   �� $ DevOwner�$ TwoWay=�$ PropertyChanged=�51      53        55      $		 100Q�6)   $	
 180 68   $ 
Test Owner��6D    . $ 	TestOwner��6k    56      �� 57      .R�58      �� #��[ 6Y   $ DisplayName��6�   $ Alias��6�   ��6'   �� $ AvailableTestMembers� 6�   �� $ 	TestOwner�$ TwoWay=�$ PropertyChanged=�59      5;      5>      �� 5@   
   $��2��6   $��Stretch=�6$   $��Center=�6B   
���   � �� ShowsPreview . $��5��6�   � 5B   
   $��3��6   .+�5C      �  5D      �� 5E      $��64 �6$   �� 5F      $��* �6$   .+�5I      � $��0��6    5J      �� 5K      $��80 �6+   �� 5L      $��* �6+   .+�5N      �� $��People��6    $��0��6.   #��[  6>      PeopleOverviewToolTip# Z 6l   �� 5P       �� Orientation$ 
Horizontal=�6!   $��1��6:   .+�5Q      �� $��Dev Manager: ��6$     ItemGridTitleStyle#��[ 69   $��Normal"�6e   �� 5R      #��[ 6K   $��Normal"�6w   $��5q�6�   ��6$   �� *DevManagerDisplayName+6�   �� 5S      $��50��6J   $��50��6U   ��6    �� $ DevManagerPicture�6U   �� 5U      $��Test Manager: ��6$   #��[ 6:   $��Normal"�6f   �� 5V      #��[ 6L   $��Normal"�6x   $��5q�6�   ��6$   �� *TestManagerDisplayName+6�   �� 5W      $��50��6K   $��50��6V   ��6    �� $ TestManagerPicture�6V   �� 5Y      $��GPM: ��6$   #��[ 61   $��Normal"�6]   �� 5Z      #��[ 6H   $��Normal"�6t   $��5q�6�   ��6$   �� *GroupPMDisplayName+6�   �� 5[      $��50��6G   $��50��6R   ��6    �� $ GroupPMPicture�6R   �� 5]      $��Primary Pillar:��6      FilterComboLabelStyle#��[ 6:   $��15,0,0,0q�6i   �� 5^      -   5         ��Name$ MemberPillarCombo��5^   #     FilterComboStyle#��[ 6�   $ Title��6�   $
�� 240��6�   ��6>   �� *PillarsWithAllNone+ 	 PillarManager �� Source# [	 6�   �� 5`      $��Discipline:��6    #��[ 66   $��15,0,0,0q�6e   �� 5a      -   5        $ DisciplineCombo��5a   #   #��[ 6<   $
�� 100��6f   5b      5c        5e      -   5        $��
PeopleGrid��5e      $��1��6-   #��[ 6:    5f        5g      $
 50 61   $  ��6<    .  $ Title��6Z    5h      �� 5i   "   .R�5j   &   �� $��2q�6P   ��6,   �� $ UserPicture�6P   5k   #   5m        5n      $	
 200 61   $ Display Name��6=    . $ DisplayName��6f   $ 	AscendingX�6�    5o      �� 5p   "   .R�5q   &   ��  
 ItemGridTextBlockStyle#��[
 6M   ��60   �� *DisplayName+6M   5r   #   5t        5u      $	
 100 61   $ Alias��6=    . $ Title��6_    5v      �� 5w   "   .R�5x   &   �� #��[
 6G   ��60   �� *Title+6G   5y   #   5{        5|      $	
 100 61   $ 
Discipline��6=    . $ 
Discipline��6d    5}      �� 5~   "   .R�5   &   �� #��[
 6L   ��60   �� *
Discipline+6L   5�   #   5�        5�      $
 80 61   $ Off Time��6<    .   5�      �� 5�   "   .R�5�   &   �� #��[
 6N   ��60   �� *TotalOffDays+6N   5�   #   5�        5�      $	 80Q�61   $
 80 6?   $ Avg Work Capacity��6J    . $ CapacityPerDay��6x    5�      �� 5�   "   .R�5�   &   �� $ AvgCapacityCombo��6/   #��[ 6�   ��6I   �� *AllowedCapacityValues+ 6w   �� *CapacityPerDay+$ TwoWay=�$ PropertyChanged=�6�   5�   #   5�        5�          ColumnHeaderPrimaryPillar# Z 62   $		 140Q�6q   $	
 200 6�    . $ 
PillarName��6�    5�      �� 5�   "   .R�5�   &   �� #��[ 6~   $ Title��6�   ��6/   �� *PillarsWithNone+# [	  �� SelectedItem 6�   �� $ ParentPillarItem�$ TwoWay=�$ PropertyChanged=�5�   #   5�        5�      $	
 240 61   $ 	Job Title��6=    . $ JobTitle��6c    5�      �� 5�   "   .R�5�   &   �� #��[
 6J   ��60   �� *
JobTitle+6J   5�   #   5�        5�      $	
 200 61   $ 	Telephone��6=    . $ 	Telephone��6c    5�      �� 5�   "   .R�5�   &   �� #��[
 6K   ��60   �� *	Telephone+6K   5�   #   5�        5�      $	
 150 61   $ Office��6=    . $ 
OfficeName��6`    5�      �� 5�   "   .R�5�   &   �� #��[
 6L   ��60   �� *
OfficeName+6L   5�   #   5�      5�      5�      5�      5�      5�      