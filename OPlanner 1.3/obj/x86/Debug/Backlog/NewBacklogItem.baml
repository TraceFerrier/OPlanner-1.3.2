   M S B A M L   `   `   `  ����  OPlanner 1.0+    %PlannerNameSpace.NewBacklogItemDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  New Backlog Item��5   	    � SizeToContent$ WidthAndHeight=�6"    � 
ResizeMode$ NoResize=�6A   .��5      � 
������� � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��50 �6   .+�5   
   �� $��0��6   $��0��6   $��1��6.   
���   �$��1q�6S   ���	#FFE95C365      � 5   
    5      �� 5      $��10 �6    �� 5      $��32 �6    �� 5      $��32 �6    �� 5      $��32 �6    �� 5      $��32 �6    �� 5      $��32 �6    �� 5      $	��120 �6    �� 5      $��32 �6    �� 5      $��60 �6    �� 5      $��60 �6    �� 5      $��90 �6    �� 5      $��* �6     � ColumnDefinitions 5      �� 5      $��20 �6#   �� 5       $��* �6#   �� 5!      $��20 �6#   .+�5$      �� $��1��6   $��1��6&   $��2��66   $
��Left=�6G   $	��Top=�6b    �� Orientation$ 
Horizontal=�6z   .+�5%      �� $��Title:��6   $��0,0,5,0q�6*   $��14(�6;   �� 5&      -   5         ��Name$ TitleBox��5&      $
��Left=�6,   $	��Top=�6G   $	��323��6_   $��60��6k   5'      �� 5)      $��3��6   $��1��6%   $E��?Select the Pillar and Train to assign this new Backlog Item to:��65   $��14(�6|   �� 5+      $��4��6   $��1��6&   $
��Left=�66   $	��Top=�6Q   $ 
Horizontal=�6i   .+�5,      �� $��Pillar:��6   $��0,0,5,0q�6+   $��14(�6<   �� 5-      -   5         ��Name$ PillarCombo��5-      $
��Left=�60   $	��Top=�6K   $	��323��6c    �� DisplayMemberPath$ Title��6o   5.      �� 50      $��5��6   $��1��6&   $
��Left=�66   $	��Top=�6Q   $ 
Horizontal=�6i   .+�51      �� $��Train:��6   $��0,0,5,0q�6*   $��14(�6;   �� 52      -   5        $ 
TrainCombo��52      $
��Left=�6/   $	��Top=�6J   $	��323��6b   $ Title��6n   53      �� 55      $��6��6   $��1��6%   $����Select the Spec that will describe the functionality to be implemented for this backlog item (or, you can select 'Spec'd in Backlog', and then use the RichText Description and Acceptance Criteria fields of this backlog item to author your spec).��65   $	��350��62  $
��Wrap=�6>  $��14(�6R  $��Center=�6`  �� 57      $��7                     ��6   $��1��58      $
��Left=�6'   $	��Top=�6B   $ 
Horizontal=�6Z   .+�59      �� $��Spec:��6   $��0,0,5,0q�6)   $��14(�6:   �� 5:      -   5        $ 	SpecCombo��5:      $
��Left=�6.   $	��Top=�6I   $	��323��6a   5;      �� 5=      $��8��6   $��1��6&   $ Vertical=�66   $��10,0,0,0q�6M   .+�5>      �� $����If you don't see any specs for your product group on the above list (or the spec for this backlog item is in the collection of specs for a different product group), select a different Spec Team below.��6   $	��350��6�   $
��Wrap=�6�   $��12(�6  $��Center=�6  5?      �� 5A      $��9��6   $��1��6&   $ Vertical=�66   $��
40,0,120,0q�6M   .+�5B      �� $)��#Spec Team (for this Backlog Item): ��6    %  !StandardDialogTextBlockLabelStyle#��[  6G   �� 5C      -   5        $ SpecTeamListCombo��5C      5D      �� 5F      $��10��6   $��1��6'   $ 
Horizontal=�67   .+�5G      �� $����Note: Backlog Items are intended to be created only by one of the Product Owners (usually your PM) for your pillar.  If you are not your pillar's Product Owner, but you need a Backlog Item created, please contact your Product Owner before doing so.��6   $	��350��6  $
��Wrap=�6(  $��12(�6<  $��Center=�6J  5H      5K      �� 5L   
   $��1��6   $��1��6%   $ 
Horizontal=�62   $��Right=�6K   $��5q�6g   .+�5M      �� -   5        	 ��Name$	 OKButton��5M      $��OK��6'   $��24��64   $��50��6@   $��5q�6K     RoundCorner#��[ 6m   �� 5N      -   5        $	 CancelButton��5N      $��Cancel��6+   $��24��6<   $��50��6H   $��5q�6S   #��[ 6y   5O      5R      5S      