using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RadialOption : UIInteractable
{
    public TweeningAnimator hoverAnim;
    public Image icon;
    public Text info;
    public Text time;

    private void Start()
    {
        hoverAnim.GetReferences();
        hoverAnim.anim = Instantiate(hoverAnim.anim);
        hoverAnim.anim.SetAtStartState(hoverAnim);
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
