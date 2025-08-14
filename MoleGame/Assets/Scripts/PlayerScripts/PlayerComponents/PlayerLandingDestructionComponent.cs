using UnityEngine;
[RequireComponent(typeof(PlayerActionHandler), typeof(Rigidbody))]
public class PlayerLandingDestructionComponent : IPlayerComponent
{
    [SerializeField] float baseRadius;
    [SerializeField] float radiusIncrease;
    [SerializeField] float characterHeightOffset;
    [SerializeField] float intensity;
    [SerializeField] float minAirTime;
    [SerializeField] float terrainDestructionRate;


    internal float currentBaseRadius;
    internal float currentRadiusIncrease;
    internal float currentCharacterHeightOffset;
    internal float currentIntensity;
    internal float currentMinAirTime;
    internal float currentTerrainDestructionRate;

    private Rigidbody rb;
    private float terrainDestructionTimer;
    bool cancelLandUntilNext;
    internal override void Start()
    {
        GetComponent<PlayerActionHandler>().RegisterLandAction(OnLand);
        //GetComponent<PlayerActionHandler>().RegisterGrabbableAction(OnGrab);
        rb = GetComponent<Rigidbody>();

        base.Start();
    }

    internal override void ResetStatsOnRespawn()
    {
        currentBaseRadius = baseRadius;
        currentRadiusIncrease         = radiusIncrease;
        currentCharacterHeightOffset  = characterHeightOffset;
        currentIntensity              = intensity;
        currentMinAirTime             = minAirTime;
        currentTerrainDestructionRate = terrainDestructionRate;
    }
    void OnLand(float airTime)
    {
        if (airTime > currentMinAirTime && currentTerrainDestructionRate <= Time.time - currentTerrainDestructionRate)
        {
            if (cancelLandUntilNext)
            {
                cancelLandUntilNext = false;
                return;
            } 
                

            LevelMarchingPlatform.Instance.DeformSphereAt(transform.position + Vector3.up * currentCharacterHeightOffset, currentBaseRadius + (airTime - minAirTime) * currentRadiusIncrease, currentIntensity);

            terrainDestructionTimer = Time.time;

            var velocity = rb.linearVelocity;
            velocity.y = 0;
            rb.linearVelocity = velocity;
        }
    }

    //void OnGrab(bool grabbed)
    //{
    //    cancelLandUntilNext = true;
    //}

    internal override void OnDestroy()
    {
        base.OnDestroy();
        GetComponent<PlayerActionHandler>().UnregisterLandAction(OnLand);
    }
}
