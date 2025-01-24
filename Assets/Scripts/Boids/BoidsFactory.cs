using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class BoidsFactory : MonoBehaviour
{
    [SerializeField] private GameObject boidprefab;
    [SerializeField] private float randomSpawnRadius;
    [SerializeField] private int spawnCount = 10000;
    private static List<Boid> _spawnedBoids = new();
    private static Transform _parent;
    private long _totalMilliSeconds;

    private void Awake()
    {
        _parent = GetComponent<Transform>();
    }

    private async void Start()
    {
        SpawnBoids(spawnCount);
        await Awaitable.EndOfFrameAsync();
        Debug.Log(_totalMilliSeconds);
    }

    private void SpawnBoids(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var boid = Instantiate(boidprefab, _parent).GetComponent<Boid>();
            boid.transform.position = RandomSpawnVec();
            _spawnedBoids.Add(boid);
        }
    }

    private void Update()
    {
        foreach (var boid in _spawnedBoids)
        {
            Vector3 dir = Cohesion(boid.gameObject,
                GetNeighboursOverlap(boid.gameObject, boid.DetectionRadius).ToList());
            dir += Alignment(boid.gameObject,
                GetNeighboursOverlap(boid.gameObject, boid.DetectionRadius).ToList());
            dir += Separation(boid.gameObject,
                GetNeighboursOverlap(boid.gameObject, boid.SeparationRadius).ToList());

            dir = Vector3.Lerp(boid.transform.forward, dir, Time.deltaTime);

            dir.Normalize();

            boid.transform.position += dir * boid.MoveSpeed * Time.deltaTime;
            boid.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private Vector3 Separation(GameObject thisBoid, List<Collider> neighbours)
    {
        if (neighbours == null || neighbours.Count == 0 || thisBoid == null) return Vector3.zero;

        Vector3 dir = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            dir += thisBoid.transform.position - neighbour.transform.position;
        }

        return dir.normalized;
    }

    private Vector3 Alignment(GameObject thisBoid, List<Collider> neighbours)
    {
        if (thisBoid == null) return Vector3.zero;

        if (neighbours == null || neighbours.Count == 0) return thisBoid.transform.forward;

        Vector3 dir = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            dir += neighbour.transform.forward;
        }

        return (dir / neighbours.Count).normalized;
    }

    private Vector3 Cohesion(GameObject thisBoid, List<Collider> neighbours)
    {
        if (thisBoid == null || neighbours.Count == 0 || neighbours == null) return Vector3.zero;

        Vector3 dir = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            dir += neighbour.transform.position;
        }

        var averagePos = (dir / neighbours.Count);
        return (averagePos - thisBoid.transform.position).normalized;
    }

    private Vector3 RandomSpawnVec()
    {
        return Random.insideUnitSphere * randomSpawnRadius;
    }

    public Collider[] GetNeighboursOverlap(GameObject thisBoid, float range)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();
        Collider[] neighbours = Physics.OverlapSphere(thisBoid.transform.position, range, LayerMask.GetMask("Boid"));

        stopwatch.Stop();
        _totalMilliSeconds += stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
        return neighbours;
    }

    // public List<GameObject> GetNeighboursDistance(GameObject thisBoid, float range)
    // {
    //     Stopwatch stopwatch = new();
    //     stopwatch.Start();
    //     List<GameObject> neighbors = new();
    //     foreach (var boid in _spawnedBoids)
    //     {
    //         if (Vector3.Distance(thisBoid.transform.position, boid.transform.position) <= range)
    //         {
    //             neighbors.Add(boid);
    //         }
    //     }
    //
    //     stopwatch.Stop();
    //     _totalMilliSeconds += stopwatch.ElapsedMilliseconds;
    //     stopwatch.Reset();
    //     return neighbors;
    // }
}