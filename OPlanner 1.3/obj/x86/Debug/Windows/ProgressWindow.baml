   M S B A M L   `   `   `  ����G  COPlanner 1.0, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:PlannerNameSpace.ViewsPlannerNameSpace.Views  5   	   %    PlannerNameSpace.ProgressWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @PlannerControls$clr-namespace:PlannerNameSpace.Views     5   	   -   5          � Title$  ProgressWindow��5   	   $	��200��6    $	��403��6-    � WindowStartupLocation$ CenterOwner=�69    � Topmost .  � 
ResizeMode$ NoResize=�6�    J� Effect 5      3  -System.Windows.Media.Effects.DropShadowEffect  5   
   .��5	      � ��5
   
   �� 5       �� EndPoint$ 0.5,1&�6"    �� 
StartPoint$ 0.5,0&�63   .��5      �  � Color$ PaleGoldenrod��6    � Offset$ 0Q�65   � 5      $ White��6   $ 1Q�6-   5      .+�5   
   � -   5        $��ProgressBarControl��5   T   $��20��6   $��Stretch=�6"   $��
15,89,15,0q�6@   $	��Top=�6n   �� 5   
   -   5        $��CancelButton��5   ]   $��Cancel��6   $��23��6"   $��Center=�6.   $��0,0,0,10q�6K   $��Bottom=�6q   $��75��6�   6�   �� 5   
   -   5        $��
MessageBox��5   R   $��60��6   $��Stretch=�6    $��
15,10,15,0q�6>   $�� ��6d   $	��Top=�6l   $
��Wrap=�6�   	 �� TextAlignment$	 Center=�6�   $��14(�6�   5      5      