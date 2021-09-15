using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State
{
    Normal,
    Tired,
    Relaxing
}
public class NPC : MonoBehaviour
{
    [Header("Debug Options")]
    public bool overrideDefault;
    public Item oDesiredItem;

    [Header("Regular Options")]
    public Item desiredItem;
    public State state;
    public float minCooldownTime = 5;
    public float maxCooldownTime = 10;
    float endTime;

    Sprite[] emotions;

    GameManager gm;
    NavMeshAgent ai;
    Vector3 startPos;

    public ParticleSystem particles;
    public GameObject emotion;

    private GameObject heldItem;
    public GameObject HeldItem
	{
        get { return heldItem; }
	}

    bool isClose(Vector3 a, Vector3 b, float distance)
	{
        return ((a.x - b.x) * (a.x - b.x)) 
             + ((a.y - b.y) * (a.y - b.y))
             + ((a.z - b.z) * (a.z - b.z))
             <= distance * distance;
        
	}


    // Start is called before the first frame update
    void Start()
    {
        endTime = Random.Range(minCooldownTime, maxCooldownTime);
        emotion.SetActive(false);
        gm = FindObjectOfType<GameManager>();
        emotions = gm.ImgArray;

        Color col = Random.ColorHSV(0, 1, 1, 1, 0.5f, 1, 1, 1);
        GetComponent<MeshRenderer>().material.color = col;
        ai = GetComponent<NavMeshAgent>();
        ai.enabled = false;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsGameActive)
        {
            if (Time.time >= endTime && state == State.Normal)
            {
                SetEmotion(State.Tired);
                //state = State.Tired;
            }

            if (isClose(transform.position, startPos, 1) && (state == State.Normal || state == State.Tired))
			{
                ai.enabled = false;
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

    public void NavMeshEnable(bool enabled)
	{
        ai.enabled = enabled;
	}

    public State GetEmotion()
	{
        return state;
	}

    public void SetEmotion(State emotion)
	{
        state = emotion;
		switch (emotion)
		{
			case State.Normal:
                endTime = Time.time + Random.Range(minCooldownTime, maxCooldownTime);
                particles.Stop();
                if (!isClose(transform.position, startPos, 1))
                {
                    ai.enabled = true;
                    ai.SetDestination(startPos);
                }
                if (heldItem != null && ItemEnum.IsItemStatic(desiredItem))
				{
                    heldItem.GetComponent<ItemObject>().isOccupied = false;
                    heldItem.GetComponent<ItemObject>().npcRelaxingSpot = null;
                    heldItem = null;
				}
                GetComponent<CapsuleCollider>().enabled = true;
                break;

			case State.Tired:
                if (overrideDefault)
				{
                    this.emotion.SetActive(true);
                    desiredItem = oDesiredItem;
                    this.emotion.GetComponent<SpriteRenderer>().sprite = emotions[(int)desiredItem];
                    particles.Stop();
                    if (!isClose(transform.position, startPos, 1))
                    {
                        ai.enabled = true;
                        ai.SetDestination(startPos);
                    }
                    break;
				}
                
                this.emotion.SetActive(true);
                desiredItem = (Item)Random.Range(0, ItemEnum.Length());
                this.emotion.GetComponent<SpriteRenderer>().sprite = emotions[(int)desiredItem];
                particles.Stop();
                if (!isClose(transform.position, startPos, 1))
                {
                    ai.enabled = true;
                    ai.SetDestination(startPos);
                }
                break;

			case State.Relaxing:
                relaxingEndtime = Time.time + Random.Range(minCooldownTime, maxCooldownTime);
                this.emotion.SetActive(false);
                particles.Play();
                ai.enabled = false;
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
        if (item.GetComponent<ItemObject>().isPickUp)
        {
            item.transform.parent = transform;
            item.transform.localPosition = transform.right;
        }

        heldItem = item;

        if (state == State.Tired && item.GetComponent<ItemObject>().item == desiredItem)
        {
            SetEmotion(State.Relaxing);
        }
    }
}
