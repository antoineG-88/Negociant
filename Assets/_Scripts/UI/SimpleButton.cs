using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleButton : UIInteractable
{
    public TweeningAnimator hoverAnim;
    public TweeningAnimator disableAnim;
    [HideInInspector] public bool canBeUsed;
    [HideInInspector] public RectTransform rectTransform;
    private bool isBig;

    private void Start()
    {
        hoverAnim.anim = Instantiate(hoverAnim.anim);
        hoverAnim.GetReferences();
        hoverAnim.anim.SetAtStartState(hoverAnim);
        if(disableAnim.anim != null)
        {
            disableAnim.anim = Instantiate(disableAnim.anim);
            disableAnim.GetReferences();
            disableAnim.anim.SetAtEndState(disableAnim);
        }

        rectTransform = hoverAnim.rectTransform;
    }

    public void SetEnable(bool enabled)
    {
        if(enabled)
        {
            if (!canBeUsed)
            {
                if (disableAnim.anim != null)
                    StartCoroutine(disableAnim.anim.PlayBackward(disableAnim, true));
                canBeUsed = true;
            }
        }
        else
        {
            if (canBeUsed)
            {
                if (disableAnim.anim != null)
                    StartCoroutine(disableAnim.anim.Play(disableAnim));
                canBeUsed = false;
            }
        }
    }

    private void Update()
    {
        if (isBig && !canBeUsed)
        {
            StartCoroutine(hoverAnim.anim.PlayBackward(hoverAnim, true));
            isBig = false;
        }
    }

    public override void OnHoverIn()
    {
        if (canBeUsed)
        {
            StartCoroutine(hoverAnim.anim.Play(hoverAnim));
            isBig = true;
        }
    }

    public override void OnHoverOut()
    {
        if(isBig)
        {
            StartCoroutine(hoverAnim.anim.PlayBackward(hoverAnim, true));
            isBig = false;
        }
    }
}
