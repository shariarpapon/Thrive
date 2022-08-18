using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class DropContainerManager : MonoBehaviour
{
    public static DropContainerManager instance;
    public DropContainerIdentity activeContainer;

    private void Awake()
    {
        if (instance != null)
            return;
        else
            instance = this;
    }

    public void UpdateContainer(DropContainerIdentity container)
    {
            activeContainer = container;
    }

    public void SaveContainerData() { }
    public void LoadContainerData() { }
    public void ClearContainerData() { }
}
