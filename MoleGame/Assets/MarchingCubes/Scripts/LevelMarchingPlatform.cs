using UnityEngine;

public class LevelMarchingPlatform : MarchingWorld
{
    public static LevelMarchingPlatform Instance {get; private set;}

    public bool debugToggle = false;
    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    public void TestToggle()
    {
        debugToggle = !debugToggle;
        base.GenerateWorld();
    }

    // Update is called once per frame
    void Update()
    {
        if (debugToggle)
        {
            TestToggle();
        }
    }
}
