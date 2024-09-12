using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour, IPointerClickHandler
{
    public static UIManager main;
    private bool isHoveringUI;

    private void Awake()
    {
        main = this;
    }

    private void Update()
    {
        // Check if the pointer is currently over any UI element
        isHoveringUI = EventSystem.current.IsPointerOverGameObject();
    }

     public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("UI clicked!");
        // Other logic for handling UI clicks
    }

    public void SetHoveringState(bool state)
    {
        isHoveringUI = state;
    }

    public bool IsHoveringUI()
    {
        return isHoveringUI;
    }
}
