using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// move and control the player character
/// </summary>

public class CharacterControls : MonoBehaviour
{
    public float move_speed = 7f;
    public float move_accel = 40f;
    public float rotate_speed = 150f;

    [Header("Ground")]
    public float gravity = 2f;
    public float ground_dist = 0.2f;
    public LayerMask ground_mask;

    [Header("Hide")]
    public bool can_hide = true;
    public bool victory = false;
    public KeyCode hide_key = KeyCode.LeftShift;

    private Vector3 current_move = Vector3.zero;
    private Vector3 current_face = Vector3.forward;
    private Rigidbody rigid;
    private Animator animator;
    private Collider collide;
    private VisionTarget vision_target;
    private GameObject[] recipe;
    private GameObject[] van;
    private GameObject[] enemies;
    private GameObject foundRecipe;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        collide = GetComponentInChildren<Collider>();
        vision_target = GetComponent<VisionTarget>();

        enemies = GameObject.FindGameObjectsWithTag("npc");
        recipe = GameObject.FindGameObjectsWithTag("recipe");
        van = GameObject.FindGameObjectsWithTag("van");
    }

    void FixedUpdate()
    {
        Vector3 move_dir = Vector3.zero;
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

        //Move
        move_dir = move_dir.normalized * Mathf.Min(move_dir.magnitude, 1f);
        current_move = Vector3.MoveTowards(current_move, move_dir, move_accel * Time.fixedDeltaTime);
        rigid.velocity = current_move * move_speed;

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
        else if (foundRecipe != null && victory == false){

            foundRecipe.transform.position = transform.position;
            Vector3 p = foundRecipe.transform.position;
            p.y = transform.position.y + 0.7f;
            foundRecipe.transform.position = p;

            Vector3 dist_vect = (van[0].transform.position - transform.position);
            dist_vect.y = 0f;
            if (dist_vect.magnitude < 2f)
            {
                victory = true;
                invisible = true;
                Debug.Log("winning");
            }
            // foundRecipe.transform.position.y = transform.position.y + 2;
        }
        if(victory){
            Vector3 p = van[0].transform.position;
            p.z ++;
            van[0].transform.position = p;
            foundRecipe.SetActive(false);
            this.gameObject.SetActive(false);
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
            animator.SetBool("Move", current_move.magnitude > 0.1f);
            animator.SetBool("Run", current_move.magnitude > 0.1f);
            animator.SetBool("Hide", invisible);
        }
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
