using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleJSON;

public class HotSlot : MonoBehaviour
{
    public Item hotItem;
    public Image slotImage;
    public bool isOccupied;

    private EquipmentManager equipment;
    private InventoryManager inventory;

    private void Awake()
    {
        inventory = FindObjectOfType<InventoryManager>();
        equipment = FindObjectOfType<EquipmentManager>();
        SetIconTransparent(true);
    }

    public void InsertItem(Item item , bool swapWithISlot = false, Slot swapSlot = null)
    {
        Item prev = hotItem;
        if (item.canEquip == false)
            return;

        if (swapWithISlot)
        {
            hotItem = swapSlot.slotItem;
            slotImage.sprite = swapSlot.slotItem.icon;
            isOccupied = true;
            SetIconTransparent(false);
            equipment.UpdateActiveEquipment();

            swapSlot.ClearSlot();
            swapSlot.AddItem(prev);
            return;
        }

        if (isOccupied == false)
        {
            ItemToHotSlot(item);
        }
    }

    private void ItemToHotSlot(Item item)
    {
        hotItem = item;
        slotImage.sprite = item.icon;
        isOccupied = true;
        SetIconTransparent(false);
        equipment.UpdateActiveEquipment();
    }

    public void ClearHotSlot()
    {
        hotItem = null;
        slotImage.sprite = null;
        isOccupied = false;
        SetIconTransparent(true);
        equipment.UpdateActiveEquipment();
    }

    private void SetIconTransparent(bool makeTransparent)
    {
        if (makeTransparent)
        {
            slotImage.color = Color.clear;
        }
        else
        {
            slotImage.color = new Color(255, 255, 255, 255);
        }
    }

    public void Save(JSONObject json, int key)
    {
        json.Add("htocc" + key.ToString(), isOccupied);
        if(hotItem != null)
        {
            json.Add("hname" + key.ToString(), hotItem.name);
        }
    }


    public void Load(JSONObject js, int key)
    {
        ItemManager im = FindObjectOfType<ItemManager>();
        bool occupy = js["htocc" + key.ToString()];
        isOccupied = occupy;
        if (isOccupied)
        {
            Item hItem = im.ItemWithName(js["hname" + key.ToString()]);
            hotItem = hItem;
            slotImage.sprite = hItem.icon;
            SetIconTransparent(false);
        }
    }
}
