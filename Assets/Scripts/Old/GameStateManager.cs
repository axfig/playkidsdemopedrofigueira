using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class GameStateManager : MonoBehaviour
{

    public bool draggingJewel;

    [Header("Public Components")]
    public Jewel currentSelection;
    public Jewel targetSelection;
    public Physics2DRaycaster raycaster;


    //Private Components
    private Jewel recentlyMovedTarget, recentlyMovedSelection;
    private BoardSpace savedTargetBoard, savedSelectionBoard ;

    #region DefaultFunctions

    private void Awake()
    {
        SetComponents();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnDragStart();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            OnDragEnd();
        }

        if (draggingJewel)
        {
            OnDrag();
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene(0);
        }

    }

    #endregion

    #region StateManagers


    void OnDragStart()
    {
        if (!draggingJewel)
        {
            Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(Ray.origin, Ray.direction);

            if (hit.collider != null)
            {
                if (hit.transform.tag == "jewel")
                {

                    currentSelection = hit.transform.GetComponent<Jewel>();
                
                    draggingJewel = true;
                }

            }
        }
       
    }

    void OnDrag()
    {
        Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(Ray.origin, Ray.direction);

        if (hit.collider != null)
        {
            if (hit.transform.tag == "jewel" && hit.transform != currentSelection.transform )
            {                           

                targetSelection = hit.transform.GetComponent<Jewel>();            

            }
        }

    }


    void OnDragEnd()
    {
        if (targetSelection != null)
        {
            for (int i = 0; i < currentSelection.boardSpace.neighbourSpaces.Length;i++)
            {
                if (currentSelection.boardSpace.neighbourSpaces[i] == targetSelection.boardSpace)
                {
                    Debug.Log("hey neighbour!");
                    // Store in case there is no match
                    recentlyMovedSelection = currentSelection;
                    recentlyMovedTarget = targetSelection;
                    savedTargetBoard = targetSelection.boardSpace;
                    savedSelectionBoard = currentSelection.boardSpace;

                    currentSelection.OnMove(targetSelection.boardSpace);
                    targetSelection.OnMove(currentSelection.boardSpace);
                }
            }
        }

        draggingJewel = false;
    }

    public void NoMatchRecovery()
    {
        recentlyMovedSelection.OnMove(savedSelectionBoard);
        recentlyMovedTarget.OnMove(savedTargetBoard);
    }


    #endregion

    #region Utility



    private Vector2 GetTouchPosition(Vector2 mousePos)
    {
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void SetComponents()
    {
    }

    #endregion


}
