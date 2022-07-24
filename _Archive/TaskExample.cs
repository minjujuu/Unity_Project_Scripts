using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks; // Task

public class TaskExample : MonoBehaviour
{
    /* Task Method
     * Factory.StartNew(): ������ ������ ����
     * Start(): Task ������ ����
     * Wait(): Task ���� ������ ���
     * **/
    void Start()
    {
        // ���� ȣ��, ������ ���� �� ����
        Task.Factory.StartNew( () => { Debug.Log( "Task" ); } );

        // Action
        Task task2 = new Task( new System.Action( DebugLog ) );
        task2.Start();

        // delegate �븮�� 
        Task task3 = new Task( delegate { DebugLog(); } );
        task3.Start();

        // ���ٽ� 
        Task task4 = new Task( () => DebugLog() );
        task4.Start();

        // ���ٿ� �͸� �޼���
        Task task5 = new Task( () => { DebugLog(); });
        task5.Start();

        // �� Task�� ���� ������ ���
        task2.Wait();
        task3.Wait();
        task4.Wait();
        task5.Wait();
    }

    void DebugLog()
    {
        Debug.Log("Task");
    }
}
