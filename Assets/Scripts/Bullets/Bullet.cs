using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>())
            other.gameObject.GetComponent<Unit>()?.TakeDamage(damage);

        Destroy(gameObject);
    }
}
