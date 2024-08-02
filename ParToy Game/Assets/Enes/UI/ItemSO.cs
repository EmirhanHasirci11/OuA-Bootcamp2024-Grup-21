using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopMenu", menuName = "Scriptable Objects/New Shop Item", order = 1 )]
public class ItemSO : ScriptableObject
{
    public string title;
    public string description;
    public int price;
    public string category;
    public bool isOwned = false;
}
