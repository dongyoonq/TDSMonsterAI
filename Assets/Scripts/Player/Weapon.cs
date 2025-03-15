using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Transform mainTf = null;
    [SerializeField] Projectile proj;

    [SerializeField] float atkRate = 0.15f;

    float time = 0f;
    [SerializeField] float coolTime = 0.5f;

    bool nowCoolTime = false;

    private void Update()
    {
        if (nowCoolTime == true)
        {
            time += Time.deltaTime;
            if (time > coolTime)
            {
                time = 0f;
                nowCoolTime = false;
            }
        }
    }

    public void UseWeapon(Zombie zombie)
    {
        if (nowCoolTime == false)
        {
            CreateProjectile(zombie);
            nowCoolTime = true;
        }
    }

    void CreateProjectile(Zombie zombie)
    {
        if (zombie != null)
        {
            var prj = Instantiate(proj);
            prj.SetData(Projectile.ProjectileType.Missile, mainTf, zombie.transform, atkRate);
        }
    }
}
