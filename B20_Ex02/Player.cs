using System;
using System.Collections.Generic;

namespace B20_Ex02
{
    public class Player
    {
        private readonly string m_Name;
        private int m_PointsForCorrectGuesses;
        private AI m_AIPlayer;
        //----------------------------------------------------------------------//
        public Player(string i_Name)
        {
            this.m_Name = i_Name;
            this.m_PointsForCorrectGuesses = 0;
            this.m_AIPlayer = null;
        }
        //----------------------------------------------------------------------//
        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
        //----------------------------------------------------------------------//
        public int PointsForCorrectGuesses
        {
            get
            {
                return this.m_PointsForCorrectGuesses;
            }
            set
            {
                this.m_PointsForCorrectGuesses = value;
            }
        }
        //----------------------------------------------------------------------//
        public AI AIPlayer
        {
            get
            {
                return this.m_AIPlayer;
            }
            set
            {
                this.m_AIPlayer = value;
            }
        }
    }
}
