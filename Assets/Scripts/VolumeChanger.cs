using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider)), ExecuteInEditMode]
public class VolumeChanger : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private FillImage image;
    private Slider slider;

    void Awake(){
        slider = GetComponent<Slider>();
    }

    public void UpdateVolumeValue(){
        mixer.SetFloat("Volume", Mathf.Log10(Mathf.Max((slider.value/slider.maxValue), 0.0001f))*20f);
        if(image){
            image.MaxValue = 1;
            image.CurrentValue = slider.value;
        }
    }
}
