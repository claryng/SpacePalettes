using UnityEngine;

public class CameraFit : MonoBehaviour
{
    [SerializeField] private float _refResolution;
    [SerializeField] private float _camSize;

    private Camera _cam;
    private float _sceneHeight;
    private Vector3 _camPos;

    private void Start()
    {
        _cam = Camera.main;
        _camPos = _cam.transform.position;
        _sceneHeight = _cam.orthographicSize;
        _cam.orthographicSize = GetHeightProportion() * _camSize / (_refResolution * 100);
        _cam.transform.position = new Vector3(_camPos.x, -1 * (_sceneHeight - _cam.orthographicSize), _camPos.z);
    }

    float GetHeightProportion()
    {
        return ((float)Screen.height * 100) / Screen.width;
    }
}
