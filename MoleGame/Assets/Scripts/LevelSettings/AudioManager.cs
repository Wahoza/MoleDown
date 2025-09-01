using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static AudioManager Instance {  get; private set; }
    [SerializeField] GameObject audioSpatialPrefab;
    [SerializeField] GameObject audioFlatPrefab;
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource ambientPlayer;

    [SerializeField] int spatialSourceCount;
    [SerializeField] int flatSourceCount;

    [SerializeField] float minPitchShift;
    [SerializeField] float maxPitchShift;

    [SerializedDictionary("Name", "Audio")]
    public SerializedDictionary<string, AudioClip> audioClipNameDict;

    [SerializedDictionary("Name", "Music")]
    public SerializedDictionary<string, AudioClip> audioMusicNameDict;
    
    [SerializedDictionary("Name", "Ambient")]
    public SerializedDictionary<string, AudioClip> audioAmbientNameDict;


    private int currentAudioSpatial;
    private int currentAudioFlat;

    private List<AudioSource> audioSourcesSpatial;
    private List<AudioSource> audioSourcesFlat;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }


        Instance = this;
        Setup();
    }

    private void Setup()
    {
        audioSourcesSpatial = new();
        for (int i = 0; i < spatialSourceCount; i++)
        {
            var src = Instantiate(audioSpatialPrefab, transform);
            audioSourcesSpatial.Add(src.GetComponent<AudioSource>());
        }
        currentAudioSpatial = 0;

        audioSourcesFlat = new();
        
        for(int i = 0;i < flatSourceCount; i++)
        {
            var src = Instantiate (audioFlatPrefab, transform);
            audioSourcesFlat.Add(src.GetComponent<AudioSource>());
        }
        currentAudioFlat = 0;
    }

    public void Play3DAudioAtPosition(string clipName, Vector3 position, bool randomPitch) 
    {
        if (!audioClipNameDict.ContainsKey(clipName))
        {
            Debug.Log("Couldnt find clip!");
            return;
        }

        if (randomPitch)
        {
            audioSourcesSpatial[currentAudioSpatial].pitch = Random.Range(minPitchShift, maxPitchShift);
        }
        else
        {
            audioSourcesSpatial[currentAudioSpatial].pitch = 1;
        }

        audioSourcesSpatial[currentAudioSpatial].transform.position = position;
        audioSourcesSpatial[currentAudioSpatial].PlayOneShot(audioClipNameDict[clipName]);

        currentAudioSpatial++;
        currentAudioSpatial = currentAudioSpatial >= audioSourcesSpatial.Count ? 0 : currentAudioSpatial;
    }
    public void Play2DAudio(string clipName, bool randomPitch) 
    {
        if (!audioClipNameDict.ContainsKey(clipName))
        {
            Debug.Log("Couldnt find clip!");
            return;
        }

        if (randomPitch)
        {
            audioSourcesFlat[currentAudioFlat].pitch = Random.Range(minPitchShift, maxPitchShift);
        }
        else
        {
            audioSourcesFlat[currentAudioFlat].pitch = 1;
        }

        audioSourcesFlat[currentAudioFlat].PlayOneShot(audioClipNameDict[clipName]);

        currentAudioFlat++;
        currentAudioFlat = currentAudioFlat >= audioSourcesFlat.Count ? 0 : currentAudioFlat;
    }

    public void SetMusic(string musicName)
    {
        if (audioMusicNameDict.ContainsKey(musicName))
        {
            musicPlayer.clip = audioMusicNameDict[musicName];
        }
    }

    public void SetAmbient(string ambientName)
    {
        if (audioAmbientNameDict.ContainsKey(ambientName))
        {
            ambientPlayer.clip = audioAmbientNameDict[ambientName];
        }
    }
    public void PlayAmbient()
    {
        ambientPlayer.Play();
    }
    public void StopAmbient()
    {
        ambientPlayer.Stop();
    }
    public void PlayMusic()
    {
        musicPlayer.Play();
    }

    public void StopMusic()
    {
        musicPlayer.Pause();
    }
}
