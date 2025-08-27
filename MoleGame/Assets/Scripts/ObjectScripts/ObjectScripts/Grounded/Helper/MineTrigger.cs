using UnityEngine;

public class MineTrigger : MonoBehaviour
{
    [SerializeField] MineObstacle mine;
    int currentTriggers;
    private void OnTriggerEnter(Collider other)
    {
        currentTriggers++;
    }

    private void OnTriggerExit(Collider other)
    {
        currentTriggers--;

        Debug.Log(currentTriggers + "Triggerssss ");
        if(currentTriggers <= 0)
        {
            mine.TriggerExplosion();
        }
    }
}
