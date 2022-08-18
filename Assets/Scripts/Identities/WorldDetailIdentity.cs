using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDetailIdentity : Identity
{
    public bool allowDetection;
    public WorldGenerator.ObjectData myData;

    private void OnDestroy()
    {
        if (allowDetection)
        {
            WorldGenerator wg = FindObjectOfType<WorldGenerator>();
            if (wg != null)
                wg.spawnData.Remove(myData);
            else
                print("World Generator has been destroyed");
        }
    }

}
