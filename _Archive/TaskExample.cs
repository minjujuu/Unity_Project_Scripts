using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks; // Task

public class TaskExample : MonoBehaviour
{
    /* Task Method
     * Factory.StartNew(): 스레드 생성과 시작
     * Start(): Task 스레드 시작
     * Wait(): Task 끝날 때까지 대기
     * **/
    void Start()
    {
        // 직접 호출, 스레드 생성 및 시작
        Task.Factory.StartNew( () => { Debug.Log( "Task" ); } );

        // Action
        Task task2 = new Task( new System.Action( DebugLog ) );
        task2.Start();

        // delegate 대리자 
        Task task3 = new Task( delegate { DebugLog(); } );
        task3.Start();

        // 람다식 
        Task task4 = new Task( () => DebugLog() );
        task4.Start();

        // 람다와 익명 메서드
        Task task5 = new Task( () => { DebugLog(); });
        task5.Start();

        // 각 Task가 끝날 때까지 대기
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
