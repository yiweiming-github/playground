using System;

namespace SimpleGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Training();
        }

        public static void Training()
        {
            Console.WriteLine("Hello SimpleGame!");

            var player1 = new Player(1, "player1");
            var player2 = new Player(2, "player2");
            var game = new Game(player1, player2);

            while(!game.IsEnd)
            {
                game.NextMove();
            }

            Console.WriteLine("Game: ");
            Console.WriteLine(game);
            Console.WriteLine("Winner is : {0}", game.Winner);
        }

        public static void Test()
        {
            var board1 = new Board(new int[,]{{1,1,1},
                                              {1,2,2},
                                              {2,2,1}});
            Console.WriteLine("board1 winner is: {0}", board1.Winner);

            var board2 = new Board(new int[,]{{2,1,1},
                                              {1,2,2},
                                              {1,1,2}});
            Console.WriteLine("board2 winner is: {0}", board2.Winner);

            var board4 = new Board(new int[,]{{1,2,1},
                                              {2,1,2},
                                              {1,1,2}});
            Console.WriteLine("board4 winner is: {0}", board4.Winner);
        }
    }
}
