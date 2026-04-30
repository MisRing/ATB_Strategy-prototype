using System.Collections.Generic;
using UnityEngine;

public class SceneEntryPoint : MonoBehaviour
{
    [Header("Teams controls")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private EnemyController _enemyController;
    
    [Header("Start parameters")]
    [SerializeField] private Vector3 _unitsStartDirection = Vector3.right;
    
    [Header("Units instance settings")]
    [SerializeField] private List<UnitController> _playerUnits = new List<UnitController>();
    [SerializeField] private List<Vector3Int> _playerPositionPreset = new List<Vector3Int>();
    [SerializeField] private List<UnitController> _enemyUnits = new List<UnitController>();
    private UnitAIController[] _enemyAIControls;
    [SerializeField] private List<Vector3Int> _enemyPositionPreset = new List<Vector3Int>();

    private void Awake()
    {
        Debug.Log("Initializing start...");
        if(GridParameters.LevelGrid == null)
        {
            GridParameters.LevelGrid = FindFirstObjectByType(typeof(GridMap)) as GridMap;
        }
        
        SetUnitsOnLevel();
        
        _playerController.Init(_playerUnits);
        _enemyController.Init(_enemyUnits, _enemyAIControls);
        _playerController.CameraController.Init();

        Debug.Log("Initializing complete.");
    }

    private void SetUnitsOnLevel()
    {
        Quaternion startDirection = Quaternion.LookRotation(_unitsStartDirection);
        for (int i = 0; i < _playerUnits.Count; i++)
        {
            _playerUnits[i].Init(GridParameters.LevelGrid.GetTile(_playerPositionPreset[i].x, _playerPositionPreset[i].z, _playerPositionPreset[i].y));
            _playerUnits[i].gameObject.transform.rotation = startDirection;
        }
        
        _enemyAIControls = new UnitAIController[_enemyUnits.Count];
        for (int i = 0; i < _enemyUnits.Count; i++)
        {
            _enemyUnits[i].Init(GridParameters.LevelGrid.GetTile(_enemyPositionPreset[i].x, _enemyPositionPreset[i].z, _enemyPositionPreset[i].y));
            _enemyUnits[i].gameObject.transform.rotation = startDirection;
            _enemyAIControls[i] = _enemyUnits[i].gameObject.AddComponent<UnitAIController>();
            _enemyAIControls[i].Init();
        }
    }
    
    //-------------------------------DEBUG-GIZMO------------------------------------
    
    private void OnDrawGizmos()
    {
        if(GridParameters.LevelGrid == null)
        {
            GridParameters.LevelGrid = FindFirstObjectByType(typeof(GridMap)) as GridMap;
        }

        foreach(Vector3Int point in _playerPositionPreset)
        {
            if(GridParameters.LevelGrid.CheckTile(point.x, point.z, point.y))
            {
                Gizmos.color = Color.darkGreen;
                Gizmos.DrawSphere(GridParameters.LevelGrid.GetTileWorldPos(point.x, point.z, point.y), 0.3f);
            }
        }
        
        foreach(Vector3Int point in _enemyPositionPreset)
        {
            if(GridParameters.LevelGrid.CheckTile(point.x, point.z, point.y))
            {
                Gizmos.color = Color.darkRed;
                Gizmos.DrawSphere(GridParameters.LevelGrid.GetTileWorldPos(point.x, point.z, point.y), 0.3f);
            }
        }
    }
}
