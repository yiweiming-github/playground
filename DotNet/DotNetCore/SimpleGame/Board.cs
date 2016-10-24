using System;
using System.Text;

namespace SimpleGame
{
    public class Board
    {
        public Board()
        {
            _status = new int[3,3];
        }

        public Board(int[,] status)
        {
            _status = new int[3,3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    _status[i, j] = status[i, j];
                }
            }
        }

        public bool PlaceMove(int id, int row, int col)
        {
            if (_status[row, col] == 0)
            {
                _status[row, col] = id;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Board Clone()
        {
            return new Board(_status);
        }

        public int Winner
        {
            get 
            {
                if (WinOne)
                {
                    return 1;
                }
                else if (WinTwo)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    strBuilder.AppendFormat("{1} ", _status[i, j]);
                }
            }
            return strBuilder.ToString();
        }

        private bool WinOne
        {
            get
            {
                return true;
            }
        }

        private bool WinTwo
        {
            get
            {
                return true;
            }
        }
        
        private int[,] _status;
    }
}