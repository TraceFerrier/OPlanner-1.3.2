   M S B A M L   `   `   `  ����  OPlanner 1.04    .PlannerNameSpace.MoveWorkItemToNextTrainDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$%  Move WorkItem to the Next Train��5   	    � SizeToContent$ WidthAndHeight=�61    � 
ResizeMode$ NoResize=�6P   .��5      � ���	#FFE95C36 � RowDefinitions 5   
   �� 5      $��* �6   �� 5	      $��50 �6   .+�5   
   � -   5         �Name$ DialogContext��5       5      �� 5      $��80 �6    �� 5      $��90 �6    �� 5      $��60 �6    �� 5      $��60 �6    �� 5      $��60 �6     � ColumnDefinitions 5      �� 5      $��88 �6#   �� 5      $	��460 �6#   .+�5      �� $��0��6   $��0��6!   $��/Images/OPlannerLogo.jpg��61    J� Opacity$	 0.7Q�6S   �� 5       �� Orientation$ Vertical=�6   $��0��60   $��1��6=   $��1��6M   .+�5      �� $����To move this work item to the next train, a new backlog item needs to be created to represent the remaining work in the next train.  This work item will then be moved to that backlog item.��6      WelcomeTextBlockStyle#��[  6�   $��0,10,0,0q�6  5      �� 5       $��1��6   $��0��6&   $��2��66    ! StandardDialogStackPanelStyle#��[ 6J   .+�5!      �� $��Work Item Title:��6   #��[  64   $
��Left=�6c   $��0q�6~   �� 5"       ' #StandardDialogTextBlockContentStyle#��[ 63   $��48��5#      $
��Wrap=�6&   $��3q�6:   
�������$��CharacterEllipsis=�6X   ��5"      �� *Title+5#   X   5$      �� 5&      $ Vertical=�6   $��2��60   $��0��6=   $��2��6M   #��[ 6a   .+�5'      �� $3��-The new Backlog Item will be given this name:��6   #��[  6Q   $
��Left=�6�   $��0q�6�   �� 5(      -   5         ��Name$ NewBacklogItemNameBox��5(       % !StandardDialogTextBoxContentStyle#��[ 69   $��16(�6t   $��3q�6�   5)      �� 5+      $ Vertical=�6   $��3��60   $��0��6=   $��2��6M   #��[ 6a   .+�5,      �� $-��'And will be assigned to the next train:��6   #��[  6K   $
��Left=�6z   �� 5-      -   5        	 ��Name$	 NextTrainNameBox��5-      $��
Next Train��66   #��[  6H   $��16(�6w   $
��Left=�6�   
���   �$��3q�6�   5.      �� 50      $ Vertical=�6   $��5��60   $��0��6=   $��2��6M   $��Bottom=�6a   .+�51      �� $,��&Click OK if you would like to proceed:��6   #��[  6J   $��Center=�6y   52      55      �� 57   
   $��1��6   
���   �$��1q�62   �� 58   
   $ 
Horizontal=�6   $��1��6.   $��Center=�6;   .+�59      �� -   5        
 ��Name$
 OKButton��59      $��OK��6'   $��24��64   $��70��6@   $��10q�6K     RoundCorner#��[ 6W   �� 5:      -   5        $
 CancelButton��5:      $��Cancel��6+   $��24��6<   $��70��6H   $��10q�6S   #��[ 6_   5;      5<      5=      