using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameResource
{
    [SerializeField] private int amount;
    [SerializeField] private int maxAmount;

    public GameObject ItemPrefab;
    public Text resourceUiText;

    public bool CanAdd()
    {
        return amount < maxAmount;
    }

    public bool Add()
    {
        if (CanAdd())
        {
            amount++;
            return true;
        }

        return false;
    }

    public int GetCurrentAmount()
    {
        return amount;
    }

    public int GetMaxAmount()
    {
        return maxAmount;
    }
}
