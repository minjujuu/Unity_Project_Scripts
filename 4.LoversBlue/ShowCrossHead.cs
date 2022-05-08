using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCrossHead : MonoBehaviour {

    public static ShowCrossHead Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    public CanvasRenderer crossHead;
    
	void Start () {
        crossHead.SetAlpha(0);
        
    }

    public void ShowHead()
    {
        Cursor.lockState = CursorLockMode.Locked;
        crossHead.SetAlpha(100);
    }

    public void HideHead()
    {
        Cursor.lockState = CursorLockMode.None;
        crossHead.SetAlpha(0);

    }

}
