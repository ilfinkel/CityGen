using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class districtTest : MapSimpleUnits
{
    public int kind = 10;
    public int districtNum = int.MaxValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int otherKind = collision.GetComponent<districtTest>().kind;
        int otherDistrictNum = collision.GetComponent<districtTest>().districtNum;
        if (collision.tag == "districtTest")
        {
            if (otherKind < kind) { kind = otherKind; }
            if (otherDistrictNum < districtNum) { districtNum = otherDistrictNum; }
        }

        SpriteRenderer sprite = transform.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            switch (kind)
            {
                case 0:
                    sprite.color = new Color(0.3f, 0.3f, 1, 1);
                    break;
                case 1:
                    sprite.color = new Color(0, 1f, 0, 1);
                    break;
                case 2:
                    sprite.color = new Color(0.7f, 0.3f, 0.7f, 1);
                    break;
                case 3:
                    sprite.color = new Color(0.9f, 0.9f, 0, 1);
                    break;
                case 4:
                    sprite.color = new Color(0.5f, 1f, 0.5f, 1);
                    break;
                default:
                    sprite.color = new Color(0, 0, 0, 1);
                    break;
            }
        }
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    int otherKind = collision.GetComponent<districtTest>().kind;
    //    if (collision.tag == "districtTest")
    //        if (otherKind < kind) 
    //        { kind = otherKind; }

    //    SpriteRenderer sprite = transform.GetComponentInChildren<SpriteRenderer>();
    //    if (sprite != null)
    //    {
    //        switch (kind)
    //        {
    //            case 0:
    //                sprite.color = new Color(0.3f, 0.3f, 1, 1);
    //                break;
    //            case 1:
    //                sprite.color = new Color(0, 1f, 0, 1);
    //                break;
    //            case 2:
    //                sprite.color = new Color(0.7f, 0.3f, 0.7f, 1);
    //                break;
    //            case 3:
    //                sprite.color = new Color(0.9f, 0.9f, 0, 1);
    //                break;
    //            case 4:
    //                sprite.color = new Color(0.5f, 1f, 0.5f, 1);
    //                break;
    //            default:
    //                sprite.color = new Color(0, 0, 0, 1);
    //                break;
    //        }
    //    }
    //}
    public void SetKindColor()
    {
        
    }
}
