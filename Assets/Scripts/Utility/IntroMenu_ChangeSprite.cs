using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IntroMenu_ChangeSprite : MonoBehaviour
{
    public int currStep;
    public Sprite[] allSprites;
    public Image mySprite;
    public void Hop()
    {
        if (currStep < allSprites.Length - 1)
        {
            currStep++;
        }
        else
        {
            currStep = 0;
        }

        mySprite.sprite = allSprites[currStep];

    }
}
