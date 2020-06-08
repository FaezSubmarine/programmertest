using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI : MonoBehaviour
{

    [SerializeField]Transform patrolWaypoint;
    int patrolNum;
    [SerializeField]float distToWaypoint;

    [SerializeField] float visionRange;
    [SerializeField] float visionHalfAngle;
    [SerializeField] bool isMelee;

    [SerializeField] float speed;

    SpriteRenderer rend;

    [SerializeField] Transform hurtbox;

    bool currentlyAttacking;
    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyAttacking)
        {
            return;
        }
        Collider2D coll = Physics2D.OverlapCircle(transform.position, visionRange, 1 << 8);
        if (coll != null)
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, coll.transform.position - transform.position, visionRange, ~(1 << 9 | 1<<10));

            Debug.DrawRay(transform.position, coll.transform.position - transform.position);
            if (raycastHit.transform == coll.transform)
            {
                //TODO: make sure the enemy flips according to velocity
                if (Vector3.Angle(-transform.right,
                    raycastHit.transform.position - transform.position)
                    < visionHalfAngle)
                {
                    attacks(coll.transform);
                    return;
                }
            }
        }

        Vector3 dir = patrolWaypoint.GetChild(patrolNum).position - transform.position;
        dir.y = 0;
        rend.flipX = (dir.x < 0);
        
        transform.Translate(dir.normalized*Time.deltaTime*speed);
        if (Vector2.SqrMagnitude(dir) < distToWaypoint * distToWaypoint)
        {
            ++patrolNum;
            if (patrolNum == patrolWaypoint.childCount)
            {
                patrolNum = 0;
            }
        }
    }
    void attacks(Transform coll)
    {
        if (isMelee)
        {
            Vector3 dir = coll.position - transform.position;
            dir.y = 0;

            if (Vector2.SqrMagnitude(dir) < 5 * 5)
            {
                StartCoroutine(meleeAttack());
                return;
            }
            rend.flipX = (dir.x < 0);

            transform.Translate(dir.normalized * Time.deltaTime * speed);

        }
        else
        {
            Vector3 dir = coll.position - transform.position;
            StartCoroutine(rangedAttack(dir));
        }
    }
    IEnumerator rangedAttack(Vector3 dir)
    {
        currentlyAttacking = true;
        rend.flipX = (dir.x < 0);
        yield return new WaitForSeconds(1);
        Vector3 hurtBoxScale = new Vector3(1, 1);
        Vector3 offset = new Vector3((rend.bounds.extents.x) + (hurtBoxScale.x / 2), 0) * (!rend.flipX ? 1 : -1);

        Transform newBox = Instantiate(hurtbox, transform.position + offset, Quaternion.identity);
        newBox.GetComponent<HurtBox>().setUpBox(5, 5f, hurtBoxScale, true);

        Rigidbody2D rb = newBox.gameObject.AddComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(dir.x * 100, 0));
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;

        currentlyAttacking = false;
    }
    IEnumerator meleeAttack()
    {
        currentlyAttacking = true;
        yield return new WaitForSeconds(1);

        Vector3 hurtBoxScale = new Vector3(3, 3);
        Vector3 offset = new Vector3((rend.bounds.extents.x)+(hurtBoxScale.x/2), 0) * (!rend.flipX ? 1 : -1);

        Transform newBox = Instantiate(hurtbox, transform.position+offset, Quaternion.identity);
        newBox.GetComponent<HurtBox>().setUpBox(5, 0.1f, hurtBoxScale);
        yield return new WaitForSeconds(0.1f);
        currentlyAttacking = false;
    }
}