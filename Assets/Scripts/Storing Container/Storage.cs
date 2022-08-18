using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class StoreData
{
    public Item item;
    public int amount;

    public StoreData(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
}

public class Storage : MonoBehaviour
{
    public int maxSpace;
    public int spaceLeft;
    public bool isOpen = false; 

    private Animator animator;
    private ItemManager itemManager;

    public List<StoreData> storage = new List<StoreData>();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        itemManager = FindObjectOfType<ItemManager>();
    }

    public void Store(Item item, int amount)
    {
        StoreData data = new StoreData(item, amount);
        storage.Add(data);
        UpdateStorage(data);
    }

    private void UpdateStorage(StoreData data)
    {
        print("Updating storage;  " + data.item);
    }

    private void OnMouseDown()
    {
        if (animator != null)
            animator.Play("Open");

        isOpen = true;
        print("Open storage UI");
    }

    public void Save()
    {
        Clear();
        JSONObject json = new JSONObject();
        json.Add("storageCount", storage.Count);
        for (int i = 0; i < storage.Count; i++)
        {
            json.Add("itemName" + i, storage[i].item.name);
            json.Add("amount" + i, storage[i].amount);
        }
        System.IO.File.WriteAllText(DataPath(), json.ToString());
    }

    public void Load()
    {
        JSONObject json = JSON.Parse(System.IO.File.ReadAllText(DataPath())) as JSONObject;
        int count = json["storageCount"];
        for (int i = 0; i < count; i++)
        {
            Item item = itemManager.ItemWithName(json["itemName" + i]);
            int amount = json["amount" + i];
            Store(item, amount);
        }
    }

    public void Clear()
    {
        if (System.IO.File.Exists(DataPath()))
            System.IO.File.Delete(DataPath());
    }

    private string DataPath()
    {
        return Application.persistentDataPath + "/builtitemstorage.dat";
    }
    public bool HasSpace()
    {
        if (spaceLeft > 0)
            return true;
        else
            return false;
    }

    public bool IsFull()
    {
        if (spaceLeft <= 0)
        {
            spaceLeft = 0;
            return true;
        }
        return false;
    }

    public bool IsEmpty()
    {
        if (spaceLeft >= maxSpace)
        {
            spaceLeft = maxSpace;
            return true;
        }
        return false;
    }
    
}
