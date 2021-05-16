using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInventory
{
    public List<Belonging> belongings;

    [System.Serializable]
    public class Belonging
    {
        public Object ownedObject;
        public bool[] featureKnowledge;

        public Belonging(Object newObject)
        {
            featureKnowledge = new bool[newObject.features.Count];
            ownedObject = newObject;
        }
    }

    public Belonging GetBelongingFromObject(Object searchedObject)
    {
        Belonging potentialBelonging = null;
        for (int i = 0; i < belongings.Count; i++)
        {
            if(belongings[i].ownedObject == searchedObject)
            {
                potentialBelonging =  belongings[i];
            }
        }
        return potentialBelonging;
    }
}
