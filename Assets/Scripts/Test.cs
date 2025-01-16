using Unity.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    private NativeArray<int> navtiveArray;

    private void Start()
    {
        navtiveArray = new NativeArray<int>(10, Allocator.Persistent);
        TestCode();
    }

    private void TestCode()
    {
        for (int i = 0; i < navtiveArray.Length; i++)
        {
            navtiveArray[i] = i;
        }
        navtiveArray.Dispose();
    }
}