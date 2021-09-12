using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Item
{
    Cube,
    Sphear,
    Cylinder,
    Capsule
}

public class EnumLength
{
    public static int Length()
	{
        return Enum.GetNames(typeof(Item)).Length;
	}
}

public class ItemObject : MonoBehaviour
{
    public Item item;

}
