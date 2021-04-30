﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StallObject : UIInteractable
{
    public Object linkedObject;
    public Text nameText;
    public Image illustration;
    public TweeningAnimator hoverAnim;
    public RectTransform rectTransform;
    public Text interestLevel;
    public StallSpace stallSpace;
    public CanvasGroup canvasGroup;
    public Image categoryDisplay1;
    public Image categoryDisplay2;
    public Image interestLevelFiller;
    public RectTransform objectDirectInfoPanel;
    [HideInInspector] public float interestLevelToShow;
    public float interestLevelAnimLerpRatio;
    [HideInInspector] public bool canBeHovered;

    private void Start()
    {
        canBeHovered = true;
        canvasGroup = GetComponent<CanvasGroup>();
        hoverAnim.GetReferences();
        hoverAnim.anim = Instantiate(hoverAnim.anim);
    }

    private void Update()
    {
        objectDirectInfoPanel.gameObject.SetActive(!isDragged);

        if(interestLevelToShow < 0)
        {
            interestLevelFiller.gameObject.SetActive(false);
        }
        else
        {
            interestLevelFiller.gameObject.SetActive(true);
            interestLevelFiller.fillAmount = Mathf.Lerp(interestLevelFiller.fillAmount, interestLevelToShow, interestLevelAnimLerpRatio * Time.deltaTime);
        }
    }

    public void SetInterestLevelDisplay(float level, Color interestFillerColor)
    {
        interestLevelFiller.fillAmount = 0;
        interestLevelFiller.color = interestFillerColor;
        interestLevelToShow = level;
    }

    public void RefreshDisplay()
    {
        nameText.text = linkedObject.objectName;
        illustration.sprite = linkedObject.illustration;
        categoryDisplay1.color = GameData.GetCategoryPropertiesFromCategory(linkedObject.categories[0]).color;
        categoryDisplay1.sprite = GameData.GetCategoryPropertiesFromCategory(linkedObject.categories[0]).icon;
        categoryDisplay2.color = GameData.GetCategoryPropertiesFromCategory(linkedObject.categories[1]).color;
        categoryDisplay2.sprite = GameData.GetCategoryPropertiesFromCategory(linkedObject.categories[1]).icon;

    }

    public override void OnHoverIn()
    {
        if(canBeHovered)
        {
            StartCoroutine(hoverAnim.anim.Play(hoverAnim));
        }
        else
        {
            isHovered = false;
        }
    }

    public override void OnHoverOut()
    {
        StartCoroutine(hoverAnim.anim.PlayBackward(hoverAnim, true));
    }
}
