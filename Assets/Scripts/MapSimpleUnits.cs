using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSimpleUnits : MonoBehaviour
{
    public Transform Begin;
    public Transform End;
    public Transform Right;
    public Transform Left;

    private Color myColor;
    void Start()
    {
        myColor = transform.GetComponentInChildren<SpriteRenderer>().color;
    }
    
    void OnMouseEnter()
    {
        SpriteRenderer sprite = transform.GetComponentInChildren<SpriteRenderer>();
        myColor = transform.GetComponentInChildren<SpriteRenderer>().color;
        if (sprite != null)
        {
            sprite.color = new Color(1, 0, 0, 1);
        }
        Debug.Log(name);
    }

    void OnMouseExit()
    {
        SpriteRenderer sprite = transform.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = myColor;
        }
        Debug.Log(name);
    }
}

