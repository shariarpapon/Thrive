using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DCButtonIdentity : Identity
{
    public Drop drop;

    private InventoryManager inventory;

    private void Awake()
    {
        inventory = FindObjectOfType<InventoryManager>();
    }

    public void AddSingle()
    {
        DropContainerIdentity container = drop.instance.GetComponent<DropContainerIdentity>();
        if (inventory.AddToInventory(drop.item))
        {
            Drop _drop = container.GetDropFromList(drop);
            if (_drop != null)
            {
                _drop.amount--;
                UpdateButtonText(_drop.amount);
                if (_drop.amount <= 0)
                {
                    container.RemoveDrop(_drop);
                    Destroy(gameObject);
                }
            }
        }
    }

    public void UpdateButtonText(int newAmount)
    {
        TextMeshProUGUI tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tmp.text = drop.item.name + " [" + newAmount.ToString() + "]";
        if (newAmount == 0)
            Destroy(gameObject);
    }
    public void AddPreferred(int amount) { }
}
