using System;
using System.Collections.Generic;

namespace B20_Ex02
{
    public class AI
    {
        private List<Board.Tile> m_RememberFlips;
        //----------------------------------------------------------------------//
        public AI()
        {
            this.m_RememberFlips = new List<Board.Tile>(36);
        }
        //----------------------------------------------------------------------//
        internal List<Board.Tile> RememberFlip
        {
            get
            {
                return this.m_RememberFlips;
            }
            set
            {
                this.m_RememberFlips = value;
            }
        }
        //----------------------------------------------------------------------//
        internal void MakeAIMove(UserInterface i_UI, out Board.Tile o_FirstTile, out Board.Tile o_SecondTile, Player i_Player)
        {
            bool foundMatchInList = findAMatchInList(out o_FirstTile, out o_SecondTile, i_UI.Game);

            if (!foundMatchInList)
            {
                o_SecondTile = this.ReturnARandomTileFromBoard(i_UI.Game);
                while (o_SecondTile == o_FirstTile)
                {
                    o_SecondTile = this.ReturnARandomTileFromBoard(i_UI.Game);
                }
                if (!this.m_RememberFlips.Contains(o_SecondTile) && !i_UI.Game.IsMatchingFlip(o_FirstTile, o_SecondTile))
                {
                    this.m_RememberFlips.Add(o_SecondTile);
                }
                else if(i_UI.Game.IsMatchingFlip(o_FirstTile, o_SecondTile))
                {
                    this.m_RememberFlips.Remove(o_FirstTile);
                }
            }
            else
            {
                this.m_RememberFlips.Remove(o_FirstTile);
                this.m_RememberFlips.Remove(o_SecondTile);
            }

            this.OpenAndPrintBoard(o_FirstTile, o_SecondTile, i_UI, i_Player);
        }
        //----------------------------------------------------------------------//
        internal bool ReturnRandomTileFromList(out Board.Tile io_Tile, Random i_Random)
        {
            io_Tile = null;
            bool isTileFound = false;

            if (this.m_RememberFlips.Count != 0)
            {
                io_Tile = m_RememberFlips[i_Random.Next(0, m_RememberFlips.Count)];
            }

            while (this.m_RememberFlips.Count > 1  && io_Tile.IsOpen)
            {
                this.m_RememberFlips.Remove(io_Tile);
                io_Tile = m_RememberFlips[i_Random.Next(0, m_RememberFlips.Count)];
            }

            if (this.m_RememberFlips.Count != 0 && io_Tile.IsOpen)
            {
                this.m_RememberFlips.Remove(io_Tile);
            }
            else if (io_Tile != null)
            {
                isTileFound = true;
            }

            return isTileFound;
        }
        //----------------------------------------------------------------------//
        internal Board.Tile ReturnARandomTileFromBoard(GuessingGame i_Game)
        {
            int rowIndex = i_Game.RandomNumber.Next(0, i_Game.Board.RowBorder);
            int colIndex = i_Game.RandomNumber.Next(0, i_Game.Board.ColumnBorder);

            while (i_Game.Board[rowIndex, colIndex].IsOpen)
            {
                rowIndex = i_Game.RandomNumber.Next(0, i_Game.Board.RowBorder);
                colIndex = i_Game.RandomNumber.Next(0, i_Game.Board.ColumnBorder);
            }

            return i_Game.Board[rowIndex, colIndex];
        }
        //----------------------------------------------------------------------//
        internal bool findAMatchingSet(Board.Tile i_FirstTile, out Board.Tile o_SecondTile, GuessingGame i_Game)
        {
            o_SecondTile = null;
            bool isMatchFound = false;

            foreach (Board.Tile currentTile in this.m_RememberFlips)
            {
                if (i_Game.IsMatchingFlip(i_FirstTile, currentTile))
                {
                    if (currentTile != i_FirstTile)
                    {
                        o_SecondTile = currentTile;
                        isMatchFound = true;
                        break;
                    }
                }
            }

            return isMatchFound;
        }
        //----------------------------------------------------------------------//
        internal void OpenAndPrintBoard(Board.Tile i_FirstTile, Board.Tile i_SecondTile, UserInterface i_UI, Player i_Player)
        {
            i_FirstTile.OpenTile();
            i_UI.ClearScreenShowBoard(i_Player);
            System.Threading.Thread.Sleep(2000);
            i_SecondTile.OpenTile();
            i_UI.ClearScreenShowBoard(i_Player);
        }
        //----------------------------------------------------------------------//
        private bool findAMatchInList(out Board.Tile o_FirstTile, out Board.Tile o_SecondTile, GuessingGame i_Game)
        {
            bool isFoundMatch = false;
            o_SecondTile = o_FirstTile = null;

            foreach (Board.Tile currentTile in this.m_RememberFlips)
            {
                if (!currentTile.IsOpen)
                {
                    isFoundMatch = this.findAMatchingSet(currentTile, out o_SecondTile, i_Game);

                    if (isFoundMatch)
                    {
                        o_FirstTile = currentTile;
                        break;
                    }
                }
            }

            if (!isFoundMatch)
            {
                o_FirstTile = this.ReturnARandomTileFromBoard(i_Game);

                if (!this.m_RememberFlips.Contains(o_FirstTile))
                {
                    this.m_RememberFlips.Add(o_FirstTile);

                    if (findAMatchingSet(o_FirstTile, out o_SecondTile, i_Game))
                    {
                        isFoundMatch = true;
                    }
                }
            }

            return isFoundMatch;
        }
        //----------------------------------------------------------------------//
        public void ForgetList()
        {
            this.m_RememberFlips.Clear();
        }
    }
}
