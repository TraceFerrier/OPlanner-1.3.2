   M S B A M L   `   `   `  ����G  COPlanner 1.0, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:PlannerNameSpacePlannerNameSpace  5          PlannerNameSpace.App   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ,cclr-namespace:PlannerNameSpace     5        �� 	Resources  5         ReviewPagesBackgroundBrush"   PlannerNameSpace.BacklogItem!   ReviewPagesBackgroundColor �� Color=  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType �� Property �� Value �� TextAlignment �� Triggers P� Property	 P� Value
 �� RelativeSource  ReviewDataGridHeaderStyle  DialogTabStyle  ExperienceViewTitleStyle ! WorkItemSummaryViewTitleStyle �� BasedOn ! BacklogViewSummaryHeaderStyle  TrainCommitmentLabelStyle  BacklogSpecNameStyle  TrainCommitmentValueStyle  	 BacklogViewSummaryStatsStyle %
 !BacklogViewSummaryStatsPanelStyle + 'BacklogViewSummaryHeaderBackgroundStyle  ExperienceViewPropertyStyle $  ExperienceViewPropertyLabelStyle ! ExperienceViewItemBorderStyle # ExperienceViewTopRowBorderStyle # ExperienceViewStatusBorderStyle  StatusBarTextBlockStyle  LandingDateTextBlockStyle # NotificationPanelTextBlockStyle  WelcomeTextBlockStyle  WelcomeButtonStyle  RoundCorner �� 
TargetType �Name ��Name �� CornerRadius 
� GradientOrigin �� RelativeTransform �� CenterX �� CenterY �� ScaleX �� ScaleY T� X T� Y � Offset � Color �� Triggers �� 
TargetName  InformationPanelBorderStyle  DataGridTextColumnStyle & "DataGridReviewPagesTextColumnStyle # DataGridReadOnlyTextColumnStyle  PropToolTip*  $System.Windows.Controls.DataGridCell  CellErrorStyle �� Path " DataGridTextColumnEditingStyle 1 -DataGridTextColumnEditingStyleCustomMaxLength  StandardTextBlockLabelStyle %  !StandardDialogTextBlockLabelStyle '! #StandardDialogTextBlockContentStyle %" !StandardDialogTextBoxContentStyle -# )StandardDialogTextBoxReadOnlyContentStyle *$ &StandardDialogLargeTextBoxContentStyle % StandardDialogComboStyle !& StandardDialogStackPanelStyle ' StandardTextBlockStyle ( StandardHoverTextBlockStyle ) HoverBorderStyle * HoverStackPanelStyle + FilterComboLabelStyle , FilterComboStyle - DataGridComboStyle . ItemGridStyle&   System.Windows.Controls.DataGrid / ItemGridTextBoxStyle 0 ItemGridTextBlockStyle 1 ItemUpdateTextStyle 2 ItemGridTitleStyle3  -System.Windows.Media.Effects.DropShadowEffect "3 OverviewHeadingStyleDropShadow   
BlurRadius   ShadowDepth   Opacity    Color 4 OverviewHeadingStyle 5 BacklogLargeTitleStyle 6 FeatureTeamEditCellStyle 7 HoverUnderlineStyle 8 HandOnHoverBorderStyle 9 DeleteButtonStyle : ToolTipTextBlockStyle! �� RadiusX" �� RadiusY# J� BitmapEffect$ F� ShadowDepth% �� Orientation  ; TimelineStandardToolTipStyle& �� EndPoint' �� 
StartPoint < BacklogToolTipStyle( � DataContext) �� AncestorType��%�E  &	        '       &	 �    &	 �    &	 �    &	 L    7  &	 �    &	 "    &	 �    &	 �	    &		     &	
 .    &	 �    &	 �    &	 i    7  &	     &	 M    &	 �    &	 #    &	 4    &	 �    &	     &	 \    &	 �    &	 �    &	 k    &	     7   &	 �    &	 y    &	 �    &	 �     &	 �!    &	 k"    &	  6#    &	! $    &	" �$    &	# �%    7 " &	$ 1&    7 " &	% �&    &	& �'    &	' $(    &	( )    7 ' &	) �)    &	* i*    &	+ �*    &	, �+    &	- �,    &	. `-    &	/ �.    &	0 u/    &	1 0    &	2 1    &	3 �1    &	4 :2    7 3 &	5 q3    &	6 �4    &	7 z5    7 0 &	8 �6    &	9 j7    &	: *9    '_� �9    7 : &	; q<    7 : &	< �>    �� 5	   
   # Z 6=   �� 5   
   "  6   .�5      ��  � �� 5      �� 5      .R�5      �� $��Segoe UI)�6$   $��12(�6:   $
��Wrap=�6H   $ Center=�6\   $
��Bold"�6s   ��6�   �� 6�   5      5       5      P� 5       �@J�IsMouseOver	 . . �5      ��  �@�ToolTip 60   �� *!Column.(ToolTipService.ToolTip)+
 �� *Self+5      5      �� 5   
   "  62   .�5      ��  � ��
 � ���6+   �� 5       � ��$ 1q�6'   �� 5       � �� 5      �� 5      .R�5       �� $��2q�6$   $��Segoe UI)�6/   $��12(�6E   $
��Wrap=�6S   $ Center=�6g   $
��Bold"�6~   ��6�   �� 6�   5!      5#      5$      �� 5&   
   " ��6'   .�5'      ��  � ��$ Segoe UI)�6+   �� 5(       � ��$ 14(�6)   �� 5)       � ��$ Normal"�6+   �� 5*       � ��
 �   �6+   �� 5+       � ��$ Center=�62   �� 5,       � ��$ 0,0,0,0q�6'   5-      �� 5/   
   " ��61   .�50      ��  � ��$ Segoe UI)�6+   �� 51       � ��$ 16(�6)   �� 52       � ��$ SemiBold"�6+   �� 53       � ��
 �   �6+   �� 54       � ��$	 Top=�62   �� 55       � ��$ 0,0,0,0q�6'   �� 56       � ��$
 Wrap=�6-   �� 57       � ��$ CharacterEllipsis=�6-   58      �� 5:   
   " ��66   8   .�5;      ��  � ��$ 14(�6)   5<      �� 5>   
   " ��66   .�5?      ��  � ��$ Segoe UI)�6+   �� 5@       � ��$ 16(�6)   �� 5A       � ��$ SemiBold"�6+   �� 5B       � ��
 �   �6+   �� 5C       � ��$ Center=�62   �� 5D       � ��$ 5q�6'   �� 5E       � ��$
 Wrap=�6-   �� 5F       � ��$ CharacterEllipsis=�6-   �� 5G       � ��$ 2��6-   5H      �� 5J   
   " ��62   .�5K      ��  � ��$ Segoe UI)�6+   �� 5L       � ��$ 14(�6)   �� 5M       � ��$ SemiBold"�6+   �� 5N       � ��
 �   �6+   �� 5O       � ��$
 Left=�64   �� 5P       � ��$ Center=�62   �� 5Q       � ��$ 10,0,0,0q�6'   �� 5R       � ��$
 Wrap=�6-   5S      �� 5U   
   " ��6-   .�5V      ��  � ��$ Segoe UI)�6+   �� 5W       � ��$ 14(�6)   �� 5X       � ��$ SemiBold"�6+   �� 5Y       � ��
 �   �6+   �� 5Z       � ��$ Center=�64   �� 5[       � ��$ Center=�62   �� 5\       � ��$ 1q�6'   �� 5]       � ��$
 Wrap=�6-   5^      �� 5`   
   " ��62   .�5a      ��  � ��$ Segoe UI)�6+   �� 5b       � ��$ 14(�6)   �� 5c       � ��$ Normal"�6+   �� 5d       � ��
 �   �6+   �� 5e       � ��$
 Left=�64   �� 5f       � ��$ Center=�62   �� 5g       � ��$
 Wrap=�6-   5h      �� 5j   
   " ��65   .�5k      ��  � ��$ Segoe UI)�6+   �� 5l       � ��$ 12(�6)   �� 5m       � ��$ SemiBold"�6+   �� 5n       � ��
 �   �6+   �� 5o       � ��$ Center=�64   �� 5p       � ��$ CharacterEllipsis=�6-   5q      �� 5s   
   " ��6:   .�5t      ��  �@��Orientation$ 
Horizontal=�6,   �� 5u       � ��$ Right=�64   �� 5v       � ��$	 Top=�62   �� 5w       � ��$ 0,3,8,3q�6'   5x      �� 5z   
   " ��6@   .�5{      ��  � ��$ 1q�60   �� 5|       � ��
 �����6,   �� 5}       � ��$ 1q�6'   �� 5~       � ��
 ��ح�6+   �� 5       � ��$ 2��6-   5�      �� 5�   
   " ��64   .�5�      ��  � ��$ Segoe UI)�6+   �� 5�       � ��$ 11(�6)   �� 5�       � ��$
 Bold"�6+   �� 5�       � ��
 �   �6+   �� 5�       � ��$ Center=�62   �� 5�       � ��$ 0,0,5,0q�6'   �� 5�       � ��$ NoWrap=�6-   �� 5�       � ��$ CharacterEllipsis=�6-   �� 5�       � ��$ Right=�64   5�      �� 5�   
   " ��69   8   .�5�      ��  � ��$ Normal"�6+   �� 5�       � ��
 �   �6+   �� 5�       � ��$
 Left=�64   5�      �� 5�   
   " ��66   .�5�      ��  � ��$ 1q�60   5�      �� 5�   
   " ��68   .�5�      ��  � ��$ 1q�60   �� 5�       � ��$ 6q�6'   �� 5�       � ��$	 Top=�62   5�      �� 5�   
   " ��68   .�5�      ��  � ��$ 1q�60   5�      �� 5�   
   " ��60   .�5�      ��  � ��$ Segoe UI)�6+   �� 5�       � ��$ 18(�6)   �� 5�       � ��$ Normal"�6+   �� 5�       � ��
 �����6+   �� 5�       � ��$ Center=�62   �� 5�       � ��$ 0,0,5,0q�6'   5�      �� 5�   
   " ��62   .�5�      ��  � ��$ Segoe UI)�6+   �� 5�       � ��$ 14(�6)   �� 5�       � ��$ SemiBold"�6+   �� 5�       � ��$ Center=�62   5�      �� 5�   
   " ��68   .�5�      ��  � ��$ Segoe UI)�6+   �� 5�       � ��$ 12(�6)   �� 5�       � ��$ Normal"�6+   �� 5�       � ��
 ��  �6+   �� 5�       � ��$ Center=�62   �� 5�       � ��$ 3q�6'   5�      �� 5�   
   " ��6.   .�5�      ��  � ��$ Segoe UI)�6+   �� 5�       � ��$ 16(�6)   �� 5�       � ��$ Normal"�6+   �� 5�       � ��
 �����6+   �� 5�       � ��$ Center=�64   �� 5�       � ��$ Center=�62   �� 5�       � ��$
 Wrap=�6-   �� 5�       � ��$ 5q�6'   5�      �� 5�   
   " ��6+   .�5�      ��  � ��$ 70��6)   �� 5�       � ��$ 28��6'   �� 5�       � ��$ 10q�6'   �� 5�       � ��$ Segoe UI)�6+   �� 5�       � ��$ 14(�6)   �� 5�       � ��
 �   �6+   �� 5�       � ��
 �����6+   5�      �� 5�   
   " ��6$   .�5�      ��  � ��$ 70��6)   �� 5�       � ��$ 24��6'   �� 5�       � ��$ 10q�6'   �� 5�       � ��$ Segoe UI)�6+   �� 5�       � ��$ 14(�6)   �� 5�       � ��$ Center=�6;   �� 5�       � ��$ Center=�69   �� 5�       � ��$ 1q�6(   �� 5�       � �� 5�      �� 5�      " ��6&   .R�5�      � $
 grid��6   .+�5�      �� $ border��6%   $ 3��65   
���   �$��1q�6Z   ��5�   "   
� 5�   &   $ 0.496,1.052&�6:    5�   *   V� 5�   .   .~�5�   2   �� $	 0.5Q�6A   $	 0.5Q�6O   $	 1.5Q�5�   $   $	 1.5Q�61   T� 5�   2   $
 0.02Q�6E   $	 0.3Q�6N   5�   /   .��5�   *   � $ 1Q�67   $ 	#00000000��6B   � 5�   *   $	 0.3Q�67   $ 	#FFFFFFFF��6D   5�   '   ._�5�   "   �� $��Center=�63   $��Center=�5�      $��Normal"�5�      6<   5�      5�       5�      P� 5�       � ��	 . . �5�   "   �� $ border��6?    � �� 5�   &   
� 5�   *   $ 0.496,1.052&�6>    5�   .   V� 5�   2   .~�5�   6   �� $	 0.5Q�6E   $	 0.5Q�6S   $	 1.5Q�6a   $	 1.5Q�6n   T� 5�   6   $
 0.02Q�6I   $	 0.3Q�6R   5�   3   .��5�   .   � $ 	#00000000��6;   $ 1Q�6M   � 5�   .   $ 	#FF303030��6;   $	 0.3Q�6M   5   +   5  #   5     P� 5      �@J�IsMouseOver	 . . �5  "   �� $ border��6@    � �� �	#FF33962B6T   5     P� 5      � {�	 .  . �5  "   �� $
 grid��6<    �@J�Opacity$
 0.25Q�6N   5	     5     5     5     �� 5  
   " ��64   .�5     ��  � ��$ 32��6'   �� 5      � ��
 �����6+   �� 5      � ��
 �   �6,   �� 5      � ��$ 1q�60   5     �� 5  
   " ��60   .�5     ��  � ��$
 Wrap=�6-   �� 5      � ��$ Segoe UI)�6+   �� 5      � ��$ 14(�6)   5     �� 5  
   " ��6;   .�5      ��  � ��$
 Wrap=�6-   �� 5!      � ��$ Segoe UI)�6+   �� 5"      � ��$ 14(�6)   �� 5#      � ��8   6+   5$     �� 5&  
   " ��68   .�5'     ��  � ��$
 Wrap=�6-   �� 5(      � ��$ Segoe UI)�6+   �� 5)      � ��$ 14(�6)   �� 5*      � ��
 �����6+   5+     �� 5-  
   "  6$   .�5.     ��  �@�ToolTip 6(   �� *PropToolTip+5/     �� 51  
   " ��6'   .�52     ��  � ��$ -2q�6(    53     P� 54      �@=�HasError	 . . �55     ��  � ��
 �  ��63   �� 56      �@�ToolTip 57     �� 
 �� *Self+$) #(Validation.Errors)[0].ErrorContent�59     5;     �� 5=  
   " ��67   .�5>     ��  �@��TextWrapping$
 Wrap=�6-   �� 5?      � ��$ Segoe UI)�6+   �� 5@      � ��$ 14(�6)   5A     �� 5C  
   " ��6F   .�5D     ��  �@��TextWrapping$
 Wrap=�6-   �� 5E      � ��$ Segoe UI)�6+   �� 5F      � ��$ 14(�6)   �� 5G      �@��	MaxLength$	 100��6*   5H     �� 5J  
   " ��64   .�5K     ��  �@��Padding$ 6,0,0,0q�6(   �� 5L      � ��$ Center=�62   �� 5M      � ��$ Segoe UI)�6+   �� 5N      � ��$ 14(�6)   5O     �� 5Q  
   " ��6:   .�5R     ��  �@��Padding$ 0,0,0,0q�6(   �� 5S      � ��$ Center=�62   �� 5T      � ��$ Segoe UI)�6+   �� 5U      � ��$ 12(�6)   5V     �� 5X  
   " ��6<   .�5Y     ��  �@��Padding$ 0,0,0,0q�6(   �� 5Z      � ��$ Center=�62   �� 5[      � ��$ Segoe UI)�6+   �� 5\      � ��
 �����6+   �� 5]      � ��$ 14(�6)   5^     �� 5`  
   " ��6:   .�5a     ��  � ��$ 0,0,0,0q�6(   �� 5b      � ��$ Center=�62   �� 5c      � ��$ Segoe UI)�6+   �� 5d      � ��$ 14(�6)   5e     �� 5g  
   " ��6B   8   .�5h     ��  �@��
IsReadOnly . 6+   �� 5i      � ��
 �����6+   5j     �� 5l  
   " ��6?   8   .�5m     ��  � ��$ 3q�6'   �� 5n      �@��TextWrapping$
 Wrap=�6-   �� 5o     # �@��VerticalScrollBarVisibility$
 Auto=�6<   5p     �� 5r  
   " ��61   .�5s     ��  � ��$ 0,0,0,0q�6(   �� 5t      � ��$ Center=�62   �� 5u      � ��$ Segoe UI)�6+   �� 5v      � ��$ 14(�6)   5w     �� 5y  
   " ��66   .�5z     ��  � ��$ 5q�6'   �� 5{      � ��$ Center=�62   5|     �� 5~  
   " ��6/   .�5     ��  �@��Padding$ 0,0,0,0q�6(   �� 5�      � ��$ Center=�62   �� 5�      � ��$ Segoe UI)�6+   �� 5�      � ��$ 14(�6)   �� 5�      � ��$ SemiBold"�6+   5�     �� 5�  
   " ��64   8    5�     P� 5�      �@J�IsMouseOver	 . . �5�     ��  � ��$ 	Underline{�68   �� 5�      �@�Cursor$
 Hand��6/   5�     5�     �� 5�  
   " ��6)    5�     P� 5�      �@J�IsMouseOver	 . . �5�     ��  �@�Cursor$
 Hand��6/   5�     5�     �� 5�  
   " ��6-    5�     P� 5�      �@J�IsMouseOver	 . . �5�     ��  �@�Cursor$
 Hand��6/   5�     5�     �� 5�  
   " ��6.   .�5�     ��  � ��$ 0,0,4,0q�6(   �� 5�      � ��$ Center=�62   �� 5�      � ��$ Segoe UI)�6+   �� 5�      � ��$ 14(�6)   5�     �� 5�  
   " ��6)   .�5�     ��  � ��$ 4,0,0,0q�6(   �� 5�      � ��$
 Left=�64   �� 5�      � ��$ Center=�62   �� 5�      � ��$ 24��6'   �� 5�      � ��$	 180��6&   �� 5�      � ��$ Segoe UI)�6+   �� 5�      � ��$ 14(�6)   5�     �� 5�  
   " ��6+   .�5�     ��  � ��$ Segoe UI)�6+   �� 5�      � ��$ 14(�6)   5�     �� 5�  
   "  6&   .�5�     ��  �@ AutoGenerateColumns .  64   �� 5�      � ��$ Stretch=�64   �� 5�      � ��$ Stretch=�62   �� 5�      � ��$ 5,2,5,5q�6'   �� 5�      �@ CanUserAddRows .  6/   �� 5�      �@ CanUserDeleteRows .  62   �� 5�      �@ SelectionMode$ Single=�6.   5�     �� 5�  
   " ��6-   .�5�     ��  �@��TextWrapping$
 Wrap=�6-   �� 5�      � ��$ Segoe UI)�6+   �� 5�      � ��$ 14(�6)   5�     �� 5�  
   " ��6/   .�5�     ��  � ��$
 Wrap=�6-   �� 5�      � ��$ Segoe UI)�6+   �� 5�      � ��$ 14(�6)   5�     �� 5�  
   " ��6,   .�5�     ��  � ��$
 Wrap=�6-   �� 5�      �@��TextAlignment$ Right=�6.   �� 5�      � ��$ Segoe UI)�6+   �� 5�      � ��$ 12(�6)   �� 5�      �@��Padding$ 0,0,10,0q�6(   5�     �� 5�  
   " ��6+   .�5�     ��  � ��$
 Left=�64   �� 5�      � ��$ Center=�62   �� 5�      � ��$ 10,0,0,0q�6'   �� 5�      � ��$ 14(�6)   �� 5�      � ��$
 Bold"�6+   5�       5�  
   $ 10Q�6B   $ 0Q�6R   $	 0.6Q�6b   $
  Gray��6p   �� 5�  
   " ��6-   .�5�     ��  � ��$
 Left=�64   �� 5�      � ��$ Center=�62   �� 5�      � ��$ Segoe UI)�6+   �� 5�      � ��$ 10,0,0,0q�6'   �� 5�      � ��$ 16(�6)   �� 5�      � ��$
 Bold"�6+   �� 5�      �@J�Effect8   6'   5�     �� 5�  
   " ��6/   .�5�     ��  � ��$
 Wrap=�6-   �� 5�      � ��$ Center=�62   �� 5�      � �� 6+   �� *TrainCommitmentStatusColor+�� 5�      � ��$ 10,0,0,0q�6'   �� 5�      � ��$ 16(�6)   �� 5�      � ��$ SemiBold"�6+   5�     �� 5�  
   " ��61   .�5�     ��  � ��$ 30��6'   �� 5�      � ��
 �����6+   �� 5�      � ��
 �   �6,   �� 5�      � ��$ 1q�60   �� 5�      � ��$	 Top=�6/   5�     �� 5�  
   " ��6,   8    5�     P� 5�      �@J�IsMouseOver	 . . �5�     ��  � ��$ 	Underline{�68   �� 5�      �@�Cursor$
 Hand��6/   5�     .�5�     ��  � ��
 ��  �6+   �� 5       � ��$ Normal"�6+   5     �� 5  
   " ��6/    5     P� 5      �@J�IsMouseOver	 . . �5     ��  �@�Cursor$
 Hand��6/   �� 5      � ��
 �����64   5     .�5
     ��  � ��
 �   �6,   5     �� 5  
   " ��6*   .�5     ��  �@�OverridesDefaultStyle . 66   �� 5      � �� 5     �� 5     " ��6&   .R�5     � .+�5     �� $��/Images/DeleteDate.png��6$   $��1q�6D   $��16��6O   $��16��6[   �� 5     $��Center=�6/   $��Center=�6L   5     5     5      5     P� 5      �@J�IsMouseOver	 . . �5     ��  �@�Cursor$
 Hand��6/   5     5      �� 5"  
   " ��6.   .�5#     ��  � ��$
 Wrap=�6-   �� 5$      � ��$ Segoe UI)�6+   �� 5%      � ��$ 14(�6)   �� 5&      � ��$ 2q�6'   5'     �� 5)  
   " _�6)   .�5*     ��  �@�OverridesDefaultStyle . 66   �� 5+      �@J�Opacity$	 .95Q�6(   �� 5,      � �� 5-     �� 5/     " _�6&   .R�50     � 
������ $��5q�68   #��z��6C   #��z��6c   .+�51     �� 
�������#��z��65   $	! 7.5��6W   $	" 7.5��6e   # 52  "   F� 53  &   $$ 3Q�6=   55     �� 56     $��5q�6/   $��Center=�6;   $��Center=�6X   57     58     5:     �� 5;      � �� 5<     �� 5=     .R�5>     �� $% 
Horizontal=�6%   .+�5?     �� $	��325��69   8��  
���   ���6(   �� 6w   5@     5A     5C     5D     �� 5F  
   " _�65   .�5G     ��  �@�OverridesDefaultStyle . 66   �� 5H      �@_�HasDropShadow . 6.   �� 5I      � �� 5J     �� 5K     " _�6&   .R�5L     �� $% 
Horizontal=�6%   ��5M     �� 5N  "   $& 1,0.5&�66   $' 0,0.5&�6G   .��5O  &   � $ White��63   $ 0Q�6A   � 5P  &   $ 	LightBlue��63   $ 1Q�6E   5Q  #   .+�5S     �� $��/Images/DateAndTime.png��6$   $��5q�6E   �� 5T     ��5U  "   �� 5V  &   .R�5W  *   �� 8��  $��40��6t   $	��250��6�   ��64   �� 6�   5X  '   5Z     5[     5\     5^     5_     �� 5a  
   " _�6,   .�5b     ��  �@J�Opacity$	 .95Q�6(   �� 5c      � �� 5d     �� 5f     " _�6&   .R�5g     � 
������ $��5q�68   #��z��6C   #��z��6c   .+�5h     �� 
�������#��z��65   $	! 7.5��6W   $	" 7.5��6e   # 5i  "   F� 5j  &   $$ 3Q�6=   5l     �� 5m     $��5q�6/   $��Center=�6;   $��Center=�6X   5n     5o     5q     �� 5r      � �� 5s     �� 5t     .R�5v     �� $% 
Horizontal=�6%   ( 5w  !   �� $ DataContext�
 �� ") _�.+�5y     �� $��30(�6(   $
��Bold"�66   $��0,0,5,0q�6H   ��5z  %   �� $ Title��� 5|     $% Vertical=�6)   .+�5}  "   �� $
��Bold"�6,   ��6?   �� $ Class��� 5~  "   $% 
Horizontal=�6-   .+�5  &   �� $��Pillar��60   �� 5�  &   $��: ��60   �� 5�  &   ��60   �� $ 
PillarName�5�  #   �� 5�  "   $% 
Horizontal=�6-   .+�5�  &   �� $��Train��60   �� 5�  &   $��: ��60   �� 5�  &   ��60   �� $ 	TrainName�5�  #   5�     5�     5�     5�     5�     5�     5�     