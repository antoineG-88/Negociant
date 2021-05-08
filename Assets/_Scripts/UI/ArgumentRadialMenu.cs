using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArgumentRadialMenu : UIInteractable
{
    public List<FeatureRadialOption> categoryRadialOptions;
    public List<FeatureRadialOption> featureRadialOptions;
    public Image objectIllustration;
    public Image characterFace;
    public Text objectNameText;
    public Text argumentDescriptionText;
    public TweeningAnimator appearAnim;

    private bool isOpened;
    private StallObject currentStallObject;
    private CharacterHandler characterTargeted;
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
                    argumentDescriptionText.text = categoryRadialOptions[i].feature.categoryProperties.argumentDescription;
                }

                if(categoryRadialOptions[i].radialOption.isClicked && !NegoceManager.I.playerHandler.IsPlayerTalking())
                {
                    NegoceManager.I.playerHandler.ArgumentFeature(currentStallObject, characterTargeted, categoryRadialOptions[i].feature);
                    Close();
                }
            }

            
            for (int i = 0; i < featureRadialOptions.Count; i++)
            {
                if (featureRadialOptions[i].radialOption.isHovered)
                {
                    argumentDescriptionText.text = featureRadialOptions[i].feature.description;
                }

                if (featureRadialOptions[i].radialOption.isClicked && !NegoceManager.I.playerHandler.IsPlayerTalking())
                {
                    NegoceManager.I.playerHandler.ArgumentFeature(currentStallObject, characterTargeted, featureRadialOptions[i].feature);
                    Close();
                }
            }
        }

        if(isOpened && !isHovered && Input.GetMouseButtonDown(0))
        {
            Close();
        }
    }

    public void Initialize(StallObject stallObject, CharacterHandler characterHandler)
    {
        StartCoroutine(appearAnim.anim.PlayBackward(appearAnim, true));
        categoryRadialOptions[0].radialOption.icon.sprite = stallObject.linkedObject.features[0].categoryProperties.icon;
        categoryRadialOptions[0].radialOption.icon.color = stallObject.linkedObject.features[0].categoryProperties.color;
        categoryRadialOptions[0].feature = stallObject.linkedObject.features[0];
        categoryRadialOptions[0].radialOption.time.text = stallObject.linkedObject.features[0].categoryProperties.argumentTime.ToString() + " s.";

        if(stallObject.linkedObject.categories.Count > 1)
        {
            categoryRadialOptions[1].radialOption.gameObject.SetActive(true);
            categoryRadialOptions[1].radialOption.icon.sprite = stallObject.linkedObject.features[1].categoryProperties.icon;
            categoryRadialOptions[1].radialOption.icon.color = stallObject.linkedObject.features[1].categoryProperties.color;
            categoryRadialOptions[1].feature = stallObject.linkedObject.features[1];
            categoryRadialOptions[1].radialOption.time.text = stallObject.linkedObject.features[1].categoryProperties.argumentTime.ToString() + " s.";
        }
        else
        {
            categoryRadialOptions[1].radialOption.icon.color = Color.clear;
            categoryRadialOptions[1].radialOption.gameObject.SetActive(false);
        }


        for (int i = 0; i < featureRadialOptions.Count; i++)
        {
            if(i + 2 < stallObject.linkedObject.features.Count)
            {
                featureRadialOptions[i].radialOption.gameObject.SetActive(true);
                featureRadialOptions[i].radialOption.info.text = stallObject.linkedObject.features[i + 2].argumentTitle;
                featureRadialOptions[i].radialOption.time.text = stallObject.linkedObject.features[i + 2].argumentSpeakTime.ToString() + " s.";
                featureRadialOptions[i].feature = stallObject.linkedObject.features[i + 2];
            }
            else
            {
                featureRadialOptions[i].radialOption.gameObject.SetActive(false);
            }
        }

        currentStallObject = stallObject;
        objectIllustration.sprite = stallObject.linkedObject.illustration;
        characterFace.sprite = characterHandler.character.faceSprite;
        objectNameText.text = stallObject.linkedObject.objectName;
        characterTargeted = characterHandler;
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
    public class FeatureRadialOption
    {
        public RadialOption radialOption;
        [HideInInspector] public Object.Feature feature;
    }
}
