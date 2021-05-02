using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisiveCharacterHandler : CharacterHandler
{
    private List<PotentialObject> potentialObjectsToLook;
    private int objectsLookedNumber;
    private bool reflexionFlag;

    public override void Init()
    {
        base.Init();
        potentialObjectsToLook = new List<PotentialObject>();
        reflexionFlag = true;
    }

    public override void UpdateBehavior()
    {
        if (!isTalking)
        {
            if (reflexionTimeRMN <= 0 && gazeTimeRmn <= 0)
            {
                potentialObjectsToLook.Clear();
                potentialObjectsToLook.AddRange(GetAllInterestingObjectsOnVitrine());
                if (reflexionFlag)
                {
                    reflexionFlag = false;
                    potentialObjectsToLook.Clear();
                    potentialObjectsToLook.AddRange(GetAllInterestingObjectsOnVitrine());
                    objectsLookedNumber = 0;
                }

                if (objectsLookedNumber < potentialObjectsToLook.Count)
                {
                    LookObject(GetMaxCuriosityObjectOnVitrine(potentialObjectsToLook), baseLookingTime);
                    objectsLookedNumber++;
                }
                else
                {
                    StartReflexion(reflexionTime);
                    reflexionFlag = true;
                }
            }

            if (gazeTimeRmn > 0)
            {
                gazeTimeRmn -= Time.deltaTime;
            }

            if (reflexionTimeRMN > 0)
            {
                reflexionTimeRMN -= Time.deltaTime;
            }
        }
        else
        {
            StartReflexion(reflexionTime);
        }


        foreach (PotentialObject potentialObject in potentialObjects)
        {
            if (potentialObject != lookedObject && potentialObject.stallObject.stallSpace.isVitrine && DoesObjectHaveHigherInterestLevel(potentialObject))
            {
                potentialObject.curiosityLevel += Time.deltaTime * curiosityIncreaseSpeed;
            }
        }

        timeSpendRefreshEnthousiasm += Time.deltaTime;

        if (timeSpendRefreshEnthousiasm > timeBeforeEnthousiasmDecrease)
        {
            if (currentEnthousiasm > 0)
            {
               currentEnthousiasm -= Time.deltaTime * enthousiasmDecreaseRate;
            }
            else
            {
                currentEnthousiasm = 0;
            }
        }

        if (currentEnthousiasm <= 0)
        {
            if (!isTalking)
            {
                Leave();
            }
        }
    }

    private List<PotentialObject> GetAllInterestingObjectsOnVitrine()
    {
        List<PotentialObject> interestingPotentialObjects = new List<PotentialObject>();
        for (int i = 0; i < potentialObjects.Count; i++)
        {
            if (DoesObjectHaveHigherInterestLevel(potentialObjects[i]) && potentialObjects[i].stallObject.stallSpace.isVitrine)
            {
                interestingPotentialObjects.Add(potentialObjects[i]);
            }
        }
        return interestingPotentialObjects;
    }
}