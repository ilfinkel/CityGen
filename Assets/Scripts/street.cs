using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Street : MonoBehaviour
{
    public int CountNo;
    public string Name;
    public List<roadChunk> RoadChunks = new List<roadChunk>();
    public float Length = 0f;
    public bool ToDelete = false;

    private bool isBlink = false;

    

    // Start is called before the first frame update
    void Start()
    {
        name = "street";
        RoadChunks = null;
    }

    // Update is called once per frame
    void Update()
    {
        Length = 0f;
        foreach (var roadChunk in RoadChunks)
        {
            Length += roadChunk.Length;
        }
        Blink();
    }

    public void Add(roadChunk _road)
    {
        RoadChunks.Add(_road);
        Length += (_road.End.position - _road.Begin.position).magnitude;
    }
    public void Insert0(roadChunk _road)
    {
        RoadChunks.Insert(0, _road);
        Length += (_road.End.position - _road.Begin.position).magnitude;
    }
    
    public void Blink(bool _isBlink = false)
    {
        isBlink = _isBlink;
        StartBlinking();
    }

    public void StartBlinking()
    {
        if (isBlink)
        {
            foreach (roadChunk road in RoadChunks)
            {
                SpriteRenderer sprite = road.transform.GetComponentInChildren<SpriteRenderer>();
                if (sprite != null)
                {
                    sprite.color = new Color(1, 0, 0, 1);
                }
            }
        }
        else
        {
            foreach (roadChunk road in RoadChunks)
            {
                SpriteRenderer sprite = road.transform.GetComponentInChildren<SpriteRenderer>();
                if (sprite != null)
                {
                    sprite.color = new Color(0, 0, 0, 1);
                }
            }
        }
    }
}
