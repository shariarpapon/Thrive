using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    public Camera mainCam;
    public Transform buildingHolder;
    public Transform previewHolder;
    public bool showing = false;
    [Range(10, 100)]
    public float maxActiveRadius = 30;
    public float buildRadius;

    private GameObject activePreview;
    private GameObject activePrefab;
    private Slot activeSlot;

    private Ray ray;
    private RaycastHit hit;
    private Transform player;
    private WorldGenerator world;

    public List<GameObjectData> data = new List<GameObjectData>();

    private void Awake()
    {
        if (instance == null)
            instance = this;

        player = FindObjectOfType<FPController>().transform;
        world = FindObjectOfType<WorldGenerator>();
    }

    private void Update()
    {
        Initiate();
    }

    private void CheckRotation()
    {
        if (activePreview == null)
            return;

        Transform tf = activePreview.transform;

        if (Input.GetKeyDown(KeyCode.E))
        {
            tf.rotation = Quaternion.Euler(tf.rotation.eulerAngles.x, tf.rotation.eulerAngles.y + 45, tf.rotation.eulerAngles.z);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            tf.rotation = Quaternion.Euler(tf.rotation.eulerAngles.x, tf.rotation.eulerAngles.y - 45, tf.rotation.eulerAngles.z);
        }
    }

    private void Initiate()
    {
        if (showing == true)
        {
            if (InventoryManager.instance.InventoryIsOpen())
                return;

            CheckRotation();
            ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, maxActiveRadius, LayerMask.GetMask("Ground")))
            {
                activePreview.transform.position = hit.point;

                if (Input.GetMouseButtonDown(0))
                {
                    if (CanPlace())
                    {
                        EndPreview(true);
                    }
                    else
                        EndPreview(false);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    StopShowing(false);
                    EndPreview(false);
                }
            }
        }
    }

    private void EndPreview(bool build = false)
    {
        if (build == false)
        {
            return;
        }
        else
        {
            if (activeSlot.slotItem.build_SFX == true)
                AudioManager.instance.InstantPlay(activeSlot.slotItem.buildSFX);

            GameObjectData d = new GameObjectData(activePrefab.name, activePreview.transform.position, activePreview.transform.rotation.eulerAngles, activePreview);
            data.Add(d);
            activePreview.transform.SetParent(buildingHolder);
            GameObjectData.Identify(activePreview, d);
            activeSlot.ClearSlot();

            PhysicalInteractor PI = activePreview.GetComponent<PhysicalInteractor>();
            if (PI != null)
                PI.interactable = true;

            StopShowing(true);
            return;
        }
    }

    private void StopShowing(bool keep)
    {
        if (keep == false)
        {
            Destroy(activePreview);
        }
        activePrefab = null;
        activePreview = null;
        showing = false;
    }

    public void InitiatePreview(GameObject prefab, Slot slot)
    {
        if (showing == false)
        {
            InventoryManager.instance.Close();
            activePrefab = prefab;
            activePreview = Instantiate(prefab, hit.point, Quaternion.identity, previewHolder) as GameObject;
            activeSlot = slot;
            showing = true;
        }
    }

    private bool CanPlace()
    {
        bool placeable = false;
        float dist = Vector3.Distance(player.position, activePreview.transform.position);
        if (dist <= buildRadius)
        {
            placeable = true; //DO all checks and make sure item is avaiable in inventory
        }
        return placeable;
    }

    public void SaveData()
    {
        JSONObject js = new JSONObject();
        js.Add("dataCount", data.Count);

        for (int i = 0; i < data.Count; i++)
        {
            GameObjectData d = data[i];
            js.Add("px" + i, d.position.x);
            js.Add("py" + i, d.position.y);
            js.Add("pz" + i, d.position.z);
            js.Add("rx" + i, d.rotation.x);
            js.Add("ry" + i, d.rotation.y);
            js.Add("rz" + i, d.rotation.z);
            js.Add("id" + i, d.ID);

            PhysicalInteractor pd = data[i].instance.GetComponent<PhysicalInteractor>();
            if (pd != null)
            {
                pd.SaveData(new PDAccesor(i.ToString(), js));
            }
        }
        System.IO.File.WriteAllText(DataPath(), js.ToString());
    }

    public void LoadData()
    {
        string textData = System.IO.File.ReadAllText(DataPath());
        JSONObject js = JSON.Parse(textData) as JSONObject;
        int count = js["dataCount"];
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(js["px" + i],  js["py" + i],  js["pz" + i]);
            Vector3 rot = new Vector3(js["rx" + i], js["ry" + i], js["rz" + i]);
            string id = js["id" + i];
            GameObject instance = Instantiate((GameObject)Resources.Load(ResourcePath(id)), pos, Quaternion.Euler(rot.x, rot.y, rot.z), buildingHolder) as GameObject;
            GameObjectData myData = new GameObjectData(id, pos, rot, instance);
            GameObjectData.Identify(instance, myData);
            data.Add(myData);
            CheckComponents(instance);

            PhysicalInteractor pd = instance.GetComponent<PhysicalInteractor>();
            if (pd != null)
            {
                pd.LoadData(new PDAccesor(i.ToString(), js));
            }
        }
    }

    private void CheckComponents(GameObject instance)
    {
        PhysicalInteractor interactor = instance.GetComponent<PhysicalInteractor>();
        if (interactor != null)
            interactor.interactable = true;
    }

    public void ClearData()
    {
        if (System.IO.File.Exists(DataPath()))
        {
            System.IO.File.Delete(DataPath());
        }
    }
    private static string ResourcePath(string key)
    {
        return "Buildings/" + key;
    }

    private static string DataPath()
    {
        return Application.persistentDataPath + "/crtdbuildings.dat";
    }
}
