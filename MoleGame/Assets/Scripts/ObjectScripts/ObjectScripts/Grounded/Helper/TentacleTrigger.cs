using UnityEngine;

public class TentacleTrigger : MonoBehaviour
{
    [SerializeField] TentacleObstacle tentacle;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerStateComponent>() is var ps && ps != null && !ps.IsDead())
        {
            tentacle.DoAttack(other.transform);
        }
    }
}
