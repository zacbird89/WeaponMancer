using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
    Weaponmancer playerInfo;
    public Transform target;
    Vector3 tgtPos = new Vector3(0, 5, -5);
    float distance = -4.0f;
    float maxDist = -10;
    float minDist = -2;
    float height = -0.5f;

    float angle = 0;
    float tgtDist = 0;
    float refDist = 0;
    float lastMouseY = 0;

    float zoomIncrement = 1;
    float zoomSpeed = 0.2f;
    float rotateSpeed = 200;
    float vertSpeed = 5f;

    //fov
    float normalFov;
    float runningFovInc = 5;
    float sprintingFovInc = 10;
    float tgtFov = 0;

    void Start() {
        Cursor.visible = false; 
        tgtDist = distance;
        lastMouseY = Input.GetAxisRaw("Mouse Y");
        playerInfo = GameObject.Find("Player").GetComponent<Weaponmancer>();
        normalFov = GetComponent<Camera>().fieldOfView;
        tgtFov = normalFov;
    }

    void Update() {
        reactToMouse();

        //move circularly around the player
        angle = playerInfo.getAngle();
        if (angle > 360 || angle < -360) angle = angle % 360;
        distance = Mathf.SmoothDamp(distance, tgtDist, ref refDist, zoomSpeed);
        float xpos = Mathf.Sin(Mathf.Deg2Rad * angle);
        float zpos = Mathf.Cos(Mathf.Deg2Rad * angle);
        tgtPos = new Vector3(xpos, height, zpos).normalized * distance;
        transform.position = Vector3.Lerp(transform.position, target.transform.position + tgtPos, Time.deltaTime * 12);

        //look at bro
        Quaternion lookrotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = lookrotation;

        //fov
        if (playerInfo.isWalkingForward()) tgtFov = normalFov + runningFovInc;
        else if (playerInfo.isSprinting()) tgtFov = normalFov + sprintingFovInc;
        else tgtFov = normalFov;
        GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, tgtFov, Time.deltaTime * 6);

        lastMouseY = Input.GetAxisRaw("Mouse Y");
    }

    void reactToMouse() {
        tgtDist += Input.mouseScrollDelta.y * zoomIncrement;
        tgtDist = Mathf.Clamp(tgtDist, maxDist, minDist);
        height += (Input.GetAxisRaw("Mouse Y")) * Time.deltaTime * vertSpeed;
    }
}
