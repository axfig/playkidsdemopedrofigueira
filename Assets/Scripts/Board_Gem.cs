using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board_Gem : MonoBehaviour
{
    public int gemID;
    public float speed;
    public bool goingToParent;
    public GameObject explosion;


    private Board_Manager boardManager;
    private Player_Controller playerController;
    
    private void Awake()
    {
        boardManager = FindObjectOfType<Board_Manager>();
        playerController = FindObjectOfType<Player_Controller>();
        
    }

    public void Update()
    {
        if (goingToParent)
        {
            AnimateMovement();
        }
    }

    private void AnimateMovement()
    {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, Vector2.zero, Time.deltaTime * speed);

        if (Vector2.Distance(transform.localPosition, Vector2.zero) == 0)
        {
            goingToParent = false;
            OnReachParent();
        }
    }

    public void OnExplode()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnReachParent()
    { 
        
    }

    public void GoToParent()
    {
        goingToParent = true;
    }
}
