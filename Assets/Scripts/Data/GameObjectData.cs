using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectData
{
    public string ID;
    public Vector3 position;
    public Vector3 rotation;
    public GameObject instance;

    public GameObjectData(string id, Vector3 pos, Vector3 rot, GameObject inst)
    {
        ID = id;
        position = pos;
        rotation = rot;
        instance = inst;
    }

    public static void Identify(GameObject instance, GameObjectData data)
    {
        GameObjectIdentity id = instance.AddComponent(typeof(GameObjectIdentity)) as GameObjectIdentity;
        id.selfData = data;
    }
}
