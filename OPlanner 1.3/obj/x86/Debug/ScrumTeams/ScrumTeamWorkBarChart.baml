   M S B A M L   `   `   `  ����G  COPlanner 1.0, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:PlannerNameSpacePlannerNameSpace  5      ,    &PlannerNameSpace.ScrumTeamWorkBarChart   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,cclr-namespace:PlannerNameSpace     5        � 	Resources  5
         	MemberBar �� Orientation �� Path��%�  &	        �� 5   
   .R�5      �� $ Vertical=�6   $��10q�60   .+�5      �� ��6   �� $ DisplayName�� 5      .+�5      �� $��20��6=   
���   �$��1q�6]   
���������6   �� *WorkBarWidth+._�5      �� $
��Left=�6?   $��1q�6Z   ��6    �� *WorkFillColor+��6e   �� *WorkFillWidth+5      5      �� 5      ��6   �� $ DisplayHoursOfTotal�5      5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��* �6   .+�5      �� $��0��6   $��Scrum Team Members��6%   $��Segoe UI)�6?   $��16(�6U   $��
10,10,10,5q�6c   �� 5      $��1��6   $
��Auto=�6(   $
��Auto=�6K   $��Stretch=�6p   $��Stretch=�6�   .��5      �� $ Vertical=�6   .+�5       �� $��Stretch=�6   $��Stretch=�65   $��2q�6S   
�������
���   �$��1q�6�   ._�5!      �� -   5         ��Name$ BarChartListView��5!   #   ��6=   �� *	Members+5"      5#      5$      5%      5&      