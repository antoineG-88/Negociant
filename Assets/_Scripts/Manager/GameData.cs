using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameData : MonoBehaviour
{
    public List<Object> _allObjects;
    public List<CategoryProperties> _categoriesProperties;
    public List<CharacterBehavior> _allTempersBehavior;

    public static List<Object> allObjects;
    public static List<CategoryProperties> categoriesProperties;
    public static List<CharacterBehavior> allTempersBehavior;

    private void Awake()
    {
        allObjects = _allObjects;
        categoriesProperties = _categoriesProperties;
        allTempersBehavior = _allTempersBehavior;
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

    public static CharacterBehavior GetBehaviorFromTemper(Temper searchedTemper)
    {
        CharacterBehavior theBehavior = null;
        for (int i = 0; i < allTempersBehavior.Count; i++)
        {
            if (allTempersBehavior[i].temper == searchedTemper)
            {
                theBehavior = allTempersBehavior[i];
            }
        }
        return theBehavior;
    }
}
