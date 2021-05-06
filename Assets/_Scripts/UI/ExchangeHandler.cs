using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExchangeHandler : MonoBehaviour
{
    public SimpleButton windowButton;
    public SimpleButton exchangeButton;
    public List<ExchangeSpace> characterSpaces;
    public List<ExchangeSpace> playerSpaces;
    public TweeningAnimator windowAnim;
    public Image characterFace;
    public Image totalInterestFiller;
    public Image totalPersonnalValueFiller;

    private bool isOpened;
    private List<StallObject> stallObjectsSelected;
    private List<CharaObject> charaObjectsSelected;
    private float totalInterest;
    private float totalPersonnalValue;

    void Start()
    {
        windowAnim.GetReferences();
        windowAnim.anim.SetAtEndState(windowAnim);
        stallObjectsSelected = new List<StallObject>();
        charaObjectsSelected = new List<CharaObject>();
        exchangeButton.SetEnable(false);
    }

    void Update()
    {
        UpdateWindow();
        UpdateTrade();
    }

    private void UpdateTrade()
    {
        if (isOpened)
        {
            if (NegoceManager.I.selectedCharacter == null)
            {
                Close();
            }
            else
            {
                foreach (ExchangeSpace playerSpace in playerSpaces)
                {
                    playerSpace.canBeUsed = NegoceManager.I.playerHandler.draggedStallObject != null;

                    if (playerSpace.isClicked)
                    {
                        if (playerSpace.stallObjectHeld != null)
                        {
                            stallObjectsSelected.Remove(playerSpace.stallObjectHeld);
                            playerSpace.stallObjectHeld = null;
                            playerSpace.objectImage.color = Color.clear;
                        }
                    }
                }


                foreach (ExchangeSpace charaSpace in characterSpaces)
                {
                    charaSpace.canBeUsed = NegoceManager.I.draggedCharaObject != null;

                    if (charaSpace.isClicked)
                    {
                        if (charaSpace.charaObjectHeld != null)
                        {
                            charaObjectsSelected.Remove(charaSpace.charaObjectHeld);
                            charaSpace.charaObjectHeld = null;
                            charaSpace.objectImage.color = Color.clear;
                        }
                    }
                }

                if (charaObjectsSelected.Count > 0 && stallObjectsSelected.Count > 0)
                {
                    exchangeButton.SetEnable(true);
                }
                else
                {
                    exchangeButton.SetEnable(false);
                }


                totalInterest = 0;
                foreach (StallObject stallObjectSelected in stallObjectsSelected)
                {
                    totalInterest += NegoceManager.I.selectedCharacter.GetPotentialFromStallObject(stallObjectSelected).interestLevel;
                }

                totalPersonnalValue = 0;
                foreach (CharaObject charaObjectSelected in charaObjectsSelected)
                {
                    totalPersonnalValue += charaObjectSelected.personnalValue;
                }
                totalPersonnalValueFiller.fillAmount = totalPersonnalValue / Mathf.Max(totalPersonnalValue, totalInterest);
                totalInterestFiller.fillAmount = totalInterest / Mathf.Max(totalPersonnalValue, totalInterest);


                if (exchangeButton.canBeUsed && exchangeButton.isClicked)
                {
                    ProposeExchange();
                }
            }
        }
    }

    private void UpdateWindow()
    {
        if (NegoceManager.I.selectedCharacter != null)
        {
            windowButton.SetEnable(true);
            characterFace.color = Color.white;
            characterFace.sprite = NegoceManager.I.selectedCharacter.character.faceSprite;

            if (windowButton.isClicked)
            {
                if (!isOpened)
                {
                    Open();
                }
                else
                {
                    Close();
                }
            }
        }
        else
        {
            windowButton.SetEnable(false);
            exchangeButton.SetEnable(false);
            characterFace.color = Color.clear;
        }
    }

    public void Open()
    {
        if(!isOpened)
        {
            StartCoroutine(windowAnim.anim.PlayBackward(windowAnim, true));
            isOpened = true;
            foreach (ExchangeSpace playerSpace in playerSpaces)
            {
                playerSpace.stallObjectHeld = null;
                playerSpace.objectImage.color = Color.clear;
                stallObjectsSelected.Clear();
            }

            foreach (ExchangeSpace charaSpace in characterSpaces)
            {
                charaSpace.charaObjectHeld = null;
                charaSpace.objectImage.color = Color.clear;
                charaObjectsSelected.Clear();
            }
        }
    }

    public void Close()
    {
        if(isOpened)
        {
            StartCoroutine(windowAnim.anim.Play(windowAnim));
            isOpened = false;
        }
    }

    public void DropStallObject(StallObject stallObjectDropped)
    {
        if(isOpened)
        {
            ExchangeSpace playerSpaceDropOn = null;
            for (int i = 0; i < playerSpaces.Count; i++)
            {
                if (playerSpaces[i].isHovered)
                {
                    playerSpaceDropOn = playerSpaces[i];
                }
            }

            if (playerSpaceDropOn != null && !stallObjectsSelected.Contains(stallObjectDropped))
            {
                stallObjectsSelected.Add(stallObjectDropped);
                playerSpaceDropOn.stallObjectHeld = stallObjectDropped;
                playerSpaceDropOn.objectImage.color = Color.white;
                playerSpaceDropOn.objectImage.sprite = stallObjectDropped.linkedObject.illustration;
            }
        }
    }

    public void DropCharaObject(CharaObject charaObjectDropped)
    {
        if (isOpened)
        {
            ExchangeSpace charaSpaceDropOn = null;
            for (int i = 0; i < characterSpaces.Count; i++)
            {
                if (characterSpaces[i].isHovered)
                {
                    charaSpaceDropOn = characterSpaces[i];
                }
            }

            if (charaSpaceDropOn != null && !charaObjectsSelected.Contains(charaObjectDropped))
            {
                charaObjectsSelected.Add(charaObjectDropped);
                charaSpaceDropOn.charaObjectHeld = charaObjectDropped;
                charaSpaceDropOn.objectImage.color = Color.white;
                charaSpaceDropOn.objectImage.sprite = charaObjectDropped.linkedObject.illustration;
            }
        }
    }

    private void ProposeExchange()
    {
        if(totalInterest >= totalPersonnalValue)
        {
            Close();
            Debug.Log("It's a deal !");

            foreach (CharaObject charaObject in charaObjectsSelected)
            {
                NegoceManager.I.selectedCharacter.belongings.Remove(charaObject);
                Destroy(charaObject.gameObject);
                NegoceManager.I.selectedCharacter.RefreshCharacterDisplay();
            }

            foreach (StallObject stallObject in stallObjectsSelected)
            {
                NegoceManager.I.playerHandler.RemoveStallObject(stallObject);
            }

        }
        else
        {
            Debug.Log("I can't accept this !");
        }
    }


}
