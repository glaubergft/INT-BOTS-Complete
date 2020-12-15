using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAim : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private Transform target;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        var pos = Input.mousePosition;
        var ray = cam.ScreenPointToRay(pos);
    }
}
