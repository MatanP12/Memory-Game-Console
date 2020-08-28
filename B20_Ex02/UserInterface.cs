using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace B20_Ex02
{
    // $G$ DSN-999 (-5) all of the class is public with no reason

    public class UserInterface
    {
        private GuessingGame m_Game;
        //----------------------------------------------------------------------//
        public GuessingGame Game
        {
            get
            {
                return this.m_Game;
            }
        }
        //----------------------------------------------------------------------//
        public UserInterface()
        {
            this.m_Game = new GuessingGame();
        }
        //----------------------------------------------------------------------//
        public void StartGame()
        {
            bool continuePlaying = true;
            bool firstGame = true;

            while (continuePlaying)
            {
                if (firstGame)
                {
                    this.CreatePlayers();
                    firstGame = false;
                }
                else
                {
                    Ex02.ConsoleUtils.Screen.Clear();
                    this.Game.RestartGame();
                }

                this.CreateBoard();
                Ex02.ConsoleUtils.Screen.Clear();
                continuePlaying = this.PlayGame();
            }
        }
        //----------------------------------------------------------------------//
        public void CreatePlayers()
        { 
            Console.Write("Hello player, please enter your name: ");
            string choice, name = Console.ReadLine();

            this.m_Game.AddPlayerToList(name);
            Console.WriteLine(@"
{0}, would you like to play against another player or against the PC?
1. Another player
2. Against the PC", name);
            choice = Console.ReadLine();

            while (!this.m_Game.IsUserGameTypeInputValid(choice))
            {
                Console.WriteLine("Invalid input, please try again: ");
                choice = Console.ReadLine();
            }

            if (this.m_Game.GameType == GuessingGame.eGameType.PlayerVSPlayer)
            {
                Console.Write("Please Enter the 2nd players name: ");
                name = Console.ReadLine();
                this.m_Game.AddPlayerToList(name);
            }
            else
            {
                this.m_Game.AddPlayerToList("PC");
                this.m_Game.Players.Last().AIPlayer = new AI();
            }
        }
        //----------------------------------------------------------------------//
        public void CreateBoard()
        {
            Console.WriteLine("Please enter the board size, range of the board is minimum 4x4 and maximum 6x6");
            Console.Write("Rows: ");
            string rowSize = Console.ReadLine();
            Console.Write("Columns: ");
            string colSize = Console.ReadLine();

            while (this.m_Game.CheckForValidBoardLimitInput(rowSize, colSize) == false)
            {
                Console.WriteLine("Invalid size, please enter the board size, range of the board is minimum 4x4 and maximum 6x6");
                Console.Write("Rows: ");
                rowSize = Console.ReadLine();
                Console.Write("Columns: ");
                colSize = Console.ReadLine();
            }

            this.m_Game.SetBoard(int.Parse(rowSize), int.Parse(colSize));
        }
        //----------------------------------------------------------------------//
        public bool PlayGame()
        {
            Player currentPlayer = this.m_Game.Players.First();

            while (!this.m_Game.GameEnd())
            {
                this.makeAPlayerMove(ref currentPlayer);
            }

            this.DeclareWinner();
            Console.Write("{0}, would you like to play again? Y/N: ", this.m_Game.Players.First().Name);
            string userAnswer = Console.ReadLine();

            while (userAnswer != "Y" && userAnswer != "N")
            {
                Console.Write("No such option, please enter Y/N: ");
                userAnswer = Console.ReadLine();
            }

            return userAnswer == "Y";
        }
        //----------------------------------------------------------------------//
        private void makeAPlayerMove(ref Player io_Player)
        {
            Board.Tile firstFlipTile, secondFlipTile;
            this.ClearScreenShowBoard(io_Player);

            if (this.m_Game.IsPlayerAI(io_Player))
            {
                io_Player.AIPlayer.MakeAIMove(this, out firstFlipTile, out secondFlipTile, io_Player);
            }
            else
            {
                Console.Write("Please enter a first card to flip: ");
                this.userInputMove(out firstFlipTile, io_Player);
                Console.Write("Please enter a second card to flip: ");
                this.userInputMove(out secondFlipTile, io_Player);
            }

            if (this.m_Game.IsMatchingFlip(firstFlipTile, secondFlipTile))
            {
                Console.WriteLine("Good job, you get a point!");
                ++io_Player.PointsForCorrectGuesses;
            }
            else
            {
                Console.WriteLine("Oops... not a match!");
                firstFlipTile.CloseTile();
                secondFlipTile.CloseTile();
                this.m_Game.SwitchPlayer(ref io_Player);
            }

            System.Threading.Thread.Sleep(2000);
        }
        //----------------------------------------------------------------------//
        private void userInputMove(out Board.Tile io_Tile, Player i_Player)
        {
            string userMoveInput = Console.ReadLine();
            this.m_Game.DidUserQuit(userMoveInput);

            while (!this.m_Game.CheckForValidGameMove(userMoveInput, out io_Tile) || (io_Tile != null && io_Tile.IsOpen == true))
            {
                if ((io_Tile != null && io_Tile.IsOpen == true))
                {
                    Console.Write("Tile already open, please re-enter another tile to flip: ");
                }
                else
                {
                    Console.Write("Invalid tile, please re-enter another tile to flip: ");
                }

                userMoveInput = Console.ReadLine();
                this.m_Game.DidUserQuit(userMoveInput);
            }

            io_Tile.OpenTile();
            this.ClearScreenShowBoard(i_Player);
        }
        //----------------------------------------------------------------------//
        public void ClearScreenShowBoard(Player i_Player)
        {
            Ex02.ConsoleUtils.Screen.Clear();
            printScoreBoard();
            Console.WriteLine("{0}'s turn now: {1}", i_Player.Name, Environment.NewLine);
            this.PrintBoard();
            Console.WriteLine();
        }
        //----------------------------------------------------------------------//
        public void PrintBoard()
        {
            int rowNumber = 1;
            char columnChar = 'A';
            Console.Write("   ");

            for (int j = 0; j < this.m_Game.Board.ColumnBorder; ++j)
            {
                Console.Write(" {0}  ", columnChar);
                ++columnChar;
            }

            Console.WriteLine();
            this.printSeparationLine();

            for (int i = 0; i < this.m_Game.Board.RowBorder; ++i)
            {
                Console.Write("{0} |", rowNumber);
                ++rowNumber;

                for (int j = 0; j < this.m_Game.Board.ColumnBorder; ++j)
                {
                    if(this.m_Game.Board[i, j].ContentOfTile == -1)
                    {
                        Console.Write("   |");
                    }
                    else
                    {
                        Console.Write(" {0} |",this.m_Game.HashObject(this.m_Game.Board[i,j].ContentOfTile));
                    }
                }

                Console.WriteLine();
                this.printSeparationLine();
            }
        }
        //----------------------------------------------------------------------//
        private void printScoreBoard()
        {
            Console.WriteLine(@"
         Score Board
||============================||
||    {0,-12}|  {1,-5}    ||
||----------------------------||
||    {2,-12}|  {3,-5}    ||
||============================||{4}",
                this.m_Game.Players.First().Name,
                this.Game.Players.First().PointsForCorrectGuesses,
                this.Game.Players.Last().Name,
                this.Game.Players.Last().PointsForCorrectGuesses,
                Environment.NewLine);
        }
        //----------------------------------------------------------------------//
        private void printSeparationLine()
        {
            Console.Write("  ");

            for (int i = 0; i < this.m_Game.Board.ColumnBorder * 4 + 1; ++i)
            {
                Console.Write("=");
            }
            
            Console.WriteLine();
        }
        //----------------------------------------------------------------------//
        public void DeclareWinner()
        {
            bool isADraw;
            Player winner = this.m_Game.FindWinner(out isADraw);
            Ex02.ConsoleUtils.Screen.Clear();
            this.printScoreBoard();

            if (isADraw)
            {
                Console.WriteLine("It's a DRAW!");
            }
            else
            {
                Console.WriteLine("Congratulations {0}, you are the winner!", winner.Name);
            }
        }
    }
}
