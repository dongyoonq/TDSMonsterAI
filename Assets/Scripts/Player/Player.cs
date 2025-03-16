using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int hp = 3; // Ã¼·Â
    private int nowHp = 0;
    private bool isTerminate = false;
    public bool IsTerminate => isTerminate;

    private HpBar hpBar = null;

    [SerializeField] Weapon weapon = null;
    [SerializeField] BoxCollider2D block = null;

    Zombie target = null;

    void Awake()
    {
        nowHp = hp;
        isTerminate = false;
    }

    private void Start()
    {
        hpBar = ResourceManager.Inst.Instantiate<HpBar>("hpBar", true);
        //hpBar = Instantiate(hpObj);
        hpBar.Init(gameObject, 1.3f, Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTerminate)
            return;

        FindUnitTarget();

        if (weapon != null) 
        { 
            weapon.UseWeapon(target);
        }
    }

    void FindUnitTarget()
    {
        target = MainManager.Inst.FindTargetUnit(this);
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
            block.enabled = false;
            isTerminate = true;
        }
    }
}
