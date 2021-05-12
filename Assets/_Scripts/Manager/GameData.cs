using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameData : MonoBehaviour
{
    public List<Object> _allObjects;
    public List<CategoryProperties> _categoriesProperties;
    public DecisiveCharacterHandler _decisiveCharaPrefab;
    public OpenMindedCharacterHandler _openMindedCharaPrefab;
    [Header("Random Feature option / Temporary")]
    public Vector2Int _minMaxArgumentTime;
    public Vector2Int _minMaxRememberTime;
    public Vector2Int _minMaxNumberOfTraits;
    public Vector2Int _minMaxInterestIncrease;

    public static List<Object> allObjects;
    public static List<CategoryProperties> categoriesProperties;
    public static DecisiveCharacterHandler decisiveCharaPrefab;
    public static OpenMindedCharacterHandler openMindedCharaPrefab;
    public static Vector2Int minMaxArgumentTime;
    public static Vector2Int minMaxRememberTime;
    public static Vector2Int minMaxNumberOfTraits;
    public static Vector2Int minMaxInterestIncrease;

    private void Awake()
    {
        allObjects = _allObjects;
        categoriesProperties = _categoriesProperties;
        decisiveCharaPrefab = _decisiveCharaPrefab;
        openMindedCharaPrefab = _openMindedCharaPrefab;
        minMaxArgumentTime = _minMaxArgumentTime;
        minMaxRememberTime = _minMaxRememberTime;
        minMaxNumberOfTraits = _minMaxNumberOfTraits;
        minMaxInterestIncrease = _minMaxInterestIncrease;
    }

    [Serializable]
    public class CategoryProperties
    {
        public Category category;
        public Color color;
        public Sprite icon;
        public float argumentTime;
        public float argumentInterestLevelIncrease;
        public float argumentRememberTime;
        [TextArea] public string argumentDescription;
        [TextArea] public string argumentSpeechGoodReaction;
        [TextArea] public string argumentSpeechBadReaction;
    }
    public static CategoryProperties GetCategoryPropertiesFromCategory(Category searchedCategory)
    {
        CategoryProperties theCategoryProperties = null;
        for (int i =0; i < categoriesProperties.Count; i++)
        {
            if (categoriesProperties[i].category == searchedCategory)
            {
                theCategoryProperties = categoriesProperties[i];
            }
        }
        return theCategoryProperties;
    }

    public static CharacterHandler GetCharacterHandlerPrefabFromTemper(Temper searchedTemper)
    {
        CharacterHandler theCharacterPrefab = null;
        switch (searchedTemper)
        {
            case Temper.Decisive:
                theCharacterPrefab = decisiveCharaPrefab;
                break;

            case Temper.OpenMinded:
                theCharacterPrefab = openMindedCharaPrefab;
                break;
        }
        return theCharacterPrefab;
    }
}
