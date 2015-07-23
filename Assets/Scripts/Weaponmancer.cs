using UnityEngine;
using System.Collections;

public class Weaponmancer : MonoBehaviour {
    CharacterController movementControl;
    Transform model;

    //movement
    float moveSpeed = 0;
    [SerializeField]
    float walkSpeed = 3.4f;
    [SerializeField]
    float sprintSpeed = 10;
    const float gravity = 20.0F;
    Vector3 moveDirection = Vector3.zero;

    //player angles
    float moveAngle = 0;
    float modelAngle = 0;
    float camTurnSpeed = 200;
    float strafeAngle = 0;
    float strafeSpeed = 200;
    float strafeMaxAngle = 90;
    float keyboardLR, keyboardUD = 0;

    //anims
    Animator anims;
    float tgtAnimSpeed = 0;
    float animSpeed = 0;
    float animTransitionSpeed = 4;


    // Use this for initialization
    void Start() {
        movementControl = GetComponent<CharacterController>();
        model = transform.FindChild("Model");
        anims = model.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        //wasd movement relative to direction 
        keyboardLR = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0); //keyboardLR = Input.GetAxis("Horizontal"); 
        keyboardUD = (Input.GetKey(KeyCode.S) ? -0.5f : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);//keyboardUD = Input.GetAxis("Vertical");

        //move direction turning + strafing
        moveAngle += (Input.GetAxisRaw("Mouse X")) * camTurnSpeed * Time.deltaTime;
        float tgtStrafeAngle = Vector3.Angle(new Vector3(0, 0, 1), new Vector3(keyboardLR, 0, Mathf.Abs(keyboardUD))) * getRotXZMag();
        strafeAngle = Mathf.Lerp(strafeAngle, tgtStrafeAngle, Time.deltaTime * strafeSpeed);
        transform.rotation = Quaternion.AngleAxis(moveAngle + strafeAngle, Vector3.up);

        //smooth model turning
        modelAngle = Mathf.Lerp(modelAngle, moveAngle + strafeAngle, Time.deltaTime * 6);
        model.rotation = Quaternion.Euler(new Vector3(0, modelAngle, 0));

        //apply movement 
        moveSpeed = getMoveSpeed();
        Vector3 playerForward = transform.forward * moveSpeed * getDirXZMag();
        moveDirection = new Vector3(playerForward.x, moveDirection.y, playerForward.z);
        if (!movementControl.isGrounded)
            moveDirection.y = moveDirection.y - (gravity * Time.deltaTime); //gravity
        movementControl.Move(moveDirection * Time.deltaTime);

        //anims
        updateAnimations();
    }

    public bool isWalkingForward() {
        return keyboardUD > 0 && !Input.GetKey(KeyCode.LeftShift);
    }

    public bool isSprinting() {
        return keyboardUD > 0 && Input.GetKey(KeyCode.LeftShift);
    }

    float getMoveSpeed() {
        float speed = 0;
        if (keyboardLR == 0 && keyboardUD == 0 && !Input.GetKey(KeyCode.LeftControl)) {
            speed = 0f; //idle
        }
        else if (Input.GetKey(KeyCode.LeftShift)) {
            if (keyboardUD > 0 || Mathf.Abs(keyboardLR) > 0)
                speed = sprintSpeed; //sprinting
            if (keyboardUD < 0)
                speed = walkSpeed; //no sprinting backwards >:O
        }
        else {
            speed = walkSpeed; //walking 
        }
        return speed;
    }

    float getDirXZMag() {
        if (keyboardUD != 0)
            return (Mathf.Sign(keyboardUD));
        if (keyboardLR != 0)
            return 1;
        return 0;
    }

    void updateAnimations() {
        if (keyboardLR == 0 && keyboardUD == 0 && !Input.GetKey(KeyCode.LeftControl)) {
            tgtAnimSpeed = 0f; //idle
        }
        else if (Input.GetKey(KeyCode.LeftShift)) {
            if (keyboardUD > 0 || Mathf.Abs(keyboardLR) > 0)
                tgtAnimSpeed = 1f; //sprinting f
            if (keyboardUD < 0)
                tgtAnimSpeed = -0.5f; //no sprinting b
        }
        else {
            if (keyboardUD > 0 || Mathf.Abs(keyboardLR) > 0)
                tgtAnimSpeed = 0.5f; //walking f
            if (keyboardUD < 0)
                tgtAnimSpeed = -0.5f; //walking b
        }

        animSpeed = Mathf.Lerp(animSpeed, tgtAnimSpeed, Time.deltaTime * animTransitionSpeed);
        anims.SetFloat("MoveSpeed", animSpeed);
    }

    float getRotXZMag() {
        return keyboardLR != 0 && keyboardUD != 0 ? (Mathf.Sign(keyboardUD)) * (Mathf.Sign(keyboardLR)) : keyboardLR + keyboardUD;
    }

    public float getAngle() {
        return moveAngle;
    }
}
