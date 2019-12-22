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
        AudioClip sound = GetCachedAudioClip(soundname);

        newSource.clip = sound;
        newSource.Play();

        Destroy(newSource, sound.length);
    }
}
