using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demo script on how to link animations and use Enemy events
/// </summary>

[RequireComponent(typeof(EnemyVision))]
public class EnemyDemo : MonoBehaviour
{
    public GameObject exclama_prefab;
    public GameObject death_fx_prefab;

    private EnemyVision enemy;
    private Animator animator;
    private int alertTimer = 0;
    private bool alertBool = false;
    private bool isHitting = false;
    
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if(animator != null){
            animator.SetBool("Move", false);
            animator.SetBool("Run", false);
            // animator.SetBool("Idle", true);
            animator.SetBool("Alert", false);
            animator.SetBool("Hit", false);
            animator.Rebind();
        }
        enemy = GetComponent<EnemyVision>();
        enemy.onDeath += OnDeath;
        enemy.onAlert += OnAlert;
        enemy.onSeeTarget += OnSeen;
        enemy.onDetectTarget += OnDetect;
        enemy.onTouchTarget += OnTouch;
        isHitting = false;
        
    }

    void Update()
    {
        // if (animator != null)
        // {
            // animator.SetBool("Move", enemy.GetEnemy().GetMove().magnitude > 0.5f || enemy.GetEnemy().GetRotationVelocity() > 10f);
            // animator.SetBool("Run", enemy.GetEnemy().IsRunning());
        // }
        alertTimer--;
        if(alertTimer <= 0 && alertBool && isHitting == false){
            //  if(enemy.enemy.GetState() == 10){
                animator.SetBool("Move", false);
                animator.SetBool("Run", true);
                // animator.SetBool("Idle", false);
                animator.SetBool("Alert", false);
                // animator.SetBool("Hit", false);
            // }
            // else {
            //     animator.SetBool("Move", true);
            //     animator.SetBool("Run", false);
            //     animator.SetBool("Idle", false);
            //     animator.SetBool("Alert", false);
            // }
           
        }
       
    }

    //Can be either because seen or heard noise
    private void OnAlert(Vector3 target)
    {
        if (exclama_prefab != null)
            Instantiate(exclama_prefab, transform.position + Vector3.up * 2f, Quaternion.identity);
        if (animator != null && isHitting == false){
            animator.SetTrigger("Surprised");
            animator.SetBool("Idle", false);
            animator.SetBool("Run", false);
            animator.SetBool("Move", false);
            animator.SetBool("Alert", true);
            animator.SetBool("Hit", false);
            alertTimer = 60;
            alertBool = true;
            if(transform.name != "Client"){
                animator.Play("Surprised", -1, 0f);
            }else{
                //  animator.Play("Alert", -1, 0f);
            }
           
        }
            
    }
    public void Hit(){
        if (animator != null && isHitting == false){
            isHitting = true;
            animator.SetBool("Move", false);
            animator.SetBool("Run", false);
            // animator.SetBool("Idle", false);
            animator.SetBool("Alert", false);
            animator.SetBool("Hit", true);
             animator.Play("Hit", -1, 0f);
        }
    }

    private void OnSeen(VisionTarget target, int distance)
    {
        //Add code for when target get seen and enemy get alerted, 0=touch, 1=near, 2=far, 3=other
    }

    private void OnDetect(VisionTarget target, int distance)
    {
        //Add code for when the enemy detect you as a threat (and start chasing), 0=touch, 1=near, 2=far, 3=other
    }

    private void OnTouch(VisionTarget target)
    {
        //Add code for when you get caughts
    }

    private void OnDeath()
    {
        if(death_fx_prefab)
            Instantiate(death_fx_prefab, transform.position + Vector3.up * 0.5f, death_fx_prefab.transform.rotation);
    }
}
