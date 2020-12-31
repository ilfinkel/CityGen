using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject Map;
    private float height;
    private float width;
    // Start is called before the first frame update
    void Start()
    {
        var rt = (RectTransform) Map.transform;
        width = rt.rect.width;
        height = rt.rect.height;
        transform.position = Map.transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y, 
            transform.position.z - Convert.ToSingle(Math.Sqrt(width*width+height*height)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
