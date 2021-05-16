using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMindedCharacterHandler : CharacterHandler
{
    [Header("Open-Minded Options")]
    public float minLookingTime;
    public float highLevelInterestCuriosityBoost;
    public int[] gazedObjectPerGazeTimePerInterestingObjectOnVitrine;
    [HideInInspector] public int gazedObjectThisGazeTime;

    public override void Init()
    {
        base.Init();
    }
    public override void UpdateBehavior()
    {
        if (!isListening)
        {
            if (gazeTimeRmn > 0)
            {
                gazeTimeRmn -= Time.deltaTime;
            }

            if (nonGazingTimeRMN > 0)
            {
                nonGazingTimeRMN -= Time.deltaTime;
            }

            if (nonGazingTimeRMN <= 0 && gazeTimeRmn <= 0)
            {
                if (gazedObjectPerGazeTimePerInterestingObjectOnVitrine[Mathf.Clamp(GetNumberOfInitialCategoryObjectOnVitrine(), 0, gazedObjectPerGazeTimePerInterestingObjectOnVitrine.Length - 1)] - gazedObjectThisGazeTime > 0)
                {
                    gazedObjectThisGazeTime++;
                    LookObject(GetMaxCuriosityObjectOnVitrine(potentialObjects), Mathf.Max(minLookingTime, baseLookingTime / gazedObjectPerGazeTimePerInterestingObjectOnVitrine[Mathf.Clamp(GetNumberOfInterestingObjectOnVitrine(), 0, gazedObjectPerGazeTimePerInterestingObjectOnVitrine.Length - 1)]));
                }
                else
                {
                    gazedObjectThisGazeTime = 0;
                    StartNonGazing(reflexionTime);
                }
            }
        }
        else
        {
            StartNonGazing(reflexionTime);
        }


        foreach (PotentialObject potentialObject in potentialObjects)
        {
            if (potentialObject != lookedObject && potentialObject.stallObject.stallSpace.isVitrine)
            {
                potentialObject.curiosityLevel += Time.deltaTime * curiosityIncreaseSpeed
                    * (DoesObjectHasCommonCategory(potentialObject.stallObject.linkedObject, character.initialInterests) ? highLevelInterestCuriosityBoost : 1);
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
                if (!isListening && !isLeaving)
                {
                    StartCoroutine(Leave());
                }
                currentEnthousiasm = 0;
            }
        }
    }

    public int GetNumberOfInitialCategoryObjectOnVitrine()
    {
        int numberOnVitrine = 0;

        foreach (PotentialObject potentialObject in potentialObjects)
        {
            if (potentialObject.stallObject.stallSpace.isVitrine && DoesObjectHasCommonCategory(potentialObject.stallObject.linkedObject, character.initialInterests))
            {
                numberOnVitrine++;
            }
        }
        return numberOnVitrine;
    }
}
