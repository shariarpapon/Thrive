using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PI_BerryBush : PhysicalInteractor, IPersistentData
{
    [Space]
    [Header("Grow Quantity")]
    public int min;
    public int max;
    public int capacity;

    [Space]
    [Header("Growth Settings")]
    public int growthSpan = 240;
    public int growth;

    [Space]
    [Header("State Checks")]
    public bool fullGrown = true;
    public bool isGrowing;

    private void Update()
    {
        if (isGrowing && fullGrown == false)
            StartCoroutine(Grow());
    }

    private void Awake()
    {
        if(fullGrown)
            capacity = RandomAmount();
    }

    public override void Interact()
    {
        base.Interact();
        Slot slot = InventoryManager.instance.HasEnoughSpace(item, capacity);
        if (slot != null)
        {
            slot.AddItem(item, capacity);
            FullyHarvest();
            print("harvesting berry");
        }
        else
            print("slot is null");
    }

    private IEnumerator Grow()
    {
        isGrowing = false;
        yield return new WaitForSeconds(1);
        growth++;

        if (growth >= growthSpan)
        {
            FullyGrow();
        }
        else
            isGrowing = true;
    }

    private void FullyGrow()
    {
        fullGrown = true;
        capacity = RandomAmount();
        transform.GetComponent<Collider>().enabled = true;
        if (Berries().activeSelf == false)
            Berries().SetActive(true);
    }

    private void FullyHarvest()
    {
        isGrowing = true;
        fullGrown = false;
        transform.GetComponent<Collider>().enabled = false;
        if (Berries().activeSelf == true)
            Berries().SetActive(false);

        growth = 0;
    }

    public override void SaveData(PDAccesor accesor)
    {
        base.SaveData(accesor);
        JSONObject j = accesor.json;
        j.Add("span" + accesor.key, growthSpan);
        j.Add("cap" + accesor.key, capacity);
        j.Add("growth" + accesor.key, growth);
        JSONBool fg = new JSONBool(fullGrown);
        j.Add("fullGrown" + accesor.key, fg);
    }

    public override void LoadData(PDAccesor accesor)
    {
        base.LoadData(accesor);
        growthSpan = accesor.json["span" + accesor.key];
        capacity = accesor.json["cap" + accesor.key];
        growth = accesor.json["growth" + accesor.key];
        fullGrown = accesor.json["fullGrown" + accesor.key].AsBool;

        if (fullGrown)
        {
            FullyGrow();
        }
        else
        {
            FullyHarvest();
        }
    }

    private int RandomAmount()
    {
        return Random.Range(min , max + 1);
    }

    private GameObject Bush()
    {
        return transform.GetChild(0).gameObject;
    }

    private GameObject Berries()
    {
        return transform.GetChild(0).GetChild(0).gameObject;
    }
}
