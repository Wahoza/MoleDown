using UnityEngine;

public class PlaySoundOnAction : IActionComponent
{
    [SerializeField] string clipName;
    [SerializeField] bool spatial;
    public override void DoAction()
    {
        if (spatial)
        {
            AudioManager.Instance.Play3DAudioAtPosition(clipName, transform.position, true);
        }
        else 
        { 
            AudioManager.Instance.Play2DAudio(clipName, true);
        }
    }
}
