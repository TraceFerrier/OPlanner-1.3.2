   M S B A M L   `   `   `  ����G  COPlanner 1.0, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:PlannerNameSpacePlannerNameSpace  5   	   '    !PlannerNameSpace.ChangeListWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ,cclr-namespace:PlannerNameSpace     5   	     � 
ResizeMode$  CanResizeWithGrip=�5   	    � Title$ Change List��5   	   $	��600��6   $	��900��6+   $	��900��67    � 	Resources 5         ChangeItemRow 9� ExpandDirection 9� 
IsExpanded&   System.Windows.Controls.DataGrid  ItemGridStyle   SelectionMode   CanUserSortColumns   SelectionUnit   HeadersVisibility	   Columns0  *System.Windows.Controls.DataGridTextColumn,  &System.Windows.Controls.DataGridColumn
   Header   MinWidth   Width5  /System.Windows.Controls.DataGridLengthConverter  DataGridTextColumnStyle1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle   Binding �� Mode��%�  &	        7  7  7  7  7  7  7  7  7  7  7  7  7  �� 5	   
   .R�5
      � .+�5      9� $
 Down=�6    . $��Stretch=�6D   $��Stretch=�6b   ��5        5      $��ChangedStoreItemGrid��6#   8��  $ Single=�6�    .  $
 Cell=�6�   $ Column=�6�   ��6?   �� *
SelfList+	 5        5   "   $
 Title��65   $ 50Q�6D   $	 240 6R   8   6^   �� *Title+$ OneWay=�6�     5   "   $
 	Item Type��65   $ 50Q�6H   $	 140 6V   8   6b   �� *StoreItemTypeName+$ OneWay=�6�     5   "   $
 ID��65   $ 50Q�6A   $	 145 6O   8   6[   �� *
StoreKey+$ OneWay=�6�     5   "   $
 Change Type��65   $ 50Q�6J   $	 145 6X   8   6d   �� *StoreChangeTypeName+$ OneWay=�6�     5   "   $
 Opened Date��65   $ 50Q�6J   $	 150 6X   8   6d   �� *
OpenedDate+$ OneWay=�6�   5      .��5      � $��	35,0,25,0q�6   $��Stretch=�6.   $��Stretch=�6L   .+�5        $��ChangedPropertiesGrid��6#   8�� $ Single=�6�    .  $
 Cell=�6�   $ Column=�6�   ��6@   �� *ChangedProperties+	 5        5   "   $
 Property Name��65   $ 50Q�6L   $	 125 6Z   8   6f   �� *PublicPropName+$ OneWay=�6�     5   "   $
 Internal Property Name��65   $ 50Q�6U   $	 125 6c   8   6o   �� *
DSPropName+$ OneWay=�6�     5   "   $
 Property Type��65   $ 50Q�6L   $	 130 6Z   8 	  6f   �� *PropertyType+$ OneWay=�6�     5    "   $
 Backing Value��65   $ 50Q�6L   $	 140 6Z   8 
  6f   �� *BackingValue+$ OneWay=�6�     5!   "   $
 Current Value��65   $ 50Q�6L   $	 140 6Z   8   6f   �� *CurrentValue+$ OneWay=�6�     5"   "   $
 Previous Value��65   $ 50Q�6M   $	 140 6[   8   6g   �� *PreviousValue+$ OneWay=�6�   5$      5%      5&      5'      5(      5)      .��5+      �  � RowDefinitions 5,   
   �� 5-      $��60 �6   �� 5.      $��* �6   .+�51   
   � $��0��6   $
��Left=�6   .+�52      �� -   5         ��Name$ 
TitlePanel��52       �� Orientation$ 
Horizontal=�6-   
��� � �$��3q�6Y    J� Opacity$	 0.9Q�6d   $	��900��6r   .+�53      �� $��/Images/OPlannerLogo.jpg��6   $	 1.0Q�6:   $��2q�6H   �� 54      $ Vertical=�6   .+�55      �� $��ChangeList Viewer��6    
�������$��20(�6L   $��Segoe UI)�6Z   $��5,0,0,0q�6p   �� 56      -   5         ��Name$ ChangeCountBox��56       $��Changes to save: 0��68   
�������$��14(�6e   $��Segoe UI)�6s   $��5,5,0,0q�6�   $��Bottom=�6�   57      58      59      �� 5;   
   $��1��6   $
��Auto=�6$   $
��Auto=�6G   $��Stretch=�6l   $
��Left=�6�   .��5=      � $	��875��6   .+�5>      � -   5         �Name$ ChangeListGrid��5>      .+�5?      �� $ Vertical=�6!   .+�5@      �� -   5         ��Name$ ChangeListItemsControl��5@   '   #��[  6G   5A      5B      5D      5E      5F      5G      