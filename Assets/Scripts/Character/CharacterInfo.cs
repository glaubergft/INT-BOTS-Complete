using System;
using UnityEngine;

public enum Character
{
    None,
    Blue,
    Green,
    Purple,
    Red,
    White,
    Yellow
}

[Serializable]
public class CharacterInfo
{
    public Character Character;
    public Material Material;
}