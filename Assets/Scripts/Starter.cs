using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Starter : MonoBehaviour
{
    public InputField X;
    public InputField Y;
    public MapGen mapGen = new MapGen();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        mapGen.GenerateMap(X.text, Y.text);
    }
}
