using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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
    public List<Sprite> categoriesIcon;
    public List<Color> categoriesColor;
    [HideInInspector] public bool canBeHovered;

    private void Start()
    {
        canBeHovered = true;
        canvasGroup = GetComponent<CanvasGroup>();
        hoverAnim.GetReferences();
        hoverAnim.anim = Instantiate(hoverAnim.anim);
    }


    public void RefreshDisplay()
    {
        nameText.text = linkedObject.objectName;
        illustration.sprite = linkedObject.illustration;
        categoryDisplay1.color = categoriesColor[(int)linkedObject.categories[0]];
        categoryDisplay1.sprite = categoriesIcon[(int)linkedObject.categories[0]];
        categoryDisplay2.color = categoriesColor[(int)linkedObject.categories[1]];
        categoryDisplay2.sprite = categoriesIcon[(int)linkedObject.categories[1]];

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
