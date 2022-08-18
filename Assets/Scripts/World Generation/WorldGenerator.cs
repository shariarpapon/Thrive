using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;
using System.Threading;

public class WorldGenerator : MonoBehaviour
{
    public Transform environmentHolder;
    public World activeWorld;

    internal List<ObjectData> spawnData = new List<ObjectData>();
    internal List<TileData> tileData = new List<TileData>();

    private List<Vector3> spawnArea = new List<Vector3>();

    internal int maxTotalSpawn;

    private void Start()
    {
        if (File.Exists(DataPath()) == false)
        {
            GenerateWorld();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F12))
        {
            GenerateWorld();
        }
    }

    //CHANGE OBJECT SPAWN POSITION HERE // TRY THREADING
    private List<Vector3> GetSpawnArea(World world)
    {
        List<Vector3> locs = new List<Vector3>();
        for (int x = -world.padding; x < activeWorld.size.x + world.padding; x += world.maxSpawnUnitDistance)
        {
            for (int z = -world.padding; z < activeWorld.size.z + world.padding; z += world.maxSpawnUnitDistance)
            {
                locs.Add(new Vector3(x, 0.0f, z));
            }
        }
        return locs;
    }

    //CHANGE TILE GENERATING METHODS HERE
    private void GenerateTiles(World world)
    {
        print("GENERATING WORLD...");
        for (int x = 0; x <= world.size.x; x += world.tileExtent)
        {
            for (int z = 0; z <= world.size.z; z += world.tileExtent)
            {
                int tileIndex = Random.Range(0, world.tiles.Length);
                Vector3 tilePosition = new Vector3(x, 0.0f, z);
                Quaternion tileRotation = GetTileRotation();
                GameObject tilePrefab = GetTile(world, tileIndex);
                GameObject tile = Instantiate(tilePrefab, tilePosition, tileRotation, transform);
                TileData data = new TileData(tileIndex, tilePosition, tileRotation.eulerAngles, tile);
                tileData.Add(data);
            }
        }
    }

    private Quaternion GetTileRotation()
    {
        Quaternion rot = Quaternion.identity;
        int decider = Random.Range(0, 3);
        switch (decider)
        {
            case 0:
                rot = Quaternion.Euler(0, 0, 0);
                break;
            case 1:
                rot = Quaternion.Euler(0, 90, 0);
                break;
            case 2:
                rot = Quaternion.Euler(0, 180, 0);
                break;
            case 3:
                rot = Quaternion.Euler(0, 270, 0);
                break;
        }
        return rot;
    }
    //TODO: MODIFY WHICH TILE TO SELECT BASED ON PREVIOUS TILE (BIOME)
    private GameObject GetTile(World world, int index)
    {
        GameObject tile = null;
        tile = world.tiles[index];
        return tile;
    }

    private void GenerateDetails(World world)
    {
        print("GENERATING DETAILS...");
        foreach (SpawnObject spawn in world.spawnObjects)
        {
            int quantity = GetSpawnQuantity(spawn.maxSpawn, spawn.frequency, world.universalMinFrequencyFactor);
            for (int f = 0; f <= quantity; f++)
            {
                Vector3 position = RandomSpawnPosition(world);
                Quaternion rotation = RandomSpawnRotation(spawn.multiAngularRotation);
                GameObject original = spawn.objects[Random.Range(0, spawn.objects.Length)];
                GameObject instance = Instantiate(original,position, rotation, environmentHolder);

                ObjectData data = GetObjectData(position, rotation, original.name, spawn.type, instance);
                spawnData.Add(data);
                AddDetailIdentity(instance, data);

                PhysicalInteractor pi = instance.GetComponent<PhysicalInteractor>();
                if (pi != null)
                    pi.interactable = true;
            }
        }
    }

    public void GenerateWorld()
    {
        DestroyWorld();
        InitializeWorldData(activeWorld);
        GenerateTiles(activeWorld);
        GenerateDetails(activeWorld);
    }

    public void DestroyWorld()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) != null)
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < environmentHolder.childCount; i++)
        {
            if (environmentHolder.GetChild(i) != null)
                GameObject.DestroyImmediate(environmentHolder.GetChild(i).gameObject);
        }
        spawnArea = null;
        maxTotalSpawn = 0;
    }

    public void AddDetailIdentity(GameObject instance, ObjectData instanceData)
    {
        WorldDetailIdentity detector = instance.AddComponent(typeof(WorldDetailIdentity)) as WorldDetailIdentity;
        detector.myData = instanceData;
        detector.allowDetection = true;
    }

    private int GetSpawnQuantity(int max, float frequency, int factor = 1)
    {
        int minFreq = Mathf.FloorToInt(max * frequency) * factor;
        return Random.Range(minFreq, max * factor);
    }

    private Quaternion RandomSpawnRotation(bool multiAngular)
    {
        Quaternion rot = Quaternion.Euler(0.0f, Random.Range(0, 270), 0.0f);
        if (multiAngular)
        {
            rot = Quaternion.Euler(Random.Range(0, 270), rot.eulerAngles.y, Random.Range(90, 360));
        }
        return rot;
    }

    private Vector3 RandomSpawnPosition(World world)
    {
        Vector3 pos = spawnArea[Random.Range(0, spawnArea.Count)];
        spawnArea.Remove(pos);
        return pos;
    }

    private ObjectData GetObjectData(Vector3 position, Quaternion rotation, string resourceIdentity, string resourcePath, GameObject instance)
    {
        ObjectData data = new ObjectData();
        data.px = position.x;
        data.py = position.y;
        data.pz = position.z;
        data.rx = rotation.eulerAngles.x;
        data.ry = rotation.eulerAngles.y;
        data.rz = rotation.eulerAngles.z;
        data.resourceID = resourceIdentity;
        data.resourcePath = resourcePath;
        data.instance = instance;
        return data;
    }

    private void InitializeWorldData(World world)
    {
        Debug.Log("Initializing world data...");
        spawnArea = GetSpawnArea(world);
        maxTotalSpawn = 0;
        foreach (SpawnObject so in world.spawnObjects)
        {
            maxTotalSpawn += so.maxSpawn;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) != null)
                Destroy(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < environmentHolder.childCount; i++)
        {
            if (environmentHolder.GetChild(i) != null)
                Destroy(environmentHolder.GetChild(i).gameObject);
        }
    }

    private void ObjectDataToJSON(JSONObject json, ObjectData data, int key)
    {
        json.Add("dpx" + key.ToString(), data.px);
        json.Add("dpz" + key.ToString(), data.pz);
        json.Add("dpy" + key.ToString(), data.py);
        json.Add("drx" + key.ToString(), data.rx);
        json.Add("dry" + key.ToString(), data.ry);
        json.Add("drz" + key.ToString(), data.rz);
        json.Add("detailID" + key.ToString(), data.resourceID);
        json.Add("res" + key.ToString(), data.resourcePath);
    }

    private ObjectData JSONToObjectData(JSONObject json, int key)
    {
        ObjectData myData = new ObjectData
        {
            px = json["dpx" + key.ToString()],
            pz = json["dpz" + key.ToString()],
            py = json["dpy" + key.ToString()],
            rx = json["drx" + key.ToString()],
            ry = json["dry" + key.ToString()],
            rz = json["drz" + key.ToString()],
            resourceID = json["detailID" + key.ToString()],
            resourcePath = json["res" + key.ToString()]
        };
        return myData;
    }

    private void TileDataToJSON(JSONObject json, TileData data, int key)
    {
        json.Add("tpx" + key.ToString(), data.pos.x);
        json.Add("tpz" + key.ToString(), data.pos.z);
        json.Add("tpy" + key.ToString(), data.pos.y);
        json.Add("trx" + key.ToString(), data.eulerRot.x);
        json.Add("try" + key.ToString(), data.eulerRot.y);
        json.Add("trz" + key.ToString(), data.eulerRot.z);
        json.Add("tileIndex" + key.ToString(), data.tileIndex);
    }

    private TileData JSONToTileData(JSONObject json, int key)
    {
        TileData data = null;
        int index = json["tileIndex" + key];
        Vector3 pos = new Vector3(json["tpx" + key], json["tpy" + key], json["tpz" + key]);
        Vector3 eulerRot = new Vector3(json["trx" + key], json["try" + key], json["trz" + key]);
        data = new TileData(index, pos, eulerRot);
        return data;
    }

    private GameObject LoadObjectDataResource(ObjectData data)
    {
        GameObject original = Resources.Load(ResourcePath(data.resourcePath, data.resourceID)) as GameObject;
        GameObject instance = Instantiate(original, new Vector3(data.px, data.py, data.pz), Quaternion.Euler(data.rx, data.ry, data.rz), environmentHolder);
        AddDetailIdentity(instance, data);
        return instance;
    }

    public static string DataPath()
    {
        return Application.persistentDataPath + "/world.dat";
    }

    public static string ResourcePath(string path, string id)
    {
        string p = "World Spawns/" + path + "/" + id;
        return p;
    }

    public void SaveData()
    {
        JSONObject json = new JSONObject();
        json.Add("w_count", spawnData.Count);
        for (int i = 0; i < spawnData.Count; i++)
        {
            ObjectDataToJSON(json, spawnData[i], i);
            IPersistentData pd = spawnData[i].instance.GetComponent<IPersistentData>();
            if (pd != null)
            {
                pd.SaveData(new PDAccesor(i.ToString(), json));
            }
        }
        json.Add("tileCount", tileData.Count);
        for (int t = 0; t < tileData.Count; t++)
        {
            TileDataToJSON(json, tileData[t], t);
        }
        File.WriteAllText(DataPath(), json.ToString());
    }

    public void LoadData()
    {
        InitializeWorldData(activeWorld);
        print("LOADING WORLD DATA...");
        string textData = File.ReadAllText(DataPath());
        JSONObject json = JSON.Parse(textData) as JSONObject;
        int detailCount = json["w_count"];
        for (int i = 0; i < detailCount; i++)
        {
            ObjectData deserialized = JSONToObjectData(json, i);
            GameObject instObject = LoadObjectDataResource(deserialized);
            deserialized.instance = instObject;
            spawnData.Add(deserialized);

            IPersistentData pd = instObject.GetComponent<IPersistentData>();
            if (pd != null)
            {
                pd.LoadData(new PDAccesor(i.ToString(), json));
            }
        }
        int tileCount = json["tileCount"];
        for (int t = 0; t < tileCount; t++)
        {
            TileData data = JSONToTileData(json, t);
            if (data.instance == null)
            {
                GameObject instance = Instantiate(activeWorld.tiles[data.tileIndex], data.pos, Quaternion.Euler(data.eulerRot), transform);
                data.instance = instance;
            }
            tileData.Add(data);
        }
    }

    public void ClearData()
    {
        if (File.Exists(DataPath()))
        {
            print("CLEARING EXISTING WORLD DATA...");
            File.Delete(DataPath());
        }
    }

    #region Structs
    public class TileData
    {
        public int tileIndex;
        public Vector3 pos;
        public Vector3 eulerRot;
        public GameObject instance;

        public TileData(int index, Vector3 _pos, Vector3 rot, GameObject _instance = null)
        {
            tileIndex = index;
            pos = _pos;
            eulerRot = rot;
            instance = _instance;
        }
    }

    [System.Serializable]
    public struct World
    {
        public Size size;
        [Tooltip("All tiles must have the same extent to avoid bad spawn position")]
        public int tileExtent;
        [Range(1, 10)] [Tooltip("Increasing this will spread out the spawn objects in a bigger area")]
        public int padding;
        [Range(1, 10)] [Tooltip("Increasing the unit distance will decrease the randomness of the spawn positions")]
        public int maxSpawnUnitDistance;
        public GameObject[] tiles;
        public int universalMinFrequencyFactor;
        public SpawnObject[] spawnObjects;
     }
    [System.Serializable]
    public struct Size
    {
        public int x;
        public int z;
    }
    [System.Serializable]
    public struct SpawnObject
    {
        public string type;
        [Range(0, 1)]
        public float frequency;
        public int maxSpawn;
        public bool multiAngularRotation;
        public GameObject[] objects;
    }
    [System.Serializable]
    public struct ObjectData
    {
        public GameObject instance;
        public string resourcePath;
        public string resourceID;
        public float px, py, pz;
        public float rx, ry, rz;
    }
    #endregion
}
