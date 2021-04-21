using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBehavior : UIInteractable
{
    public Character character;
    public Image illustrationImage;
    public TweeningAnimator selectionAnim;
    [HideInInspector] public bool isSelected;

    void Start()
    {

    }

    public override void Update()
    {
        if(clickedDown)
        {
            NegoceManager.I.playerHandler.SelectCharacter(this);
        }
        base.Update();
    }

    public void RefreshCharacterDisplay()
    {
        selectionAnim.anim = Instantiate(selectionAnim.anim);
        illustrationImage.sprite = character.illustration;
        illustrationImage.SetNativeSize();
        selectionAnim.GetReferences();
    }

    public void Select()
    {
        if(!isSelected)
        {
            isSelected = true;
            StartCoroutine(selectionAnim.anim.Play(selectionAnim, selectionAnim.originalPos));
        }
    }
    public void UnSelect()
    {
        if (isSelected)
        {
            StartCoroutine(selectionAnim.anim.PlayBackward(selectionAnim, selectionAnim.originalPos, true));
            isSelected = false;
        }
    }

    public override void OnHoverIn()
    {

    }

    public override void OnHoverOut()
    {

    }
}
