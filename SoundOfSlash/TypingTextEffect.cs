using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingTextEffect : MonoBehaviour
{
    public Text typingText;
    

    string msg;
    public float typingSpeed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        typingText = GetComponent<Text>();
        msg = "Loading...";

        StartCoroutine(Typing(typingText, msg, typingSpeed));
    }

    float time;
    public float waitingTime = 3;
    private void Update()
    {
        time += Time.deltaTime;
        if(time > waitingTime)
        {
            StartCoroutine(Typing(typingText, msg, typingSpeed));
            time = 0;
        }    
    }

    IEnumerator Typing(Text txt, string msgString, float spd)
    {
        for(int i=0; i< msgString.Length; i++)
        {
            typingText.text = msgString.Substring(0, i + 1);
            yield return new WaitForSeconds(spd);
        }
        
    }

}
