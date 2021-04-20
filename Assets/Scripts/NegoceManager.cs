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
    [Header("RandomCharacterOptions")]
    public int minChInitialInterest;
    public int maxChInitialInterest;
    public int minChNeeds;
    public int maxChNeeds;
    public List<Sprite> allCharacterIllustrations;

    private List<CharacterBehavior> allPresentCharacters;
    [HideInInspector] public bool unfoldTime;
    [HideInInspector] public float negoceTimeSpend;

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
            AppearCharacter(allPossibleCharacters[UnityEngine.Random.Range(0, allPossibleCharacters.Count)]);
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
        newCharacter.RefreshCharacterDisplay();
        RefreshCharacterPos();
    }

    private void RefreshCharacterPos()
    {
        for (int i = 0; i < allPresentCharacters.Count; i++)
        {
            allPresentCharacters[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(Vector2.Lerp(minCharacterPos, maxCharacterPos, (float)i / allPresentCharacters.Count),
                Vector2.Lerp(minCharacterPos, maxCharacterPos, (float)(i + 1) / allPresentCharacters.Count), 0.5f);
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
