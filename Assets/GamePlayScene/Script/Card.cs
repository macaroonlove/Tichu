using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private RectTransform rect;
    public int CardNum;

    public bool SCard = false;

    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (SCard)
        {
            rect.position = new Vector2(rect.position.x, 52f);
        }
        else
        {
            rect.position = new Vector2(rect.position.x, 12f);
        }
    }

    public void SelectCard()
    {
        if (!SCard)
        {
            SCard = true;
            CardManager.betCardNum++;
            CardManager.isSelectCard = true;
        }
        else if (SCard)
        {
            SCard = false;
            CardManager.betCardNum--;
            CardManager.isSelectCard = true;
        }
            
    }
}
