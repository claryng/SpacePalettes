using System.Collections;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private float _duration = .75f;

    private Material _material;
    private BoxCollider2D _boxCollider;
    private float fade = 1f;

    private void Start()
    {
        _material = GetComponent<SpriteRenderer>().material;
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        SpawnedObjectsController.OnDissolve += StartDissolve;
    }

    private void OnDisable()
    {
        SpawnedObjectsController.OnDissolve -= StartDissolve;
    }
    private void StartDissolve()
    {
        StartCoroutine(DissolveCoroutine());
    }

    private IEnumerator DissolveCoroutine()
    {
        if (_boxCollider)
        {
            Destroy( _boxCollider );
        }

        for(var t = 0f; t < _duration; t += Time.deltaTime)
        {
            fade = Mathf.Lerp(fade, 0f, t/_duration);
            _material.SetFloat("_Fade", fade);
            yield return null;
        }

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
