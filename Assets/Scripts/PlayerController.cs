using UnityEngine;
using System.Collections;

// [RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Movement))]
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float gravity = 2f;
    [SerializeField]
    private float lookSensitivityX = 3f;
    [SerializeField]
    private float lookSensitivityY = 3f;

     [SerializeField]
    private bool readyToJump = true;
     [SerializeField]
    private bool grounded = true;
     [SerializeField]
    private bool jumping = false;



    // [SerializeField]
    // private float thrusterForce = 1000f;

    // [SerializeField]
    // private float thrusterFuelBurnSpeed = 1f;
    // [SerializeField]
    // private float thrusterFuelRegenSpeed = 0.3f;
    // private float thrusterFuelAmount = 1f;

    // public float GetThrusterFuelAmount()
    // {
    //     return thrusterFuelAmount;
    // }

    // [SerializeField]
    // private LayerMask environmentMask;

    // [Header("Spring settings :")]
    // [SerializeField]
    // private float jointSpring = 20f;
    // [SerializeField]
    // private float jointMaxForce = 40f;

    private Movement motor;
    // private ConfigurableJoint joint;
    // private Animator animator;

    private void Start()
    {
        Cursor.visible = false;
        motor = GetComponent<Movement>();
    }
    // private void CounterMovement(float x, float y, Vector2 mag)
    // {
    //     if(!grounded) return;
    //     float threshold = 0.3f;
    //     float multiplier = 0.3f;


    //     if(x == 0){
    //         motor.rb.AddForce(speed, )
    //     }
    // }
    private void Update()
    {
        // Collider[] groudd = Physics.OverlapSphere(motor.rb.position, 0.01f);
        // Check for a Wall.
        LayerMask mask = LayerMask.GetMask("ShouldContactWithHero");
        Collider[] grounded = Physics.OverlapSphere(motor.rb.position, 0.5f);
        // foreach (Collider Coll in grounded)
        // {
        //     if(Coll.attachedRigidbody == motor.rb){
        //        System.Collections.Generic.List<GameObject> list = new System.Collections.Generic.List<GameObject>(grounded);
        //         list.Remove(Coll);
        //         grounded = list.ToArray();
        //     }
        // }

        motor.Move(Vector3.zero);
        motor.Rotate(Vector3.zero);
        motor.RotateCamera(0f);

        // On va calculer la vélocité du mouvement du joueur en un Vecteur 3D
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");
        if (grounded.Length > 1 && Input.GetKeyDown("space") && !jumping)
        {
            readyToJump = false;
            motor.jump();
         }
        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        motor.Move(_velocity);

        Vector3 gravityForce = new Vector3(0, gravity, 0);
        motor.rb.AddForce(gravityForce * motor.rb.mass);
        

        // On va calculer la rotation du joueur en un Vecteur 3D
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0, _yRot, 0) * lookSensitivityX;

        motor.Rotate(_rotation);

        // On va calculer la rotation de la camera en un Vecteur 3D
        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRot * lookSensitivityY;

        motor.RotateCamera(_cameraRotationX);

    }
 }