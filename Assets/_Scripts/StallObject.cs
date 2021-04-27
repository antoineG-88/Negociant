using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StallObject : UIInteractable
{
    public Object linkedObject;
    public Text nameText;
    public Image illustration;
    public TweeningAnimator hoverAnim;
    public RectTransform rectTransform;
    public Text interestLevel;
    public StallSpace stallSpace;
    public CanvasGroup canvasGroup;
    public Image categoryDisplay1;
    public Image categoryDisplay2;
    public List<Sprite> categoriesIcon;
    public List<Color> categoriesColor;
    public Image interestLevelFiller;
    [HideInInspector] public float interestLevelToShow;
    public float interestLevelAnimLerpRatio;
    [HideInInspector] public bool canBeHovered;

    private void Start()
    {
        canBeHovered = true;
        canvasGroup = GetComponent<CanvasGroup>();
        hoverAnim.GetReferences();
        hoverAnim.anim = Instantiate(hoverAnim.anim);
    }

    private void Update()
    {
        if(interestLevelToShow < 0)
        {
            interestLevelFiller.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            interestLevelFiller.transform.parent.gameObject.SetActive(true);
            interestLevelFiller.fillAmount = Mathf.Lerp(interestLevelFiller.fillAmount, interestLevelToShow, interestLevelAnimLerpRatio * Time.deltaTime);
        }
    }

    public void SetInterestLevelDisplay(float level)
    {
        interestLevelFiller.fillAmount = 0;
        interestLevelToShow = level;
    }

    public void RefreshDisplay()
    {
        nameText.text = linkedObject.objectName;
        illustration.sprite = linkedObject.illustration;
        categoryDisplay1.color = categoriesColor[(int)linkedObject.categories[0]];
        categoryDisplay1.sprite = categoriesIcon[(int)linkedObject.categories[0]];
        categoryDisplay2.color = categoriesColor[(int)linkedObject.categories[1]];
        categoryDisplay2.sprite = categoriesIcon[(int)linkedObject.categories[1]];

    }

    public override void OnHoverIn()
    {
        if(canBeHovered)
        {
            StartCoroutine(hoverAnim.anim.Play(hoverAnim));
        }
        else
        {
            isHovered = false;
        }
    }

    public override void OnHoverOut()
    {
        StartCoroutine(hoverAnim.anim.PlayBackward(hoverAnim, true));
    }
}
