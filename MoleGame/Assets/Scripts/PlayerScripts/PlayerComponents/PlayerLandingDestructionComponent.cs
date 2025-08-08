using UnityEngine;
[RequireComponent(typeof(PlayerActionHandler), typeof(Rigidbody))]
public class PlayerLandingDestructionComponent : MonoBehaviour
{
    [SerializeField] float baseRadius;
    [SerializeField] float radiusIncrease;
    [SerializeField] float characterHeightOffset;
    [SerializeField] float intensity;
    [SerializeField] float minAirTime;

    [SerializeField] float terrainDestructionRate;

    private Rigidbody rb;
    private float terrainDestructionTimer;
    bool cancelLandUntilNext;
    private void Start()
    {
        GetComponent<PlayerActionHandler>().RegisterLandAction(OnLand);
        //GetComponent<PlayerActionHandler>().RegisterGrabbableAction(OnGrab);
        rb = GetComponent<Rigidbody>();
    }

    void OnLand(float airTime)
    {
        if (airTime > minAirTime && terrainDestructionRate <= Time.time - terrainDestructionRate)
        {
            if (cancelLandUntilNext)
            {
                cancelLandUntilNext = false;
                return;
            } 
                

            LevelMarchingPlatform.Instance.DeformSphereAt(transform.position + Vector3.up * characterHeightOffset, baseRadius + (airTime - minAirTime) * radiusIncrease, intensity);

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

    private void OnDestroy()
    {
        GetComponent<PlayerActionHandler>().UnregisterLandAction(OnLand);
    }
}
