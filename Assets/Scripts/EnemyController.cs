using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Unit
{
    private Transform targetPlayer;
    private Vector3 moveDirection;
    public LayerMask targetLayer;
    public int stopDistance;
    // Start is called before the first frame update
    void Start()
    {
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        moveDirection = (targetPlayer.transform.position - transform.position).normalized;
        transform.LookAt(targetPlayer);

        var targetCollider = Physics.OverlapSphere(transform.position, stopDistance, targetLayer);
        if (targetCollider.Length == 0)
            transform.position += moveDirection * speed * Time.deltaTime;
    }
}
