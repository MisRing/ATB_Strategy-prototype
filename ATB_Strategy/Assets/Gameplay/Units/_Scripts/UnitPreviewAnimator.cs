using UnityEngine;

public class UnitPreviewAnimator : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Animator _unitAnimator;
    [SerializeField] private Animator _previewAnimator;
    [SerializeField] private WeaponIKController _previewWeaponIK;
    [SerializeField] private Transform _previewModel;
    
    private bool _isActive = false;
    
    [Header("Targeting settings")]
    [SerializeField] private float _targetDuration = 0.3f;
    
    private Transform _aimTarget;
    private float _time = 0f;

    public void StartPreview()
    {
        if (_isActive) return;

        _previewModel.gameObject.SetActive(true);
        _isActive = true;
        SetAnimationParameters();
    }

    public void EndPreview()
    {
        _isActive = false;
        _previewModel.gameObject.SetActive(false);
    }

    public void AimToTarget(Transform target)
    {
        StartPreview();

        _aimTarget = target;
        _time = 0f;
    }


    private void Update()
    {
        if (!_isActive || !_aimTarget) return;
        _time += Time.deltaTime;
        
        Vector3 dir = Vector3.ProjectOnPlane(_aimTarget.position - transform.position, Vector3.up).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);

        float lerp = Mathf.Clamp01(_time / _targetDuration);
        float eased = 1f - Mathf.Pow(1f - lerp, 2);
        _previewModel.rotation = Quaternion.Lerp(_previewModel.rotation, targetRotation, eased);
        SetCover(TileCover.None, 0, eased);

        float angle = Quaternion.Angle(_previewModel.rotation, targetRotation);
        float weight = 1f - Mathf.Clamp01(angle / 60f);
        weight = Mathf.Pow(weight, 2);

        _previewWeaponIK.SetAimWeight(weight);
        _previewWeaponIK.SetAimRotation(_aimTarget.position);
    }

    private void SetCover(TileCover cover, int look, float percent)
    {
        if (cover == TileCover.None)
        {
            _previewAnimator.SetFloat(UnitAnimator.COVER_VERTICAL_ID,
                Mathf.Lerp(_previewAnimator.GetFloat(UnitAnimator.COVER_VERTICAL_ID), 0f, percent));
            
            _previewAnimator.SetFloat(UnitAnimator.COVER_HORIZONTAL_ID,
                Mathf.Lerp(_previewAnimator.GetFloat(UnitAnimator.COVER_HORIZONTAL_ID), 0f, percent));
        }
        else
        {
            float vertical = cover == TileCover.Full ? 1f : -1f;
            float horizontal = look < 0 ? -1f : (look > 0 ? 1f : 0f);
            
            _previewAnimator.SetFloat(UnitAnimator.COVER_VERTICAL_ID,
                Mathf.Lerp(_previewAnimator.GetFloat(UnitAnimator.COVER_VERTICAL_ID), vertical, percent));
            
            _previewAnimator.SetFloat(UnitAnimator.COVER_HORIZONTAL_ID,
                Mathf.Lerp(_previewAnimator.GetFloat(UnitAnimator.COVER_HORIZONTAL_ID), horizontal, percent));
        }
    }
    
    private void SetAnimationParameters()
    {
        _previewAnimator.SetFloat(UnitAnimator.MOVEMENT_X_ID, _unitAnimator.GetFloat(UnitAnimator.MOVEMENT_X_ID));
        _previewAnimator.SetFloat(UnitAnimator.MOVEMENT_Z_ID, _unitAnimator.GetFloat(UnitAnimator.MOVEMENT_Z_ID));
        
        _previewAnimator.SetFloat(UnitAnimator.COVER_VERTICAL_ID, _unitAnimator.GetFloat(UnitAnimator.COVER_VERTICAL_ID));
        _previewAnimator.SetFloat(UnitAnimator.COVER_HORIZONTAL_ID, _unitAnimator.GetFloat(UnitAnimator.COVER_HORIZONTAL_ID));
        
        _previewAnimator.SetBool(UnitAnimator.AIM_ID, _unitAnimator.GetBool(UnitAnimator.AIM_ID));
    }
}
