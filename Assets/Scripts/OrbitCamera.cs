using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera:MonoBehaviour{
    [SerializeField]
    Transform focus = default;

    [Range(1f,20f)]
    public float distance = 5f, shoulderOffset = 2f;

    Vector3 focusPoint, previousFocusPoint;

    Vector2 orbitAngles = new Vector2(0f, 0f);
    [SerializeField, Range(1f, 360f)]
    float rotationSpeed = 90f;
    [SerializeField, Range(-89f, 89f)]
    float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    float lastManualRotationTime;

    [SerializeField]
    Transform player;

    public Transform testTrans;

    Camera regularCamera;
    Vector3 CameraHalfExtends{
        get{
            Vector3 halfExtends;
            halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
            halfExtends.x = halfExtends.y * regularCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }

    void Awake(){
        regularCamera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);
    }

    void LateUpdate() {

        Quaternion lookRotation;
        if(ManualRotation()){
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else{
            lookRotation = transform.localRotation;
        }
        // Define directions
        focusPoint = focus.position;
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 sideDirection = lookRotation * Vector3.right;



        Vector3 forwardOffset, sideOffset;
        if (Physics.Raycast(
            focusPoint, sideDirection, out RaycastHit sideHit, shoulderOffset + CameraHalfExtends.x
        )){
            sideOffset = sideDirection * sideHit.distance - new Vector3(CameraHalfExtends.x, 0, 0);
        }
        else{
            sideOffset = sideDirection * shoulderOffset;
        }
        Vector3 newFocusPoint = focusPoint + sideOffset;

        // Account for obstacles forward
        Vector3 lookPosition = newFocusPoint;
        if (Physics.BoxCast(
            newFocusPoint, CameraHalfExtends, -lookDirection, out RaycastHit forwardHit, lookRotation, distance - regularCamera.nearClipPlane
        )) {
            forwardOffset = -lookDirection * (forwardHit.distance + regularCamera.nearClipPlane);
        }
        else{
            forwardOffset = -lookDirection * distance;
        }
        lookPosition += forwardOffset;



        testTrans.position = focusPoint + sideOffset;

        // if (Physics.Raycast(
        //     lookPosition, sideDirection, out RaycastHit sideHit, shoulderOffset
        // )) {
        //     sideOffset = sideDirection * sideHit.distance;
        //     Debug.Log(sideHit.distance);
        // }
        // else{
        //     sideOffset = sideDirection * shoulderOffset;
        // }

        // lookPosition += sideOffset;
        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }


    bool ManualRotation(){
        Vector2 input = new Vector2(
            Input.GetAxis("Mouse Y"),
            Input.GetAxis("Mouse X")
        );
        const float minInput = 0.001f;
        if (input.x < -minInput || input.x > minInput || input.y < -minInput || input.y > minInput){
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        return false;
    }

    void ConstrainAngles(){
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);
        if(orbitAngles.y < 0f){
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f){
            orbitAngles.y -= 360f;
        }
        
    }

    void OnValidate(){
        if(maxVerticalAngle < minVerticalAngle){
            maxVerticalAngle = minVerticalAngle;
        }
    }

}

