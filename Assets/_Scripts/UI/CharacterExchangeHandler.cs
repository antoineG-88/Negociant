using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterExchangeHandler : MonoBehaviour
{
    public CharacterHandler characterHandler;
    public SimpleButton exchangeButton;
    public List<ExchangeSpace> characterSpaces;
    public List<ExchangeSpace> playerSpaces;
    public TweeningAnimator windowAnim;
    public Image totalInterestFiller;
    public Image totalPersonnalValueFiller;
    public SimpleButton closeButton;
    private ArgumentRadialMenu argumentRadialMenu;

    [HideInInspector] public bool isOpened;
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
        closeButton.SetEnable(true);
        argumentRadialMenu = NegoceManager.I.argumentRadialMenu;
    }

    void Update()
    {
        UpdateTrade();

        if(closeButton.isClicked)
        {
            Close();
        }
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

                    /*if (playerSpace.isClicked)    a rremmtre d'une autre manière
                    {
                        if (playerSpace.stallObjectHeld != null)
                        {
                            stallObjectsSelected.Remove(playerSpace.stallObjectHeld);
                            playerSpace.stallObjectHeld = null;
                            playerSpace.objectImage.color = Color.clear;
                        }
                    }*/

                    if(playerSpace.clickedDown && playerSpace.stallObjectHeld != null)
                    {
                        argumentRadialMenu.OpenRadialMenu(playerSpace.stallObjectHeld, characterHandler, playerSpace);
                    }
                }


                foreach (ExchangeSpace charaSpace in characterSpaces)
                {
                    charaSpace.canBeUsed = NegoceManager.I.draggedCharaObject != null;

                    /*if (charaSpace.isClicked)    a rremmtre d'une autre manière
                    {
                        if (charaSpace.charaObjectHeld != null)
                        {
                            charaObjectsSelected.Remove(charaSpace.charaObjectHeld);
                            charaSpace.charaObjectHeld = null;
                            charaSpace.objectImage.color = Color.clear;
                        }
                    }*/
                }

                if (charaObjectsSelected.Count > 0 && stallObjectsSelected.Count > 0)
                {
                    exchangeButton.SetEnable(true);
                }
                else
                {
                    exchangeButton.SetEnable(false);
                }

                /*
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
                */

                if (exchangeButton.canBeUsed && exchangeButton.isClicked)
                {
                    ProposeExchange();
                }
            }
        }
    }

    public void AddObjectToTrade(StallObject addedStallobject, CharaObject addedCharaObject)
    {
        Open();
        if(addedCharaObject != null && !charaObjectsSelected.Contains(addedCharaObject))
        {
            if(charaObjectsSelected.Count < 3)
            {
                bool objectPlaced = false;
                int i = 0;
                while (i < characterSpaces.Count && !objectPlaced)
                {
                    if(characterSpaces[i].charaObjectHeld == null)
                    {
                        charaObjectsSelected.Add(addedCharaObject);
                        characterSpaces[i].charaObjectHeld = addedCharaObject;
                        characterSpaces[i].objectImage.color = Color.white;
                        characterSpaces[i].objectImage.sprite = addedCharaObject.linkedObject.illustration;
                        objectPlaced = true;
                    }
                    i++;
                }
            }
            else
            {
                Debug.Log("can't add chara object to trade");
            }
        }

        if (addedStallobject != null && !stallObjectsSelected.Contains(addedStallobject))
        {
            if (stallObjectsSelected.Count < 3)
            {
                bool objectPlaced = false;
                int i = 0;
                while (i < playerSpaces.Count && !objectPlaced)
                {
                    if (playerSpaces[i].stallObjectHeld == null)
                    {
                        stallObjectsSelected.Add(addedStallobject);
                        playerSpaces[i].stallObjectHeld = addedStallobject;
                        playerSpaces[i].objectImage.color = Color.white;
                        playerSpaces[i].objectImage.sprite = addedStallobject.linkedObject.illustration;
                        objectPlaced = true;
                    }
                    i++;
                }
            }
            else
            {
                Debug.Log("can't add stall object to trade");
            }
        }
    }

    public void Open()
    {
        if(!isOpened)
        {
            StartCoroutine(windowAnim.anim.PlayBackward(windowAnim, true));
            isOpened = true;
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
