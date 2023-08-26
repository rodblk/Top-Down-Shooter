using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSpace : MonoBehaviour
{
    public static event Action OnFinishStage;
    private void OnTriggerEnter(Collider other)
    {
        OnFinishStage?.Invoke();
    }
}
