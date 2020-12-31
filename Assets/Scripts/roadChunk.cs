using System;
using System.Collections.Generic;
using UnityEngine;

public class roadChunk : MapSimpleUnits
{
    public Transform RightOut;
    public Transform LeftOut;
    public int streetNo;
    public List<Tuple<Vector3, Street>> GeneratedStreets = new List<Tuple<Vector3, Street>>();
    public string ended;
    //public bool isEndOfOther = false;
    public float Length = 0f;
    


    void Start()
    {
        Length = (End.position - Begin.position).magnitude;
    }

    private void Update()
    {
        Length = (End.position - Begin.position).magnitude;
        SpriteRenderer sprite = transform.GetComponentInChildren<SpriteRenderer>();
        if (ended == "end") { sprite.color = new Color(1, 0, 1, 1); }
        //if (isEndOfOther == true) { sprite.color = new Color(0, 1, 0, 1); }
    }
    public void RegionColor(int _regionNum)
    {
        SpriteRenderer sprite = transform.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            switch (_regionNum)
            {
              case 1:
                   sprite.color = new Color(0, 0, 1, 1);
                    break;
                //case 5:
                //    sprite.color = new Color(1, 0, 1, 1);
                //    break;
                default:
                    sprite.color = new Color(0, 0, 0, 1);
                    break;
            }
            
        }
    }


}


