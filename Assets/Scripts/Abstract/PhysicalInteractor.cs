using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PhysicalInteractor : MonoBehaviour
{
    protected float interactionRadius = 3;

    internal bool interactable = false;
    public Item item;

    private void OnMouseDown()
    {
        float dist = Vector3.Distance(PlayerInfo.Player().position, transform.position);
        if (dist > interactionRadius)
            return;

        if (interactable)
            Interact();
    }

    public virtual void Interact()
    {
        print("Interacting with " + gameObject.name);
    }

    public virtual void SaveData(PDAccesor accesor)
    {
        SimpleJSON.JSONBool inact = new SimpleJSON.JSONBool(interactable);
        accesor.json.Add("pirootinteractable" + accesor.key, inact);
    }

    public virtual void LoadData(PDAccesor accesor)
    {
        interactable = accesor.json["pirootinteractable" + accesor.key].AsBool;
    }

    public virtual void ClearData(string path)
    {
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    }

    public virtual string DataPath()
    {
        throw new KeyNotFoundException();
    }


    //PLACE HOLDER FUNCTIONS
}
