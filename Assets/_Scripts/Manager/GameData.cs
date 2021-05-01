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

    public static List<Object> allObjects;
    public static List<CategoryProperties> categoriesProperties;
    public static DecisiveCharacterHandler decisiveCharaPrefab;
    public static OpenMindedCharacterHandler openMindedCharaPrefab;

    private void Awake()
    {
        allObjects = _allObjects;
        categoriesProperties = _categoriesProperties;
        decisiveCharaPrefab = _decisiveCharaPrefab;
        openMindedCharaPrefab = _openMindedCharaPrefab;
    }

    [Serializable]
    public class CategoryProperties
    {
        public Category category;
        public Color color;
        public Sprite icon;
        public float argumentTime;
        public float argumentInterestLevelIncrease; 
        [TextArea] public string argumentDescription;
    }
    public static CategoryProperties GetCategoryPropertiesFromCategory(Category searchedCategory)
    {
        CategoryProperties theCategoryProperties = null;
        for (int i = 0; i < categoriesProperties.Count; i++)
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
