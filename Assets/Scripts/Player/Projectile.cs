using System.Collections;
using UnityEngine;

public class Bezier
{
    public Vector2 p0; // 시작점
    public Vector2 p1; // 제어점
    public Vector2 p2; // 도착점

    public void SetFrom(Vector2 p0) => this.p0 = p0;
    public void SetTo(Vector2 p2) => this.p2 = p2;

    public void SetControlPoint(Vector2 control) => p1 = control;

    public Vector2 GetPoint(float t)
    {
        return Mathf.Pow(1 - t, 2) * p0 +
               2 * (1 - t) * t * p1 +
               Mathf.Pow(t, 2) * p2;
    }

    public Vector2 GetTangent(float t)
    {
        return 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
    }

    public void AutoSetControlPoint()
    {
        var randY = UnityEngine.Random.Range(-2f, 4f);
        var randX = UnityEngine.Random.Range(4f, 8f);
        p1 = p0 + (Vector2.up * randY) + (Vector2.right * randX);  // 곡선 높이 조절 가능
    }
}

public class Projectile : MonoBehaviour
{
    public enum ProjectileType { Missile }

    ProjectileType pType;
    Bezier bezier;
    float duration = 1.5f; // 투사체 이동 시간
    bool isTerminated = false;

    [SerializeField] float prjDmg = 1.0f;
    float dmg;

    public void SetData(ProjectileType prjType, Transform from, Transform to, float dmgRate)
    {
        pType = prjType;

        bezier = new Bezier();
        bezier.SetFrom(from.position);
        bezier.SetTo(to.position);
        bezier.AutoSetControlPoint(); // 자동으로 곡선의 제어점 설정

        StartCoroutine(Process());

        dmg = prjDmg * (1 + dmgRate);
    }

    IEnumerator Process()
    {
        switch (pType)
        {
            case ProjectileType.Missile:
                return Missile();
        }

        return null;
    }

    IEnumerator Missile()
    {
        float time = 0f;
        while (time < duration)
        {
            if (isTerminated)
                yield break;

            float t = time / duration;
            Vector2 currentPos = bezier.GetPoint(t);
            transform.position = currentPos;

            // ⭐ 방향 설정: 이동 방향으로 자연스럽게 회전 ⭐
            Vector2 nextPos = bezier.GetPoint(Mathf.Min(t + 0.05f, 1)); // 조금 앞의 위치 계산
            Vector2 direction = (nextPos - currentPos).normalized; // 현재 위치에서 다음 위치로 향하는 벡터

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 10f);

            time += Time.deltaTime;

            yield return null;
        }

        transform.position = bezier.GetPoint(1); // 마지막 위치 보정
        Destroy(gameObject); // 도착하면 삭제
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var z = collision.GetComponent<Zombie>();
        if (z != null)
        {
            isTerminated = true;
            z.TakeDamage(dmg);
            Destroy(gameObject); // 도착하면 삭제
        }
    }
}
