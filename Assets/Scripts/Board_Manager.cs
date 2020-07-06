using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board_Manager : MonoBehaviour
{

    [Header("Configurable")]
    public int stageDifficulty;
    public int boardGrid;
    public float slotDistancing;

    [Header("Reference Objects")]
    public GameObject slotPrefab;
    public Transform board;    

    [Header("Indexing")]
    public List<Board_Slot> mySlots;
    public List<Board_Slot> matchArray;

    [Header("Keys")]

    public bool moveWithMatch;
    public bool prematureMatch;
    public bool softlocked;
    public bool canProceed;
    public bool canInput;
    public bool cascading;

    private Player_Controller playerController;
    private Session_Manager sessionManager;

    private void Awake()
    {
        Physics2D.queriesStartInColliders = false;
        playerController = FindObjectOfType<Player_Controller>();
        sessionManager = FindObjectOfType<Session_Manager>();
    }

    private void Start()
    {
       // StartCoroutine(GenerateBoard());
    }

    private void Update()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.Space)) 
            StartCoroutine(OnMovePerformed());
    }


    public IEnumerator GenerateBoard()
    {
        for (int row = 0; row < boardGrid; row++)
        {
            for (int column = 0; column < boardGrid; column++)
            {
                yield return new WaitForEndOfFrame();
                // instances
                GameObject boardSlot = Instantiate(slotPrefab, board);
                Transform boardSlotTR = boardSlot.GetComponent<Transform>();
                Board_Slot boardSlotScript = boardSlot.GetComponent<Board_Slot>();

                // Positioning
                boardSlotTR.localPosition = GetGemPosition(column, row);
                boardSlot.name = "R" + row.ToString() + " | C " + column;

                // Indexing
                mySlots.Add(boardSlotScript);

                if (row == 0)
                    boardSlotScript.topSlot = true;

                //GemNeration
                boardSlotScript.CreateGem(Random.Range(0, stageDifficulty ));
            }
        }


        StartCoroutine(DispatchNeighbourhoodCheck());

        StartCoroutine(BoardValidation());

        yield return null;
    }

    public void ClearBoard()
    {
        matchArray.Clear();
        for (int i = 0; i < mySlots.Count; i++)
        {
            Destroy(mySlots[i].gameObject);
        }

        mySlots.Clear();
    }

    // Coordinates the event order at startup
    private IEnumerator BoardValidation()
    {
        // 1 - Make Sure neighbours recognize themselves
        yield return StartCoroutine(DispatchNeighbourhoodCheck());

        // 2 - Check if any gem has two neighbours of the same kind
        yield return StartCoroutine(DispatchDetectPrematureMatch());
        
        // 3 - Check if the board is softlocked
        yield return StartCoroutine(DispatchSoftlockCheck());

        // 4 - Compare with passing condition
        yield return StartCoroutine(CheckPrematureMatch());

        // 5: Still not valid? Randomize again
        if (!canProceed)
        {
            yield return StartCoroutine(BoardReRandomize());

            StartCoroutine(BoardValidation());
        }
    }

    public IEnumerator OnMovePerformed()
    {

        // Are there matches in the board?
        yield return StartCoroutine(DispatchDetectMatch());

        
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(CheckMatch());

        if (moveWithMatch)
            playerController.moveLock = true;

        // In case there are no matches, we restore the board state
        if (playerController.moveLock)
        {
            yield return StartCoroutine(OnRightMove());
        }
        else
        {
            playerController.ReturnGems();
        }

        //Debug.Log("Movement completed");
    }


    public IEnumerator OnRightMove()
    {
        //In case there are matches. Check the extension of the match array
        yield return StartCoroutine(EnlistMatches());
        yield return StartCoroutine(DestroyGemsAndScore());

        yield return StartCoroutine(TriggerFallCascade(matchArray.Count + 2));


        if (matchArray.Count > 0)
        {
            matchArray.Clear();

            yield return StartCoroutine(OnMovePerformed());
        }
        else
        {
            cascading = false;
            playerController.ResetStatus();
            Debug.Log("all done");


            yield return StartCoroutine(DispatchSoftlockCheck());
            if (softlocked)
            {
                yield return StartCoroutine(BoardReRandomize());

                yield return StartCoroutine(BoardValidation());

            }

        }

    }

    public IEnumerator OnRewind()
    {
        // Are there matches in the board?
        yield return StartCoroutine(DispatchDetectMatch());


        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(CheckMatch());

        playerController.ResetStatus();

        //Debug.Log("Movement completed");
    }

    private IEnumerator CheckMatch()
    {
        int m = 0;
        for (int i = 0; i < mySlots.Count; i++)
        {
            if (mySlots[i].matched)
            {
                m++;
            }
        }

        if (m > 0)
            moveWithMatch = true;
        else
            moveWithMatch = false;

        yield return null;
    }

    private IEnumerator CheckPrematureMatch()
    {

        for (int i = 0; i < mySlots.Count; i++)
        {
            if (mySlots[i].matched)
            {
                prematureMatch = true;
            }
        }

        if (prematureMatch || softlocked)
        {
            canProceed = false;
        }
        else
        {
            canProceed = true;
        }
        yield return null;
    }

    private IEnumerator BoardReRandomize()
    {
        DispatchRerandomize();

        yield return null;
    }

    public Vector3 GetGemPosition(int x, int y)
    {
        return new Vector3(x * slotDistancing, y * -slotDistancing);
    }

    public IEnumerator DispatchNeighbourhoodCheck()
    {
        for (int i = 0; i < mySlots.Count; i++)
        {
            mySlots[i].ScanNeighbours();
        }

        yield return null;
    }

    public IEnumerator DispatchDetectPrematureMatch()
    {
        prematureMatch = false;
        
        for (int i = 0; i < mySlots.Count; i++)
        {
            mySlots[i].DetectPrematureMatch();            
        }

        yield return null;        
    }

    public IEnumerator DispatchDetectMatch()
    {
        for (int i = 0; i < mySlots.Count; i++)
        {
            mySlots[i].DetectMatch();
        }

        yield return null;
    }

    public IEnumerator EnlistMatches()
    {
        for (int i = 0; i < mySlots.Count; i++)
        {
            if (mySlots[i].matched == true)
            {
                if(!matchArray.Contains(mySlots[i]))
                matchArray.Add(mySlots[i]);
            }
        }

        yield return null;
    }

    public IEnumerator DestroyGemsAndScore()
    {
        for (int i = 0; i < matchArray.Count; i++)
        {
           StartCoroutine( matchArray[i].DestroyMyGem());
        }

        sessionManager.Score(matchArray.Count);

        yield return null;
    }

    public bool vacantSpacesBellow;
    public bool vacantSpaces;
    public IEnumerator TriggerFallCascade(int iterations)
    {
        for (int a = 0; a < iterations; a++)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < mySlots.Count; i++)
            {
                mySlots[i].CheckVacancyBellow();
            }


             yield return StartCoroutine(FillTheBlanks(iterations));

            yield return StartCoroutine(DispatchNeighbourhoodCheck());

            //yield return StartCoroutine(FillTheBlanks(1));

        }
       // yield return StartCoroutine(FillTheBlanks(iterations));
       // Debug.Log("fall completed");

        yield return null;
    }

    public IEnumerator FillTheBlanks(int iterations)
    {
        int prevInstance = 0;

        for (int r = 0; r < iterations; r++)
        {
            for (int i = 0; i < boardGrid; i++)
            {
                int curGem = Random.Range(0, stageDifficulty);

                while (curGem == prevInstance)
                {
                    curGem = Random.Range(0, stageDifficulty);
                }

                if (mySlots[i].noGem)
                {
                   mySlots[i].CreateGem(curGem);                    
                }
                
            }

            yield return new WaitForEndOfFrame();

            //ield return StartCoroutine(TriggerFallCascade(3));


        }


        //yield return StartCoroutine(TriggerFallCascade(2));

    }

    public void DispatchRerandomize()
    {
        for (int i = 0; i < mySlots.Count; i++)
        {
            mySlots[i].ReRandomizeGem(Random.Range(0,stageDifficulty));
        }

        DispatchNeighbourhoodCheck();
    }

    public void AddToArray(Board_Slot slot)
    {
        matchArray.Add(slot);
    }

    public IEnumerator DispatchSoftlockCheck()
    {
        bool locked = false;
        int lockedPieces = 0;

        for (int i = 0; i < mySlots.Count; i++)
        {
            if (mySlots[i].SoftLockCheck())
            {
                lockedPieces++;
                
            }
        }

        if (lockedPieces >= mySlots.Count)
        {
            Debug.Log("Soft Lock!");
            locked = true;
        }
        else
        {
            locked = false;
        }

        softlocked = locked;

        yield return null;
    }
}
