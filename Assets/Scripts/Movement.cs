using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField]
    private GameObject cam;

    public float jumpForce;

    private Vector3 velocity;
    private Vector3 rotation;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 thrusterForce;

    [SerializeField]
    private float cameraRotationLimit = 85f;
    // Start is called before the first frame update
    void Start()
    {
        // GameManagement.re = 0;
        rb = GetComponent<Rigidbody>();
        cam = GameObject.Find("CameraFps");
    }
    // public float forwardForce = 2000f;
    // public float sideForce = 100;

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    public void jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void RotateCamera(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }
    // Update is called once per frame
     private void FixedUpdate()
    {
        if(gameObject.GetComponent<CharacterControls>().modeFps == false) return;
        PerformMovement();
        PerformRotation();
    }
    private void PerformMovement() {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        // if(thrusterForce != Vector3.zero)
        // {
        //     rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        // }
    }
    
    private void PerformRotation()
    {
        // Récuperation de la rotation + Clamp la rotation 
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        // Applique les changements à la caméra après le clamp
        cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
    // void FixedUpdate()
    // {
    //     // rb.AddForce(0,0,forwardForce * Time.deltaTime);
    //     // if(Input.GetKey("escape")){
            
    //     // }
    //     if(Input.GetKey("z")){
    //         rb.AddForce(0,0,sideForce * Time.deltaTime, ForceMode.VelocityChange);
    //     }
    //     if(Input.GetKey("s")){
    //         rb.AddForce(0,0,sideForce * Time.deltaTime*-1, ForceMode.VelocityChange);
    //     }
    //     if(Input.GetKey("space")){
    //         rb.AddForce(0,sideForce * Time.deltaTime*-1,0, ForceMode.VelocityChange);
    //     }
    //     if(Input.GetKey("d")){
    //          rb.AddForce(sideForce * Time.deltaTime,0,0, ForceMode.VelocityChange);
    //     }
    //      if(Input.GetKey("q")){
    //          rb.AddForce(sideForce * -1 * Time.deltaTime,0,0, ForceMode.VelocityChange);
    //     }
    // }
}
