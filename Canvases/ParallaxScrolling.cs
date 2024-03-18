using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simulate parallax scrolling
/// </summary>
public class ParallaxScrolling : MonoBehaviour
{
    [SerializeField] private Vector2 _speed;

    private RawImage _image;

    private void Awake()
    {
        _image = GetComponent<RawImage>();
    }

    private void Start()
    {
        // Reset size and position.
        _image.uvRect = new Rect(Vector2.zero, _image.uvRect.size);
    }

    private void Update()
    {
        var newPos = new Vector2(0f, (_speed.y * Time.deltaTime) % 1);

        _image.uvRect = new Rect(_image.uvRect.position + newPos, _image.uvRect.size);
    }
}
