using UnityEngine;

public class Zombie : MonoBehaviour
{
    public bool isFrontLine = false;
    public float yoffset = 0f;
    public LayerMask currentZombieLayer;

    [SerializeField] private float climbForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private Vector2 aboveCheckOffset = new Vector2(0f, 0.6f);
    [SerializeField] private Vector2 aheadCheckOffset = new Vector2(-0.3f, 0f);
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f);

    [SerializeField] private bool isGround;
    [SerializeField] private bool isRealGround;
    [SerializeField] private bool isZombieAhead;
    [SerializeField] private bool isZombieAbove;

    [SerializeField] private int hp = 10;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCoolTime = 2f;

    private int nowHp = 0;

    private Rigidbody2D rb;
    private float jumpTimer;
    private float attackTimer = 0f;

    private bool isTerminate;
    public bool IsTerminate => isTerminate;
    public void SetTerminate() 
    {
        if (hpBar != null)
            Destroy(hpBar.gameObject);
        isTerminate = true;
    }

    private HpBar hpBar = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isTerminate = false;
        nowHp = hp;
    }

    private void Start()
    {
        hpBar = ResourceManager.Inst.Instantiate<HpBar>("hpBar", true);
        //hpBar = Instantiate(hpObj);
        hpBar.Init(gameObject, 0.5f, new Vector2(0.5f, 1.5f));
    }

    void FixedUpdate()
    {
        isGround = IsGround();
        isRealGround = IsRealGround();
        isZombieAhead = IsZombieAhead();
        isZombieAbove = IsZombieAbove();

        // 특정 x 지점을 넘었을 때 현재 이동 방향의 반대 방향으로 힘을 준다.
        if (MainManager.Inst.IsEndGame() == false &&
            transform.position.x < MainManager.Inst.MaxX)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            rb.AddForce(new Vector2(22f, 0f), ForceMode2D.Impulse);
        }
        else
        {
            if (isRealGround && isZombieAbove)
            {
                rb.AddForce(new Vector2(moveSpeed * 11f, 0f));
            }
            else
            {
                rb.AddForce(new Vector2(-moveSpeed * 10f, 0f));
            }

            if (rb.velocity.x < -moveSpeed)
            {
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            }
            else if (rb.velocity.x > moveSpeed / 3f)
            {
                rb.velocity = new Vector2(moveSpeed / 3f, rb.velocity.y);
            }

            if (jumpTimer > 0f)
            {
                jumpTimer -= Time.fixedDeltaTime;
            }

            if (isGround)
            {
                if (jumpTimer <= 0f && rb.velocity.x < 0f && !isZombieAbove && isZombieAhead)
                {
                    Vector2 v = rb.velocity;
                    v.y = climbForce;
                    v.x = climbForce / 3f;
                    rb.velocity = v;

                    jumpTimer = jumpCooldown;
                }
            }
        }

        FindTarget();
    }

    bool IsZombieAbove()
    {
        Vector2 checkPos = (Vector2)transform.position + aboveCheckOffset + new Vector2(0f, yoffset);
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, checkRadius, currentZombieLayer);
        foreach (Collider2D col in hits)
        {
            if (col.gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    bool IsZombieAhead()
    {
        Vector2 checkPos = (Vector2)transform.position + aheadCheckOffset + new Vector2(0f, yoffset);
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, checkRadius, currentZombieLayer);
        foreach (Collider2D col in hits)
        {
            if (col.gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    bool IsGround()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset + new Vector2(0f, yoffset);
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, checkRadius, groundLayer | currentZombieLayer);
        foreach (Collider2D col in hits)
        {
            if (col.gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    bool IsRealGround()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset + new Vector2(0f, yoffset);
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, checkRadius, groundLayer);
        foreach (Collider2D col in hits)
        {
            if (col.gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    void FindTarget()
    {
        if (isZombieAhead)
            return;

        if (attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
        }
        else
        {
            attackTimer = 0;
            Vector2 checkPos = (Vector2)transform.position + aheadCheckOffset + new Vector2(0f, yoffset);
            Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, checkRadius);
            foreach (Collider2D col in hits)
            {
                var tw = col.GetComponent<TowerBlock>();
                if (tw != null)
                {
                    tw.TakeDamage(damage);
                    attackTimer = attackCoolTime;
                }
                else
                {
                    var player = col.GetComponent<Player>();
                    if (player != null)
                    {
                        player.TakeDamage(damage);
                        attackTimer = attackCoolTime;
                    }
                }
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        hpBar.UpdateHpBar(nowHp, hp);
        nowHp -= (int)dmg;

        FloatingText.Inst.CreateText(((int)dmg).ToString(), transform);

        if (nowHp <= 0)
        {
            SetTerminate();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 abovecheckPos = (Vector2)transform.position + aboveCheckOffset + new Vector2(0f, yoffset);
        Gizmos.DrawWireSphere(abovecheckPos, checkRadius);

        Vector2 aheadcheckPos = (Vector2)transform.position + aheadCheckOffset + new Vector2(0f, yoffset);
        Gizmos.DrawWireSphere(aheadcheckPos, checkRadius);

        Vector2 groundcheckPos = (Vector2)transform.position + groundCheckOffset + new Vector2(0f, yoffset);
        Gizmos.DrawWireSphere(groundcheckPos, checkRadius);
    }
}