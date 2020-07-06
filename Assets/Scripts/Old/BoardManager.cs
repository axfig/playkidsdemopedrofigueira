using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    [Header("Runtime Instace")]
    public List<BoardSpace> boardSpaces;
    public List<BoardSpace> matchArray;

    [Header("Main Parameters")]
    public int  squareGridSize;
    public float jewelDistance;

    [Header("Target Objects")]
    public GameObject jewelSource;
    public GameObject boardSpaceSource;
    public Transform jewelTrash;

    // Private Variables
    // Transforms, Objects and components
    private Transform board;
    private GameStateManager gameStateManager;
    private int surroundingCheckCounter;
    private BoardSpace surroundingCheckIteration;
    private BoardSpace surroundingCheckSelection;
    private void Awake()
    {
        FetchComponents();
    }

    private void Start()
    {
        SpawnJewels();
        CheckPlayability();
    }

    private void CheckPlayability()
    {
        ClearStartMatches();
        CheckForNoSolution();
    }

    private void SpawnJewels()
    {
        for (int row = 0; row < squareGridSize; row++)
        {
            for (int column = 0; column < squareGridSize; column++)
            {
                // instances
                GameObject boardSpace = Instantiate(boardSpaceSource, board);
                Transform boardspaceTR = boardSpace.GetComponent<Transform>();
                BoardSpace boardspaceScript = boardSpace.GetComponent<BoardSpace>();
                GameObject jewel = Instantiate(jewelSource, boardspaceTR);
                // Positioning
                boardspaceTR.localPosition = GetJewelPosition(column,row);
                jewel.transform.localPosition = Vector2.zero;
                boardSpace.name = "R" + row.ToString() + " | C " + column;
                boardspaceScript.instanceID = (column ) + (row * squareGridSize);
                // Indexing
                boardSpaces.Add(boardspaceScript);
            }
        }

       
    }

    // Check: is the board impossible?
    public void CheckForNoSolution()
    {
        // Reset condition 1: no pairing neighbours
        for (int i = 0; i < boardSpaces.Count; i++)
        {
            boardSpaces[i].CheckNeighbours();
        }

        for (int i = 0; i < boardSpaces.Count; i++)
        {
            if (boardSpaces[i].neighbourSpaces[0] != null && boardSpaces[i].neighbourSpaces[1] != null)
            {
                if (boardSpaces[i].neighbourSpaces[0].mySlotType == boardSpaces[i].mySlotType || boardSpaces[i].neighbourSpaces[1].mySlotType == boardSpaces[i].mySlotType)
                {
                    return;
                }
            }
            if (boardSpaces[i].neighbourSpaces[2] != null && boardSpaces[i].neighbourSpaces[3] != null)
            {
                if (boardSpaces[i].neighbourSpaces[2].mySlotType == boardSpaces[i].mySlotType || boardSpaces[i].neighbourSpaces[3].mySlotType == boardSpaces[i].mySlotType)
                {
                    return;
                }
            }

        }

        for (int i = 0; i < boardSpaces.Count; i++)
        {
            for (int d = 0; d < 4; d++)
            {
                if (boardSpaces[i].diagonalNeighbourSpaces[d].mySlotType == boardSpaces[i].mySlotType)
                    return;

            }
        }
            // Check again only if no one has pairing neighbours AND no diagonal neighbours of a pair mach
        Debug.Log("repeat");
        CheckPlayability();

    }

    // Check: do we have matches right on start?
    public void ClearStartMatches()
    {
        for (int i = 0; i < boardSpaces.Count; i++)
        {
            boardSpaces[i].CheckNeighbours();
        }

        for (int i = 0; i < boardSpaces.Count; i++)
        {
            if (boardSpaces[i].neighbourSpaces[0] != null && boardSpaces[i].neighbourSpaces[1] != null)
            {
                if (boardSpaces[i].neighbourSpaces[0].mySlotType == boardSpaces[i].mySlotType && boardSpaces[i].neighbourSpaces[1].mySlotType == boardSpaces[i].mySlotType)
                {
                    boardSpaces[i].ReRandomize();
                    CheckPlayability();                    
                }
            }
            if (boardSpaces[i].neighbourSpaces[2] != null && boardSpaces[i].neighbourSpaces[3] != null)
            {
                if (boardSpaces[i].neighbourSpaces[2].mySlotType == boardSpaces[i].mySlotType && boardSpaces[i].neighbourSpaces[3].mySlotType == boardSpaces[i].mySlotType)
                {
                    boardSpaces[i].ReRandomize();
                    CheckPlayability();
                }
            }
        }
    }
    
    public IEnumerator SurroundingsCheckStart()
    {
        for (int i = 0; i < boardSpaces.Count; i++)
        {
            boardSpaces[i].CheckNeighbours();
        }
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < boardSpaces.Count; i++)
        {
            if (boardSpaces[i].neighbourSpaces[0] != null && boardSpaces[i].neighbourSpaces[1] != null)
            {
                if (boardSpaces[i].neighbourSpaces[0].mySlotType == boardSpaces[i].mySlotType && boardSpaces[i].neighbourSpaces[1].mySlotType == boardSpaces[i].mySlotType)
                {
                    Debug.Log("Vertical match @ " + boardSpaces[i].name);
                    OnMatch(0, boardSpaces[i]);
                   
                    
                }
            }
            if (boardSpaces[i].neighbourSpaces[2] != null && boardSpaces[i].neighbourSpaces[3] != null)
            {
                if (boardSpaces[i].neighbourSpaces[2].mySlotType == boardSpaces[i].mySlotType && boardSpaces[i].neighbourSpaces[3].mySlotType == boardSpaces[i].mySlotType)
                {
                    Debug.Log("horizontal match @ " + boardSpaces[i].name);
                    OnMatch(1, boardSpaces[i]);
                    
                }
            }            
        }

        if (matchArray.Count <= 0)
        {
            Debug.Log("No match!");
            gameStateManager.NoMatchRecovery();
        }
    }

    // 0 = vertical, 1 = horizontal
    public void OnMatch(int type, BoardSpace centerSpace)
    {
        if (type == 0)
        {
            matchArray.Add(centerSpace);
            matchArray.Add(centerSpace.neighbourSpaces[0]);
            matchArray.Add(centerSpace.neighbourSpaces[1]);
            //check for extra points
            if(centerSpace.neighbourSpaces[0] != null)
            SeekExtraPoints(0, centerSpace.neighbourSpaces[0], centerSpace.mySlotType);
            if (centerSpace.neighbourSpaces[1] != null)
            SeekExtraPoints(1, centerSpace.neighbourSpaces[1], centerSpace.mySlotType);

        }
        if (type == 1)
        {
            matchArray.Add(centerSpace);
            matchArray.Add(centerSpace.neighbourSpaces[2]);
            matchArray.Add(centerSpace.neighbourSpaces[3]);
            //check for extra points
            if (centerSpace.neighbourSpaces[2] != null)
                SeekExtraPoints(2, centerSpace.neighbourSpaces[2], centerSpace.mySlotType);
            if (centerSpace.neighbourSpaces[3] != null)
                SeekExtraPoints(3, centerSpace.neighbourSpaces[3], centerSpace.mySlotType);

        }

        for (int i = 0; i < matchArray.Count; i++)
        {

            Destroy (matchArray[i].myJewel.gameObject);
            matchArray[i].myJewel.gameObject.SetActive(false);

            matchArray[i].myJewel = null;
        }

        Invoke("Fallout",0.3f);
    }
    public void SeekExtraPoints(int direction,BoardSpace startSpace,int reference)
    {
            for (int i = 1; i < 3; i++)
            {
                switch (direction)
                {
                case 0:
                    if (startSpace.instanceID - (squareGridSize*i) > 0)
                    {
                        if (boardSpaces[startSpace.instanceID - squareGridSize * i].mySlotType == reference)
                        {
                            matchArray.Add(boardSpaces[startSpace.instanceID - squareGridSize * i]);
                            Debug.Log("extra points,  up");
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
                case 1:
                    if (startSpace.instanceID + (squareGridSize * i) < squareGridSize * squareGridSize)
                    {
                        if (boardSpaces[startSpace.instanceID + squareGridSize * i].mySlotType == reference)
                        {
                            matchArray.Add(boardSpaces[startSpace.instanceID + squareGridSize * i]);
                            Debug.Log("extra points,  down");
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
                case 2:
                    if (startSpace.instanceID % squareGridSize != 0  && startSpace.neighbourSpaces[0] != null)
                    {
                        if (boardSpaces[startSpace.instanceID - i].mySlotType == reference)
                        {
                            matchArray.Add(boardSpaces[startSpace.instanceID - i]);
                            Debug.Log("extra points, Left");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                    break;
                case 3:
                    if ((startSpace.instanceID +i) % squareGridSize  != 0)
                    {
                        if (boardSpaces[startSpace.instanceID + i].mySlotType == reference)
                        {
                            matchArray.Add(boardSpaces[startSpace.instanceID + i]);
                            Debug.Log("extra points, right");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                    break;
            }
        }
    }

    public void Fallout()
    {
        
        for (int i = boardSpaces.Count; i > 0; i--)
        {
            if (boardSpaces[i].neighbourSpaces[0] != null)
            {
                if (boardSpaces[i].myJewel == null)
                {
                    boardSpaces[i].neighbourSpaces[0].myJewel.OnFall(boardSpaces[i]);
                }
                else
                {
                    Debug.Log("Falling Complete");
                }
            }
        }
    
    }
    
    public Vector3 GetJewelPosition(int x, int y)
    {
        return new Vector3(x * jewelDistance, y *- jewelDistance);
    }

    private void FetchComponents()
    {
        board            = GetComponent<Transform>();
        gameStateManager = FindObjectOfType<GameStateManager>();
    }


}
