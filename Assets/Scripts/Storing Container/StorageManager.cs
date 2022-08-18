using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public List<Storage> storages = new List<Storage>();

    public void SaveData()
    {
        if (storages.Count <= 0)
            return;

        foreach (Storage store in storages)
        {
            store.Save();
        }
    }

    public void LoadData()
    {
        if (storages.Count <= 0)
            return;

        foreach (Storage store in storages)
        {
            store.Load();
        }
    }

    public void ClearData()
    {
        if (storages.Count <= 0)
            return;

        foreach (Storage store in storages)
        {
            store.Clear();
        }
    }
}