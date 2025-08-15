using UnityEngine;


public class LimitFPSInMain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] int targetRefreshRate = 144;
    void Start()
    {
        Application.targetFrameRate = targetRefreshRate;
    }


}
