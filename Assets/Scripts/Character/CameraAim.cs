using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAim : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private Transform target;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        var pos = Input.mousePosition;
        var ray = cam.ScreenPointToRay(pos);
    }
}
