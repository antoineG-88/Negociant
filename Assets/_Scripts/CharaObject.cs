using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaObject : UIInteractable
{
    [HideInInspector] public Object linkedObject;
    public Image illustration;
    public RectTransform rectTransform;
    public TweeningAnimator hoverAnim;
    public TweeningAnimator categoriesAppearAnim;
    public Image categoryDisplay1;
    public Image categoryDisplay2;
    public Image personnalValueFiller; 
    [HideInInspector] public float personnalValue;
    [HideInInspector] public bool isPersonnalValueKnown;

    private void Start()
    {
        hoverAnim.anim = Instantiate(hoverAnim.anim);
        hoverAnim.GetReferences();
        categoriesAppearAnim.anim = Instantiate(categoriesAppearAnim.anim);
        categoriesAppearAnim.GetReferences();
        personnalValueFiller.fillAmount = personnalValue / 50;
        isPersonnalValueKnown = true; // remove 
        categoriesAppearAnim.anim.SetAtStartState(categoriesAppearAnim);
    }

    private void Update()
    {
        personnalValueFiller.gameObject.SetActive(isPersonnalValueKnown);
    }

    public void RefreshDisplay()
    {
        illustration.sprite = linkedObject.illustration;
        categoryDisplay1.color = GameData.categoriesColor[(int)linkedObject.categories[0]];
        categoryDisplay1.sprite = GameData.categoriesIcon[(int)linkedObject.categories[0]];
        categoryDisplay2.color = GameData.categoriesColor[(int)linkedObject.categories[1]];
        categoryDisplay2.sprite = GameData.categoriesIcon[(int)linkedObject.categories[1]];

    }

    public override void OnHoverIn()
    {
        StartCoroutine(hoverAnim.anim.Play(hoverAnim));
        StartCoroutine(categoriesAppearAnim.anim.Play(categoriesAppearAnim));
    }

    public override void OnHoverOut()
    {
        StartCoroutine(hoverAnim.anim.PlayBackward(hoverAnim, true));
        StartCoroutine(categoriesAppearAnim.anim.PlayBackward(categoriesAppearAnim, true));
    }
}
