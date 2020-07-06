using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{

    public Board_Slot currentSlot;
    public Board_Slot targetSlot;
    public bool draggingGem;

    public bool moveLock;
    private Board_Manager boardManager;

    private Board_Slot movedGem01,movedGem02;


    private void Awake()
    {
        boardManager = FindObjectOfType<Board_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0) && !moveLock)
        {
            PointerUp();
        }      
    }

    public void PointerUp()
    {
        
        if (targetSlot != null && currentSlot != null && !moveLock)
        {
            targetSlot.PointerUp();
            currentSlot.PointerUp();
            StartCoroutine(PerformSwap());
        }

        draggingGem = false;

    }

    private IEnumerator PerformSwap()
    {

        AudioController.Audio_PlaySound(0);

        movedGem01 = currentSlot;
        movedGem02 = targetSlot;

        currentSlot.SwapGemWith(targetSlot);
        targetSlot.SwapGemWith(currentSlot);


        currentSlot.ReadGemProperties();
        targetSlot.ReadGemProperties();

        while (currentSlot.myGem.goingToParent || targetSlot.myGem.goingToParent)
        {
            yield return new WaitForEndOfFrame();
        }        
        
        yield return StartCoroutine(boardManager.OnMovePerformed());
        
        yield return null;

    }

    private IEnumerator RewindSwap()
    {

        AudioController.Audio_PlaySound(0);

        currentSlot.SwapGemWith(targetSlot);
        targetSlot.SwapGemWith(currentSlot);

        currentSlot.ReadGemProperties();
        targetSlot.ReadGemProperties();

        yield return StartCoroutine(boardManager.OnRewind());

        yield return null;

    }

    public void ReturnGems()
    {
        
        currentSlot = movedGem01;
        targetSlot = movedGem02;

        StartCoroutine(RewindSwap());


        AudioController.Audio_PlaySound(0);
    }

    public void GemSelected(Board_Slot slot)
    {
        if (!moveLock)
        {
            draggingGem = true;
            currentSlot = slot;

        }
    }

    public void ResetStatus()
    {
        moveLock = false;
        currentSlot = null;
        targetSlot = null;
        movedGem01 = null;
        movedGem02 = null;
    }




}
