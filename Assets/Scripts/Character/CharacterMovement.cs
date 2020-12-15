using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private float groundLeaveWaitCheck = 0.2f;

    [SerializeField]
    private float gravity = 20;

    [SerializeField]
    private Animator animator = null;

    private Vector3 inputVec;

    [SerializeField]
    private float characterSpeed = 0;

    [SerializeField]
    private float jumpSpeed = 8;

    [SerializeField]
    private float slopeForce = 5;

    [SerializeField]
    private float slopeCheckRayLength = 0.2f;

    [SerializeField]
    private float sensitivityX;

    [SerializeField]
    private float sensitivityY;

    [SerializeField]
    private Transform myCameraRail = null;

    [SerializeField]
    private Transform myCamera = null;

    [SerializeField]
    private Transform robotHips = null;

    [SerializeField]
    private float smoothZoom = 2f;

    [SerializeField]
    private AudioClip jumpSFX;

    [SerializeField]
    private AudioClip landedSFX;

    [SerializeField]
    private bool TEST_ARENA = false;

    #endregion

    #region Private Variables

    private float leavingGroundMoment = 0;

    private CharacterController characterController;

    private bool isJumping = false;

    private Vector3 cachedRotation;

    private Transform originalCameraPos;

    private PhotonView view;

    private AudioSource audioSource;

    #endregion


    internal float InitialRotationY = 0;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        inputVec = new Vector3(0, 0, 0);

        characterController = GetComponent<CharacterController>();

        if (view.IsMine || TEST_ARENA)
        {
            originalCameraPos = new GameObject("originalCameraPos").transform;
            originalCameraPos.position = myCamera.position;
            originalCameraPos.parent = myCameraRail;
            TrapCursor();
        }
        else
        {
            //myCamera.gameObject.SetActive(false);
            myCamera.GetComponent<Camera>().enabled = false;
            myCamera.GetComponent<AudioListener>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
        }
    }

    internal void TrapCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    internal void ReleaseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (!view.IsMine && TEST_ARENA == false)
        {
            return;
        }

        UpdatePosition();
        UpdateRotation();
    }

    private void UpdatePosition()
    {
        bool onSlope = OnSlope();

        bool isGroundedNow = (characterController.isGrounded || onSlope);

        if (!isGroundedNow && leavingGroundMoment == 0)
        {
            leavingGroundMoment = Time.time;
        }
        else if (isGroundedNow)
        {
            leavingGroundMoment = 0;
        }

        bool isGrounded = isGroundedNow || !isGroundedNow && (leavingGroundMoment + groundLeaveWaitCheck > Time.time);


        animator.SetBool("apGrounded", isGrounded);

        if (isGroundedNow)
        {
            inputVec = Vector3.zero;
        }

        inputVec.x = Input.GetAxis("Horizontal") * characterSpeed;
        inputVec.z = Input.GetAxis("Vertical") * characterSpeed;



        if (isGroundedNow)
        {
            isJumping = false;
            if (Input.GetButtonDown("Jump"))
            {
                animator.Rebind();
                inputVec.y = jumpSpeed;
                isJumping = true;
                animator.SetTrigger("apJump");
                audioSource.PlayOneShot(jumpSFX);
            }

            animator.SetFloat("apVertical", Input.GetAxis("Vertical"));
            animator.SetFloat("apHorizontal", Input.GetAxis("Horizontal"));
        }



        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && onSlope)
            characterController.Move(Vector3.down * characterController.height / 2 * slopeForce * Time.deltaTime);

        inputVec.y -= gravity * Time.deltaTime;


        //transform.rotation = Quaternion.Euler(0, RotationY, 0);
        inputVec = transform.TransformDirection(inputVec);

        characterController.Move(inputVec * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        float rotationY = Input.GetAxis("Mouse Y") * sensitivityX;
        float rotationX = Input.GetAxis("Mouse X") * sensitivityY;

        if (rotationY > 0)
            cachedRotation = new Vector3(Mathf.MoveTowards(cachedRotation.x, -80, rotationY), cachedRotation.y + rotationX, 0);
        else
            cachedRotation = new Vector3(Mathf.MoveTowards(cachedRotation.x, 80, -rotationY), cachedRotation.y + rotationX, 0);

        transform.rotation = Quaternion.Euler(0, cachedRotation.y + InitialRotationY, 0);
        myCameraRail.localEulerAngles = new Vector3(cachedRotation.x, 0, 0);

        DetectObstructedView();

    }

    private void DetectObstructedView()
    {
        RaycastHit hit;
        float distance = Vector3.Distance(originalCameraPos.position, robotHips.position);
        bool zoom = false;

        if (Physics.Raycast(originalCameraPos.position, robotHips.position - originalCameraPos.position, out hit, distance))
        {
            zoom = (hit.collider.gameObject != this.gameObject);
        }

        Vector3 newPosition = zoom ? originalCameraPos.localPosition.SetZ(-1) : originalCameraPos.localPosition;
        myCamera.transform.localPosition = Vector3.Lerp(myCamera.transform.localPosition,
                                                        newPosition, Time.deltaTime * smoothZoom);

    }

    bool OnSlope()
    {
        if (isJumping)
            return false;

        RaycastHit hit;

        //Slope calculation:
        //https://www.youtube.com/watch?v=b7bmNDdYPzU


        Vector3 endPos = transform.position + (slopeCheckRayLength * Vector3.down);
        //Debug.DrawLine(transform.position, endPos, Color.green);

        if (Physics.Raycast(transform.position, Vector3.down, out hit, slopeCheckRayLength))
            if (hit.normal != Vector3.up)
                return true;
        return false;
    }

    internal void Landed()
    {
        audioSource.PlayOneShot(landedSFX);
    }
}
