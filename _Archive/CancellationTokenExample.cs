using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System.Threading.Tasks;

public class CancellationTokenExample : MonoBehaviour
{
    // 1. 클래스 멤버로 CancellationTokenSource 선언
    CancellationTokenSource m_CancelTokenSource;
    Task<int> m_Task;
    
    void Start()
    {
        // 2. CancellationTokenSource 객체 생성
        m_CancelTokenSource = new CancellationTokenSource();

        CancellationToken cancellationToken = m_CancelTokenSource.Token;
        m_Task = Task.Factory.StartNew(TaskMethod, cancellationToken);
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // CancellationTokenSource 의 Cancel() 를 통해 작업 취소

            m_CancelTokenSource.Cancel();

            if(m_Task != null)
            {
                Debug.Log( $"Count: {m_Task.Result}" );
            }
        }
    }

    private int TaskMethod()
    {
        int count = 0;
        for (int i = 0; i < 10; i++)
        {
            // 비동기 작업 메서드 안에서 작업이 취소되었는지 체크
            if (m_CancelTokenSource.Token.IsCancellationRequested)
            {
                break;
            }

            ++count;
            Thread.Sleep( 1000 );
        }
        return count;
    }
}
