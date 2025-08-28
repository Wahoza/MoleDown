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

    [Header("Rainy")]
    [SerializeField] GameObject rainObject;
    [SerializeField] Material rainyWaterMat;
    [SerializeField] float spawnWeightRainy;

    [Header("Sunny")]
    [SerializeField] GameObject sunObject;
    [SerializeField] Material sunnyWaterMat;
    [SerializeField] float spawnWeightSunny;

    [Header("OtherSettings")]
    [SerializeField] MeshRenderer water;
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
                water.material = sunnyWaterMat;
                sunObject.SetActive(true);
                break;

            case Weather.Windy:
                water.material = windyWaterMat;
                windObject.SetActive(true);
                break;

            case Weather.Rainy:
                water.material = rainyWaterMat;
                rainObject.SetActive(true);
                break;
        }
    }
}
