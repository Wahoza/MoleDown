using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DoActionsOnTickComponent : MonoBehaviour
{
    [SerializeField] float tickSpeed;
    [SerializeField] List<IActionComponent> actions;
    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickSpeed);

            foreach (var action in actions)
            {
                action.DoAction();
            }
        }
    }
}
