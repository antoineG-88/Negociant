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

    private List<StandObject> allStandObjects;
    private bool atleastOneHovered;

    private void Start()
    {
        InitPlayerInfo();
        InitStandLayout();
    }

    private void Update()
    {
        atleastOneHovered = false;
        for (int i = 0; i < allStandObjects.Count; i++)
        {
            if(allStandObjects[i].isHovered)
            {
                objectInfoNameText.text = allStandObjects[i].linkedObject.objectName;
                objectInfoTitleText.text = allStandObjects[i].linkedObject.title;
                objectInfoCategoryText.text = allStandObjects[i].linkedObject.category.ToString();
                objectInfoDescriptionText.text = allStandObjects[i].linkedObject.description;
                objectInfoOriginText.text = allStandObjects[i].linkedObject.originDescription;
                objectInfoIllustration.sprite = allStandObjects[i].linkedObject.illustration;
                atleastOneHovered = true;
            }
        }

        if(atleastOneHovered)
        {
            objectInfoWindow.SetActive(true);
        }
        else
        {
            objectInfoWindow.SetActive(false);
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
            newStandObject.RefreshDisplay();
            allStandObjects.Add(newStandObject);
        }
    }
}
