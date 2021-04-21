using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandler : MonoBehaviour
{
    [Header("Temporary")]
    public List<Object> initialPlayerBelongings;
    [Header("Stand Display")]
    public GameObject objectInfoWindow;
    public TweeningAnimator objectInfoWindowAnimator;
    public RectTransform standRectTransform; 
    public float distanceBetweenStandObject;
    public Vector2 standObjectsOriginPos;
    [Header("Reference")]
    [HideInInspector] public PlayerInventory playerInventory;
    public Text objectInfoNameText;
    public Text objectInfoTitleText;
    public Text objectInfoCategoryText;
    public Text objectInfoDescriptionText;
    public Text objectInfoOriginText;
    public Image objectInfoIllustration;
    public StandObject standObjectPrefab;

    [HideInInspector] public List<StandObject> allStandObjects;
    private bool atleastOneHovered;
    private bool atleastOneClicked;
    private StandObject standObjectSelected;
    private StandObject previousSelectedObject;
    private CharacterBehavior characterSelected;

    private void Start()
    {
        objectInfoWindowAnimator.GetReferences();
        StartCoroutine(objectInfoWindowAnimator.anim.PlayBackward(objectInfoWindowAnimator, true));
        InitPlayerInfo();
        InitStandLayout();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            atleastOneHovered = false;
            for (int i = 0; i < allStandObjects.Count; i++)
            {
                if (allStandObjects[i].isHovered)
                {
                    if(standObjectSelected != null)
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
                StartCoroutine(objectInfoWindowAnimator.anim.Play(objectInfoWindowAnimator));
            }
            if (!atleastOneHovered && standObjectSelected != null)
            {
                standObjectSelected = null;
                previousSelectedObject = null;
                StartCoroutine(objectInfoWindowAnimator.anim.PlayBackward(objectInfoWindowAnimator, true));
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

    private void InitStandLayout()
    {
        allStandObjects = new List<StandObject>();
        StandObject newStandObject;
        for (int o = 0; o < playerInventory.belongings.Count; o++)
        {
            newStandObject = Instantiate(standObjectPrefab, standRectTransform);
            newStandObject.GetComponent<RectTransform>().anchoredPosition = standObjectsOriginPos + new Vector2(-((distanceBetweenStandObject * (playerInventory.belongings.Count - 1))*0.5f) + distanceBetweenStandObject * o, 0);
            newStandObject.linkedObject = playerInventory.belongings[o].ownedObject;
            newStandObject.rectTransform = newStandObject.GetComponent<RectTransform>();
            newStandObject.name = newStandObject.linkedObject.objectName;
            newStandObject.RefreshDisplay();
            allStandObjects.Add(newStandObject);
        }
    }

    public void SelectCharacter(CharacterBehavior theCharacter)
    {
        characterSelected = theCharacter;
        characterSelected.Select();
        foreach (CharacterBehavior character in NegoceManager.I.allPresentCharacters)
        {
            if(character != characterSelected)
            {
                character.UnSelect();
            }
        }
    }
}
