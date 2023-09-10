using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 input;
    [SerializeField]
    float moveSpeed = 3.5f;
    Vector3 displacement;
    new Rigidbody rigidbody;

    [SerializeField]
    Transform cameraPosition;

    Vector3 forward;
    Vector3 right;
    
    [SerializeField]
    float rotSpeed = 20f;

    void Start(){
        rigidbody = GetComponent<Rigidbody>();
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Movement()){
            rigidbody.MovePosition(transform.position + displacement*moveSpeed*Time.deltaTime);
            UpdateRotation();
        }
        if(Aiming()){
            Debug.Log("Test");
        }
    }

    bool Movement(){
        // Determine forward and right relative to camera rotation
        forward = cameraPosition.localRotation * Vector3.forward;
        forward = new Vector3(forward.x, 0, forward.z);
        forward = Vector3.Normalize(forward);
        right = Vector3.Cross(Vector3.up, forward);
        //Get inputs
        input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Forward")
        );
        input = Vector2.ClampMagnitude(input, 1f);
        const float minInput = 0.001f;
        if (input.x < -minInput || input.x > minInput || input.y < -minInput || input.y > minInput){
            displacement = right * input.x + forward * input.y;
            return true;
        }
        return false;
    }

    void UpdateRotation(){
        Quaternion finalRot = Quaternion.LookRotation(displacement);
        transform.rotation = Quaternion.Lerp(transform.rotation, finalRot, Time.deltaTime * rotSpeed);
    }

    bool Aiming(){
        return Input.GetKey(KeyCode.Mouse1);
    }
}
