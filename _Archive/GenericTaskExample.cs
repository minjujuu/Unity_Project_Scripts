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
         * ���� �����忡�� �ٸ� �۾� �����ϴ� �κ� 
         */

        int result = intTask.Result;
        Debug.Log( result );
    }

    int GetSize( string data )
    {
        return data.Length;
    }
}
