using UnityEngine;

public class LevelMarchingPlatform : MarchingWorld
{
    public static LevelMarchingPlatform instance {get; private set;}

    public bool debugToggle = false;
    public override void Start()
    {
        if(instance == null)
        {
            instance = this;
            base.Start();
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
