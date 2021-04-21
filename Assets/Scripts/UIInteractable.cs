using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public bool isHovered;
    [HideInInspector] public bool isPressed;
    [HideInInspector] public bool isClicked;
    [HideInInspector] public bool clickedDown;

    private bool hoveredFlag;

    public virtual void Update()
    {
        if (isClicked)
        {
            isClicked = false;
        }

        if (clickedDown)
        {
            clickedDown = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
        }

        if (!hoveredFlag && isHovered)
        {
            OnHoverIn();
            hoveredFlag = true;
        }

        if (!isHovered && hoveredFlag)
        {
            OnHoverOut();
            hoveredFlag = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        clickedDown = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            isClicked = true;
        }
        isPressed = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isPressed = false;
    }

    public abstract void OnHoverIn();
    public abstract void OnHoverOut();
}
