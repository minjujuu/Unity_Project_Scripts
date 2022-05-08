using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템을 관리하는 스크립트
// - 아이템을 먹었을 때 아이템이 사라지는 코드는 플레이어 스크립트에 있음.
// - 여기에서는 아이템을 먹었을 때 UI를 나타나게 하는 역할과
//   플레이어가 어떤 아이템을 먹었는지를 알 수 있는 리스트를 생성하는 역할을 함.
public class MJ_ItemScript : MonoBehaviour {

    // Item UI의 CanvasRenderer
    public CanvasRenderer eyesImage;
    public CanvasRenderer browImage;
    public CanvasRenderer mouthImage;
    public CanvasRenderer buttonImage;
    public CanvasRenderer etcDecoImage;

    // Item들의 파티클
    public GameObject eyesParti;
    public GameObject browParti;
    public GameObject mouthParti;
    public GameObject buttonParti;
    public GameObject etcDecoParti;

    public GameObject goalParticle;
    public static MJ_ItemScript Instance;

    public List<string> itemList = new List<string>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {

        HideItemUI();

    }


    void HideItemUI()
    {
        eyesImage.SetAlpha(0);
        browImage.SetAlpha(0);
        mouthImage.SetAlpha(0);
        buttonImage.SetAlpha(0);
        etcDecoImage.SetAlpha(0);
    }

    public GameObject candy;
    private void OnCollisionEnter(Collision item)
    {
        if (item.gameObject.tag == "EyesDeco")
        {
            eyesImage.SetAlpha(100);
            itemList.Add("Eyes");
            eyesParti.SetActive(false);
        }
        else if (item.gameObject.tag == "BrowDeco")
        {
            browImage.SetAlpha(100);
            itemList.Add("Brow");
            browParti.SetActive(false);
        }
        else if (item.gameObject.tag == "MouthDeco")
        {
            mouthImage.SetAlpha(100);
            itemList.Add("Mouth");
            mouthParti.SetActive(false);
        }
        else if (item.gameObject.tag == "ButtonDeco")
        {
            buttonImage.SetAlpha(100);
            itemList.Add("Button");
            buttonParti.SetActive(false);
        }
        else if (item.gameObject.tag == "EtcDeco")
        {
            etcDecoImage.SetAlpha(100);
            itemList.Add("Etc");
            etcDecoParti.SetActive(false);
            candy.SetActive(true);
        }
    }


}
