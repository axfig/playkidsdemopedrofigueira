using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board_Slot : MonoBehaviour
{

    [Header("Main Properties")]
    public int gemCode;
    public Sprite[] gemSprites;
    public GameObject gemPrefab;
    public Board_Gem myGem;

    [Header("Neighbourhood")]
    public Neighbourhood neighbourhood;
    public bool corneredUp, corneredDown, corneredLeft, corneredRight;

    //hidden variables
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Board_Slot mySlot;
    private Player_Controller playerController;
    private Board_Manager boardManager;


    [Header("Keys")]
    public bool topSlot;
    public bool matched;
    public bool locked;
    public bool noGem;
    public bool vacancyBellow;

    private void Awake()
    {
        mySlot = GetComponent<Board_Slot>();
        playerController = FindObjectOfType<Player_Controller>();
        boardManager = FindObjectOfType<Board_Manager>();
    }

    private void Update()
    {
    }

    #region Interaction

    public void PointerDown()
    {
        if(!playerController.moveLock)
        { 
            playerController.GemSelected(mySlot);
            animator.SetTrigger("Select");


            AudioController.Audio_PlaySound(1);
        }
    }

    public void PointerDragOver()
    {
        if (!playerController.moveLock)
        {
            if (playerController.draggingGem && playerController.currentSlot != mySlot)
            {
                if (AmINeighbourTo(playerController.currentSlot))
                {
                    playerController.targetSlot = mySlot;
                    animator.SetTrigger("Select");

                    AudioController.Audio_PlaySound(1);
                }
            }
        }
    }

    public void PointerExit()
    {
        if (playerController.draggingGem && playerController.currentSlot != mySlot )
        {
            if (!playerController.moveLock)
            animator.SetTrigger("Release");
        }
    }

    public void PointerUp()
    {
        if(!playerController.moveLock)
        animator.SetTrigger("Release");

    }

    #endregion


    #region BoardDispatch
    public void ScanNeighbours()
    {
        
        if (transform.childCount == 0)
        {
            noGem = true;
        }        

        neighbourhood.nUp = ScanDirection(transform.up);
        neighbourhood.nDown = ScanDirection(transform.up * -1);
        neighbourhood.nLeft = ScanDirection(transform.right * -1);
        neighbourhood.nRight = ScanDirection(transform.right);
        
        CheckCornering();

        if (vacancyBellow)
        {
            //Debug.Log("vacant bellow " + gameObject.name);
            SwapGemWith(neighbourhood.nDown);

            if (topSlot)
            {
                // CreateGem(Random.Range(0, boardManager.stageDifficulty));
            }

        }



    }

    public void DetectPrematureMatch()
    {
        ScanNeighbours(); 

        if (!corneredUp || !corneredDown)
        {
            if (neighbourhood.nUp.gemCode == gemCode && neighbourhood.nDown.gemCode == gemCode && !corneredUp && !corneredDown)
            {
                matched = true;
                //Debug.Log("Match vertical @ " + transform.name);
                return;
            }
            else
            {
                matched = false;
            }
        }


        if (!corneredLeft || !corneredRight)
        {
            if (neighbourhood.nLeft.gemCode == gemCode && neighbourhood.nRight.gemCode == gemCode && !corneredLeft && !corneredRight)
            {
                matched = true;
                //Debug.Log("Match horizontal @ " + transform.name);
                return;
            }
            else
            {
                matched = false;
            }
        }

        //ScanNeighbours();
    }

    public void DetectMatch()
    {
        ScanNeighbours();

        
        //matched = false;


            if (neighbourhood.nUp.gemCode == gemCode && neighbourhood.nDown.gemCode == gemCode && !corneredUp && !corneredDown)
            {
                matched = true;
                //Debug.Log("Match vertical @ " + transform.name);
                TriggerMatchCascade(0, gemCode);
                TriggerMatchCascade(1, gemCode);
            }
        


            if (neighbourhood.nLeft.gemCode == gemCode && neighbourhood.nRight.gemCode == gemCode && !corneredLeft && !corneredRight)
            {
                matched = true;
               // Debug.Log("Match horizontal @ " + transform.name);
                TriggerMatchCascade(2, gemCode);
                TriggerMatchCascade(3, gemCode);

            }
        

  
    }

    public void ReRandomizeGem(int gemID)
    {
        SetGemProperties(gemID);
    }

    /* In order to avoid a softlock, a board requires two criteria:
         * 1: make sure at least one piece has a similar gem
         * 2: make sure this piece's neighbours have a similar gem towards the same direction
         * By default, every corner piece will return as softlocked. Further treatment should be provided to avoid unecessary processing
        */

    public int[] surroundingGems = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
    public bool SoftLockCheck()
    {

        for (int i = 0; i < surroundingGems.Length; i++)
        {
            surroundingGems[i] = 0;
        }

            // Softlock Style 01: two matches w/o any no third possibility
            bool thisPieceIsSoftLocked = true;

        if (neighbourhood.nUp.gemCode == gemCode  )
        {

            if (!corneredUp && !corneredDown)
            {
                thisPieceIsSoftLocked = neighbourhood.nDown.SoftlockNetSweep(gemCode, 0);

                locked = thisPieceIsSoftLocked;
                return thisPieceIsSoftLocked;
            }
        }

        if (neighbourhood.nDown.gemCode == gemCode )
        {

            if (!corneredUp && !corneredDown)
            {
                thisPieceIsSoftLocked = neighbourhood.nUp.SoftlockNetSweep(gemCode, 1);

                locked = thisPieceIsSoftLocked;
                return thisPieceIsSoftLocked;
            }
        }

        if (neighbourhood.nLeft.gemCode == gemCode)
        {

            if (!corneredRight && !corneredLeft)
            {
                thisPieceIsSoftLocked = neighbourhood.nRight.SoftlockNetSweep(gemCode, 2);

                locked = thisPieceIsSoftLocked;
                return thisPieceIsSoftLocked;
            }
        }

        if (neighbourhood.nRight.gemCode == gemCode)
        {
            if (!corneredRight && !corneredLeft)
            {
                thisPieceIsSoftLocked = neighbourhood.nLeft.SoftlockNetSweep(gemCode, 3);

                locked = thisPieceIsSoftLocked;
                return thisPieceIsSoftLocked;
            }
        }

        // softlock style 2: surround check

        if(!corneredUp)
        surroundingGems[neighbourhood.nUp.gemCode]++;
        if(!corneredDown)
        surroundingGems[neighbourhood.nDown.gemCode]++;
        if(!corneredLeft)
        surroundingGems[neighbourhood.nLeft.gemCode]++;
        if(!corneredRight)
        surroundingGems[neighbourhood.nRight.gemCode]++;

        for (int i = 0; i < surroundingGems.Length; i++)
        {
            if (surroundingGems[i] > 2)
            {
                thisPieceIsSoftLocked = false;

                locked = thisPieceIsSoftLocked;
                return thisPieceIsSoftLocked;
            }
        }

        //Debug.Log("Found locked piece @ " + transform.name)
        locked = thisPieceIsSoftLocked;
        return thisPieceIsSoftLocked;
        
    }

    // This sweep is only performed by gems with two neighbours
    // Ignore variable added to avoid a neighbour checking back on it's dispatching neighbour
    public bool SoftlockNetSweep(int referenceCode,int ignore)
    {
        bool noNeighboursMatchSearch = true;

        if (neighbourhood.nUp.gemCode == referenceCode && !corneredUp && ignore != 0)
        {
            noNeighboursMatchSearch = false;
            return noNeighboursMatchSearch;
        }
        else if (neighbourhood.nDown.gemCode == referenceCode && !corneredDown && ignore != 1)
        {
            noNeighboursMatchSearch = false;
            return noNeighboursMatchSearch;
        }
        else if (neighbourhood.nLeft.gemCode == referenceCode && !corneredLeft && ignore != 2)
        {

            noNeighboursMatchSearch = false;
            return noNeighboursMatchSearch;
        }
        else if (neighbourhood.nRight.gemCode == referenceCode && !corneredRight && ignore != 3)
        {
            noNeighboursMatchSearch = false;
            return noNeighboursMatchSearch;
        }


        return noNeighboursMatchSearch;
    }

    #endregion

    #region GemSwapping

    public void SwapGemWith(Board_Slot targetSlot)
    {
        if (myGem != null)
        {
            myGem.transform.parent = targetSlot.transform;
            myGem.GoToParent();
            targetSlot.ReadGemProperties();
            //targetSlot.ScanNeighbours();
            myGem = null;
        }
        //noGem = false;
    }

    #endregion

    #region Scan

    public void CheckVacancyBellow()
    {
        if (!corneredDown)
        {
            if (neighbourhood.nDown.noGem)
            {
                vacancyBellow = true;

                //  SwapGemWith(neighbourhood.nDown);

                //   neighbourhood.nDown.noGem = false;

                ScanNeighbours();

            }
            else
            {
                vacancyBellow = false;
            }
            
        }
    }

    public void TriggerMatchCascade(int mode,int refCode)
    {


        switch (mode)
        {
            case 0:                
                if (gemCode == refCode)
                {
                    matched = true;

                    if (!corneredUp)
                        neighbourhood.nUp.TriggerMatchCascade(0, refCode);

                   
                }
                break;
            case 1:               
                
                if (gemCode == refCode)
                {
                    matched = true;

                    if (!corneredDown)
                        neighbourhood.nDown.TriggerMatchCascade(1, refCode);

                   
                }

                break;
            case 2:
                if (gemCode == refCode)
                {
                    matched = true;

                    if (!corneredLeft)
                        neighbourhood.nLeft.TriggerMatchCascade(2, refCode);

                   
                }

                break;
            case 3:

                if (gemCode == refCode)
                {
                    matched = true;

                    if (!corneredRight)
                        neighbourhood.nRight.TriggerMatchCascade(3, refCode);

                    
                }

                break;                
        }
    }


    public IEnumerator DestroyMyGem()
    {
        if (myGem != null)
        {
            matched = false;
            myGem.OnExplode();
            noGem = true;

            yield return new WaitForEndOfFrame();


            myGem = null;


            AudioController.Audio_PlaySound(2);
        }
        else
        {
            ReadGemProperties();
        }
    }

    #endregion

    // used only for the first Gem instance
    public void CreateGem(int gemID)
    {
        // Create Gem
        GameObject gemGO = Instantiate(gemPrefab, transform);
        gemGO.transform.localPosition = Vector3.zero;

        myGem = gemGO.GetComponent<Board_Gem>();
        //myGem.gemID = gemID;

        // Read Properties
        ReadGemProperties();

        // Set Gem Properties
        SetGemProperties(gemID);
    }


    public void ReadGemProperties()
    {
        if (transform.childCount > 0)
        {
            noGem = false;
       

        myGem = transform.GetChild(0).GetComponent<Board_Gem>();

        animator        = myGem.GetComponent<Animator>();
        spriteRenderer  = myGem.GetComponent<SpriteRenderer>();
        myGem           = myGem.GetComponent<Board_Gem>();

        gemCode = myGem.gemID ;

        if (myGem.transform.parent != transform)
            Debug.LogError("Gem Parent discrepancy!");
        }
    }

    private void SetGemProperties(int gemID)
    {
        gemCode = gemID;
        spriteRenderer.sprite = gemSprites[gemID];
        myGem.gemID = gemID;
    }

    // No neighbour == self to avoid null references
    private Board_Slot ScanDirection(Vector2 dir)
    {
        transform.GetComponent<BoxCollider2D>().enabled = false;

        Ray ray = new Ray(transform.position, dir);
        RaycastHit2D hit = Physics2D.Raycast( ray.origin,ray.direction );


        transform.GetComponent<BoxCollider2D>().enabled = true;

        if (hit.transform == null)
        {

            return GetComponent<Board_Slot>();
        }
        else
        {
            return hit.transform.GetComponent<Board_Slot>();
        }
    }

    

    private void CheckCornering()
    {
        corneredUp      = neighbourhood.nUp == mySlot ? true : false;
        corneredDown    = neighbourhood.nDown== mySlot ? true : false;
        corneredLeft    = neighbourhood.nLeft == mySlot ? true : false;
        corneredRight   = neighbourhood.nRight == mySlot ? true : false;
    }

    private bool AmINeighbourTo(Board_Slot slot)
    {
        bool amI = false;

        if (slot.neighbourhood.nUp == mySlot) 
        amI = true;
        
        if (slot.neighbourhood.nDown == mySlot) 
        amI = true;

        if (slot.neighbourhood.nLeft == mySlot) 
        amI = true;

        if (slot.neighbourhood.nRight == mySlot) 
        amI = true;

        return amI;
    }


    [System.Serializable]
    public class Neighbourhood
    {
        public Board_Slot nUp, nDown, nLeft, nRight;     

    }
}
