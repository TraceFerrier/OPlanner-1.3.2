   M S B A M L   `   `   `  ����  OPlanner 1.01    +PlannerNameSpace.AddItemToCommitmentsDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$!  Approve Backlog Commitments��5   	    � SizeToContent$ WidthAndHeight=�6-    � 
ResizeMode$ NoResize=�6L   .��5      � ���	#FFE95C36 � RowDefinitions 5   
   �� 5      $��* �6   �� 5	      $��50 �6   .+�5   
   � -   5         �Name$ DialogContext��5       5      �� 5      $��60 �6    �� 5      $��80 �6    �� 5      $��40 �6    �� 5      $��40 �6    �� 5      $��20 �6     � ColumnDefinitions 5      �� 5      $��88 �6#   �� 5      $	��460 �6#   .+�5      ��  �� Orientation$ Vertical=�6   $��0��60   $��0��6=   $��2��6M   $��Center=�6a   .+�5      �� $k��eAre you sure you want to add this item to the approved commitments for the assigned pillar and train?��6      WelcomeTextBlockStyle#��[  6�   $��Center=�6�   5      �� 5      $ 
Horizontal=�6   $��1��62   $��1��6?   $��1��6O   $	��Top=�6c   .+�5       �� $��Title: ��6   #��[  6+   $
��Left=�6Z   �� 5!      #��[  63   $��Center=�6b   $
��Wrap=�6   $	��400��6�   ��6   �� *Title+6�   5"      �� 5$      $ 
Horizontal=�6   $��2��62   $��1��6?   $��1��6O   .+�5%      �� $��Pillar: ��6   #��[  6,   $
��Left=�6[   �� 5&      #��[  68   $
��Left=�6g   $
��Bold"�6�   ��6   �� *
PillarName+6�   5'      �� 5)      $ 
Horizontal=�6   $��3��62   $��1��6?   $��1��6O   .+�5*      �� $��Train: ��6   #��[  6+   $
��Left=�6Z   �� 5+      #��[  67   $
��Left=�6f   $
��Bold"�6�   ��6   �� *	TrainName+6�   5,      5.      �� 50   
   $��1��6   
���   �$��1q�62   
������� J� Opacity$	 0.2Q�6Y   �� 51   
   $ 
Horizontal=�6   $��1��6.   $��Center=�6;   .+�52      �� -   5         ��Name$ OKButton��52      $��OK��6'   $��24��64   $��70��6@   $��10q�6K     RoundCorner#��[ 6W   �� 53      -   5        $ CancelButton��53      $��Cancel��6+   $��24��6<   $��70��6H   $��10q�6S   #��[ 6_   54      55      56      