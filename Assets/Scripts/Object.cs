using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UniqueObject", menuName = "Negociant/Create new unique object", order = 1)]
public class Object : ScriptableObject
{

    [System.Serializable]
    public class Feature
    {
        public string argumentDescription;
        public float argumentTime;
        public List<Trait> featureTraits;
        [TextArea] public string featureDescription;
        public bool isKnownWhenObjectAcquired;
        [Range(0f, 1f)] public float visibility;
        public Feature()
        {
        }
    }

    public string objectName;
    public string title;
    public Sprite illustration;
    [TextArea] public string description;
    [TextArea] public string originDescription;
    public Origin origin;
    public List<Category> categories;
    public List<Feature> features;

    public Object()
    {
    }
}


public enum Trait { ColdResistant, HeatResistant, Shiny, Heavy, Light, Mysterious, Magic, SkeletonProof, SandWormProof, Clean, Old, NobleOrigins, Sharp, GoodMaterial, VigorUp};
public enum Category { Armor, Weapon, Accessory, Relic, Ressource };
public enum Origin { Unknown, DuhuaDesert, SandCanyon, Hagdon, TioroCliffs, EnchantedWood, FrozenRuinedTower};

