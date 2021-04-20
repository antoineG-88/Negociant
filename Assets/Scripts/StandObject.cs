using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StandObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Object linkedObject;
    public Text nameText;
    public Image illustration;

    public bool isHovered;
    public bool isPressed;
    public bool isClicked;
    public bool clickedDown;
    private void Update()
    {
        if(isClicked)
        {
            isClicked = false;
        }

        if(clickedDown)
        {
            clickedDown = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
        }
    }

    public void RefreshDisplay()
    {
        nameText.text = linkedObject.objectName;
        illustration.sprite = linkedObject.illustration;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        clickedDown = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(isPressed)
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
    }
}
