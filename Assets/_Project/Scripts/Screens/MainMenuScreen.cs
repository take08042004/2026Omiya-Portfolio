using UnityEngine;
using System.Collections.Generic;

public class MainMenuScreen : MonoBehaviour
{
    public void OnClickCreate()
    {
        GameManager.Instance.GoToCreate();
    }

    public void OnClickView()
    {
        GameManager.Instance.GoToView();
    }
} 