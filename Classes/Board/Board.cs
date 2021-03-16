﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            int pieceEndLocation = move.getEndLocation();
            int pieceStartLocation = move.getStartLocation();
            Piece piece = move.getPiece();
            String tag = move.getTag();
            Piece secondaryPiece = null;

            bool moveIsValid = false;
            foreach (Move validmove in Game.validMoves)
            {
                if (validmove.getStartLocation() == move.getStartLocation() & validmove.getEndLocation() == move.getEndLocation() & validmove.getPiece() == move.getPiece())
                {
                    moveIsValid = true;
                    tag = validmove.getTag();
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

                if (this.getPiece()[pieceEndLocation] != null)
                {
                    Ui.canvas.Children.Remove(Game.activeBoard.getPiece()[pieceEndLocation].getImage());

                    if (this.getPiece()[pieceEndLocation].getIsWhite())
                    {
                        Image image = Game.activeBoard.getPiece()[pieceEndLocation].getImage();
                        Ui.addBlackCaptureImage(image, this.getPiece()[pieceEndLocation].getStrength());
                    }
                    else
                    {
                        Image image = Game.activeBoard.getPiece()[pieceEndLocation].getImage();
                        Ui.addWhiteCaptureImage(image, this.getPiece()[pieceEndLocation].getStrength());
                    }

                    this.getPiece()[pieceEndLocation] = null;

                }

                if (piece is ChessB.Pawn)
                {
                    piece.setCanMoveTwice(false);
                }

                if (piece is ChessB.King)
                {
                    if (piece.getIsWhite() == true)
                    {
                        this.whiteKingLocation = pieceEndLocation;
                        piece.setHasMoved(true);
                    }
                    else
                    {
                        this.blackKingLocation = pieceEndLocation;
                        piece.setHasMoved(true);
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
                    Console.WriteLine("CastleShort");
                    Piece rook = move.getSecondaryPiece();
                    int location = move.getPiece().getLocation() - 1;
                    int xLocationForRook = (location) % ((this.getBoardSize())); ;
                    int yLocationForRook = boardSize - 1 - (int)(location / this.getBoardSize()); ;
                    Canvas.SetTop(secondaryPiece.getImage(), yLocationForRook * Ui.squareSize);
                    Canvas.SetLeft(secondaryPiece.getImage(), xLocationForRook * Ui.squareSize);
                    this.setPieceAtLocation(secondaryPiece.getLocation(), null);
                    this.setPieceAtLocation(move.getPiece().getLocation() - 1, secondaryPiece);
                }

                if (tag == "CastleLong")
                {
                    Console.WriteLine("CastleLong");
                    Piece rook = move.getSecondaryPiece();
                    int location = move.getPiece().getLocation() + 1;
                    int xLocationForRook = (location) % ((this.getBoardSize())); ;
                    int yLocationForRook = boardSize - 1 - (int)(location / this.getBoardSize()); ;
                    Canvas.SetTop(secondaryPiece.getImage(), yLocationForRook * Ui.squareSize);
                    Canvas.SetLeft(secondaryPiece.getImage(), xLocationForRook * Ui.squareSize);
                    this.setPieceAtLocation(secondaryPiece.getLocation(), null);
                    this.setPieceAtLocation(move.getPiece().getLocation() + 1, secondaryPiece);

                }
                this.moveNumber++;
                this.setIsWhiteTurn(!this.getIsWhiteTurn());
                this.setUpNextTurn();
                this.moves.Add(move);
                return true;
            }


        }

        private void setUpNextTurn()
        {
            //generate all current players moves
            //generate all openents moves, check if we are in check// if in check play check sounds
            //if current player has no valid moves and we are in check then oppenent wins
            //if current player has valid moves and not in check it is stalemate
            List<Move> validMovesAfterCheck = new List<Move>();
            blackAttacking = this.generateBlackAttackingMoves(this.getPiece());
            whiteAttacking = this.generateWhiteAttackingMoves(this.getPiece());

            Console.WriteLine(this.getIsWhiteTurn());
            //genearates all moves for each piece
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
                        Console.WriteLine("Black Wins");
                    }
                    else
                    {
                        Console.WriteLine("Stalemate");
                    }
                }
                else
                {
                    if (whiteAttacking.Contains(blackKingLocation))
                    {
                        Console.WriteLine("White Wins");
                    }
                    else
                    {
                        Console.WriteLine("Stalemate");
                    }
                }

            }

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
    }
}
