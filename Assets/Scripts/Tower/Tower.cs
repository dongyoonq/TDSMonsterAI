using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private Vector2 aheadCheckOffset = new Vector2(0f, 0f);
    [SerializeField] private LayerMask fZombie;
    [SerializeField] private LayerMask bZombie;

    public float PlayerRight => player.transform.position.x + (player.transform.localScale.x / 2f);

    private bool isZombieAhead;
    private Rigidbody2D rb;

    [SerializeField] TowerBlock mainBlock = null;
    [SerializeField] GameObject blockPrefab; // 블럭 프리팹
    [SerializeField] Player player; // 플레이어 오브젝트
    [SerializeField] int towerHeight = 5; // 블럭 개수

    private List<TowerBlock> blocks = new List<TowerBlock>(); // 블럭 리스트
    public bool AllDieBlock() => player.IsTerminate && blocks.Count <= 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GenerateTower(towerHeight);
    }

    void FixedUpdate()
    {
        isZombieAhead = IsZombieAhead();
        if (!isZombieAhead)
        {
            rb.AddForce(Vector2.right * rb.mass * moveSpeed);
        }
        if (rb.velocity.x > moveSpeed)
        {
            rb.velocity = new Vector2(moveSpeed, 0f);
        }
    }

    public void GenerateTower(int n)
    {
        mainBlock.transform.localPosition = Vector3.zero;
        mainBlock.Init(this, 0);
        blocks.Add(mainBlock);

        for (int i = 1; i < n; i++)
        {
            GameObject blockObj = Instantiate(blockPrefab, transform);
            blockObj.transform.localPosition = new Vector3(0, i, 0);
            TowerBlock block = blockObj.GetComponent<TowerBlock>();
            block.Init(this, i);
            blocks.Add(block);
        }

        // 플레이어를 마지막 블럭 위로 배치
        if (player != null)
        {
            player.transform.localPosition = new Vector3(0, n - 0.5f, 0);
        }
    }

    public void RemoveBlock(TowerBlock block)
    {
        int index = blocks.IndexOf(block);
        if (index == -1) return;

        blocks.RemoveAt(index); // 리스트에서 제거
        Destroy(block.gameObject); // 블럭 삭제

        // 위에 있는 블럭들 아래로 이동
        for (int i = index; i < blocks.Count; i++)
        {
            Vector3 newPosition = new Vector3(0, i, 0); // blocks[i].transform.position + Vector3.down;
            StartCoroutine(blocks[i].MoveDownCoroutine(newPosition));
        }

        // ⭐ 플레이어가 마지막 블럭 위에 있어야 함
        if (player != null)
        {
            if (blocks.Count > 0)
            {
                Vector3 newPlayerPos = new Vector3(0, blocks.Count - 0.5f, 0);
                StartCoroutine(MovePlayer(newPlayerPos));
            }
            else
            {
                Debug.Log("타워가 완전히 무너졌습니다!");
            }
        }
    }

    bool IsZombieAhead()
    {
        Vector2 checkPos = (Vector2)transform.position + aheadCheckOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, checkRadius, fZombie | bZombie);
        foreach (Collider2D col in hits)
        {
            if (col.gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator MovePlayer(Vector3 targetPosition)
    {
        Vector3 startPosition = player.transform.localPosition;
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            player.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.transform.localPosition = targetPosition; // 마지막 위치 보정
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 aheadcheckPos = (Vector2)transform.position + aheadCheckOffset;
        Gizmos.DrawWireSphere(aheadcheckPos, checkRadius);
    }
}