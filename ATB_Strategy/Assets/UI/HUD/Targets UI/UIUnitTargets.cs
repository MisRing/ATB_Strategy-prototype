using System;
using UnityEngine;
using System.Collections.Generic;

public class UIUnitTargets : MonoBehaviour
{
    [SerializeField] private GameObject _targetIconPref;

    private List<UITargetIcon> _targetIcons = new List<UITargetIcon>();
    
    private RectTransform _rectTransform;
    private PlayerController _playerController;

    public void Init(PlayerController playerController)
    {
        _rectTransform = GetComponent<RectTransform>();
        _playerController = playerController;
        
        _playerController.OnTargetSwitched += SwitchTarget; //!
    }

    private void OnEnable()
    {
        //_playerController.OnTargetSwitched += SelectTarget;
    }

    private void OnDisable()
    {
        _playerController.OnTargetSwitched -= SwitchTarget;
    }


    public void SetUnitTargets(UnitController oldUnit, UnitController unit)
    {
        for (int i = 0; i < _targetIcons.Count; i++)
        {
            Destroy(_targetIcons[i].gameObject);
        }
        _targetIcons.Clear();
        
        if (!unit) return;

        for (int i = 0; i < unit.UnitCombat.Targets.Count; i++)
        {
            GameObject targetObject = Instantiate(_targetIconPref, _rectTransform);
            UITargetIcon targetIcon = targetObject.GetComponent<UITargetIcon>();
            targetIcon.Init(this, i);
            _targetIcons.Add(targetIcon);
        }
    }

    public void SelectTarget(int id)
    {
        _playerController.AbilitySwitchTarget(id);
    }

    private void SwitchTarget(int id)
    {
        for (int i = 0; i < _targetIcons.Count; i++)
        {
            _targetIcons[i].SetSelection(false);
        }
        if(id == -1) return;
        _targetIcons[id].SetSelection(true);
    }
}
