using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Negociant/Create new unique character", order = 2)]
public class Character : ScriptableObject
{
    [System.Serializable]
    public class Need
    {
        public Trait trait;
        [Range(0f, 1f)] public float importance;

        public Need(Trait needTrait, float needImportance)
        {
            trait = needTrait;
            importance = needImportance;
        }
    }

    public string characterName;
    public Sprite illustration;
    public Temper temper;
    public List<Need> needs;
    public List<Category> initialInterest;
}

public enum Temper { Patient, Stressed};
