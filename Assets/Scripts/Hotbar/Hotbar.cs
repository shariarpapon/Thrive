using SimpleJSON;
using System.IO;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public HotSlot[] hotSlots;

    private EquipmentManager equipment;

    private void Awake()
    {
        equipment = FindObjectOfType<EquipmentManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(hotSlots[0].hotItem != null && hotSlots[1].hotItem != null)
                SwapHotSlot(hotSlots[0], hotSlots[1]);
        }
    }

    public Item SelectedItem()
    {
        if (hotSlots[0].hotItem != null)
            return hotSlots[0].hotItem;
        else
            return null;
    }

    public void SwapHotSlot(HotSlot trigger, HotSlot target)
    {
        if (trigger.hotItem.canEquip == false || target.hotItem.canEquip == false)
            return;

        Item cItem = target.hotItem;

        target.hotItem = trigger.hotItem;
        target.slotImage.sprite = trigger.slotImage.sprite;

        trigger.hotItem = cItem;
        trigger.slotImage.sprite = cItem.icon;
    }


    public void DestroySelectedItem()
    {
        hotSlots[0].ClearHotSlot();
    }

    public static string DataPath()
    {
        return Application.persistentDataPath + "/hotbarslots.dat";
    }

    public void SaveData()
    {
        JSONObject js = new JSONObject();
        for (int i = 0; i < hotSlots.Length; i++)
        {
            hotSlots[i].Save(js, i);
        }
        File.WriteAllText(DataPath(), js.ToString());
    }

    public void LoadData()
    {
        if (File.Exists(DataPath()))
        {
            string content = File.ReadAllText(DataPath());
            JSONObject json = JSON.Parse(content) as JSONObject;
            for (int i = 0; i < hotSlots.Length; i++)
            {
                hotSlots[i].Load(json, i);
            }
        }
    }

    public void ClearData()
    {
        if (File.Exists(DataPath()))
        {
            File.Delete(DataPath());
        }
    }
}
