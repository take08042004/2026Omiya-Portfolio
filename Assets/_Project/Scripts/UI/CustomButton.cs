//using System.Threading.Tasks.Dataflow; 
//Unity標準の環境には存在しないためエラー,ここでは参照していないのでCO,不要なら消して
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour
{
    [Header("Hover Animation")]
    public float hoverScale = 1.2f;
    [Header("Click Event")]
    public UnityEvent onClick;

    private Vector3 defaultScale;
    private bool isHovered = false;

    void Start()
    {
        defaultScale = transform.localScale;
    }


    public void OnHoverEnter()
    {
        if (isHovered) return;
        isHovered = true;
        transform.localScale = defaultScale * hoverScale;
    }

    public void OnHoverExit()
    {
        if(!isHovered) return;
        isHovered = false;
        transform.localScale = defaultScale;
    }

    public void OnSelected()
    {
        if(onClick != null)
        {
            onClick.Invoke();
        }
    }
    
    public void OnClick()
    {
        // 1. ボタンクリックSEを鳴らす
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.seButtonClick);
        }
        
        // 2. インスペクターで割り当てられたイベントを実行する
        OnSelected();
    }    

}