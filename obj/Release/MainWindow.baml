   M S B A M L   `   `   `  ���� A  =ChessB, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ChessBChessB      ChessB.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:ChessB     -     �Name$  
mainWindow�� � Title$ Chess�� � Icon$ bP.PNG��$	��800��$	��800�� � WindowStartupLocation$ CenterScreen=� � WindowStyle$ ThreeDBorderWindow=�.��� -    �Name$ UiGrid��$��0q� � RowDefinitions �� $
��1.5* ��� $	��12* ��� $
��1.5* � � ColumnDefinitions �� $	��15* ��� $	��15* ��� $	��10* �.+�(� -    (�Name$ boardVB��$��0��$��1��	 J� RenderTransformOrigin$	 0.5,0.5&�$��2��.���� -   
 ��Name$
 boardCanvas��$	��200��$	��200��$	��500��$	��500��(� -   $ whiteCapturedVB��$��0��$��2��$
��Left=�$	��Top=�.��� -   $ whiteCapturedGrid��$
��Left=� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� .+��� -    ��Name$ whiteCapturedLabel��$�� ��$��16��$
��Left=�$��0q�$��Center=�$��11(�(� -   $ blackCapturedVB��$��0��$
��Left=�$	��Top=�.��� -	   $ blackCapturedGrid�� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� .+��� -
   $ blackCapturedLabel��$�� ��$��16��$
��Left=�$��0q�$��Center=�$��11(�(� -   $ chessNotationVB��$��2��$��0q�$��1�� (� Stretch$
 Fill=�.���� -    ��Name$ moveListBox��$	��142��$	��216��$��11(�$��Calibri)����	#00E8E3E3(� -   $ BlackPromotionVB��$��2��$��2��$��0q�$
 Fill=�$��Center=�$��Center=�.��� $��Center=�$��Center=� �� $��5* ���  �� �� �� �� .+��� $��0��$��0��$��Images/wQ.PNG��$
��Fill=�$��15��$��15���� $��1��$��0��$��Images/wR.PNG��$
��Fill=�$��15��$��15���� $��2��$��0��$��Images/wN.PNG��$
��Fill=�$��15��$��15���� $��3��$��0��$��Images/wB.PNG��$
��Fill=�$��15��$��15��	� -    	�Name$
 wQRB��$��0��$��1��$��0q�$	��Top=� d� 	IsChecked$
 TrueY�	� -   $
 wRRB��$��1��$��1��$��0q�	� -   $
 wNRB��$��2��$��1��$��0q�	� -   $
 wBRB��$��3��$��1��$��0q�(� -   $ whitePromotionVB��$��2��$��0��$��0q�$
 Fill=�$��Center=�$��Center=�.��� $��Center=�$��Center=� �� ��  �� �� �� �� .+��� $��0��$��0��$��bQ.PNG��$
��Fill=�$��15��$��15���� $��1��$��0��$��bR.PNG��$
��Fill=�$��15��$��15���� $��2��$��0��$��Images/bN.PNG��$
��Fill=�$��15��$��15���� $��3��$��0��$��Images/bB.PNG��$
��Fill=�$��15��$��15��	� -   $
 bQRB��$��0��$��1��$��0q�$
 TrueY�	� -   $
 bRRB��$��1��$��1��$��0q�	� -   $
 bNRB��$��2��$��1��$��0q�	� -   $
 bBRB��$��3��$��1��$��0q�