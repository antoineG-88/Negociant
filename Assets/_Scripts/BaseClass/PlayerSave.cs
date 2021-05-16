using System.Collections;
using System.Collections.Generic;

public class PlayerSave
{
    public List<string> playerOwnedObjects;
    public List<CharacterInfo> charactersInfo;
    public int passedDays;
    public class CharacterInfo
    {
        public string characterName;
        public string playerNotes;
        public int playerCategoryNote;

        public List<string> ownedObjects;
    }
}
