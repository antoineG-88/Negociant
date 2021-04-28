using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public List<Object> _allObjects;
    public List<Color> _categoriesColor;
    public List<Sprite> _categoriesIcon;

    public static List<Object> allObjects;
    public static List<Color> categoriesColor;
    public static List<Sprite> categoriesIcon;

    private void Awake()
    {
        allObjects = _allObjects;
        categoriesColor = _categoriesColor;
        categoriesIcon = _categoriesIcon;
    }
}
