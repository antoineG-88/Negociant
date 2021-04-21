using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBehavior : UIInteractable
{
    public Character character;
    public Image illustrationImage;
    public TweeningAnimator selectionAnim;

    [HideInInspector] public bool isSelected;
    [HideInInspector] public RectTransform gazeDisplay;

    private List<PotentialObject> potentialObjects;

    void Start()
    {

    }

    public override void Update()
    {
        if(clickedDown)
        {
            NegoceManager.I.playerHandler.SelectCharacter(this);
        }

        if(character.temper == Temper.Decisive)
        {
            DecisiveBehavior();
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(character.characterName + " est intéressé par : " + character.initialInterests[0].ToString());
        }

        base.Update();
    }


    #region Tempers Behaviors
    private void DecisiveBehavior()
    {
        PotentialObject choosedObject = null; // à retirer plus tard
        for (int i = 0; i < potentialObjects.Count; i++)
        {
            for (int y = 0; y < character.initialInterests.Count; y++)
            {
                for (int x = 0; x < potentialObjects[i].standObject.linkedObject.categories.Count; x++)
                {
                    if (potentialObjects[i].standObject.linkedObject.categories[x] == character.initialInterests[y])
                    {
                        choosedObject = potentialObjects[i];
                    }
                }
            }
        }

        if(choosedObject != null)
        {
            //gazeDisplay.anchoredPosition = choosedObject.standObject.rectTransform.anchoredPosition;
            gazeDisplay.position = choosedObject.standObject.rectTransform.position;
        }
    }
    #endregion

    public void RefreshPotentialObjects()
    {
        if(potentialObjects == null)
        {
            potentialObjects = new List<PotentialObject>();
        }

        foreach(PotentialObject potentialObject in potentialObjects)
        {
            if(!NegoceManager.I.playerHandler.allStandObjects.Contains(potentialObject.standObject))
            {
                potentialObjects.Remove(potentialObject);
            }
        }

        for (int i = 0; i < NegoceManager.I.playerHandler.allStandObjects.Count; i++)
        {
            bool isMissingStandObjectInPotentialObjects = true;
            foreach (PotentialObject potentialObject in potentialObjects)
            {
                if(potentialObject.standObject == NegoceManager.I.playerHandler.allStandObjects[i])
                {
                    isMissingStandObjectInPotentialObjects = false;
                }
            }

            if(isMissingStandObjectInPotentialObjects)
            {
                potentialObjects.Add(new PotentialObject(NegoceManager.I.playerHandler.allStandObjects[i]));
            }
        }
    }

    public void RefreshCharacterDisplay()
    {
        selectionAnim.anim = Instantiate(selectionAnim.anim);
        illustrationImage.sprite = character.illustration;
        illustrationImage.SetNativeSize();
        selectionAnim.GetReferences();
    }

    public void Select()
    {
        if(!isSelected)
        {
            isSelected = true;
            StartCoroutine(selectionAnim.anim.Play(selectionAnim, selectionAnim.originalPos));
        }
    }
    public void UnSelect()
    {
        if (isSelected)
        {
            StartCoroutine(selectionAnim.anim.PlayBackward(selectionAnim, selectionAnim.originalPos, true));
            isSelected = false;
        }
    }

    public override void OnHoverIn()
    {

    }

    public override void OnHoverOut()
    {

    }

    [System.Serializable]
    private class PotentialObject
    {
        public StandObject standObject;
        public float interestLevel;

        public PotentialObject(StandObject _standObject)
        {
            standObject = _standObject;
            interestLevel = 0;
        }
    }
}
