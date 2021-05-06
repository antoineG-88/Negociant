using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    public bool useDropToAct;
    public string presentSpokenText;
    public float presentTime;
    //public Image actionBarFiller;
    public Text currentSpokenText;
    //public Image objectOfTheActionDisplay;
    //public Text targetedCharacterNameText;
    //public Image targetedCharacterFaceDisplay;
    public RectTransform speakBar;
    [Header("Reference")]
    public ArgumentRadialMenu argumentRadialMenu;   
    [HideInInspector] public PlayerInventory playerInventory;
    public Text objectInfoNameText;
    public Text objectInfoTitleText;
    public Text objectInfoCategoryText;
    public Text objectInfoDescriptionText;
    public Text objectInfoOriginText;
    public Image objectInfoIllustration;
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
    private float timeSpendSpeaking;
    private float currentSpeakingTime;
    private CharacterHandler currentCharacterTalkingTo;
    private GameData.CategoryProperties argumentedCategory; // temporary

    private void Start()
    {
        objectInfoPanel.GetReferences();
        objectInfoPanel.canvasGroup.interactable = false;
        objectInfoPanel.canvasGroup.blocksRaycasts = false;
        objectInfoPanel.anim.SetAtEndState(objectInfoPanel);
        presentOption.Disable();
        argumentOption.Disable();
        characterInteractionPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
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
        UpdatePlayerAction();
        UpdateObjectInteraction();
    }

    private void UpdatePlayerAction()
    {
        UpdateCurrentAction();

        if(presentedStallObject == null && argumentedStallObject == null)
        {
            speakBar.gameObject.SetActive(false);
        }
        else
        {
            speakBar.gameObject.SetActive(true);
            speakBar.position = new Vector3(currentCharacterTalkingTo.rectTransform.position.x, speakBar.position.y, 0);
        }
    }

    public void UpdateCurrentAction()
    {
        if(useDropToAct)
        {
            if (presentedStallObject != null || argumentedStallObject != null)
            {
                //actionBarFiller.fillAmount = timeSpendOnCurrentAction / currentActionTime;
                currentCharacterTalkingTo.RefreshEnthousiasm();

                if (timeSpendSpeaking < currentSpeakingTime)
                {
                    timeSpendSpeaking += Time.deltaTime;
                }
                else
                {
                    if (presentedStallObject != null)
                    {
                        currentCharacterTalkingTo.PresentObject(presentedStallObject);
                        currentCharacterTalkingTo.isTalking = false;
                        presentedStallObject = null;
                    }
                    else if (argumentedStallObject != null)
                    {
                        currentCharacterTalkingTo.ArgumentCategoryOnObject(argumentedCategory, argumentedStallObject);
                        currentCharacterTalkingTo.isTalking = false;
                        currentCharacterTalkingTo = null;
                        argumentedStallObject = null;
                        argumentedCategory = null;
                    }
                }
            }
            else
            {
                currentSpokenText.text = "";
                /*objectOfTheActionDisplay.transform.parent.gameObject.SetActive(false);
                targetedCharacterFaceDisplay.transform.parent.gameObject.SetActive(false);
                targetedCharacterNameText.text = "";
                actionBarFiller.fillAmount = 0;*/
            }
        }/*
        else
        {
            if ((presentedStallObject != null || argumentedStallObject != null) && Input.GetButton("Talk"))
            {
                //actionBarFiller.fillAmount = timeSpendOnCurrentAction / currentActionTime;
                currentCharacterTalkingTo.RefreshEnthousiasm();

                if (timeSpendOnCurrentAction < currentSpeakingTime)
                {
                    timeSpendOnCurrentAction += Time.deltaTime;
                }
                else
                {
                    if (presentedStallObject != null)
                    {
                        currentCharacterTalkingTo.PresentObject(presentedStallObject);
                        if(Input.GetButton("Talk"))
                        {
                            PresentObject(presentedStallObject, currentCharacterTalkingTo);
                        }
                        else
                        {
                            currentCharacterTalkingTo.isTalking = false;
                            currentCharacterTalkingTo = null;
                            presentedStallObject = null;
                        }
                    }
                    else if (argumentedStallObject != null)
                    {
                        currentCharacterTalkingTo.ArgumentCategoryOnObject(argumentedCategory, argumentedStallObject);
                        if (Input.GetButton("Talk"))
                        {
                            ArgumentCategory(argumentedStallObject, currentCharacterTalkingTo, argumentedCategory);
                        }
                        else
                        {
                            currentCharacterTalkingTo.isTalking = false;
                            currentCharacterTalkingTo = null;
                            argumentedStallObject = null;
                            argumentedCategory = null;
                        }
                    }
                }
            }
            else
            {
                currentSpokenText.text = "";
                objectOfTheActionDisplay.transform.parent.gameObject.SetActive(false);
                targetedCharacterFaceDisplay.transform.parent.gameObject.SetActive(false);
                targetedCharacterNameText.text = "";
                //actionBarFiller.fillAmount = 0;
                if(currentCharacterTalkingTo != null)
                {
                    currentCharacterTalkingTo.isTalking = false;
                    currentCharacterTalkingTo = null;
                }
                presentedStallObject = null;
                argumentedStallObject = null;
                argumentedCategory = null;
            }
        }*/
    }

    public bool IsPlayerTalking()
    {
        return presentedStallObject != null || argumentedStallObject != null;
    }

    public void PresentObject(StallObject stallObjectToPresent, CharacterHandler targetCharacter)
    {
        currentCharacterTalkingTo = targetCharacter;
        currentCharacterTalkingTo.isTalking = true;
        timeSpendSpeaking = 0;
        presentedStallObject = stallObjectToPresent;
        currentSpokenText.text = presentSpokenText + stallObjectToPresent.linkedObject.objectName + ", " + targetCharacter.character.characterName + " ?";
        //objectOfTheActionDisplay.transform.parent.gameObject.SetActive(true);
        //targetedCharacterFaceDisplay.transform.parent.gameObject.SetActive(true);
        //objectOfTheActionDisplay.sprite = presentedStallObject.linkedObject.illustration;
        //targetedCharacterFaceDisplay.sprite = currentCharacterTalkingTo.character.faceSprite;
        //targetedCharacterNameText.text = currentCharacterTalkingTo.character.characterName;
        currentSpeakingTime = presentTime;
    }

    public void ArgumentCategory(StallObject stallObjectArgumented, CharacterHandler targetCharacter, GameData.CategoryProperties categoryProperties)
    {
        currentCharacterTalkingTo = targetCharacter;
        currentCharacterTalkingTo.isTalking = true;
        timeSpendSpeaking = 0;
        argumentedStallObject = stallObjectArgumented;
        currentSpokenText.text = "Argumente";
        /*objectOfTheActionDisplay.transform.parent.gameObject.SetActive(true);
        targetedCharacterFaceDisplay.transform.parent.gameObject.SetActive(true);
        objectOfTheActionDisplay.sprite = argumentedStallObject.linkedObject.illustration;
        targetedCharacterFaceDisplay.sprite = currentCharacterTalkingTo.character.faceSprite;
        targetedCharacterNameText.text = currentCharacterTalkingTo.character.characterName;*/
        currentSpeakingTime = categoryProperties.argumentTime;
        argumentedCategory = categoryProperties;
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

            if (charaHovered != null)
            {
                characterInteractionPanel.position = charaHovered.rectTransform.position;

                /*presentOption.Enable(presentTime.ToString() + " s.");
                argumentOption.Enable("");
                characterInteractionPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;*/

                if (presentedStallObject == null)
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

            if (useDropToAct)
            {
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
            }
            else
            {
                if (Input.GetButtonDown("Talk"))
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
            }

            if (Input.GetMouseButtonUp(0) || (!useDropToAct && Input.GetButtonDown("Talk")))
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
    /*wouhouououououou
    private void DragAndDropCharaObjectUpdate()
    {
        if(NegoceManager.I.selectedCharacter != null)
        {
            for (int i = 0; i < NegoceManager.I.selectedCharacter.belongings.Count; i++)
            {
                if (draggedCharaObject == null && NegoceManager.I.selectedCharacter.belongings[i].isDragged)
                {
                    draggedCharaObject = NegoceManager.I.selectedCharacter.belongings[i];
                }
            }

            if (draggedCharaObject != null)
            {
                foreach (CharaObject charaObject in NegoceManager.I.selectedCharacter.belongings)
                {
                    charaObject.canvasGroup.blocksRaycasts = false;
                }

                Vector3 objectPosToFollow = Input.mousePosition;

                draggedCharaObject.rectTransform.position = objectPosToFollow;

                if (Input.GetMouseButtonUp(0))
                {
                    NegoceManager.I.exchangeHandler.DropCharaObject(draggedCharaObject);
                    draggedCharaObject.StopDrag();
                    draggedCharaObject.rectTransform.position = draggedCharaObject.charaBelongingSpace.position;

                    draggedCharaObject.isDragged = false;
                    draggedCharaObject = null;
                }
            }
            else
            {
                foreach (CharaObject charaObject in NegoceManager.I.selectedCharacter.belongings)
                {
                    charaObject.canvasGroup.blocksRaycasts = true;
                }
            }
        }
    } 
    */
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
                selectedStallObject = null;
                previousSelectedObject = null;
                objectInfoPanel.canvasGroup.interactable = false;
                objectInfoPanel.canvasGroup.blocksRaycasts = false;
                StartCoroutine(objectInfoPanel.anim.Play(objectInfoPanel));
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
        objectInfoCategoryText.text = shownObject.linkedObject.categories[0].ToString() + " / " + (shownObject.linkedObject.categories.Count > 1 ? shownObject.linkedObject.categories[1].ToString() : "");
        objectInfoDescriptionText.text = shownObject.linkedObject.description;
        objectInfoOriginText.text = shownObject.linkedObject.originDescription;
        objectInfoIllustration.sprite = shownObject.linkedObject.illustration;
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
