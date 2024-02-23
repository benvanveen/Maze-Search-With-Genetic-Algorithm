using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeChanger : MonoBehaviour
{
    public float newSize;
    public float NScale;
    public float MScale;

    public float OffsetNScale;
    public float OffsetMScale;

    public GameObject MazeMaker;

    void Start(){
        MazeTrain scriptA = MazeMaker.GetComponent<MazeTrain>();
        transform.position = new Vector3(transform.position.x + OffsetMScale * scriptA.m, transform.position.y + OffsetNScale * scriptA.n,transform.position.z);
    }
    // Update is called once per frame
    void Update()
    {
        Camera mainCamera = GetComponent<Camera>();
        MazeTrain scriptA = MazeMaker.GetComponent<MazeTrain>();

        newSize = NScale*scriptA.n;

        if(newSize<MScale*scriptA.m){
            newSize = MScale*scriptA.m;
        }
        mainCamera.orthographicSize = newSize;
    }
}
