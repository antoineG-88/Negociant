using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RadialOption : UIInteractable
{
    public Image buttonBackground;
    public Color baseColor;
    public Color hoveredColor;

    public override void OnHoverIn()
    {
        buttonBackground.color = hoveredColor;
    }

    public override void OnHoverOut()
    {
        buttonBackground.color = baseColor;
    }
}
