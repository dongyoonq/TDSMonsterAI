using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBlock : MonoBehaviour
{
    public int hp = 3; // �� ü��
    private int nowHp = 0;

    public float moveSpeed = 2f; // �̵� �ӵ�
    private Tower tower; // ���� ���� Ÿ��

    private HpBar hpBar = null;

    [SerializeField] SpriteRenderer rnd = null;

    void Awake()
    {
        nowHp = hp;
    }

    private void Start()
    {
        var hpObj = Resources.Load<HpBar>("hpBar");
        hpBar = Instantiate(hpObj);
        hpBar.Init(gameObject, 1.5f, Vector3.left);
    }

    public void Init(Tower tower, int n)
    {
        this.tower = tower;
        rnd.color = n % 2 == 0 ? new Color(0.74f, 0.31f, 0.19f) : new Color(0.74f, 0.4f, 0.3f);
        rnd.sortingOrder = n % 2 == 0 ? 1 : -1;
    }

    public void TakeDamage(int amount)
    {
        nowHp -= amount;
        hpBar.UpdateHpBar(nowHp, hp);

        FloatingText.Inst.CreateText(amount.ToString(), transform);

        if (nowHp <= 0)
        {
            if (hpBar != null)
                Destroy(hpBar.gameObject);
            tower.RemoveBlock(this); // �� ���� ��û
        }
    }

    public IEnumerator MoveDownCoroutine(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.localPosition;
        float elapsedTime = 0f;
        float duration = Mathf.Abs(targetPosition.y - startPosition.y) / moveSpeed; // �̵� �ð� ���

        while (elapsedTime < duration)
        {
            if (transform == null || !transform.gameObject.activeInHierarchy)
                yield break;

            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition; // ������ ��ġ ����
    }
}