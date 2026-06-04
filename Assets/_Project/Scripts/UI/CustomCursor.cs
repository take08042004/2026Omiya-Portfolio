using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI; //追加

public class CustomCursor : MonoBehaviour
{
    public Image cursorImage; //追加
    public RectTransform cursorUI;

    public Sprite defaultCursor;//追加
    public Sprite hoverCursor;//追加 

    private HandInput handInput;

    private GameObject currentHover;

    private bool wasFist = false;

    private Vector2 smoothPos;

    void Start()
    {
        handInput = FindObjectOfType<HandInput>();
        smoothPos = Vector2.zero;
    }

    void Update()
    {
        if(handInput == null) return;
        if(cursorUI == null) return;

        Vector2 screenPos = handInput.GetHandPosition();

        //追加
        bool isFist = handInput.IsFist();

        if(cursorImage != null)
        {
            cursorImage.sprite = isFist ? hoverCursor : defaultCursor;
        }

        //追加終わり

        smoothPos = Vector2.Lerp(smoothPos, screenPos, 0.2f);

        cursorUI.position = smoothPos;

        RaycastUI(smoothPos);


    }

    void RaycastUI(Vector2 screenPos)
    {
        if(EventSystem.current == null) return;


        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = screenPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        GameObject newHover = null;

        foreach(var result in results)
        {
            CustomButton button = result.gameObject.GetComponent<CustomButton>();


            /*if(button != null)
            {
                newHover = result.gameObject;
                break;
            }*/

            //追加
            if(button != null)
            {
                Debug.Log("ボタンに触れた:"+ result.gameObject.name);
                
                newHover = result.gameObject;
                break;
            }
        }

        if(currentHover != newHover)
        {
            if(currentHover != null)
            {
                CustomButton oldBtn = currentHover.GetComponent<CustomButton>();
                if(oldBtn != null) oldBtn.OnHoverExit();
            }

            currentHover = newHover;

            if(currentHover != null)
            {
                CustomButton newBtn = currentHover.GetComponent<CustomButton>();
                if(newBtn != null) newBtn.OnHoverEnter();
            }
        }

        bool isFist = handInput.IsFist();

        if(!wasFist && isFist && currentHover != null)
        {
            Debug.Log("グーで決定:"+ currentHover.name);//追加
            
            CustomButton btn = currentHover.GetComponent<CustomButton>();
            if(btn != null){
                btn.OnSelected();
            }
        }

        wasFist = isFist;
    }
}