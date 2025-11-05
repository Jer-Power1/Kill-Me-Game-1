using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyChase : MonoBehaviour
{
    public Transform target;               // auto-fills from Player tag if empty
    public float moveSpeed = 3.2f;
    public float turnSpeed = 12f;
    public float gravity = -20f;

    [Header("Melee")]
    public float attackRange = 1.2f;
    public float attackDamage = 10f;
    public float attackCooldown = 0.6f;

    CharacterController cc;
    float verticalVel;
    float nextAttackTime;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!target)
        {
            var t = GameObject.FindGameObjectWithTag("Player");
            if (t) target = t.transform;
        }
    }

    void Update()
    {
        if (!target) return;

        // face player
        Vector3 to = target.position - transform.position;
        to.y = 0f;
        if (to.sqrMagnitude > 0.001f)
        {
            var look = Quaternion.LookRotation(to.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        // move toward player
        Vector3 horiz = to.normalized * moveSpeed;
        if (cc.isGrounded) verticalVel = -2f; else verticalVel += gravity * Time.deltaTime;

        Vector3 motion = new Vector3(horiz.x, verticalVel, horiz.z);
        cc.Move(motion * Time.deltaTime);

        // melee if close
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= attackRange && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            var ph = target.GetComponent<PlayerHealth>();
            if (ph) ph.TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
