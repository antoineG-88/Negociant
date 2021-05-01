using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExchangeSpace : UIInteractable
{
    public TweeningAnimator hoverWithObjectAnim;
    public Image objectImage;
    [HideInInspector] public StallObject stallObjectHeld;
    [HideInInspector] public CharaObject charaObjectHeld;
    [HideInInspector] public bool canBeUsed;
    [HideInInspector] public RectTransform rectTransform;
    private bool isBig;

    private void Start()
    {
        hoverWithObjectAnim.anim = Instantiate(hoverWithObjectAnim.anim);
        hoverWithObjectAnim.GetReferences();
        hoverWithObjectAnim.anim.SetAtStartState(hoverWithObjectAnim);
        rectTransform = hoverWithObjectAnim.rectTransform;
    }

    private void Update()
    {
        if(isBig && !canBeUsed)
        {
            StartCoroutine(hoverWithObjectAnim.anim.PlayBackward(hoverWithObjectAnim, true));
            isBig = false;
        }
    }

    public override void OnHoverIn()
    {
        if(canBeUsed)
        {
            StartCoroutine(hoverWithObjectAnim.anim.Play(hoverWithObjectAnim));
            isBig = true;
        }
    }

    public override void OnHoverOut()
    {
        if(isBig)
        {
            StartCoroutine(hoverWithObjectAnim.anim.PlayBackward(hoverWithObjectAnim, true));
            isBig = false;
        }
    }
}
