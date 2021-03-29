﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessB
{
    public class Board
    {
        /* Forsyth–Edwards Notation(FEN) describing the current state of the board. 
        By default the starting position of the board */
        private String fenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        private bool isWhiteTurn = true;

        private int blackKingLocation;
        private int whiteKingLocation;

        private bool blackCanCastle = true;
        private bool whiteCanCastle = true;

        /*This is the number of halfmoves since the last capture or pawn advance. 
        The field is used in the fifty-move rule.*/
        private int fiftyMoveRule = 50;

        private int moveNumber = 0;
        //The number of the full move. It starts at 1, and is incremented after Black's move.
        private int fullMoveNumber = 1;

        private int enPassantLocation;

        //represents current state of board
        private Piece[] piece = new Piece[boardSize * boardSize];

        //stores each move that is made
        public List<Move> moves = new List<Move>();

        //stores all tiles being attacked by a player, used for checking for checkmate. 
        private List<int> whiteAttacking = new List<int>();
        private List<int> blackAttacking = new List<int>();

        public static int boardSize = 8;

        public Board()
        {
            piece = new Piece[boardSize * boardSize];

        }

        private List<Move> Moves { get => moves; set => moves = value; }

        public Piece[] getPiece()
        {
            return this.piece;
        }

        public int getWhiteKingLocation()
        {
            return this.whiteKingLocation;
        }

        public int getBlackKingLocation()
        {
            return this.blackKingLocation;
        }

        public void setPiece(Piece[] piece)
        {
            this.piece = piece;
        }

        public void setEnPassantLocation(int location)
        {
            this.enPassantLocation = location;
        }

        public int getEnPassantLocation()
        {
            return this.enPassantLocation;
        }

        public void printBoard()
        {
            Console.WriteLine(this.getPiece().Length);
            for (int i = 0; i < (boardSize * boardSize); i++)
            {
                if (this.getPiece()[i] == null)
                {
                    Console.Write('N');
                }
                else
                {
                    Console.Write(this.getPiece()[i].getLetterRepresentation());
                }

                if (i % boardSize == boardSize - 1)
                {
                    Console.Write("\n");
                }
            }
        }




        public void setPieceAtLocation(int location, Piece piece)
        {
            if (piece != null)
            {
                piece.setLocation(location);
            }

            this.piece[location] = piece;
        }

        public List<Move> getMoves()
        {
            return this.moves;
        }


        public List<int> getBlackAttackingMoves()
        {
            return this.blackAttacking;
        }

        public List<int> getWhiteAttackingMoves()
        {
            return this.whiteAttacking;
        }

        public int getBoardSize()
        {
            return boardSize;
        }

        public bool getIsWhiteTurn()
        {
            return isWhiteTurn;
        }

        public void setIsWhiteTurn(bool isWhiteTurn)
        {
            this.isWhiteTurn = isWhiteTurn;
        }

        //Checks if board state after the move would leave you in checks, if it does it is removed
        public List<Move> removeMovesThatPutInCheck(List<Move> validMoves)
        {
            List<Move> finalvalidMoves = new List<Move>();

            foreach (Move validMove in validMoves)
            {
                Piece[] boardStateAfterMove = this.getPiece().ToArray();
                int pieceEndLocation = validMove.getEndLocation();
                int pieceStartLocation = validMove.getStartLocation();
                Piece piece = validMove.getPiece();

                boardStateAfterMove[pieceEndLocation] = piece;
                boardStateAfterMove[pieceStartLocation] = null;
                if (validMove.getTag() == "CastleShort")
                {
                    Piece rook = validMove.getSecondaryPiece();
                    boardStateAfterMove[rook.getLocation()] = null;
                    boardStateAfterMove[validMove.getPiece().getLocation() - 1] = rook;
                }

                if (validMove.getTag() == "CastleLong")
                {
                    Piece rook = validMove.getSecondaryPiece();
                    boardStateAfterMove[rook.getLocation()] = null;
                    boardStateAfterMove[validMove.getPiece().getLocation() + 1] = rook;
                }

                int bKingLocation = this.blackKingLocation;
                int wKingLocation = this.whiteKingLocation;

                if (piece is ChessB.King)
                {
                    if (piece.getIsWhite() == true)
                    {
                        wKingLocation = pieceEndLocation;
                    }
                    else
                    {
                        bKingLocation = pieceEndLocation;
                    }
                }

                List<int> attackingMoves;

                if (piece.getIsWhite() == true)
                {
                    attackingMoves = generateBlackAttackingMoves(boardStateAfterMove);
                    if (!attackingMoves.Contains(wKingLocation))
                    {
                        finalvalidMoves.Add(validMove);
                    }
                }
                else
                {
                    attackingMoves = generateWhiteAttackingMoves(boardStateAfterMove);
                    if (!attackingMoves.Contains(bKingLocation))
                    {
                        finalvalidMoves.Add(validMove);
                    }
                }
            }

            return finalvalidMoves;
        }

        public bool makeMove(Move move)
        {
            string chessNotation = "";
            setEnPassantLocation(-50);
            Ui.removeArrows();
            Ui.removeMarkedTiles();
            int pieceEndLocation = move.getEndLocation();
            int pieceStartLocation = move.getStartLocation();
            Piece piece = move.getPiece();
            String tag = move.getTag();
            Piece secondaryPiece = null;
            Ui.removeHighlightTile();

            bool moveIsValid = false;
            foreach (Move validmove in Game.validMoves)
            {
                if (validmove.getStartLocation() == move.getStartLocation() & validmove.getEndLocation() == move.getEndLocation() & validmove.getPiece() == move.getPiece())
                {
                    moveIsValid = true;
                    tag = validmove.getTag();
                    //Console.WriteLine(tag);
                    secondaryPiece = validmove.getSecondaryPiece();
                }
            }

            if (moveIsValid == false)
            {
                Console.WriteLine("Invalid Move");
                return false;
            }
            else
            {
                if (!(move.getPiece() is Pawn))
                {
                    chessNotation += move.getPiece().getLetterRepresentation();

                }
                int endLocation = move.getEndLocation();
                int endXLocation = (endLocation) % ((this.getBoardSize())); ;
                int endYLocation = boardSize - 1 - (int)(endLocation / this.getBoardSize());
                Canvas.SetTop(move.getPiece().getImage(), endYLocation * Ui.squareSize);




                Canvas.SetLeft(move.getPiece().getImage(), endXLocation * Ui.squareSize);


                if (this.getPiece()[pieceEndLocation] != null)
                {
                    Ui.canvas.Children.Remove(Game.activeBoard.getPiece()[pieceEndLocation].getImage());
                    chessNotation += "x";

                    if (this.getPiece()[pieceEndLocation].getIsWhite())
                    {
                        MoveableImage image = Game.activeBoard.getPiece()[pieceEndLocation].getImage();
                        Ui.addBlackCaptureImage(image, this.getPiece()[pieceEndLocation].getStrength());
                    }
                    else
                    {
                        MoveableImage image = Game.activeBoard.getPiece()[pieceEndLocation].getImage();
                        Ui.addWhiteCaptureImage(image, this.getPiece()[pieceEndLocation].getStrength());
                    }

                    this.getPiece()[pieceEndLocation] = null;

                }


                if (piece is ChessB.Pawn)
                {
                    piece.setCanMoveTwice(false);

                    //set enpassant squares
                    if (tag == "enPassant")
                    {
                        chessNotation += GetColumnName(pieceStartLocation % boardSize);
                        chessNotation += "x";
                        Console.WriteLine("EnPassant");
                        if (pieceStartLocation < pieceEndLocation)
                        {
                            Piece capturedPawn = getPiece()[endLocation - boardSize];
                            Ui.canvas.Children.Remove(capturedPawn.getImage());
                            if (capturedPawn.getIsWhite() == true)
                            {
                                Ui.addBlackCaptureImage(capturedPawn.getImage(), capturedPawn.getStrength());

                            }
                            else
                            {
                                Ui.addWhiteCaptureImage(capturedPawn.getImage(), capturedPawn.getStrength());

                            }

                            setPieceAtLocation(endLocation - boardSize, null);

                        }
                        else
                        {
                            Piece capturedPawn = getPiece()[endLocation + boardSize];
                            Ui.canvas.Children.Remove(capturedPawn.getImage());
                            if (capturedPawn.getIsWhite() == true)
                            {
                                Ui.addBlackCaptureImage(capturedPawn.getImage(), capturedPawn.getStrength());
                            }
                            else
                            {
                                Ui.addWhiteCaptureImage(capturedPawn.getImage(), capturedPawn.getStrength());
                            }

                            setPieceAtLocation(endLocation + boardSize, null);
                        }
                    }
                    else
                    {
                        if (move.getStartLocation() - move.getEndLocation() == -(this.getBoardSize() * 2))
                        {
                            setEnPassantLocation(move.getStartLocation() + this.getBoardSize());
                        }

                        if (move.getStartLocation() - move.getEndLocation() == (this.getBoardSize() * 2))
                        {
                            setEnPassantLocation(move.getStartLocation() - this.getBoardSize());
                        }
                    }

                }
                chessNotation += GetColumnName(endXLocation).ToLower();
                chessNotation += (boardSize - endYLocation).ToString();
            }


            if (piece is ChessB.King)
            {
                if (piece.getIsWhite() == true)
                {
                    this.whiteKingLocation = pieceEndLocation;
                    piece.setHasMoved(true);
                    this.whiteCanCastle = false;
                }
                else
                {
                    this.blackKingLocation = pieceEndLocation;
                    piece.setHasMoved(true);
                    this.blackCanCastle = false;
                }
            }



            if (piece is ChessB.Rook)
            {
                piece.setHasMoved(true);
            }

            this.setPieceAtLocation(pieceEndLocation, piece);
            this.setPieceAtLocation(pieceStartLocation, null);



            if (tag == "CastleShort")
            {
                Piece rook = move.getSecondaryPiece();
                int location = move.getPiece().getLocation() - 1;
                int xLocationForRook = (location) % ((this.getBoardSize())); ;
                int yLocationForRook = boardSize - 1 - (int)(location / this.getBoardSize()); ;
                Canvas.SetTop(secondaryPiece.getImage(), yLocationForRook * Ui.squareSize);
                Canvas.SetLeft(secondaryPiece.getImage(), xLocationForRook * Ui.squareSize);
                this.setPieceAtLocation(secondaryPiece.getLocation(), null);
                this.setPieceAtLocation(move.getPiece().getLocation() - 1, secondaryPiece);
                if (move.getPiece().getIsWhite() == true)
                {
                    this.whiteCanCastle = false;
                }
                else
                {
                    this.blackCanCastle = false;
                }
                chessNotation = "O-O";
            }

            if (tag == "CastleLong")
            {
                Piece rook = move.getSecondaryPiece();
                int location = move.getPiece().getLocation() + 1;
                int xLocationForRook = (location) % ((this.getBoardSize())); ;
                int yLocationForRook = boardSize - 1 - (int)(location / this.getBoardSize()); ;
                Canvas.SetTop(secondaryPiece.getImage(), yLocationForRook * Ui.squareSize);
                Canvas.SetLeft(secondaryPiece.getImage(), xLocationForRook * Ui.squareSize);
                this.setPieceAtLocation(secondaryPiece.getLocation(), null);
                this.setPieceAtLocation(move.getPiece().getLocation() + 1, secondaryPiece);

                if (move.getPiece().getIsWhite() == true)
                {
                    this.whiteCanCastle = false;
                }
                else
                {
                    this.blackCanCastle = false;
                }

                chessNotation = "O-O-O";
            }

            if (move.getTag() == "promoteToRook")
            {
                Ui.canvas.Children.Remove(move.getPiece().getImage());
                int location = move.getPiece().getLocation();
                int xLocation = (location) % ((this.getBoardSize())); ;
                int yLocation = boardSize - 1 - (int)(location / this.getBoardSize());
                Rook promotedPiece = new Rook(move.getPiece().getIsWhite(), move.getEndLocation());
                MoveableImage promotedImage = new MoveableImage();
                promotedPiece.setImage(promotedImage);
                String promotedImageURL = promotedPiece.getImagePath();

                ImageSource promotedPieceSource = new BitmapImage(new Uri(promotedImageURL));
                promotedImage.Source = promotedPieceSource;
                promotedImage.Width = Ui.squareSize;
                promotedImage.Height = Ui.squareSize;
                setPieceAtLocation(move.getEndLocation(), promotedPiece);

                Canvas.SetTop(promotedImage, yLocation * Ui.squareSize);
                Canvas.SetLeft(promotedImage, xLocation * Ui.squareSize);
                Canvas.SetZIndex(promotedImage, 1100);
                if (move.getPiece().getIsWhite() == true)
                {
                    Ui.addWhiteScore(4);
                }
                else
                {
                    Ui.addBlackScore(4);
                }
                Ui.canvas.Children.Add(promotedImage);
                chessNotation += "R";
            }
            else if (move.getTag() == "promoteToBishop")
            {
                Ui.canvas.Children.Remove(move.getPiece().getImage());
                int location = move.getPiece().getLocation();
                int xLocation = (location) % ((this.getBoardSize())); ;
                int yLocation = boardSize - 1 - (int)(location / this.getBoardSize());
                Bishop promotedPiece = new Bishop(move.getPiece().getIsWhite(), move.getEndLocation());

                MoveableImage promotedImage = new MoveableImage();
                promotedPiece.setImage(promotedImage);
                String promotedImageURL = promotedPiece.getImagePath();

                ImageSource promotedPieceSource = new BitmapImage(new Uri(promotedImageURL));
                promotedImage.Source = promotedPieceSource;
                promotedImage.Width = Ui.squareSize;
                promotedImage.Height = Ui.squareSize;
                setPieceAtLocation(move.getEndLocation(), promotedPiece);

                Canvas.SetTop(promotedImage, yLocation * Ui.squareSize);
                Canvas.SetLeft(promotedImage, xLocation * Ui.squareSize);
                Canvas.SetZIndex(promotedImage, 1100);
                Ui.canvas.Children.Add(promotedImage);

                if (move.getPiece().getIsWhite() == true)
                {
                    Ui.addWhiteScore(2);
                }
                else
                {
                    Ui.addBlackScore(2);
                }

                chessNotation += "B";
            }
            else if (move.getTag() == "promoteToQueen")
            {
                Ui.canvas.Children.Remove(move.getPiece().getImage());
                int location = move.getPiece().getLocation();
                int xLocation = (location) % ((this.getBoardSize())); ;
                int yLocation = boardSize - 1 - (int)(location / this.getBoardSize());
                Queen promotedPiece = new Queen(move.getPiece().getIsWhite(), move.getEndLocation());
                MoveableImage promotedImage = new MoveableImage();
                promotedPiece.setImage(promotedImage);
                String promotedImageURL = promotedPiece.getImagePath();


                ImageSource promotedPieceSource = new BitmapImage(new Uri(promotedImageURL));
                promotedImage.Source = promotedPieceSource;
                promotedImage.Width = Ui.squareSize;
                promotedImage.Height = Ui.squareSize;
                setPieceAtLocation(move.getEndLocation(), promotedPiece);

                Canvas.SetTop(promotedImage, yLocation * Ui.squareSize);
                Canvas.SetLeft(promotedImage, xLocation * Ui.squareSize);
                Canvas.SetZIndex(promotedImage, 1100);

                Ui.canvas.Children.Add(promotedImage);
                if (move.getPiece().getIsWhite() == true)
                {
                    Ui.addWhiteScore(7);
                }
                else
                {
                    Ui.addBlackScore(7);
                }

                chessNotation += "Q";

            }
            else if (move.getTag() == "promoteToKnight")
            {
                Ui.canvas.Children.Remove(move.getPiece().getImage());
                int location = move.getPiece().getLocation();
                int xLocation = (location) % ((this.getBoardSize())); ;
                int yLocation = boardSize - 1 - (int)(location / this.getBoardSize());
                Knight promotedPiece = new Knight(move.getPiece().getIsWhite(), move.getEndLocation());
                MoveableImage promotedImage = new MoveableImage();
                promotedPiece.setImage(promotedImage);
                String promotedImageURL = promotedPiece.getImagePath();

                ImageSource promotedPieceSource = new BitmapImage(new Uri(promotedImageURL));
                promotedImage.Source = promotedPieceSource;
                promotedImage.Width = Ui.squareSize;
                promotedImage.Height = Ui.squareSize;
                setPieceAtLocation(move.getEndLocation(), promotedPiece);

                Canvas.SetTop(promotedImage, yLocation * Ui.squareSize);
                Canvas.SetLeft(promotedImage, xLocation * Ui.squareSize);
                Canvas.SetZIndex(promotedImage, 1100);
                Ui.canvas.Children.Add(promotedImage);

                if (move.getPiece().getIsWhite() == true)
                {
                    Ui.addWhiteScore(2);
                }
                else
                {
                    Ui.addBlackScore(2);
                }

                chessNotation += "N";
            }



            this.moveNumber++;
            int moveCount = moveNumber;
            Console.WriteLine(moveNumber);
            this.setIsWhiteTurn(!this.getIsWhiteTurn());
            this.moves.Add(move);
            Ui.drawHighlightTile(pieceStartLocation);
            Ui.drawHighlightTile(pieceEndLocation);

            bool isEnPassant = false;
            if (tag == "enPassant")
            {
                isEnPassant = true;
            }

            this.setUpNextTurn(move, chessNotation, isEnPassant);

            return true;

        }

        private void setUpNextTurn(Move move, string chessNotation, bool isEnPassant)
        {
            Ui.removeCheckTile();
            bool blackWins = false;
            bool whiteWins = false;
            bool draw = false;
            bool check = false;
            bool staleMate = false;
            //reset enpassant square
            //generate all current players moves
            //generate all opponenets attacking moves, check if we are in check// if in check play check sounds
            //if current player has no valid moves and we are in check then oppenent wins
            //if current player has valid moves and not in check it is stalemate
            List<Move> validMovesAfterCheck = new List<Move>();


            if (this.isWhiteTurn == true)
            {
                blackAttacking = this.generateBlackAttackingMoves(this.getPiece());
                if (blackAttacking.Contains(whiteKingLocation))
                {
                    check = true;
                    Ui.drawCheckTile(whiteKingLocation);
                }
            }
            else
            {
                whiteAttacking = this.generateWhiteAttackingMoves(this.getPiece());
                if (whiteAttacking.Contains(blackKingLocation))
                {
                    check = true;
                    Ui.drawCheckTile(blackKingLocation);
                }
            }
            //genearates all valid moves for each piece
            foreach (Piece piece in this.getPiece())
            {
                if (piece != null)
                {
                    if (piece.getIsWhite() == this.isWhiteTurn)
                    {
                        if (piece is ChessB.King)
                        {
                            validMovesAfterCheck.AddRange(removeMovesThatPutInCheck(piece.generateValidMoves(this, this.getPiece(), this.getBlackAttackingMoves(), this.getWhiteAttackingMoves())));
                        }
                        else
                        {
                            validMovesAfterCheck.AddRange(removeMovesThatPutInCheck(piece.generateValidMoves(this, this.getPiece())));
                        }
                    }
                }

            }

            //removes moves that would leave king in check.

            if (validMovesAfterCheck.Count == 0)
            {
                if (this.isWhiteTurn == true)
                {
                    if (blackAttacking.Contains(whiteKingLocation))
                    {
                        blackWins = true;
                        Console.WriteLine("Black Wins");
                    }
                    else
                    {
                        staleMate = true;
                        Console.WriteLine("Stalemate");
                    }
                }
                else
                {
                    if (whiteAttacking.Contains(blackKingLocation))
                    {
                        whiteWins = true;
                        Console.WriteLine("White Wins");
                    }
                    else
                    {
                        staleMate = true;
                        Console.WriteLine("Stalemate");
                    }
                }

            }

            if (isEnPassant)
            {
                chessNotation += " e.p.";
            }

            if (blackWins == true || whiteWins == true)
            {
                chessNotation += "#";
            }
            else if (staleMate == true)
            {
                chessNotation += "=";
            }
            else if (check == true)
            {
                chessNotation += "+";
            }

            move.setChessNotation(chessNotation);
            Ui.moveListBox.Items.Add(moveNumber + "\t" + chessNotation);

            if (draw == true)
            {
                Ui.moveListBox.Items.Add("1/2 - 1/2");
                return;
            }
            else if (whiteWins == true)
            {
                Ui.moveListBox.Items.Add("1 - 0");
                return;
            }
            else if (blackWins == true)
            {
                Ui.moveListBox.Items.Add("0 - 1");
                return;

            }

            if (this.getIsWhiteTurn() == false)
            {
                makeRandomMove();
            }


        }

        public void makeRandomMove()
        {
            whiteAttacking = generateWhiteAttackingMoves(this.getPiece());
            blackAttacking = generateBlackAttackingMoves(this.getPiece());
            List<Move> validMovesAfterCheck = new List<Move>();
            foreach (Piece piece in this.getPiece())
            {
                if (piece != null)
                {
                    if (piece.getIsWhite() == this.isWhiteTurn)
                    {
                        if (piece is ChessB.King)
                        {
                            validMovesAfterCheck.AddRange(removeMovesThatPutInCheck(piece.generateValidMoves(this, this.getPiece(), this.getBlackAttackingMoves(), this.getWhiteAttackingMoves())));
                        }
                        else
                        {
                            validMovesAfterCheck.AddRange(removeMovesThatPutInCheck(piece.generateValidMoves(this, this.getPiece())));
                        }
                    }
                }
            }
            var random = new Random();
            int randomIndex = random.Next(validMovesAfterCheck.Count - 1);
            Game.validMoves = validMovesAfterCheck;
            makeMove(validMovesAfterCheck[randomIndex]);

        }

        public List<int> generateWhiteAttackingMoves(Piece[] pieceArray)
        {
            List<Move> whiteAttackingMoves = new List<Move>();

            foreach (Piece piece in pieceArray)
            {
                if (piece != null)
                {

                    if (piece.GetType() != typeof(Pawn) && piece.GetType() != typeof(King))
                    {
                        if (piece.getIsWhite() == true)
                        {
                            whiteAttackingMoves.AddRange(piece.generateValidMoves(this, pieceArray));
                        }

                    }
                    else if (piece.GetType() == typeof(Pawn))
                    {
                        if (piece.getIsWhite() == true)
                        {
                            whiteAttackingMoves.AddRange(piece.generateAttackingMoves(this));
                        }
                    }
                    else if (piece.GetType() == typeof(King))
                    {
                        if (piece.getIsWhite() == true)
                        {
                            whiteAttackingMoves.AddRange(piece.generateValidMoves(this, pieceArray, this.getBlackAttackingMoves(), this.getWhiteAttackingMoves()));
                        }
                    }

                }
            }

            List<int> tilesAttacked = new List<int>();

            foreach (Move move in whiteAttackingMoves)
            {
                tilesAttacked.Add(move.getEndLocation());
            }

            return tilesAttacked.Distinct().ToList();
        }


        public List<int> generateBlackAttackingMoves(Piece[] pieceArray)
        {
            List<Move> blackAttackingMoves = new List<Move>();

            foreach (Piece piece in pieceArray)
            {
                if (piece != null)
                {

                    if (piece.GetType() != typeof(Pawn) && piece.GetType() != typeof(King))
                    {
                        if (piece.getIsWhite() == false)
                        {
                            blackAttackingMoves.AddRange(piece.generateValidMoves(this, pieceArray));
                        }

                    }
                    else if (piece.GetType() == typeof(Pawn))
                    {
                        if (piece.getIsWhite() == false)
                        {
                            blackAttackingMoves.AddRange(piece.generateAttackingMoves(this));
                        }

                    }
                    else if (piece.GetType() == typeof(King))
                    {
                        if (piece.getIsWhite() == false)
                        {
                            blackAttackingMoves.AddRange(piece.generateValidMoves(this, pieceArray, this.getBlackAttackingMoves(), this.getWhiteAttackingMoves()));

                        }
                    }
                }


            }

            List<int> tilesAttacked = new List<int>();

            foreach (Move move in blackAttackingMoves)
            {
                tilesAttacked.Add(move.getEndLocation());
            }

            return tilesAttacked.Distinct().ToList();
        }


        public void generateBoardFromFEN()
        {
            char[] charFenArray = this.fenString.ToCharArray();
            //Fen string starts from top left of board
            int currentFile = 0;
            int currentRank = boardSize - 1;
            //Capital letter = White, lower case = black

            foreach (char symbol in charFenArray)
            {
                int location = currentRank * boardSize + currentFile;
                if (symbol == '/')
                {
                    currentFile = 0;
                    currentRank--;
                }
                else if (symbol == 'R')
                {
                    Rook rook = new Rook(true, location);
                    piece[location] = rook;
                    currentFile++;
                }
                else if (symbol == 'r')
                {
                    Rook rook = new Rook(false, location);
                    piece[location] = rook;
                    currentFile++;
                }
                else if (symbol == 'N')
                {
                    Knight knight = new Knight(true, location);
                    piece[location] = knight;
                    currentFile++;
                }
                else if (symbol == 'n')
                {
                    Knight knight = new Knight(false, location);
                    piece[location] = knight;
                    currentFile++;
                }
                else if (symbol == 'B')
                {
                    Bishop bishop = new Bishop(true, location);
                    piece[location] = bishop;
                    currentFile++;
                }
                else if (symbol == 'b')
                {
                    Bishop bishop = new Bishop(false, location);
                    piece[location] = bishop;
                    currentFile++;
                }

                else if (symbol == 'Q')
                {
                    Queen queen = new Queen(true, location);
                    piece[location] = queen;
                    currentFile++;
                }
                else if (symbol == 'q')
                {
                    Queen queen = new Queen(false, location);
                    piece[location] = queen;
                    currentFile++;
                }
                else if (symbol == 'K')
                {
                    King king = new King(true, location);
                    piece[location] = king;
                    this.whiteKingLocation = location;
                    currentFile++;
                }
                else if (symbol == 'k')
                {
                    King king = new King(false, location);
                    piece[location] = king;
                    this.blackKingLocation = location;
                    currentFile++;
                }
                else if (symbol == 'P')
                {
                    Pawn pawn = new Pawn(true, location);
                    piece[location] = pawn;
                    currentFile++;
                }
                else if (symbol == 'p')
                {
                    Pawn pawn = new Pawn(false, location);
                    pawn.setMovingUp(false);
                    piece[location] = pawn;
                    currentFile++;
                }
                else if (char.IsNumber(symbol))
                {
                    currentFile += (int)symbol;
                }
                else if (symbol == 'w')
                {
                    isWhiteTurn = true;
                    return;
                }
                else if (symbol == 'b')
                {
                    isWhiteTurn = false;
                    return;
                }
            }

            this.blackAttacking = generateBlackAttackingMoves(piece);
            this.whiteAttacking = generateWhiteAttackingMoves(piece);
        }

        private static string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];

            return value;
        }
    }
}
