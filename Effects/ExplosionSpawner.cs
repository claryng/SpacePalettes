using System.Collections;
using UnityEngine;
using ColorsAvailable;
using System.Collections.Generic;

public class ExplosionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _whiteExp;
    [SerializeField] private GameObject _purpleExp;
    [SerializeField] private GameObject _greenExp;
    [SerializeField] private GameObject _special;
    [SerializeField] private GameObject _shield;
    [SerializeField] private Player _player;
    [SerializeField] private SpawnedObjectsController _spawnedObjectsController;
    [SerializeField] private float _padding = .5f;
    [SerializeField] private float _duration = .5f;

    private Dictionary<Colors, GameObject> _exps;
    private Transform _t;
    private GameObject _particleSystem;

    private void OnEnable()
    {
        _t = _player.transform;
        _player.OnExplode += StartExploding;
        _spawnedObjectsController.OnDestroyHitBars += StartDestroying;
    }

    private void OnDisable()
    {
        _player.OnExplode -= StartExploding;
        _spawnedObjectsController.OnDestroyHitBars -= StartDestroying;
    }

    private void Start()
    {
        _exps = new Dictionary<Colors, GameObject>
        {
            { Colors.Purple, _purpleExp },
            { Colors.Green, _greenExp },
            { Colors.White, _whiteExp }
        };
    }

    private void StartExploding(Colors color, string name)
    {
        StartCoroutine(Explode(color, name));
    }

    private void StartDestroying(string color, Vector3 pos)
    {
        StartCoroutine(DestroyBars(color, pos));
    }

    private IEnumerator Explode(Colors color, string name)
    {
        if (_exps.Count > 0 && _exps.ContainsKey(color))
        {
            _particleSystem = _exps[color];
        }
        else
        {
            if(name == "Shield")
            {
                _particleSystem = _shield;
            }else if (name == "Special")
            {
                _particleSystem = _special;
            }
            else
            {
                _particleSystem = _whiteExp;
            }
        }

        GameObject instance = Instantiate(_particleSystem, Helpers.GetVector3Y
            (_t.position.y + _padding, _t.position.x), Quaternion.identity);
        yield return Helpers.GetWaitForSeconds(_duration);
        Destroy(instance);
    }

    private IEnumerator DestroyBars(string color, Vector3 pos)
    {
        _particleSystem = color switch
        {
            "Purple" => _purpleExp,
            "Green" => _greenExp,
            "White" => _whiteExp,
            _ => _whiteExp,
        };
        GameObject instance = Instantiate(_particleSystem, pos, Quaternion.identity);
        yield return Helpers.GetWaitForSeconds(_duration);
        Destroy(instance);
    }
}
