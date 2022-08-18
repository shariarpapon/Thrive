using UnityEngine;
using SimpleJSON;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public static bool AllowAccess = true;
    public InventoryButtonEvents inventoryEvents;
    public GameObject inventory;
    public Slot[] inventorySlots;

    private string dataPath;

    private void Awake()
    {
        if (instance != null)
            return;
        else
            instance = this;
    }
    private void Start()
    {
        dataPath = Application.persistentDataPath + "/inventory.dat";
        inventory.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && AllowAccess)
        {
            CheckState();
        }
    }


    private void CheckState()
    {
        SlotDragStateCheck();
        inventory.SetActive(!inventory.activeSelf);
        if (inventory.activeSelf)
        {
            inventoryEvents.ResetCraftingDisplay();
            Cursor.lockState = CursorLockMode.None;
            FPController.RestrictControls = true;
            FPMouseLook.RestrictRotation = true;
            PickUpItem.AllowSelection = false;
        }
        else
        {
            inventoryEvents.OnSlotMenuClose();
            Cursor.lockState = CursorLockMode.Locked;
            FPController.RestrictControls = false;
            FPMouseLook.RestrictRotation = false;
            PickUpItem.AllowSelection = true;
        }
    }

    public void Close()
    {
        inventory.SetActive(false);
        inventoryEvents.OnSlotMenuClose();
        Cursor.lockState = CursorLockMode.Locked;
        FPController.RestrictControls = false;
        FPMouseLook.RestrictRotation = false;
        PickUpItem.AllowSelection = true;
    }

    private void SlotDragStateCheck()
    {
        if (inventory.activeSelf == false)
            return;
        foreach (Slot s in inventorySlots)
        {
            DynamicSlot ds = s.transform.GetChild(0).GetComponent<DynamicSlot>();
            if (ds.beingDragged)
            {
                ds.DragEndEvent();
            }
        }
    }

    public Slot GetSlotWithItem(Item item, int amount = 0)
    {
        if (amount > 0)
        {
            foreach (Slot slot in inventorySlots)
            {
                if (slot.slotItem == item && slot.itemCount >= amount)
                    return slot;
            }
        }
        else
        {
            foreach (Slot slot in inventorySlots)
            {
                if (slot.slotItem == item)
                    return slot;
            }
        }
        return null;
    }

    public int GetItemCount(Item item)
    {
        int total = 0;
        foreach (Slot slot in inventorySlots)
        {
            if (slot.slotItem == item)
                total += slot.itemCount;
        }
        return total;
    }

    public Slot HasEnoughSpace(Item item, int amount)
    {
        foreach (Slot slot in inventorySlots)
        {
            if (slot.slotItem == item)
            {
                int total = item.spaceCost * amount;

                if (slot.spaceLeft >= total)
                    return slot;
            }
            else if (slot.slotItem == null)
                return slot;
        }
        return null;
    }

    public bool AddToInventory(Item item)
    {
        bool added = false;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].StackItem(item))
            {
                added = true;
                break;
            }
        }
        if (added == false)
        {
            for (int k = 0; k < inventorySlots.Length; k++)
            {
                if (inventorySlots[k].slotItem == null)
                {
                    if (inventorySlots[k].AddItem(item))
                    {
                        added = true;
                        break;
                    }
                }
            }
        }
        return added;
    }

    public bool InventoryIsOpen()
    {
        return inventory.activeSelf;
    }
    
    public void UpdateSlotValues(Slot slot, bool reset = false)
    {
        slot.UI.text_ItemCount.text = slot.itemCount.ToString();
        if (reset == true || slot.itemCount == 0)
        {
            slot.UI.text_ItemCount.text = string.Empty;
        }
    }

    public void SaveData()
    {
        JSONObject json = new JSONObject();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SaveData(json, i);
        }
        File.WriteAllText(dataPath, json.ToString());
    }

    public void LoadData()
    {
        if (File.Exists(dataPath))
        {
            string content = File.ReadAllText(dataPath);
            JSONObject json = JSON.Parse(content) as JSONObject;
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].LoadData(json, i);
            }
        }
    }

    public void ClearData()
    {
        if (File.Exists(dataPath))
            File.Delete(dataPath);
    }

}
