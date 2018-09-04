using UnityEngine;

namespace Wispfire.Cameras.ThirdPerson
{
    [RequireComponent(typeof(ThirdPersonCamera))]
    public class ThirdPersonUnityCamera : MonoBehaviour
    {
        public Camera TargetCamera;
        private ThirdPersonCamera _thirdPersonCamera;

        private void Awake() => _thirdPersonCamera = GetComponent<ThirdPersonCamera>();
        
        void LateUpdate()
        {
            TargetCamera.transform.position = _thirdPersonCamera.Position;
            TargetCamera.transform.rotation = _thirdPersonCamera.Rotation;
            TargetCamera.fieldOfView = _thirdPersonCamera.FieldOfView;
        }
    }
}
