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

    private Collider CurrentTarget
    {
        set
        {
            if (value != null && currentTarget != value)
            {
                currentTarget = value;
                StartCoroutine(nameof(StartShooting));
            }
            else if (value == null)
            {
                currentTarget = null;
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
        if (targets.Length > 0)
        {
            CurrentTarget = targets[0];
            LookAtTarget();
        }
        else if(targets.Length <= 0)
        {
            CurrentTarget = null;
        }
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

    public override void Die()
    {
        OnPlayerDie?.Invoke();
        base.Die();
    }

    // Usa para ver raio
    // public void OnDrawGizmos()
    // {
    //     var collider = GetComponent<Collider>();
    //
    //     if (!collider)
    //     {
    //         return; // nothing to do without a collider
    //     }
    //
    //     Vector3 closestPoint = collider.ClosestPoint(transform.position);
    //
    //     Gizmos.DrawSphere(transform.position, 3f);
    //     Gizmos.DrawWireSphere(closestPoint, 3f);
    // }
}
