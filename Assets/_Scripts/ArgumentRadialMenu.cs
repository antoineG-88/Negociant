using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArgumentRadialMenu : UIInteractable
{
    public List<CategoryRadialOption> categoryRadialOptions;
    public List<RadialOption> featureRadialOptions;
    public Image objectIllustration;
    public Image characterFace;
    public Text objectNameText;
    public Text argumentDescriptionText;
    public TweeningAnimator appearAnim;

    private bool isOpened;
    private StallObject currentStallObject;
    private CharacterBehavior characterTargeted;
    private void Start()
    {
        appearAnim.GetReferences();
        appearAnim.anim = Instantiate(appearAnim.anim);
        appearAnim.anim.SetAtEndState(appearAnim);
    }

    private void Update()
    {
        if(isOpened)
        {
            for (int i = 0; i < categoryRadialOptions.Count; i++)
            {
                if(categoryRadialOptions[i].radialOption.isHovered)
                {
                    argumentDescriptionText.text = GameData.GetCategoryPropertiesFromCategory(categoryRadialOptions[i].category).argumentDescription;
                }

                if(categoryRadialOptions[i].radialOption.isClicked && !NegoceManager.I.playerHandler.IsPlayerTalking())
                {
                    NegoceManager.I.playerHandler.ArgumentCategory(currentStallObject, characterTargeted, GameData.GetCategoryPropertiesFromCategory(categoryRadialOptions[i].category));
                    Close();
                }
            }
        }

        if(isOpened && !isHovered && Input.GetMouseButtonDown(0))
        {
            Close();
        }
    }

    public void Initialize(StallObject stallObject, CharacterBehavior characterBehavior)
    {
        StartCoroutine(appearAnim.anim.PlayBackward(appearAnim, true));
        categoryRadialOptions[0].radialOption.icon.sprite = GameData.GetCategoryPropertiesFromCategory(stallObject.linkedObject.categories[0]).icon;
        categoryRadialOptions[1].radialOption.icon.sprite = GameData.GetCategoryPropertiesFromCategory(stallObject.linkedObject.categories[1]).icon;

        categoryRadialOptions[0].radialOption.icon.color = GameData.GetCategoryPropertiesFromCategory(stallObject.linkedObject.categories[0]).color;
        categoryRadialOptions[1].radialOption.icon.color = GameData.GetCategoryPropertiesFromCategory(stallObject.linkedObject.categories[1]).color;

        categoryRadialOptions[0].category = stallObject.linkedObject.categories[0];
        categoryRadialOptions[1].category = stallObject.linkedObject.categories[1];

        categoryRadialOptions[0].radialOption.time.text = GameData.GetCategoryPropertiesFromCategory(stallObject.linkedObject.categories[0]).argumentTime.ToString() + " s.";
        categoryRadialOptions[1].radialOption.time.text = GameData.GetCategoryPropertiesFromCategory(stallObject.linkedObject.categories[1]).argumentTime.ToString() + " s.";

        currentStallObject = stallObject;
        objectIllustration.sprite = stallObject.linkedObject.illustration;
        characterFace.sprite = characterBehavior.character.faceSprite;
        objectNameText.text = stallObject.linkedObject.objectName;
        characterTargeted = characterBehavior;
        isOpened = true;
    }

    public void Close()
    {
        StartCoroutine(appearAnim.anim.Play(appearAnim));
        isOpened = false;
    }

    public override void OnHoverIn()
    {

    }

    public override void OnHoverOut()
    {

    }

    [System.Serializable]
    public class CategoryRadialOption
    {
        public RadialOption radialOption;
        [HideInInspector] public Category category;
    }
}
