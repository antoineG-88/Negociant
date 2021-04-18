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
    }

    public Temper temper;
    public List<Need> needs;
}

public enum Temper { Patient, Stressed};
