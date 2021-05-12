using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UniqueObject", menuName = "Negociant/new Object", order = 1)]
public class Object : ScriptableObject
{
    public string objectName;
    public string title;
    public Sprite illustration;
    [TextArea] public string description;
    [TextArea] public string originDescription;
    public Origin origin;
    public List<Category> categories;
    public List<Feature> features;

    public void SetAutomaticCategoryFeatures()
    {
        Feature newCategoryFeature = null;
        List<Feature> existingFeatures = new List<Feature>();
        existingFeatures.AddRange(features);
        for (int i = 0; i < existingFeatures.Count; i++)
        {
            if (existingFeatures[i].isCategoryFeature)
            {
                existingFeatures.RemoveAt(i);
                i--;
            }
        }
        features.Clear();
        foreach (Category category in categories)
        {
            newCategoryFeature = new Feature();
            newCategoryFeature.categoryProperties = GameData.GetCategoryPropertiesFromCategory(category);
            newCategoryFeature.argumentTitle = "default : " + newCategoryFeature.categoryProperties.category.ToString();
            newCategoryFeature.argumentSpokenText = newCategoryFeature.categoryProperties.argumentDescription;
            newCategoryFeature.argumentSpeakTime = newCategoryFeature.categoryProperties.argumentTime;
            newCategoryFeature.description = "default : " + newCategoryFeature.categoryProperties.category.ToString();
            newCategoryFeature.isKnownWhenObjectAcquired = true;
            newCategoryFeature.interestLevelIncrease = newCategoryFeature.categoryProperties.argumentInterestLevelIncrease;
            newCategoryFeature.isCategoryFeature = true;
            newCategoryFeature.rememberTime = newCategoryFeature.categoryProperties.argumentRememberTime;
            features.Add(newCategoryFeature);
        }
        features.AddRange(existingFeatures);
    }

    public Object()
    {
    }

    [System.Serializable]
    public class Feature
    {
        public List<Trait> traits;
        public string argumentTitle;
        [TextArea] public string argumentSpokenText;
        public float argumentSpeakTime;
        [TextArea] public string description;
        public bool isKnownWhenObjectAcquired;
        public float interestLevelIncrease;
        public bool isCategoryFeature;
        public GameData.CategoryProperties categoryProperties;
        public float rememberTime;

        public Feature()
        {
        }
    }
}


public enum Trait { SandWorm, Corruption, Shiny, AuroraMagic, DuskMagic, Noble, Sharp, WeaknessGiving, StrenghGiving, Lost, Glorious, RareMaterial, Light, Heavy, Cold, Hot, Heskmar};
public enum Category { Armor, Weapon, Accessory, Relic, Knowledge, Magic};
public enum Origin { Unknown, DuhuaDesert, SandCanyon, Hagdon, TioroCliffs, EnchantedWood, FrozenRuinedTower};

