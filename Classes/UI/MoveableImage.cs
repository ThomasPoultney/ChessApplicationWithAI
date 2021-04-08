﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Timers;
using System.Collections.Generic;
using System.Linq;

namespace ChessB
{
    public class MoveableImage : Image
    {

        public static Point point;
        public String name;
        static int pieceStartLocation;
        public bool isCapturedPiece;
        private static List<Board> boardStates = new List<Board>();

        public MoveableImage()
        {
        }

        //Destructor.
        ~MoveableImage()
        {
        }

        public bool getIsCapturedPiece()
        {
            return this.isCapturedPiece;
        }

        public void setIsCapturedPiece(bool isCaptured)
        {
            this.isCapturedPiece = isCaptured;
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (Ui.imageSelected == null && this.getIsCapturedPiece() == false && Ui.upgradeChoiceRequired == false)
                {

                    Ui.imageSelected = this;
                    point = e.GetPosition(Ui.imageSelected);
                    Ui.resetPositionY = Canvas.GetTop(Ui.imageSelected);
                    Ui.resetPositionX = Canvas.GetLeft(Ui.imageSelected);

                    int xlocation = (int)(Canvas.GetLeft(Ui.imageSelected) / Ui.squareSize);
                    int ylocation = Board.boardSize - 1 - (int)(Canvas.GetTop(Ui.imageSelected) / Ui.squareSize);
                    pieceStartLocation = Board.boardSize * ylocation + xlocation;
                    Ui.pieceSelected = Game.activeBoard.getPiece()[pieceStartLocation];
                    if (Ui.pieceSelected != null)
                    {
                        if (Ui.pieceSelected.getIsWhite() != Game.activeBoard.getIsWhiteTurn())
                        {
                            Ui.imageSelected = null;
                            return;
                        }
                    }

                    Board activeBoard = Game.activeBoard;
                    Piece pieceSelected = Ui.pieceSelected;
                    Piece[] pieceArray = activeBoard.getPiece();
                    if (Ui.pieceSelected.GetType() == typeof(King))
                    {
                        Game.validMoves = activeBoard.removeMovesThatPutInCheck(pieceSelected.generateValidMoves(activeBoard, pieceArray, Ui.pieceSelected.getLocation(),
                                                                                activeBoard.generateBlackAttackingMoves(pieceArray),
                                                                                activeBoard.generateWhiteAttackingMoves(pieceArray)));
                    }
                    else
                    {
                        Game.validMoves = activeBoard.removeMovesThatPutInCheck(pieceSelected.generateValidMoves(activeBoard, pieceArray, Ui.pieceSelected.getLocation()));
                    }

                    Ui.drawValidMoves();

                }
                else
                {
                    return;
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Ui.removeHoveredTileImage();
            if (Ui.imageSelected != null)
            {
                double topPosition = Math.Round(Canvas.GetTop(Ui.imageSelected) / Ui.squareSize) * Ui.squareSize;
                double leftPosition = Math.Round(Canvas.GetLeft(Ui.imageSelected) / Ui.squareSize) * Ui.squareSize;
                Canvas.SetTop(Ui.imageSelected, topPosition);
                Canvas.SetLeft(Ui.imageSelected, leftPosition);

                int endXlocation = (int)(leftPosition / Ui.squareSize);
                int endYlocation = Board.boardSize - 1 - (int)(topPosition / Ui.squareSize);
                int pieceEndLocation = Board.boardSize * endYlocation + endXlocation;
                Move move = new Move(pieceStartLocation, pieceEndLocation, Game.activeBoard.getPiece()[pieceStartLocation]);
                int boardSize = Game.activeBoard.getBoardSize();
                Board boardAfterMove = Game.activeBoard.makeMoveOnNewBoard(move);

                if (boardAfterMove == null)
                {
                    resetImage();
                }
                else
                {

                    if (Ui.pieceSelected is ChessB.Pawn)
                    {
                        if (pieceEndLocation < boardSize || pieceEndLocation >= boardSize * (boardSize - 1))
                        {
                            if (move.getPiece().getIsWhite() == true)
                            {
                                Canvas.SetTop(Ui.queenUpgradeButton, topPosition);
                                Canvas.SetLeft(Ui.queenUpgradeButton, leftPosition);
                                Canvas.SetZIndex(Ui.queenUpgradeButton, 150000);
                                Ui.canvas.Children.Add(Ui.queenUpgradeButton);


                                Canvas.SetTop(Ui.rookUpgradeButton, topPosition + Ui.squareSize);
                                Canvas.SetLeft(Ui.rookUpgradeButton, leftPosition);
                                Canvas.SetZIndex(Ui.rookUpgradeButton, 150000);
                                Ui.canvas.Children.Add(Ui.rookUpgradeButton);

                                Canvas.SetTop(Ui.bishopUpgradeButton, topPosition + 2 * Ui.squareSize);
                                Canvas.SetLeft(Ui.bishopUpgradeButton, leftPosition);
                                Canvas.SetZIndex(Ui.bishopUpgradeButton, 150000);
                                Ui.canvas.Children.Add(Ui.bishopUpgradeButton);

                                Canvas.SetTop(Ui.knightUpgradeButton, topPosition + 3 * Ui.squareSize);
                                Canvas.SetLeft(Ui.knightUpgradeButton, leftPosition);
                                Canvas.SetZIndex(Ui.knightUpgradeButton, 150000);
                                Ui.canvas.Children.Add(Ui.knightUpgradeButton);

                                Canvas.SetTop(Ui.upgradeCancelButton, topPosition + 4 * Ui.squareSize);
                                Canvas.SetLeft(Ui.upgradeCancelButton, leftPosition);
                                Canvas.SetZIndex(Ui.upgradeCancelButton, 150000);
                                Ui.canvas.Children.Add(Ui.upgradeCancelButton);
                                Ui.upgradeChoiceRequired = true;

                            }
                            else
                            {


                                Canvas.SetTop(Ui.queenUpgradeButton, topPosition);
                                Canvas.SetLeft(Ui.queenUpgradeButton, leftPosition);
                                Canvas.SetZIndex(Ui.queenUpgradeButton, 150000);
                                Ui.canvas.Children.Add(Ui.queenUpgradeButton);


                                Canvas.SetTop(Ui.rookUpgradeButton, topPosition - Ui.squareSize);
                                Canvas.SetLeft(Ui.rookUpgradeButton, leftPosition);
                                Canvas.SetZIndex(Ui.rookUpgradeButton, 150000);
                                Ui.canvas.Children.Add(Ui.rookUpgradeButton);

                                Canvas.SetTop(Ui.bishopUpgradeButton, topPosition - 2 * Ui.squareSize);
                                Canvas.SetLeft(Ui.bishopUpgradeButton, leftPosition);
                                Canvas.SetZIndex(Ui.bishopUpgradeButton, 150000);
                                Ui.canvas.Children.Add(Ui.bishopUpgradeButton);

                                Canvas.SetTop(Ui.knightUpgradeButton, topPosition - 3 * Ui.squareSize);
                                Canvas.SetLeft(Ui.knightUpgradeButton, leftPosition);
                                Canvas.SetZIndex(Ui.knightUpgradeButton, 150000);
                                Ui.canvas.Children.Add(Ui.knightUpgradeButton);

                                Canvas.SetTop(Ui.upgradeCancelButton, topPosition - 4 * Ui.squareSize);
                                Canvas.SetLeft(Ui.upgradeCancelButton, leftPosition);
                                Canvas.SetZIndex(Ui.upgradeCancelButton, 150000);
                                Ui.canvas.Children.Add(Ui.upgradeCancelButton);
                                Ui.upgradeChoiceRequired = true;

                            }
                        }
                    }
                    Ui.upgradeMove = move;

                    if (Ui.upgradeChoiceRequired == false)
                    {
                        Game.activeBoard = boardAfterMove;
                        //Console.WriteLine(boardAfterMove.moveGenerationTest(3));
                        Console.WriteLine(boardAfterMove.getIsWhiteTurn() + " " + boardAfterMove.getValidMoves().Count);
                        boardStates.Add(boardAfterMove);
                        Ui.drawUi(boardAfterMove, Ui.canvas);
                    }
                }
            }
            Ui.pieceSelected = null;
            Ui.imageSelected = null;
            Ui.removeValidMovesImages();
        }




        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            Ui.removeHoveredTileImage();
            if (Ui.imageSelected != null)
            {

                resetImage();
            }
            else
            {
                if (Ui.tileSelected == null)
                {
                    Ui.tileSelected = this;
                    Ui.ArrowStartingPosition = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                }
            }


        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (Ui.tileSelected != null)
            {
                Ui.drawArrow(this);
                Ui.tileSelected = null;
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
                pieceStartLocation = 0;

            }
        }

        public void MoveImage(Image image, int location, Board board)
        {
            int xLocation = (location) % ((board.getBoardSize()));
            int yLocation = (int)(location / board.getBoardSize());
            Canvas.SetTop(image, xLocation);
            Canvas.SetLeft(image, yLocation);
        }
    }
}