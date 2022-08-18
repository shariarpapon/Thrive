using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = target as DataManager;

        DrawDefaultInspector();
        if (GUILayout.Button("Clear Game Data"))
        {
            script.ClearGameData();
        }
    }
}
