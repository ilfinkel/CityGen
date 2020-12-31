using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartPointClass2
{
    public StartPointClass2(){}
    public StartPointClass2(Vector3 _position, int _kind, List<Quaternion> _quat, double _koeff = 1f)
    {
        position = _position;
        kind = _kind;
        quat = _quat;
        sizeKoeff = _koeff;
    }
    public StartPointClass2(Vector3 _position, int _kind, Quaternion _quat, double _koeff = 1f)
    {
        position = _position;
        kind = _kind;
        quat.Add(_quat);
        sizeKoeff = _koeff;
    }
    public StartPointClass2(Vector3 _position, int _kind, double _koeff = 1f)
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

public class MapGen2 : MonoBehaviour
{
    public int MapSpawnCounter = 1;

    public Mesh seaMesh;

    public buildingChunk[] buildingChunks;
    public ChooseRoad chooseRoad = new ChooseRoad();
    public GameObject Map;
    public riverChunk[] riverChunks;
    public roadChunk[] roadChunks;
    public districtTest districtTest;
    public bool generateDistricts;

    private float riverUnit;
    private float roadUnit;
    private readonly List<buildingChunk> spawnedBuildingChunks = new List<buildingChunk>();
    private readonly List<riverChunk> spawnedRiverChunks = new List<riverChunk>();
    private readonly List<Street> spawnedStreets = new List<Street>();
    private readonly List<StartPointClass2> startingPoints = new List<StartPointClass2>();
    private int streetCounter;
    private int streetNo;
    private int riverCounter = 0;
    private float height;
    private float width;
    private double quarterDistance;
    private double quarterDistance1;

    // Start is called before the first frame update
    private void Start()
    {
        var rt = (RectTransform)Map.transform;
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

        for (int i = 0; i < MapSpawnCounter; i++)
        {
            SpawnRiver();
        }
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
        var rt = (RectTransform)Map.transform;
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
        riverCounter += 2;
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
                startingPoints.Add(new StartPointClass2(newRiverChunk.Right.position, 1, newRiverChunk.Right.rotation,
                    1f));

                startingPoints.Add(new StartPointClass2(newRiverChunk.Left.position, 1, newRiverChunk.Left.rotation,
                    1f));

                createCustomMapSimpleUnit(newRiverChunk.Right.position, newRiverChunk.Left.position, roadChunks[0], 2f);
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
            createCustomMapSimpleUnit(rChunk.End.position, hit.point, riverChunks[1], 1f);
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
        var rt = (RectTransform)Map.transform;
        var xMap = Map.transform.position.x;
        var yMap = Map.transform.position.y;

        var RoadQuat = Map.transform.rotation;
        var xPoints = 5f;
        var yPoints = 5f;
        var streetsFromPoint = 3;
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
                startingPoints.Add(new StartPointClass2(RoadBeg, 0));
            }
        }

        ///Рассчитываем построения изначальных дорог
        foreach (var startPoint in startingPoints)
        {
            List<StartPointClass2> otherPoints = startingPoints;

            //выбираем ближайшие точки
            List<Vector3> posList = new List<Vector3>();

            posList.Add(startPoint.position);
            otherPoints.Sort((v1, v2) =>
                (startPoint.position - v1.position).magnitude.CompareTo((startPoint.position - v2.position).magnitude));
            int streetsFrom = Random.Range(streetsFromPoint - 1, streetsFromPoint + 1);
            for (int i = 1, j = 0; j < streetsFrom && i < otherPoints.Count; i++)
            {
                // posList.Add(otherPoints[i].position);

                Quaternion q = new Quaternion();
                Vector3 vectorToTarget = otherPoints[i].position - startPoint.position;
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;

                q = Quaternion.AngleAxis(angle, Vector3.forward);
                float magn = (otherPoints[i].position - startPoint.position).magnitude;
                if (checkDistance(startPoint.position, q * Quaternion.Euler(0, 0, -90) * Vector3.up, magn) != true)
                {
                    continue;
                }

                ///я правда не знаю почему все дороги надо поворачивать на 90 градусов.
                startPoint.quat.Add(q * Quaternion.Euler(0, 0, -90));
                j++;

            }

            if (startPoint.kind == 1 && startPoint.quat.Count > 1)
            {
                startPoint.quat.RemoveAt(0);
            }
        }

        StartPointClass2 startPoint1 = findNearestStartingPoint(new Vector3(width / 2, height / 2), true);
        startPoint1.kind = 5;
        startPoint1.sizeKoeff = 2;

        foreach (var StartPointer in startingPoints)
        {
            foreach (var quat in StartPointer.quat)
            {
                streetCounter++;
                var _street = new Street();
                spawnedStreets.Add(_street);
                _street.Name = streetNo.ToString();
                streetNo++;
                GenerateRoad(StartPointer.position, quat, rt, _street, true, 0, 1.5f /*, true*/);
            }
        }
    }

    public void GenerateRoad(Vector3 _pos, Quaternion _rot, RectTransform rt, Street _street, bool _isStart = true,
        int _possibleCorner = 0, float _size = 1f, bool _loneRoad = false, float _time = 0.5f)
    {
        StartCoroutine(WaitRoad(_time, _pos, _rot, rt, _street, _size, _isStart, _possibleCorner, _loneRoad));
    }

    private IEnumerator WaitRoad(float waitTime, Vector3 _pos, Quaternion _rot, RectTransform rt, Street _street,
        float _size = 1f, bool _isStart = true, int _possibleCorner = 0, bool _loneRoad = false)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        GenerateRoadTile(_pos, _rot, rt, _street, _size, _isStart, _possibleCorner, _loneRoad);
    }



    private void GenerateRoadTile(Vector3 _pos, Quaternion _rot, RectTransform rt, Street _street, float _size = 1f,
        bool _toLeft = true, int _possibleCorner = 0, bool _loneRoad = false)
    {
        var _isThatEnd = false;
        string endReason = "";
        if (!checkStartingPointsToThis(_pos)) { _isThatEnd = true; endReason = "end"; }

        var newRoadChunk = roadChunks[0];
        newRoadChunk.name = "street" + _street.Name + " - " + (_street.RoadChunks.Count + 1);

        var rChunk = Instantiate(newRoadChunk, _pos, _rot, Map.transform);
        rChunk.RegionColor(checkStartingPoints(_pos));
        rChunk.ended = endReason;
        rChunk.streetNo = _street.CountNo;

        float roadscale = Random.Range(5, 9);

        var oldPos = rChunk.End.position;
        rChunk.transform.rotation *= Quaternion.Euler(0, 0, Random.Range(-_possibleCorner, _possibleCorner + 1));
        var newPos = rChunk.End.position;
        rChunk.transform.position += newPos - oldPos;

        float realRoadScale = checkForwardRoadChunk(rChunk, roadscale);

        if (realRoadScale == 0 || realRoadScale >= roadscale)
        {
            realRoadScale = roadscale;
        }
        else if (realRoadScale < roadscale)
        {

            realRoadScale += 0.5f;
            _isThatEnd = true;
        }

        rChunk.transform.position += (realRoadScale) * (rChunk.End.position - rChunk.transform.position);

        rChunk.transform.localScale = new Vector3
        (rChunk.transform.localScale.x * _size,
            rChunk.transform.localScale.y * (realRoadScale),
            rChunk.transform.localScale.z);

        rChunk.Length = (rChunk.End.position - rChunk.Begin.position).magnitude;

        if (_toLeft)
            _street.Insert0(rChunk);
        else
            _street.Add(rChunk);


        _rot = rChunk.End.rotation;
        _pos = rChunk.End.position;

        if (_isThatEnd || _pos.x < 0 || _pos.y < 0 || _pos.x > width || _pos.y > height)
        {
            streetCounter--;
            if (streetCounter == 0) GenerateHouses();
            return;
        }

        GenerateRoad(_pos, _rot, rt, _street, _toLeft, _possibleCorner, _size, _loneRoad, 0.2f);

        if (checkDistance(_pos, _rot * Quaternion.Euler(0, 0, -90) * Vector3.up, 9) == false)
        {
            return;
        }
        
        var _newStreet = new Street();
        spawnedStreets.Add(_newStreet);
        _newStreet.Name = streetNo.ToString();
        _newStreet.CountNo = streetNo;


        streetNo++;
        bool isLeft = false;
        bool isRight = false;
        if (Random.Range(0, 10) < 3)
        {
            isLeft = true;
            streetCounter++;
            GenerateRoad(_pos, _rot * Quaternion.Euler(0, 0, 90), rt, _newStreet, true, _possibleCorner + 2, 1f, _loneRoad,
                0.9f);
        }

        if (Random.Range(0, 10) < 2)
        {
            isRight = true;
            streetCounter++;
            GenerateRoad(_pos, _rot * Quaternion.Euler(0, 0, -90), rt, _newStreet, false, _possibleCorner + 2,
                1f, _loneRoad, 0.9f);
        }
        if(isLeft || isRight)
        {
            rChunk.GeneratedStreets.Add(Tuple.Create(rChunk.End.position, _newStreet));
            int a = 0;
        }

    }

    private float checkForwardRoadChunk(roadChunk _roadChunk, float _roadscale, Street _street = null)
    {
        var hit = Physics2D.Raycast((_roadChunk.Begin.position), _roadChunk.End.up);
        var hitDistance = hit.distance;
        if (hit.distance < roadUnit / 2)
        {
            hit = Physics2D.Raycast((_roadChunk.transform.position), _roadChunk.End.up);
            hitDistance = hit.distance;
        }
        else
        {
            hitDistance = (hit.distance - (roadUnit / 2));
        }
        if (hitDistance != 0 && (hitDistance / roadUnit) < _roadscale && hit.collider.gameObject.tag == "road")
        {
            _roadChunk.ended = "";
            hit.collider.gameObject.GetComponent<roadChunk>().ended = "";
            Vector3 pointOfHit = hit.point;
            _roadChunk.GeneratedStreets.Add(Tuple.Create(pointOfHit, spawnedStreets[hit.collider.gameObject.GetComponent<roadChunk>().streetNo]));
            hit.collider.gameObject.GetComponent<roadChunk>().GeneratedStreets.Add(Tuple.Create(pointOfHit, spawnedStreets[_roadChunk.streetNo]));
        }
        return hitDistance / roadUnit;
    }

    private bool checkDistance(Vector3 _pos, Vector3 _dir, float magn = 10000000f)
    {
        var hit = Physics2D.Raycast(_pos, _dir);
        if (hit.distance == 0 || hit.distance >= magn * 7 / 8)
            return true;
        return false;
    }

    private void GenerateHouses()
    {
        riverChunks[0].transform.localScale = new Vector3(100, 100, 1);

        roadChunks[0].transform.localScale = new Vector3(10, 10, 1);

        //foreach (var street in spawnedStreets)
        //{
        //    if (street.Length / roadUnit <= 5 || street.ToDelete)
        //    {
        //        foreach (var chunk in street.RoadChunks) Destroy(chunk.gameObject);
        //        street.RoadChunks.Clear();
        //    }
        //}

        for (int a = spawnedStreets.Count - 1; a >= 0; a--)
        {
            var street = spawnedStreets[a];
            if (street.RoadChunks.Count == 0) continue;
            if (street.RoadChunks[0].ended == "end")
            {
                int count1 = 0;
                for (int i = 0; i < street.RoadChunks.Count; i++)
                {
                    //if ((street.RoadChunks[i].GeneratedStreets.Count != 0 &&
                    //    street.RoadChunks[i].GeneratedStreet.RoadChunks.Count != 0) ||
                    //    street.RoadChunks[i].isEndOfOther) { break; }
                    if (street.RoadChunks[i].GeneratedStreets != null && street.RoadChunks[i].GeneratedStreets.Count != 0)
                    {
                        int counterOfStreets = 0;
                        foreach (var GeneratedStreet in street.RoadChunks[i].GeneratedStreets)
                        {
                            counterOfStreets += GeneratedStreet.Item2.RoadChunks.Count;
                        }
                        if (counterOfStreets > 0)
                            break;
                    }

                    Destroy(street.RoadChunks[i].gameObject);
                    count1++;
                }
                street.RoadChunks.RemoveRange(0, count1);
            }
            if (street.RoadChunks.Count > 0)
            {
                if (street.RoadChunks[street.RoadChunks.Count - 1].ended == "end")
                {
                    int count2 = 0;
                    for (int i = street.RoadChunks.Count - 1; i >= 0; i--)
                    {
                        //if ((street.RoadChunks[i].GeneratedStreet != null &&
                        //    street.RoadChunks[i].GeneratedStreet.RoadChunks.Count != 0) ||
                        //    street.RoadChunks[i].isEndOfOther == true) { break; }
                        if (street.RoadChunks[i].GeneratedStreets != null && street.RoadChunks[i].GeneratedStreets.Count != 0)
                        {
                            int counterOfStreets = 0;
                            foreach (var GeneratedStreet in street.RoadChunks[i].GeneratedStreets)
                            {
                                counterOfStreets += GeneratedStreet.Item2.RoadChunks.Count;
                            }
                            if (counterOfStreets > 0)
                                break;

                        }
                        Destroy(street.RoadChunks[i].gameObject);
                        count2++;
                    }
                    street.RoadChunks.RemoveRange(street.RoadChunks.Count - count2, count2);
                }
            }
        }

        foreach (var street in spawnedStreets)
        {
            if (street.Length / roadUnit <= 5 || street.ToDelete)
            {
                foreach (var chunk in street.RoadChunks) Destroy(chunk.gameObject);
                street.RoadChunks.Clear();
            }
        }


        //     var street = spawnedStreets[i];
        //
        //     if (street.RoadChunks.Count < 6)
        //     {
        //         foreach (var chunk in street.RoadChunks) Destroy(chunk.gameObject);
        //         street.RoadChunks.Clear();
        //     }
        //
        //     // foreach (var chunk in street.RoadChunks)
        //     //     if (checkStartingPoints(chunk.transform.position) != -1)
        //     //     {
        //     //         var buildingChunk = buildingChunks[0];
        //     //         int a = Random.Range(0, 5), b = Random.Range(0, 5);
        //     //         if (a < 3)
        //     //         {
        //     //             if (checkLeftRoadChunk(chunk))
        //     //             {
        //     //                 /*buildingChunk.transform.localScale = new Vector3(10, 10, 1);
        //     //                 buildingChunk.transform.localScale = new Vector3(Random.Range(500, 2000) / 100,
        //     //                     Random.Range(500, 2000) / 100, 1);*/
        //     //                 var rot = chunk.Right.rotation;
        //     //                 var pos = chunk.transform.position +
        //     //                           2 * (chunk.Left.position - chunk.transform.position);
        //     //                 var bChunk = Instantiate(buildingChunk, pos, rot, Map.transform);
        //     //                 bChunk.name = "building" + spawnedBuildingChunks.Count;
        //     //                 spawnedBuildingChunks.Add(bChunk);
        //     //             }
        //     //         }
        //     //
        //     //         if (b < 3)
        //     //         {
        //     //             if (checkRightRoadChunk(chunk))
        //     //             {
        //     //                 /*buildingChunk.transform.localScale = new Vector3(10, 10, 1);
        //     //                 buildingChunk.transform.localScale = new Vector3(Random.Range(500, 2000) / 100,
        //     //                     Random.Range(500, 2000) / 100, 1);*/
        //     //                 var rot = chunk.Left.rotation;
        //     //                 var pos = chunk.transform.position +
        //     //                           2 * (chunk.Right.position - chunk.transform.position);
        //     //                 var bChunk = Instantiate(buildingChunk, pos, rot, Map.transform);
        //     //                 bChunk.name = "building" + spawnedBuildingChunks.Count;
        //     //                 spawnedBuildingChunks.Add(bChunk);
        //     //             }
        //     //         }
        //     //     }
        // }
        spawnedStreets.RemoveAll(item => item.RoadChunks.Count == 0);
        //chooseRoad.UpdateSources();
        if (generateDistricts)
        {
            CreateTestDistricts();
            //CreateDistricts();
        }
    }


    private bool checkStartingPointsToThis(Vector3 point)
    {
        //int status = -1;
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

    private StartPointClass2 findNearestStartingPoint(Vector3 point, bool _withNoKind = false)
    {
        double min_value = quarterDistance;
        StartPointClass2 returnPoint = startingPoints[0];
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


    private void createCustomMapSimpleUnit(Vector3 _pos1, Vector3 _pos2, MapSimpleUnits _mapUnit, float _width = 1f, float _unitHeight = 1f)
    {
        var newChunk = _mapUnit;
        float length = (_mapUnit.Begin.position - _mapUnit.End.position).magnitude;

        Quaternion q = new Quaternion();
        Vector3 vectorToTarget = _pos1 - _pos2;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;

        q = Quaternion.AngleAxis(angle, Vector3.forward);

        var rChunk = Instantiate(newChunk, (_pos1 + _pos2) / 2, q * Quaternion.Euler(0, 0, -90));
        //rChunk.RegionColor(checkStartingPoints(_pos1));

        rChunk.transform.localScale = new Vector3
        (rChunk.transform.localScale.x * _width,
            rChunk.transform.localScale.y * (((_pos1 - _pos2).magnitude / length) / _unitHeight),
            rChunk.transform.localScale.z);
    }
    void CreateDistricts()
    {
        StartCoroutine(AnotherStreetDistr());
    }

    private IEnumerator AnotherStreetDistr()
    {
        
        foreach (var spawnedStreet in spawnedStreets)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            foreach (roadChunk road in spawnedStreet.RoadChunks)
            {

                DistrictObject district = new DistrictObject();
                var hit = Physics2D.Raycast(road.Begin.position, road.Left.rotation * Vector3.up, 24 * roadUnit);
                var hit2 = Physics2D.Raycast(road.End.position, road.Left.rotation * Vector3.up, 24 * roadUnit);

                //var hit = Physics.Raycast(road.Left.position, road.Left.rotation * Quaternion.Euler(0, 0, -90) * Vector3.up, 6 * roadUnit);
                if (hit && hit2)
                    district.setDistrict(road.Begin.position, road.End.position, hit.point, hit2.point, true, 24 * roadUnit);
                DistrictObject district2 = new DistrictObject();
                hit = Physics2D.Raycast(road.Begin.position, road.Right.rotation * Vector3.up);
                hit2 = Physics2D.Raycast(road.End.position, road.Right.rotation * Vector3.up);
                if (checkDistance(hit.point, hit2.point))
                    if (hit && hit2)
                        district2.setDistrict(road.Begin.position, road.End.position, hit.point, hit2.point, false, 24 * roadUnit);
            }
        }
    }
    void CreateTestDistricts()
    {
        StartCoroutine(AnotherStreetTestDistr());
    }

    private IEnumerator AnotherStreetTestDistr()
    {

        List<Tuple<int, Vector3, Vector2>> distObj = new List<Tuple<int, Vector3, Vector2>>();
        List<districtTest> distList = new List<districtTest>();
        foreach (var spawnedStreet in spawnedStreets)
        {
            foreach (roadChunk road in spawnedStreet.RoadChunks)
            {
                districtTest distObject2 = districtTest;
                float maxDistance = 40 * roadUnit;
                
                //DistrictObject district = new DistrictObject();
                var hit11 = Physics2D.Raycast(road.transform.position, road.Left.rotation * Vector3.up, maxDistance);
                var hit12 = Physics2D.Raycast(road.End.position, road.Left.rotation * Vector3.up, maxDistance);
                var hit13 = Physics2D.Raycast(road.Begin.position, road.Left.rotation * Vector3.up, maxDistance);
                RaycastHit2D hit = hit13;

                if (hit11.distance > hit12.distance && hit11.distance > hit13.distance) hit = hit11;
                else if (hit12.distance > hit11.distance && hit12.distance > hit13.distance) hit = hit12;

                int kind = 100;
                if (hit)
                {
                    float maxMagn = hit.distance;
                    if (hit.collider.tag == "river") { kind = 0; }
                    else if (maxMagn <= 3 * roadUnit) { kind = 1; }
                    else if (maxMagn <= maxDistance / 3) { kind = 2; }
                    else if (maxMagn <= maxDistance * 2 / 3) { kind = 3; }
                    else { kind = 4; }

                    var hitLeftEnd = Physics2D.Raycast(road.LeftOut.position, road.End.rotation * Vector3.up, maxDistance / 2);
                    var hitLeftBegin = Physics2D.Raycast(road.LeftOut.position, road.Begin.rotation * Vector3.up, maxDistance / 2);
                    if (hitLeftEnd) { distObj.Add(Tuple.Create(kind, road.LeftOut.position, hitLeftEnd.point)); }
                    if (hitLeftBegin) { distObj.Add(Tuple.Create(kind, road.LeftOut.position, hitLeftBegin.point)); }
                    distObj.Add(Tuple.Create(kind, road.LeftOut.position, hit.point));

                    //createCustomMapSimpleUnit(road.transform.position, hit.point, distObject);
                }

                var hit21 = Physics2D.Raycast(road.transform.position, road.Right.rotation * Vector3.up, maxDistance);
                var hit22 = Physics2D.Raycast(road.End.position, road.Right.rotation * Vector3.up, maxDistance);
                var hit23 = Physics2D.Raycast(road.Begin.position, road.Right.rotation * Vector3.up, maxDistance);
                RaycastHit2D hit2 = hit23;

                if (hit21.distance > hit22.distance && hit21.distance > hit23.distance) hit2 = hit21;
                else if (hit22.distance > hit21.distance && hit22.distance > hit23.distance) hit2 = hit22;
                if (hit2)
                {
                    float maxMagn = hit2.distance;
                    if (hit2.collider.tag == "river") { kind = 0; }
                    else if (maxMagn <= 1 * roadUnit) { kind = 1; }
                    else if (maxMagn <= maxDistance / 3) { kind = 2; }
                    else if (maxMagn <= maxDistance * 2 / 3) { kind = 3; }
                    else { kind = 3; }

                    var hitRightEnd = Physics2D.Raycast(road.RightOut.position, road.End.rotation * Vector3.up, maxDistance / 2);
                    var hitRightBegin = Physics2D.Raycast(road.RightOut.position, road.Begin.rotation * Vector3.up, maxDistance / 2);
                    if (hitRightEnd) { distObj.Add(Tuple.Create(kind, road.RightOut.position, hitRightEnd.point)); }
                    if (hitRightBegin) { distObj.Add(Tuple.Create(kind, road.RightOut.position, hitRightBegin.point)); }
                    distObj.Add(Tuple.Create(kind, road.RightOut.position, hit2.point));
                    //createCustomMapSimpleUnit(road.transform.position, hit2.point, distObject);
                }

            }
        }

        yield return new WaitForSecondsRealtime(0.01f);
        foreach (var dist in distObj)
        {
            districtTest distObject = districtTest;
            distObject.kind = dist.Item1;
            distList.Add(distObject);
            createCustomMapSimpleUnit(dist.Item2, dist.Item3, distObject);
        }

    }
}