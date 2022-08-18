using UnityEngine;
using System.IO;
using System.Linq;

public class DataManager : MonoBehaviour
{
    public TimeCycle timeCycle;
    public PlayerVitals playerVitals;
    public WorldGenerator worldGenerator;
    public Hotbar hotbar;

    private void Start()
    {
        if (File.Exists(WorldGenerator.DataPath()))
            LoadGameData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            SaveGameData();

         if (Input.GetKeyDown(KeyCode.F4))
            LoadGameData();

        if (Input.GetKey(KeyCode.LeftShift))
            if (Input.GetKeyDown(KeyCode.P)) SaveGameData();

        if (Input.GetKey(KeyCode.LeftShift))
            if(Input.GetKeyDown(KeyCode.M)) LoadGameData();

        if (Input.GetKey(KeyCode.LeftShift))
            if (Input.GetKeyDown(KeyCode.K)) ClearGameData();

        if (Input.GetKeyDown(KeyCode.F8))
            ClearGameData();
    }

    public void SaveGameData()
    {
        print("SAVING GAME DATA...");
        HarvestManager.instance.SaveData();
        InventoryManager.instance.SaveData();
        BuildingManager.instance.SaveData();
        timeCycle.SaveData();
        worldGenerator.SaveData();
        playerVitals.SaveData();
        hotbar.SaveData();
    }

    public void LoadGameData()
    {
        print("LOADING GAME DATA...");
        HarvestManager.instance.LoadData();
        InventoryManager.instance.LoadData();
        BuildingManager.instance.LoadData();
        timeCycle.LoadDate();
        worldGenerator.LoadData();
        playerVitals.LoadData();
        hotbar.LoadData();
    }

    public void ClearGameData()
    {
        print("CLEARING EXISTING GAME DATA...");
        HarvestManager.instance.ClearData();
        InventoryManager.instance.ClearData();
        BuildingManager.instance.ClearData();
        timeCycle.ClearData();
        worldGenerator.ClearData();
        playerVitals.ClearData();
        hotbar.ClearData();
    }

    public PhysicalInteractor[] GetData()
    {
        PhysicalInteractor[] data = FindObjectsOfType<MonoBehaviour>().OfType<PhysicalInteractor>() as PhysicalInteractor[];
        return data;
    }
}
