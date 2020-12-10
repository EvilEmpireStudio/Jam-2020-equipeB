using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// move and control the player character
/// </summary>

public class CharacterControls : MonoBehaviour
{
    public float move_speed = 7f;
    public float move_accel = 40f;
    public float rotate_speed = 150f;
    private int timerRollInc = 400;

    [Header("Ground")]
    public float gravity = 2f;
    public float ground_dist = 0.2f;
    public LayerMask ground_mask;

    [Header("Hide")]
    public bool can_hide = true;
    public bool victory = false;
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
    private GameObject[] recipe;
    private GameObject[] van;
    private GameObject[] enemies;
    private GameObject[] levels;
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
        foundRecipe = null;
        victory = false;

        if(Master!=null && Master.latestPosInWorldMap != null ){

            Vector3 p = Master.latestPosInWorldMap;
            p.z -= 4;
            transform.position = p;
        }

    }

    void FixedUpdate()
    {
        move_dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            move_dir += Vector3.forward;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            move_dir += Vector3.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            move_dir += Vector3.right;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            move_dir += Vector3.back;

        bool invisible = can_hide && Input.GetKey(hide_key);
        if (vision_target)
            vision_target.visible = !invisible;
        if (collide)
            collide.enabled = !invisible;

        if (invisible)
            move_dir = Vector3.zero;

        if(Master != null && SceneManager.GetActiveScene().name == "WorldMap"){
            Master.latestPosInWorldMap = transform.position;
        }

        //Move
        move_dir = move_dir.normalized * Mathf.Min(move_dir.magnitude, 1f);
        current_move = Vector3.MoveTowards(current_move, move_dir, move_accel * Time.fixedDeltaTime);
        rigid.velocity = current_move * move_speed;
        current_speed = rigid.velocity.magnitude;
        timerRollInc --;
        if(timerRollInc <= 350)
            duringRoll = false;

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.DownArrow) && timerRollInc <=0){
            move_dir += Vector3.back;
            timerRollInc = 400;
            duringRoll = true;
            Roll();
        }
            

        bool grounded = CheckIfGrounded();
        if (!grounded)
            rigid.velocity += Vector3.down * gravity;

        if (current_move.magnitude > 0.1f)
            current_face = new Vector3(current_move.x, 0f, current_move.z).normalized;

        if(recipe.Length > 0 && foundRecipe == null && victory == false){
            //Check if reached target
            Vector3 dist_vect = (recipe[0].transform.position - transform.position);
            dist_vect.y = 0f;
            if (dist_vect.magnitude < 2f)
            {
                Debug.Log("found the recipe");
                foundRecipe = recipe[0];
            }
        }
        else if ((foundRecipe != null || SceneManager.GetActiveScene().name == "Home")  && victory == false){

            if(foundRecipe != null){
                foundRecipe.transform.position = transform.position;
                Vector3 p = foundRecipe.transform.position;
                p.y = transform.position.y + 0.7f;
                foundRecipe.transform.position = p;
            }
           

            Vector3 dist_vect = (van[0].transform.position - transform.position);
            dist_vect.y = 0f;
            if (dist_vect.magnitude < 2f && victory == false)
            {
                victory = true;
                invisible = true;
                Debug.Log("winning");
                InvokeRepeating("startNextScene", 1.5f, 10f);
            }
            // foundRecipe.transform.position.y = transform.position.y + 2;
        }
        if(victory){
            Vector3 p = van[0].transform.position;
            p.z ++;
            van[0].transform.position = p;
            if(foundRecipe != null)foundRecipe.SetActive(false);
            transform.position = van[0].transform.position;
            // this.gameObject.SetActive(false);
        }
        for(int i = 0; i < enemies.Length; i++)
        {
             Vector3 dist_vect = (enemies[i].transform.position - transform.position);
             if(dist_vect.magnitude < 1.5f){
                  SceneManager.LoadScene(SceneManager.GetActiveScene().name);
             }
        }
        for(int i = 0; i < levels.Length; i++)
        {
             Vector3 dist_vect = (levels[i].transform.position - transform.position);
             if(dist_vect.magnitude < 2.5f){
                  SceneManager.LoadScene(levels[i].name);
             }
        }
        //Rotate
        Vector3 dir = current_face;
        dir.y = 0f;
        if (dir.magnitude > 0.1f)
        {
            Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);
            Quaternion reachedRotation = Quaternion.RotateTowards(transform.rotation, target, rotate_speed * Time.deltaTime);
            transform.rotation = reachedRotation;
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
        animator.SetBool("Roll", true);
        animator.SetBool("Idle", false);
        animator.SetBool("Move", false);
        animator.SetBool("Run", false);
        current_move = Vector3.MoveTowards(current_move, move_dir, move_accel*3 * Time.fixedDeltaTime);
        rigid.velocity = current_move * move_speed;
        current_speed = rigid.velocity.magnitude;
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
