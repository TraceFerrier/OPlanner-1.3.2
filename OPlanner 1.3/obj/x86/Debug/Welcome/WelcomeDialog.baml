   M S B A M L   `   `   `  ����  OPlanner 1.0$    PlannerNameSpace.WelcomeDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Welcome to OPlanner��5   	    � SizeToContent$ WidthAndHeight=�6%    � 
ResizeMode$ NoResize=�6D   .��5      � ���	#33E53D10 � RowDefinitions 5   
   �� 5      $��* �6   �� 5	      $��50 �6   .+�5   
   � ��5   
   �� 5       �� ImageSource$( "/Images/ProductGroupBackground.jpg��6    j� Stretch$ UniformToFill=�6J    �� Opacity$	 0.8Q�6b    5   
   �� 5      $	��100 �6   �� 5      $��60 �6   �� 5      $��90 �6   �� 5      $��90 �6   �� 5      $��50 �6    � ColumnDefinitions 5   
   �� 5      $��88 �6   �� 5      $	��525 �6   .+�5   
   �� $��0��6   $��0��6   $��/Images/OPlannerLogo.jpg��6-    J� Opacity$	 0.7Q�6O   �� 5   
   $��0��6   $��1��6!   $��1��61   $����Welcome to OPlanner!  This tool is designed to allow you and your Office team to more easily schedule and track your work, both short-term (the current train), and longer-term ('Release Planning').��6E      WelcomeTextBlockStyle#��[  5       �� 5"   
   $��3��6   $��2��6!   $����When you click the 'Next' button, you'll be taken to a dialog that shows you all the Product Groups already defined and available in OPlanner.  If your team is not on the list, you'll be able to easily create a new Product Group that represents your team.��65   #��[  5#      5%      �� 5'   
   $��1��6   
���   �$��1q�62   �� 5(   
   	 �� Orientation$	 
Horizontal=�6   $��1��6.   $��Center=�6;   .+�5)      �� -   5        
 ��Name$
 
NextButton��5)      $��Next >��6)   $��24��6:   $��70��6F   $��10q�6Q     RoundCorner#��[ 6]   �� 5*      -   5        $
 MoreInfo��5*      $��More Info...��6'   $��24��6>   $��90��6J   $��10q�6U   #��[ 6a   �� 5+      -   5        $
 CancelButton��5+      $��Cancel��6+   $��24��6<   $��70��6H   $��10q�6S   #��[ 6_   5,      5-      5.      