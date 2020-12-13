using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.SceneManagement;

/// <summary>
/// move and control the player character
/// </summary>

public class CharacterControls : MonoBehaviour
{
    public float speed_normal = 7f;
    public float speed_roll = 14f;
    public float move_speed = 7f;
    public float move_accel = 40f;
    public float rotate_speed = 150f;
    private int timerRollInc = 0;

    [Header("Ground")]
    public float gravity = 2f;
    public float ground_dist = 0.2f;
    public LayerMask ground_mask;

    [Header("Hide")]
    public bool can_hide = true;
    public bool cantMove = false;
    public bool victory = false;
    public bool modeFps = false;
    private bool duringRoll = false;
    public KeyCode hide_key = KeyCode.LeftShift;

    public float current_speed = 0.0f;
    private Vector3 current_move = Vector3.zero;
    private Vector3 current_face = Vector3.forward;
    private Vector3 move_dir;
    private Rigidbody rigid;
    private Animator animator;
    private Collider collide;
    private VisionTarget vision_target;
    private StudioEventEmitter fmodEmit;
    private GameObject[] recipe;
    private GameObject[] van;
    private GameObject[] enemies;
    private GameObject[] levels;
    private GameObject cam;
    private GameObject camFps;
    private GameObject foundRecipe;

    public GameMaster Master;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        collide = GetComponentInChildren<Collider>();
        vision_target = GetComponent<VisionTarget>();

        enemies = GameObject.FindGameObjectsWithTag("npc");
        levels = GameObject.FindGameObjectsWithTag("level");
        recipe = GameObject.FindGameObjectsWithTag("recipe");
        van = GameObject.FindGameObjectsWithTag("van");
        cam = GameObject.Find("Camera");
        camFps = GameObject.Find("CameraFps");
        if(cam!= null){
            fmodEmit = cam.GetComponent<StudioEventEmitter>();
            if(fmodEmit !=null){
                fmodEmit.SetParameter("volumetrack2", 100.0f);
            }
        }
        foundRecipe = null;
        victory = false;
        cantMove = false;

        if(Master!=null && Master.latestPosInWorldMap != null ){

            Vector3 p = Master.latestPosInWorldMap;
            p.z -= 4;
            transform.position = p;
        }
       
    }

    void FixedUpdate()
    {
        if(cantMove)return;
        move_dir = Vector3.zero;
        if (Input.GetKey(KeyCode.Tab))
            modeFps = !modeFps;
        
        if(modeFps){
            cam.SetActive(false);
            camFps.SetActive(true);
        }
        else{
             cam.SetActive(true);
             if(camFps != null)camFps.SetActive(false);
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            move_dir += Vector3.forward;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                move_dir += Vector3.left;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                move_dir += Vector3.right;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                move_dir += Vector3.back;
        }
        

        bool invisible = false;
        if (vision_target)
            vision_target.visible = !invisible;
        if (collide)
            collide.enabled = !invisible;

        if (invisible)
            move_dir = Vector3.zero;

        if(Master != null && SceneManager.GetActiveScene().name == "WorldMap"){
            Master.latestPosInWorldMap = transform.position;
        }
        if(!modeFps){
             move_dir = move_dir.normalized * Mathf.Min(move_dir.magnitude, 1f);
            current_move = Vector3.MoveTowards(current_move, move_dir, move_accel * Time.fixedDeltaTime);
            rigid.velocity = current_move * move_speed;
            current_speed = rigid.velocity.magnitude;
            timerRollInc --;

            if (current_move.magnitude > 0.1f)
            current_face = new Vector3(current_move.x, 0f, current_move.z).normalized;
        }
        //Move
       

        if(timerRollInc <= 85 && duringRoll){
            duringRoll = false;
            if(animator != null)
            {
                animator.SetBool("Roll", false);
                animator.SetBool("Idle", false);
                animator.SetBool("Move", false);
                animator.SetBool("Run", false);
            }
            
            move_speed = speed_normal;
        }
            


        if (Input.GetKey(KeyCode.Space)  && timerRollInc <=0){
            // move_dir += Vector3.back;
            timerRollInc = 100;
            duringRoll = true;
            Roll();
        }
            

        bool grounded = CheckIfGrounded();
        if (!grounded)
            rigid.velocity += Vector3.down * gravity;

        

        if(recipe.Length > 0 && foundRecipe == null && victory == false){
            //Check if reached target
            Vector3 dist_vect = (recipe[0].transform.position - transform.position);
            dist_vect.y = 0f;
            if (dist_vect.magnitude < 2f)
            {
                Debug.Log("found the recipe");
                
                foundRecipe = recipe[0];
                foundRecipe.SetActive(false);
                foundRecipe.SetActive(true);
            }
        }
        if (victory == false && van != null && van.Length > 0){

            if(foundRecipe != null){
                foundRecipe.transform.position = transform.position;
                Vector3 p = foundRecipe.transform.position;
                p.y = transform.position.y + 0.9f;
                foundRecipe.transform.position = p;
            }
           

            Vector3 dist_vect = (van[0].transform.position - transform.position);
            dist_vect.y = 0f;
            if (dist_vect.magnitude < 2f && victory == false)
            {
                van[0].SetActive(false);
                van[0].SetActive(true);
                victory = true;
                invisible = true;
                Debug.Log("winning");
                InvokeRepeating("startNextScene", 1.5f, 10f);
            }
            // foundRecipe.transform.position.y = transform.position.y + 2;
        }
        if(victory == true){
            Vector3 p = van[0].transform.position;
            if(SceneManager.GetActiveScene().name == "Restau_02")
                p.x -= 0.5f;
            else 
                p.z += 0.5f;
            van[0].transform.position = p;
            if(foundRecipe != null)foundRecipe.SetActive(false);
            transform.position = van[0].transform.position;
            // this.gameObject.SetActive(false);
        }
        for(int i = 0; i < enemies.Length; i++)
        {
             Vector3 dist_vect = (enemies[i].transform.position - transform.position);
             if(dist_vect.magnitude < 1.5f){
                 cantMove = true;
                 enemies[i].GetComponent<EnemyDemo>().Hit();
                 gameObject.SetActive(false);
                 gameObject.SetActive(true);
                 InvokeRepeating("startCurrentScene", 0.5f, 10f);
                  
             }
        }
        for(int i = 0; i < levels.Length; i++)
        {
             Vector3 dist_vect = (levels[i].transform.position - transform.position);
             if(dist_vect.magnitude < 3.5f){
                  SceneManager.LoadScene(levels[i].name);
             }
        }
        if(modeFps == false){
            //Rotate
            Vector3 dir = current_face;
            dir.y = 0f;
            if (dir.magnitude > 0.1f)
            {
                Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);
                Quaternion reachedRotation = Quaternion.RotateTowards(transform.rotation, target, rotate_speed * Time.deltaTime);
                transform.rotation = reachedRotation;
            }
        }
        

        if (animator != null)
        {
            Debug.Log("current_speed : " + current_speed );

            // animator.SetBool("Move", current_move.magnitude > 0.1f);
            if(duringRoll == false){
                 if(current_speed > 5){
                    animator.SetBool("Move", false);
                    animator.SetBool("Run", true);
                    animator.SetBool("Idle", false);
                    animator.SetBool("Roll", false);
                }
                else if(current_speed <= 5 && current_speed > 0){
                    animator.SetBool("Move", true);
                    animator.SetBool("Run", false);
                    animator.SetBool("Idle", false);
                    animator.SetBool("Roll", false);
                }
                else{
                    animator.SetBool("Idle", true);
                    animator.SetBool("Move", false);
                    animator.SetBool("Run", false);
                    animator.SetBool("Roll", false);
                } 
            }
            
            
        }
    }
    public void Roll(){
        if(animator != null){
            animator.SetBool("Roll", true);
            animator.SetBool("Idle", false);
            animator.SetBool("Move", false);
            animator.SetBool("Run", false);
            // current_move = Vector3.MoveTowards(current_move, move_dir, move_accel*3 * Time.fixedDeltaTime);
            // rigid.velocity = current_move * move_speed;
            // current_speed = rigid.velocity.magnitude;
            move_speed = speed_roll;
        }
        
    }
    public void startCurrentScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void startNextScene(){
        SceneManager.LoadScene("WorldMap");
    }
    public bool CheckIfGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * ground_dist * 0.5f;
        return RaycastObstacle(origin + Vector3.forward * 0.5f, Vector3.down * ground_dist)
            || RaycastObstacle(origin + Vector3.back * 0.5f, Vector3.down * ground_dist)
            || RaycastObstacle(origin + Vector3.left * 0.5f, Vector3.down * ground_dist)
            || RaycastObstacle(origin + Vector3.right * 0.5f, Vector3.down * ground_dist);
    }

    public bool RaycastObstacle(Vector3 origin, Vector3 dir)
    {
        RaycastHit hit;
        return Physics.Raycast(new Ray(origin, dir.normalized), out hit, dir.magnitude, ground_mask.value);
    }

    public Vector3 GetMove()
    {
        return current_move;
    }

    public Vector3 GetFace()
    {
        return current_face;
    }
}
