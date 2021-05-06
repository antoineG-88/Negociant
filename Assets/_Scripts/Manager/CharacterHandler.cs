using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterHandler : UIInteractable
{
    [Header("References")]
    [HideInInspector] public Character character;
    public Image illustrationImage;
    public TweeningAnimator selectionAnim;
    public RectTransform rectTransform;
    public float gazeHeadOffset;
    public Text nameText;
    public Image enthousiasmFiller;
    [HideInInspector] public RectTransform characterCanvasRectTransform;
    public GameObject annoyedFxPrefab;
    public GameObject happyFxPrefab;
    public TweeningAnimator backWardApparitionAnim;
    public GameObject enthousiasmGauge;
    public float gazeLerpRatio;
    public Image identificationCircle;
    public List<RectTransform> belongingsSpaces;
    public TweeningAnimator belongingsAnim;
    public TweeningAnimator hoveredWithStallObjectAnim;
    [Header("Speak&Think Options")]
    public TweeningAnimator speakingBoxAnim;
    public RectTransform speakingBox;
    public Text speakingText;
    public TweeningAnimator thinkingBoxAnim;
    public RectTransform thinkingBox;
    public Image thinkingObjectImage;
    [Header("Random object > Temporary")]
    public List<Object> allObjectsForCharacter;
    public Vector2Int minMaxObjectPerChara;
    public CharaObject charaObjectPrefab;
    public Vector2 minMaxObjectPersonnalValue;
    [Header("Behavior General Options")]
    public float initialInterestLevel;
    public float initialCuriosityLevel;
    public float baseLookingTime;
    public float reflexionTime;
    public float curiosityIncreaseSpeed;
    public Vector2 minMaxRandomCuriosity;
    public float enthousiasmIncreaseWithCorrectPresent;
    public float enthousiasmDecreaseWithIncorrectPresent;
    public float interestLevelMultiplierWithCorrectCategoryArgument;
    public float interestLevelMultiplierWithIncorrectCategoryArgument;
    public float enthousiasmDecreaseWithIncorrectArgument;
    [Range(0f, 1f)] public float initialEntousiasm;
    public float timeBeforeEnthousiasmDecrease;
    public float enthousiasmDecreaseRate;
    [Space]
    public float reactTimePresent;
    public DropOption askOption;
    public CanvasGroup dropOptionCanvasGroup;

    [HideInInspector] public List<CharaObject> belongings;
    [HideInInspector] public bool isSelected;
    [HideInInspector] public RectTransform gazeDisplay;
    [HideInInspector] public Color identificationColor;
    [HideInInspector] public bool isLeaving;
    [HideInInspector] public bool isTalking;
    [HideInInspector] public float exchangeTreshold;

    [HideInInspector] public List<PotentialObject> potentialObjects;
    [HideInInspector] public PotentialObject lookedObject;
    [HideInInspector] public float gazeTimeRmn;
    [HideInInspector] public float nonGazingTimeRMN;
    [HideInInspector] public float maxCuriosityLevel;
    [HideInInspector] public float timeSpendRefreshEnthousiasm;
    [HideInInspector] public float currentEnthousiasm;
    [HideInInspector] public bool isAppearing;
    [HideInInspector] public bool isHoveredWithStallObject;
    private bool hoveredWithObjectFlag;
    private bool selectedFlag;
    [HideInInspector] public bool isSpeaking;
    [HideInInspector] public bool isThinking;
    private float currentThinkTimeRmn;
    private float currentSpeakTimeRmn;
    private PotentialObject presentedObjectToThink;
    private CharaObject draggedCharaObject;

    public virtual void Init()
    {
        hoveredWithStallObjectAnim.GetReferences();
        hoveredWithStallObjectAnim.anim = Instantiate(hoveredWithStallObjectAnim.anim);
        belongingsAnim.GetReferences();
        belongingsAnim.anim = Instantiate(belongingsAnim.anim);
        belongingsAnim.anim.SetAtEndState(belongingsAnim);
        timeSpendRefreshEnthousiasm = 0;
        SetInitialState(initialInterestLevel, initialCuriosityLevel, initialEntousiasm);
        thinkingBoxAnim.anim = Instantiate(thinkingBoxAnim.anim);
        thinkingBoxAnim.GetReferences();
        thinkingBoxAnim.anim.SetAtEndState(thinkingBoxAnim);
        speakingBoxAnim.anim = Instantiate(speakingBoxAnim.anim);
        speakingBoxAnim.GetReferences();
        speakingBoxAnim.anim.SetAtEndState(speakingBoxAnim);
        belongingsAnim.canvasGroup.blocksRaycasts = false;
        belongingsAnim.canvasGroup.interactable = false;
        dropOptionCanvasGroup.blocksRaycasts = false;
        askOption.Disable();
    }

    public abstract void UpdateBehavior();

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
                UpdateBehavior();
            }

            UpdateGazeDisplay();

            UpdateCharacterInfoDisplay();

            UpdatePlayerActionAsTarget();

            UpdateSpeakingAndThinking();

            DragAndDropCharaObjectUpdate();
        }
    }

    private void UpdatePlayerActionAsTarget()
    {
        if(NegoceManager.I.playerHandler.draggedStallObject != null && isHovered)
        {
            isHoveredWithStallObject = true;
        }
        if(NegoceManager.I.playerHandler.draggedStallObject == null || (!isHovered && !NegoceManager.I.playerHandler.presentOption.isCurrentlyHoveredCorrectly && !NegoceManager.I.playerHandler.argumentOption.isCurrentlyHoveredCorrectly))
        {
            isHoveredWithStallObject = false;
        }

        if(isHoveredWithStallObject && !hoveredWithObjectFlag)
        {
            hoveredWithObjectFlag = true;
            StartCoroutine(hoveredWithStallObjectAnim.anim.Play(hoveredWithStallObjectAnim));
        }

        if (!isHoveredWithStallObject && hoveredWithObjectFlag)
        {
            hoveredWithObjectFlag = false;
            StartCoroutine(hoveredWithStallObjectAnim.anim.PlayBackward(hoveredWithStallObjectAnim, true));
        }
    }

    private void UpdateCharacterInfoDisplay()
    {
        if(isSelected)
        {
            if(!selectedFlag)
            {
                StartCoroutine(belongingsAnim.anim.PlayBackward(belongingsAnim, true));
                belongingsAnim.canvasGroup.blocksRaycasts = true;
                belongingsAnim.canvasGroup.interactable = true;
                selectedFlag = true;
            }
            nameText.gameObject.SetActive(true);
        }
        else
        {
            if(selectedFlag)
            {
                StartCoroutine(belongingsAnim.anim.Play(belongingsAnim));
                belongingsAnim.canvasGroup.blocksRaycasts = false;
                belongingsAnim.canvasGroup.interactable = false;
                selectedFlag = false;
            }
            nameText.gameObject.SetActive(false);
        }

        enthousiasmFiller.fillAmount = currentEnthousiasm / 1;
    }

    public void UpdateSpeakingAndThinking()
    {
        if (currentThinkTimeRmn > 0)
        {
            currentThinkTimeRmn -= Time.deltaTime;
        }

        if (presentedObjectToThink != null)
        {
            if(currentThinkTimeRmn <= 0)
            {
                ReactToPresent(presentedObjectToThink);
                presentedObjectToThink = null;
                StartCoroutine(thinkingBoxAnim.anim.Play(thinkingBoxAnim));
            }
        }
    }

    public void ReactToPresent(PotentialObject presentedObject)
    {
        if (DoesObjectHaveHigherInterestLevel(presentedObject))
        {
            currentEnthousiasm += enthousiasmIncreaseWithCorrectPresent;
            Instantiate(happyFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), happyFxPrefab.transform.rotation, characterCanvasRectTransform);
        }
        else
        {
            currentEnthousiasm -= enthousiasmDecreaseWithIncorrectPresent;
            Instantiate(annoyedFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), annoyedFxPrefab.transform.rotation, characterCanvasRectTransform);
        }
        presentedObject.IncreaseInterestLevel(0, true);

        if (NegoceManager.I.selectedCharacter == this)
        {
            presentedObject.stallObject.SetInterestLevelDisplay(presentedObject.knownInterestLevel / exchangeTreshold, identificationColor);
        }
        currentEnthousiasm = Mathf.Clamp(currentEnthousiasm, 0f, 1f);
    }

    public void PresentObject(StallObject presentedObject)
    {
        presentedObjectToThink = GetPotentialFromStallObject(presentedObject);
        LookObject(presentedObjectToThink, reactTimePresent);
        Think(reactTimePresent, presentedObjectToThink);
    }

    public void ArgumentCategoryOnObject(GameData.CategoryProperties categoryProperties, StallObject argumentedObject)
    {
        bool categoryInitialInterest = false;
        for (int i = 0; i < character.initialInterests.Count; i++)
        {
            if(character.initialInterests[i] == categoryProperties.category)
            {
                categoryInitialInterest = true;
            }
        }

        if(categoryInitialInterest)
        {
            GetPotentialFromStallObject(argumentedObject).IncreaseInterestLevel(interestLevelMultiplierWithCorrectCategoryArgument * categoryProperties.argumentInterestLevelIncrease, true);
            Instantiate(happyFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), happyFxPrefab.transform.rotation, characterCanvasRectTransform);
        }
        else
        {
            GetPotentialFromStallObject(argumentedObject).IncreaseInterestLevel(interestLevelMultiplierWithIncorrectCategoryArgument * categoryProperties.argumentInterestLevelIncrease, true);
            currentEnthousiasm -= enthousiasmDecreaseWithIncorrectArgument;
            Instantiate(annoyedFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), annoyedFxPrefab.transform.rotation, characterCanvasRectTransform);
        }

        if (NegoceManager.I.selectedCharacter == this)
        {
            argumentedObject.SetInterestLevelDisplay(GetPotentialFromStallObject(argumentedObject).knownInterestLevel / exchangeTreshold, identificationColor);
        }

        currentEnthousiasm = Mathf.Clamp(currentEnthousiasm, 0f, 1f);
    }

    public void Leave()
    {
        NegoceManager.I.MakeCharacterLeave(this);
        isLeaving = true;
    }

    public void LookObject(PotentialObject objectToLook, float timeToLook)
    {
        objectToLook.curiosityLevel = 0;
        gazeTimeRmn = timeToLook;
        lookedObject = objectToLook;
    }

    public void StartNonGazing(float nonGazingTime)
    {
        nonGazingTimeRMN = nonGazingTime;
        lookedObject = null;
    }

    public void RefreshEnthousiasm()
    {
        timeSpendRefreshEnthousiasm = 0;
    }

    public void Think(float timeToThink, PotentialObject potentialObjectToThink)
    {
        currentThinkTimeRmn = timeToThink;
        if(potentialObjectToThink != null)
        {
            thinkingObjectImage.color = Color.white;
            thinkingObjectImage.sprite = potentialObjectToThink.stallObject.linkedObject.illustration;
        }
        else
        {
            thinkingObjectImage.color = Color.clear;
        }
        StartCoroutine(thinkingBoxAnim.anim.PlayBackward(thinkingBoxAnim, true));
    }


    private void DragAndDropCharaObjectUpdate()
    {
        if (isSelected)
        {
            for (int i = 0; i < belongings.Count; i++)
            {
                if (draggedCharaObject == null && belongings[i].isDragged)
                {
                    draggedCharaObject = belongings[i];
                }
            }

            if (draggedCharaObject != null)
            {
                foreach (CharaObject charaObject in belongings)
                {
                    charaObject.canvasGroup.blocksRaycasts = false;
                }

                Vector3 objectPosToFollow = Input.mousePosition;

                if (!NegoceManager.I.playerHandler.IsPlayerTalking())
                {
                    askOption.Enable("");
                    dropOptionCanvasGroup.blocksRaycasts = true;
                }
                else
                {
                    dropOptionCanvasGroup.blocksRaycasts = false;
                    askOption.Disable();
                }

                if (askOption.isCurrentlyHoveredCorrectly)
                {
                    objectPosToFollow = askOption.rectTransform.position;
                }

                draggedCharaObject.rectTransform.position = objectPosToFollow;

                if (Input.GetMouseButtonUp(0))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (askOption.isCurrentlyHoveredCorrectly)
                        {
                            StartCoroutine(askOption.Select());
                            Debug.Log("discute");
                        }
                    }

                    NegoceManager.I.exchangeHandler.DropCharaObject(draggedCharaObject);
                    draggedCharaObject.StopDrag();
                    draggedCharaObject.rectTransform.position = draggedCharaObject.charaBelongingSpace.position;

                    draggedCharaObject.isDragged = false;
                    draggedCharaObject = null;
                }
                NegoceManager.I.draggedCharaObject = draggedCharaObject;
            }
            else
            {
                foreach (CharaObject charaObject in belongings)
                {
                    charaObject.canvasGroup.blocksRaycasts = true;
                }
            }
        }
    }

    #region UsefullFunctions

    public bool IsLookingAt(StallObject stallObject)
    {
        return lookedObject != null && lookedObject.stallObject == stallObject;
    }

    public PotentialObject GetMaxCuriosityObjectOnVitrine(List<PotentialObject> potentialObjectsToCheck)
    {
        maxCuriosityLevel = 0;
        PotentialObject maxCuriosityObject = null;
        foreach (PotentialObject potentialObject in potentialObjectsToCheck)
        {
            if (potentialObject.curiosityLevel > maxCuriosityLevel && potentialObject.stallObject.stallSpace.isVitrine)
            {
                maxCuriosityObject = potentialObject;
                maxCuriosityLevel = potentialObject.curiosityLevel;
            }
        }
        return maxCuriosityObject;
    }

    public bool DoesObjectHasCommonCategory(Object objectToCheck, List<Category> categoryPool)
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

    public bool DoesObjectHaveHigherInterestLevel(PotentialObject objectToCheck)
    {
        float averageInterestLevel = 0;
        for (int i = 0; i < potentialObjects.Count; i++)
        {
            averageInterestLevel += potentialObjects[i].interestLevel;
        }
        averageInterestLevel /= potentialObjects.Count;
        
        return objectToCheck.interestLevel > averageInterestLevel;
    }

    public int GetNumberOfInterestingObjectOnVitrine()
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

    public PotentialObject GetPotentialFromStallObject(StallObject searchedStallObject)
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
        backWardApparitionAnim.anim = Instantiate(backWardApparitionAnim.anim);
        backWardApparitionAnim.GetReferences();
        nameText.gameObject.SetActive(false);
        identificationCircle.gameObject.SetActive(false);
        enthousiasmGauge.SetActive(false);
        StartCoroutine(backWardApparitionAnim.anim.PlayBackward(backWardApparitionAnim, true));
        yield return new WaitForSeconds(backWardApparitionAnim.anim.animationTime);
        identificationCircle.gameObject.SetActive(true);
        enthousiasmGauge.SetActive(true);
        isAppearing = false;
        UnSelect(true);
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

    public void SetInitialState(float startInterestLevel, float startCuriosityLevel, float startEnthousiasm)
    {
        currentEnthousiasm = startEnthousiasm;
        foreach (PotentialObject potentialObject in potentialObjects)
        {
            if (DoesObjectHasCommonCategory(potentialObject.stallObject.linkedObject, character.initialInterests))
            {
                potentialObject.interestLevel = startInterestLevel;
                potentialObject.curiosityLevel = startCuriosityLevel;
            }
            potentialObject.curiosityLevel = Random.Range(minMaxRandomCuriosity.x, minMaxRandomCuriosity.y);
        }
    }

    #endregion

    #region Display

    Vector2 gazeDirection;
    float gazeAngle;
    public void UpdateGazeDisplay()
    {
        if (lookedObject != null)
        {
            if (lookedObject.stallObject == null)
            {
                lookedObject = null;
            }
            else
            {
                gazeDirection = rectTransform.position + Vector3.up * gazeHeadOffset - gazeDisplay.position;
                gazeDirection.Normalize();
                gazeAngle = Vector2.SignedAngle(Vector2.up, gazeDirection);

                gazeDisplay.rotation = Quaternion.Euler(0, 0, gazeAngle);


                gazeDisplay.position = Vector2.Lerp(gazeDisplay.position, lookedObject.stallObject.rectTransform.position, gazeLerpRatio * Time.deltaTime);
                if (!gazeDisplay.gameObject.activeSelf)
                {
                    RefreshGazeOrigin();
                }
                gazeDisplay.gameObject.SetActive(true);
            }
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

        for (int i = 0; i < potentialObjects.Count; i++)
        {
            if (!NegoceManager.I.playerHandler.allStallObjects.Contains(potentialObjects[i].stallObject))
            {
                potentialObjects.RemoveAt(i);
                i--;
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
        illustrationImage.preserveAspect = true;
        if(gazeDisplay != null)
        {
            gazeDisplay.GetComponent<Image>().color = identificationColor;
        }
        identificationCircle.color = new Color(identificationColor.r, identificationColor.g, identificationColor.b, 0.5f);
        selectionAnim.GetReferences();

        for (int i = 0; i < belongingsSpaces.Count; i++)
        {

            if(i >= belongings.Count)
            {
                belongingsSpaces[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < belongings.Count; i++)
        {
            belongings[i].rectTransform.position = belongingsSpaces[i].position;
            belongings[i].charaBelongingSpace = belongingsSpaces[i];
        }
    }

    public void SetRandomListOfCharaObject()
    {
        CharaObject newCharaObject = null;
        for (int i = 0; i < Random.Range(minMaxObjectPerChara.x, minMaxObjectPerChara.y + 1); i++)
        {
            newCharaObject = Instantiate(charaObjectPrefab, belongingsAnim.rectTransform);
            newCharaObject.linkedObject = allObjectsForCharacter[Random.Range(0, allObjectsForCharacter.Count)];
            newCharaObject.personnalValue = Random.Range(minMaxObjectPersonnalValue.x, minMaxObjectPersonnalValue.y);
            newCharaObject.RefreshDisplay();
            belongings.Add(newCharaObject);
        }

        exchangeTreshold = 0;
        for (int i = 0; i < belongings.Count; i++)
        {
            exchangeTreshold += belongings[i].personnalValue;
        }
        exchangeTreshold /= belongings.Count;
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
        public float knownInterestLevel;

        public void IncreaseInterestLevel(float amount, bool refreshKnownLevel)
        {
            if (interestLevel < 0)
            {
                interestLevel = 0;
            }
            interestLevel += amount;

            if (refreshKnownLevel)
            {
                knownInterestLevel = interestLevel;
            }
        }
        public PotentialObject(StallObject _standObject)
        {
            stallObject = _standObject;
            interestLevel = 0;
            curiosityLevel = 0;
            knownInterestLevel = -1;
        }
    }
}
