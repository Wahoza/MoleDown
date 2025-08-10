using System.Collections.Generic;
using UnityEngine;

public class DoActionOnCallComponent : MonoBehaviour
{
    [SerializeField] List<IActionComponent> actions;
    public void DoActions()
    {
        foreach (IActionComponent action in actions)
        {
            action.DoAction();
        }
    }
}
