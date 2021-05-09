using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandler : MonoBehaviour
{
    [Header("Temporary")]
    public List<Object> initialPlayerBelongings;
    [Header("Stall")]
    public RectTransform stallRectTransform; 
    public float distanceBetweenStandObject;
    public List<StallSpace> allStallSpaces;
    public int vitrineStallSpacesAvailable;
    public int backStallSpacesAvailable;
    [Header("Actions")]
    public string presentSpokenText;
    public float presentTime;
    public Text currentSpokenText;
    public TweeningAnimator speakBoxAnim;
    public string askSpokenText;
    public float askTime;
    public float speechPauseTime;
    public float speechTimeBetweenSentences;
    [Header("Reference")]
    public ArgumentRadialMenu argumentRadialMenu;   
    [HideInInspector] public PlayerInventory playerInventory;
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
    public StallObject stallObjectPrefab;
    [Space]
    public TweeningAnimator objectInfoPanel;
    public RectTransform characterInteractionPanel;
    public DropOption presentOption;
    public DropOption argumentOption;

    [HideInInspector] public List<StallObject> allStallObjects;
    private bool atleastOneHovered;
    private bool atLeastOneClicked;
    private StallObject selectedStallObject;
    private StallObject previousSelectedObject;
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
    private bool isTalking;

    private void Start()
    {
        objectInfoPanel.GetReferences();
        objectInfoPanel.canvasGroup.interactable = false;
        objectInfoPanel.canvasGroup.blocksRaycasts = false;
        objectInfoPanel.anim.SetAtEndState(objectInfoPanel);
        presentOption.Disable();
        argumentOption.Disable();
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
        InitPlayerInfo();
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
            currentCharacterTalkingTo.RefreshEnthousiasm();

            currentSpokenText.text = currentSpeech.GetCurrentSpeechProgression(Time.deltaTime);

            if (currentSpeech.isFinished)
            {
                isTalking = false;
                StartCoroutine(speakBoxAnim.anim.Play(speakBoxAnim));

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

    public bool IsPlayerTalking()
    {
        return isTalking;
    }

    public void PresentObject(StallObject stallObjectToPresent, CharacterHandler targetCharacter)
    {
        currentCharacterTalkingTo = targetCharacter;
        currentCharacterTalkingTo.isListening = true;
        presentedStallObject = stallObjectToPresent;
        Speak(presentSpokenText + stallObjectToPresent.linkedObject.objectName + ", " + targetCharacter.character.characterName + " ?", presentTime);
    }

    public void ArgumentFeature(StallObject stallObjectArgumented, CharacterHandler targetCharacter, Object.Feature argumentedFeature)
    {
        currentCharacterTalkingTo = targetCharacter;
        currentCharacterTalkingTo.isListening = true;
        argumentedStallObject = stallObjectArgumented;
        featureArgumented = argumentedFeature;
        Speak(argumentedFeature.argumentSpokenText, argumentedFeature.argumentSpeakTime);
    }
    public void AskAbout(CharaObject charaObjectAsked, CharacterHandler targetCharacter)
    {
        currentCharacterTalkingTo = targetCharacter;
        currentCharacterTalkingTo.isListening = true;
        askedCharaObject = charaObjectAsked;
        Speak(askSpokenText, askTime);
    }

    private void Speak(string speechToSpeak, float speechTime)
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

            if (charaHovered != null && !charaHovered.isListening && !charaHovered.isThinking && !charaHovered.isSpeaking)
            {
                characterInteractionPanel.position = charaHovered.rectTransform.position;

                if (!isTalking)
                {
                    presentOption.Enable(presentTime.ToString() + " s.");
                    argumentOption.Enable("");
                    characterInteractionPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
                else
                {
                    characterInteractionPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    presentOption.Disable();
                    argumentOption.Disable();
                }

                if (presentOption.isCurrentlyHoveredCorrectly)
                {
                    objectPosToFollow = presentOption.rectTransform.position;
                }
                if (argumentOption.isCurrentlyHoveredCorrectly)
                {
                    objectPosToFollow = argumentOption.rectTransform.position;
                }
            }
            else
            {
                characterInteractionPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                presentOption.Disable();
                argumentOption.Disable();
            }

            draggedStallObject.rectTransform.position = objectPosToFollow;

            if (Input.GetMouseButtonUp(0))
            {
                if (presentOption.isCurrentlyHoveredCorrectly)
                {
                    StartCoroutine(presentOption.Select());
                    PresentObject(draggedStallObject, charaHovered);
                }
                else if (argumentOption.isCurrentlyHoveredCorrectly)
                {
                    argumentRadialMenu.Initialize(draggedStallObject, charaHovered);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                NegoceManager.I.exchangeHandler.DropStallObject(draggedStallObject);
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
            argumentOption.Disable();
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
                    atLeastOneClicked = true;
                }
            }

            if (atLeastOneClicked)
            {
                if(previousSelectedObject != selectedStallObject)
                {
                    objectInfoPanel.canvasGroup.interactable = true;
                    objectInfoPanel.canvasGroup.blocksRaycasts = true;
                    StartCoroutine(objectInfoPanel.anim.PlayBackward(objectInfoPanel, true));
                    UpdateObjectInfoWindow(selectedStallObject);
                }
            }
            else if(selectedStallObject != null)
            {
                /*
                selectedStallObject = null;
                previousSelectedObject = null;
                objectInfoPanel.canvasGroup.interactable = false;
                objectInfoPanel.canvasGroup.blocksRaycasts = false;
                StartCoroutine(objectInfoPanel.anim.Play(objectInfoPanel));
                */
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
    }

    private void InitPlayerInfo()
    {
        playerInventory.belongings = new List<PlayerInventory.Belonging>();
        for (int i = 0; i < initialPlayerBelongings.Count; i++)
        {
            playerInventory.belongings.Add(new PlayerInventory.Belonging(initialPlayerBelongings[i]));
            for (int y = 0; y < playerInventory.belongings[playerInventory.belongings.Count - 1].featureKnowledge.Length; y++)
            {
                playerInventory.belongings[playerInventory.belongings.Count - 1].featureKnowledge[y] = true;
            }
        }
    }

    private void UpdateObjectInfoWindow(StallObject shownObject)
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
            if(i + 2 < shownObject.linkedObject.features.Count)
            {
                featuresInfo[i].gameObject.SetActive(true);
                featuresInfo[i].GetChild(0).GetChild(0).GetComponent<Text>().text = shownObject.linkedObject.features[i + 2].argumentTitle;
                featuresInfo[i].GetChild(1).GetComponent<Text>().text = shownObject.linkedObject.features[i + 2].description;
            }
            else
            {
                featuresInfo[i].gameObject.SetActive(false);
            }
        }
        featureContentRect.sizeDelta = new Vector2(0, (shownObject.linkedObject.features.Count - 2) * 130 + 10);
    }

    private int halfObjectNumber;
    private void InitStallLayout()
    {
        allStallObjects = new List<StallObject>();
        StallObject newStallObject;
        halfObjectNumber = playerInventory.belongings.Count / 2;
        foreach(StallSpace stallSpace in allStallSpaces)
        {
            stallSpace.Init();
        }
        for (int o = 0; o < playerInventory.belongings.Count; o++)
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
            newStallObject.linkedObject = playerInventory.belongings[o].ownedObject;
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
}
