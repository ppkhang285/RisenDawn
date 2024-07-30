using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    public Collider2D attackCollider;
    public LayerMask mask;

    private Animator animator;

    private float _attackDuration = 0.08f;
    private float attackTimer = 0;
    private float _attckCooldown = 0.01f;
    private float attackCooldownTimer = 0;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void Setup()
    {
        attackTimer = _attackDuration;
    }
    
    void Update()
    {
        Attack();
    }

    private void FixedUpdate()
    {
        
    }

    private void Attack()
    {
        

        if (Input.GetKeyDown(KeyCode.Mouse0) && attackCooldownTimer <=0)
        {
            animator.Play("Attack");
            attackTimer = _attackDuration;
        }
        
    }

    private void CalculateAttack()
    {
        // Calculate attack cooldown
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.fixedDeltaTime;
        }

        // Calculate attack time
        if (attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;

            if (attackTimer <= 0)
            {
                attackCooldownTimer = _attckCooldown;
            }

        }
     
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        List<Collider2D> hitCollider = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();

        contactFilter.SetLayerMask(mask);

        Physics2D.OverlapCollider(attackCollider, contactFilter, hitCollider);

        foreach(Collider2D collider in hitCollider)
        {
            Debug.Log(collider);
        }
    }
}
