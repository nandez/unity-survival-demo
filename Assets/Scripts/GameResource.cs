public class GameResource
{
    public string Name { get; private set; }
    public int Amount { get; private set; }
    public int MaxAmount { get; private set; }

    public GameResource(string name, int initialAmount, int maxAmount)
    {
        Name = name;
        Amount = initialAmount;
        MaxAmount = maxAmount;
    }

    public void Add(int value)
    {
        Amount += value;
        if (Amount < 0)
            Amount = 0;
    }

    public bool CanAdd()
    {
        return Amount < MaxAmount;
    }
}
