using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;


public class GameController : MonoBehaviour
{
    Vector3[] locations;
    public GameObject hex;

    void Start() {
        locations = MapUtil.HexagonalLattice(Vector3.zero, 9, 3, 0.5f);
        foreach(Vector3 l in locations) {
            GameObject g = GameObject.Instantiate(hex, l, Quaternion.Euler(0, 30, 0));
            g.transform.localScale = new Vector3(3,0,3);
            
            Color c = Random.ColorHSV();

            Vector3[] points = MapUtil.HexagonFromPoint(new Vector2(l.x, l.z), 3);
            Material mat = new Material(Shader.Find("Specular"));
            mat.color = c;
            foreach(Vector3 p in points) {
                GameObject point = GameObject.CreatePrimitive(PrimitiveType.Cube);
                point.transform.position = p;
                point.GetComponent<MeshRenderer>().material = mat;
    
            }
        }
    }


    void OnDrawGizmos()
    {
        if(locations == null) {
            return;
        }

        foreach(Vector3 l in locations)
        {
            Gizmos.DrawSphere(l, 0.3f);
        }
    }
}