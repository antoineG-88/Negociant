using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoader : MonoBehaviour
{
    public List<Object> initialPlayerBelongings;
    public List<Object> allObjects;
    public PlayerSave playerSave;
    public static SaveLoader I;
    private void Awake()
    {
        if(I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        playerSave = new PlayerSave();
        playerSave.charactersInfo = new List<PlayerSave.CharacterInfo>();
        playerSave.playerOwnedObjects = new List<string>();
        playerSave.passedDays = 0;
        for (int i = 0; i < initialPlayerBelongings.Count; i++)
        {
            playerSave.playerOwnedObjects.Add(initialPlayerBelongings[i].objectName);
        }
    }

    private void Update()
    {
    }

    public void SaveCharacter(CharacterHandler characterHandler)
    {
        PlayerSave.CharacterInfo currentInfo = GetCharacterInfoFromCharacter(characterHandler.character);
        if(currentInfo != null)
        {
            playerSave.charactersInfo.Remove(currentInfo);
        }

        currentInfo = new PlayerSave.CharacterInfo();
        currentInfo.characterName = characterHandler.character.characterName;
        currentInfo.playerNotes = characterHandler.playerNotes;
        currentInfo.playerCategoryNote = characterHandler.playerCategoryNote;
        currentInfo.ownedObjects = new List<string>();
        for (int i = 0; i < characterHandler.belongings.Count; i++)
        {
            currentInfo.ownedObjects.Add(characterHandler.belongings[i].linkedObject.objectName);
        }

        playerSave.charactersInfo.Add(currentInfo);
    }

    public void SavePlayerObjects()
    {
        playerSave.playerOwnedObjects = new List<string>();

        for (int i = 0; i < NegoceManager.I.playerHandler.allStallObjects.Count; i++)
        {
            playerSave.playerOwnedObjects.Add(NegoceManager.I.playerHandler.allStallObjects[i].linkedObject.objectName);
        }
    }

    public Object GetObjectFromName(string objectName)
    {
        Object searchedObject = null;
        for (int i = 0; i < allObjects.Count; i++)
        {
            if(allObjects[i].objectName == objectName)
            {
                searchedObject = allObjects[i];
            }
        }
        return searchedObject;
    }

    public PlayerSave.CharacterInfo GetCharacterInfoFromCharacter(Character searchedCharacter)
    {
        PlayerSave.CharacterInfo characterInfo = null;
        for (int i = 0; i < playerSave.charactersInfo.Count; i++)
        {
            if(playerSave.charactersInfo[i].characterName == searchedCharacter.characterName)
            {
                characterInfo = playerSave.charactersInfo[i];
            }
        }
        return characterInfo;
    }
}
