   M S B A M L   `   `   `  ����  OPlanner 1.0-    'PlannerNameSpace.SelectResolutionDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Select Bug Resolution��5   	    � SizeToContent$ WidthAndHeight=�6'    � 
ResizeMode$ NoResize=�6F   .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��50 �6   .+�5   
   � $��0��6    5      �� 5      $��50 �6    �� 5      $	��100 �6     � ColumnDefinitions 5      �� 5      $	��110 �6#   �� 5      $	��100 �6#   �� 5      $	��100 �6#   �� 5      $	��100 �6#   .+�5      �� $��0��6   $��0��6&   $��2��66    !  StandardDialogStackPanelStyle#��[  6J    �� Orientation$ Vertical=�6�   .+�5      �� $%��Select Resolution for this bug:��6    % !StandardDialogTextBlockLabelStyle#��[ 6C   $��SemiBold"�6~   �� 5      -   5         ��Name$ ResolutionComboBox��5        StandardDialogComboStyle#��[ 67   $��22��5      5      �� 5      $��1��6   $��0��6&   $��4��66   $��1��6J   #��[  6[   $ Vertical=�6�   $	��Top=�6�   $��5,5,5,0q�6�   .+�5      �� $��
Comments: ��6   #��[ 6.   $��SemiBold"�6i   �� 5      -   5         ��Name$ CommentsBox��5       % !StandardDialogTextBoxContentStyle#��[ 6/   $��70��5       �� TextWrapping$
 Wrap=�6&   $	 �� HorizontalScrollBarVisibility$
	 Auto=�6:   "
 �� VerticalScrollBarVisibility$

 Auto=�6_    �� AcceptsReturn . 5!      5"      5#      �� 5$   
   $��1��6   
���   �$��1q�62   �� 5%   
   $ 
Horizontal=�6   $��1��6.   $��Center=�6;   .+�5&      �� -   5         ��Name$ OkButton��5&      $��Ok��6'   $��24��64   $��70��6@   $��10q�6K     RoundCorner#��[ 6W   �� 5'      -   5        $ CancelButton��5'      $��Cancel��6+   $��24��6<   $��70��6H   $��10q�6S   #��[ 6_   5(      5)      5*      