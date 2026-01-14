using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InstructionCycle : MonoBehaviour
{
    int iterator = 0;
    [SerializeField]  List<GameObject> infoScreens;
    private void OnEnable()
    {
        iterator = 0;

        foreach (GameObject info in infoScreens)
        {
            info.SetActive(false);
        }

        infoScreens[0].SetActive(true);
    }

    public void CycleToNext(int cycleAmount)
    {
        infoScreens[iterator].SetActive(false);

        iterator += cycleAmount;

        if(iterator >= infoScreens.Count)
        {
            iterator = 0;
        }

        if(iterator < 0)
        {
            iterator = infoScreens.Count - 1;
        }

        infoScreens[iterator].SetActive(true);
    }
}
