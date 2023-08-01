using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform hand;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            HandSorter.UpdateHandPositionsAndRotations(hand);
        }
    }
}
