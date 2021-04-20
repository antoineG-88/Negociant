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
    public TweeningAnimator hoverAnim;

    [HideInInspector] public bool isHovered;
    [HideInInspector] public bool isPressed;
    [HideInInspector] public bool isClicked;
    [HideInInspector] public bool clickedDown;
    [HideInInspector] public bool canBeHovered;

    private bool hoveredFlag;

    private void Start()
    {
        canBeHovered = true;
        hoverAnim.GetReferences();
        hoverAnim.anim = Instantiate(hoverAnim.anim);
    }

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

        if(canBeHovered)
        {
            if(!hoveredFlag && isHovered)
            {
                StartCoroutine(hoverAnim.anim.Play(hoverAnim, hoverAnim.originalPos));
                hoveredFlag = true;
            }
        }
        else
        {
            isHovered = false;
        }

        if(!isHovered && hoveredFlag)
        {
            StartCoroutine(hoverAnim.anim.PlayBackward(hoverAnim, hoverAnim.originalPos, true));
            hoveredFlag = false;
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
        isPressed = false;
    }
}
