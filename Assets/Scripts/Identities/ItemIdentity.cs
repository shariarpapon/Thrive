using UnityEngine;

public class ItemIdentity : Identity
{
    public Item item;

    [HideInInspector]
    public bool isEquiped = false;

    public void Destroy()
    {
        Destroy(this);
    }
}
