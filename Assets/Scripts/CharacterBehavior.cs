using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBehavior : MonoBehaviour
{
    public Character character;
    public Image illustrationImage;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void RefreshCharacterDisplay()
    {
        illustrationImage.sprite = character.illustration;
    }
}
