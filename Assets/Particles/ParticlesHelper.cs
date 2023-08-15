using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesHelper : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 3;

    private void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
