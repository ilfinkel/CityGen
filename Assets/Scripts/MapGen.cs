using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartPointClass
{
    public StartPointClass(){}
    public StartPointClass(Vector3 _position, int _kind, List<Quaternion> _quat, double _koeff = 1f)
    {
        position = _position;
        kind = _kind;
        quat = _quat;
        sizeKoeff = _koeff;
    }
    public StartPointClass(Vector3 _position, int _kind, Quaternion _quat, double _koeff = 1f)
    {
        position = _position;
        kind = _kind;
        quat.Add(_quat);
        sizeKoeff = _koeff;
    }
    public StartPointClass(Vector3 _position, int _kind, double _koeff = 1f)
    {
        position = _position;
        kind = _kind;
        sizeKoeff = _koeff;
    }
    public Vector3 position;
    public int kind;
    public List<Quaternion> quat = new List<Quaternion>();
    public double sizeKoeff;
}

//using UnityEngine.UI;

public class MapGen : MonoBehaviour
{
    public Mesh seaMesh;
    
    public buildingChunk[] buildingChunks;
    public ChooseRoad chooseRoad = new ChooseRoad();
    private float height;
    public GameObject Map;
    private double quarterDistance;
    private double quarterDistance1;
    public riverChunk[] riverChunks;

    private int riverCounter = 2;
    private float riverUnit;

    public roadChunk[] roadChunks;
    private float roadUnit;

    private readonly List<buildingChunk> spawnedBuildingChunks = new List<buildingChunk>();

    //private Vector3 riverScale;
    private readonly List<riverChunk> spawnedRiverChunks = new List<riverChunk>();
    private readonly List<Street> spawnedStreets = new List<Street>();

    private readonly List<StartPointClass> startingPoints = new List<StartPointClass>();

    private int streetCounter;

    private float width;

    // Start is called before the first frame update
    private void Start()
    {
        var rt = (RectTransform) Map.transform;
        width = rt.rect.width;
        height = rt.rect.height;
        var xMap = Map.transform.position.x;
        var yMap = Map.transform.position.y;

        quarterDistance1 = Math.Sqrt(xMap * xMap + yMap * yMap) / 4;
        quarterDistance = Math.Sqrt(xMap * xMap + yMap * yMap) / 5;
        riverUnit = (riverChunks[0].End.localPosition.y - riverChunks[0].Begin.localPosition.y) *
                    riverChunks[0].transform.localScale.y;
        roadUnit = (roadChunks[0].End.localPosition.y - roadChunks[0].Begin.localPosition.y) *
                   roadChunks[0].transform.localScale.y;
        //riverScale = roadChunks[0].transform.localScale;
        SpawnRiver();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public List<Street> GetStreets()
    {
        return spawnedStreets;
    }

    public void GenerateMap(string X, string Y)
    {
        Map.GetComponent<RectTransform>().sizeDelta = new Vector2(int.Parse(X), int.Parse(Y));
    }

    private void SpawnRiver()
    {
        var rt = (RectTransform) Map.transform;
        var xMap = Map.transform.position.x;
        var yMap = Map.transform.position.y;

        var xPlace = Random.Range(width * (1 / 3), width * ((1 / 3)));
        var yPlace = Random.Range(height * (1 / 3), height * (2 / 3));
        var riverBeg = new Vector3(xPlace, yPlace);
        xPlace = Random.Range(Convert.ToSingle(quarterDistance1), Convert.ToSingle(width - quarterDistance1));
        yPlace = Random.Range(Convert.ToSingle(quarterDistance1), Convert.ToSingle(height - quarterDistance1));
        riverBeg = new Vector3(xPlace, yPlace);
        var riverQuat = Map.transform.rotation * Quaternion.Euler(0, 0, Random.Range(0, 91));
        GenerateRiver(riverBeg, riverQuat, rt, riverChunks[0].transform.localScale);

        GenerateRiver(riverBeg, riverQuat * Quaternion.Euler(0, 0, 180), rt, riverChunks[0].transform.localScale);
    }

    public void GenerateRiver(Vector3 _pos, Quaternion _rot, RectTransform rt, Vector3 localScale)
    {
        StartCoroutine(WaitFor(0.01f, _pos, _rot, rt, localScale));
    }

    private IEnumerator WaitFor(float waitTime, Vector3 _pos, Quaternion _rot, RectTransform rt, Vector3 localScale)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        var rChunk = chooseRiverChunk();

        GenerateRiverTile(_pos, _rot, rt, 15, localScale);

        float b = Random.Range(0, 100);
        if (b > 95)
        {
            riverChunk rChunk2;
            rChunk2 = chooseRiverChunk();
            yield return new WaitForSecondsRealtime(waitTime);
            riverCounter++;
            GenerateRiverTile(_pos, _rot, rt, 15, localScale);
        }
    }

    public void GenerateRiverTile(Vector3 _pos, Quaternion _rot, RectTransform rt, int _possibleCorner,
        Vector3 localScale)
    {
        var newRiverChunk = riverChunks[0];
        newRiverChunk.name = "river" + spawnedRiverChunks.Count;


        newRiverChunk.transform.localScale = new Vector3
        (localScale.x * Random.Range(90, 111) / 100,
            localScale.y,
            transform.localScale.z);

        localScale = newRiverChunk.transform.localScale;

        var rChunk = Instantiate(newRiverChunk, _pos, _rot, Map.transform);
        spawnedRiverChunks.Add(rChunk);

        if (!checkRiverChunk(rChunk))
        {
            riverCounter--;
            if (riverCounter == 0) spawnRiverOrRoad();
            return;
        }
        newRiverChunk.transform.position = _pos;
        newRiverChunk.transform.rotation = _rot;
        if (!(_pos.x < quarterDistance1
              || _pos.y < quarterDistance1
              || _pos.x > width - quarterDistance1
              || _pos.y > height - quarterDistance1))
        {
            float b = Random.Range(0, 100);
            if (b > 75)
            {
                startingPoints.Add(new StartPointClass(newRiverChunk.Right.position, 1, newRiverChunk.Right.rotation, 1f));

                startingPoints.Add(new StartPointClass(newRiverChunk.Left.position, 1, newRiverChunk.Left.rotation, 1f));
            }
        }

        var oldPos = newRiverChunk.End.position;
        newRiverChunk.transform.rotation *= Quaternion.Euler(0, 0, Random.Range(-_possibleCorner, _possibleCorner + 1));
        var newPos = newRiverChunk.End.position;
        newRiverChunk.transform.position += oldPos - newPos;

        _rot = newRiverChunk.End.rotation;
        _pos = newRiverChunk.transform.position +
               2 * (newRiverChunk.End.position - newRiverChunk.transform.position);

        if (_pos.x < 0 || _pos.y < 0 || _pos.x > width || _pos.y > height)
        {
            riverCounter--;
            if (riverCounter == 0) spawnRiverOrRoad();
            return;
        }

        GenerateRiver(_pos, _rot, rt, localScale);
    }

    private riverChunk chooseRiverChunk()
    {
        var newRiverChunk = riverChunks[0];
        return newRiverChunk;
    }

    private bool checkRiverChunk(riverChunk rChunk)
    {
        var hit = Physics2D.Raycast(rChunk.End.position, rChunk.End.up);
        if (hit.distance > 0.15f * riverUnit && hit.distance < 1.25f * riverUnit && hit.transform.tag == "river")
        {
            var rChunkLocal = riverChunks[1];
            rChunkLocal.transform.localScale = rChunk.transform.localScale;
            rChunkLocal.transform.position = rChunk.End.position +
                                             (new Vector3(hit.point.x, hit.point.y, 0) - rChunk.End.position) / 2;
            rChunkLocal.transform.rotation = rChunk.transform.rotation;
            riverChunks[1].transform.localScale = new Vector3
            (rChunkLocal.transform.localScale.x,
                (new Vector3(hit.point.x, hit.point.y, 0) - rChunk.End.position).magnitude, // * riverUnit,
                rChunkLocal.transform.localScale.z);
            var rChunk2 = Instantiate(riverChunks[1], rChunkLocal.transform.position, rChunkLocal.transform.rotation,
                Map.transform);
            return false;
        }

        return true;
    }

    private void spawnRiverOrRoad()
    {
        //if (spawnedRiverChunks.Count < ((width + height) / riverUnit) * 2)
        //{
        //    realStartingPoints.Clear();
        //    startingPoints.Clear();
        //    foreach (var chunk in spawnedRiverChunks)
        //    {
        //        Destroy(chunk.gameObject);
        //    }
        //    spawnedRiverChunks.Clear();
        //    spawnRiver();
        //}
        //else
        //{
        spawnRoad();
        //}
    }

    //=================================================
    private void spawnRoad()
    {
        var rt = (RectTransform) Map.transform;
        var xMap = Map.transform.position.x;
        var yMap = Map.transform.position.y;

        var RoadQuat = Map.transform.rotation;
        var xPoints = 5f;
        var yPoints = 5f;
        var streetsFromPoint = 4;
        var l = 0;
        Vector3 RoadBeg;
        var neededTryes = 250;
        while (l < neededTryes)
        {
            l = 0;

            var xPlace = Random.Range(width * (1 / xPoints), width * ((xPoints - 1) / xPoints));
            var yPlace = Random.Range(height * (1 / yPoints), height * ((yPoints - 1) / yPoints));
            RoadBeg = new Vector3(xPlace, yPlace);
            while (checkStartingPointsToThis(RoadBeg))
            {
                xPlace = Random.Range(Convert.ToSingle(quarterDistance1), Convert.ToSingle(width - quarterDistance1));
                yPlace = Random.Range(Convert.ToSingle(quarterDistance1), Convert.ToSingle(height - quarterDistance1));
                RoadBeg = new Vector3(xPlace, yPlace);
                l++;
                if (l >= neededTryes) break;
            }

            if (l < neededTryes)
            {
                startingPoints.Add(new StartPointClass(RoadBeg, 0));
            }
        }
        
        ///Рассчитываем построения изначальных дорог
        foreach (var startPoint in startingPoints)
        {
            // if (startPoint.quat.Count == 0)
            // {
                List<StartPointClass> otherPoints = startingPoints;
                
                //выбираем ближайшие точки
                List<Vector3> posList = new List<Vector3>();
                //posList.Add(startPoint.position);
                otherPoints.Sort((v1,v2)=>(startPoint.position - v1.position).magnitude.CompareTo((startPoint.position - v2.position).magnitude));
                int streetsFrom = Random.Range(streetsFromPoint - 1, streetsFromPoint + 1);
                //если две точки наодятся рядом друг с другом(например, 2 стороны реки), эта точка не тянется к более дальней
                for (int i= 1, j=0; j < streetsFrom && i < otherPoints.Count; i++)
                {
                    bool isNear = false;
                    foreach (var point1 in posList)
                    {
                        if ((point1 - otherPoints[i].position).magnitude < quarterDistance)
                        {
                            isNear = true;
                        }
                    }
                    if (isNear==true) continue;

                    posList.Add(otherPoints[i].position);
                    
                    
                    Quaternion q = new Quaternion();
                    Vector3 vectorToTarget = otherPoints[i].position - startPoint.position;
                    float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                    
                    q = Quaternion.AngleAxis(angle, Vector3.forward);
                    
                    if (startPoint.kind == 1)
                    {
                        Vector3 vectorToTarget2 = otherPoints[i].position - startPoint.position;
                        float angle2 = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                        if (Math.Abs(Mathf.DeltaAngle(angle, angle2)) >= 90)
                        {
                            continue;
                        }
                    }

                    ///я правда не знаю почему все дороги надо поворачивать на 90 градусов.
                    startPoint.quat.Add(q*Quaternion.Euler(0,0,-90));
                    j++;

                }

                if (startPoint.kind == 1 && startPoint.quat.Count > 1)
                {
                    startPoint.quat.RemoveAt(0);
                }

                
        }
        


        StartPointClass startPoint1 = findNearestStartingPoint(new Vector3(width / 2, height / 2), true);
        startPoint1.kind = 5;
        startPoint1.sizeKoeff = 2;

        foreach (var StartPointer in startingPoints)
        {
            foreach (var quat in StartPointer.quat)
            {
                streetCounter++;
                var _street = new Street();
                spawnedStreets.Add(_street);
                _street.Name = spawnedStreets.Count.ToString();
                GenerateRoad(StartPointer.position, quat, rt, _street, true, 0/*, true*/);
            }
        }
    }

    public void GenerateRoad(Vector3 _pos, Quaternion _rot, RectTransform rt, Street _street, bool _isStart = true,
        int _possibleCorner = 0, bool _loneRoad = false, int _counterOfCross = 0)
    {
        StartCoroutine(WaitFor2(0.04f, _pos, _rot, rt, _street, _isStart, _possibleCorner, _loneRoad, _counterOfCross));
    }

    private IEnumerator WaitFor2(float waitTime, Vector3 _pos, Quaternion _rot, RectTransform rt, Street _street,
        bool _isStart = true, int _possibleCorner = 0, bool _loneRoad = false, int _counterOfCross = 0)
    {
        yield return new WaitForSecondsRealtime(waitTime);

        var isCross = chooseRoadChunk(1, _counterOfCross);

        if (isCross && !_loneRoad)
            GenerateCrossTile(_pos, _rot, rt, _street, _isStart, _possibleCorner, _loneRoad);
        else
            GenerateRoadTile(_pos, _rot, rt, _street, _isStart, _possibleCorner, _loneRoad, _counterOfCross);
    }

    private bool chooseRoadChunk(int _region, int counterOfCross = 0)
    {
        var crossPossibility = 15;

        var isCross = false;
        var b = Random.Range(0, crossPossibility);
        if (b == 0 && counterOfCross > 5)
            isCross = true;
        else
            isCross = false;
        return isCross;
    }

    private void GenerateRoadTile(Vector3 _pos, Quaternion _rot, RectTransform rt, Street _street, bool _toLeft = true,
        int _possibleCorner = 0, bool _loneRoad = false, int counterOfCross = 0)
    {
        var _isThatEnd = false;
        if (!checkStartingPointsToThis(_pos)) _isThatEnd = true;
        
        counterOfCross++;
        var newRoadChunk = roadChunks[0];

        var newRoadChunkPointer = Instantiate(newRoadChunk, _pos, _rot, Map.transform);
            newRoadChunkPointer.RegionColor(checkStartingPoints(_pos));
        if (_toLeft)
            _street.RoadChunks.Insert(0, newRoadChunkPointer);
        else
            _street.RoadChunks.Add(newRoadChunkPointer);
        newRoadChunk.name = "Street" + _street.Name + ", Road" + _street.RoadChunks.Count;

        newRoadChunk.transform.position = _pos;
        newRoadChunk.transform.rotation = _rot;

        var oldPos = newRoadChunk.End.position;

        if (Random.Range(0, 100) < 100 / (_possibleCorner + 1))
            newRoadChunk.transform.rotation *=
                Quaternion.Euler(0, 0, Random.Range(-_possibleCorner, _possibleCorner + 1));
        var newPos = newRoadChunk.End.position;
        newRoadChunk.transform.position += oldPos - newPos;

        if (checkForwardRoadChunk(newRoadChunk, _street))
        {
            var rot = newRoadChunk.End.rotation;
            var pos = newRoadChunk.transform.position +
                      2 * (newRoadChunk.End.position - newRoadChunk.transform.position);
            if (pos.x > 0 && pos.y > 0 && pos.x < width && pos.y < height)
            {
                if (_loneRoad)
                {
                    
                    if (_isThatEnd)
                    {
                        GenerateRoad(pos, rot, rt, _street, _toLeft, _possibleCorner, true, counterOfCross);
                    }
                    else
                    {
                        GenerateRoad(pos, rot, rt, _street, _toLeft, _possibleCorner, false, counterOfCross);
                        //Debug.Log(streetCounter.ToString() + " -Street" + _street.Name);
                        //return;
                    }
                }
                else
                {
                    if (!_isThatEnd)
                    {
                        GenerateRoad(pos, rot, rt, _street, _toLeft, _possibleCorner, _loneRoad, counterOfCross);
                    }
                    else
                    {
                        if (Random.Range(0, 10) < 3)
                        {
                            GenerateRoad(pos, rot, rt, _street, _toLeft, _possibleCorner, true, counterOfCross);
                        }
                        else
                        {
                            streetCounter--;
                            return;
                        }
                    }
                }
            }
            else
            {
                streetCounter--;
                //Debug.Log(streetCounter.ToString() + " -Street" + _street.Name);
            }
        }
        else
        {
            streetCounter--;
            //Debug.Log(streetCounter.ToString() + " -Street" + _street.Name);
        }

        if (streetCounter == 0) GenerateHouses();
    }

    private void GenerateCrossTile(Vector3 _pos, Quaternion _rot, RectTransform rt, Street _street,
        bool _isStart = true, int _possibleCorner = 0, bool _loneRoad = false)
    {
        var _isThatEnd = false;
        if (checkStartingPoints(_pos) == -1)
            _isThatEnd = true;
        //return;
        if (_isThatEnd)
        {
            streetCounter--;
            //Debug.Log(streetCounter.ToString() + " -Street" + _street.Name);
        }
        else
        {
            var newRoadChunk = roadChunks[0];
            var newRoadChunkPointer = Instantiate(newRoadChunk, _pos, _rot, Map.transform);
            if (_isStart)
                _street.RoadChunks.Insert(0, newRoadChunkPointer);
            else
                _street.RoadChunks.Add(newRoadChunkPointer);
            newRoadChunk.name = "Street" + _street.Name + ", Road" + _street.RoadChunks.Count;

            if (checkForwardRoadChunk(newRoadChunk, _street))
            {
                newRoadChunk.transform.position = _pos;
                newRoadChunk.transform.rotation = _rot;
                var rot = newRoadChunk.End.rotation;
                var pos = newRoadChunk.transform.position +
                          2 * (newRoadChunk.End.position - newRoadChunk.transform.position);
                if (pos.x > 0 && pos.y > 0 && pos.x < width && pos.y < height)
                    GenerateRoadTile(pos, rot, rt, _street, false, _possibleCorner, _loneRoad);
                else
                    streetCounter--;
                //Debug.Log(streetCounter.ToString() + " -Street" + _street);
            }
            else
            {
                streetCounter--;
                //Debug.Log(streetCounter.ToString() + " -Street " + _street);
            }
            var _newStreet = new Street();
            spawnedStreets.Add(_newStreet);
            _newStreet.Name = spawnedStreets.Count.ToString();
            
            if(Random.Range(1,5)<2)
             if (checkRightRoadChunk(newRoadChunk))
             {
                 streetCounter++;
                 //Debug.Log(streetCounter.ToString() + " +Street " + _newStreet.Name);
                 newRoadChunk.transform.position = _pos;
                 newRoadChunk.transform.rotation = _rot;
                 var rot = newRoadChunk.Right.rotation;
                 var pos = newRoadChunk.Right.position;
                           // + 2 * (newRoadChunk.Right.position - newRoadChunk.transform.position);
                 if (pos.x > 0 && pos.y > 0 && pos.x < width && pos.y < height)
                     GenerateRoadTile(pos, rot, rt, _newStreet, false, _possibleCorner+2);
                 else
                     streetCounter--;
                 //Debug.Log(streetCounter.ToString() + " -Street" + _newStreet.Name);
             }

            if(Random.Range(1,5)<4)
            if (checkLeftRoadChunk(newRoadChunk))
            {
                streetCounter++;
                //Debug.Log(streetCounter.ToString() + " +Street " + _newStreet.Name);
                newRoadChunk.transform.position = _pos;
                newRoadChunk.transform.rotation = _rot;
                var rot = newRoadChunk.Left.rotation;
                var pos = newRoadChunk.Left.position;
                          // + 2 * (newRoadChunk.Left.position - newRoadChunk.transform.position);
                if (pos.x > 0 && pos.y > 0 && pos.x < width && pos.y < height)
                    GenerateRoadTile(pos, rot, rt, _newStreet, true, _possibleCorner+2);
                else
                    streetCounter--;
                //Debug.Log(streetCounter.ToString() + " -Street" + _newStreet.Name);
            }
        }

        if (streetCounter == 0) GenerateHouses();
    }

    // private bool checkForwardRoadChunk(roadChunk rChunk, Street _street)
    // {
    //     var hit = Physics2D.Raycast(rChunk.End.position, rChunk.transform.up);
    //     if (hit.distance > 0 /*.4 * roadUnit*/ && hit.distance < /*1.5 * */roadUnit)
    //     {
    //         if (hit.transform.tag == "road")
    //         {
    //             var rChunkLocal = roadChunks[1];
    //             rChunkLocal.transform.position = rChunk.End.position +
    //                                              (new Vector3(hit.point.x, hit.point.y, 0) - rChunk.End.position) / 2;
    //             rChunkLocal.transform.rotation = rChunk.transform.rotation;
    //             roadChunks[1].transform.localScale = new Vector3
    //             (rChunkLocal.transform.localScale.x,
    //                 (new Vector3(hit.point.x, hit.point.y, 0) - rChunk.End.position).magnitude * roadUnit,
    //                 rChunkLocal.transform.localScale.z);
    //             var rChunk2 = Instantiate(roadChunks[1], rChunkLocal.transform.position, rChunkLocal.transform.rotation,
    //                 Map.transform);
    //             _street.RoadChunks.Add(rChunk2);
    //         }
    //
    //         return false;
    //     }
    //
    //     return true;
    // }

    private bool checkForwardRoadChunk(roadChunk rChunk, Street _street)
    {
        var hit = Physics2D.Raycast(rChunk.transform.position, rChunk.transform.up);
        if (hit.distance > 0 && hit.distance < roadUnit) return false;
        return true;
    }
    
    private bool checkRightRoadChunk(roadChunk rChunk)
    {
        var hit = Physics2D.Raycast(rChunk.Right.position, rChunk.Right.up);
        if (hit.distance > 0 && hit.distance < 1.5 * roadUnit) return false;
        // hit = Physics2D.Raycast(rChunk.Begin.position, rChunk.Right.up);
        // if (hit.distance > 0 && hit.distance < 1.5 * roadUnit) return false;
        // hit = Physics2D.Raycast(rChunk.End.position, rChunk.Right.up);
        // if (hit.distance > 0 && hit.distance < 1.5 * roadUnit) return false;
        return true;
    }

    private bool checkLeftRoadChunk(roadChunk rChunk)
    {
        var hit = Physics2D.Raycast(rChunk.Left.position, rChunk.Left.up);
        if (hit.distance > 0 && hit.distance < 1.5 * roadUnit) return false;
        // hit = Physics2D.Raycast(rChunk.Begin.position, rChunk.Left.up);
        // if (hit.distance > 0 && hit.distance < 1.5 * roadUnit) return false;
        // hit = Physics2D.Raycast(rChunk.End.position, rChunk.Left.up);
        // if (hit.distance > 0 && hit.distance < 1.5 * roadUnit) return false;
        return true;
    }

    private void GenerateHouses()
    {
        riverChunks[0].transform.localScale = new Vector3(100, 100, 1);
        riverChunks[1].transform.localScale = new Vector3(100, 100, 1);

        for (var i = 0; i < spawnedStreets.Count; i++)
        {
            var street = spawnedStreets[i];

            if (street.RoadChunks.Count < 6)
            {
                foreach (var chunk in street.RoadChunks) Destroy(chunk.gameObject);
                street.RoadChunks.Clear();
            }

            // foreach (var chunk in street.RoadChunks)
            //     if (checkStartingPoints(chunk.transform.position) != -1)
            //     {
            //         var buildingChunk = buildingChunks[0];
            //         int a = Random.Range(0, 5), b = Random.Range(0, 5);
            //         if (a < 3)
            //         {
            //             if (checkLeftRoadChunk(chunk))
            //             {
            //                 /*buildingChunk.transform.localScale = new Vector3(10, 10, 1);
            //                 buildingChunk.transform.localScale = new Vector3(Random.Range(500, 2000) / 100,
            //                     Random.Range(500, 2000) / 100, 1);*/
            //                 var rot = chunk.Right.rotation;
            //                 var pos = chunk.transform.position +
            //                           2 * (chunk.Left.position - chunk.transform.position);
            //                 var bChunk = Instantiate(buildingChunk, pos, rot, Map.transform);
            //                 bChunk.name = "building" + spawnedBuildingChunks.Count;
            //                 spawnedBuildingChunks.Add(bChunk);
            //             }
            //         }
            //
            //         if (b < 3)
            //         {
            //             if (checkRightRoadChunk(chunk))
            //             {
            //                 /*buildingChunk.transform.localScale = new Vector3(10, 10, 1);
            //                 buildingChunk.transform.localScale = new Vector3(Random.Range(500, 2000) / 100,
            //                     Random.Range(500, 2000) / 100, 1);*/
            //                 var rot = chunk.Left.rotation;
            //                 var pos = chunk.transform.position +
            //                           2 * (chunk.Right.position - chunk.transform.position);
            //                 var bChunk = Instantiate(buildingChunk, pos, rot, Map.transform);
            //                 bChunk.name = "building" + spawnedBuildingChunks.Count;
            //                 spawnedBuildingChunks.Add(bChunk);
            //             }
            //         }
            //     }
        }
        spawnedStreets.RemoveAll(item => item.RoadChunks.Count == 0);
        //chooseRoad.UpdateSources();
    }


    private bool checkStartingPointsToThis(Vector3 point)
    {
        int status = -1;
        double min_value = quarterDistance;
        foreach (var startPoint in startingPoints)
        {
            if ((startPoint.position - point).magnitude < min_value)
            {
                return true;
            }
        }

        return false;
    }
    
    private int checkStartingPoints(Vector3 point)
    {
        int status = -1;
        double min_value = 0;
        foreach (var startPoint in startingPoints)
            if (startPoint.sizeKoeff / (startPoint.position - point).magnitude > min_value)
            {
                min_value = startPoint.sizeKoeff / (startPoint.position - point).magnitude;
                status = startPoint.kind;
            }
        return status;
    }

    private StartPointClass findNearestStartingPoint(Vector3 point, bool _withNoKind = false)
    {
        double min_value = quarterDistance;
        StartPointClass returnPoint = startingPoints[0];
        foreach (var startPoint in startingPoints)
            {
                if (_withNoKind && startPoint.kind != 0)
                {
                    continue;
                }

                if ((startPoint.position - point).magnitude * (1 / startPoint.sizeKoeff) < min_value)
                {
                    min_value = (startPoint.position - point).magnitude;
                    returnPoint = startPoint;
                }
            }
        return returnPoint;
    }

    private List<StartPointClass> findNearestStartingPoints(Vector3 point, int num)
    {
        List<StartPointClass> list1 = startingPoints;
        List<StartPointClass> list2 = new List<StartPointClass>();
        for (int i = 0; i < num; i++)
        {
            double min_value = Double.MaxValue;
            StartPointClass returnPoint = startingPoints[0];
            foreach (var startPoint in list1)
            {
                if (startPoint.position == point)
                {
                    continue;
                }

                bool isTrue = false;
                foreach (var startPoint2 in list2)
                {
                    if (startPoint.position == startPoint2.position)
                    {
                        isTrue = true;
                        break;
                    }
                }

                if (isTrue) continue;


                if ((startPoint.position - point).magnitude < min_value)
                {
                    min_value = (startPoint.position - point).magnitude;
                    returnPoint = startPoint;
                }
            }

            list2.Add(returnPoint);
        }

        return list2;
    }
}