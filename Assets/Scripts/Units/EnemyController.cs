using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Unit
{
    private GameObject targetPlayer;
    private Vector3 moveDirection;
    // public LayerMask targetLayer;
    public int stopDistance;
    public int shootDistance;
    private NavMeshAgent agent;
    
    public int fireRate;
    public GameObject bullet;
    public GameObject shootingPoint;
    private float lastFired;

    private bool isShooting;
    private Animator anim;

    public static event Action OnEnemyDestroyed;

    private void OnEnable()
    {
        PlayerController.OnPlayerDie += StopShooting;
        SetHitPoints();
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDie -= StopShooting;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;
        agent.speed = Speed;
    }

    private void FixedUpdate()
    {
        if (targetPlayer)
        {
            agent.SetDestination(targetPlayer.transform.position);
            transform.LookAt(targetPlayer.transform);
        }
        else
        {
            StopAllCoroutines();
            return;
        }

        if ((transform.position - targetPlayer.transform.position).magnitude < shootDistance && !isShooting)
        {
            isShooting = true;
            StartCoroutine(StartShooting());
        }
        else
        {
            isShooting = false;
            StopCoroutine(StartShooting());
        }
        
    }

    public override void TakeDamage(int damage)
    {
        anim.Play("Take Damage");
        HitPoints -= damage;
    }

    public override void Die()
    {
        OnEnemyDestroyed?.Invoke();
        base.Die();
    }
    
    IEnumerator StartShooting()
    {
        while (true)
        {
            if (Time.time - lastFired > 1 / fireRate)
            {
                lastFired = Time.time;
                var projectile = Instantiate(bullet, shootingPoint.transform.position, shootingPoint.transform.rotation);
                projectile.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 200));
                // projectile.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 10);
            }
            yield return null;
        }
    }

    void StopShooting()
    {
        StopCoroutine(StartShooting());
    }
}
