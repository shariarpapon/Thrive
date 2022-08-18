using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo info;

    private void Awake()
    {
        if (info == null)
            info = this;
        else
            Destroy(gameObject);
    }

    public static Transform Player()
    {
        return info.transform;
    }
}
