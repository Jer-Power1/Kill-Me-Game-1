using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyChase : MonoBehaviour
{
    public Transform target;

    [Header("Movement")]
    public float moveSpeed = 3.2f;
    public float turnSpeed = 12f;
    public float gravity = -20f;

    [Header("Melee")]
    public float attackRange = 1.2f;
    public float attackDamage = 10f;
    public float attackCooldown = 0.6f;

    // Base stats (used for scaling)
    [HideInInspector] public float baseMoveSpeed;
    [HideInInspector] public float baseAttackDamage;

    CharacterController cc;
    float verticalVel;
    float nextAttackTime;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        // Cache base values ONCE
        baseMoveSpeed = moveSpeed;
        baseAttackDamage = attackDamage;

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
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                look,
                turnSpeed * Time.deltaTime
            );
        }

        // movement
        Vector3 horiz = to.normalized * moveSpeed;

        if (cc.isGrounded)
            verticalVel = -2f;
        else
            verticalVel += gravity * Time.deltaTime;

        Vector3 motion = new Vector3(horiz.x, verticalVel, horiz.z);
        cc.Move(motion * Time.deltaTime);

        // melee attack
        if (Time.time >= nextAttackTime)
        {
            Collider[] hits = Physics.OverlapSphere(
                transform.position + Vector3.up * 1f,
                attackRange
            );

            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Player")) continue;

                var ph = hit.GetComponent<PlayerHealth>();
                if (ph)
                {
                    nextAttackTime = Time.time + attackCooldown;
                    ph.TakeDamage(attackDamage);
                    break;
                }
            }
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
