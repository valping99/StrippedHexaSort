using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SpawnerHexTileController : MonoBehaviour
{
    [SerializeField] private SelectedHexTileGroupSO _selectedHexTileGroup;
    [SerializeField] private HexTileBehaviour _hexTilePrefab;
    [SerializeField] private HexTileGroup _hexBase;
    [SerializeField] private Transform _hexParent;
    [SerializeField] private Transform _defaultSpawnPoint;
    [SerializeField] private List<TileGroupSO> _listGroupSO;
    [SerializeField] private List<HexTileGroup> _listSpawn;
    [SerializeField] private int _objectToSpawn;
    [SerializeField] private int _baseToSpawn;
    [SerializeField] private float _spaceLength;
    [SerializeField] private float _movingTimeDuration;

    private int totalSpawnerValue = 0;
    private List<TileGroupSO> _usedTileGroups;

    private void OnEnable()
    {
        _selectedHexTileGroup.OnEventRaised += OnSelectedHexTile;
    }

    private void OnDisable()
    {
        _selectedHexTileGroup.OnEventRaised -= OnSelectedHexTile;
    }

    private async void Start()
    {
        await SpawnEditorGroup();
    }

    private void OnSelectedHexTile(HexTileGroup hexTileGroup)
    {
        if (hexTileGroup == null) return;
        if (_listSpawn.Contains(hexTileGroup))
        {
            _listSpawn.Remove(hexTileGroup);
        }

        if (_listSpawn.Count == 0)
        {
            SpawnHexTileGroup().Forget();
        }
    }

    public async UniTask SpawnEditorGroup()
    {
        ClearObjects(_hexParent);
        _listSpawn = new();
        totalSpawnerValue = 0;
        float space = 0;
        for (int i = 0; i < _baseToSpawn; i++)
        {
            Vector3 newPosition =
                new Vector3(_hexParent.position.x + space, _hexParent.position.y, _hexParent.position.z);
            var baseTile = Instantiate(_hexBase, newPosition, Quaternion.identity, _hexParent);
            baseTile.ClearHexTileConfigurations();
            SpawnHexTile(baseTile);
            _listSpawn.Add(baseTile);
            space += _spaceLength;
        }
    }

    private async UniTask SpawnHexTileGroup()
    {
        AudioManager.Instance.PlaySfxSource(EAudioSfxTracking.Sfx_HexSpawn);
        totalSpawnerValue = 0;
        float space = 0;

        for (int i = 0; i < _baseToSpawn; i++)
        {
            Vector3 newPosition = GetNextSpawnPosition(space);
            var baseTile = CreateBaseTile();
            SpawnHexTile(baseTile);
            _listSpawn.Add(baseTile);
            var moveToDestination = MoveToSpawnPoint(baseTile.transform, newPosition);
            await UniTask.WhenAny(moveToDestination);
            baseTile.ToggleInputAction(true);
            space += _spaceLength;
        }

    }

    private Vector3 GetNextSpawnPosition(float space)
    {
        return new Vector3(_hexParent.position.x + space, _hexParent.position.y, _hexParent.position.z);
    }

    private HexTileGroup CreateBaseTile()
    {
        var baseTile = Instantiate(_hexBase, _defaultSpawnPoint.position, Quaternion.identity, _hexParent);
        baseTile.ClearHexTileConfigurations();
        return baseTile;
    }

    private async UniTask MoveToSpawnPoint(Transform hexTile, Vector3 destination)
    {
        Sequence moveSequence = DOTween.Sequence();

        moveSequence.Append(hexTile.transform.DOMove(destination, _movingTimeDuration).SetEase(Ease.OutQuart));

        await moveSequence.ToUniTask();
    }

    private void SpawnHexTile(HexTileGroup hexTileGroup)
    {
        totalSpawnerValue = 0;
        float height = 0f;
        _usedTileGroups = new List<TileGroupSO>(_listGroupSO);
        for (int i = 0; i < _usedTileGroups.Count; i++)
        {
            if (totalSpawnerValue >= _objectToSpawn) break;

            TileGroupSO randomGroup = _usedTileGroups[Random.Range(0, _usedTileGroups.Count)];
            int spawnCount = Random.Range(randomGroup.Min, randomGroup.Max + 1);

            spawnCount = Mathf.Min(spawnCount, _objectToSpawn - totalSpawnerValue);

            for (int j = 0; j < spawnCount; j++)
            {
                height += _hexTilePrefab.Height;
                Vector3 position = new Vector3(0, height, 0);
                SpawnHexTile(randomGroup, hexTileGroup, position);
                totalSpawnerValue++;
            }

            _usedTileGroups.Remove(randomGroup);

            if (totalSpawnerValue >= _objectToSpawn) break;
        }
    }

    private void SpawnHexTile(TileGroupSO group, HexTileGroup hexTileGroup, Vector3 position)
    {
        var hexTile = Instantiate(_hexTilePrefab, hexTileGroup.transform);
        hexTile.Configuration(group);
        hexTile.transform.localPosition = position;
        hexTile.SetOriginalPosition(position);
        hexTileGroup.AddHexTileConfiguration(hexTile);
    }

    private void ClearObjects(Transform transformToClear)
    {
        List<GameObject> hexTiles = new List<GameObject>();
        for (int i = 0; i < transformToClear.childCount; i++)
        {
            hexTiles.Add(transformToClear.GetChild(i).gameObject);
        }

        foreach (var hex in hexTiles)
        {
            DestroyImmediate(hex, true);
        }
    }
}