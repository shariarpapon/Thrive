using System.Collections;
using UnityEngine;
using SimpleJSON;
using UnityEngine.ParticleSystemJobs;
using UnityEditor;

public abstract class PI_Fire : PhysicalInteractor, IPersistentData
{
    public GameObject fireUI;
    public GameObject extinguishSight;
    internal GameObject activeSight;

    internal bool fireExists = true;
    internal bool burn = true;

    public float burnedFuel = 0.0f;
    internal float burnRate = 1;
    internal float maxBurnCapacity = 100;

    public AnimationCurve fireSizeCurve;

    private void Awake()
    {
        maxBurnCapacity = item.durability;
    }

    private void Update()
    {
        if (fireExists && interactable == true)
        {
            if (burn == true)
                StartCoroutine(BurnFuel(burnRate));
        }
    }

    public override void Interact()
    {
        base.Interact();
        print("Going through PI_Fire");

        if (fireUI)
        {
            fireUI.SetActive(true);
            InitializeUI();
        }
    }

    private void InitializeUI()
    {
        print("Init UI");
    }

    private IEnumerator BurnFuel(float delta)
    {
        burn = false;

        yield return new WaitForSeconds(delta);

        burnedFuel++;

        float sizeEva = fireSizeCurve.Evaluate(burnedFuel / maxBurnCapacity);
        ParticleRoot().transform.localScale = new Vector3(sizeEva, sizeEva, sizeEva);

        if (burnedFuel >= maxBurnCapacity)
        {
            Extinguish();
        }

        burn = true;
    }

    public void Extinguish()
    {
        FireRoot().SetActive(false);
        burnedFuel = 0.0f;

        if (extinguishSight)
        {
            activeSight = Instantiate(extinguishSight, transform.GetChild(0).position, RandomYRotation(), transform);
        }
        else
            activeSight = null;

        fireExists = false;
    }

    public void Ignite()
    {
        FireRoot().SetActive(true);
        if (activeSight)
        {
            Destroy(activeSight);
            activeSight = null;
        }
        fireExists = true;
    }

    public GameObject ParticleRoot()
    {
        return transform.GetComponentInChildren<AudioSource>().transform.GetChild(0).gameObject;
    }

    public GameObject FireRoot()
    {
        return transform.GetComponentInChildren<AudioSource>().gameObject;
    }

    public Quaternion RandomYRotation()
    {
        return Quaternion.Euler(0, Random.Range(0, 270), 0);
    }

    public override void SaveData(PDAccesor accesor)
    {
        base.SaveData(accesor);
        JSONObject json = accesor.json;
        string key = accesor.key;
        json.Add("pdafuel" + key, burnedFuel);
        json.Add("pdacapacity" + key, maxBurnCapacity);
        JSONBool frext = new JSONBool(fireExists);
        JSONBool brn = new JSONBool(true);
        json.Add("pdaburn" + accesor.key, brn);
        json.Add("pdafire" + accesor.key, frext);

    }

    public override void LoadData(PDAccesor accesor)
    {
        base.LoadData(accesor);
        JSONObject json = accesor.json;
        fireExists = json["pdafire" + accesor.key].AsBool;
        burn = json["pdaburn" + accesor.key].AsBool;
        burnedFuel = json["pdafuel" + accesor.key];
        maxBurnCapacity = json["pdacapacity" + accesor.key];

        if (fireExists == false)
            Extinguish();
    }

    public override void ClearData(string path)
    {
        base.ClearData(path);
    }
}
