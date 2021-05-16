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
        if (!isListening)
        {
            if (nonGazingTimeRMN <= 0 && gazeTimeRmn <= 0)
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
                    if(GetMaxCuriosityObjectOnVitrine(potentialObjectsToLook) != null)
                    {
                        LookObject(GetMaxCuriosityObjectOnVitrine(potentialObjectsToLook), baseLookingTime);
                        objectsLookedNumber++;
                    }
                }
                else
                {
                    StartNonGazing(reflexionTime);
                    reflexionFlag = true;
                }
            }

            if (gazeTimeRmn > 0)
            {
                gazeTimeRmn -= Time.deltaTime;
            }

            if (nonGazingTimeRMN > 0)
            {
                nonGazingTimeRMN -= Time.deltaTime;
            }
        }
        else
        {
            StartNonGazing(reflexionTime);
        }


        foreach (PotentialObject potentialObject in potentialObjects)
        {
            if (potentialObject != lookedObject && potentialObject.stallObject.stallSpace.isVitrine && DoesObjectHasCommonCategory(potentialObject.stallObject.linkedObject, character.initialInterests))
            {
                potentialObject.curiosityLevel += Time.deltaTime * curiosityIncreaseSpeed;
            }
        }

        timeSpendRefreshEnthousiasm += Time.deltaTime;

        if (timeSpendRefreshEnthousiasm > timeBeforeEnthousiasmDecrease || GetAllInterestingObjectsOnVitrine().Count == 0)
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
            if (!isListening && !isLeaving)
            {
                StartCoroutine(Leave());
            }
        }
    }

    private List<PotentialObject> GetAllInterestingObjectsOnVitrine()
    {
        List<PotentialObject> interestingPotentialObjects = new List<PotentialObject>();
        for (int i = 0; i < potentialObjects.Count; i++)
        {
            if (DoesObjectHasCommonCategory(potentialObjects[i].stallObject.linkedObject, character.initialInterests) && potentialObjects[i].stallObject.stallSpace.isVitrine)
            {
                interestingPotentialObjects.Add(potentialObjects[i]);
            }
        }
        return interestingPotentialObjects;
    }
}