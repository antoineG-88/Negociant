using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecisiveBehavior", menuName = "Negociant/Create new decisive behavior", order = 3)]
public class CharaDecisiveBehavior : CharacterBehavior
{
    private List<CharacterHandler.PotentialObject> potentialObjectsToLook;
    private int objectsLookedNumber;
    private bool reflexionFlag;

    public override void Init(CharacterHandler characterHandler, ref List<CharacterHandler.PotentialObject> potentialObjects)
    {
        potentialObjectsToLook = new List<CharacterHandler.PotentialObject>();
        reflexionFlag = true;
    }

    public override void UpdateBehavior(CharacterHandler characterHandler, ref List<CharacterHandler.PotentialObject> potentialObjects)
    {
        if (!characterHandler.isTalking)
        {
            if (characterHandler.reflexionTimeRMN <= 0 && characterHandler.gazeTimeRmn <= 0)
            {
                potentialObjectsToLook.Clear();
                potentialObjectsToLook.AddRange(GetAllInterestingObjectsOnVitrine(characterHandler, potentialObjects));
                if (reflexionFlag)
                {
                    reflexionFlag = false;
                    potentialObjectsToLook.Clear();
                    potentialObjectsToLook.AddRange(GetAllInterestingObjectsOnVitrine(characterHandler, potentialObjects));
                    objectsLookedNumber = 0;
                }

                if (objectsLookedNumber < potentialObjectsToLook.Count)
                {
                    characterHandler.LookObject(characterHandler.GetMaxCuriosityObjectOnVitrine(potentialObjectsToLook), baseLookingTime);
                    objectsLookedNumber++;
                }
                else
                {
                    characterHandler.StartReflexion(reflexionTime);
                    reflexionFlag = true;
                }
            }

            if (characterHandler.gazeTimeRmn > 0)
            {
                characterHandler.gazeTimeRmn -= Time.deltaTime;
            }

            if (characterHandler.reflexionTimeRMN > 0)
            {
                characterHandler.reflexionTimeRMN -= Time.deltaTime;
            }
        }
        else
        {
            characterHandler.StartReflexion(reflexionTime);
        }


        foreach (CharacterHandler.PotentialObject potentialObject in potentialObjects)
        {
            if (potentialObject != characterHandler.lookedObject && potentialObject.stallObject.stallSpace.isVitrine && characterHandler.DoesObjectHaveHigherInterestLevel(potentialObject))
            {
                potentialObject.curiosityLevel += Time.deltaTime * curiosityIncreaseSpeed;
            }
        }

        characterHandler.timeSpendRefreshEnthousiasm += Time.deltaTime;

        if (characterHandler.timeSpendRefreshEnthousiasm > timeBeforeEnthousiasmDecrease)
        {
            if (characterHandler.currentEnthousiasm > 0)
            {
                characterHandler.currentEnthousiasm -= Time.deltaTime * enthousiasmDecreaseRate;
            }
            else
            {
                if (!characterHandler.isTalking)
                {
                    characterHandler.Leave();
                }
                characterHandler.currentEnthousiasm = 0;
            }
        }
    }

    private List<CharacterHandler.PotentialObject> GetAllInterestingObjectsOnVitrine(CharacterHandler characterHandler, List<CharacterHandler.PotentialObject> potentialObjectsToCheck)
    {
        List<CharacterHandler.PotentialObject> interestingPotentialObjects = new List<CharacterHandler.PotentialObject>();
        for (int i = 0; i < potentialObjectsToCheck.Count; i++)
        {
            if(characterHandler.DoesObjectHaveHigherInterestLevel(potentialObjectsToCheck[i]) && potentialObjectsToCheck[i].stallObject.stallSpace.isVitrine)
            {
                interestingPotentialObjects.Add(potentialObjectsToCheck[i]);
            }
        }
        return interestingPotentialObjects;
    }
}
