using UnityEngine;

[CreateAssetMenu(fileName = "CrownPassive", menuName = "Scriptable Objects/CrownPassive")]

public class CrownedPassive : IPlayerPassive
{
    [SerializeField] float pointsPerSecond = 1.0f;

    PlayerConfig myPlayer;
    internal bool isCrowned { get; private set; }


    public override void OnStart()
    {
        base.OnStart();

        myPlayer = player.GetComponent<PlayerInputHandler>().playerConfig;

        player.GetComponent<PlayerActionHandler>().RegisterPushAction(OnPlayerPush);
        player.GetComponent<PlayerActionHandler>().RegisterDeathAction(OnPlayerDie);

        isCrowned = false;
        player.GetComponent<PlayerModelControllerComponent>().HideWinObjects();
    }
    public void SetIsCrowned(bool isCrowned)
    {
        this.isCrowned = isCrowned;

        if (isCrowned)
        {
            player.GetComponent<PlayerModelControllerComponent>().ShowWinObjects();

            myPlayer.pointsThisRound++; //Add an extra point for aquiring the crown for a more snappy feel
        }
        else
        {
            player.GetComponent<PlayerModelControllerComponent>().HideWinObjects();
        }
    }

    private void OnPlayerPush(Transform target)
    {
        if (target.GetComponent<PlayerPassiveComponent>() is var playerPassive && playerPassive != null)
        {
            if (playerPassive.GetPassive<CrownedPassive>() is var crownedOtherPassive && crownedOtherPassive != null)
            {
                if (crownedOtherPassive.isCrowned)
                {
                    GameManager.Instance.RequestCrownTransfer(player.transform);
                }
            }
        }
    }

    private void OnPlayerDie()
    {
        if (isCrowned)
        {
            GameManager.Instance.RequestCrownSpawn();
            isCrowned = false;
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (isCrowned)
        {
            myPlayer.pointsThisRound += Time.deltaTime * pointsPerSecond;
        }
    }   
    public override void OnEnd()
    {
        player.GetComponent<PlayerActionHandler>().UnregisterPushAction(OnPlayerPush);
        player.GetComponent<PlayerActionHandler>().UnregisterDeathAction(OnPlayerDie);

        player.GetComponent<PlayerModelControllerComponent>().HideWinObjects();

        base.OnEnd();
    }
}
