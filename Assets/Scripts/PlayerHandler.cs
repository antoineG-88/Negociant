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

    [HideInInspector] public List<StandObject> allStandObjects;
    private bool atleastOneHovered;
    private bool atleastOneClicked;
    private StandObject standObjectSelected;
    private StandObject previousSelectedObject;
    private StandObject hoveredStandObject;

    private void Start()
    {
        objectInfoWindowAnimator.GetReferences();
        objectInfoWindowAnimator.anim.SetAtStartState(objectInfoWindowAnimator);
        playerActionPanel.gameObject.SetActive(false);
        InitPlayerInfo();
        InitStandLayout();
    }

    private void Update()
    {
        UpdateObjectSelection();
    }

    private void UpdateObjectSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            atleastOneHovered = false;
            for (int i = 0; i < allStandObjects.Count; i++)
            {
                if (allStandObjects[i].isHovered)
                {
                    if (standObjectSelected != null)
                    {
                        previousSelectedObject = standObjectSelected;
                    }
                    standObjectSelected = allStandObjects[i];
                    objectInfoNameText.text = standObjectSelected.linkedObject.objectName;
                    objectInfoTitleText.text = standObjectSelected.linkedObject.title;
                    objectInfoCategoryText.text = standObjectSelected.linkedObject.categories[0].ToString() + " / " + (standObjectSelected.linkedObject.categories.Count > 1 ? standObjectSelected.linkedObject.categories[1].ToString() : "");
                    objectInfoDescriptionText.text = standObjectSelected.linkedObject.description;
                    objectInfoOriginText.text = standObjectSelected.linkedObject.originDescription;
                    objectInfoIllustration.sprite = standObjectSelected.linkedObject.illustration;
                    atleastOneHovered = true;
                }
            }

            if (atleastOneHovered && previousSelectedObject != standObjectSelected)
            {
                StartCoroutine(objectInfoWindowAnimator.anim.Play(objectInfoWindowAnimator, objectInfoWindowAnimator.originalPos));
                playerActionPanel.gameObject.SetActive(true);
                playerActionPanel.position = standObjectSelected.rectTransform.position;
            }
            if (!atleastOneHovered && standObjectSelected != null)
            {

            }
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            standObjectSelected = null;
            previousSelectedObject = null;
            StartCoroutine(objectInfoWindowAnimator.anim.PlayBackward(objectInfoWindowAnimator, objectInfoWindowAnimator.originalPos, true));
            playerActionPanel.gameObject.SetActive(false);
        }

        /*atleastOneHovered = false;
        for (int i = 0; i < allStandObjects.Count; i++)
        {
            if (allStandObjects[i].isHovered)
            {
                hoveredStandObject = allStandObjects[i];
                atleastOneHovered = true;
            }
        }

        if(atleastOneHovered)
        {
            playerActionPanel.gameObject.SetActive(true);
        }
        else
        {
            playerActionPanel.gameObject.SetActive(false);
        }*/
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
            NegoceManager.I.characterSelected.PresentObject(standObjectSelected);
        }
        else
        {
            Debug.Log("Aucun personnage sélectionné");
        }
    }
}
