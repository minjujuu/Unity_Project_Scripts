using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System.Threading.Tasks;

public class AsyncAwaitExample : MonoBehaviour
{
    /* Async, Await Method
     * Run(): 비동기 시작
     * FromResult(): 비동기 시작 후 결과값을 얻음 */

    void Start()
    {
        TaskRun();
        TaskFromResult();
    }

    async void TaskRun()
    {
        var task = Task.Run( () => TaskRunMethod( 3 ) );
        int count = await task;
        Debug.Log( $"Count: {task.Result}" );
    }

    private int TaskRunMethod(int limit)
    {
        int count = 0;
        for (int i = 0; i < limit; i++)
        {
            ++count;
            Thread.Sleep( 1000 );
        }

        return count;
    }

    async void TaskFromResult()
    {
        int sum = await Task.FromResult( Add(1, 2) );
        Debug.Log( sum );
    }

    private int Add(int a, int b)
    {
        return a + b;
    }
}
