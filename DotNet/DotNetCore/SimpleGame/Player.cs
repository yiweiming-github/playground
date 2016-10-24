
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
            
        }

        public static bool operator==(Player p1, Player p2)
        {
            return p1.Id == p2.Id;
        }

        public static bool operator!=(Player p1, Player p2)
        {
            return p1.Id != p2.Id;
        }

        public string Name {get; set;}
        public int Id {get; set;}
    }
}