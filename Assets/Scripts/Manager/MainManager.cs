using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    private static MainManager instance = null;
    public static MainManager Inst => instance;

    [SerializeField] Tower tower = null;
    public float MaxX => tower.PlayerRight;

    List<Zombie> Zombies = new List<Zombie>();
    float currentspawntime;

    [SerializeField] float spawnrate = 2f;
    [SerializeField] GameObject[] zombiePrefabs;
    [SerializeField] Transform spawnTf;

    [SerializeField] LayerMask frontZombieLayer;
    [SerializeField] LayerMask backZombieLayer;
    [SerializeField] List<Color> zombiesColor;

    float backYoffset = -0.25f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        if (IsEndGame())
        {
            if (endCor == null)
                endCor = StartCoroutine(EndRoutine());
            return;
        }

        RemoveTerminated();

        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int i = 0; i < Zombies.Count; i++)
            {
                Destroy(Zombies[i].gameObject);
            }
            Zombies.Clear();
        }
    }

    private void FixedUpdate()
    {
        currentspawntime += Time.fixedDeltaTime;
        if (currentspawntime > 1 / spawnrate)
        {
            currentspawntime -= 1 / spawnrate;
            SpawnZombie();
        }
    }

    void SpawnZombie()
    {
        bool isfront = Random.value > 0.5f;
        GameObject zombie = Instantiate(zombiePrefabs[isfront ? 0 : 1], spawnTf);
        Zombie z = zombie.GetComponent<Zombie>();
        Zombies.Add(z);

        z.isFrontLine = isfront;
        z.currentZombieLayer = isfront ? frontZombieLayer : backZombieLayer;
        z.gameObject.layer = isfront ? 7 : 8;
        //zombie.GetComponent<SpriteRenderer>().color = zombiesColor[isfront ? 0 : 1];
        zombie.GetComponent<SpriteRenderer>().sortingOrder = isfront ? 0 : -1;
        float offset = isfront ? 0f : backYoffset;
        zombie.GetComponent<CapsuleCollider2D>().offset = new Vector2(0f, offset);
        z.yoffset = offset;
    }

    public void RemoveTerminated()
    {
        Zombies.ForEach(x =>
        {
            if (x.IsTerminate)
            {
                Destroy(x.gameObject);
            }
        });
        Zombies.RemoveAll(x => x.IsTerminate);
    }

    Coroutine endCor = null;
    public bool IsEndGame()
    {
        return tower.AllDieBlock();
    }
    
    IEnumerator EndRoutine()
    {
        yield return new WaitForSeconds(3f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    public Zombie FindTargetUnit(Player player)
    {
        float dist = float.MaxValue;
        Zombie target = null;

        foreach (var z in Zombies)
        {
            if (z.IsTerminate == true)
                continue;

            if (z.transform.position.x + 1 < player.transform.position.x)
                continue;

            var d = Vector2.Distance(player.transform.position, z.transform.position);
            if (d < dist)
            {
                target = z;
                dist = d;
            }
        }

        return target;
    }
}
