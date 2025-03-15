using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;
    public static CameraManager Inst => instance;

    [SerializeField] public Canvas Canvas;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public Transform followTransform;
    public Vector3 cameraOffset;

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, followTransform.position + cameraOffset, 5f * Time.fixedDeltaTime);
    }
}
