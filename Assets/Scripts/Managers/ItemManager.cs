using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public Item[] items;

    public  Item ItemWithName(string name)
    {
        foreach (Item item in items)
        {
            if (item.name == name)
                return item;
        }
        return null;
    }
}
