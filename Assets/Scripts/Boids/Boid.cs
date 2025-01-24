using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField] private float detectionRadius;
    [SerializeField] private float separationRadius;
    [SerializeField] private float _moveSpeed;

    public float DetectionRadius => detectionRadius;
    public float SeparationRadius => separationRadius;
    public float MoveSpeed => _moveSpeed;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}