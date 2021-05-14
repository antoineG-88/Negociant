using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public abstract class CharacterHandler : UIInteractable
{
    [Header("References")]
    [HideInInspector] public Character character;
    public Image illustrationImage;
    public TweeningAnimator permanentDisplaySelectionAnim;
    public TweeningAnimator illuSelectionAnim;
    public RectTransform rectTransform;
    public float gazeHeadOffset;
    public Text nameText;
    public Image enthousiasmFiller;
    public float enthousiasmLerpRatio;
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
    public TweeningAnimator selectionDisplayAppearAnim;
    public CharacterExchangeHandler characterExchangeHandler;
    [Header("Speak&Think Options")]
    public TweeningAnimator speakingBoxAnim;
    public RectTransform speakingBox;
    public TMP_Text speakingText;
    public TweeningAnimator thinkingBoxAnim;
    public RectTransform thinkingBox;
    public Image thinkingObjectImage;
    public Text speakBoxName;
    public Image thinkBoxDeco;
    [Header("Random object > Temporary")]
    public List<Object> allObjectsForCharacter;
    public Vector2Int minMaxObjectPerChara;
    public CharaObject charaObjectPrefab;
    public Vector2 minMaxObjectPersonnalValue;
    [Header("Behavior General Options")]
    public float maxPersonnalValue;
    public float baseLookingTime;
    public float reflexionTime;
    public float curiosityIncreaseSpeed;
    public Vector2 minMaxRandomCuriosity;
    public float enthousiasmIncreaseWithCorrectPresent;
    public float enthousiasmDecreaseWithIncorrectPresent;
    public float interestLevelMultiplierWithCorrectCategoryArgument;
    public float interestLevelMultiplierWithCorrectFeatureArgument;
    public float enthousiasmDecreaseWithIncorrectArgument;
    [Range(0f, 1f)] public float initialEntousiasm;
    public float timeBeforeEnthousiasmDecrease;
    public float enthousiasmDecreaseRate;
    [Space]
    public float reactTimePresent;
    public DropOption askOption;
    public DropOption exchangeOption;
    public CanvasGroup dropOptionCanvasGroup;
    public float reactTimeArgument;

    [HideInInspector] public List<CharaObject> belongings;
    [HideInInspector] public bool isSelected;
    [HideInInspector] public RectTransform gazeDisplay;
    [HideInInspector] public Color identificationColor;
    [HideInInspector] public bool isLeaving;
    [HideInInspector] public bool isListening;

    [HideInInspector] public List<PotentialObject> potentialObjects;
    [HideInInspector] public PotentialObject lookedObject;
    [HideInInspector] public float gazeTimeRmn;
    [HideInInspector] public float nonGazingTimeRMN;
    [HideInInspector] public float maxCuriosityLevel;
    [HideInInspector] public float timeSpendRefreshEnthousiasm;
    [HideInInspector] public float currentEnthousiasm;
    [HideInInspector] public bool isAppearing;
    [HideInInspector] public bool isHoveredWithStallObject;
    [HideInInspector] public bool isHoveredWithCharaObject;
    [HideInInspector] public Vector2 targetPositionAtTheStall;
    private bool hoveredWithObjectFlag;
    private bool selectedFlag;
    [HideInInspector] public bool isSpeaking;
    [HideInInspector] public bool isThinking;
    private float currentThinkTimeRmn;
    private float currentSpeakTimeRmn;
    private PotentialObject presentedObjectToThink;
    private PotentialObject argumentedObjectToThink;
    private Object.Feature argumentedFeatureToThink;
    private CharaObject draggedCharaObject;
    private Speech currentSpeech;
    [HideInInspector] public string playerNotes;
    [HideInInspector] public int playerCategoryNote;

    public virtual void Init()
    {
        hoveredWithStallObjectAnim.GetReferences();
        hoveredWithStallObjectAnim.anim = Instantiate(hoveredWithStallObjectAnim.anim);
        timeSpendRefreshEnthousiasm = 0;
        SetInitialState(initialEntousiasm);
        thinkingBoxAnim.anim = Instantiate(thinkingBoxAnim.anim);
        thinkingBoxAnim.GetReferences();
        thinkingBoxAnim.anim.SetAtEndState(thinkingBoxAnim);
        speakingBoxAnim.anim = Instantiate(speakingBoxAnim.anim);
        speakingBoxAnim.GetReferences();
        speakingBoxAnim.anim.SetAtEndState(speakingBoxAnim);
        dropOptionCanvasGroup.blocksRaycasts = false;
        askOption.Disable();
        exchangeOption.Disable();
        enthousiasmFiller.fillAmount = initialEntousiasm;
        selectionDisplayAppearAnim.anim = Instantiate(selectionDisplayAppearAnim.anim);
        selectionDisplayAppearAnim.GetReferences();
        selectionDisplayAppearAnim.anim.SetAtStartState(selectionDisplayAppearAnim);
        illuSelectionAnim.anim = Instantiate(illuSelectionAnim.anim);
        permanentDisplaySelectionAnim.anim = Instantiate(permanentDisplaySelectionAnim.anim);
        belongingsAnim.anim = Instantiate(belongingsAnim.anim);
        selectionDisplayAppearAnim.anim = Instantiate(selectionDisplayAppearAnim.anim);
        illuSelectionAnim.GetReferences();
        permanentDisplaySelectionAnim.GetReferences();
        belongingsAnim.GetReferences();
        selectionDisplayAppearAnim.GetReferences();
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

                UpdateSpeakingAndThinking();

                DragAndDropCharaObjectUpdate();

                UpdateKnownFeatures();
            }

            UpdateGazeDisplay();

            UpdateCharacterInfoDisplay();

            UpdatePlayerActionAsTarget();

        }

    }

    private void UpdatePlayerActionAsTarget()
    {
        if(NegoceManager.I.playerHandler.draggedStallObject != null && isHovered )
        {
            isHoveredWithStallObject = true;
            NegoceManager.I.SelectCharacter(this);
        }
        if(NegoceManager.I.playerHandler.draggedStallObject == null || (!isHovered && !NegoceManager.I.playerHandler.presentOption.isCurrentlyHoveredCorrectly && !NegoceManager.I.playerHandler.exchangeOption.isCurrentlyHoveredCorrectly))
        {
            isHoveredWithStallObject = false;
        }

        if (draggedCharaObject != null && isHovered)
        {
            isHoveredWithCharaObject = true;
        }
        if (draggedCharaObject == null || (!isHovered && !askOption.isCurrentlyHoveredCorrectly && !exchangeOption.isCurrentlyHoveredCorrectly))
        {
            isHoveredWithCharaObject = false;
        }


        if ((isHoveredWithStallObject || isHoveredWithCharaObject) && !hoveredWithObjectFlag)
        {
            hoveredWithObjectFlag = true;
            StartCoroutine(hoveredWithStallObjectAnim.anim.Play(hoveredWithStallObjectAnim));
        }

        if (!isHoveredWithStallObject && !isHoveredWithCharaObject && hoveredWithObjectFlag)
        {
            hoveredWithObjectFlag = false;
            StartCoroutine(hoveredWithStallObjectAnim.anim.PlayBackward(hoveredWithStallObjectAnim, true));
        }
    }

    public void UpdateKnownFeatures()
    {
        foreach(PotentialObject potentialObject in potentialObjects)
        {
            foreach (PotentialObject.KnownFeature knownFeature in potentialObject.knownFeatures)
            {
                if(knownFeature.isKnown)
                {
                    if(knownFeature.timeRememberedRmn > 0)
                    {
                        knownFeature.timeRememberedRmn -= Time.deltaTime;
                    }
                    else
                    {
                        knownFeature.ForgetFeature();
                    }
                }
            }

            potentialObject.RefreshInterestLevel();
            if (NegoceManager.I.selectedCharacter == this)
            {
                potentialObject.stallObject.SetInterestLevelDisplay(potentialObject.interestLevel / maxPersonnalValue, identificationColor, false);
            }
        }
    }

    public void UpdateSpeakingAndThinking()
    {
        if (currentThinkTimeRmn > 0)
        {
            currentThinkTimeRmn -= Time.deltaTime;
        }
        else
        {
            isThinking = false;
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

        if (argumentedObjectToThink != null)
        {
            if (currentThinkTimeRmn <= 0)
            {
                ReactToArgumentFeature(argumentedFeatureToThink, argumentedObjectToThink.stallObject);
                argumentedObjectToThink = null;
                StartCoroutine(thinkingBoxAnim.anim.Play(thinkingBoxAnim));
            }
        }

        if (isSpeaking)
        {
            speakingText.text = currentSpeech.GetCurrentSpeechProgression(Time.deltaTime);
            if(currentSpeech.isFinished)
            {
                StartCoroutine(speakingBoxAnim.anim.Play(speakingBoxAnim));
                isSpeaking = false;
            }
        }
    }

    public void ReactToPresent(PotentialObject presentedObject)
    {
        if (DoesObjectHasCommonCategory(presentedObject.stallObject.linkedObject, character.initialInterests))
        {
            currentEnthousiasm += enthousiasmIncreaseWithCorrectPresent;
            Instantiate(happyFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), happyFxPrefab.transform.rotation, characterCanvasRectTransform);
        }
        else
        {
            currentEnthousiasm -= enthousiasmDecreaseWithIncorrectPresent;
            Instantiate(annoyedFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), annoyedFxPrefab.transform.rotation, characterCanvasRectTransform);
        }
        presentedObject.RefreshInterestLevel();

        if (NegoceManager.I.selectedCharacter == this)
        {
            presentedObject.stallObject.SetInterestLevelDisplay(presentedObject.interestLevel / maxPersonnalValue, identificationColor, false);
        }
        currentEnthousiasm = Mathf.Clamp(currentEnthousiasm, 0f, 1f);
    }

    public void PresentObject(StallObject presentedObject)
    {
        presentedObjectToThink = GetPotentialFromStallObject(presentedObject);
        LookObject(presentedObjectToThink, reactTimePresent);
        Think(reactTimePresent, presentedObjectToThink);
    }

    public void AskAbout(CharaObject askedObject)
    {
        askedObject.isPersonnalValueKnown = true;
        if(character.randomlyGenerated)
        {
            Speak(character.needs[Random.Range(0, character.needs.Count)].defaultHintToTell, 3);
        }
        else
        {
            Speak(character.GetPersonnalObjectFromObject(askedObject.linkedObject).infoGivenWhenAsked);
        }
    }

    public void ReactToArgumentFeature(Object.Feature featureArgumented, StallObject argumentedObject)
    {
        PotentialObject argumentedPotentialObject = GetPotentialFromStallObject(argumentedObject);
        PotentialObject.KnownFeature knownFeatureArgumented = argumentedPotentialObject.GetKnownFeatureFromFeature(featureArgumented);

        if (featureArgumented.isCategoryFeature)
        {
            bool categoryIsInitialInterest = false;
            for (int i = 0; i < character.initialInterests.Count; i++)
            {
                if (character.initialInterests[i] == featureArgumented.categoryProperties.category)
                {
                    categoryIsInitialInterest = true;
                }
            }

            if (categoryIsInitialInterest)
            {
                knownFeatureArgumented.LearnFeature(featureArgumented.rememberTime, interestLevelMultiplierWithCorrectCategoryArgument * featureArgumented.categoryProperties.argumentInterestLevelIncrease);
                Instantiate(happyFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), happyFxPrefab.transform.rotation, characterCanvasRectTransform);
                Speak(featureArgumented.categoryProperties.argumentSpeechGoodReaction, 5);
            }
            else
            {
                currentEnthousiasm -= enthousiasmDecreaseWithIncorrectArgument;
                Instantiate(annoyedFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), annoyedFxPrefab.transform.rotation, characterCanvasRectTransform);
                Speak(featureArgumented.categoryProperties.argumentSpeechBadReaction, 5);
            }
        }
        else
        {
            Character.Need correspondingNeed = null;
            for (int i = 0; i < character.needs.Count; i++)
            {
                for (int y = 0; y < featureArgumented.traits.Count; y++)
                {
                    if (character.needs[i].trait == featureArgumented.traits[y])
                    {
                        correspondingNeed = character.needs[i];
                    }
                }
            }

            if (correspondingNeed != null)
            {
                knownFeatureArgumented.LearnFeature(featureArgumented.rememberTime, interestLevelMultiplierWithCorrectFeatureArgument * featureArgumented.interestLevelIncrease);
                Instantiate(happyFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), happyFxPrefab.transform.rotation, characterCanvasRectTransform);
                Speak(correspondingNeed.reactionSpokenWhenArgumented);
            }
            else
            {
                Instantiate(annoyedFxPrefab, rectTransform.position + new Vector3(0, gazeHeadOffset, 0), happyFxPrefab.transform.rotation, characterCanvasRectTransform);
                currentEnthousiasm -= enthousiasmDecreaseWithIncorrectArgument;
                Speak(character.defaultSpeachWhenWrongArgument);
            }
        }

        currentEnthousiasm = Mathf.Clamp(currentEnthousiasm, 0f, 1f);
    }

    public void ArgumentFeature(Object.Feature featureArgumented, StallObject argumentedObject)
    {
        argumentedObjectToThink = GetPotentialFromStallObject(argumentedObject);
        argumentedFeatureToThink = featureArgumented;
        Think(reactTimeArgument, argumentedObjectToThink);
    }

    public void Think(float timeToThink, PotentialObject potentialObjectToThink)
    {
        isThinking = true;
        currentThinkTimeRmn = timeToThink;
        if (potentialObjectToThink != null)
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

    public void Speak(Speech speechToSpeak)
    {
        isSpeaking = true;
        currentSpeech = speechToSpeak;
        currentSpeech.InitSpeechStructure(character);
        currentSpeech.ResetProgression();
        StartCoroutine(speakingBoxAnim.anim.PlayBackward(speakingBoxAnim, true));
    }

    public void Speak(string singleSentenceToSpeak, float speakTime)
    {
        isSpeaking = true;
        currentSpeech = new Speech(singleSentenceToSpeak);
        currentSpeech.InitSpeechStructure(character);
        currentSpeech.ResetProgression();
        StartCoroutine(speakingBoxAnim.anim.PlayBackward(speakingBoxAnim, true));
    }

    public void Interrupt()
    {
        if(isSpeaking)
        {
            StartCoroutine(speakingBoxAnim.anim.Play(speakingBoxAnim));
            isSpeaking = false;
        }
        currentSpeech = null;
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
                if(isHoveredWithCharaObject && !isThinking && !isListening)
                {
                    if (!NegoceManager.I.playerHandler.IsPlayerTalking())
                    {
                        askOption.Enable("");
                        exchangeOption.Enable("");
                        dropOptionCanvasGroup.blocksRaycasts = true;
                    }
                    else
                    {
                        dropOptionCanvasGroup.blocksRaycasts = false;
                        askOption.Disable();
                        exchangeOption.Disable();
                    }

                    if (askOption.isCurrentlyHoveredCorrectly)
                    {
                        objectPosToFollow = askOption.rectTransform.position;
                    }

                    if (exchangeOption.isCurrentlyHoveredCorrectly)
                    {
                        objectPosToFollow = exchangeOption.rectTransform.position;
                    }
                }
                else
                {
                    dropOptionCanvasGroup.blocksRaycasts = false;
                    askOption.Disable();
                    exchangeOption.Disable();
                }

                draggedCharaObject.rectTransform.position = objectPosToFollow;

                if (Input.GetMouseButtonUp(0))
                {
                    if (askOption.isCurrentlyHoveredCorrectly)
                    {
                        StartCoroutine(askOption.Select());
                        NegoceManager.I.playerHandler.AskAbout(draggedCharaObject, this);
                    }

                    if (exchangeOption.isCurrentlyHoveredCorrectly)
                    {
                        StartCoroutine(exchangeOption.Select());
                        characterExchangeHandler.AddObjectToTrade(null, draggedCharaObject);
                        characterExchangeHandler.Open();
                    }

                    characterExchangeHandler.DropCharaObject(draggedCharaObject);

                    draggedCharaObject.StopDrag();
                    draggedCharaObject.rectTransform.position = draggedCharaObject.charaBelongingSpace.position;

                    draggedCharaObject.isDragged = false;
                    draggedCharaObject = null;
                }
                NegoceManager.I.draggedCharaObject = draggedCharaObject;
            }
            else
            {
                dropOptionCanvasGroup.blocksRaycasts = false;
                askOption.Disable();
                exchangeOption.Disable();
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

    public CharaObject GetCharaObjectFromObject(Object searchedObject)
    {
        CharaObject potentialCharaObject = null;
        foreach (CharaObject charaObject in belongings)
        {
            if (charaObject.linkedObject == searchedObject)
            {
                potentialCharaObject = charaObject;
            }
        }
        return potentialCharaObject;
    }

    public IEnumerator Appear()
    {
        isAppearing = true;
        backWardApparitionAnim.anim = Instantiate(backWardApparitionAnim.anim);
        backWardApparitionAnim.GetReferences();
        identificationCircle.gameObject.SetActive(false);
        enthousiasmGauge.SetActive(false);
        UnSelect(true);
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
            StartCoroutine(illuSelectionAnim.anim.Play(illuSelectionAnim));
            StartCoroutine(permanentDisplaySelectionAnim.anim.Play(permanentDisplaySelectionAnim));
            StartCoroutine(selectionDisplayAppearAnim.anim.Play(selectionDisplayAppearAnim));
            StartCoroutine(belongingsAnim.anim.PlayBackward(belongingsAnim, true));
            belongingsAnim.canvasGroup.blocksRaycasts = true;
            belongingsAnim.canvasGroup.interactable = true;
        }
    }

    public void UnSelect(bool forced)
    {
        if (forced)
        {
            illuSelectionAnim.anim.SetAtStartState(illuSelectionAnim);
            permanentDisplaySelectionAnim.anim.SetAtStartState(permanentDisplaySelectionAnim);
            selectionDisplayAppearAnim.anim.SetAtStartState(selectionDisplayAppearAnim);
            belongingsAnim.anim.SetAtEndState(belongingsAnim);
            belongingsAnim.canvasGroup.blocksRaycasts = false;
            belongingsAnim.canvasGroup.interactable = false;
            isSelected = false;
        }
        else if (isSelected)
        {
            StartCoroutine(illuSelectionAnim.anim.PlayBackward(illuSelectionAnim, true));
            StartCoroutine(permanentDisplaySelectionAnim.anim.PlayBackward(permanentDisplaySelectionAnim, true));
            StartCoroutine(selectionDisplayAppearAnim.anim.PlayBackward(selectionDisplayAppearAnim, true));
            StartCoroutine(belongingsAnim.anim.Play(belongingsAnim));
            belongingsAnim.canvasGroup.blocksRaycasts = false;
            belongingsAnim.canvasGroup.interactable = false;
            isSelected = false;
        }

    }

    public void SetInitialState(float startEnthousiasm)
    {
        currentEnthousiasm = startEnthousiasm;
        foreach (PotentialObject potentialObject in potentialObjects)
        {
            /*if (DoesObjectHasCommonCategory(potentialObject.stallObject.linkedObject, character.initialInterests))
            {
                potentialObject.interestLevel = startInterestLevel;
                potentialObject.curiosityLevel = startCuriosityLevel;
            }*/
            potentialObject.curiosityLevel = Random.Range(minMaxRandomCuriosity.x, minMaxRandomCuriosity.y);
        }
    }

    #endregion

    #region Display

    private void UpdateCharacterInfoDisplay()
    {
        /*if (isSelected)
        {
            if (!selectedFlag)
            {
                selectedFlag = true;
            }
            nameText.gameObject.SetActive(true);
        }
        else
        {
            if (selectedFlag)
            {
                selectedFlag = false;
            }
            nameText.gameObject.SetActive(false);
        }*/

        enthousiasmFiller.fillAmount =  Mathf.Lerp(enthousiasmFiller.fillAmount, currentEnthousiasm / 1, Time.deltaTime * enthousiasmLerpRatio);
    }

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
        illustrationImage.sprite = character.illustration;
        illustrationImage.preserveAspect = true;
        if(gazeDisplay != null)
        {
            gazeDisplay.GetComponent<Image>().color = identificationColor;
        }
        identificationCircle.color = new Color(identificationColor.r, identificationColor.g, identificationColor.b, 0.5f);
        thinkBoxDeco.color = identificationColor;
        speakBoxName.text = character.characterName;
        speakBoxName.color = identificationColor;
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
            newCharaObject.personnalValueMaxRatio = maxPersonnalValue;
            newCharaObject.RefreshDisplay();
            belongings.Add(newCharaObject);
        }

        /*exchangeTreshold = 0;
        for (int i = 0; i < belongings.Count; i++)
        {
            exchangeTreshold += belongings[i].personnalValue;
        }
        exchangeTreshold /= belongings.Count;*/
    }

    public void GetBelongingsFromCharacter()
    {
        CharaObject newCharaObject = null;
        for (int i = 0; i < character.personnalObjects.Count; i++)
        {
            newCharaObject = Instantiate(charaObjectPrefab, belongingsAnim.rectTransform);
            newCharaObject.linkedObject = character.personnalObjects[i].ownedObject;
            newCharaObject.personnalValue = character.personnalObjects[i].value;
            newCharaObject.personnalValueMaxRatio = maxPersonnalValue;
            newCharaObject.RefreshDisplay();
            belongings.Add(newCharaObject);
        }

        /*exchangeTreshold = 0;
        for (int i = 0; i < belongings.Count; i++)
        {
            exchangeTreshold += belongings[i].personnalValue;
        }
        exchangeTreshold /= belongings.Count;*/
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
        public List<KnownFeature> knownFeatures;

        public void RefreshInterestLevel()
        {
            interestLevel = 0;

            for (int i = 0; i < knownFeatures.Count; i++)
            {
                interestLevel += knownFeatures[i].interest;
            }
        }
        public PotentialObject(StallObject _standObject)
        {
            stallObject = _standObject;
            interestLevel = 0;
            curiosityLevel = 0;
            knownFeatures = new List<KnownFeature>();
            for (int i = 0; i < stallObject.linkedObject.features.Count; i++)
            {
                KnownFeature newKnownFeature = new KnownFeature();
                newKnownFeature.feature = stallObject.linkedObject.features[i];
                newKnownFeature.interest = 0;
                newKnownFeature.timeRememberedRmn = 0;
                newKnownFeature.isKnown = false;
                knownFeatures.Add(newKnownFeature);
            }
        }

        public KnownFeature GetKnownFeatureFromFeature(Object.Feature searchedFeature)
        {
            KnownFeature potentialknownFeature = null;
            for (int i = 0; i < knownFeatures.Count; i++)
            {
                if (searchedFeature == knownFeatures[i].feature)
                {
                    potentialknownFeature = knownFeatures[i];
                }
            }
            return potentialknownFeature;
        }

        public class KnownFeature
        {
            /// <summary>
            /// the current interest toward the feature, if the feature is not known it must be set to zero
            /// </summary>
            public float interest;
            public Object.Feature feature;
            public float timeRememberedRmn;
            public bool isKnown;

            public void LearnFeature(float timeToRemember, float _interest)
            {
                interest = _interest;
                isKnown = true;
                timeRememberedRmn = timeToRemember;
            }
            public void ForgetFeature()
            {
                interest = 0;
                isKnown = false;
                timeRememberedRmn = 0;
            }

            public KnownFeature()
            {

            }
        }
    }
}
