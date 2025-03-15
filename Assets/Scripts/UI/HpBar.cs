using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    GameObject owner;

    [SerializeField] RectTransform mainObj = null;
    [SerializeField] Image fillImg = null;

    private float offset;
    private Vector3 dir;

    public void Init(GameObject from, float offset, Vector3 dir)
    {
        owner = from;
        this.offset = offset;
        this.dir = dir;
    }

    private void Start()
    {
        if (owner == null)
            return;

        transform.SetParent(CameraManager.Inst.Canvas.transform);
        mainObj.gameObject.SetActive(false);
        fillImg.fillAmount = 1f;

        //Vector3 hpBarPos =
        //    Camera.main.WorldToScreenPoint(
        //        new Vector3(owner.transform.position.x, owner.transform.position.y + height, 0));
        //transform.position = hpBarPos;
    }

    private void Update()
    {
        var world = owner.transform.position + (dir * offset);
        Vector3 hpBarPos =
            Camera.main.WorldToScreenPoint(world);
        transform.position = hpBarPos;
    }

    public void UpdateHpBar(float cur, float max)
    {
        var fill = cur / max;
        fillImg.fillAmount = fill;

        if (showCor != null)
            StopCoroutine(showCor);
        showCor = StartCoroutine(ShowRoutine());
    }

    WaitForSeconds wfs = new WaitForSeconds(1f);
    Coroutine showCor = null;
    IEnumerator ShowRoutine()
    {
        mainObj.gameObject.SetActive(true);
        yield return wfs;
        mainObj.gameObject.SetActive(false);
    }
}