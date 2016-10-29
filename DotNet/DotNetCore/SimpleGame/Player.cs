using System;
using System.Text;
using System.Collections.Generic;

namespace SimpleGame
{
    public class Player
    {
        public Player(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Board Move(Board board)
        {            
            var move = SelectMove(board.PossibleMoves());
            board.PlaceMove(Id, move.Item1, move.Item2);
            return board;
        }

        public static bool operator==(Player p1, Player p2)
        {
            return p1.Id == p2.Id;
        }

        public static bool operator!=(Player p1, Player p2)
        {
            return p1.Id != p2.Id;
        }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("Id: {0}, Name: {1}", Id, Name);
            return strBuilder.ToString();
        }

        private Tuple<int, int> SelectMove(List<Tuple<int, int>> moves)
        {
            var index = RandomIndex();
            return moves[index % moves.Count];
        }

        private static int RandomIndex()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            return rand.Next(0, 10000);
        }

        public string Name {get; set;}
        public int Id {get; set;}
    }
}