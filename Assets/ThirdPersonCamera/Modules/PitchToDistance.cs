using Unity.Mathematics;
using UnityEngine;

namespace Wispfire.Cameras.ThirdPerson.Modules
{
    [RequireComponent(typeof(ThirdPersonCamera))]
    public class PitchToDistance : MonoBehaviour, CameraProcessor<Pitch>, CameraProcessor<Distance>
    {
        public float PitchMinimum;
        public float PitchMaximum;
        public AnimationCurve PitchToDistanceCurve;

        public int OrderDistanceProcessor = 1;
        public int OrderPitchProcessor = 1;
        int CameraProcessor<Pitch>.Order => OrderPitchProcessor;
        int CameraProcessor<Distance>.Order => OrderDistanceProcessor;

        private float _cachedPitch;
        private ThirdPersonCamera _camera;

        void Awake() => _camera = GetComponent<ThirdPersonCamera>();
    
        float CameraProcessor<Pitch>.Process(float value)
        {
            _cachedPitch = value;
            return math.clamp(value, PitchMinimum, PitchMaximum);
        }

        float CameraProcessor<Distance>.Process(float value)
        {
            return PitchToDistanceCurve.Evaluate(_cachedPitch);
        }

        void OnEnable()
        {
            _camera.Register(this as CameraProcessor<Pitch>);
            _camera.Register(this as CameraProcessor<Distance>);
        }

        private void OnDisable()
        {
            _camera.Unregister(this as CameraProcessor<Pitch>);
            _camera.Unregister(this as CameraProcessor<Distance>);
        }
    }
}
