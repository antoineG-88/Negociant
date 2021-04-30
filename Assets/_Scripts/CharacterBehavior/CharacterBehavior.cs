using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBehavior : ScriptableObject
{
    public Temper temper;
    [Space]
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

    public abstract void Init(CharacterHandler characterHandler, ref List<CharacterHandler.PotentialObject> potentialObjects);

    public abstract void UpdateBehavior(CharacterHandler characterHandler, ref List<CharacterHandler.PotentialObject> potentialObjects);
}
