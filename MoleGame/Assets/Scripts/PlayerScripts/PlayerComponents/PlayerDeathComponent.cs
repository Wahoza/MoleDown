using System.Collections;
using UnityEngine;

public class PlayerDeathComponent : IPlayerComponent
{
    [SerializeField] float lowestHeight;
    [SerializeField] float highestHeight;
    [SerializeField] float graceTimer;

    private Coroutine checkDeathRoutine;
    private Coroutine deathTimerRoutine;
    internal override void Start()
    {
        checkDeathRoutine = StartCoroutine(DeathCoroutine());
        base.Start();
    }

    private IEnumerator DeathCoroutine()
    {
        while (true)
        {
            if (transform.position.y < lowestHeight)
            {

                if (deathTimerRoutine == null)
                {
                    deathTimerRoutine = StartCoroutine(DeathUpdateRoutine());
                }
            }
            else if (transform.position.y > highestHeight)
            {
                if (deathTimerRoutine == null)
                {
                    deathTimerRoutine = StartCoroutine(DeathUpdateRoutine());
                }
            }
            else
            {
                if (deathTimerRoutine != null)
                {
                    StopCoroutine(deathTimerRoutine);
                    deathTimerRoutine = null;
                }
            }
            yield return null;
        }
    }

    private IEnumerator DeathUpdateRoutine()
    {
        yield return new WaitForSeconds(graceTimer);
        Die();
    }
    void Die()
    {
        GetComponent<PlayerActionHandler>().OnDie();
    }
}
