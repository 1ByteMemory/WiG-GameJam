using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    //List<GameObject> itemObjects = new List<GameObject>();
    //List<Item> items = new List<Item>();
    Camera cam;
    public GameObject ui;
    public GameObject uiText;

    public Dictionary<Item, List<GameObject>> ItemsCollected = new Dictionary<Item, List<GameObject>>();
    //public Dictionary<Item, int> ItemsCollected = new Dictionary<Item, int>();

    public LayerMask layerHit;


    int index;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
		for (int i = 0; i < EnumLength.Length(); i++)
		{
            ItemsCollected.Add((Item)i, new List<GameObject>());

            GameObject text = Instantiate(uiText, ui.transform);
            RectTransform rect = text.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(1, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(0, 60);
            rect.anchoredPosition = new Vector2(0, -ItemsCollected.Count * 30);

            text.GetComponent<Text>().text = string.Format("{0}: {1}", (Item)i, 0);
            text.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;

            // Add the text element for easy access later
            ItemsCollected[(Item)i].Add(text);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsGameActive)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 10, layerHit))
            {
                // Outline Object
                if (hit.transform.GetComponent<Outline>())
                {
                    hit.transform.GetComponent<Outline>().Selected();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    NPC npc = hit.transform.GetComponent<NPC>();

                    // If Item, Pick up Item
                    if (hit.transform.GetComponent<ItemObject>() != null)
                    {
                        Item pickUp = hit.transform.GetComponent<ItemObject>().item;

                        hit.transform.gameObject.SetActive(false);


                        ItemsCollected[pickUp].Add(hit.transform.gameObject);
                        ItemsCollected[pickUp][0].GetComponent<Text>().text = string.Format("{0}: {1}", pickUp, ItemsCollected[pickUp].Count - 1);

                    }
                    else if (npc != null && npc.HeldItem != null)
                    {
                        // If NPC has item, take it

                        // Add Item to dictionary
                        Item pickUp = npc.HeldItem.GetComponent<ItemObject>().item;

                        ItemsCollected[pickUp].Add(npc.HeldItem);
                        ItemsCollected[pickUp][0].GetComponent<Text>().text = string.Format("{0}: {1}", pickUp, ItemsCollected[pickUp].Count - 1);

                        // Remove it from the npc
                        npc.StealItem();
                    }
                    else if (npc != null && npc.HeldItem == null)
                    {
                        // If NPC, Give Item

                        Item select = (Item)index;
                        //                          // Gets Item key      // Gets GameObject from List value
                        GameObject selectedItem = ItemsCollected[select][ItemsCollected[select].Count - 1];

                        if (selectedItem.GetComponent<ItemObject>())
                        {

                            // Set selected item to active and set it's position
                            selectedItem.SetActive(true);
                            hit.transform.GetComponent<NPC>().GiveItem(selectedItem);


                            // Remove it from the Dictionary
                            ItemsCollected[select].Remove(selectedItem);

                            // Update ui test==xt

                            ItemsCollected[select][0].GetComponent<Text>().text = string.Format("{0}: {1}", select, ItemsCollected[select].Count - 1);


                            // Reset positions of the text
                            for (int j = 0; j < ui.transform.childCount; j++)
                            {
                                //ui.transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(j + 1) * 30);
                            }
                        }
                    }
                }
            }

            // Cycle through inventory
            if (ui.transform.childCount > 0 && Input.mouseScrollDelta.y != 0)
            {
                ItemsCollected[(Item)index][0].GetComponent<Text>().color = Color.black;

                // Increase/Decrease index by scroll wheel
                index += -Input.mouseScrollDelta.y > 0 ? 1 : -1;

                //index = FindIndex(index, -Input.mouseScrollDelta.y > 0);

                // Loop the index when below zero or above count
                if (index < 0) index = ItemsCollected.Count - 1;
                if (index >= ItemsCollected.Count) index = 0;

                ItemsCollected[(Item)index][0].GetComponent<Text>().color = Color.red;

            }

        }
    }

    int emergeancyStop;
    int FindIndex(int index, bool increase)
	{
        emergeancyStop++;
        if (emergeancyStop > 10)
		{
            throw new System.ArgumentException("Recursion took too long.");
		}

        int i = 0;
        // Loop the index when below zero or above count
        if (index < 0) index = ItemsCollected.Count;
        if (index > ItemsCollected.Count) index = 0;


        // Skip index of empty gameObject list
        if (ItemsCollected[(Item)index].Count == 0)
		{
            if (increase) i = FindIndex(index++, true);
            else i = FindIndex(index--, true);
		}

        return i;
    }
}
