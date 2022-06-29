namespace Core
{
    public abstract class Player : Entity
    {
        public string Name { private set; get; }

        public Player(string name)
        {
            Name = name;
        }
    }
}
