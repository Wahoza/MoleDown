using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }
    enum Weather
    {
        Sunny = 0,
        Rainy = 1,
        Windy = 2,
    }
    [Header("Windy")]
    [SerializeField] GameObject windObject;
    [SerializeField] Material windyWaterMat;
    [SerializeField] float spawnWeightWindy;
    [SerializeField] string ambientNameWindy;

    [Header("Rainy")]
    [SerializeField] GameObject rainObject;
    [SerializeField] Material rainyWaterMat;
    [SerializeField] float spawnWeightRainy;
    [SerializeField] string ambientNameRainy;

    [Header("Sunny")]
    [SerializeField] GameObject sunObject;
    [SerializeField] Material sunnyWaterMat;
    [SerializeField] float spawnWeightSunny;

    [Header("OtherSettings")]
    [SerializeField] List<MeshRenderer> waters;
    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetRandomWeather()
    {
        float totalWeatherWeight = spawnWeightWindy + spawnWeightRainy + spawnWeightSunny;
        float spawnObject = Random.value * totalWeatherWeight;

        Debug.Log("Spawn result " +spawnObject);

        Weather result = Weather.Sunny;

        if(spawnObject <= spawnWeightWindy)
        {
            result = Weather.Windy;
        }
        else if(spawnObject <= spawnWeightRainy + spawnWeightWindy)
        {
            result = Weather.Rainy;
        }
        Debug.Log(result.ToString() + "This the weather");
            SetWeather(result);
    }

    void SetWeather(Weather weather)
    {
        windObject.SetActive(false);
        rainObject.SetActive(false);
        sunObject.SetActive(false);

        switch (weather)
        {
            case Weather.Sunny:
                foreach (var water in waters)
                {
                    water.material = sunnyWaterMat;
                }
                sunObject.SetActive(true);
                AudioManager.Instance.StopAmbient();

                break;

            case Weather.Windy:
                foreach (var water in waters)
                {
                    water.material = windyWaterMat;
                }
                windObject.SetActive(true);
                AudioManager.Instance.SetAmbient(ambientNameWindy);
                AudioManager.Instance.PlayAmbient();
                break;

            case Weather.Rainy:
                foreach (var water in waters)
                {
                    water.material = rainyWaterMat;
                }
                rainObject.SetActive(true);
                AudioManager.Instance.SetAmbient(ambientNameRainy);
                AudioManager.Instance.PlayAmbient();
                break;
        }
    }
}
