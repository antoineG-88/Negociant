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
    public RectTransform standRectTransform; 
    public float distanceBetweenStandObject;
    public Vector2 standObjectsOriginPosLine1;
    public Vector2 standObjectsOriginPosLine2;
    [Header("Reference")]
    [HideInInspector] public PlayerInventory playerInventory;
    public Text objectInfoNameText;
    public Text objectInfoTitleText;
    public Text objectInfoCategoryText;
    public Text objectInfoDescriptionText;
    public Text objectInfoOriginText;
    public Image objectInfoIllustration;
    public StandObject standObjectPrefab;
    [Space]
    public RectTransform playerActionPanel;
    public TweeningAnimator playerActionPanelAnim;

    [HideInInspector] public List<StandObject> allStandObjects;
    private bool atleastOneHovered;
    private bool atLeastOnePressed;
    private StandObject selectedStandObject;
    private StandObject previousSelectedObject;
    private StandObject hoveredStandObject;
    private StandObject previousHoveredObject;

    private void Start()
    {
        objectInfoWindowAnimator.GetReferences();
        objectInfoWindowAnimator.anim.SetAtStartState(objectInfoWindowAnimator);
        playerActionPanelAnim.GetReferences();
        playerActionPanelAnim.canvasGroup.interactable = false;
        playerActionPanelAnim.canvasGroup.blocksRaycasts = false;
        playerActionPanelAnim.anim.SetAtEndState(playerActionPanelAnim);
        InitPlayerInfo();
        InitStandLayout();
    }

    private void Update()
    {
        UpdateObjectSelection();
    }

    private void UpdateObjectSelection()
    {
        atleastOneHovered = false;
        atLeastOnePressed = false;
        for (int i = 0; i < allStandObjects.Count; i++)
        {
            if(selectedStandObject == null && allStandObjects[i].isPressed)
            {
                if (selectedStandObject != null)
                {
                    previousSelectedObject = selectedStandObject;
                }
                selectedStandObject = allStandObjects[i];
                atLeastOnePressed = true;
            }

            if (selectedStandObject == null)
            {
                if (allStandObjects[i].isHovered)
                {
                    SetHoveredObject(allStandObjects[i]);
                }
            }
            else
            {
                SetHoveredObject(selectedStandObject);
            }
        }

        if (atleastOneHovered && previousHoveredObject != hoveredStandObject)
        {
            StartCoroutine(objectInfoWindowAnimator.anim.Play(objectInfoWindowAnimator, objectInfoWindowAnimator.originalPos));
        }
        if (!atleastOneHovered && hoveredStandObject != null)
        {
            hoveredStandObject = null;
            previousHoveredObject = null;
            StartCoroutine(objectInfoWindowAnimator.anim.PlayBackward(objectInfoWindowAnimator, objectInfoWindowAnimator.originalPos, true));
        }

        if (atLeastOnePressed && previousSelectedObject != selectedStandObject)
        {
            playerActionPanel.position = hoveredStandObject.rectTransform.position;
            playerActionPanelAnim.GetReferences();
            playerActionPanelAnim.canvasGroup.interactable = true;
            playerActionPanelAnim.canvasGroup.blocksRaycasts = true;
            StartCoroutine(playerActionPanelAnim.anim.PlayBackward(playerActionPanelAnim, playerActionPanelAnim.originalPos, true));
        }

        if (selectedStandObject != null)
        {
            if(selectedStandObject.isPressed == false)
            {
                selectedStandObject = null;
                previousSelectedObject = null;
                playerActionPanelAnim.canvasGroup.interactable = false;
                playerActionPanelAnim.canvasGroup.blocksRaycasts = false;
                StartCoroutine(playerActionPanelAnim.anim.Play(playerActionPanelAnim, playerActionPanelAnim.originalPos));
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

    private void SetHoveredObject(StandObject newHoveredObject)
    {
        if (hoveredStandObject != null)
        {
            previousHoveredObject = hoveredStandObject;
        }
        hoveredStandObject = newHoveredObject;
        objectInfoNameText.text = hoveredStandObject.linkedObject.objectName;
        objectInfoTitleText.text = hoveredStandObject.linkedObject.title;
        objectInfoCategoryText.text = hoveredStandObject.linkedObject.categories[0].ToString() + " / " + (hoveredStandObject.linkedObject.categories.Count > 1 ? hoveredStandObject.linkedObject.categories[1].ToString() : "");
        objectInfoDescriptionText.text = hoveredStandObject.linkedObject.description;
        objectInfoOriginText.text = hoveredStandObject.linkedObject.originDescription;
        objectInfoIllustration.sprite = hoveredStandObject.linkedObject.illustration;
        atleastOneHovered = true;
    }

    private int maxObjectOnLine;

    private void InitStandLayout()
    {
        allStandObjects = new List<StandObject>();
        StandObject newStandObject;
        maxObjectOnLine = playerInventory.belongings.Count / 2;
        for (int o = 0; o < playerInventory.belongings.Count; o++)
        {
            newStandObject = Instantiate(standObjectPrefab, standRectTransform);

            if(o < maxObjectOnLine)
            {
                newStandObject.GetComponent<RectTransform>().anchoredPosition = standObjectsOriginPosLine1 + new Vector2(o % 2 == 0 ? ((o+0.5f)/2 * distanceBetweenStandObject) : (-(o + 0.5f)/2 * distanceBetweenStandObject), 0);
                //newStandObject.GetComponent<RectTransform>().anchoredPosition = standObjectsOriginPosLine1 + new Vector2(-((distanceBetweenStandObject * (Mathf.Clamp(playerInventory.belongings.Count - 1, 0, maxObjectOnLine - 1))) * 0.5f) + distanceBetweenStandObject * o, 0);
            }
            else
            {
                int realo = o - maxObjectOnLine;
                newStandObject.GetComponent<RectTransform>().anchoredPosition = standObjectsOriginPosLine2 + new Vector2(realo % 2 == 0 ? ((realo + 0.5f) / 2 * distanceBetweenStandObject) : (-(realo + 0.5f) / 2 * distanceBetweenStandObject), 0);
                //newStandObject.GetComponent<RectTransform>().anchoredPosition = standObjectsOriginPosLine2 + new Vector2(-((distanceBetweenStandObject * (playerInventory.belongings.Count - (1 + maxObjectOnLine))) * 0.5f) + distanceBetweenStandObject * o, 0);
            }

            newStandObject.linkedObject = playerInventory.belongings[o].ownedObject;
            newStandObject.rectTransform = newStandObject.GetComponent<RectTransform>();
            newStandObject.name = newStandObject.linkedObject.objectName;
            newStandObject.RefreshDisplay();
            allStandObjects.Add(newStandObject);
        }
    }

    public void PresentObject()
    {
        if(NegoceManager.I.characterSelected != null)
        {
            NegoceManager.I.characterSelected.PresentObject(selectedStandObject);
        }
        else
        {
            Debug.Log("Aucun personnage sélectionné");
        }
    }
}
