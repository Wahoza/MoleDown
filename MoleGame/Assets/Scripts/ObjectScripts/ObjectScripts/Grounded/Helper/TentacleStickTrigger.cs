using UnityEngine;

public class TentacleStickTrigger : MonoBehaviour
{
    [SerializeField] TentacleObstacle tentacle;
    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerActionHandler>() != null)
        {
            tentacle.DoStick(other.GetComponent<Rigidbody>());
        }
    }
}
