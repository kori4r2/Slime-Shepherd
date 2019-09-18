using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyenimator : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;
    public int rightEyeMaterialSlot = 2;
    public int leftEyeMaterialSlot = 4;
    public int eyesCount = 8;

    protected int currentRightEye = 0;
    protected int currentLeftEye = 0;
    protected Material materialRightEye = null;
    protected Material materialLeftEye = null;

    public void Start()
    {
        if(meshRenderer == null)
        {
            meshRenderer = GetComponent<SkinnedMeshRenderer>();
            if(meshRenderer == null)
                meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        List<Material> materials = new List<Material>();
        meshRenderer.GetMaterials(materials);

        if(rightEyeMaterialSlot < materials.Count)
            materialRightEye = materials[rightEyeMaterialSlot];
        else
            Debug.LogWarning("Right eye material index invalid");

        if(leftEyeMaterialSlot < materials.Count)
            materialLeftEye = materials[leftEyeMaterialSlot];
        else
            Debug.LogWarning("Left eye material index invalid");

        currentLeftEye = 0;
        currentRightEye = 0;
        UpdateEyes();


    }

    protected void UpdateEyes()
    {
        float deltaEye = 1.0f / eyesCount;

        if(materialRightEye != null)
        {
            materialRightEye.SetTextureOffset("_MainTex", new Vector2(deltaEye * currentRightEye,0));
        }

        if(materialLeftEye != null)
        {
            materialLeftEye.SetTextureOffset("_MainTex", new Vector2(deltaEye * currentLeftEye,0));
        }
    }

    public void SetLeftEyeIndex(int id)
    {
        if(id < eyesCount)
        {
            currentLeftEye = id;
        
            UpdateEyes();
        }
    }

    public void SetRightEyeIndex(int id)
    {
        if(id < eyesCount)
        {
            currentRightEye = id;
        
            UpdateEyes();
        }
    }

    public void SetBothEyes(int id)
    {
        if(id < eyesCount)
        {
            currentRightEye = id;
            currentLeftEye = id;

            UpdateEyes();
        }
    }

    public void SetEyes(int left, int right)
    {
        if(left < eyesCount)
        {
            currentLeftEye = left;
        }

        if(right < eyesCount)
        {
            currentRightEye = right;
        }

        UpdateEyes();
    }

}
