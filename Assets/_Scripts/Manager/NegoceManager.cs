using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NegoceManager : MonoBehaviour
{
    public List<Character> allPossibleCharacters;
    public PlayerHandler playerHandler;
    public ArgumentRadialMenu argumentRadialMenu;
    public RectTransform charactersDisplay;
    public Vector2 minCharacterPos;
    public Vector2 maxCharacterPos;
    public int maxCharacterPresent;
    public TweeningAnimator stallOpeningAnim;
    public SimpleButton openStallButton;
    public SimpleButton nextDayButton;
    public TweeningAnimator blackScreenAnim;
    public bool isTuto;
    public GameObject anneauVictoryText;
    public GameObject dagueVictoryText;
    public GameObject defeatText;
    [Header("CharacterInfoRef")]
    public bool debugInfo;
    public GameObject charaInfoPanel;
    public Text charaInitialInterestsText;
    public Text charaNameText;
    [Header("Characters options")]
    public RectTransform gazePrefab;
    public RectTransform gazePanel;
    public List<Color> identificationColors;
    public Vector2 timeRangeBetweenCharacterApparition;
    public float characterMoveLerpRatio;
    public int additionnalSpaceTakenBySelectedCharacter;
    public int additionnalSpaceTakenBySelectedCharacterWhileTrading;
    public float selectedCharacterPositionRatioOffsetWhileTrading;
    public TweeningAnimator notesWindowAnim;
    public InputField notesField;
    public Dropdown categoryDropdown;
    public Text notesWindowCharacterText;
    public Image notesWindowCharacterFace;
    public SimpleButton askToLeaveButton;
    [Header("Market Day Options")]
    public float marketDayTime;
    public RectTransform sundialArrow;
    public Vector2 minMaxSundialArrowAngle;
    [Header("RandomCharacterOptions")]
    public int minChInitialInterest;
    public int maxChInitialInterest;
    public int minChNeeds;
    public int maxChNeeds;
    public List<Sprite> allCharacterIllustrations;
    public List<Sprite> allCharacterFaces;
    public int randomCharacterGenerated;

    [HideInInspector] public List<CharacterHandler> allPresentCharacters;
    [HideInInspector] public bool unfoldTime;
    [HideInInspector] public float negoceTimeSpend;
    [HideInInspector] public CharacterHandler selectedCharacter;
    [HideInInspector] public CharaObject draggedCharaObject;
    private float timeSpendSinceLastCharacterApparition;
    private float nextCharacterApparitionTime;
    private CharacterHandler previousSelectedCharacter;
    private List<Character> characterAlreadyCame;
    private bool charaInfoWindowFlag;

    public static NegoceManager I;
    private void Awake()
    {
        I = this;
    }

    void Start()
    {
        blackScreenAnim.GetReferences();
        StartCoroutine(blackScreenAnim.anim.PlayBackward(blackScreenAnim, true));

        characterAlreadyCame = new List<Character>();
        unfoldTime = false;
        negoceTimeSpend = 0;
        allPresentCharacters = new List<CharacterHandler>();
        if(randomCharacterGenerated > 0)
        {
            for (int i = 0; i < randomCharacterGenerated; i++)
            {
                allPossibleCharacters.Add(GetRandomGeneratedCharacter());
            }
        }
        else
        {

        }

        nextCharacterApparitionTime = UnityEngine.Random.Range(5, 8);
        for (int i = 0; i < playerHandler.allStallObjects.Count; i++)
        {
            playerHandler.allStallObjects[i].interestLevelToShow = -1;
        }
        notesWindowAnim.GetReferences();
        notesWindowAnim.anim.SetAtEndState(notesWindowAnim);
        askToLeaveButton.SetEnable(true);
        stallOpeningAnim.GetReferences();
        stallOpeningAnim.anim.SetAtStartState(stallOpeningAnim);
        nextDayButton.SetEnable(false);
        nextDayButton.GetComponent<CanvasGroup>().alpha = 0;
    }

    void Update()
    {
        if(negoceTimeSpend < marketDayTime)
        {
            if(unfoldTime)
            {
                negoceTimeSpend += Time.deltaTime;
                sundialArrow.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(minMaxSundialArrowAngle.x, minMaxSundialArrowAngle.y, negoceTimeSpend / marketDayTime));
            }
        }
        else if(unfoldTime)
        {
            unfoldTime = false;
            Invoke("CloseShop", 5);
            foreach (CharacterHandler characterPresent in allPresentCharacters)
            {
                characterPresent.Interrupt();
                characterPresent.isThinking = false;
                StartCoroutine(characterPresent.Leave());
            }
        }

        UpdateCharacterApparition();

        UpdateSelectedCharaInfo();

        if(selectedCharacter == null)
        {
            draggedCharaObject = null;
        }

        if(openStallButton.isClicked & openStallButton.canBeUsed)
        {
            OpenShop();
        }

        if (nextDayButton.isClicked & nextDayButton.canBeUsed)
        {
            nextDayButton.SetEnable(false);
            if(isTuto || SaveLoader.I.playerSave.passedDays >= 2)
            {
                GoToMainMenu();
            }
            else
            {
                StartCoroutine(LoadSceneWithTransition(SceneManager.GetActiveScene().buildIndex));
            }
        }
    }

    private void UpdateCharacterApparition()
    {
        if(allPresentCharacters.Count < maxCharacterPresent && unfoldTime)
        {
            if(timeSpendSinceLastCharacterApparition < nextCharacterApparitionTime)
            {
                timeSpendSinceLastCharacterApparition += Time.deltaTime;
            }
            else
            {
                timeSpendSinceLastCharacterApparition = 0;
                nextCharacterApparitionTime = UnityEngine.Random.Range(timeRangeBetweenCharacterApparition.x, timeRangeBetweenCharacterApparition.y);
                Character toAppearCharacter = GetRandomAbsentCharacter();
                if(toAppearCharacter != null)
                {
                    AppearCharacter(toAppearCharacter);
                }
                else
                {
                    //Debug.LogWarning("Can't appear character because he is already present at the stall");
                }
            }
        }
    }

    private void UpdateSelectedCharaInfo()
    {
        if (debugInfo)
        {
            charaInfoPanel.SetActive(true);
        }
        else
        {
            charaInfoPanel.SetActive(false);
        }
        if (selectedCharacter != null && previousSelectedCharacter != selectedCharacter && !selectedCharacter.isLeaving)
        {
            previousSelectedCharacter = selectedCharacter;

            charaInitialInterestsText.text = selectedCharacter.character.initialInterests[0].ToString()
                + " / " + (selectedCharacter.character.initialInterests.Count > 1 ? selectedCharacter.character.initialInterests[1].ToString() : "")
                + " / " + (selectedCharacter.character.initialInterests.Count > 2 ? selectedCharacter.character.initialInterests[2].ToString() : "");
            charaNameText.text = selectedCharacter.character.characterName;
            foreach (StallObject standObject in playerHandler.allStallObjects)
            {
                foreach (CharacterHandler.PotentialObject potentialObject in selectedCharacter.potentialObjects)
                {
                    if (standObject == potentialObject.stallObject)
                    {
                        standObject.interestLevel.text = Mathf.RoundToInt(potentialObject.interestLevel).ToString();
                        standObject.SetInterestLevelDisplay(potentialObject.interestLevel / selectedCharacter.maxPersonnalValue, selectedCharacter.identificationColor, true);
                    }
                }

                standObject.interestLevel.gameObject.SetActive(debugInfo);
            }

            charaInfoWindowFlag = true;
            notesWindowCharacterFace.sprite = selectedCharacter.character.faceSprite;
            notesWindowCharacterText.text = selectedCharacter.character.characterName;
            notesField.text = selectedCharacter.playerNotes;
            categoryDropdown.value = selectedCharacter.playerCategoryNote;
            StartCoroutine(notesWindowAnim.anim.PlayBackward(notesWindowAnim, true));
        }
        if ((selectedCharacter == null || selectedCharacter.isLeaving) && charaInfoWindowFlag)
        {
            charaInfoWindowFlag = false;
            StartCoroutine(notesWindowAnim.anim.Play(notesWindowAnim));
        }

        if(selectedCharacter == null)
        {
            charaInfoPanel.SetActive(false);
        }

        RefreshCharactersDisplay();

        foreach (CharacterHandler characterPresent in allPresentCharacters)
        {
            characterPresent.rectTransform.anchoredPosition = Vector2.Lerp(characterPresent.rectTransform.anchoredPosition, characterPresent.targetPositionAtTheStall, characterMoveLerpRatio * Time.deltaTime);
        }

        if (askToLeaveButton.isClicked && !selectedCharacter.isListening && !selectedCharacter.isThinking && !playerHandler.IsPlayerTalking())
        {
            selectedCharacter.Interrupt();
            playerHandler.Speak("Reviens un autre jour !", 1.5f);
            StartCoroutine(selectedCharacter.Leave());
        }
    }

    public void ChangeSelectedCharacterNotes(string playerNotes)
    {
        if(selectedCharacter != null)
        {
            selectedCharacter.playerNotes = playerNotes;
        }
    }
    public void ChangeSelectedCharacterCategoryNote(int playerNotes)
    {
        if (selectedCharacter != null)
        {
            selectedCharacter.playerCategoryNote = playerNotes;
        }
    }

    public void CloseShop()
    {
        StartCoroutine(stallOpeningAnim.anim.PlayBackward(stallOpeningAnim, true));
        unfoldTime = false;
        nextDayButton.SetEnable(true);
        nextDayButton.GetComponent<CanvasGroup>().alpha = 1;

        SaveLoader.I.playerSave.passedDays++;

        if (SaveLoader.I.playerSave.passedDays == 2 && !isTuto)
        {
            if (SaveLoader.I.playerSave.playerOwnedObjects.Contains("Anneau Saparkus"))
            {
                anneauVictoryText.SetActive(true);
            }

            if (SaveLoader.I.playerSave.playerOwnedObjects.Contains("Poignard d'Emerith"))
            {
                dagueVictoryText.SetActive(true);
            }

            if (!SaveLoader.I.playerSave.playerOwnedObjects.Contains("Anneau Saparkus") && !SaveLoader.I.playerSave.playerOwnedObjects.Contains("Poignard d'Emerith"))
            {
                defeatText.SetActive(true);
            }
        }
    }

    public void OpenShop()
    {
        StartCoroutine(stallOpeningAnim.anim.Play(stallOpeningAnim));
        playerHandler.Speak(playerHandler.welcomeSpeech, 5);
        unfoldTime = true;
        openStallButton.SetEnable(false);
        openStallButton.GetComponent<CanvasGroup>().alpha = 0;
    }


    private Character GetRandomAbsentCharacter()
    {
        Character chosenChara = null;

        List<Character> allAbsentCharacter = new List<Character>();
        allAbsentCharacter.AddRange(allPossibleCharacters);
        for (int i = 0; i < allPresentCharacters.Count; i++)
        {
            allAbsentCharacter.Remove(allPresentCharacters[i].character);
        }
        for (int i = (allAbsentCharacter.Count - 1); i >= 0; i--)
        {
            if(characterAlreadyCame.Contains(allAbsentCharacter[i]))
            {
                allAbsentCharacter.RemoveAt(i);
            }
            else if(!(allAbsentCharacter[i].dayRangeComing.x < negoceTimeSpend / marketDayTime && allAbsentCharacter[i].dayRangeComing.y > negoceTimeSpend / marketDayTime))
            {
                allAbsentCharacter.RemoveAt(i);
            }
            else if(SaveLoader.I.GetCharacterInfoFromCharacter(allAbsentCharacter[i]) != null && SaveLoader.I.GetCharacterInfoFromCharacter(allAbsentCharacter[i]).ownedObjects.Count == 0)
            {
                allAbsentCharacter.RemoveAt(i);
            }
        }

        if (allAbsentCharacter.Count > 0)
        {
            chosenChara = allAbsentCharacter[UnityEngine.Random.Range(0, allAbsentCharacter.Count)];
        }

        return chosenChara;
    }

    private void AppearCharacter(Character theCharacter)
    {
        CharacterHandler newCharacterHandler;
        newCharacterHandler = Instantiate(GameData.GetCharacterHandlerPrefabFromTemper(theCharacter.temper), charactersDisplay);
        allPresentCharacters.Add(newCharacterHandler);
        newCharacterHandler.character = theCharacter;
        if (allPresentCharacters.Count > maxCharacterPresent)
        {
            //MakeCharacterLeave(allPresentCharacters[0]);
            Debug.LogWarning("There is too many character at the stall");
        }

        newCharacterHandler.RefreshPotentialObjects();
        newCharacterHandler.gazeDisplay = Instantiate(gazePrefab, gazePanel);
        newCharacterHandler.nameText.text = newCharacterHandler.character.characterName;
        newCharacterHandler.gameObject.name = newCharacterHandler.character.characterName;
        newCharacterHandler.gazeDisplay.gameObject.name = newCharacterHandler.character.characterName + "'s gaze";
        newCharacterHandler.characterCanvasRectTransform = charactersDisplay;
        newCharacterHandler.Init();
        if(newCharacterHandler.character.randomlyGenerated)
        {
            newCharacterHandler.SetRandomListOfCharaObject();
        }
        else
        {
            newCharacterHandler.GetBelongingsFromCharacter();
            newCharacterHandler.GetSavedNotes();
        }

        RefreshCharactersDisplay();
        characterAlreadyCame.Add(theCharacter);
        StartCoroutine(newCharacterHandler.Appear());
    }

    private void RefreshCharactersDisplay()
    {
        float passedSelectedCharacter = 0;
        int modifiedCount = allPresentCharacters.Count * 20 + (selectedCharacter != null ? (selectedCharacter.characterExchangeHandler.isOpened ? additionnalSpaceTakenBySelectedCharacterWhileTrading : additionnalSpaceTakenBySelectedCharacter) : 0);
        for (int i = 0; i < allPresentCharacters.Count; i++)
        {
            if(!allPresentCharacters[i].isLeaving)
            {
                if(allPresentCharacters[i].isSelected)
                {
                    passedSelectedCharacter = allPresentCharacters[i].characterExchangeHandler.isOpened ? additionnalSpaceTakenBySelectedCharacterWhileTrading : additionnalSpaceTakenBySelectedCharacter;
                }

                allPresentCharacters[i].targetPositionAtTheStall = Vector2.Lerp(Vector2.Lerp(minCharacterPos, maxCharacterPos, (i * 20 + (allPresentCharacters[i].isSelected ? 0 : passedSelectedCharacter)) / modifiedCount),
                    Vector2.Lerp(minCharacterPos, maxCharacterPos, ((i + 1) * 20 + passedSelectedCharacter) / modifiedCount), allPresentCharacters[i].isSelected && allPresentCharacters[i].characterExchangeHandler.isOpened ? selectedCharacterPositionRatioOffsetWhileTrading : 0.5f);

                if(allPresentCharacters[i].rectTransform.anchoredPosition == Vector2.zero)
                {
                    allPresentCharacters[i].rectTransform.anchoredPosition = allPresentCharacters[i].targetPositionAtTheStall;
                }
                allPresentCharacters[i].identificationColor = identificationColors[i];
                allPresentCharacters[i].RefreshCharacterDisplay();
                
            }
        }
    }

    private Character GetRandomGeneratedCharacter()
    {
        Character newCharacter = ScriptableObject.CreateInstance<Character>();
        List<Category> characterInitialPreferences = new List<Category>();
        for (int i = 0; i < UnityEngine.Random.Range(minChInitialInterest, maxChInitialInterest); i++)
        {
            characterInitialPreferences.Add((Category)Enum.ToObject(typeof(Category), UnityEngine.Random.Range(0, Enum.GetValues(typeof(Category)).Length)));
        }
        newCharacter.initialInterests = new List<Category>(characterInitialPreferences);

        //newCharacter.temper = (Temper)Enum.ToObject(typeof(Temper), UnityEngine.Random.Range(0, Enum.GetValues(typeof(Temper)).Length));
        newCharacter.temper = Temper.Decisive;
        newCharacter.needs = new List<Character.Need>();
        for (int i = 0; i < UnityEngine.Random.Range(minChNeeds, maxChNeeds); i++)
        {
            newCharacter.needs.Add(new Character.Need((Trait)Enum.ToObject(typeof(Trait), UnityEngine.Random.Range(0, Enum.GetValues(typeof(Trait)).Length)), true));
        }
        System.Random rnd = new System.Random(UnityEngine.Random.Range(2000, 3000));
        newCharacter.name = "";
        for (int i = 0; i < 8; i++)
        {
            char randomChar = (char)rnd.Next('a', 'z');
            newCharacter.characterName += randomChar;
        }
        char firstLetter = char.ToUpper(newCharacter.characterName[0]);
        newCharacter.characterName = firstLetter + newCharacter.characterName.Remove(0, 1);
        int rand = UnityEngine.Random.Range(0, allCharacterIllustrations.Count);
        newCharacter.illustration = allCharacterIllustrations[rand];
        newCharacter.faceSprite = allCharacterFaces[rand];
        newCharacter.name = newCharacter.characterName;

        newCharacter.personnalObjects = new List<Character.PersonnalObject>();
        newCharacter.randomlyGenerated = true;

        newCharacter.speechBaseSpeed = 30;
        newCharacter.speechPauseTimeBetweenSentenceParts = 0.3f;
        newCharacter.speechPauseTimeBetweenSentences = 3f;

        newCharacter.defaultSpeachWhenWrongArgument = new Speech("Peu importe");

        return newCharacter;
    }

    public void LeaveCharacterPresent(CharacterHandler leavingCharacter)
    {
        allPresentCharacters.Remove(leavingCharacter);
        Destroy(leavingCharacter.gazeDisplay.gameObject);
        Destroy(leavingCharacter.gameObject);
        RefreshCharactersDisplay();
    }

    public void SelectCharacter(CharacterHandler theCharacter)
    {
        selectedCharacter = theCharacter;
        selectedCharacter.Select();
        foreach (CharacterHandler character in NegoceManager.I.allPresentCharacters)
        {
            if (character != selectedCharacter)
            {
                character.UnSelect(false);
            }
        }
    }

    public IEnumerator LoadSceneWithTransition(int index)
    {
        StartCoroutine(blackScreenAnim.anim.Play(blackScreenAnim));
        yield return new WaitForSeconds(blackScreenAnim.anim.animationTime);
        SceneManager.LoadScene(index);
    }

    public void GoToMainMenu()
    {
        StartCoroutine(LoadSceneWithTransition(0));
    }

    public static Vector2 GetDirectionFromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}
