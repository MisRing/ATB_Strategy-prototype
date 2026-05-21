using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntryPoint : MonoBehaviour
{
    [SerializeField] private CameraController _camera;
    [Header("Teams controls")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private List<TeamControls> _enemyTeams;
    
    [Header("Start parameters")]
    [SerializeField] private Vector3 _unitsStartDirection = Vector3.right;
    
    [Header("Units instance settings")]
    [SerializeField] private List<UnitController> _playerUnits = new List<UnitController>();
    [SerializeField] private List<Vector3Int> _playerPositionPreset = new List<Vector3Int>();

    [Header("Fog of War settings")]
    [SerializeField] private FogOfWarRenderer _fogOfWarRenderer;


    private void Awake()
    {
        Debug.Log("Initializing start...");

        if(GridParameters.LevelGrid == null)
        {
            GridParameters.LevelGrid = FindFirstObjectByType(typeof(GridMap)) as GridMap;
        }

        SetUnitsOnLevel();

        _playerController?.Init(_playerUnits);

        foreach (TeamControls team in _enemyTeams)
        {
            team.Controller.Init(team.Owner, team.Units, team.AIs);
        }
        
        _camera.Init();
        
        _fogOfWarRenderer.Initialize();
        FogOfWarUtility.Renderer = _fogOfWarRenderer;

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

        foreach (TeamControls team in _enemyTeams)
        {
            team.AIs = new UnitAIController[team.Units.Count];
            for (int i = 0; i < team.Units.Count; i++)
            {
                team.Units[i].Init(GridParameters.LevelGrid.GetTile(team.StartPositions[i].x, team.StartPositions[i].z, team.StartPositions[i].y));
                team.Units[i].Owner = team.Owner;
                team.Units[i].gameObject.transform.rotation = startDirection;
                team.AIs[i] = team.Units[i].gameObject.AddComponent<UnitAIController>();
                team.AIs[i].Init();
            }
        }
    }
    
    //-------------------------------DEBUG-GIZMO------------------------------------
#if  UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(GridParameters.LevelGrid == null)
        {
            GridParameters.LevelGrid = FindFirstObjectByType(typeof(GridMap)) as GridMap;
        }

        if (_playerController != null)
        {
            foreach (Vector3Int point in _playerPositionPreset)
            {
                if (GridParameters.LevelGrid.CheckTile(point.x, point.z, point.y))
                {
                    Gizmos.color = Color.darkGreen;
                    GridTile tile = GridParameters.LevelGrid.GetTile(point.x, point.z, point.y);
                    if (tile != null)
                    {
                        Gizmos.DrawSphere(tile.WorldPosition, 0.3f);
                    }
                }
            }
        }

        foreach (TeamControls team in _enemyTeams)
        {
            foreach (Vector3Int point in team.StartPositions)
            {
                if (GridParameters.LevelGrid.CheckTile(point.x, point.z, point.y))
                {
                    Gizmos.color = Color.darkRed;
                    GridTile tile = GridParameters.LevelGrid.GetTile(point.x, point.z, point.y);
                    if (tile != null)
                    {
                        Gizmos.DrawSphere(tile.WorldPosition, 0.3f);
                    }
                }
            }
        }
    }
#endif
}

[Serializable]
public class TeamControls
{
    public UnitOwner Owner;
    public EnemyController Controller;
    public List<UnitController> Units;
    public UnitAIController[] AIs;
    public List<Vector3Int> StartPositions;
}
