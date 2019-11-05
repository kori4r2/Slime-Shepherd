using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FillImage : MonoBehaviour
{
    private Image image = null;
    [SerializeField] private Text text;
    private float maxValue;
    public float MaxValue{
        get => maxValue;
        set {
            maxValue = Mathf.Clamp(value, 0, float.MaxValue);
        }
    }
    private float currentValue;
    public float CurrentValue{
        get => currentValue;
        set {
            currentValue = Mathf.Clamp(value, 0, maxValue);
        }
    }

    public void Reset(){
        image = GetComponent<Image>();
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
    }
    
    void Awake()
    {
        if(image == null){
            image = GetComponent<Image>();
        }
        image.fillAmount = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Debug.Log("currentValue = " + currentValue + "; maxValue = " + maxValue);
        if(maxValue == 0){
            image.fillAmount = 0;
        }else{
            image.fillAmount = (currentValue/maxValue);
        }
        if(text != null){
            text.text = currentValue + "/" + maxValue;
        }
    }
}
