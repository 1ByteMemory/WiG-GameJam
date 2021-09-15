using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public Sprite[] ImgArray;

    [Header("")]
    public Slider ProgressBar;
    public Text Timer;
    public Text Productivity;

    [Header("")]
    public int StartingTimer = 60;
    public float progressSpeed = 1;

    [Header("")]
    [Range(0, 1)]
    public float workingProdModifier;
    [Range(0, 1)]
    public float tiredProdModifier;
    [Range(0, 1)]
    public float relaxinfProdModifier;


    float fProgress;
    int iTimer;
    float fProductivity;

    static bool isGameEnd;
    public static bool IsGameActive
	{
        get { return !isGameEnd; }
	}
    
    NPC[] npcArray;

    public UnityEvent OnWin;
    public UnityEvent OnLose;

    IEnumerator CountDown()
	{
        while (!isGameEnd)
        {
            iTimer--;
            yield return new WaitForSeconds(1);
        }
	}

	private void Start()
	{
        SetCursor(false);
        isGameEnd = false;

        // Add all npcs to the array
        npcArray = FindObjectsOfType<NPC>();

        iTimer = StartingTimer;

        // Start the count down timer
        StartCoroutine(CountDown());
	}


	private void Update()
	{
        // Update timer ui
        Timer.text = iTimer.ToString();
        if (iTimer == 0 && !isGameEnd)
		{
            isGameEnd = true;
            StopCoroutine(CountDown());
            GameLose();
		}

        if (fProgress >= 100 && !isGameEnd)
		{
            isGameEnd = true;
            StopCoroutine(CountDown());
            GameWin();

		}

        if (Input.GetKeyDown(KeyCode.T))
		{
            fProgress = 100;
		}

        // Restart the level
        if (Input.GetKeyDown(KeyCode.F5))
		{
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

        if (Input.GetKeyDown(KeyCode.Escape))
		{
            isGameEnd = !isGameEnd;

            SetCursor(isGameEnd);

            // Resume Count Down
            if (!isGameEnd) StartCoroutine(CountDown());
		}

        if (!isGameEnd)
		{

        // ------ Calculate productivity ----- //
            
            // Count amount of emotions
            int tiredCount = 0;
            int relaxingCount = 0;
            int workingCount = 0;
			for (int i = 0; i < npcArray.Length; i++)
			{
				switch (npcArray[i].state)
				{
					case State.Normal:
                        workingCount++;
                        continue;
					case State.Tired:
                        tiredCount++;
						continue;
					case State.Relaxing:
                        relaxingCount++;
						continue;
				}
			}


            float percentTired = (float)tiredCount / (float)npcArray.Length;
            float percentRelaxing = (float)relaxingCount / (float)npcArray.Length;
            float percentWorking = (float)workingCount / (float)npcArray.Length;

            
            fProductivity = ((percentWorking * workingProdModifier) + 
                            (percentTired * tiredProdModifier) + 
                            (percentRelaxing * relaxinfProdModifier)) * 100;

            // Limit fPrductivity to two decimal places
            fProductivity = Mathf.Round(fProductivity * 100) / 100;


            // Increase progress based on productivity
            Productivity.text = "+" + fProductivity.ToString() + "%";
            fProgress += fProductivity * progressSpeed * Time.deltaTime;

            ProgressBar.value = fProgress;
        }
	}

    public void SetCursor(bool @lock)
	{

        Cursor.lockState = (CursorLockMode)(@lock ? 0 : 1);
        Cursor.visible = @lock;
	}

    public void GameLose()
	{
        SetCursor(true);
        OnLose.Invoke();
        Debug.Log("Game Over!");
	}

    public void GameWin()
	{
        SetCursor(true);
        OnWin.Invoke();
        Debug.Log("You Win!");
	}

}
