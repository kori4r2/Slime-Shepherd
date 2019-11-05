using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioClipsPlayer : MonoBehaviour
{
    private AudioSource source;
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

    // Precisa ser declarado com apenas um parametro para poder ser usado como evento de animação
    public void PlayClip(int index){
        PlayClip(index, false);
    }

    public void PlayClip(int index, bool loop){
        if(audioClips.Count < 1){
            return;
        }
        index = Mathf.Clamp(index, 0, audioClips.Count-1);

        source.Stop();
        source.loop = loop;
        source.clip = audioClips[index];
        source.Play();
    }

    public void StopPlaying(){
        source.Stop();
        source.clip = null;
        source.loop = false;
    }

    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.Stop();
    }
}
