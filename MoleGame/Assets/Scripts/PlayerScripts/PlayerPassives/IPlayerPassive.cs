using UnityEngine;

//[CreateAssetMenu(fileName = "IPlayerPassive", menuName = "Scriptable Objects/IPlayerPassive")]
public class IPlayerPassive : ScriptableObject
{
    public PlayerPassiveComponent player {get; private set;}

    public void SetPlayer(PlayerPassiveComponent player)
    {
        this.player = player;
    }
    public  virtual void OnStart() { }
    public  virtual void OnUpdate() { }
    public  virtual void OnEnd() { }
}
