using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class DropContainerIdentity : Identity
{
    private HarvestManager harvestManager;
    private DropContainerUIEvents events;
    private DropContainerManager manager;
    private Transform player;

    internal List<Drop> drops = new List<Drop>();
    public const float interactionRadius = 3.4f;

    private void Awake()
    {
        harvestManager = FindObjectOfType<HarvestManager>();
        manager = FindObjectOfType<DropContainerManager>();
        events = FindObjectOfType<DropContainerUIEvents >();
        player = FindObjectOfType<FPController>().transform;
    }

    private void OnMouseDown()
    {
        if (events.containerIsOpen == true)
            return;
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist > interactionRadius)
            return;

        manager.UpdateContainer(this);
        events.Open();
        events.Clear();
        foreach (Drop _drop in drops)
        {
            events.AddItemButtons(_drop);
        }
    }

    public DCButtonIdentity GetDCID(Drop d)
    {
        DropContainerUIEvents ui = FindObjectOfType<DropContainerUIEvents>();
        for (int i = 0; i < ui.transform.childCount; i++)
        {
            DCButtonIdentity id = ui.transform.GetChild(i).transform.GetComponent<DCButtonIdentity>();
            if (id.drop.item == d.item)
                return id;
        }
        return null;
    }

    public void RemoveDrop(Drop _drop)
    {
        drops.Remove(_drop);

        if (drops.Count <= 0)
        {
            events.Close();
            Destroy(gameObject);
        }
    }

    public Drop GetDropFromList(Drop drop)
    {
        for (int i = 0; i < drops.Count; i++)
        {
            if (drops[i].item == drop.item)
                return drops[i];
        }
        return null;
    }

    private void OnDestroy()
    {
        ContainerData data = harvestManager.GetContainerData(gameObject);
        if(data != null)
            harvestManager.containerData.Remove(data);
    }
}
