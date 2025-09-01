using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixer testMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private Slider SFXSlider;

    [SerializeField] private AudioSource testAudioSource;
    [SerializeField] private float minDelayBetweenBlips;

    private float blipDelayTimer;

    float MasterVolume = 0.4f;
    float MusicVolume = 0.6f;
    float SFXVolume = 0.7f;
    float AmbienceVolume = 0.3f;

    string masterExposed = "MasterVol";
    string musicExposed = "MusicVol";
    string sfxExposed = "SFXVol";
    string ambientExposed = "AmbienceVol";
    string testExposed = "TestMasterVol";

    public enum testSliderSetType
    {
        Master = 0,
        SFX = 1,
        Music = 2,
        Ambiance = 3
    }
    private void Start()
    {
        testAudioSource.enabled = false;
        blipDelayTimer = Time.time;

        if ((PlayerPrefs.HasKey(musicExposed) && PlayerPrefs.HasKey(sfxExposed) && PlayerPrefs.HasKey(masterExposed)))
        {
            LoadVolume();
        }
        else
        {
            masterSlider.value = MasterVolume;
            musicSlider.value = MusicVolume;
            SFXSlider.value = SFXVolume;
            ambienceSlider.value = AmbienceVolume;

            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
            SetAmbienceVolume();
        }

        testAudioSource.enabled = true;
    }
    public void PlayTestBlip(int slider)
    {
        if (blipDelayTimer < Time.time)
        {
            float volume = 0.0001f;

            if (slider == (int)testSliderSetType.Master)
            {
                volume = Mathf.Log10(masterSlider.value) * 20;
            }
            else if (slider == (int)testSliderSetType.SFX)
            {
                volume = Mathf.Log10(SFXSlider.value) * 20;
            }
            else if (slider == (int)testSliderSetType.Music)
            {
                volume = Mathf.Log10(musicSlider.value) * 20;
            }
            else if (slider == (int)testSliderSetType.Ambiance)
            {
                volume = Mathf.Log10(ambienceSlider.value) * 20;
            }

            testMixer.SetFloat(testExposed, volume);

            testAudioSource.Play();

            blipDelayTimer = Time.time + minDelayBetweenBlips;
        }
    }
    public void SetAmbienceVolume()
    {
        AmbienceVolume = ambienceSlider.value;
        mixer.SetFloat(ambientExposed, Mathf.Log10(AmbienceVolume) * 20);
        PlayerPrefs.SetFloat(ambientExposed, AmbienceVolume);
    }
    public void SetMasterVolume()
    {
       MasterVolume = masterSlider.value;
       mixer.SetFloat(masterExposed, Mathf.Log10(MasterVolume) * 20);
       PlayerPrefs.SetFloat(masterExposed, MasterVolume);
    }
    public void SetMusicVolume()
    {
       MusicVolume = musicSlider.value;
       mixer.SetFloat(musicExposed, Mathf.Log10(MusicVolume) * 20);
       PlayerPrefs.SetFloat(musicExposed, MusicVolume);
    }

    public void SetSFXVolume()
    {
        SFXVolume = SFXSlider.value;
        mixer.SetFloat(sfxExposed, Mathf.Log10(SFXVolume) * 20);
        PlayerPrefs.SetFloat(sfxExposed, SFXVolume);
    }
    public void LoadVolume()
    {
        MasterVolume = PlayerPrefs.GetFloat(masterExposed);
        masterSlider.value = MasterVolume;
        MusicVolume = PlayerPrefs.GetFloat(musicExposed);
        musicSlider.value = MusicVolume;
        SFXVolume = PlayerPrefs.GetFloat(sfxExposed);
        SFXSlider.value = SFXVolume;
        AmbienceVolume = PlayerPrefs.GetFloat(ambientExposed);
        ambienceSlider.value = AmbienceVolume;

        SaveVolumeSettings();
    }
    public void SaveVolumeSettings()
    {
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
        SetAmbienceVolume();
    }
}
