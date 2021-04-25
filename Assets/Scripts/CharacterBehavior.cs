using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBehavior : UIInteractable
{
    public Character character;
    public Image illustrationImage;
    public TweeningAnimator selectionAnim;
    public RectTransform rectTransform;
    public float gazeHeadOffset;
    public Text nameText;
    public Image enthousiasmFiller;
    [Header("Decisive options")]
    public float d_initialInterestLevel;
    public float d_initialCuriosityLevel;
    public bool[] d_gazeReflexionAlternance;
    public float d_minLookingTime;
    public float d_reflexionTime;
    public float d_curiosityDecreaseSpeed;
    public float d_curiosityIncreaseSpeed;
    public float d_initialInterestCuriosityBoost;
    public float d_initialInterestLookTimeRatio;
    public Vector2 minMaxRandomCuriosity;
    public float d_enthousiasmIncreaseWithCorrectPresent;
    public float d_enthousiasmDecreaseWithIncorrectPresent;
    public float d_enthousiasmBonusIncreaseWhenlooking;
    public float d_interestLevelIncreaseByPresenting;
    [Range(0f, 1f)] public float d_initialEntousiasm;
    [Header("More options")]
    public float timeBeforeEnthousiasmDecrease;
    public float enthousiasmDecreaseRate;
    [Header("Gaze options")]
    public float gazeLerpRatio;
    public Image identificationCircle;

    [HideInInspector] public bool isSelected;
    [HideInInspector] public RectTransform gazeDisplay;
    [HideInInspector] public Color identificationColor;
    [HideInInspector] public bool isLeaving;

    [HideInInspector] public List<PotentialObject> potentialObjects;
    private PotentialObject lookedObject;
    private float gazeTimeRmn;
    private float reflexionTimeRMN;
    private float maxCuriosityLevel;
    private PotentialObject potential;
    private int alternanceIndex;
    private float timeSpendRefreshEnthousiasm;
    private float currentEnthousiasm;

    void Start()
    {
        alternanceIndex = 0;
        timeSpendRefreshEnthousiasm = 0;
        if(character.temper == Temper.Decisive)
        {
            SetInitialState(d_initialInterestLevel, d_initialCuriosityLevel, d_initialEntousiasm);
        }
    }

    public override void Update()
    {
        if(clickedDown)
        {
            NegoceManager.I.SelectCharacter(this);
        }

        if(potentialObjects != null)
        {
            if (character.temper == Temper.Decisive)
            {
                DecisiveBehavior();
            }
        }

        UpdateGazeDisplay();

        UpdateCharacterInfoDisplay();
        base.Update();
    }

    private void UpdateCharacterInfoDisplay()
    {
        if(isSelected)
        {
            nameText.gameObject.SetActive(true);
        }
        else
        {
            nameText.gameObject.SetActive(false);
        }
    }


    #region Tempers Behaviors
    private void DecisiveBehavior()
    {
        if(gazeTimeRmn > 0)
        {
            gazeTimeRmn -= Time.deltaTime;
        }

        if (reflexionTimeRMN > 0)
        {
            reflexionTimeRMN -= Time.deltaTime;
        }

        if(reflexionTimeRMN <= 0 && gazeTimeRmn <= 0)
        {
            if(d_gazeReflexionAlternance[alternanceIndex])
            {
                LookObject(GetMaxCuriosityObject(), d_minLookingTime * (DoesObjectHasCommonCategory(GetMaxCuriosityObject().stallObject.linkedObject, character.initialInterests) ? d_initialInterestLookTimeRatio : 1));
            }
            else
            {
                StartReflexion(d_reflexionTime);
            }
        }

        foreach(PotentialObject potentialObject in potentialObjects)
        {
            if(potentialObject != lookedObject && potentialObject.stallObject.stallSpace.isVitrine)
            {
                potentialObject.curiosityLevel += Time.deltaTime * d_curiosityIncreaseSpeed
                    * (DoesObjectHasCommonCategory(potentialObject.stallObject.linkedObject, character.initialInterests) ? d_initialInterestCuriosityBoost : 1);
            }
        }

        timeSpendRefreshEnthousiasm += Time.deltaTime;

        if(timeSpendRefreshEnthousiasm > timeBeforeEnthousiasmDecrease)
        {
            if(currentEnthousiasm > 0)
            {
                currentEnthousiasm -= Time.deltaTime * enthousiasmDecreaseRate;
            }
            else
            {
                Leave();
                currentEnthousiasm = 0;
            }
        }
        enthousiasmFiller.fillAmount = currentEnthousiasm / 1;
    }
    #endregion

    public void PresentObject(StallObject presentedObject)
    {
        RefreshEnthousiasm();
        PotentialObject presentedPotentialObject = null;
        foreach(PotentialObject potentialObject in potentialObjects)
        {
            if(potentialObject.stallObject == presentedObject)
            {
                presentedPotentialObject = potentialObject;
            }
        }

        if(DoesObjectHasCommonCategory(presentedPotentialObject.stallObject.linkedObject, character.initialInterests))
        {
            currentEnthousiasm += d_enthousiasmIncreaseWithCorrectPresent;
            Debug.Log("Great ! C'était un objet de ses préférences");
        }
        else
        {
            currentEnthousiasm -= d_enthousiasmDecreaseWithIncorrectPresent;
            Debug.Log("Raté ... Il n'aime ce genre d'objet");
        }

        if (lookedObject == presentedPotentialObject)
        {
            currentEnthousiasm += d_enthousiasmBonusIncreaseWhenlooking;
            Debug.Log("Bon timing ! Il regardé quand tu lui a montré !!!!!");
        }

        currentEnthousiasm = Mathf.Clamp(currentEnthousiasm ,0f, 1f);
        reflexionTimeRMN = 0;
        LookObject(presentedPotentialObject, d_minLookingTime);
    }

    private void Leave()
    {
        NegoceManager.I.MakeCharacterLeave(this);
        isLeaving = true;
    }

    private void IncreaseAlternanceIndex()
    {
        alternanceIndex++;
        if (alternanceIndex >= d_gazeReflexionAlternance.Length)
        {
            alternanceIndex = 0;
        }
    }

    private PotentialObject GetMaxCuriosityObject()
    {
        maxCuriosityLevel = 0;
        potential = null;
        foreach (PotentialObject potentialObject in potentialObjects)
        {
            if (potentialObject.curiosityLevel > maxCuriosityLevel && potentialObject.stallObject.stallSpace.isVitrine)
            {
                potential = potentialObject;
                maxCuriosityLevel = potentialObject.curiosityLevel;
            }
        }
        return potential;
    }

    private void LookObject(PotentialObject objectToLook, float timeToLook)
    {
        if (objectToLook != lookedObject)
        {
            IncreaseAlternanceIndex();
        }
        objectToLook.curiosityLevel = 0;
        gazeTimeRmn = timeToLook;
        lookedObject = objectToLook;
    }

    private void StartReflexion(float reflexionTime)
    {
        IncreaseAlternanceIndex();
        reflexionTimeRMN = reflexionTime;
        lookedObject = null;
    }

    private void RefreshEnthousiasm()
    {
        timeSpendRefreshEnthousiasm = 0;
    }

    private void SetInitialState(float startInterestLevel, float startCuriosityLevel, float startEnthousiasm)
    {
        currentEnthousiasm = startEnthousiasm;
        foreach(PotentialObject potentialObject in potentialObjects)
        {
            if(DoesObjectHasCommonCategory(potentialObject.stallObject.linkedObject, character.initialInterests))
            {
                potentialObject.interestLevel = startInterestLevel;
                potentialObject.curiosityLevel = startCuriosityLevel;
            }
            potentialObject.curiosityLevel = Random.Range(minMaxRandomCuriosity.x, minMaxRandomCuriosity.y);
        }
    }

    private bool DoesObjectHasCommonCategory(Object objectToCheck, List<Category> categoryPool)
    {
        bool hasCommonCategory = false;
        for (int y = 0; y < categoryPool.Count; y++)
        {
            for (int x = 0; x < objectToCheck.categories.Count; x++)
            {
                if (objectToCheck.categories[x] == categoryPool[y])
                {
                    hasCommonCategory = true;
                }
            }
        }
        return hasCommonCategory;
    }

    Vector2 gazeDirection;
    float gazeAngle;
    public void UpdateGazeDisplay()
    {
        if (lookedObject != null)
        {
            gazeDirection = rectTransform.position + Vector3.up * gazeHeadOffset - gazeDisplay.position;
            gazeDirection.Normalize();
            gazeAngle = Vector2.SignedAngle(Vector2.up, gazeDirection);

            gazeDisplay.rotation = Quaternion.Euler(0, 0, gazeAngle);


            gazeDisplay.position = Vector2.Lerp(gazeDisplay.position , lookedObject.stallObject.rectTransform.position, gazeLerpRatio * Time.deltaTime);
            if(!gazeDisplay.gameObject.activeSelf)
            {
                RefreshGazeOrigin();
            }
            gazeDisplay.gameObject.SetActive(true);
        }
        else
        {
            gazeDisplay.gameObject.SetActive(false);
        }
    }

    private void RefreshGazeOrigin()
    {
        gazeDisplay.position = rectTransform.position + (Vector3)(new Vector2(0, gazeHeadOffset));
    }

    public void RefreshPotentialObjects()
    {
        if(potentialObjects == null)
        {
            potentialObjects = new List<PotentialObject>();
        }

        foreach(PotentialObject potentialObject in potentialObjects)
        {
            if(!NegoceManager.I.playerHandler.allStallObjects.Contains(potentialObject.stallObject))
            {
                potentialObjects.Remove(potentialObject);
            }
        }

        for (int i = 0; i < NegoceManager.I.playerHandler.allStallObjects.Count; i++)
        {
            bool isMissingStandObjectInPotentialObjects = true;
            foreach (PotentialObject potentialObject in potentialObjects)
            {
                if(potentialObject.stallObject == NegoceManager.I.playerHandler.allStallObjects[i])
                {
                    isMissingStandObjectInPotentialObjects = false;
                }
            }

            if(isMissingStandObjectInPotentialObjects)
            {
                potentialObjects.Add(new PotentialObject(NegoceManager.I.playerHandler.allStallObjects[i]));
            }
        }
    }

    public void RefreshCharacterDisplay()
    {
        selectionAnim.anim = Instantiate(selectionAnim.anim);
        illustrationImage.sprite = character.illustration;
        illustrationImage.SetNativeSize();
        if(gazeDisplay != null)
        {
            gazeDisplay.GetComponent<Image>().color = identificationColor;
        }
        identificationCircle.color = new Color(identificationColor.r, identificationColor.g, identificationColor.b, 0.5f);
        selectionAnim.GetReferences();
    }

    public void Select()
    {
        if(!isSelected)
        {
            isSelected = true;
            StartCoroutine(selectionAnim.anim.Play(selectionAnim, selectionAnim.originalPos));
        }
    }
    public void UnSelect(bool forced)
    {
        if (forced)
        {
            selectionAnim.anim.SetAtStartState(selectionAnim);
            isSelected = false;
        }
        else if (isSelected)
        {
            StartCoroutine(selectionAnim.anim.PlayBackward(selectionAnim, selectionAnim.originalPos, true));
            isSelected = false;
        }

    }

    public override void OnHoverIn()
    {

    }

    public override void OnHoverOut()
    {

    }

    [System.Serializable]
    public class PotentialObject
    {
        public StallObject stallObject;
        public float interestLevel;
        public float curiosityLevel;

        public PotentialObject(StallObject _standObject)
        {
            stallObject = _standObject;
            interestLevel = 0;
            curiosityLevel = 0;
        }
    }
}
