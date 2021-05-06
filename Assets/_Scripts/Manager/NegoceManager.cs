using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class NegoceManager : MonoBehaviour
{
    public List<Character> allPossibleCharacters;
    public PlayerHandler playerHandler;
    public ExchangeHandler exchangeHandler;
    public RectTransform charactersDisplay;
    public Vector2 minCharacterPos;
    public Vector2 maxCharacterPos;
    public int maxCharacterPresent;
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
    public int randomCharacterGenerated;
    [Header("RandomCharacterOptions")]
    public int minChInitialInterest;
    public int maxChInitialInterest;
    public int minChNeeds;
    public int maxChNeeds;
    public List<Sprite> allCharacterIllustrations;
    public List<Sprite> allCharacterFaces;

    [HideInInspector] public List<CharacterHandler> allPresentCharacters;
    [HideInInspector] public bool unfoldTime;
    [HideInInspector] public float negoceTimeSpend;
    [HideInInspector] public CharacterHandler selectedCharacter;
    [HideInInspector] public CharaObject draggedCharaObject;
    private float timeSpendSinceLastCharacterApparition;
    private float nextCharacterApparitionTime;
    private CharacterHandler previousSelectedCharacter;

    public static NegoceManager I;
    private void Awake()
    {
        I = this;
    }

    void Start()
    {
        unfoldTime = true;
        negoceTimeSpend = 0;
        allPresentCharacters = new List<CharacterHandler>();
        for (int i = 0; i < randomCharacterGenerated; i++)
        {
            allPossibleCharacters.Add(GetRandomGeneratedCharacter());
        }
        nextCharacterApparitionTime = UnityEngine.Random.Range(1, 2);
        for (int i = 0; i < playerHandler.allStallObjects.Count; i++)
        {
            playerHandler.allStallObjects[i].interestLevelToShow = -1;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            debugInfo = !debugInfo;
        }

        if(unfoldTime)
        {
            negoceTimeSpend += Time.deltaTime;
        }

        UpdateCharacterApparition();

        UpdateSelectedCharaInfo();

        if(selectedCharacter == null)
        {
            draggedCharaObject = null;
        }
    }

    private void UpdateCharacterApparition()
    {
        if(allPresentCharacters.Count < maxCharacterPresent)
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
                    Debug.LogWarning("Can't appear character because he is already present at the stall");
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
        if (selectedCharacter != null && previousSelectedCharacter != selectedCharacter)
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
                        standObject.SetInterestLevelDisplay(potentialObject.knownInterestLevel / selectedCharacter.exchangeTreshold, selectedCharacter.identificationColor);
                    }
                }

                standObject.interestLevel.gameObject.SetActive(debugInfo);
            }
        }
        if(selectedCharacter == null)
        {
            charaInfoPanel.SetActive(false);
        }
    }

    private Character GetRandomAbsentCharacter()
    {
        Character chosenChara;
        bool charaAlreadyPresent = false;
        int iteration = 0;
        do
        {
            charaAlreadyPresent = false;
            iteration++;
            chosenChara = allPossibleCharacters[UnityEngine.Random.Range(0, allPossibleCharacters.Count)];

            for (int i = 0; i < allPresentCharacters.Count; i++)
            {
                if (allPresentCharacters[i].character == chosenChara)
                {
                    charaAlreadyPresent = true;
                }
            }

        } while (charaAlreadyPresent && iteration < 200);

        return charaAlreadyPresent ? null : chosenChara;
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
        newCharacterHandler.SetRandomListOfCharaObject();

        RefreshCharactersDisplay();
        StartCoroutine(newCharacterHandler.Appear());
    }

    private void RefreshCharactersDisplay()
    {
        for (int i = 0; i < allPresentCharacters.Count; i++)
        {
            if(!allPresentCharacters[i].isLeaving)
            {
                allPresentCharacters[i].rectTransform.anchoredPosition = Vector2.Lerp(Vector2.Lerp(minCharacterPos, maxCharacterPos, (float)i / allPresentCharacters.Count),
                    Vector2.Lerp(minCharacterPos, maxCharacterPos, (float)(i + 1) / allPresentCharacters.Count), 0.5f);
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
        List<Character.Need> characterNeeds = new List<Character.Need>();
        for (int i = 0; i < UnityEngine.Random.Range(minChNeeds, maxChNeeds); i++)
        {
            characterNeeds.Add(new Character.Need((Trait)Enum.ToObject(typeof(Trait), UnityEngine.Random.Range(0, Enum.GetValues(typeof(Trait)).Length)), UnityEngine.Random.Range(0f, 1f)));
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
        //Debug.Log(newCharacter.characterName + " added to possibleCharacters");
        return newCharacter;
    }

    public void MakeCharacterLeave(CharacterHandler leavingCharacter)
    {
        leavingCharacter.isLeaving = true;
        allPresentCharacters.Remove(leavingCharacter);
        Destroy(leavingCharacter.gazeDisplay.gameObject);
        Destroy(leavingCharacter.gameObject);
        RefreshCharactersDisplay();
    }

    public void SelectCharacter(CharacterHandler theCharacter)
    {
        selectedCharacter = theCharacter;
        selectedCharacter.Select();
        exchangeHandler.Close();
        foreach (CharacterHandler character in NegoceManager.I.allPresentCharacters)
        {
            if (character != selectedCharacter)
            {
                character.UnSelect(false);
            }
        }
    }

    public static Vector2 GetDirectionFromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}
