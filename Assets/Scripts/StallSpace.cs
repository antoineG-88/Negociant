using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StallSpace : UIInteractable
{
    public RectTransform rectTransform;
    public StallObject stallObject;
    public TweeningAnimator placeAnim;
    public PlayerHandler playerHandler;
    public bool isVitrine;

    private bool highlightedFlag;

    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        placeAnim.GetReferences();
        placeAnim.anim = Instantiate(placeAnim.anim);
        placeAnim.anim.SetAtStartState(placeAnim);
    }

    public override void Update()
    {
        if(playerHandler.draggedStallObject == null && highlightedFlag)
        {
            highlightedFlag = false;
            StartCoroutine(placeAnim.anim.PlayBackward(placeAnim, placeAnim.originalPos, true));
        }
        base.Update();
    }


    public override void OnHoverIn()
    {
        if(playerHandler.draggedStallObject != null && !highlightedFlag)
        {
            highlightedFlag = true;
            StartCoroutine(placeAnim.anim.Play(placeAnim, placeAnim.originalPos));
        }
    }

    public override void OnHoverOut()
    {
        if (playerHandler.draggedStallObject != null && highlightedFlag)
        {
            highlightedFlag = false;
            StartCoroutine(placeAnim.anim.PlayBackward(placeAnim, placeAnim.originalPos, true));
        }
    }
}
