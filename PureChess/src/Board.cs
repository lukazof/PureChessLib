﻿using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PureChess
{
    internal class Board
    {
        public string defaultPosition = "RNBQKBNR/PPPPPPPP/......../......../......../......../pppppppp/rnbqkbnr"; // Better than FEN? Nah..
        public int columns = 8;
        public int rows = 8;

        public List<Square> squares = new List<Square>();
        public void GenerateBoard(string position)
        {
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Square square = new Square();
                    square.x = x;
                    square.y = y;

                    square.index = (columns + rows) / 2 * x + y;

                    squares.Add(square);

                    Game.Instance.settings.DebugMessage($"Generating Square {square.index}");
                }
            }

            Game.Instance.settings.DebugMessage("Board sucessfuly generated");
            if(position == null || position == "" || position == " " || position == "default" || position == "startpos") { position = defaultPosition; }
            LoadPosition(position);


        }

        public void LoadPosition(string positionToLoad)
        {
            string legacyPosition = positionToLoad;

            Game.Instance.settings.DebugMessage($"Loading Position '{positionToLoad}'");
            positionToLoad = positionToLoad.Replace("/", "");

            int i = 0;
            foreach (char c in positionToLoad)
            {
                switch (c)
                {
                    case 'p' or 'P':
                        squares[i].piece.type = "Pawn";
                        break;
                    case 'r' or 'R':
                        squares[i].piece.type = "Rook";
                        break;
                    case 'n' or 'N':
                        squares[i].piece.type = "Knight";
                        break;
                    case 'b' or 'B':
                        squares[i].piece.type = "Bishop";
                        break;
                    case 'q' or 'Q':
                        squares[i].piece.type = "Queen";
                        break;
                    case 'k' or 'K':
                        squares[i].piece.type = "King";
                        break;
                    case '.':
                        squares[i].piece.type = "None";
                        break;
                    default:
                        break;
                }

                if (char.IsUpper(c)) { squares[i].piece.color = 0; }
                else if (char.IsLower(c)) { squares[i].piece.color = 1; }
                else { squares[i].piece.color = 0; }; // No piece at that square (Set to White (0) by default)


                if (squares[i].piece.type != "None")
                {
                    Game.Instance.settings.DebugMessage($"Generating {squares[i].piece.GetPieceName()} at Square {squares[i].index}");
                }

                i++;
            }

            Game.Instance.settings.DebugMessage("Pieces sucessfuly generated!");
            Game.Instance.settings.DebugMessage("§aThe game is ready!");
            DrawCurrentPosition();

            Game.Instance.state = GameState.Playing;
        }

        public string GetUpdatedPosition()
        {
            string rawPosition = string.Empty;

            foreach (Square square in squares)
            {
                Piece currentPiece = square.piece;

                rawPosition = rawPosition + currentPiece.GetPieceSymbol();

            }

            StringBuilder result = new StringBuilder();
            int i = 0;

            foreach (char c in rawPosition)
            {
                if (i == 8)
                {
                    result.Append("/");
                    i = 0;
                }

                result.Append(c);
                i++;
            }

            string reversedCase = new string(result.ToString().Select(c => char.IsLetter(c) ? char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c) : c).ToArray());

            Game.Instance.currentPosition = reversedCase;
            return Game.Instance.currentPosition;

        }

        public void DrawCurrentPosition()
        {

            string[] lines = GetUpdatedPosition().Split('/');

            if (!Game.Instance.settings.graphicalBoard)
            {
                Console.WriteLine($"{Game.Instance.currentPosition} - {Game.Instance.playerTurn}");
                return;
            }


            Console.WriteLine("---------------");
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                string line = lines[i];

                for (int j = 0; j < line.Length; j++)
                {
                    char c = TurnCharToSprite(line[j]);

                    if (c == '.')
                    {
                        Console.Write(". ");
                    }
                    else if (c == ' ')
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        Console.Write($"{c} ");
                        Console.ResetColor();
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine("---------------");

            Console.WriteLine((Game.Instance.playerTurn == 0 ? "White's" : "Black's") + " turn");
        }

        char TurnCharToSprite(char c)
        {
            if (Game.Instance.settings.charMode == true) { return c; }

            switch (c)
            {
                case 'p':
                    c = '♙';
                    break;
                case 'P':
                    c = '♟';
                    break;
                case 'n':
                    c = '♘';
                    break;
                case 'N':
                    c = '♞';
                    break;
                case 'b':
                    c = '♗';
                    break;
                case 'B':
                    c = '♝';
                    break;
                case 'q':
                    c = '♕';
                    break;
                case 'Q':
                    c = '♛';
                    break;
                case 'k':
                    c = '♔';
                    break;
                case 'K':
                    c = '♚';
                    break;
                case 'r':
                    c = '♖';
                    break;
                case 'R':
                    c = '♜';
                    break;
            }

            return c;
        }

    }
}