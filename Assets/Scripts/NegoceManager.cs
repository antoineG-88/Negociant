using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NegoceManager : MonoBehaviour
{
    public List<Character> allPossibleCharacters;
    public PlayerHandler playerHandler;
    public CharacterBehavior characterBehaviorPrefab;
    public RectTransform charactersDisplay;
    public Vector2 minCharacterPos;
    public Vector2 maxCharacterPos;
    public int maxCharacterPresent;
    [Header("RandomCharacterOptions")]
    public int minChInitialInterest;
    public int maxChInitialInterest;
    public int minChNeeds;
    public int maxChNeeds;
    public List<Sprite> allCharacterIllustrations;

    [HideInInspector] public List<CharacterBehavior> allPresentCharacters;
    [HideInInspector] public bool unfoldTime;
    [HideInInspector] public float negoceTimeSpend;

    public static NegoceManager I;
    private void Awake()
    {
        I = this;
    }

    void Start()
    {
        unfoldTime = true;
        negoceTimeSpend = 0;
        allPresentCharacters = new List<CharacterBehavior>();
    }

    void Update()
    {
        if(unfoldTime)
        {
            negoceTimeSpend += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.S))
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
                    if(allPresentCharacters[i].character == chosenChara)
                    {
                        charaAlreadyPresent = true;
                    }
                }

            } while (charaAlreadyPresent && iteration < 200);

            AppearCharacter(chosenChara);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            allPossibleCharacters.Add(GenerateRandomCharacter());
        }
    }

    private void AppearCharacter(Character theCharacter)
    {
        CharacterBehavior newCharacter;
        newCharacter = Instantiate(characterBehaviorPrefab, charactersDisplay);
        allPresentCharacters.Add(newCharacter);
        newCharacter.character = theCharacter;
        if(allPresentCharacters.Count > maxCharacterPresent)
        {
            Destroy(allPresentCharacters[0].gameObject);
            allPresentCharacters.RemoveAt(0);
        }
        newCharacter.UnSelect();
        RefreshCharactersDisplay();
    }

    private void RefreshCharactersDisplay()
    {
        for (int i = 0; i < allPresentCharacters.Count; i++)
        {
            allPresentCharacters[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(Vector2.Lerp(minCharacterPos, maxCharacterPos, (float)i / allPresentCharacters.Count),
                Vector2.Lerp(minCharacterPos, maxCharacterPos, (float)(i + 1) / allPresentCharacters.Count), 0.5f);
            allPresentCharacters[i].RefreshCharacterDisplay();
        }
    }

    private Character GenerateRandomCharacter()
    {
        Character newCharacter = ScriptableObject.CreateInstance<Character>();
        List<Category> characterInitialPreferences = new List<Category>();
        for (int i = 0; i < UnityEngine.Random.Range(minChInitialInterest, maxChInitialInterest); i++)
        {
            characterInitialPreferences.Add((Category)Enum.ToObject(typeof(Category), UnityEngine.Random.Range(0, Enum.GetValues(typeof(Category)).Length)));
        }
        newCharacter.initialInterest = new List<Category>(characterInitialPreferences);

        newCharacter.temper = (Temper)Enum.ToObject(typeof(Temper), UnityEngine.Random.Range(0, Enum.GetValues(typeof(Temper)).Length));

        List<Character.Need> characterNeeds = new List<Character.Need>();
        for (int i = 0; i < UnityEngine.Random.Range(minChNeeds, maxChNeeds); i++)
        {
            characterNeeds.Add(new Character.Need((Trait)Enum.ToObject(typeof(Trait), UnityEngine.Random.Range(0, Enum.GetValues(typeof(Trait)).Length)), UnityEngine.Random.Range(0f, 1f)));
        }

        System.Random rnd = new System.Random();
        newCharacter.name = "";
        for (int i = 0; i < 8; i++)
        {
            char randomChar = (char)rnd.Next('a', 'z');
            newCharacter.characterName += randomChar;
        }
        char firstLetter = char.ToUpper(newCharacter.characterName[0]);
        newCharacter.characterName = firstLetter + newCharacter.characterName.Remove(0, 1);
        newCharacter.illustration = allCharacterIllustrations[UnityEngine.Random.Range(0, allCharacterIllustrations.Count)];
        Debug.Log(newCharacter.characterName + " added to possibleCharacters");
        return newCharacter;
    }
}
