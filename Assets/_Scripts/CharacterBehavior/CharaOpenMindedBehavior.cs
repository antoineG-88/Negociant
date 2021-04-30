using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenMindedBehavior", menuName = "Negociant/Create new open-minded behavior", order = 4)]
public class CharaOpenMindedBehavior : CharacterBehavior
{
    public float minLookingTime;
    public float highLevelInterestCuriosityBoost;
    public int[] gazedObjectPerGazeTimePerInterestingObjectOnVitrine;
    [HideInInspector] public int gazedObjectThisGazeTime;

    public override void Init(CharacterHandler characterHandler, ref List<CharacterHandler.PotentialObject> potentialObjects)
    {
        gazedObjectThisGazeTime = 0;
    }

    public override void UpdateBehavior(CharacterHandler characterHandler, ref List<CharacterHandler.PotentialObject> potentialObjects)
    {
        if (!characterHandler.isTalking)
        {
            if (characterHandler.gazeTimeRmn > 0)
            {
                characterHandler.gazeTimeRmn -= Time.deltaTime;
            }

            if (characterHandler.reflexionTimeRMN > 0)
            {
                characterHandler.reflexionTimeRMN -= Time.deltaTime;
            }

            if (characterHandler.reflexionTimeRMN <= 0 && characterHandler.gazeTimeRmn <= 0)
            {
                if (gazedObjectPerGazeTimePerInterestingObjectOnVitrine[Mathf.Clamp(characterHandler.GetNumberOfInterestingObjectOnVitrine(), 0, gazedObjectPerGazeTimePerInterestingObjectOnVitrine.Length - 1)] - gazedObjectThisGazeTime > 0)
                {
                    gazedObjectThisGazeTime++;
                    characterHandler.LookObject(characterHandler.GetMaxCuriosityObjectOnVitrine(potentialObjects), Mathf.Max(minLookingTime, baseLookingTime / gazedObjectPerGazeTimePerInterestingObjectOnVitrine[Mathf.Clamp(characterHandler.GetNumberOfInterestingObjectOnVitrine(), 0, gazedObjectPerGazeTimePerInterestingObjectOnVitrine.Length - 1)]));
                }
                else
                {
                    gazedObjectThisGazeTime = 0;
                    characterHandler.StartReflexion(reflexionTime);
                }
            }
        }
        else
        {
            characterHandler.StartReflexion(reflexionTime);
        }


        foreach (CharacterHandler.PotentialObject potentialObject in potentialObjects)
        {
            if (potentialObject != characterHandler.lookedObject && potentialObject.stallObject.stallSpace.isVitrine)
            {
                potentialObject.curiosityLevel += Time.deltaTime * curiosityIncreaseSpeed
                    * (characterHandler.DoesObjectHaveHigherInterestLevel(potentialObject) ? highLevelInterestCuriosityBoost : 1);
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
}
