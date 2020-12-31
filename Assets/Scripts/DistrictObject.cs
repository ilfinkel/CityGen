using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
public class DistrictObject : MonoBehaviour
{
    //public Material material;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    int type = 15;
    GameObject gameObject;

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Stay");
        //mesh.Clear();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enter");
        //mesh.Clear();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        //mesh.Clear();
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Stay");
        //mesh.Clear();
        Destroy(gameObject);
    }


    public void setDistrict(Vector3 _pos1, Vector3 _pos2, Vector3 _pos3, Vector3 _pos4, bool _isLeft, float _maxDistance)
    {
        mesh = new Mesh();

        vertices = new Vector3[] { _pos1, _pos2, _pos3, _pos4 };
        if (_isLeft) { triangles = new int[] { 0, 2, 1, 3, 1, 2 }; }
        else { triangles = new int[] { 1, 3, 2, 2, 0, 1 }; }

        float maxMagn = (_pos1 - _pos2).magnitude > (_pos3 - _pos4).magnitude ? (_pos1 - _pos2).magnitude : (_pos3 - _pos4).magnitude;
        Color color = Color.green;
        if (maxMagn<= _maxDistance / 3) { color = Color.green; type = 1; }
        else if(maxMagn <= _maxDistance *2 / 3) { color = Color.blue; type = 2; }
        else  { color = Color.red; type = 3; }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody), typeof(MeshCollider));

        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.GetComponent<MeshCollider>().convex = true;
        gameObject.GetComponent<MeshCollider>().isTrigger = true;


        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material.color = color;
    }
}
