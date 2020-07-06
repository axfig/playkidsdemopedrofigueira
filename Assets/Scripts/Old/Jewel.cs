using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jewel : MonoBehaviour
{
    [Header("Parameters")]
    public int jewelType;

    [Header("Visual Properties")]
    public SpriteRenderer mySprite;
    public Sprite[] spriteList;
    public BoardSpace boardSpace;
    
    // Private Properties
    // Components
    private GameStateManager gameStateManager;    
    private Animator animator;
    private BoardManager boardManager;
    // State keys
    private bool moving;
    private bool falling;
    #region defaultComponents
    private void Awake()
    {
        SetComponents();
        SetJewel();
    }
    private void FixedUpdate()
    {
        if (moving)
        {
            MovePieces();
        }

        if (falling)
        {
            FallPieces();
        }
    }

    #endregion

    #region interaction

    public void CheckNeighbours(int direction)
    {
       
    }
    
    public void OnMove(BoardSpace spaceToMove)
    {
        transform.parent = spaceToMove.transform;
        moving = true;
    }

    public void OnFall(BoardSpace spaceToMove)
    {
        transform.parent = spaceToMove.transform;
        falling = true;
        
    }

    public void MovePieces()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition , Vector3.zero,Time.deltaTime*10f);

        if (Vector3.Distance(transform.localPosition, Vector3.zero) == 0 )
        {
            moving = false;
            transform.parent.GetComponent<BoardSpace>().CheckState(GetComponent<Jewel>());
            StartCoroutine(boardManager.SurroundingsCheckStart());
        
        }
    }

    public void FallPieces()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, Time.deltaTime * 10f);

        if (Vector3.Distance(transform.localPosition, Vector3.zero) == 0)
        {
            falling = false;
            transform.parent.GetComponent<BoardSpace>().CheckState(GetComponent<Jewel>());
            boardSpace.CheckNeighbours();
            boardManager.Fallout();
          
        }

    }


    #endregion

    #region Setup

    public void SetJewel()
    {
        jewelType = Random.Range(0, 7);
        mySprite.sprite = spriteList[jewelType];
    }

    private void SetComponents()
    {
        mySprite    = GetComponent<SpriteRenderer>();
        gameStateManager = FindObjectOfType<GameStateManager>();
        boardSpace = transform.GetComponentInParent<BoardSpace>();
        animator = GetComponent<Animator>();
        boardManager = FindObjectOfType<BoardManager>();
    }

  
    #endregion

}
