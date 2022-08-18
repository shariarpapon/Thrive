using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using TMPro;

public class Slot : MonoBehaviour
{
    [System.Serializable]
    public struct SlotUI
    {
        public TextMeshProUGUI text_ItemCount;
    }

    public int spaceLeft = 20;
    public int maxSpace = 20;
    public int itemCount;
    [HideInInspector]
    public Item slotItem = null;
    public SlotUI UI;
    public bool stackable;
    public Image slotIcon;

    private InventoryManager inventoryManager;
    private ItemManager itemManager;

    private void Awake()
    {
        itemManager = FindObjectOfType<ItemManager>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        slotIcon = transform.GetChild(0).GetComponent<Image>();
        slotIcon.gameObject.SetActive(false);
        UI.text_ItemCount.text = string.Empty;
    }

    public bool AddItem(Item item, int amount = 0)
    {
        if (amount > 0 && (item == slotItem || item == null))
        {
            itemCount += amount;
            if (item == null)
            {
                slotIcon.gameObject.SetActive(true);
                slotIcon.sprite = item.icon;
                slotItem = item;
                spaceLeft -= amount;
            }
            inventoryManager.UpdateSlotValues(this);
            return true;
        }

        if (slotItem == null)
        {
            slotIcon.gameObject.SetActive(true);
            slotIcon.sprite = item.icon;
            slotItem = item;
            spaceLeft -= item.spaceCost;
            itemCount++;
            int futureSpace = spaceLeft - item.spaceCost;

            if (futureSpace >= 0)
                stackable = true;
            else
                stackable = false;

            inventoryManager.UpdateSlotValues(this);
            return true;
        }
        return false;
    }

    public bool RemoveItem(int amount)
    {
        if (slotItem != null)
        {
            int amt = amount;
            spaceLeft += amt * slotItem.spaceCost;
            itemCount -= amount;

            if (spaceLeft >= maxSpace)
            {
                itemCount = 0;
                spaceLeft = maxSpace;
                slotItem = null;
                slotIcon.sprite = null;
                slotIcon.gameObject.SetActive(false);
                inventoryManager.UpdateSlotValues(this, true);
                return true;
            }
            inventoryManager.UpdateSlotValues(this);
        }
        return false;
    }

    public bool StackItem(Item item)
    {
        if (slotItem == item)
        {
            int sp = spaceLeft;
            int after = sp - item.spaceCost;
            if (after >= 0)
            {
                itemCount++;
                spaceLeft -= item.spaceCost;
                inventoryManager.UpdateSlotValues(this);
                return true;
            }
            else
                return false;
        }
        return false;
    }

    public void ClearSlot()
    {
        itemCount = 0;
        spaceLeft = maxSpace;
        slotItem = null;
        slotIcon.sprite = null;
        inventoryManager.UpdateSlotValues(this, true);
        slotIcon.gameObject.SetActive(false);
    }

    public void CheckToSwap(Slot target, Vector3 original, CanvasGroup canvasGroup)
    {
        canvasGroup.blocksRaycasts = true;
        slotIcon.transform.localPosition = original;

        if (target.slotItem == null)
        {
            target.slotItem = slotItem;
            target.slotIcon.gameObject.SetActive(true);
            target.slotIcon.sprite = slotItem.icon;
            target.spaceLeft = spaceLeft;
            target.stackable = stackable;
            target.itemCount = itemCount;
            inventoryManager.UpdateSlotValues(target);
            slotItem = null;
            spaceLeft = maxSpace;
            itemCount = 0;
            slotIcon.sprite = null;
            slotIcon.gameObject.SetActive(false);
            inventoryManager.UpdateSlotValues(this);
        }
        else if (target.slotItem == slotItem)
        {
            int mySpaceTaken = maxSpace - spaceLeft;
            if (target.spaceLeft >= mySpaceTaken)
            {
                int temp = target.spaceLeft - mySpaceTaken;
                if (temp >= 0)
                {
                    target.spaceLeft -= mySpaceTaken;
                    target.itemCount += itemCount;
                    RemoveItem(itemCount);
                }
            }
            else if (target.spaceLeft < mySpaceTaken)
            {
                int newSpace = target.spaceLeft;
                int newItemCount = GetItemCountBasedOnSpace(newSpace, slotItem.spaceCost);
                RemoveItem(newItemCount);
                target.spaceLeft -= newSpace;
                target.itemCount += newItemCount;
                target.stackable = false;
            }
            inventoryManager.UpdateSlotValues(this);
            inventoryManager.UpdateSlotValues(target);
        }
        else if (target.slotItem != null && target.slotItem != slotItem)
        {
            Item cItem = target.slotItem;
            int cItemCount = target.itemCount;
            int cSpaceLeft = target.spaceLeft;
            bool cStackable = target.stackable;

            target.slotItem = slotItem;
            target.itemCount = itemCount;
            target.spaceLeft = spaceLeft;
            target.stackable = stackable;
            target.slotIcon.sprite = slotIcon.sprite;

            slotItem = cItem;
            itemCount = cItemCount;
            spaceLeft = cSpaceLeft;
            stackable = cStackable;
            slotIcon.sprite = cItem.icon;
        }
        InventoryManager.instance.UpdateSlotValues(target);
        InventoryManager.instance.UpdateSlotValues(this);
    }

    public void RemoveOne()
    {
        if (itemCount <= 0)
            return;

        itemCount--;
        if (itemCount <= 0)
        {
            ClearSlot();
            itemCount = 0;
        }
        InventoryManager.instance.UpdateSlotValues(this);
    }

    private int GetItemCountBasedOnSpace(int space, int perSpace)
    {
        int count = 0;
        for (int i = 0; i < space; i += perSpace)
        {
            count++;
        }
        return count;
    }

    public void UseItem()
    {
        if(slotItem != null)
            slotItem.Use(this);
    }

    public void SaveData(JSONObject json, int key)
    {
        json.Add("space" + key, spaceLeft);
        json.Add("stackable" + key, stackable);
        json.Add("icount" + key, itemCount);
        if (slotItem != null)
            json.Add("name" + key, slotItem.name);
    }

    public void LoadData(JSONObject json, int key)
    {
        spaceLeft = json["space" + key];
        stackable = json["stackable" + key];
        itemCount = json["icount" + key];
        if (itemCount > 0)
        {
            Item item = itemManager.ItemWithName(json["name" + key]);
            slotIcon.gameObject.SetActive(true);
            slotIcon.sprite = item.icon;
            slotItem = item;
        }
        inventoryManager.UpdateSlotValues(this);
    }

}

