   M S B A M L   `   `   `  ����  OPlanner 1.0$    PlannerNameSpace.PersonaEditor   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Persona Editor��5   	   $	��600��6    $
��1100��6-   .��5      � ��5   
   �� 5       �� ImageSource$# /Images/PersonaBackground.jpg��6    j� Stretch$ UniformToFill=�6E    �� Opacity$
 0.12Q�6]    � RowDefinitions 5
   
   �� 5      $��* �6   �� 5      $��55 �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5        $��PersonaEditorGrid��5         ItemGridStyle#��[  6,      SelectionMode$ Extended=�6S      SelectionUnit$ FullRow=�6l      CanUserSortColumns . $��10q�6�    J� Opacity$ .7Q�6�   	   Columns	 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn
   Header$
 Persona Title��6%      MinWidth$	 120Q�6<      Width5  /System.Windows.Controls.DataGridLengthConverter$	 240 6K     DataGridTextColumnStyle1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 6�    " DataGridTextColumnEditingStyle   EditingElementStyle# [ 6�      Binding 6W   �� *Title+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ 	LostFocus=�6�     5      $
 Description��6%   $	 120Q�6:   $	 450 6I   # [ 6�   # [ 6�    6U   �� *PersonaDescription+$ TwoWay=�$ 	LostFocus=�6�   5      �� 5   
   $��1��6   $��0��6"   $��1��62    �� Orientation$ 
Horizontal=�6F    ! StandardDialogStackPanelStyle#��[ 6_   $
��Left=�6�   $��Center=�6�   .+�5      �� -   5         ��Name$ DeleteButton��5      $��Delete��6+     RoundCorner#��[ 6<   �� 5      -   5        $ 	NewButton��5      $	��New��6(   #��[ 66   5      �� 5   
   $��1��6   $��0��6"   $��1��62   $ 
Horizontal=�6F   #��[ 6_   $��Right=�6�   $��Center=�6�   .+�5      �� -   5        $ OkButton��5      $��OK��6'   #��[ 64   5      5      5      