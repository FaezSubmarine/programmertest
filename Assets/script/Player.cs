using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer rend;
    [SerializeField] float jumpPower = 50;
    [SerializeField] float walkSpeed = 100;

    [SerializeField] float swingSpeed = 30;
    [SerializeField] float halfSwingAngle = 90;
    float totalSwing = 0;
    [SerializeField] IntVariable pointCollection;

    bool pendulumJump;
    Vector3? swingingPoint;
    bool swingingLeft = false;
    bool stopSwinging;
    [SerializeField]FloatVariable healthVariable;
    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded())
        {
            rb.AddForce(Vector2.up * jumpPower);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position + new Vector3(rend.bounds.extents.x * (!rend.flipX?1:-1),0, 0),new Vector2(5,3), 0, Vector2.right, 0.01f, (1 << 9));
            if(hit.collider != null)
            {
                if(hit.collider.gameObject.layer == 9)
                {
                    BaseAI ai = hit.collider.GetComponent<BaseAI>();
                    if(ai != null)
                    {
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
        }
        if (pendulumJump && Input.GetKey(KeyCode.X) && !isGrounded())
        {
            if (swingingPoint == null)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                rb.gravityScale = 0;
                swingingLeft = rend.flipX;
                swingingPoint = transform.position + new Vector3(swingingLeft?2:-2, 10);
            }
            float deltaSwing = (swingingLeft ? -1 : 1) * swingSpeed * Time.deltaTime;

            //if (Vector3.Cross(Vector3.right, (Vector3)swingingPoint - transform.position).z < 0 )
            if(Vector3.Angle(Vector3.up, (Vector3)swingingPoint - transform.position)>halfSwingAngle)
            {
                Debug.Log("go the other way " + Vector3.Angle(Vector3.up, (Vector3)swingingPoint - transform.position));
                swingingLeft = !swingingLeft;
                deltaSwing = (swingingLeft ? -1 : 1) * swingSpeed * Time.deltaTime;
            }
            if(!stopSwinging)
                transform.RotateAround((Vector3)swingingPoint, Vector3.forward, deltaSwing);

            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else if(Input.GetKeyUp(KeyCode.X) && swingingPoint != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 1;
            swingingPoint = null;
        }
    }
private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if(swingingPoint == null)
            transform.Translate(walkSpeed* horizontalInput*Time.fixedDeltaTime,0,0);

        Debug.DrawRay(transform.position + new Vector3(0, -rend.bounds.extents.y, 0), Vector2.down * 0.01f,Color.blue);
        if((horizontalInput>0 && rend.flipX) ||(horizontalInput<0 && !rend.flipX))
        {
            rend.flipX = !rend.flipX;
        }

    }

    bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(rend.bounds.extents.x,0.01f), 0,Vector2.down, rend.bounds.extents.y+0.1f, ~(1<<8));
        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 0)
        {
            stopSwinging = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 0)
        {
            stopSwinging = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collectible"))
        {
            switch (collision.name[0])
            {
                case 'r':
                    pendulumJump = true;
                    break;
                case 'g':
                    ++pointCollection.point;
                    break;
            }
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Hurt"))
        {
            HurtBox hurtBox = collision.GetComponent<HurtBox>();
            healthVariable.point -= hurtBox.damage;
            Destroy(hurtBox.gameObject);
        }
    }
}
