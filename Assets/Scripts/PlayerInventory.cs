using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInventory
{
    public List<Belonging> objectsOwned;

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
}
