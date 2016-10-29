using System;
using System.Text;
using System.Collections.Generic;

namespace SimpleGame
{
    public class Board
    {
        public Board()
        {
            _status = new int[3,3];
            _places1 = new List<Tuple<int, int>>();
            _places2 = new List<Tuple<int, int>>();
        }

        public Board(int[,] status)
        {
            _status = new int[3, 3];
            _places1 = new List<Tuple<int, int>>();
            _places2 = new List<Tuple<int, int>>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    _status[i, j] = status[i, j];
                    if (_status[i, j] == 1)
                    {
                        _places1.Add(new Tuple<int, int>(i, j));
                    }
                    else if(_status[i, j] == 2)
                    {
                        _places2.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
        }

        public bool PlaceMove(int id, int row, int col)
        {
            if (_status[row, col] == 0)
            {
                _status[row, col] = id;
                if (id == 1)
                {
                    _places1.Add(new Tuple<int, int>(row, col));
                }
                else
                {
                    _places2.Add(new Tuple<int, int>(row, col));
                }
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

        public int[,] GetStatus()
        {
            return _status;
        }
        
        public List<Tuple<int, int>> PossibleMoves()
        {
            var moves = new List<Tuple<int, int>>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (_status[i, j] == 0)
                    {
                        moves.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
            return moves;
        }

        public int Winner
        {
            get 
            {
                if (IsWinner(1))
                {
                    return 1;
                }
                else if (IsWinner(2))
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
                    strBuilder.AppendFormat("{0} ", _status[i, j]);
                }
                strBuilder.AppendLine("");
            }
            strBuilder.AppendLine("= = =");
            return strBuilder.ToString();
        }
        
        private bool IsWinner(int id)
        {
            var places = GetPlaces(id);
            
            var winDirect = IsWinDirect(places);
            if (winDirect)
            {
                return true;
            } 
            else
            {
                return IsWinDiamond(places);
            }
        }

        private List<Tuple<int, int>> GetPlaces(int id)
        {
            return (id == 1) ? _places1 : _places2; 
        }

        private bool IsWinDirect(List<Tuple<int, int>> places)
        {
            var countInRow = new Dictionary<int, int>();
            var countInCol = new Dictionary<int, int>();
            foreach (var place in places)
            {
                if (!countInRow.ContainsKey(place.Item1))
                {
                    countInRow[place.Item1] = 1;
                }
                else
                {
                    countInRow[place.Item1] += 1;
                }

                if (countInRow[place.Item1] == 3)
                {
                    return true;
                }

                if (!countInCol.ContainsKey(place.Item2))
                {
                    countInCol[place.Item2] = 1;
                }
                else
                {
                    countInCol[place.Item2] += 1;
                }

                if (countInCol[place.Item2] == 3)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsWinDiamond(List<Tuple<int, int>> places)
        {
            var winLeft = IsWinLeft(places);
            if (winLeft)
            {
                return true;
            }
            else
            {
                return IsWinRight(places);
            }
        }

        private bool IsWinLeft(List<Tuple<int, int>> places)
        {
            var count = 0;
            foreach (var place in places)
            {
                if (place.Item1 == place.Item2)
                {
                    count++;
                    if (count == 3)
                    {
                        return true;
                    }
                }
            }
            return count == 3;
        }

        private bool IsWinRight(List<Tuple<int, int>> places)
        {
            var count = 0;
            foreach (var place in places)
            {
                if (place.Item1 + place.Item2 == 2)
                {
                    count++;
                    if (count == 3)
                    {
                        return true;
                    }
                }
            }
            return count == 3;
        }

        private int[,] _status;
        private List<Tuple<int, int>> _places1;
        private List<Tuple<int, int>> _places2;
    }
}