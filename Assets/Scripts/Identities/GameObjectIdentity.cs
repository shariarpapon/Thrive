using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectIdentity : Identity
{
    public GameObjectData selfData;
    private BuildingManager bm;

    private void Awake()
    {
        bm = FindObjectOfType<BuildingManager>();
    }

    private void OnDestroy()
    {
        if (selfData == null)
            return;

        if (bm != null && isNeeded == false)
            bm.data.Remove(selfData);
    }
}
