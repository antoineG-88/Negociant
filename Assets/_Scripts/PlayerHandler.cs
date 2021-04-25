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
    public float presentTime;
    public float presentShortTime;
    public Image actionBarFiller;
    public Text currentActionText;
    public Image objectOfTheActionDisplay;
    public Text targetedCharacterNameText;
    public Image targetedCharacterFaceDisplay;
    public GameObject actionBar;
    [Header("Reference")]
    [HideInInspector] public PlayerInventory playerInventory;
    public Text objectInfoNameText;
    public Text objectInfoTitleText;
    public Text objectInfoCategoryText;
    public Text objectInfoDescriptionText;
    public Text objectInfoOriginText;
    public Image objectInfoIllustration;
    public StallObject stallObjectPrefab;
    [Space]
    public RectTransform playerActionPanel;
    public TweeningAnimator playerActionPanelAnim;
    public RadialOption presentButton;
    public RadialOption argumentButton;

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
    private float timeSpendOnCurrentAction;
    private float currentPresentTime;
    private CharacterBehavior currentCharacterTalkingTo;

    private void Start()
    {
        playerActionPanelAnim.GetReferences();
        playerActionPanelAnim.canvasGroup.interactable = false;
        playerActionPanelAnim.canvasGroup.blocksRaycasts = false;
        playerActionPanelAnim.anim.SetAtEndState(playerActionPanelAnim);
        foreach(StallSpace stallSpace in allStallSpaces)
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
        UpdatePresentAction();

        if(presentedStallObject == null)
        {
            actionBar.SetActive(false);
        }
        else
        {
            actionBar.SetActive(true);
        }
    }

    public void UpdatePresentAction()
    {
        if (presentButton.isClicked && presentedStallObject == null)
        {
            PresentObject();
        }

        if (presentedStallObject != null)
        {
            actionBarFiller.fillAmount = timeSpendOnCurrentAction / currentPresentTime;


            if(timeSpendOnCurrentAction < currentPresentTime)
            {
                timeSpendOnCurrentAction += Time.deltaTime;
            }
            else
            {
                currentCharacterTalkingTo.PresentObject(presentedStallObject);
                currentCharacterTalkingTo.isTalking = false;
                presentedStallObject = null;
            }
        }
        else
        {
            currentActionText.text = "";
            objectOfTheActionDisplay.transform.parent.gameObject.SetActive(false);
            targetedCharacterFaceDisplay.transform.parent.gameObject.SetActive(false);
            targetedCharacterNameText.text = "";
            actionBarFiller.fillAmount = 0;
        }
    }

    public void PresentObject()
    {
        if (NegoceManager.I.characterSelected != null)
        {
            currentCharacterTalkingTo = NegoceManager.I.characterSelected;
            currentCharacterTalkingTo.isTalking = true;
            timeSpendOnCurrentAction = 0;
            presentedStallObject = selectedStallObject;
            currentActionText.text = "Présente";
            objectOfTheActionDisplay.transform.parent.gameObject.SetActive(true);
            targetedCharacterFaceDisplay.transform.parent.gameObject.SetActive(true);
            objectOfTheActionDisplay.sprite = selectedStallObject.linkedObject.illustration;
            targetedCharacterFaceDisplay.sprite = currentCharacterTalkingTo.character.faceSprite;
            targetedCharacterNameText.text = currentCharacterTalkingTo.character.characterName;
            if (currentCharacterTalkingTo.IsLookingAt(presentedStallObject))
            {
                currentPresentTime = presentShortTime;
            }
            else
            {
                currentPresentTime = presentTime;
            }
        }
        else
        {
            Debug.Log("Aucun personnage sélectionné");
        }
    }

    private void UpdateObjectInteraction()
    {
        HoverStallObjectUpdate();

        SelectionStallObjectUpdate();

        DragAndDropStallObjectUpdate();
    }

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
            draggedStallObject.rectTransform.position = Input.mousePosition;


            if (Input.GetMouseButtonUp(0))
            {
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
                    playerActionPanelAnim.canvasGroup.interactable = true;
                    playerActionPanelAnim.canvasGroup.blocksRaycasts = true;
                    StartCoroutine(playerActionPanelAnim.anim.PlayBackward(playerActionPanelAnim, true));
                    UpdateObjectInfoWindow(selectedStallObject);
                }
            }
            else if(selectedStallObject != null && !presentButton.isHovered && !argumentButton.isHovered)
            {
                selectedStallObject = null;
                previousSelectedObject = null;
                playerActionPanelAnim.canvasGroup.interactable = false;
                playerActionPanelAnim.canvasGroup.blocksRaycasts = false;
                StartCoroutine(playerActionPanelAnim.anim.Play(playerActionPanelAnim));
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
