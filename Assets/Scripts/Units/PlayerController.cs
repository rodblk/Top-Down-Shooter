using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Unit
{
    private PlayerControls _playerControls;
    public LayerMask targetMask;
    
    public int fireRate;
    public GameObject bullet;
    public GameObject shootingPoint;
    private float lastFired;

    public static event Action OnPlayerDie;

    private Collider currentTarget;
    private Animator anim;

    private Collider CurrentTarget
    {
        set
        {
            if (value != null && currentTarget != value)
            {
                currentTarget = value;
                Invoke(nameof(CallShooting), 0.1f);
            }
            else if (value == null)
            {
                currentTarget = null;
                CancelInvoke();
                StopCoroutine(nameof(StartShooting));
            }
        }
    }

    private void OnEnable()
    {
        SetHitPoints();
        _playerControls.gameplay.Enable();
    }

    private void OnDisable()
    {
        _playerControls.gameplay.Disable();
    }

    private void Awake()
    {
        _playerControls = new PlayerControls();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Player Inputs
        if (_playerControls.gameplay.move.inProgress)
        {
            Move();
        }
        
        // SeekTarget();
    }

    private void FixedUpdate()
    {
        SeekTarget();
    }

    void Move()
    {
        Vector3 moveVector = new Vector3(_playerControls.gameplay.move.ReadValue<Vector2>().x * Time.deltaTime * Speed, 0, _playerControls.gameplay.move.ReadValue<Vector2>().y * Time.deltaTime * Speed);
        transform.position += moveVector;

        LookAtTarget(moveVector);
    }

    void LookAtTarget()
    {
        var targetPosition = currentTarget.gameObject.transform.position;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
    }

    void LookAtTarget(Vector3 moveDirection)
    {
        // Look at direction of movement
        if (!currentTarget)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * 5);
        // Look at target
        else
        {
            var targetPosition = currentTarget.gameObject.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }
    }

    void SeekTarget()
    {
        var targets = Physics.OverlapSphere(transform.position, 8, targetMask);

        float closestDistance = Mathf.Infinity;
        Collider closestCollider = null;

        foreach (var target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollider = target;
            }
        }

        if (closestCollider != null)
        {
            CurrentTarget = closestCollider;
            LookAtTarget();
        }
        else
        {
            CurrentTarget = null;
        }
    }

    public void CallShooting()
    {
        StartCoroutine(nameof(StartShooting));
    }

    IEnumerator StartShooting()
    {
        while (true)
        {
            if (Time.time - lastFired > 1 / fireRate)
            {
                lastFired = Time.time;
                var projectile = Instantiate(bullet, shootingPoint.transform.position, shootingPoint.transform.rotation);
                projectile.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 400));
            }
            yield return null;
        }
    }

    public override void TakeDamage(int damage)
    {
        anim.Play("Take Damage");
        HitPoints -= damage;
    }

    public override void Die()
    {
        OnPlayerDie?.Invoke();
        base.Die();
        Time.timeScale = 0;
    }

    // Usa para ver raio
    public void OnDrawGizmos()
    {
        var collider = GetComponent<Collider>();
    
        if (!collider)
        {
            return; // nothing to do without a collider
        }
    
        Vector3 closestPoint = collider.ClosestPoint(transform.position);
    
        Gizmos.DrawSphere(transform.position, 8f);
        Gizmos.DrawWireSphere(closestPoint, 8f);
    }
}
