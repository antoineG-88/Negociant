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

    public void Update()
    {
        if(playerHandler.draggedStallObject == null && highlightedFlag)
        {
            highlightedFlag = false;
            StartCoroutine(placeAnim.anim.PlayBackward(placeAnim, true));
        }
    }


    public override void OnHoverIn()
    {
        if(playerHandler.draggedStallObject != null && !highlightedFlag)
        {
            highlightedFlag = true;
            StartCoroutine(placeAnim.anim.Play(placeAnim));
        }
    }

    public override void OnHoverOut()
    {
        if (playerHandler.draggedStallObject != null && highlightedFlag)
        {
            highlightedFlag = false;
            StartCoroutine(placeAnim.anim.PlayBackward(placeAnim, true));
        }
    }
}
