using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private int maxHitPoints;
    private int hitPoints;
    [SerializeField] private float speed;

    public int HitPoints
    {
        get => hitPoints;
        set
        {
            hitPoints = value;
            if (hitPoints <= 0)
            {
                Die();
            }
        }
    }

    public float Speed => speed;

    private void OnEnable()
    {
        SetHitPoints();
    }

    public void SetHitPoints()
    {
        hitPoints = maxHitPoints;
    }

    public void TakeDamage(int damage)
    {
        HitPoints -= damage;
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
