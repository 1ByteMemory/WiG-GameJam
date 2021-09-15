using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum State
{
    Normal,
    Tired,
    Relaxing
}
public class NPC : MonoBehaviour
{
    public Item desiredItem;
    public State state;
    public float minCooldownTime = 5;
    public float maxCooldownTime = 10;
    float endTime;

    Sprite[] emotions;

    GameManager gm;

    public ParticleSystem particles;
    public GameObject emotion;

    private GameObject heldItem;
    public GameObject HeldItem
	{
        get { return heldItem; }
	}


    // Start is called before the first frame update
    void Start()
    {
        endTime = Random.Range(minCooldownTime, maxCooldownTime);
        emotion.SetActive(false);
        gm = FindObjectOfType<GameManager>();
        emotions = gm.ImgArray;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsGameActive)
        {
            if (Time.time >= endTime && state == State.Normal)
            {
                SetEmotion(State.Tired);
                state = State.Tired;
            }


            switch (state)
            {
                case State.Normal:
                    break;
                case State.Tired:

                    break;
                case State.Relaxing:
                    Relaxing();
                    break;
            }
        }
	}


    float relaxingEndtime;
    public void Relaxing()
	{
        // Stop relaxing after set time
        if (Time.time >= relaxingEndtime)
		{
            SetEmotion(State.Normal);
        }
	}

    public void SetEmotion(State emotion)
	{
        state = emotion;
		switch (emotion)
		{
			case State.Normal:
                endTime = Time.time + Random.Range(minCooldownTime, maxCooldownTime);
                particles.Stop();
                break;

			case State.Tired:
                this.emotion.SetActive(true);
                desiredItem = (Item)Random.Range(0, EnumLength.Length());
                this.emotion.GetComponent<SpriteRenderer>().sprite = emotions[(int)desiredItem];
                particles.Stop();
				break;

			case State.Relaxing:
                relaxingEndtime = Time.time + Random.Range(minCooldownTime, maxCooldownTime);
                this.emotion.SetActive(false);
                particles.Play();
                break;
		}
	}

    public void SetDesiredItem(Item item)
    {
        desiredItem = item;
        GetComponent<SpriteRenderer>().sprite = emotions[(int)item];
    }


    public void StealItem()
	{
        heldItem.transform.parent = null;
        heldItem.SetActive(false);
        heldItem = null;

        if (state == State.Relaxing)
		{
            Item temp = desiredItem;
            SetEmotion(State.Tired);

            // NPC wants the item that was stolen from it
            desiredItem = temp;
            emotion.GetComponent<SpriteRenderer>().sprite = emotions[(int)desiredItem];
        }
    }

    public void GiveItem(GameObject item)
	{
        item.transform.parent = transform;
        item.transform.localPosition = transform.right;

        heldItem = item;

        if (state == State.Tired && item.GetComponent<ItemObject>().item == desiredItem)
        {
            SetEmotion(State.Relaxing);
        }
    }
}
