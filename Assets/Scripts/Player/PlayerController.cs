using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Unit
{
    private PlayerControls _playerControls;
    public LayerMask targetMask;
    // Start is called before the first frame update

    private void OnEnable()
    {
        _playerControls.gameplay.Enable();
        Debug.Log("TESTING");
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
    }

    void Move()
    {
        Vector3 moveVector = new Vector3(_playerControls.gameplay.move.ReadValue<Vector2>().x * Time.deltaTime * speed, 0, _playerControls.gameplay.move.ReadValue<Vector2>().y * Time.deltaTime * speed);
        transform.position += moveVector;
        
        LookAtTarget(moveVector);
    }

    void LookAtTarget(Vector3 moveDirection)
    {
        // Look for colliders in radius
        var targets = Physics.OverlapSphere(transform.position, 3, targetMask);
        if (targets.Length == 0)
            transform.rotation = Quaternion.LookRotation(moveDirection);
        // Look at direction of movement
        else
        {
            var targetPosition = targets[0].gameObject.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }
        
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
