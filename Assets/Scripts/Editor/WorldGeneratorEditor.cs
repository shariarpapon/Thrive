using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var targ = target as WorldGenerator;

        DrawDefaultInspector();
        if (GUILayout.Button("Generate World"))
        {
            targ.GenerateWorld();
        }
        if (GUILayout.Button("Destroy World"))
        {
            targ.DestroyWorld();
        }
    }
}

