using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour{
    [SerializeField]
    Vector3 roatationSpeed = new Vector3(0, 0, 1);

    void Update(){
        transform.Rotate(roatationSpeed * Time.deltaTime);
    }
}
