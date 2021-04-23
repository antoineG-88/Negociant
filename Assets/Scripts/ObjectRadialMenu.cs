using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRadialMenu : MonoBehaviour
{
    public RadialOption presentRadialOption;
    public RadialOption argumentRadialOption;
    public PlayerHandler playerHandler;

    private void Update()
    {
        if(presentRadialOption.clickedUp)
        {
            playerHandler.PresentObject();
        }

        if (argumentRadialOption.clickedUp)
        {
            Debug.Log("Argument");
        }
    }
}
