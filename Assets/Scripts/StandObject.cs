using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StandObject : UIInteractable
{
    public Object linkedObject;
    public Text nameText;
    public Image illustration;
    public TweeningAnimator hoverAnim;
    public RectTransform rectTransform;
    public Text interestLevel;
    [HideInInspector] public bool canBeHovered;

    private void Start()
    {
        canBeHovered = true;
        hoverAnim.GetReferences();
        hoverAnim.anim = Instantiate(hoverAnim.anim);
    }

    public override void Update()
    {
        base.Update();
    }

    public void RefreshDisplay()
    {
        nameText.text = linkedObject.objectName;
        illustration.sprite = linkedObject.illustration;
    }

    public override void OnHoverIn()
    {
        if(canBeHovered)
        {
            StartCoroutine(hoverAnim.anim.Play(hoverAnim, hoverAnim.originalPos));
        }
        else
        {
            isHovered = false;
        }
    }

    public override void OnHoverOut()
    {
        StartCoroutine(hoverAnim.anim.PlayBackward(hoverAnim, hoverAnim.originalPos, true));
    }
}
