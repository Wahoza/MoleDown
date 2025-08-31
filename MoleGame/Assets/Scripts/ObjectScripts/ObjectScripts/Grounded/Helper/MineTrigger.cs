using UnityEngine;

public class MineTrigger : MonoBehaviour
{
    [SerializeField] MineObstacle mine;
    [SerializeField] string triggerSoundName;

    int currentTriggers;
    private void OnTriggerEnter(Collider other)
    {
        currentTriggers++;
        AudioManager.Instance.Play3DAudioAtPosition(triggerSoundName, transform.position, true);
    }

    private void OnTriggerExit(Collider other)
    {
        AudioManager.Instance.Play3DAudioAtPosition(triggerSoundName, transform.position, true);

        currentTriggers--;

        Debug.Log(currentTriggers + "Triggerssss ");
        if(currentTriggers <= 0)
        {
            mine.TriggerExplosion();
        }
    }
}
