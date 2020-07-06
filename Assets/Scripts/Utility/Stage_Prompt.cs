using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage_Prompt : MonoBehaviour
{
    public Image foodSprite;
    public Sprite[] allSprites;
    public int stageLevel;
    public Text stageHeader;
    public Text stageText;
    private Animator myAnimator;
    private Session_Manager sessionManager;
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        sessionManager = FindObjectOfType<Session_Manager>();
        canvasGroup = FindObjectOfType<CanvasGroup>();
    }


    public void SetStage(int s)
    {
        foodSprite.sprite = allSprites[s];
        stageLevel = s;
        stageHeader.text = "STAGE";
        stageText.text = (s+1).ToString();
        myAnimator.SetTrigger("Activate");
    }

    public void ResetStage(int s)
    {
        foodSprite.sprite = allSprites[s];
        stageLevel = s;
        stageHeader.text = "TRY AGAIN!";


        myAnimator.SetTrigger("Activate");
    }

    public void PromptDone()
    {
        canvasGroup.blocksRaycasts = false;
        sessionManager.StageStart();
        gameObject.SetActive(false);
    }

    public void StageCleared()
    {
        stageText.text = (stageLevel+1).ToString();
        sessionManager.NextStage();
    }
}
