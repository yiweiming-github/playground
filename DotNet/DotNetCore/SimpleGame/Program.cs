using System;

namespace SimpleGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello SimpleGame!");

            var player1 = new Player("player1");
            var player2 = new Player("player2");
            var game = new Game(player1, player2);

            while(!game.IsEnd)
            {
                game.NextMove();
            }

            
        }
    }
}
