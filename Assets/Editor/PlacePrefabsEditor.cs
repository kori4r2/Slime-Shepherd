using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlacePrefabs))]
public class PlacePrefabsEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlacePrefabs myTarget = (PlacePrefabs)target;

        if(GUILayout.Button("Instantiate prefabs"))
        {
            myTarget.StartGeneration();
        }
    }
}