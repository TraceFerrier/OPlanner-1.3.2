   M S B A M L   `   `   `  ����  OPlanner 1.04    .PlannerNameSpace.ApproveTrainCommitmentsDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$!  Approve Backlog Commitments��5   	    � SizeToContent$ WidthAndHeight=�6-    � 
ResizeMode$ NoResize=�6L   .��5      � ���	#FFE95C36 � RowDefinitions 5   
   �� 5      $��* �6   �� 5	      $��50 �6   .+�5   
   �  5      �� 5      $��* �6    �� 5      $��40 �6    �� 5      $��40 �6    �� 5      $��40 �6    �� 5      $��90 �6     � ColumnDefinitions 5      �� 5      $��88 �6#   �� 5      $	��460 �6#   .+�5      9�  9� ExpandDirection$
 Down=�6    9� 
IsExpanded .  $��Stretch=�6A   $��Stretch=�6_   $��0��6{   $��1��6�   $��1��6�   $��	0,10,0,10q�6�   ��5      �� 5      $&�� Approve Pillar/Train Commitments��6    $��Center=�5      
�������$��16(�5       $��Normal$�5!      $��SemiBold"�5"      $
��Wrap=�5#      .��5%      ��  �� Orientation$ Vertical=�6   .+�5&      �� $����OPlanner gives each of the pillars within your Product Group the option of formally 'approving' the set of backlog items that each pillar will commit to completing within a given train.  The general idea is that at the start of each train (or at any other agreed-to time each train), the owners for each pillar will decide on the work they want to commit to for that train, create backlog items to represent that work (if they don't exist already), and then set the commitment status of those backlog items to 'Committed' (or 'In Progress' if the work has already begun).��6       WelcomeTextBlockStyle#��[  6c  $��0,10,0,0q�6�  $��12(�6�  �� 5'      $����Once you click the 'OK' button below to approve all the committed backlog items you've selected for the current train, OPlanner will then take a snapshot of all these items for posterity, and at any time thereafter, you can select the 'Train Review' view using the ribbon on the Backlog tab to review the current state of all these items, for any pillar or train.��6    #��[  6�  $��0,10,0,0q�6�  $��12(�6�  5(      5)      �� 5+      $ 
Horizontal=�6   $��1��62   $��1��6?   $��1��6O   .+�5,      �� $��Pillar: ��6   #��[  6,   $
��Left=�6[   �� 5-      -   5         ��Name$ 	PillarBox��5-      $��WinBlue ��6/   #��[  6?   $
��Left=�6n   $
��Bold"�6�   5.      �� 50      $ 
Horizontal=�6   $��2��62   $��1��6?   $��1��6O   .+�51      �� $��Train: ��6   #��[  6+   $
��Left=�6Z   �� 52      -   5        $ TrainBox��52      $��xxxxxxx ��6.   #��[  6>   $
��Left=�6m   $
��Bold"�6�   53      �� 55      $ 
Horizontal=�6   $��3��62   $��1��6?   $��1��6O   .+�56      �� $��Item Count: ��6   #��[  60   $
��Left=�6_   �� 57      -   5        $ ItemCountBox��57      $��xxxxxxx ��62   #��[  6B   $
��Left=�6q   $
��Bold"�6�   58      �� 5:      $ Vertical=�6   $��4��60   $��0��6=   $��2��6M   $��Center=�6a   .+�5;      �� $q��kClick OK if your pillar is ready to commit to the completion of these backlog items for the selected train:��6   #��[  6�   $��Center=�6�   5<      5?      �� 5A   
   $��1��6   
���   �$��1q�62   �� 5B   
   $ 
Horizontal=�6   $��1��6.   $��Center=�6;   .+�5C      �� -   5        	 ��Name$	 OKButton��5C      $��OK��6'   $��24��64   $��70��6@   $��10q�6K     RoundCorner#��[ 6W   �� 5D      -   5        $	 CancelButton��5D      $��Cancel��6+   $��24��6<   $��70��6H   $��10q�6S   #��[ 6_   5E      5F      5G      