using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlacePrefabs : MonoBehaviour
{
    public Transform leftBottomCorner;
    public Transform rightTopCorner;

    public float squareSize = 10;
    public int objectsPerSquare = 50;

    public GameObject[] prefabs;

    public Transform parentTransform;

    public void StartGeneration()
    {
        Vector3 deltaPosition = rightTopCorner.position - leftBottomCorner.position;
        Vector3 bottomLeft = leftBottomCorner.position;
        Vector3 topRight = rightTopCorner.position;

        int sizeX = Mathf.CeilToInt(deltaPosition.x/squareSize);
        int sizeZ = Mathf.CeilToInt(deltaPosition.z/squareSize);

        for(int i = 0; i < sizeX; i++)
        {
            float lowerX = bottomLeft.x + i * squareSize;
            float upperX = 0;
            if(i < sizeX -1)
            {
                upperX = bottomLeft.x + (i+1) * squareSize;
            }
            else
            {
                upperX = topRight.x;
            }
            for(int j = 0; j < sizeZ; j++)
            {
                float lowerZ = bottomLeft.z + j * squareSize;
                float upperZ = 0;
                if(j < sizeZ -1)
                {
                    upperZ = bottomLeft.z + (j+1) * squareSize;
                }
                else
                {
                    upperZ = topRight.z;
                }

                Transform parentTransform = new GameObject("Quadrant " + i + "," + j).transform;

                Vector3 localBottomLeft = new Vector3(lowerX,bottomLeft.y,lowerZ);
                Vector3 localTopRight = new Vector3(upperX,topRight.y,upperZ);
                parentTransform.position = Vector3.Lerp(localBottomLeft,localTopRight,0.5f);
                parentTransform.SetParent(parentTransform);
                GenerateSquare(localBottomLeft, localTopRight,parentTransform);
            }
        }
    }

    protected void GenerateSquare(Vector3 bottomLeft, Vector3 topRight, Transform parentTransform)
    {
        for(int i = 0; i < objectsPerSquare; i++)
        {
            Vector3 position = randomPositionSquare(bottomLeft, topRight);
            GameObject toInstantiante = prefabs[Random.Range(0,prefabs.Length)];

            GameObject instantiated = GameObject.Instantiate(toInstantiante, position, 
            Quaternion.Euler(0,Random.Range(0,360),0),parentTransform);
        }
    }

    protected Vector3 randomPositionSquare(Vector3 bottomLeft, Vector3 topRight)
    {
        Vector3 delta = topRight - bottomLeft;
        Vector3 randomPorcent = new Vector3(Random.Range(0,1),0,Random.Range(0,1));
        Vector3 randomPosition = bottomLeft;
        randomPosition.x += delta.x * randomPorcent.x;
        randomPosition.z += delta.z * randomPorcent.z;
        return  randomPosition;
    }
}

[CustomEditor(typeof(PlacePrefabs))]
public class LevelScriptEditor : Editor 
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