using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private static FloatingText inst;
    public static FloatingText Inst => inst;

    [SerializeField] TMP_Text tmProOrg;
    //[SerializeField] float floatTime = 1f;

    WaitForSeconds wfs = new WaitForSeconds(0.02f);

    private void Awake()
    {
        if (inst == null)
            inst = this;

        //wfs = new WaitForSeconds(floatTime);
    }

    public void CreateText(string text, Transform mainTf)
    {
        StartCoroutine(FloatText(text, mainTf));
    }

    IEnumerator FloatText(string text, Transform mainTf)
    {
        var tp = Instantiate(tmProOrg);
        tp.transform.SetParent(CameraManager.Inst.Canvas.transform);
        Vector3 targetPos = Camera.main.WorldToScreenPoint(
            new Vector3(mainTf.position.x, mainTf.position.y + 1f, 0));
        tp.transform.position = targetPos;

        tp.text = text;

        for (int i = (int)tp.fontSize; i >= 0; i--)
        {
            tp.fontSize = i;
            yield return wfs;
        }

        Destroy(tp.gameObject);
    }
}