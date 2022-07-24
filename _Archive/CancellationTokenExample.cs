using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System.Threading.Tasks;

public class CancellationTokenExample : MonoBehaviour
{
    // 1. Ŭ���� ����� CancellationTokenSource ����
    CancellationTokenSource m_CancelTokenSource;
    Task<int> m_Task;
    
    void Start()
    {
        // 2. CancellationTokenSource ��ü ����
        m_CancelTokenSource = new CancellationTokenSource();

        CancellationToken cancellationToken = m_CancelTokenSource.Token;
        m_Task = Task.Factory.StartNew(TaskMethod, cancellationToken);
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // CancellationTokenSource �� Cancel() �� ���� �۾� ���

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
            // �񵿱� �۾� �޼��� �ȿ��� �۾��� ��ҵǾ����� üũ
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
