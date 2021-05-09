using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Negociant/new Character", order = 2)]
public class Character : ScriptableObject
{
    public string characterName;
    public Sprite illustration;
    public Sprite faceSprite;
    public Temper temper;
    public List<Category> initialInterests;
    public List<Need> needs;
    public List<PersonnalObject> personnalObjects;
    public Speech defaultSpeachWhenWrongArgument;
    public float speechBaseSpeed;
    public float speechPauseTimeBetweenSentences;
    public float speechPauseTimeBetweenSentenceParts;
    public bool randomlyGenerated;

    public PersonnalObject GetPersonnalObjectFromObject(Object searchedObject)
    {
        PersonnalObject potentialPersonnalObject = null;
        foreach(PersonnalObject personnalObject in personnalObjects)
        {
            if(personnalObject.ownedObject == searchedObject)
            {
                potentialPersonnalObject = personnalObject;
            }
        }
        return potentialPersonnalObject;
    }

    [System.Serializable]
    public class Need
    {
        public Trait trait;
        [HideInInspector] public string defaultHintToTell;
        public Speech reactionSpokenWhenArgumented;

        public Need(Trait needTrait, bool defaultResponse)
        {
            trait = needTrait;
            if(defaultResponse)
            {
                defaultHintToTell = "hum ... j'aime le " + trait.ToString();
                reactionSpokenWhenArgumented = new Speech("Parfait !_ J'aime le " + trait.ToString());
            }
        }
    }

    [System.Serializable]
    public class PersonnalObject
    {
        public Object ownedObject;
        public Speech infoGivenWhenAsked;
        public List<Trait> traitHintedWithInfo;
        public float value;
    }
}

public enum Temper {Decisive, OpenMinded};
