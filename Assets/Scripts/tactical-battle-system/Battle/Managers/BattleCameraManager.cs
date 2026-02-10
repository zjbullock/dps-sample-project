using Unity.Cinemachine;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
public class BattleCameraManager : MonoBehaviour
{

    [Header("Brain")]
    [SerializeField]
    private CinemachineBrain _cinemachineBrain;

    public CinemachineBrain CinemachineBrain { get => this._cinemachineBrain; }

    [Space]
    [Header("Cameras")]
    [SerializeField]
    private CinemachineCameraEventController _freeLookCamera;

    public CinemachineCameraEventController FreeLookCamera { get => this._freeLookCamera; }

    [SerializeField]
    private CinemachineCameraEventController _focusPointCamera;

    public CinemachineCameraEventController FocusPointCamera { get => this._focusPointCamera; }

    [SerializeField]
    private CinemachineCameraEventController _sideCamera;
    public CinemachineCameraEventController SideCamera { get => this._sideCamera; }

    [Space]
    [Header("Camera Tracker")]
    [Tooltip("The camera tracker controller")]
    [SerializeField]
    private CameraTrackerController _cameraTrackerController;

    public CameraTrackerController CameraTracker { get => this._cameraTrackerController; }

    void Awake()
    {
        if (this._cameraTrackerController == null)
            this._cameraTrackerController = GetComponentInChildren<CameraTrackerController>();
    }

}
}