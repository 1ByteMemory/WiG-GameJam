using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Item
{
    Cube,
    Sphear,
    Cylinder,
    Capsule,
    Treadmil,
    Hamock
}

public class ItemEnum
{
    public static int Length()
	{
        return Enum.GetNames(typeof(Item)).Length;
	}

    /// <summary>
    /// Returns a list of items that can't be picked up by the player
    /// </summary>
    /// <returns>Item[]</returns>
    public static Item[] StaticItems()
	{
        return new Item[] { Item.Treadmil, Item.Hamock};
	}


    public static bool IsItemStatic(Item desiredItem)
	{
        Item[] items = StaticItems();

        for (int i = 0; i < items.Length; i++)
		{
            if (desiredItem == items[i]) return true;
		}
        return false;
	}
}

public class ItemObject : MonoBehaviour
{
    public Item item;

    public bool isPickUp;
    public Transform npcRelaxingSpot;
    [HideInInspector]
    public bool isOccupied;
}
