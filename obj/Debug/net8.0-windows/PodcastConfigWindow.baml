   M S B A M L   `   `   `  ����M  IVlogManager_Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:VlogManager_Client.ViewModelVlogManager_Client.ViewModel  5   	   C&clr-namespace:VlogManager_Client.RulesVlogManager_Client.Rules  5   	   K*clr-namespace:VlogManager_Client.AnimationVlogManager_Client.Animation  5	   	   6    0VlogManager_Client.ViewModel.PodcastConfigWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   <local*clr-namespace:VlogManager_Client.ViewModel     5   	   8rules&clr-namespace:VlogManager_Client.Rules     5   	   <anims*clr-namespace:VlogManager_Client.Animation     5	   	     � 
ResizeMode$  NoResize=�5
   	    � Title$ PodcastConfigWindow��5   	   $	��350��6%   $	��600��62    � 	Resources 5         ChangeColumnsToEdit6   0VlogManager_Client.Animation.GridLengthAnimation   From	   To g� 	BeginTime g� Duration �� TargetProperty �� 
TargetName  ChangeColumnsToView��%  &	        &	     �� 5   
   .��5        $ 0* �6(   $ 1* �62   	 0:0:06:   $ 0:0:2D�6L   $ Width�6]   $ EditPodNameCol��6     5      $ 1* �6(   $ 0* �62   	 0:0:06:   $ 0:0:2D�6L   $ Width�6]   $ ViewPodNameCol��6   5      �� 5   
   .��5        $ 0* �6(   $ 1* �62   	 0:0:06:   $ 0:0:2D�6L   $ Width�6]   $ ViewPodNameCol��6     5      $ 1* �6(   $ 0* �62   	 0:0:06:   $ 0:0:2D�6L   $ Width�6]   $ EditPodNameCol��6   5      5      .��5      �� .@�5   
   �� $
��Info��6   	 � DataContext	 6    �� *RecordToConfig+.��5      �� $��Hidden=�6   .��5      �� .+�5      �� -   5        $��PodcastIcon��5      $��Center=�60   ��5      �� 5      
 �� Path$
 ImageUrl�6&    �� ValidationRules 5   "   .   (VlogManager_Client.Rules.ImageSourceRule  5   &   5       5"      � 5#      -   5        $��NameInputField��5#       � ColumnDefinitions 5$      �� 5%      -   5         �Name$ ViewPodNameCol��5%   /   �� 5&      -   5        $ EditPodNameCol��5&   /   $��0 �6E   .+�5(      �  5)      �� 5*   "   �� 5+   "   $��40 �63   .+�5-      �� $��22(�6>   $
��Bold)�6L   $
��Left=�6^   $��Center=�6y   ${�False��6�   ��6(   �� *Name+6�   �� 5.      -   5        $��Center=�5.   %   $��1��6G   ��5/   "   �� 50   &   $��/Icons/4277132.png��6,   $��40��6H   52      53      � 54      $��1��6    55      �� 56   "   �� 57   "   $��50 �63   .+�59      �� -   5        $��	NameInput��59   &   $��22(�67   $
��Bold)�6E   �� 5:      $��1��6)   .+�5;   "   �� $
��Save��6)   ��68   �� *ChangeNameCommand+��6^   ��  �� ElementName$ 	NameInput��$

 Text��� 5<   "   -   5        $��Stop editing��5<   )   6@   5=      5>      5?      �� 5A      $��15(�6    $
��Left=�6K   $
��Wrap=�6f   ��6.   �� *Description+6f   �� 5C      $��	20,80,0,0q�6    $��Podcast link��63   $��16(�6G   $
��Left=�6U   �� 5D      -   5        $
��Left=�5D   6   $��15(�6Q   
����� ���6    �� *Link+��5E      �� 5F       �� 
TargetType" ��6$    �� Triggers 5G   "   P� 5H   &    P� Property �@J�IsMouseOver P� Value$
 True��6E   . �5I   *   ��  �� Property � �� �� Value
 ��� �6G   �� 5J   *    �@�Cursor$
 Hand��6C   5K   '   5M      5O      � 5P      $��	20,50,0,0q�6    5Q      �� 5R      �� 5S      $��30 �6/   .+�5U      �� $��12(�6@   $��CharacterEllipsis=�6N   ��6%   �� *	Directory+6N   �� 5V      -	   5        $��1��5V   !   ��5W      �� 5X   "   $��/Icons/folder_icon.png��6(   5Z      5[      5]      5^      5_      �� 5`   
   $��	Configure��6   .��5a      �� $��Hidden=�6   .��5b      �  5c      �� 5d      �� 5e       � RowDefinitions 5g      �� 5h      $��60 �6(   �� 5i      $
��Auto �6(   �� 5j      .+�5l      �� $��
Group name��6    $��16(�62   $��Center=�6@   $��Right=�6[   $��0,0,15,0q�6w   �� 5m      -
   5        $��Center=�5m   >   $��30��6Y   $��15,0,0,0q�6e   $��1��6w   ��6   �� *Groups+ � ContextMenu 5n      �� 5o      .@�5p   "   m� $��Add new Group��6+   5q      5s      �� 5t      $��Image configuring��6    $��16(�69   $��2��6G   $��1��6[   $��Center=�6h   $��Center=�6�   � 5u      $��2��6   $��2��6(    5v      �� 5w      �� 5x      $��50 �6/   .+�5z      �� $��0��6$   ��5{      !� 5|       !� Visual 6*   �� $ PodcastIcon��6W   5~      �� 5      -   5        $��1��5   !   
���������5�      �� 5�   "   $��/Icons/frame_icon.png��6(   $��40��6G   $��40��6S   5�      5�      5�      5�      5�      5�      5�      