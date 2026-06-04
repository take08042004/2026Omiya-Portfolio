using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DrumRollController : MonoBehaviour
{
    [Header("Scroll")]
    public ScrollRect scrollRect;
    [Header("Parts")]
    public List<PartData> parts = new List<PartData>(); 
    [Header("Input")]
    public HandInput handInput;
    [Header("Hit Area")]
    public RectTransform upArea;
    public RectTransform downArea;

    [Header("Roll Settings")]
    public float velocityPower = 25f;

    [Header("Slot Items")]
    public List<RectTransform> itemRects = new List<RectTransform>();

    public float moveSpeed = 500f;
    public float itemHeight = 120f;

    private bool wasGrabbing;
    private float previousHandY;

    [Header("Center Detection")]
    public RectTransform centerPoint;
    public List<RollItem> rollItems = new List<RollItem>();
    
    private float areaStayTimer = 0f;
    
    /*/ --- Debug ---
    [Header("Debug")]
    public bool useMouseDebug = true;*/

    [Header("Area Stay")]
    public float requiredStayTime = 0.2f; //エリア内に留まる必要のある時間

    private PartData currentSelectedPart;
    public System.Action<PartData> onPartChanged;

    void Start()
    {
        if(handInput == null)
        {
            handInput = FindFirstObjectByType<HandInput>();
        }
    }

    void Update()
    {
        UpdateRoll();
    }

    public void UpdateRoll()
    {
        if (handInput == null) return;
        if (scrollRect == null) return;

        Vector2 handPos = handInput.GetHandPosition();

        /*Vector2 handPos;

        if(useMouseDebug)
        {
            handPos = Input.mousePosition;
        }
        else
        {
            if (handInput == null) return;
            handPos = handInput.GetHandPosition();
        }*/

        bool inUp = upArea != null && RectTransformUtility.RectangleContainsScreenPoint(upArea, handPos, null);
        bool inDown = downArea != null && RectTransformUtility.RectangleContainsScreenPoint(downArea, handPos, null);

        //ここに追加
        Debug.Log("inUp:" + inUp);
        Debug.Log("inDown:" + inDown);

        if (!inUp && !inDown)
        {
            /*previousHandY = handPos.y;
            return;*/

            areaStayTimer = 0f;
            previousHandY = handPos.y;
            return;
        }

        areaStayTimer += Time.deltaTime;

        if(areaStayTimer < requiredStayTime)
        {
            previousHandY = handPos.y;
            return;
        }

        /*float deltaY = handPos.y - previousHandY;

        if (inUp)
        {
            scrollRect.velocity = new Vector2(0,Mathf.Abs(deltaY) * velocityPower);
        }
        else if (inDown)
        {
            scrollRect.velocity = new Vector2(0, -Mathf.Abs(deltaY) * velocityPower);
        }

        //scrollRect.velocity = new Vector2(0, deltaY * velocityPower);
        previousHandY = handPos.y;

        CheckSelectedPartChanged();*/

        //書き換え
        /*float deltaY = handPos.y - previousHandY;

        float move = deltaY * moveSpeed * Time.deltaTime;

        if (inDown)
        {
            move *= -1f;
        }*/
        if (!handInput.IsGrabbing())
        {
            previousHandY = handPos.y;
            return;
        }

        
        float move = moveSpeed * Time.deltaTime;

        if (inDown)
        {
            move *= -1f;
        }

        foreach (RectTransform item in itemRects)
        {
            Vector2 pos = item.anchoredPosition;
            pos.y += move;

            //下へ行きすぎたら上へ戻す
            if (pos.y < -itemHeight * itemRects.Count / 2f)
            {
                pos.y += itemHeight * itemRects.Count;
            }

            //上へ行きすぎたら下へ戻す
            if (pos.y > itemHeight * itemRects.Count / 2f)
            {
                pos.y -= itemHeight * itemRects.Count;
            }
            item.anchoredPosition = pos;
        }

        previousHandY = handPos.y;

        CheckSelectedPartChanged();

        DetectCenterItem();
    }

    public string SnapToNearestItem()
    {
        scrollRect.velocity = Vector2.zero;
        PartData nearest = GetNearestPart();

        if (nearest != null)
        {
            currentSelectedPart = nearest;
            onPartChanged?.Invoke(currentSelectedPart);
            return nearest.partID;
        }
        return null;
    }

    public bool IsReleased()
    {
        bool isGrabbing = (handInput != null && handInput.IsGrabbing());
        bool released = wasGrabbing && !isGrabbing;
    
        if (released) wasGrabbing = false;
        return released;
    }

    public bool IsGrabbing()
    {
        if (handInput == null) return false;
    
        bool isGrabbing = handInput.IsGrabbing();
    
        if (!wasGrabbing && isGrabbing)
        {
            previousHandY = handInput.GetHandPosition().y;
        }
    
        wasGrabbing = isGrabbing;
        return isGrabbing;
    }

    private void CheckSelectedPartChanged()
    {
        PartData nearest = GetNearestPart();

        if(nearest != null && nearest != currentSelectedPart)
        {
            currentSelectedPart = nearest;
            onPartChanged?.Invoke(currentSelectedPart);
        }
    }

    // --- RandomRollVisualizer 用の追加メソッド ---

    public void StartRandomRoll(float power)
    {
        if(scrollRect == null) return;
        scrollRect.velocity = new Vector2(0, power);
    }

    public void StopAndSnapFromExternal()
    {
        SnapToNearestItem();
    }

    private PartData GetNearestPart()
    {
        if (parts == null || parts.Count == 0) return null;

        float normalized = scrollRect.verticalNormalizedPosition;
        int index = Mathf.RoundToInt((1f - normalized) * (parts.Count - 1));
        index = Mathf.Clamp(index, 0, parts.Count - 1);

        return parts[index];
    }

    public PartData GetCurrentSelectedPart()
    {
        return currentSelectedPart;
    }

    void DetectCenterItem()
    {
        if (rollItems == null || rollItems.Count == 0) return;
        
        float minDistance = float.MaxValue;
        RollItem nearest = null;
        
        foreach (var item in rollItems)
        {
            float distance = Mathf.Abs(
                item.GetComponent<RectTransform>().position.y
                - centerPoint.position.y
                );
                
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = item;
            }
        }
        
        // 全部falseにする
        foreach (var item in rollItems)
        {
            item.isCenter = false;
        }

    // 一番近いものだけtrue
        if (nearest != null)
        {
            nearest.isCenter = true;
            
            if (nearest.partData != currentSelectedPart)
            {
                currentSelectedPart = nearest.partData;
                onPartChanged?.Invoke(currentSelectedPart);
            }
        }
    }
    
}