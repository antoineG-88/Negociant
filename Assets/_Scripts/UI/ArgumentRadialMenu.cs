using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArgumentRadialMenu : UIInteractable
{
    public List<FeatureRadialOption> categoryRadialOptions;
    public List<FeatureRadialOption> featureRadialOptions;
    public RadialOption removeObjectFromTradeOption;
    public Image objectIllustration;
    public Text objectNameText;
    public Text argumentDescriptionText;
    public TweeningAnimator appearAnim;

    public RectTransform rectTransform;
    private bool isOpened;
    private StallObject currentStallObject;
    private CharacterHandler.PotentialObject currentPotentialObject;
    private CharacterHandler characterTargeted;
    private ExchangeSpace currentExchangeSpace;
    private bool atLeastOneOptionChosen;
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
            rectTransform.position = currentExchangeSpace.rectTransform.position;
            if (Input.GetMouseButtonUp(0))
            {
                atLeastOneOptionChosen = false;
            }

            for (int i = 0; i < categoryRadialOptions.Count; i++)
            {
                if(categoryRadialOptions[i].radialOption.gameObject.activeSelf)
                {
                    categoryRadialOptions[i].knownFiller.fillAmount = categoryRadialOptions[i].knownFeature.timeRememberedRmn / categoryRadialOptions[i].feature.rememberTime;
                }

                if (categoryRadialOptions[i].radialOption.isHovered)
                {
                    argumentDescriptionText.text = categoryRadialOptions[i].feature.categoryProperties.argumentDescription;

                    if (Input.GetMouseButtonUp(0) && !NegoceManager.I.playerHandler.IsPlayerTalking() && !characterTargeted.isThinking)
                    {
                        NegoceManager.I.playerHandler.ArgumentFeature(currentStallObject, characterTargeted, categoryRadialOptions[i].feature);
                        atLeastOneOptionChosen = true;
                        Close();
                    }
                }
            }

            
            for (int i = 0; i < featureRadialOptions.Count; i++)
            {
                if (featureRadialOptions[i].radialOption.gameObject.activeSelf)
                {
                    featureRadialOptions[i].knownFiller.fillAmount = featureRadialOptions[i].knownFeature.timeRememberedRmn / featureRadialOptions[i].feature.rememberTime;
                }

                if (featureRadialOptions[i].radialOption.isHovered)
                {
                    argumentDescriptionText.text = featureRadialOptions[i].feature.description;

                    if(Input.GetMouseButtonUp(0) && !NegoceManager.I.playerHandler.IsPlayerTalking() && !characterTargeted.isThinking)
                    {
                        NegoceManager.I.playerHandler.ArgumentFeature(currentStallObject, characterTargeted, featureRadialOptions[i].feature);
                        atLeastOneOptionChosen = true;
                        Close();
                    }
                }
            }


            if(Input.GetMouseButtonUp(0))
            {
                if (removeObjectFromTradeOption.isHovered)
                {
                    characterTargeted.characterExchangeHandler.RemoveStallObjectFromTrade(currentExchangeSpace);
                    atLeastOneOptionChosen = true;
                    Close();
                }

                if (!atLeastOneOptionChosen)
                {
                    Close(); // delay maybe
                }
            }
        }
    }

    public void OpenRadialMenu(StallObject stallObject, CharacterHandler characterHandler, ExchangeSpace exchangeSpace)
    {
        currentExchangeSpace = exchangeSpace;
        rectTransform.position = exchangeSpace.rectTransform.position;
        appearAnim.GetReferences();
        StartCoroutine(appearAnim.anim.PlayBackward(appearAnim, true));


        currentStallObject = stallObject;
        objectIllustration.sprite = stallObject.linkedObject.illustration;
        objectNameText.text = stallObject.linkedObject.objectName;
        characterTargeted = characterHandler;
        currentPotentialObject = characterTargeted.GetPotentialFromStallObject(currentStallObject);
        isOpened = true;

        categoryRadialOptions[0].radialOption.icon.sprite = stallObject.linkedObject.features[0].categoryProperties.icon;
        categoryRadialOptions[0].radialOption.icon.color = stallObject.linkedObject.features[0].categoryProperties.color;
        categoryRadialOptions[0].feature = stallObject.linkedObject.features[0];
        categoryRadialOptions[0].knownFeature = currentPotentialObject.GetKnownFeatureFromFeature(categoryRadialOptions[0].feature);
        categoryRadialOptions[0].radialOption.time.text = stallObject.linkedObject.features[0].categoryProperties.argumentRememberTime.ToString() + " s.";

        if(stallObject.linkedObject.categories.Count > 1)
        {
            categoryRadialOptions[1].radialOption.gameObject.SetActive(true);
            categoryRadialOptions[1].radialOption.icon.sprite = stallObject.linkedObject.features[1].categoryProperties.icon;
            categoryRadialOptions[1].radialOption.icon.color = stallObject.linkedObject.features[1].categoryProperties.color;
            categoryRadialOptions[1].feature = stallObject.linkedObject.features[1];
            categoryRadialOptions[1].knownFeature = currentPotentialObject.GetKnownFeatureFromFeature(categoryRadialOptions[1].feature);
            categoryRadialOptions[1].radialOption.time.text = stallObject.linkedObject.features[1].categoryProperties.argumentRememberTime.ToString() + " s.";
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
                featureRadialOptions[i].radialOption.time.text = stallObject.linkedObject.features[i + 2].rememberTime.ToString() + " s.";
                featureRadialOptions[i].feature = stallObject.linkedObject.features[i + 2];
                featureRadialOptions[i].knownFeature = currentPotentialObject.GetKnownFeatureFromFeature(featureRadialOptions[i].feature);
            }
            else
            {
                featureRadialOptions[i].radialOption.gameObject.SetActive(false);
            }
        }

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
        public Image knownFiller;
        public CharacterHandler.PotentialObject.KnownFeature knownFeature;
    }
}
