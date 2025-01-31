﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;

namespace ChessB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        Point startingPosition = new Point(0, 0);
        Board board;
        Image hoveredTileImage = new Image();
        Timer UiUpdatetimer;




        public MainWindow()
        {

            InitializeComponent();

            board = new Board();
            board.generateBoardFromFEN();

            board.setValidMoves(board.generateValidMoves(board));
            Game.activeBoard = board;
            Game.boardStates.Add(board);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //Console.WriteLine(board.moveGenerationTest(3));
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine(elapsedMs + " ms spent generating moves.");
            Ui.canvas = this.boardCanvas;
            Ui.squareSize = (int)(this.boardCanvas.Width / board.getBoardSize());
            Thickness boardThickness = new Thickness();
            boardThickness.Left = Ui.squareSize;
            boardThickness.Bottom = Ui.squareSize;
            this.boardVB.Margin = boardThickness;
            Ui.MainWindow = this;
            Ui.grid = this.UiGrid;
            Ui.whiteImageGrid = this.whiteCapturedGrid;
            Ui.blackImageGrid = this.blackCapturedGrid;
            Ui.whiteScoreLabel = this.whiteCapturedLabel;
            Ui.blackScoreLabel = this.blackCapturedLabel;
            Ui.dataGrid = this.dataGrid1;
            Ui.whiteImageVB = this.whiteCapturedVB;
            Ui.blackImageVB = this.blackCapturedVB;
            Ui.blackTimer = this.blackTimerTB;
            Ui.whiteTimer = this.whiteTimerTB;
            Ui.whiteTimerVB = this.whiteTimerVB;
            Ui.blackTimerVB = this.blackTimerVB;

            //Ui.moveListBox = moveListBox;

            //drawBoard(board,grid);
            Ui.drawBoardGrid(board, Ui.canvas);
            Ui.drawUi(board, Ui.canvas);
            switchPerspectiveBtn.Click += SpBtnClick;
            ResignBtn.Click += ResignBtnClick;
            prevMoveBtn.Click += prevMoveBtnClick;
            nextMoveBtn.Click += nextMoveBtnClick;
            //create a timer with a one second interval
            UiUpdatetimer = new Timer(100);
            Game.blackStartTime = DateTime.Now;
            Game.whiteStartTime = DateTime.Now;
            UiUpdatetimer.Elapsed += blackTimeEvent;
            UiUpdatetimer.AutoReset = true;
            UiUpdatetimer.Enabled = true;


            Ui.initializeUpgradeButtons();
            //initialise hoveredTileImage
            Ui.initializeHoveredTileImage();

        }


        private void nextMoveBtnClick(object sender, RoutedEventArgs e)
        {
            if (Ui.moveBeingDisplayed < Game.boardStates.Count - 1)
            {
                Game.activeBoard = Game.boardStates[Ui.moveBeingDisplayed + 1];
                Ui.moveBeingDisplayed++;
                Ui.drawUi(Game.activeBoard, Ui.canvas);
            }
            else
            {
                Console.WriteLine("Already displaying current Move");
            }
        }
        private void prevMoveBtnClick(object sender, RoutedEventArgs e)
        {
            if (Ui.moveBeingDisplayed > 0)
            {
                Game.activeBoard = Game.boardStates[Ui.moveBeingDisplayed - 1];
                Ui.moveBeingDisplayed--;
                Ui.drawUi(Game.activeBoard, Ui.canvas);
            }
            else
            {
                Console.WriteLine("Cant show previous move, already in starting position");
            }

        }

        private void ResignBtnClick(object sender, RoutedEventArgs e)
        {
            Game.activeBoard = new Board();
            Game.activeBoard.generateBoardFromFEN();
            Game.activeBoard.setIsWhiteTurn(true);
            Game.activeBoard.setValidMoves(Game.activeBoard.generateValidMoves(Game.activeBoard));
            Ui.drawUi(Game.activeBoard, Ui.canvas);
        }

        private void blackTimeEvent(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (Game.whitesTurn)
                {

                    TimeSpan whiteTimeElapsed = (DateTime.Now - Game.whiteStartTime);

                    if (whiteTimeElapsed > Game.whiteTimeLeft)
                    {
                        Console.WriteLine("Game Over");
                        UiUpdatetimer.Stop();
                    }

                    Ui.whiteTimer.Text = (Game.whiteTimeLeft - whiteTimeElapsed).ToString(@"%m\:ss");
                }
                else
                {
                    TimeSpan blackTimeElapsed = (DateTime.Now - Game.blackStartTime);

                    if (blackTimeElapsed > Game.blackTimeLeft)
                    {
                        Console.WriteLine("Game Over");
                        UiUpdatetimer.Stop();
                    }

                    Ui.blackTimer.Text = (Game.blackTimeLeft - blackTimeElapsed).ToString(@"%m\:ss");
                }
            }));
        }

        private static void SpBtnClick(object sender, RoutedEventArgs e)
        {
            Ui.switchPerspective();
            Ui.upgradeChoiceRequired = false;
            Ui.imageSelected = null;
            Ui.drawBoardGrid(Game.activeBoard, Ui.canvas);
            Ui.drawUi(Game.activeBoard, Ui.canvas);

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Image imageSelected = Ui.imageSelected;

            if (imageSelected != null)
            {
                Point point2 = e.GetPosition(imageSelected);

                Canvas.SetTop(imageSelected, (int)((Canvas.GetTop(imageSelected)) + point2.Y - MoveableImage.point.Y));
                Canvas.SetLeft(imageSelected, (int)((Canvas.GetLeft(imageSelected)) + point2.X - MoveableImage.point.X));
                Ui.drawHoveredTileImage();

            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            Ui.removeHoveredTileImage();

            if (Ui.imageSelected != null)
            {
                resetImage();
            }
        }

        public void resetImage()
        {
            if (Ui.imageSelected != null)
            {
                Ui.removeValidMovesImages();
                Canvas.SetTop(Ui.imageSelected, Ui.resetPositionY);
                Canvas.SetLeft(Ui.imageSelected, Ui.resetPositionX);

                Ui.pieceSelected = null;
                Ui.imageSelected = null;

            }
        }

    }
}
