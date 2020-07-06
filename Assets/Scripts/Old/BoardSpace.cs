using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpace : MonoBehaviour
{
    [Header("Neighbourhood")]
    // 0 = Up, 1 = down, 2 = left, 3 = right
    public BoardSpace[] neighbourSpaces;
    
    public BoardSpace[] diagonalNeighbourSpaces;

    public Jewel myJewel;

    [Header("Settings")]
    public int mySlotType;
    public bool isEmpty;
    public int instanceID;
   

    // Private variables
    // Components
    private BoardManager boardManager;
    private GameStateManager gameStateManager;

    #region DefaultScripts
    private void Awake()
    {

    }

    private void Start()
    {

        SetComponents();

        CheckNeighbours();
    }
    #endregion

    #region Runtime

    public void CheckNeighbours()
    {

        if (boardManager == null)
        {
            boardManager = GetComponentInParent<BoardManager>();
            myJewel = transform.GetChild(0).GetComponent<Jewel>();
            mySlotType = myJewel.jewelType;
        }

        int maxJewels = boardManager.squareGridSize * boardManager.squareGridSize;

        // Up
        neighbourSpaces[0] = instanceID - boardManager.squareGridSize >= 0 ? boardManager.boardSpaces[instanceID - boardManager.squareGridSize] : null;
        // Down
        neighbourSpaces[1] = instanceID + boardManager.squareGridSize < maxJewels ? boardManager.boardSpaces[instanceID + boardManager.squareGridSize] : null;
        // Left    
        neighbourSpaces[2] = instanceID % boardManager.squareGridSize != 0 ? boardManager.boardSpaces[instanceID - 1] : null;
        // Right  
        if (instanceID > 0)
            neighbourSpaces[3] = (instanceID +1 ) % boardManager.squareGridSize  != 0 ? boardManager.boardSpaces[instanceID + 1] : null;
        else
            neighbourSpaces[3] = boardManager.boardSpaces[1];
    }
    public void CheckDiagonalNeighbours()
    {
        if(neighbourSpaces[0] != null)
        diagonalNeighbourSpaces[0] = neighbourSpaces[0].neighbourSpaces[2];
        if (neighbourSpaces[3] != null)
        diagonalNeighbourSpaces[1] = neighbourSpaces[3].neighbourSpaces[0];
        if (neighbourSpaces[1] != null)
        diagonalNeighbourSpaces[2] = neighbourSpaces[0].neighbourSpaces[2];
        if (neighbourSpaces[2] != null)
        diagonalNeighbourSpaces[2] = neighbourSpaces[1].neighbourSpaces[1];

    }

    public void ReRandomize()
    {
        myJewel.SetJewel();
        mySlotType = myJewel.jewelType;
    }

    public void CheckState(Jewel jewel)
    {
        myJewel = jewel;
        myJewel.boardSpace = GetComponent<BoardSpace>();
        mySlotType = myJewel.jewelType;
        CheckNeighbours();
    }

    public void CheckFreeSpace()
    {
      

    }

    public void TriggerFall()
    {
        if (neighbourSpaces[0] != null)
            neighbourSpaces[0].TriggerFall();

        if (instanceID + boardManager.squareGridSize < boardManager.squareGridSize * boardManager.squareGridSize)
            myJewel.OnMove(boardManager.boardSpaces[instanceID + boardManager.squareGridSize]);
    }

   

   
    #endregion

    #region Utilities
    private void SetComponents()
    {
        boardManager = GetComponentInParent<BoardManager>();
        gameStateManager = FindObjectOfType<GameStateManager>();
        myJewel = transform.GetChild(0).GetComponent<Jewel>();
        mySlotType = myJewel.jewelType;


    }

    #endregion

}
