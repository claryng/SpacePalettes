using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> SpawnedObjects { get; private set; }

    [SerializeField] private float _timeToAddToTimer = 0.25f;

    [Header("Time to spawn Special")]
    [SerializeField] private float _maxSpawnSpecial = 0.4f;
    [SerializeField] private float _minSpawnSpecial = 0.25f;
    [SerializeField] private float _decreaseTimeSpecial = 0.1f;

    [Header("Objects")]
    [SerializeField] private ColorBarCreator _creator;
    [SerializeField] private SpecialObject _specialObjectRandomPos;
    [SerializeField] private GameObject _specialObject;
    [SerializeField] private GameObject _shield;
    [SerializeField] private Player _player;

    [SerializeField] private float _timeToSpawnSpecial;
    [SerializeField] private float _timeToSpawnShield = 0.0009f;

    private float _timerToSpawnSpecial;
    private float _timerToSpawnShield;
    private bool _hasCollided;

    private void Awake()
    {
        SpawnedObjects = new List<GameObject>();
    }

    private void Start()
    {
        _timeToSpawnSpecial = _maxSpawnSpecial;
        Spawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!_hasCollided)
        {
            Spawn();
            _hasCollided = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(_hasCollided)
        {
            _hasCollided = false;
        }
    }

    private void Update()
    {
        if(SpawnedObjects.Count == 0)
        {
            Spawn();
            _hasCollided = false;
        }
    }
    private void Spawn()
    {
        CheckDestroyedBars();
        GameObject instance;
        if (_timerToSpawnSpecial < _timeToSpawnSpecial)
        {
            instance = _creator.OnGenerate?.Invoke(_player.PlayerColor);
            _timerToSpawnSpecial += Time.deltaTime * _timeToAddToTimer;
        }
        else
        {
            _timerToSpawnSpecial = 0;
            if (_timerToSpawnShield < _timeToSpawnShield)
            {
                instance = _specialObjectRandomPos.OnGenerateSpecialObject?.Invoke(_specialObject);
                _timeToSpawnSpecial -= Time.deltaTime * _decreaseTimeSpecial;
                _timeToSpawnSpecial = Mathf.Clamp(_timeToSpawnSpecial,
                    _minSpawnSpecial, _maxSpawnSpecial);

                _timerToSpawnShield += Time.deltaTime * _timeToAddToTimer;
            }
            else
            {
                _timerToSpawnShield = 0;
                instance = _specialObjectRandomPos.OnGenerateSpecialObject?.Invoke(_shield);
            }
        }
        SpawnedObjects.Add(instance);
    }
    private void CheckDestroyedBars()
    {
        if (SpawnedObjects.Count > 0)
        {
            for (var i = 0; i < SpawnedObjects.Count; i++)
            {
                GameObject tempObj = SpawnedObjects[i];

                if (!tempObj)
                {
                    SpawnedObjects.RemoveAt(i);
                }
            }
        }
    }
}
