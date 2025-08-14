using UnityEngine;

public class PlayerAudioComponent : IPlayerComponent
{
    [SerializeField] string grabAudioName;
    [SerializeField] string jumpAudioName;
    [SerializeField] string landAudioName;
    [SerializeField] string dieAudioName;

    internal override void Start()
    {
        GetComponent<PlayerActionHandler>().RegisterGrabAction((b) => PlayAudioOnEvent(grabAudioName));
        GetComponent<PlayerActionHandler>().RegisterJumpAction(() => PlayAudioOnEvent(jumpAudioName));
        GetComponent<PlayerActionHandler>().RegisterLandAction((f) => PlayAudioOnEvent(landAudioName));
        GetComponent<PlayerActionHandler>().RegisterDeathAction(() => PlayAudioOnEvent(dieAudioName));
    }

    void PlayAudioOnEvent(string audioName)
    {
        AudioManager.Instance?.Play3DAudioAtPosition(audioName, transform.position, true);
    }

    internal override void OnDestroy()
    {
        base.OnDestroy();

        GetComponent<PlayerActionHandler>().UnregisterGrabAction((b) => PlayAudioOnEvent(grabAudioName));
        GetComponent<PlayerActionHandler>().UnregisterJumpAction(() => PlayAudioOnEvent(jumpAudioName));
        GetComponent<PlayerActionHandler>().UnregisterLandAction((f) => PlayAudioOnEvent(landAudioName));
        GetComponent<PlayerActionHandler>().UnregisterDeathAction(() => PlayAudioOnEvent(dieAudioName));
    }
}
