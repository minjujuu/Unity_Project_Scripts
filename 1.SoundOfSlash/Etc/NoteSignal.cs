using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RhythmGameStarter;

public class NoteSignal : MonoBehaviour
{
    public SpriteRenderer note_track0_spriteRenderer = null;
    public SpriteRenderer note_track1_spriteRenderer = null;
    public SpriteRenderer note_track2_spriteRenderer = null;
    public SpriteRenderer note_track3_spriteRenderer = null;
    public SpriteRenderer note_track4_spriteRenderer = null;
    public SpriteRenderer note_track5_spriteRenderer = null;

    public Sprite noteSprite_track0 = null;
    public Sprite noteSprite_track1 = null;
    public Sprite noteSprite_track2 = null;
    public Sprite noteSprite_track3 = null;
    public Sprite noteSprite_track4 = null;
    public Sprite noteSprite_track5 = null;

    public Sprite comboSprite = null;

    private SpawnManager spawnManager;
    private InGameManager inGameManager;
    private ComboChecker comboChecker = null;
    private Note selfNote;
    private int layer = -1;
    private int tempIndex = 0;
    private int speedCalCount = 0;
    private float y = 0.0f;
    private float alpha = 1.0f;  
    private float noteSpeed;
    private float speedUpdateDelay = 1.0f;
    private bool isExecuted = false;
    private bool enableBlind = false;

    private void Awake()
    {
        selfNote = GetComponent<Note>();
        spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        inGameManager = GameObject.FindObjectOfType<InGameManager>();
        comboChecker = GameObject.FindObjectOfType<ComboChecker>();
    }

    
    private void Start()
    {
        int randomInt = Random.Range(0, 2);
        enableBlind = randomInt == 0 ? true : false;

        //selfNote.OnNoteRespawned = OnNoteRespawned;
    }

    
    //private void OnNoteRespawned()
    //{
        
    //    Debug.Log($"ComboNoteTest index : {selfNote.spawnedIndex}");
    //    Debug.Log($"ComboNoteTest tempIndex : {tempIndex}");
    //    if (comboChecker.IsComboIndex(tempIndex))
    //    {
    //        leftRenderer.sprite = comboSprite;
    //        rightRenderer.sprite = comboSprite;
    //    }
    //    else
    //    {
    //        leftRenderer.sprite = leftSprite;
    //        rightRenderer.sprite = rightSprite;
    //        //leftRenderer.sprite = transform.parent.parent.name == "1" ? leftSprite : rightSprite;
    //    }
    //}


    
    private void Update()
    {
        
        tempIndex = selfNote.spawnedIndex;
        if (comboChecker.IsComboIndex(tempIndex))
        {
            note_track0_spriteRenderer.sprite = comboSprite;
            note_track1_spriteRenderer.sprite = comboSprite;
        }
        else
        {
            note_track0_spriteRenderer.sprite = noteSprite_track0;
            note_track1_spriteRenderer.sprite = noteSprite_track1;
            note_track2_spriteRenderer.sprite = noteSprite_track2;
            note_track3_spriteRenderer.sprite = noteSprite_track3;
            note_track4_spriteRenderer.sprite = noteSprite_track4;
            note_track5_spriteRenderer.sprite = noteSprite_track5;
            //leftRenderer.sprite = transform.parent.parent.name == "1" ? leftSprite : rightSprite;
        }

        if (ModeData.playMode == PlayMode.Blind)
        {
            if (enableBlind)
            {
                y = Mathf.Abs(transform.localPosition.y);
                y = Mathf.Clamp(y, 1, 3);

                alpha = (y - 1) / 2.0f;

                Color color = selfNote.GetActive_SpriteRenderer().color;
                color.a = alpha;
                selfNote.GetActive_SpriteRenderer().color = color;
            }
        }

        //AutoPlay(); // TODO : AUTO PLAY ENABLE
        
    }

   
    private void AutoPlay ()
    {
        if (isExecuted)
            return;

        layer = gameObject.layer;
        bool isLeft = transform.parent.parent.name == "1" ? true : false;

        if (transform.localPosition.y <= 0.1f)
        {
            AutoPlayer autoPlayer = FindObjectOfType<AutoPlayer>();
            string _isLeft = isLeft ? "LEFT" : "RIGHT";
            Debug.Log($"AutoPlayer {_isLeft}");

            Debug.Log($"AutoPlayer LAYER : {layer}");
            if (layer == 11 || layer == 12 || layer == 13)
            {
                autoPlayer.OnComboButtonClicked();
                Debug.Log($"AutoPlayer LAYER Combo");
                isExecuted = true;
                return;
            }
            else
            {
                Debug.Log($"AutoPlayer Not Combo Layer : {layer}");
            }

            if (!isLeft)
            {
                autoPlayer.OnRightButtonClicked();
                isExecuted = true;
            }
            else
            {
                autoPlayer.OnLeftButtonClicked();
                isExecuted = true;
            }
        }
    }

    private void OnEnable()
    {
        isExecuted = false;

        if (speedCalCount < 20)
            StartCoroutine(SpeedReckoner());

        if (this.transform.parent != null)
        {
            if (spawnManager == null)
            {
                spawnManager = GameObject.FindObjectOfType<SpawnManager>();
            }
            spawnManager.SpawnMonster(this.transform.parent.transform.parent.name);
        }
    }

    /* 몬스터가 노트에서 분리되었을 때 속도는 노트의 속도를 따라가므로 노트 속도 계산 필요 */
    /* 초반에만 노트 속도를 계산하며, InGameManager에서 관리 함 */
    private IEnumerator SpeedReckoner()
    {
        YieldInstruction timedWait = new WaitForSeconds(speedUpdateDelay);
        Vector3 lastPosition = transform.localPosition;
        float lastTimestamp = Time.time;
        while (enabled)
        {
            yield return timedWait;
            var deltaPosition = (transform.localPosition - lastPosition).magnitude;
            var deltaTime = Time.time - lastTimestamp;
            if (Mathf.Approximately(deltaPosition, 0f)) // Clean up "near-zero" displacement
                deltaPosition = 0f;
            noteSpeed = deltaPosition / deltaTime;

            inGameManager.CalNoteSpeed(noteSpeed);
            speedCalCount++;
            lastPosition = transform.localPosition;
            lastTimestamp = Time.time;
        }
    }

    private void OnDisable()
    {
        ImageHelper.FadeAlpha(selfNote.GetActive_SpriteRenderer(), 1, 0.1f);
    }

}