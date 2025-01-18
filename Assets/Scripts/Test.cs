using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Test : MonoBehaviour
{
    private void Start()
    {
        int size = 100000000;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        int[] intArray = new int[size];
        for (int i = 0; i < intArray.Length; i++)
        {
            intArray[i] = (int)Mathf.Pow(i, i);
        }
        stopwatch.Stop();
        Debug.Log($"milliseconds : {stopwatch.ElapsedMilliseconds}");

        stopwatch.Reset();
        stopwatch.Start();
        NativeArray<int> nativeArray = new NativeArray<int>(size, Allocator.TempJob);
        var job = new BurstJob() { nativeArray = nativeArray };
        job.Schedule(nativeArray.Length, 64).Complete();
        stopwatch.Stop();
        Debug.Log($"milliseconds : {stopwatch.ElapsedMilliseconds}");
        nativeArray.Dispose();
    }

    [BurstCompile]
    struct BurstJob : IJobParallelFor
    {
        public NativeArray<int> nativeArray;

        public void Execute(int index)
        {
            nativeArray[index] = (int)Mathf.Pow(index, index);
            ;
            // for (int i = 0;
            //      i < nativeArray.Length;
            //      i++)
            // {
            //     nativeArray[i] = (int)Mathf.Pow(i, i);
            // }
        }
    }
}