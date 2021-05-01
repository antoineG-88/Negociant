﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMindedCharacterHandler : CharacterHandler
{
    [Header("Open-Minded Options")]
    public float minLookingTime;
    public float highLevelInterestCuriosityBoost;
    public int[] gazedObjectPerGazeTimePerInterestingObjectOnVitrine;
    [HideInInspector] public int gazedObjectThisGazeTime;

    public override void UpdateBehavior()
    {
        if (!isTalking)
        {
            if (gazeTimeRmn > 0)
            {
                gazeTimeRmn -= Time.deltaTime;
            }

            if (reflexionTimeRMN > 0)
            {
                reflexionTimeRMN -= Time.deltaTime;
            }

            if (reflexionTimeRMN <= 0 && gazeTimeRmn <= 0)
            {
                if (gazedObjectPerGazeTimePerInterestingObjectOnVitrine[Mathf.Clamp(GetNumberOfInterestingObjectOnVitrine(), 0, gazedObjectPerGazeTimePerInterestingObjectOnVitrine.Length - 1)] - gazedObjectThisGazeTime > 0)
                {
                    gazedObjectThisGazeTime++;
                    LookObject(GetMaxCuriosityObjectOnVitrine(potentialObjects), Mathf.Max(minLookingTime, baseLookingTime / gazedObjectPerGazeTimePerInterestingObjectOnVitrine[Mathf.Clamp(GetNumberOfInterestingObjectOnVitrine(), 0, gazedObjectPerGazeTimePerInterestingObjectOnVitrine.Length - 1)]));
                }
                else
                {
                    gazedObjectThisGazeTime = 0;
                    StartReflexion(reflexionTime);
                }
            }
        }
        else
        {
            StartReflexion(reflexionTime);
        }


        foreach (CharacterHandler.PotentialObject potentialObject in potentialObjects)
        {
            if (potentialObject != lookedObject && potentialObject.stallObject.stallSpace.isVitrine)
            {
                potentialObject.curiosityLevel += Time.deltaTime * curiosityIncreaseSpeed
                    * (DoesObjectHaveHigherInterestLevel(potentialObject) ? highLevelInterestCuriosityBoost : 1);
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
                if (!isTalking)
                {
                    Leave();
                }
                currentEnthousiasm = 0;
            }
        }
    }
}
