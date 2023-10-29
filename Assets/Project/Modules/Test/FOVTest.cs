using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVTest : MonoBehaviour
{
    [Serializable]
    public struct Instanceables
    {
        public GameObject prefab;
        public float distance;
        public int amount;
    }

    [SerializeField] private GameObject _camera;
    private Camera _cam;
    [SerializeField] private GameObject screenMarker;
    [SerializeField] private List<Instanceables> _instanceablesList = new();

    private float yPos = 0.85f;

    private float fov;
    private float dist;

    private Vector3 point;

    private void Start()
    {
        _cam = _camera.GetComponent<Camera>();
        
        foreach (var inst in _instanceablesList)
        {
            for (int i = 0; i < inst.amount; i++)
            {
                for (int j = 0; j < inst.amount; j++)
                {
                    //Instantiate(inst.prefab, new Vector3(i * inst.distance, yPos, j * inst.distance), Quaternion.identity);
                    //Instantiate(inst.prefab, new Vector3(-i * inst.distance, yPos, j * inst.distance), Quaternion.identity);
                    //Instantiate(inst.prefab, new Vector3(i * inst.distance, yPos, -j * inst.distance), Quaternion.identity);
                    //Instantiate(inst.prefab, new Vector3(-i * inst.distance, yPos, -j * inst.distance), Quaternion.identity);
                }
            }
        }
        
        //get 10% screen pos from left to right
        //put a stick there
        
        
        //Instantiate(screenMarker, _cam.ScreenToWorldPoint(new Vector3(_cam.pixelWidth / 10, _cam.pixelHeight / 2, 0)), Quaternion.identity);
        //Instantiate(screenMarker, _cam.ScreenToWorldPoint(new Vector3(1000, 0, _cam.nearClipPlane)), Quaternion.identity);

        StartCoroutine(AAAAA());
    }

    private IEnumerator AAAAA()
    {
        yield return new WaitForSeconds(3.0f);
        
        //Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray r = Camera.main.ScreenPointToRay(new Vector2(_cam.pixelWidth / 10, _cam.pixelHeight / 2));
        Debug.DrawRay(r.origin , r.direction * 100, Color.red, 100, true);

        Vector3 pos = r.GetPoint(_camera.GetComponent<OrbitingCamera>()._distance);
        Debug.Log(pos);
        Instantiate(screenMarker, pos, Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            fov = _cam.fieldOfView;
            dist = _camera.GetComponent<OrbitingCamera>()._distance;
            
            ScreenCapture.CaptureScreenshot("screenshot_lone_" + "Fov" + fov + "_Dist" + dist + System.DateTime.Now.ToString(" MM-dd-yy (HH-mm-ss)") + ".png");
            Debug.Log("Screenshot taken" + System.DateTime.Now.ToString("MM-dd-yy (HH-mm-ss)"));
        }

        //_cam.fieldOfView = _camera.GetComponent<OrbitingCamera>()._distance;
    }

    void OnGUI()
    {
        Event   currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = _cam.pixelHeight - currentEvent.mousePosition.y;

        point = _cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0.0f));

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + _cam.pixelWidth + ":" + _cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + point.ToString("F3"));
        GUILayout.EndArea();
    }
}
