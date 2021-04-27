using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaObject : UIInteractable
{
    public Object linkedObject;
    public Image illustration;
    public RectTransform rectTransform;
    public TweeningAnimator hoverAnim;
    public float personnalValue;

    private void Start()
    {
        hoverAnim.anim = Instantiate(hoverAnim.anim);
        hoverAnim.GetReferences();
    }

    public override void OnHoverIn()
    {
        StartCoroutine(hoverAnim.anim.Play(hoverAnim));
    }

    public override void OnHoverOut()
    {
        StartCoroutine(hoverAnim.anim.PlayBackward(hoverAnim, true));
    }
}
