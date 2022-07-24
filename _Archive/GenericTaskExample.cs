using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks; // Task

public class GenericTaskExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Task<int> intTask = Task.Factory.StartNew<int>( () => GetSize( "GenericTask " ) );

        /*
         * 메인 스레드에서 다른 작업 실행하는 부분 
         */

        int result = intTask.Result;
        Debug.Log( result );
    }

    int GetSize( string data )
    {
        return data.Length;
    }
}
