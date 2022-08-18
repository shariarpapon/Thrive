using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;

[System.Serializable]
public class Drop
{
    public Item item;
    public int amount;
    public GameObject instance; 

    public Drop(Item _item, int _amount, GameObject _instance)
    {
        item = _item;
        amount = _amount;
        instance = _instance;
    }
}

[System.Serializable]
public class ContainerData
{
    public GameObject instance;
    public List<Drop> drops;
    public float px, py, pz;
    public float rx, ry, rz;
    public string resourcePath;

    public ContainerData(GameObject inst , List<Drop> _drops, float psx, float psy, float psz, float rtx, float rty, float rtz, string resPath)
    {
        instance = inst;
        drops = _drops;
        px = psx;
        py = psy;
        pz = psz;
        rx = rtx;
        ry = rty;
        rz = rtz;
        resourcePath = resPath;
    }
}

public class HarvestManager : MonoBehaviour
{
    public static HarvestManager instance;
    public Transform containerHolder;
    public Transform particleHolder;
    public GameObject defaultContainer;
    public Camera mainCam;
    public int defaultDamage;
    public float detectionRadius;

    private Ray ray;
    private RaycastHit hit;
    private FPController player;
    private Hotbar hotbar;
    private ItemManager itemManager;
    private BuildingManager buildingManager;


    internal bool isHarvesting = false;
    internal List<ContainerData> containerData = new List<ContainerData>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        itemManager = FindObjectOfType<ItemManager>();
        hotbar = FindObjectOfType<Hotbar>();
        player = FindObjectOfType<FPController>();
        buildingManager = FindObjectOfType<BuildingManager>();
    }

    private void Update()
    {
        if (buildingManager.showing)
            return;

        if (Input.GetMouseButton(0))
        {
            ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, detectionRadius))
            {
                HarvestIdentity harvID = hit.transform.GetComponent<HarvestIdentity>();
                Item requiredItem = hotbar.SelectedItem();

                if (isHarvesting == false && harvID != null && InventoryManager.instance.InventoryIsOpen() == false)
                    StartCoroutine(InitiateHarvest(harvID.harvestable.harvestDelay, requiredItem, harvID, hit));
            }
        }
    }

    private IEnumerator InitiateHarvest(float delay, Item requiredItem, HarvestIdentity harvID, RaycastHit myHit)
    {
        Vector3 containerPosition = myHit.transform.position;
        int dmg = defaultDamage;
        if (harvID.harvestable.hasRquirement && hotbar.SelectedItem() != null)
        {
            if (harvID.harvestable.requiredType == requiredItem.itemType)
                dmg = requiredItem.damage;
        }
        isHarvesting = true;

        if (harvID.harvestable.particle != null)
            Instantiate(harvID.harvestable.particle, myHit.point, GetRandomParticleRotation(), particleHolder);

        if (harvID.InflictDamage(dmg))
        {
            if (harvID.harvestable.end_SFX == true)
                AudioManager.instance.InstantPlay(harvID.harvestable.finalSFX);

            Harvest(harvID.harvestable, containerPosition);
            Destroy(myHit.transform.gameObject);
            isHarvesting = false;
            yield return new WaitForSeconds(0);
        }
        else
        {
            if (harvID.harvestable.start_SFX == true)
                AudioManager.instance.InstantPlay(harvID.harvestable.SFX);
            else
                Debug.LogWarning("Harvest start_SFX does not exit for : " + harvID.harvestable.name);
        }

        yield return new WaitForSeconds(delay);
         isHarvesting = false;
    }

    private Quaternion GetRandomParticleRotation()
    {
        Quaternion rot = Quaternion.Euler(Random.Range(0, 270), Random.Range(0, 270), Random.Range(0, 270));
        return rot;
    }

    private Quaternion GetRandomContainerRotation()
    {
        Quaternion rot = Quaternion.Euler(0, Random.Range(0, 270), 0);
        return rot;
    }

    public void Harvest(Harvestable harvestable, Vector3 containerPosition)
    {
        GameObject containerPrefab = defaultContainer;

        if (harvestable.dropContainer != null)
            containerPrefab = harvestable.dropContainer;

         GameObject containerInstance = Instantiate(containerPrefab, containerPosition, GetRandomContainerRotation(), containerHolder);
         DropContainerIdentity dropContainer = containerInstance.transform.GetComponent<DropContainerIdentity>();

        foreach (Harvest h in harvestable.harvests)
        {
            int dropAmount = EvaluateDropAmount(h.frequency.minAmount, h.frequency.maxAmount, h.frequency.chance);
            if (dropAmount > 0)
            {
                AddItemsToContainer(dropContainer, h.dropItem, dropAmount);
            }
        }
        AddContainerData(dropContainer, containerPrefab.name);
    }

    public void AddContainerData(DropContainerIdentity containerID, string containerPath, ContainerData _data = null)
    {
        if (_data != null)
        {
            containerData.Add(_data);
            return;
        }
        Vector3 pos = containerID.transform.position;
        Vector3 rot = containerID.transform.rotation.eulerAngles;
        ContainerData data = new ContainerData(containerID.gameObject, containerID.drops, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, containerPath);
        containerData.Add(data);
    }

    public int EvaluateDropAmount(int min, int max, float chance)
    {
        int amount = 0;
        int chancePercent = Mathf.RoundToInt(chance * 100);
        int decider = Random.Range(0, 100);

        if (decider < chancePercent)
            amount = Random.Range(min, max + 1);

        return amount;
    }

    private void AddItemsToContainer(DropContainerIdentity container, Item item, int amount)
    {
        Drop drop = new Drop(item, amount, container.gameObject);
        container.drops.Add(drop);
    }

    //Just in case if ever need to force update the container-item data.
    public void ForceUpdateContainerData(GameObject instance, Item itemModified, int modAmount)
    {
        ContainerData data = GetContainerData(instance);
        for (int i = 0; i < data.drops.Count; i++)
        {
            if (data.drops[i].item == itemModified)
            {
                data.drops[i].amount -= modAmount;
                if (data.drops[i].amount <= 0)
                {
                    data.drops.Remove(data.drops[i]);
                    break;
                }
            }
        }
    }

    private string ResourcePath(string _name)
    {
        return "Drop Containers/" + _name;
    }

    public void SaveData()
    {
        JSONObject json = new JSONObject();
        json.Add("dataCount", containerData.Count);
        for (int i = 0; i < containerData.Count; i++)
        {
            ContainerData data = containerData[i];
            json.Add("px" + i, data.px);
            json.Add("py" + i, data.py);
            json.Add("pz" + i, data.pz);
            json.Add("rx" + i, data.rx);
            json.Add("ry" + i, data.ry);
            json.Add("rz" + i, data.rz);
            json.Add("dropCount" + i, data.drops.Count);
            json.Add("rpath" + i, data.resourcePath);
            for (int x = 0; x < data.drops.Count; x++)
            {
                json.Add("item" + i + x, data.drops[x].item.name);
                json.Add("itemCount" + i + x, data.drops[x].amount);
            }
        }
        File.WriteAllText(DataPath(), json.ToString());
    }

    public void LoadData()
    {
        JSONObject json = new JSONObject();
        json = JSON.Parse(File.ReadAllText(DataPath()).ToString()) as JSONObject;
        int dataCount = json["dataCount"];
        for (int i = 0; i < dataCount; i++)
        {
            int dropCount = json["dropCount" + i];
            List<Drop> _drops = new List<Drop>();

            Vector3 pos = new Vector3(json["px" + i], json["py" + i], json["pz" + i]);
            Vector3 rot = new Vector3(json["rx" + i], json["ry" + i], json["rz" + i]);
            string loadedPath = json["rpath" + i];
            string res = ResourcePath(loadedPath);
            GameObject contPrefab = Resources.Load<GameObject>(res);
            GameObject contInstance = Instantiate(contPrefab, pos, Quaternion.Euler(rot), containerHolder);
            DropContainerIdentity id = contInstance.GetComponent<DropContainerIdentity>();

            for (int x = 0; x < dropCount; x++)
            {
                Item thisItem = itemManager.ItemWithName(json["item" + i + x]);
                int amount = json["itemCount" + i + x];
                _drops.Add(new Drop(thisItem, amount, contInstance.gameObject));
            }
            id.drops = _drops;
            ContainerData loadedData = new ContainerData(contInstance, _drops, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, contPrefab.name);
            AddContainerData(id, null, loadedData);
        }
    }

    public void ClearData()
    {
        if (File.Exists(DataPath()))
        {
            File.Delete(DataPath());
        }
    }

    public ContainerData GetContainerData(GameObject instance)
    {
        foreach (ContainerData data in containerData)
        {
            if (data.instance == instance)
                return data;
        }
        return null;
    }

    private string DataPath()
    {
        return Application.persistentDataPath + "/dropcontainerdata.dat";
    }


}
