using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2.0f;
    public LayerMask breakableLayer;
    public GameObject attackAnimationPrefab;
    public float attackAnimationDuration = 1.0f;
    public float animationOffset = 1.0f;
    public Vector3 effectScale = new Vector3(2.0f, 2.0f, 2.0f);

    private bool isAttacking = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        Debug.Log("PlayerAttack Start - Constraints: " + rb.constraints);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isAttacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, breakableLayer);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Breakable"))
            {
                BreakableObject breakableObject = hitCollider.GetComponent<BreakableObject>();
                if (breakableObject != null)
                {
                    StartCoroutine(PerformAttack(breakableObject, hitCollider.transform));
                }
            }
        }
    }

    private IEnumerator PerformAttack(BreakableObject breakableObject, Transform target)
    {
        isAttacking = true;

        Vector3 spawnPosition = transform.position + transform.forward * animationOffset;
        GameObject attackAnimation = Instantiate(attackAnimationPrefab, spawnPosition, Quaternion.identity);
        attackAnimation.transform.localScale = effectScale;

        yield return new WaitForSeconds(attackAnimationDuration);

        Destroy(attackAnimation);
        breakableObject.Hit();
        isAttacking = false;
    }
}
