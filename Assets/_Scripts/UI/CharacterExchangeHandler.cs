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
    public TweeningAnimator firstStallSpaceAnim;
    public TweeningAnimator secondStallSpaceAnim;
    public TweeningAnimator thirdStallSpaceAnim;
    public TweeningAnimator firstCharaSpaceAnim;
    public TweeningAnimator secondCharaSpaceAnim;
    public TweeningAnimator thirdCharaSpaceAnim;
    public float interestFillerLerpRatio;
    public TweeningAnimator successExchangeWindowAnim;
    public TweeningAnimator failExchangeWindowAnim;

    private ArgumentRadialMenu argumentRadialMenu;
    [HideInInspector] public bool isOpened;
    private List<StallObject> stallObjectsSelected;
    private List<CharaObject> charaObjectsSelected;
    private float totalInterest;
    private float totalPersonnalValue;
    private int lastNumberOfStallObjectSelected;
    private int lastNumberOfCharaObjectSelected;


    void Start()
    {
        successExchangeWindowAnim.anim = Instantiate(successExchangeWindowAnim.anim);
        failExchangeWindowAnim.anim = Instantiate(failExchangeWindowAnim.anim);
        successExchangeWindowAnim.GetReferences();
        failExchangeWindowAnim.GetReferences();
        successExchangeWindowAnim.anim.SetAtStartState(successExchangeWindowAnim);
        windowAnim.GetReferences();
        windowAnim.anim.SetAtEndState(windowAnim);
        stallObjectsSelected = new List<StallObject>();
        charaObjectsSelected = new List<CharaObject>();
        exchangeButton.SetEnable(false);
        closeButton.SetEnable(true);
        argumentRadialMenu = NegoceManager.I.argumentRadialMenu;
        firstStallSpaceAnim.GetReferences();
        firstStallSpaceAnim.anim.SetAtStartState(firstStallSpaceAnim);

        secondStallSpaceAnim.anim = Instantiate(secondStallSpaceAnim.anim);
        secondStallSpaceAnim.GetReferences();
        secondStallSpaceAnim.anim.SetAtStartState(secondStallSpaceAnim);

        thirdStallSpaceAnim.anim = Instantiate(thirdStallSpaceAnim.anim);
        thirdStallSpaceAnim.GetReferences();
        thirdStallSpaceAnim.anim.SetAtStartState(thirdStallSpaceAnim);

        firstCharaSpaceAnim.GetReferences();
        firstCharaSpaceAnim.anim.SetAtStartState(firstCharaSpaceAnim);
        
        secondCharaSpaceAnim.anim = Instantiate(secondCharaSpaceAnim.anim);
        secondCharaSpaceAnim.GetReferences();
        secondCharaSpaceAnim.anim.SetAtStartState(secondCharaSpaceAnim);

        thirdCharaSpaceAnim.anim = Instantiate(thirdCharaSpaceAnim.anim);
        thirdCharaSpaceAnim.GetReferences();
        thirdCharaSpaceAnim.anim.SetAtStartState(thirdCharaSpaceAnim);
        lastNumberOfStallObjectSelected = -1;
        lastNumberOfCharaObjectSelected = -1;
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
            if(lastNumberOfStallObjectSelected != stallObjectsSelected.Count)
            {
                if(lastNumberOfStallObjectSelected == -1)
                {
                    secondStallSpaceAnim.anim.SetAtStartState(secondStallSpaceAnim);
                    thirdStallSpaceAnim.anim.SetAtStartState(thirdStallSpaceAnim);
                }

                if(stallObjectsSelected.Count == 1 && lastNumberOfStallObjectSelected > 1)
                {
                    StartCoroutine(firstStallSpaceAnim.anim.PlayBackward(firstStallSpaceAnim, true));
                    StartCoroutine(secondStallSpaceAnim.anim.PlayBackward(secondStallSpaceAnim, true));
                    StartCoroutine(thirdStallSpaceAnim.anim.PlayBackward(thirdStallSpaceAnim, true));
                }

                if(stallObjectsSelected.Count > 1 && lastNumberOfStallObjectSelected == 1)
                {
                    StartCoroutine(firstStallSpaceAnim.anim.Play(firstStallSpaceAnim));
                    StartCoroutine(secondStallSpaceAnim.anim.Play(secondStallSpaceAnim));
                    StartCoroutine(thirdStallSpaceAnim.anim.Play(thirdStallSpaceAnim));
                }

                lastNumberOfStallObjectSelected = stallObjectsSelected.Count;
            }

            if (lastNumberOfCharaObjectSelected != charaObjectsSelected.Count)
            {
                if (lastNumberOfCharaObjectSelected == -1)
                {
                    secondCharaSpaceAnim.anim.SetAtStartState(secondCharaSpaceAnim);
                    thirdCharaSpaceAnim.anim.SetAtStartState(thirdCharaSpaceAnim);
                }

                if (charaObjectsSelected.Count == 1 && lastNumberOfCharaObjectSelected > 1)
                {
                    StartCoroutine(firstCharaSpaceAnim.anim.PlayBackward(firstCharaSpaceAnim, true));
                    StartCoroutine(secondCharaSpaceAnim.anim.PlayBackward(secondCharaSpaceAnim, true));
                    StartCoroutine(thirdCharaSpaceAnim.anim.PlayBackward(thirdCharaSpaceAnim, true));
                }

                if (charaObjectsSelected.Count > 1 && lastNumberOfCharaObjectSelected == 1)
                {
                    StartCoroutine(firstCharaSpaceAnim.anim.Play(firstCharaSpaceAnim));
                    StartCoroutine(secondCharaSpaceAnim.anim.Play(secondCharaSpaceAnim));
                    StartCoroutine(thirdCharaSpaceAnim.anim.Play(thirdCharaSpaceAnim));
                }

                lastNumberOfCharaObjectSelected = charaObjectsSelected.Count;
            }

            foreach (ExchangeSpace playerSpace in playerSpaces)
            {
                playerSpace.canBeUsed = NegoceManager.I.playerHandler.draggedStallObject != null;
                playerSpace.interestFiller.fillAmount = Mathf.Lerp(playerSpace.interestFiller.fillAmount, (playerSpace.stallObjectHeld == null ? 0 : characterHandler.GetPotentialFromStallObject(playerSpace.stallObjectHeld).interestLevel) / characterHandler.maxPersonnalValue, interestFillerLerpRatio * Time.deltaTime);

                if (playerSpace.clickedDown && playerSpace.stallObjectHeld != null)
                {
                    argumentRadialMenu.OpenRadialMenu(playerSpace.stallObjectHeld, characterHandler, playerSpace);
                }
            }


            foreach (ExchangeSpace charaSpace in characterSpaces)
            {
                charaSpace.canBeUsed = NegoceManager.I.draggedCharaObject != null;

                if (charaSpace.isClicked)
                {
                    RemoveCharaObjectFromTrade(charaSpace);
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
            float knownTotalPersonnalValue = 0;
            foreach (CharaObject charaObjectSelected in charaObjectsSelected)
            {
                totalPersonnalValue += charaObjectSelected.personnalValue;
                knownTotalPersonnalValue += charaObjectSelected.isPersonnalValueKnown ? charaObjectSelected.personnalValue : 0;
            }
            totalPersonnalValueFiller.fillAmount = Mathf.Lerp(totalPersonnalValueFiller.fillAmount, knownTotalPersonnalValue / characterHandler.maxPersonnalValue, Time.deltaTime * 5);
            totalInterestFiller.fillAmount =  Mathf.Lerp(totalInterestFiller.fillAmount, totalInterest / characterHandler.maxPersonnalValue, Time.deltaTime * 5);

            if (exchangeButton.canBeUsed && exchangeButton.isClicked)
            {
                ProposeExchange();
            }
        }
    }

    public void AddObjectToTrade(StallObject addedStallobject, CharaObject addedCharaObject)
    {
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

    public void RemoveStallObjectFromTrade(ExchangeSpace playerSpace)
    {
        if (playerSpace.stallObjectHeld != null)
        {
            stallObjectsSelected.Remove(playerSpace.stallObjectHeld);
            playerSpace.stallObjectHeld = null;
            playerSpace.objectImage.color = Color.clear;

            if (stallObjectsSelected.Count == 1 && playerSpace == playerSpaces[0])
            {
                StallObject stallObjectToMove = null;
                for (int i = 0; i < playerSpaces.Count; i++)
                {
                    if(playerSpaces[i].stallObjectHeld != null)
                    {
                        stallObjectToMove = playerSpaces[i].stallObjectHeld;
                        stallObjectsSelected.Remove(playerSpaces[i].stallObjectHeld);
                        playerSpaces[i].stallObjectHeld = null;
                        playerSpaces[i].objectImage.color = Color.clear;
                    }
                }
                AddObjectToTrade(stallObjectToMove, null);
            }
        }
    }
    public void RemoveCharaObjectFromTrade(ExchangeSpace charaSpace)
    {
        if (charaSpace.charaObjectHeld != null)
        {
            charaObjectsSelected.Remove(charaSpace.charaObjectHeld);
            charaSpace.stallObjectHeld = null;
            charaSpace.objectImage.color = Color.clear;

            if (charaObjectsSelected.Count == 1 && charaSpace == characterSpaces[0])
            {
                CharaObject charaObjectToMove = null;
                for (int i = 0; i < characterSpaces.Count; i++)
                {
                    if (characterSpaces[i].charaObjectHeld != null)
                    {
                        charaObjectToMove = characterSpaces[i].charaObjectHeld;

                        charaObjectsSelected.Remove(characterSpaces[i].charaObjectHeld);
                        characterSpaces[i].charaObjectHeld = null;
                        characterSpaces[i].objectImage.color = Color.clear;
                    }
                }
                AddObjectToTrade(null, charaObjectToMove);
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
            characterHandler.Speak("Ca me parait équitable_, j'accepte l'échange !", 4);
            StartCoroutine(SuccesEffect());
        }
        else
        {
            characterHandler.Speak("Je ne peux pas accepter ça", 3);
            StartCoroutine(FailEffect());
        }
    }

    private IEnumerator SuccesEffect()
    {
        StartCoroutine(successExchangeWindowAnim.anim.Play(successExchangeWindowAnim));
        yield return new WaitForSeconds(successExchangeWindowAnim.anim.animationTime);
        successExchangeWindowAnim.anim.SetAtStartState(successExchangeWindowAnim);
        Exchange();
        Close();
    }
    private IEnumerator FailEffect()
    {
        StartCoroutine(failExchangeWindowAnim.anim.Play(failExchangeWindowAnim));
        yield return new WaitForSeconds(failExchangeWindowAnim.anim.animationTime);
        successExchangeWindowAnim.anim.SetAtStartState(successExchangeWindowAnim);
    }

    private void Exchange()
    {
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
}
