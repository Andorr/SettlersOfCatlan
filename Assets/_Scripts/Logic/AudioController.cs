using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    public bool active = true;
    public AudioClip[] backgroundTracks;

    private LinkedList<AudioClip> backgroundAudio;
    private AudioSource backgroundSource;
    private LinkedListNode<AudioClip> currentNode;
    private Dictionary<string, AudioClip> cache = new Dictionary<string, AudioClip>();


    // Start is called before the first frame update
    void Start()
    {
        backgroundSource = GetComponent<AudioSource>();
        backgroundAudio = new LinkedList<AudioClip>(backgroundTracks);
    }

    // Update is called once per frame
    void Update()
    {
        if (!backgroundSource.isPlaying) {
            currentNode = currentNode == null ? backgroundAudio.First : currentNode.Next ?? backgroundAudio.First;
            backgroundSource.clip = currentNode.Value;
            backgroundSource.Play();
        }
    }

    private AudioClip GetCachedAudioClip(string name) {
        if (cache.ContainsKey(name)) {
            return cache[name];
        } else {
            var sound = Resources.Load(name) as AudioClip;
            cache[name] = sound;
            return sound;
        }
    }

    // Function uses soundname to find audioclip from resources
    public void PlayOnPosition(Vector3 position, string soundname) {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.transform.position = position;
        newSource.spatialBlend = 1f;
        newSource.dopplerLevel = 0;
        AudioClip sound = GetCachedAudioClip(soundname);

        newSource.clip = sound;
        newSource.Play();

        Destroy(newSource, sound.length);
    }

    public void PlayClipOnPosition(Vector3 position, AudioClip clip) {
        if(clip == null) {
            return;
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.transform.position = position;
        newSource.spatialBlend = 1f;
        newSource.dopplerLevel = 0;
        newSource.clip = clip;
        newSource.Play();
        Destroy(newSource, clip.length);
    }

    public void PlayClip(AudioClip clip, float volume = 0.5f) {
        if(clip == null) {
            return;
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = clip;
        newSource.volume = volume;
        newSource.Play();
        Destroy(newSource, clip.length);
    }

    public void PlayClip(string soundname, float volume = 0.5f) {
        PlayClip(GetCachedAudioClip(soundname), volume);
    }

    public void BackgroundMusic(bool shouldPlay) {
        if(shouldPlay && !backgroundSource.isPlaying) {
            backgroundSource.Play();
        } else if (!shouldPlay) {
            backgroundSource.Stop();
        }
    }
}
