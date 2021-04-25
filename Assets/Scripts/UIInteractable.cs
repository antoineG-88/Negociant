using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
{
    [HideInInspector] public bool isHovered;
    [HideInInspector] public bool isPressed;
    [HideInInspector] public bool isClicked;
    [HideInInspector] public bool clickedDown;
    [HideInInspector] public bool clickedUp;
    [HideInInspector] public bool isDragged;
    [HideInInspector] public bool isDropped;

    private bool hasBeenClickedWithoutLeaving;
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

        if (clickedUp)
        {
            clickedUp = false;
        }

        if (isDropped)
        {
            isDropped = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
        }

        if (!hoveredFlag && isHovered)
        {
            hoveredFlag = true;
        }

        if (!isHovered && hoveredFlag)
        {
            hoveredFlag = false;
        }

        if(Input.GetMouseButtonUp(0))
        {
            if(isHovered)
            {
                clickedUp = true;
            }
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        clickedDown = true;
        hasBeenClickedWithoutLeaving = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (hasBeenClickedWithoutLeaving)
        {
            isClicked = true;
        }
        hasBeenClickedWithoutLeaving = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverIn();
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverOut();
        isHovered = false;
        hasBeenClickedWithoutLeaving = false;
    }

    public abstract void OnHoverIn();
    public abstract void OnHoverOut();

    public void OnDrag(PointerEventData eventData)
    {
        isDragged = true;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        isDragged = false;
        isDropped = true;
    }
}
