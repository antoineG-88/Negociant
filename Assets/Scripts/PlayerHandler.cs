using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerHandler : MonoBehaviour
{
    [Header("Temporary")]
    public List<Object> initialPlayerBelongings;
    [Header("Stand Display")]
    public GameObject objectInfoWindow;
    public TweeningAnimator objectInfoWindowAnimator;
    public RectTransform stallRectTransform; 
    public float distanceBetweenStandObject;
    public List<StallSpace> allStallSpaces;
    public int vitrineStallSpacesAvailable;
    public int backStallSpacesAvailable;
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

    [HideInInspector] public List<StallObject> allStallObjects;
    private bool atleastOneHovered;
    private bool atLeastOnePressed;
    private StallObject selectedStallObject;
    private StallObject previousSelectedObject;
    private StallObject hoveredStallObject;
    private StallObject previousHoveredObject;
    [HideInInspector] public StallObject draggedStallObject;
    private Camera cam;
    private int vitrineSpaceNumber;

    private void Start()
    {
        cam = Camera.main;
        objectInfoWindowAnimator.GetReferences();
        objectInfoWindowAnimator.anim.SetAtStartState(objectInfoWindowAnimator);
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
        UpdateObjectSelection();
    }

    private void UpdateObjectSelection()
    {
        atleastOneHovered = false;
        atLeastOnePressed = false;
        for (int i = 0; i < allStallObjects.Count; i++)
        {
            /*
            if(selectedStallObject == null && allStallObjects[i].isPressed)
            {
                if (selectedStallObject != null)
                {
                    previousSelectedObject = selectedStallObject;
                }
                selectedStallObject = allStallObjects[i];
                atLeastOnePressed = true;
            }*/

            if (selectedStallObject == null)
            {
                if (allStallObjects[i].isHovered)
                {
                    SetHoveredObject(allStallObjects[i]);
                }
            }
            else
            {
                SetHoveredObject(selectedStallObject);
            }

            if (draggedStallObject == null && allStallObjects[i].isDragged)
            {
                draggedStallObject = allStallObjects[i];
            }
        }

        if (atleastOneHovered && previousHoveredObject != hoveredStallObject)
        {
            StartCoroutine(objectInfoWindowAnimator.anim.Play(objectInfoWindowAnimator, objectInfoWindowAnimator.originalPos));
        }
        if (!atleastOneHovered && hoveredStallObject != null)
        {
            hoveredStallObject = null;
            previousHoveredObject = null;
            StartCoroutine(objectInfoWindowAnimator.anim.PlayBackward(objectInfoWindowAnimator, objectInfoWindowAnimator.originalPos, true));
        }

        if (atLeastOnePressed && previousSelectedObject != selectedStallObject)
        {
            /*playerActionPanel.position = hoveredStallObject.rectTransform.position;
            playerActionPanelAnim.GetReferences();
            playerActionPanelAnim.canvasGroup.interactable = true;
            playerActionPanelAnim.canvasGroup.blocksRaycasts = true;
            StartCoroutine(playerActionPanelAnim.anim.PlayBackward(playerActionPanelAnim, playerActionPanelAnim.originalPos, true));*/
        }

        if (selectedStallObject != null)
        {
            if(selectedStallObject.isPressed == false)
            {
                /*selectedStallObject = null;
                previousSelectedObject = null;
                playerActionPanelAnim.canvasGroup.interactable = false;
                playerActionPanelAnim.canvasGroup.blocksRaycasts = false;
                StartCoroutine(playerActionPanelAnim.anim.Play(playerActionPanelAnim, playerActionPanelAnim.originalPos));*/
            }
        }

        if(draggedStallObject != null)
        {
            foreach(StallObject stallObject in allStallObjects)
            {
                stallObject.canvasGroup.blocksRaycasts = false;
            }
            draggedStallObject.rectTransform.position = Input.mousePosition;


            if(Input.GetMouseButtonUp(0))
            {
                bool droppedOnStallSpace = false;
                StallSpace draggedObjectStallSpace = draggedStallObject.stallSpace;
                foreach(StallSpace hoveredStallSpace in allStallSpaces)
                {
                    if(hoveredStallSpace.isHovered)
                    {
                        droppedOnStallSpace = true;
                        if(hoveredStallSpace.stallObject != null)
                        {
                            hoveredStallSpace.stallObject.stallSpace = draggedObjectStallSpace;
                            draggedObjectStallSpace.stallObject = hoveredStallSpace.stallObject;
                            draggedObjectStallSpace.stallObject.rectTransform.position = draggedObjectStallSpace.rectTransform.position;
                        }
                        else
                        {
                            draggedObjectStallSpace.stallObject = null;
                        }
                        draggedStallObject.rectTransform.position = hoveredStallSpace.rectTransform.position;
                        hoveredStallSpace.stallObject = draggedStallObject;
                        draggedStallObject.stallSpace = hoveredStallSpace;
                    }
                }

                if(!droppedOnStallSpace)
                {
                    draggedStallObject.rectTransform.position = draggedStallObject.stallSpace.rectTransform.position;
                }
                draggedObjectStallSpace = null;
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

    private void SetHoveredObject(StallObject newHoveredObject)
    {
        if (hoveredStallObject != null)
        {
            previousHoveredObject = hoveredStallObject;
        }
        hoveredStallObject = newHoveredObject;
        objectInfoNameText.text = hoveredStallObject.linkedObject.objectName;
        objectInfoTitleText.text = hoveredStallObject.linkedObject.title;
        objectInfoCategoryText.text = hoveredStallObject.linkedObject.categories[0].ToString() + " / " + (hoveredStallObject.linkedObject.categories.Count > 1 ? hoveredStallObject.linkedObject.categories[1].ToString() : "");
        objectInfoDescriptionText.text = hoveredStallObject.linkedObject.description;
        objectInfoOriginText.text = hoveredStallObject.linkedObject.originDescription;
        objectInfoIllustration.sprite = hoveredStallObject.linkedObject.illustration;
        atleastOneHovered = true;
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

    public void PresentObject()
    {
        if(NegoceManager.I.characterSelected != null)
        {
            NegoceManager.I.characterSelected.PresentObject(selectedStallObject);
        }
        else
        {
            Debug.Log("Aucun personnage sélectionné");
        }
    }
}
