using System.Collections;
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
    public RectTransform characterCanvasRectTransform;
    public GameObject annoyedFxPrefab;
    public GameObject happyFxPrefab;
    public TweeningAnimator backWardApparitionAnim;
    public GameObject enthousiasmGauge;
    public float gazeLerpRatio;
    public Image identificationCircle;

    [Header("Decisive options")]
    public float d_initialInterestLevel;
    public float d_initialCuriosityLevel;
    public float d_minLookingTime;
    public float d_baseLookingTime;
    public float d_reflexionTime;
    public float d_curiosityIncreaseSpeed;
    public float d_highLevelInterestCuriosityBoost;
    public int[] d_gazedObjectPerGazeTimePerInterestingObjectOnVitrine;
    public Vector2 minMaxRandomCuriosity;
    public float d_enthousiasmIncreaseWithCorrectPresent;
    public float d_enthousiasmDecreaseWithIncorrectPresent;
    public float d_interestLevelIncreaseByPresenting;
    public float d_curiosityLevelIncreaseByPresenting;
    [Range(0f, 1f)] public float d_initialEntousiasm;
    [Header("More options")]
    public float timeBeforeEnthousiasmDecrease;
    public float enthousiasmDecreaseRate;

    [HideInInspector] public bool isSelected;
    [HideInInspector] public RectTransform gazeDisplay;
    [HideInInspector] public Color identificationColor;
    [HideInInspector] public bool isLeaving;
    [HideInInspector] public bool isTalking;

    [HideInInspector] public List<PotentialObject> potentialObjects;
    private PotentialObject lookedObject;
    private float gazeTimeRmn;
    private float reflexionTimeRMN;
    private float maxCuriosityLevel;
    private float timeSpendRefreshEnthousiasm;
    private float currentEnthousiasm;
    private int gazedObjectThisGazeTime;
    private bool isAppearing;

    void Start()
    {
        timeSpendRefreshEnthousiasm = 0;
        if(character.temper == Temper.Decisive)
        {
            SetInitialState(d_initialInterestLevel, d_initialCuriosityLevel, d_initialEntousiasm);
        }
    }

    public void Update()
    {
        if(!isAppearing)
        {
            if (clickedDown)
            {
                NegoceManager.I.SelectCharacter(this);
            }

            if (potentialObjects != null)
            {
                if (character.temper == Temper.Decisive)
                {
                    DecisiveBehavior();
                }
            }

            UpdateGazeDisplay();

            UpdateCharacterInfoDisplay();
        }
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
        if(!isTalking)
        {
            if (gazeTimeRmn > 0)
            {
                gazeTimeRmn -= Time.deltaTime;
            }

            if (reflexionTimeRMN > 0)
            {
                reflexionTimeRMN -= Time.deltaTime;
            }

            if (reflexionTimeRMN <= 0 && gazeTimeRmn <= 0)
            {
                if (d_gazedObjectPerGazeTimePerInterestingObjectOnVitrine[Mathf.Clamp(GetNumberOfInterestingObjectOnVitrine(), 0, d_gazedObjectPerGazeTimePerInterestingObjectOnVitrine.Length - 1)] - gazedObjectThisGazeTime > 0)
                {
                    LookObject(GetMaxCuriosityObject(), Mathf.Max(d_minLookingTime, d_baseLookingTime / d_gazedObjectPerGazeTimePerInterestingObjectOnVitrine[Mathf.Clamp(GetNumberOfInterestingObjectOnVitrine(), 0, d_gazedObjectPerGazeTimePerInterestingObjectOnVitrine.Length - 1)]));
                }
                else
                {
                    StartReflexion(d_reflexionTime);
                }
            }
        }
        else
        {
            StartReflexion(d_reflexionTime);
        }


        foreach(PotentialObject potentialObject in potentialObjects)
        {
            if(potentialObject != lookedObject && potentialObject.stallObject.stallSpace.isVitrine)
            {
                potentialObject.curiosityLevel += Time.deltaTime * d_curiosityIncreaseSpeed
                    * (DoesObjectHaveHigherInterestLevel(potentialObject) ? d_highLevelInterestCuriosityBoost : 1);
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
                if(!isTalking)
                {
                    Leave();
                }
                currentEnthousiasm = 0;
            }
        }
        enthousiasmFiller.fillAmount = currentEnthousiasm / 1;
    }
    #endregion

    public void PresentObject(StallObject presentedObject)
    {
        RefreshEnthousiasm();
        PotentialObject presentedPotentialObject = GetPotentialFromStallObject(presentedObject);
        if (DoesObjectHaveHigherInterestLevel(presentedPotentialObject))
        {
            currentEnthousiasm += d_enthousiasmIncreaseWithCorrectPresent;
            //Debug.Log("Great ! C'était un objet de ses préférences");
            Instantiate(happyFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), happyFxPrefab.transform.rotation, characterCanvasRectTransform);
        }
        else
        {
            currentEnthousiasm -= d_enthousiasmDecreaseWithIncorrectPresent;
            //Debug.Log("Raté ... Il n'aime ce genre d'objet");
            Instantiate(annoyedFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), annoyedFxPrefab.transform.rotation, characterCanvasRectTransform);
        }

        presentedPotentialObject.interestLevel += d_interestLevelIncreaseByPresenting;
        presentedPotentialObject.curiosityLevel += d_curiosityLevelIncreaseByPresenting;
        currentEnthousiasm = Mathf.Clamp(currentEnthousiasm ,0f, 1f);
    }

    private void Leave()
    {
        NegoceManager.I.MakeCharacterLeave(this);
        isLeaving = true;
    }

    private PotentialObject GetMaxCuriosityObject()
    {
        maxCuriosityLevel = 0;
        PotentialObject maxCuriosityObject = null;
        foreach (PotentialObject potentialObject in potentialObjects)
        {
            if (potentialObject.curiosityLevel > maxCuriosityLevel && potentialObject.stallObject.stallSpace.isVitrine)
            {
                maxCuriosityObject = potentialObject;
                maxCuriosityLevel = potentialObject.curiosityLevel;
            }
        }
        return maxCuriosityObject;
    }

    private void LookObject(PotentialObject objectToLook, float timeToLook)
    {
        gazedObjectThisGazeTime++;
        objectToLook.curiosityLevel = 0;
        gazeTimeRmn = timeToLook;
        lookedObject = objectToLook;
    }

    private void StartReflexion(float reflexionTime)
    {
        gazedObjectThisGazeTime = 0;
        reflexionTimeRMN = reflexionTime;
        lookedObject = null;
    }

    private void RefreshEnthousiasm()
    {
        timeSpendRefreshEnthousiasm = 0;
    }

    public bool IsLookingAt(StallObject stallObject)
    {
        return lookedObject != null && lookedObject.stallObject == stallObject;
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

    private bool DoesObjectHaveHigherInterestLevel(PotentialObject objectToCheck)
    {
        float averageInterestLevel = 0;
        for (int i = 0; i < potentialObjects.Count; i++)
        {
            averageInterestLevel += potentialObjects[i].interestLevel;
        }
        averageInterestLevel /= potentialObjects.Count;
        
        return objectToCheck.interestLevel > averageInterestLevel;
    }

    private int GetNumberOfInterestingObjectOnVitrine()
    {
        int numberOnVitrine = 0;

        foreach (PotentialObject potentialObject in potentialObjects)
        {
            if(potentialObject.stallObject.stallSpace.isVitrine && DoesObjectHaveHigherInterestLevel(potentialObject))
            {
                numberOnVitrine++;
            }
        }
        return numberOnVitrine;
    }

    private PotentialObject GetPotentialFromStallObject(StallObject searchedStallObject)
    {
        PotentialObject potentialObjectSearched = null;
        foreach (PotentialObject potentialObject in potentialObjects)
        {
            if (potentialObject.stallObject == searchedStallObject)
            {
                potentialObjectSearched = potentialObject;
            }
        }
        return potentialObjectSearched;
    }

    public IEnumerator Appear()
    {
        isAppearing = true;
        backWardApparitionAnim.GetReferences();
        nameText.gameObject.SetActive(false);
        identificationCircle.gameObject.SetActive(false);
        enthousiasmGauge.SetActive(false);
        StartCoroutine(backWardApparitionAnim.anim.PlayBackward(backWardApparitionAnim, true));
        yield return new WaitForSeconds(backWardApparitionAnim.anim.animationTime);
        identificationCircle.gameObject.SetActive(true);
        enthousiasmGauge.SetActive(true);
        isAppearing = false;

    }

    public void Select()
    {
        if (!isSelected)
        {
            isSelected = true;
            StartCoroutine(selectionAnim.anim.Play(selectionAnim));
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
            StartCoroutine(selectionAnim.anim.PlayBackward(selectionAnim, true));
            isSelected = false;
        }

    }

    #region Display

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

    public override void OnHoverIn()
    {

    }

    public override void OnHoverOut()
    {

    }

#endregion

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
