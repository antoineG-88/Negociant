﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropOption : UIInteractable
{
    public TweeningAnimator hoverAnim;
    public TweeningAnimator selectAnim;
    [HideInInspector] public RectTransform rectTransform;
    public GameObject display;
    public Text actionTimeText;

    [HideInInspector] public bool isCurrentlyHoveredCorrectly;
    [HideInInspector] public bool canReceive;
    private bool isPlaying;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        hoverAnim.GetReferences();
        selectAnim.GetReferences();
    }

    void Update()
    {
        if(!canReceive && isCurrentlyHoveredCorrectly)
        {
            isCurrentlyHoveredCorrectly = false;
        }
    }

    public void Enable(string timeText)
    {
        display.SetActive(true);
        canReceive = true;
        actionTimeText.text = timeText;
    }

    public void Disable()
    {
        canReceive = false;
        if (!isPlaying)
        {
            display.SetActive(false);
        }
    }

    public IEnumerator Select()
    {
        StartCoroutine(selectAnim.anim.Play(selectAnim));
        isPlaying = true;
        yield return new WaitForSeconds(selectAnim.anim.animationTime);
        isPlaying = false;
        hoverAnim.anim.SetAtStartState(hoverAnim);
        selectAnim.anim.SetAtStartState(selectAnim);
        Disable();
    }

    public override void OnHoverIn()
    {
        if(canReceive && !isCurrentlyHoveredCorrectly)
        {
            isCurrentlyHoveredCorrectly = true;
            StartCoroutine(hoverAnim.anim.Play(hoverAnim));
        }
    }

    public override void OnHoverOut()
    {
        if (canReceive && isCurrentlyHoveredCorrectly)
        {
            isCurrentlyHoveredCorrectly = false;
            StartCoroutine(hoverAnim.anim.PlayBackward(hoverAnim, true));
        }
    }
}
