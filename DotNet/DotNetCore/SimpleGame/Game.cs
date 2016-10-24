using System.Collections.Generic;
using System.Text;

namespace SimpleGame
{
    public class Game
    {
        public Game(Player player1, Player player2)
        {
            _player1 = player1;
            _player2 = player2;
            _nextMovePlayer = player1;
            _boardSeries = new List<Board>();
        }

        public void NextMove()
        {
            Board board = null;
            if (_boardSeries.Count == 0)
            {
                board = new Board();
            }
            else
            {
                board = _boardSeries[_boardSeries.Count - 1].Clone();
            }

            board = _nextMovePlayer.Move(board);
            _boardSeries.Add(board);

            if (board.Winner > 0)
            {
                _isEnd = true;
            }

            _nextMovePlayer = (_nextMovePlayer.Id == 1) ? _player2 : _player1;
        }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            foreach (var board in _boardSeries)
            {
                strBuilder.AppendLine(board.ToString());
            }
            return strBuilder.ToString();
        }

        public bool IsEnd
        {
            get
            {
                return _isEnd;
            }
        }

        private readonly Player _player1;
        private readonly Player _player2;
        private bool _isEnd;
        private Player _winner;
        private Player _nextMovePlayer;
        private List<Board> _boardSeries;
    }
}