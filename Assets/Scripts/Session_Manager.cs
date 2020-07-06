using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Session_Manager : MonoBehaviour
{
    public bool paused;

    [Header("Stage Settings")]
    public StageConditions[] stageConditions;
    public int currentStage;
    public int requiredScore;
    public int stageTime;
    public int timer;

    [Header("Scoring")]
    public int currentScore;
    public int scorePlus;
    public int scoreExtraPoints;

    [Header("UI")]
    public Text scoreText;
    public Text scorePlusText;
    public Text timeText;
    public Text scoreGoalText;
    public Image scoreBar;
    public Animator uIAnimator;
    public Animator scorePlusAnimator;

    [Header("Menus")]
    public CanvasGroup introMenu;
    public Stage_Prompt stagePrompt;
    public GameObject hudScore;
    public GameObject hudPause;
    public GameObject stageDone;
    public GameObject gameDone;
    public GameObject pauseMenu;
    private Board_Manager boardManager;
    
    private void Start()
    {
        //StageStart();
        boardManager = FindObjectOfType<Board_Manager>();
    }

    public void NextStage()
    {
        if (currentStage >= stageConditions.Length)
        {
            EndGame();
        }
        else
        {
            stageDone.SetActive(false);

            stagePrompt.gameObject.SetActive(true);
            stagePrompt.SetStage(currentStage);
        }
    }


    public void StageStart()
    {
        boardManager.board.gameObject.SetActive(true);
        currentScore = 0;

        requiredScore = stageConditions[currentStage].requiredScore;
        boardManager.stageDifficulty = stageConditions[currentStage].gems;

        hudScore.SetActive(true);
        hudPause.SetActive(true);

        InvokeRepeating("TimeCounter", 2f, 1f);
        scoreText.text = "0";
        timeText.text = stageTime.ToString();
        timer = stageTime;
        scoreGoalText.text = "GOAL: " + requiredScore.ToString();
        StartCoroutine(boardManager.GenerateBoard());
    }

    public void BeginGame()
    {
        StartCoroutine(FadeCanvasGroup(introMenu));

        stagePrompt.gameObject.SetActive(true);
        stagePrompt.SetStage(currentStage);

    }


    public void EndGame()
    {
        stageDone.SetActive(false);
        gameDone.SetActive(true);

    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }

    public void StageEnd()
    {
        //  boardManager.board.gameObject.SetActive(false);
        hudPause.SetActive(false);
        hudScore.SetActive(false);
        stageDone.SetActive(true);

        stageDone.GetComponent<Stage_Prompt>().stageLevel = currentStage + 1;
        currentStage++;

    }

    public void StageReset()
    {
        hudPause.SetActive(false);
        hudScore.SetActive(false);
        stagePrompt.gameObject.SetActive(true);
        stagePrompt.ResetStage(currentStage);
    }

    private void TimeUp()
    {


        boardManager.ClearBoard();
        CancelInvoke("TimeCounter");

        if (currentScore >= requiredScore)
            StageEnd();
        else
            StageReset();


    }


    private void TimeCounter()
    {
        if (!paused)
        {
            timer--;

            timeText.text = timer.ToString();

            if (timer <= 0)
            {
                TimeUp();
            }
        }
    }

    public void Score(int gems)
    {
        int extraPoints = 0;

        if (gems > 3)
        {
            extraPoints = gems - 3;
        }

        int scoreAdd = (gems * scorePlus) + (extraPoints * scoreExtraPoints);

        currentScore += scoreAdd;

        UpdateScoreUI(scoreAdd);
    }


    private void UpdateScoreUI(int ammount)
    {
        scoreText.text = currentScore.ToString();

        if (ammount > 0)
        {
            scorePlusAnimator.SetTrigger("Score");
            scorePlusText.text = "+ " + ammount.ToString();
        }

        scoreBar.fillAmount = 1 - (requiredScore / currentScore);
    }

    public void PauseGame(bool state)
    {
        paused = state;

        hudPause.gameObject.SetActive(!state);
        boardManager.board.gameObject.SetActive(!state);
        hudScore.SetActive(!state);
        pauseMenu.gameObject.SetActive(state);
            
        


    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg)
    {
        for (float f = 1; f > -0.1f; f -= 0.05f)
        {
            yield return new WaitForEndOfFrame();
            cg.alpha = f;
        }
        cg.blocksRaycasts = false;
    }

    [System.Serializable]
    public class StageConditions
    {
        public int requiredScore;
        public int gems;


    }
}
