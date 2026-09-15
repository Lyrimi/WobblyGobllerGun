using UnityEngine;

public class CameraAspectForcer : MonoBehaviour
{
    private Camera _camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.aspect = 16f / 9f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
