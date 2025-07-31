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
    
    private void Start()
    {
        GetComponent<PlayerActionHandler>().RegisterLandAction(OnLand);
        rb = GetComponent<Rigidbody>();
    }

    void OnLand(float airTime)
    {
        if (airTime > minAirTime && terrainDestructionRate <= Time.time - terrainDestructionRate)
        {
            LevelMarchingPlatform.instance.DeformSphereAt(transform.position + Vector3.up * characterHeightOffset, baseRadius + (airTime - minAirTime) * radiusIncrease, intensity);

            terrainDestructionTimer = Time.time;

            var velocity = rb.linearVelocity;
            velocity.y = 0;
            rb.linearVelocity = velocity;
        }
    }

    private void OnDestroy()
    {
        GetComponent<PlayerActionHandler>().UnregisterLandAction(OnLand);
    }
}
