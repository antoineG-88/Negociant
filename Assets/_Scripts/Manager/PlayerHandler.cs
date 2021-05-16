using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerHandler : MonoBehaviour
{
    //public List<Object> initialPlayerBelongings;
    [Header("Stall")]
    public RectTransform stallRectTransform; 
    public float distanceBetweenStandObject;
    public List<StallSpace> allStallSpaces;
    public int vitrineStallSpacesAvailable;
    public int backStallSpacesAvailable;
    [Header("Actions")]
    public string presentSpokenText;
    public float presentTime;
    public TMP_Text currentSpokenText;
    public TweeningAnimator speakBoxAnim;
    public string[] askSpokenTexts;
    public float askTime;
    public float speechPauseTime;
    public float speechTimeBetweenSentences;
    public string welcomeSpeech;
    [Header("Reference")]
    //[HideInInspector] public PlayerInventory playerInventory;
    public Text objectInfoNameText;
    public Text objectInfoTitleText;
    public Text objectInfoCategory1Text;
    public Text objectInfoCategory2Text;
    public Image objectInfoCategory1Icon;
    public Image objectInfoCategory2Icon;
    public Text objectInfoDescriptionText;
    public Text objectInfoOriginText;
    public Image objectInfoIllustration;
    public RectTransform featureContentRect;
    public RectTransform[] featuresInfo;
    public Image objectInfoHeaderBack;
    public Image objectInfoBodyBack;
    public Image objectInfoFeatureBack;
    public Color[] stallObjectWindowTheme;
    public Color[] charaObjectWindowTheme;
    public StallObject stallObjectPrefab;
    [Space]
    public TweeningAnimator objectInfoPanel;
    public RectTransform characterInteractionPanel;
    public DropOption presentOption;
    public DropOption exchangeOption;

    [HideInInspector] public List<StallObject> allStallObjects;
    private bool atleastOneHovered;
    private bool atLeastOneClicked;
    [HideInInspector] public StallObject selectedStallObject;
    private StallObject previousSelectedObject;
    [HideInInspector] public CharaObject selectedCharaObject;
    private CharaObject previousSelectedCharaObject;
    private StallObject hoveredStallObject;
    private StallObject previousHoveredObject;
    [HideInInspector] public StallObject draggedStallObject;
    private int vitrineSpaceNumber;

    private StallObject presentedStallObject;
    private StallObject argumentedStallObject;
    private CharaObject askedCharaObject;
    private CharacterHandler currentCharacterTalkingTo;
    private Object.Feature featureArgumented;
    private Speech currentSpeech;
    private bool isWaitingForResponse;
    private bool isTalking;

    private void Start()
    {
        objectInfoPanel.GetReferences();
        objectInfoPanel.canvasGroup.interactable = false;
        objectInfoPanel.canvasGroup.blocksRaycasts = false;
        objectInfoPanel.anim.SetAtEndState(objectInfoPanel);
        presentOption.Disable();
        exchangeOption.Disable();
        characterInteractionPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        speakBoxAnim.anim = Instantiate(speakBoxAnim.anim);
        speakBoxAnim.GetReferences();
        speakBoxAnim.anim.SetAtEndState(speakBoxAnim);

        foreach (StallSpace stallSpace in allStallSpaces)
        {
            if(stallSpace.isVitrine)
            {
                vitrineSpaceNumber++;
            }
        }
        //InitPlayerInfo();
        InitStallLayout();
    }

    private void Update()
    {
        UpdateCurrentAction();
        UpdateObjectInteraction();
    }

    public void UpdateCurrentAction()
    {
        if (isTalking)
        {
            if( isWaitingForResponse)
            {
                currentCharacterTalkingTo.RefreshEnthousiasm();
            }

            currentSpokenText.text = currentSpeech.GetCurrentSpeechProgression(Time.deltaTime);

            if (currentSpeech.isFinished)
            {
                isTalking = false;
                StartCoroutine(speakBoxAnim.anim.Play(speakBoxAnim));
                if (isWaitingForResponse)
                {
                    isWaitingForResponse = false;
                    if (presentedStallObject != null)
                    {
                        currentCharacterTalkingTo.PresentObject(presentedStallObject);
                        currentCharacterTalkingTo.isListening = false;
                        presentedStallObject = null;
                    }
                    else if (argumentedStallObject != null)
                    {
                        currentCharacterTalkingTo.ArgumentFeature(featureArgumented, argumentedStallObject);
                        currentCharacterTalkingTo.isListening = false;
                        currentCharacterTalkingTo = null;
                        argumentedStallObject = null;
                        featureArgumented = null;
                    }
                    else if (askedCharaObject != null)
                    {
                        currentCharacterTalkingTo.AskAbout(askedCharaObject);
                        currentCharacterTalkingTo.isListening = false;
                        askedCharaObject = null;
                    }
                }
            }
        }
    }

    public bool IsPlayerTalking()
    {
        return isWaitingForResponse;
    }

    public void PresentObject(StallObject stallObjectToPresent, CharacterHandler targetCharacter)
    {
        currentCharacterTalkingTo = targetCharacter;
        currentCharacterTalkingTo.isListening = true;
        presentedStallObject = stallObjectToPresent;
        currentCharacterTalkingTo.Interrupt();
        isWaitingForResponse = true;
        Speak(presentSpokenText + stallObjectToPresent.linkedObject.possessivePronom + stallObjectToPresent.linkedObject.objectName + ", " + targetCharacter.character.characterName + " ?", presentTime);
    }

    public void ArgumentFeature(StallObject stallObjectArgumented, CharacterHandler targetCharacter, Object.Feature argumentedFeature)
    {
        currentCharacterTalkingTo = targetCharacter;
        currentCharacterTalkingTo.isListening = true;
        argumentedStallObject = stallObjectArgumented;
        featureArgumented = argumentedFeature;
        currentCharacterTalkingTo.Interrupt();
        isWaitingForResponse = true;
        Speak(argumentedFeature.argumentSpokenText, argumentedFeature.argumentSpeakTime);
    }
    public void AskAbout(CharaObject charaObjectAsked, CharacterHandler targetCharacter)
    {
        currentCharacterTalkingTo = targetCharacter;
        currentCharacterTalkingTo.isListening = true;
        askedCharaObject = charaObjectAsked;
        currentCharacterTalkingTo.Interrupt();
        isWaitingForResponse = true;
        Speak(askSpokenTexts[Random.Range(0, askSpokenTexts.Length)], askTime);
    }

    public void Speak(string speechToSpeak, float speechTime)
    {
        isTalking = true;
        currentSpeech =  new Speech(speechToSpeak);
        currentSpeech.InitSpeechStructure(speechPauseTime, speechTimeBetweenSentences, speechTime);
        currentSpeech.ResetProgression();
        StartCoroutine(speakBoxAnim.anim.PlayBackward(speakBoxAnim, true));
    }

    private void UpdateObjectInteraction()
    {
        HoverStallObjectUpdate();

        SelectionStallObjectUpdate();

        DragAndDropStallObjectUpdate();
    }

    CharacterHandler charaHovered;
    private void DragAndDropStallObjectUpdate()
    {
        for (int i = 0; i < allStallObjects.Count; i++)
        {
            if (draggedStallObject == null && allStallObjects[i].isDragged)
            {
                draggedStallObject = allStallObjects[i];
            }
        }

        if (draggedStallObject != null)
        {
            foreach (StallObject stallObject in allStallObjects)
            {
                stallObject.canvasGroup.blocksRaycasts = false;
            }
            charaHovered = null;
            foreach (CharacterHandler charaPresent in NegoceManager.I.allPresentCharacters)
            {
                if(charaPresent.isHoveredWithStallObject)
                {
                    charaHovered = charaPresent;
                }
            }

            Vector3 objectPosToFollow = Input.mousePosition;

            if (charaHovered != null && !charaHovered.isListening && !charaHovered.isThinking)
            {
                characterInteractionPanel.position = charaHovered.rectTransform.position;

                if (!isWaitingForResponse)
                {
                    presentOption.Enable(presentTime.ToString() + " s.");
                    exchangeOption.Enable("");
                    characterInteractionPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
                else
                {
                    characterInteractionPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    presentOption.Disable();
                    exchangeOption.Disable();
                }

                if (presentOption.isCurrentlyHoveredCorrectly)
                {
                    objectPosToFollow = presentOption.rectTransform.position;
                }
                if (exchangeOption.isCurrentlyHoveredCorrectly)
                {
                    objectPosToFollow = exchangeOption.rectTransform.position;
                }
            }
            else
            {
                characterInteractionPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                presentOption.Disable();
                exchangeOption.Disable();
            }

            draggedStallObject.rectTransform.position = objectPosToFollow;

            if (Input.GetMouseButtonUp(0))
            {
                if (presentOption.isCurrentlyHoveredCorrectly)
                {
                    StartCoroutine(presentOption.Select());
                    PresentObject(draggedStallObject, charaHovered);
                }
                else if (exchangeOption.isCurrentlyHoveredCorrectly)
                {
                    charaHovered.characterExchangeHandler.AddObjectToTrade(draggedStallObject, null);
                    charaHovered.characterExchangeHandler.Open();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                foreach(CharacterHandler presentCharacter in NegoceManager.I.allPresentCharacters)
                {
                    presentCharacter.characterExchangeHandler.DropStallObject(draggedStallObject);
                }

                draggedStallObject.StopDrag();
                bool droppedOnStallSpace = false;
                foreach (StallSpace hoveredStallSpace in allStallSpaces)
                {
                    if (hoveredStallSpace.isHovered)
                    {
                        droppedOnStallSpace = true;
                        PlaceStallObjectInStallSpace(draggedStallObject, hoveredStallSpace);
                    }
                }

                if (!droppedOnStallSpace)
                {
                    draggedStallObject.rectTransform.position = draggedStallObject.stallSpace.rectTransform.position;
                }

                draggedStallObject.isDragged = false;
                draggedStallObject = null;
            }
        }
        else
        {
            characterInteractionPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
            presentOption.Disable();
            exchangeOption.Disable();
            foreach (StallObject stallObject in allStallObjects)
            {
                stallObject.canvasGroup.blocksRaycasts = true;
            }
        }
    }
    private void SelectionStallObjectUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            atLeastOneClicked = false;
            for (int i = 0; i < allStallObjects.Count; i++)
            {
                if (allStallObjects[i].isHovered)
                {
                    if (selectedStallObject != null)
                    {
                        previousSelectedObject = selectedStallObject;
                    }
                    selectedStallObject = allStallObjects[i];

                    if (previousSelectedObject != selectedStallObject || selectedCharaObject != null)
                    {
                        selectedCharaObject = null;
                        previousSelectedCharaObject = null;
                        objectInfoPanel.canvasGroup.interactable = true;
                        objectInfoPanel.canvasGroup.blocksRaycasts = true;
                        StartCoroutine(objectInfoPanel.anim.PlayBackward(objectInfoPanel, true));
                        UpdateObjectInfoWindow(selectedStallObject);
                    }
                    else
                    {
                        selectedStallObject = null;
                        previousSelectedObject = null;
                        objectInfoPanel.canvasGroup.interactable = false;
                        objectInfoPanel.canvasGroup.blocksRaycasts = false;
                        StartCoroutine(objectInfoPanel.anim.Play(objectInfoPanel));
                    }
                    atLeastOneClicked = true;
                }
            }

            foreach(CharacterHandler presentCharacter in NegoceManager.I.allPresentCharacters)
            {
                for (int y = 0; y < presentCharacter.belongings.Count; y++)
                {
                    if (presentCharacter.belongings[y].isHovered)
                    {
                        if (selectedCharaObject != null)
                        {
                            previousSelectedCharaObject = selectedCharaObject;
                        }
                        selectedCharaObject = presentCharacter.belongings[y];

                        if (previousSelectedCharaObject != selectedCharaObject || selectedStallObject != null)
                        {
                            selectedStallObject = null;
                            previousSelectedObject = null;
                            objectInfoPanel.canvasGroup.interactable = true;
                            objectInfoPanel.canvasGroup.blocksRaycasts = true;
                            StartCoroutine(objectInfoPanel.anim.PlayBackward(objectInfoPanel, true));
                            UpdateObjectInfoWindow(selectedCharaObject);
                        }
                        else
                        {
                            selectedCharaObject = null;
                            previousSelectedCharaObject = null;
                            objectInfoPanel.canvasGroup.interactable = false;
                            objectInfoPanel.canvasGroup.blocksRaycasts = false;
                            StartCoroutine(objectInfoPanel.anim.Play(objectInfoPanel));
                        }
                    }
                }
            }
        }
    }

    private void HoverStallObjectUpdate()
    {
        atleastOneHovered = false;
        for (int i = 0; i < allStallObjects.Count; i++)
        {
            if (allStallObjects[i].isHovered)
            {
                if (hoveredStallObject != null)
                {
                    previousHoveredObject = hoveredStallObject;
                }
                hoveredStallObject = allStallObjects[i];
                atleastOneHovered = true;
            }
        }

        if (atleastOneHovered && previousHoveredObject != hoveredStallObject)
        {
            //StartCoroutine(objectInfoWindowAnimator.anim.Play(objectInfoWindowAnimator, objectInfoWindowAnimator.originalPos));
        }
        if (!atleastOneHovered && hoveredStallObject != null)
        {
            hoveredStallObject = null;
            previousHoveredObject = null;
            //StartCoroutine(objectInfoWindowAnimator.anim.PlayBackward(objectInfoWindowAnimator, objectInfoWindowAnimator.originalPos, true));
        }
    }

    private void PlaceStallObjectInStallSpace(StallObject movedStallObject, StallSpace targetStallSpace)
    {
        StallSpace movedObjectStallSpace = movedStallObject.stallSpace;
        if (targetStallSpace.stallObject != null)
        {
            targetStallSpace.stallObject.stallSpace = movedObjectStallSpace;
            movedObjectStallSpace.stallObject = targetStallSpace.stallObject;
            movedObjectStallSpace.stallObject.rectTransform.position = movedObjectStallSpace.rectTransform.position;
        }
        else
        {
            movedObjectStallSpace.stallObject = null;
        }
        movedStallObject.rectTransform.position = targetStallSpace.rectTransform.position;
        targetStallSpace.stallObject = movedStallObject;
        movedStallObject.stallSpace = targetStallSpace;
    }

    public void RemoveStallObject(StallObject stallObjectToRemove)
    {
        stallObjectToRemove.stallSpace.stallObject = null;
        allStallObjects.Remove(stallObjectToRemove);
        Destroy(stallObjectToRemove.gameObject);
        foreach (CharacterHandler characterHandler in NegoceManager.I.allPresentCharacters)
        {
            characterHandler.RefreshPotentialObjects();
        }
        SaveLoader.I.SavePlayerObjects();
    }

    public void UpdateObjectInfoWindow(StallObject shownObject)
    {
        objectInfoNameText.text = shownObject.linkedObject.objectName;
        objectInfoTitleText.text = shownObject.linkedObject.title;
        objectInfoCategory1Text.text = shownObject.linkedObject.categories[0].ToString();
        objectInfoCategory1Icon.sprite = GameData.GetCategoryPropertiesFromCategory(shownObject.linkedObject.categories[0]).icon;
        objectInfoCategory1Icon.color = GameData.GetCategoryPropertiesFromCategory(shownObject.linkedObject.categories[0]).color;

        if (shownObject.linkedObject.categories.Count > 1)
        {
            objectInfoCategory2Icon.gameObject.SetActive(true);
            objectInfoCategory2Text.text = shownObject.linkedObject.categories[1].ToString();
            objectInfoCategory2Icon.sprite = GameData.GetCategoryPropertiesFromCategory(shownObject.linkedObject.categories[1]).icon;
            objectInfoCategory2Icon.color = GameData.GetCategoryPropertiesFromCategory(shownObject.linkedObject.categories[1]).color;
        }
        else
        {
            objectInfoCategory2Icon.gameObject.SetActive(false);
        }
        objectInfoDescriptionText.text = shownObject.linkedObject.description;
        objectInfoOriginText.text = shownObject.linkedObject.originDescription;
        objectInfoIllustration.sprite = shownObject.linkedObject.illustration;

        for (int i = 0; i < featuresInfo.Length; i++)
        {
            if(i + (shownObject.linkedObject.categories.Count > 1 ? 2 : 1) < shownObject.linkedObject.features.Count)
            {
                featuresInfo[i].gameObject.SetActive(true);
                featuresInfo[i].GetChild(0).GetChild(0).GetComponent<Text>().text = shownObject.linkedObject.features[i + (shownObject.linkedObject.categories.Count > 1 ? 2 : 1)].argumentTitle;
                featuresInfo[i].GetChild(1).GetComponent<Text>().text = shownObject.linkedObject.features[i + (shownObject.linkedObject.categories.Count > 1 ? 2 : 1)].description;
            }
            else
            {
                featuresInfo[i].gameObject.SetActive(false);
            }
        }
        featureContentRect.sizeDelta = new Vector2(0, (shownObject.linkedObject.features.Count - (shownObject.linkedObject.categories.Count > 1 ? 2 : 1)) * 130 + 10);
        objectInfoHeaderBack.color = stallObjectWindowTheme[0];
        objectInfoBodyBack.color = stallObjectWindowTheme[1];
        objectInfoFeatureBack.color = stallObjectWindowTheme[2];
    }

    public void UpdateObjectInfoWindow(CharaObject shownObject)
    {
        objectInfoIllustration.sprite = shownObject.linkedObject.illustration;

        objectInfoCategory1Text.text = shownObject.linkedObject.categories[0].ToString();
        objectInfoCategory1Icon.sprite = GameData.GetCategoryPropertiesFromCategory(shownObject.linkedObject.categories[0]).icon;
        objectInfoCategory1Icon.color = GameData.GetCategoryPropertiesFromCategory(shownObject.linkedObject.categories[0]).color;

        if (shownObject.linkedObject.categories.Count > 1)
        {
            objectInfoCategory2Icon.gameObject.SetActive(true);
            objectInfoCategory2Text.text = shownObject.linkedObject.categories[1].ToString();
            objectInfoCategory2Icon.sprite = GameData.GetCategoryPropertiesFromCategory(shownObject.linkedObject.categories[1]).icon;
            objectInfoCategory2Icon.color = GameData.GetCategoryPropertiesFromCategory(shownObject.linkedObject.categories[1]).color;
        }
        else
        {
            objectInfoCategory2Icon.gameObject.SetActive(false);
        }

        if(shownObject.isPersonnalValueKnown)
        {
            objectInfoNameText.text = shownObject.linkedObject.objectName;
            objectInfoTitleText.text = shownObject.linkedObject.title;
            objectInfoDescriptionText.text = shownObject.linkedObject.description;
            objectInfoOriginText.text = shownObject.linkedObject.originDescription;
        }
        else
        {
            objectInfoNameText.text = "???";
            objectInfoTitleText.text = "";
            objectInfoDescriptionText.text = "";
            objectInfoOriginText.text = "";
        }


        for (int i = 0; i < featuresInfo.Length; i++)
        {
            if (i + (shownObject.linkedObject.categories.Count > 1 ? 2 : 1) < shownObject.linkedObject.features.Count && shownObject.isPersonnalValueKnown)
            {
                featuresInfo[i].gameObject.SetActive(true);
                featuresInfo[i].GetChild(0).GetChild(0).GetComponent<Text>().text = shownObject.linkedObject.features[i + (shownObject.linkedObject.categories.Count > 1 ? 2 : 1)].argumentTitle;
                featuresInfo[i].GetChild(1).GetComponent<Text>().text = shownObject.linkedObject.features[i + (shownObject.linkedObject.categories.Count > 1 ? 2 : 1)].description;
            }
            else
            {
                featuresInfo[i].gameObject.SetActive(false);
            }
        }
        featureContentRect.sizeDelta = new Vector2(0, (shownObject.linkedObject.features.Count - (shownObject.linkedObject.categories.Count > 1 ? 2 : 1)) * 130 + 10);

        objectInfoHeaderBack.color = charaObjectWindowTheme[0];
        objectInfoBodyBack.color = charaObjectWindowTheme[1];
        objectInfoFeatureBack.color = charaObjectWindowTheme[2];
    }

    private int halfObjectNumber;
    private void InitStallLayout()
    {
        allStallObjects = new List<StallObject>();
        StallObject newStallObject;
        halfObjectNumber = SaveLoader.I.playerSave.playerOwnedObjects.Count / 2;
        foreach(StallSpace stallSpace in allStallSpaces)
        {
            stallSpace.Init();
        }
        for (int o = 0; o < SaveLoader.I.playerSave.playerOwnedObjects.Count; o++)
        {
            newStallObject = Instantiate(stallObjectPrefab, stallRectTransform);

            if(o < halfObjectNumber)
            {
                newStallObject.GetComponent<RectTransform>().position = allStallSpaces[o].rectTransform.position;
                newStallObject.stallSpace = allStallSpaces[o];
                newStallObject.stallSpace.stallObject = newStallObject;
            }
            else
            {
                int realo = o - halfObjectNumber + vitrineSpaceNumber;
                newStallObject.GetComponent<RectTransform>().position = allStallSpaces[realo].rectTransform.position;
                newStallObject.stallSpace = allStallSpaces[realo];
                newStallObject.stallSpace.stallObject = newStallObject;
            }
            newStallObject.linkedObject = SaveLoader.I.GetObjectFromName(SaveLoader.I.playerSave.playerOwnedObjects[o]);
            newStallObject.rectTransform = newStallObject.GetComponent<RectTransform>();
            newStallObject.name = newStallObject.linkedObject.objectName;
            newStallObject.RefreshDisplay();
            allStallObjects.Add(newStallObject);
        }


        for (int i = 0; i < allStallSpaces.Count; i++)
        {
            if((i >= vitrineStallSpacesAvailable && i < vitrineSpaceNumber) || (i >= vitrineSpaceNumber && i >= (backStallSpacesAvailable + vitrineSpaceNumber)))
            {
                allStallSpaces[i].gameObject.SetActive(false);
            }
        }
    }

    public void AddStallObject(Object newObject)
    {
        StallSpace stallSpace = null;
        foreach(StallSpace stallSpace1 in allStallSpaces)
        {
            if(stallSpace1.stallObject == null && stallSpace1.gameObject.activeSelf)
            {
                stallSpace = stallSpace1;
            }
        }
        StallObject newStallObject;
        newStallObject = Instantiate(stallObjectPrefab, stallRectTransform);
        newStallObject.rectTransform = newStallObject.GetComponent<RectTransform>();
        newStallObject.stallSpace = stallSpace;
        newStallObject.stallSpace.stallObject = newStallObject;
        newStallObject.rectTransform.position = stallSpace.rectTransform.position;
        newStallObject.linkedObject = newObject;
        newStallObject.name = newStallObject.linkedObject.objectName;
        newStallObject.RefreshDisplay();
        allStallObjects.Add(newStallObject);

        foreach (CharacterHandler characterHandler in NegoceManager.I.allPresentCharacters)
        {
            characterHandler.RefreshPotentialObjects();
        }

        SaveLoader.I.SavePlayerObjects();
    }
}
