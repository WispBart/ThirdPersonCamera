using System.Collections.Generic;
using UnityEngine;

namespace Wispfire.Cameras.ThirdPerson
{
	public class ThirdPersonCamera : MonoBehaviour
    {
        public Transform Pivot; // Vector3 position
        public Transform LookAt; // Quaternion Rotation

        [SerializeField] private float _yaw;
        public float Yaw
        {
            get => _yaw;
            set => _yaw = value % 360;
        }

        [SerializeField] private float _pitch;
        public float Pitch
        {
            get => _pitch;
            set => _pitch = value % 360;
        }

        [SerializeField] private float _roll;
        public float Roll
        {
            get => _roll;
            set => _roll = value % 360;
        }

        public float Distance;
        public float OffsetLateral;
        public float OffsetVertical;

        [Range(1, 180)] public float FieldOfView = 70;


        private Vector3 _position;
        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        private Quaternion _rotation;
        public Quaternion Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }
        
        private Vector3 _lookAtPosition;

        public void CalculateValues()
        {
            Pitch = ProcessValue(_pitch, _pitchProcessors);
            Yaw = ProcessValue(_yaw, _yawProcessors);
            Roll = ProcessValue(_roll, _rollProcessors);
            Distance = ProcessValue(Distance, _distanceProcessors);
            OffsetLateral = ProcessValue(OffsetLateral, _offsetLProcessors);
            OffsetVertical = ProcessValue(OffsetVertical, _offsetVProcessors);
            FieldOfView = ProcessValue(FieldOfView, _fovProcessors);            
            CalculatePositionAndRotation();
            Position = PostProcessPosition(Position, _positionPostProcessors);
            Rotation = PostProcessRotation(Rotation, _rotationPostProcessors);
        }

        private float ProcessValue<T>(float startValue, IEnumerable<CameraProcessor<T>> list) where T : CameraValue
        {
            var value = startValue;
            foreach (var item in list)
            {
                value = item.Process(value);
            }
            return value;
        }

        private Vector3 PostProcessPosition(Vector3 startValue, IEnumerable<CameraPositionPostProcessor> list)
        {
            var value = startValue;
            foreach (var item in list)
            {
                value = item.Process(value);
            }
            return value;
        }

        private Quaternion PostProcessRotation(Quaternion startValue, IEnumerable<CameraRotationPostProcessor> list)
        {
            var value = startValue;
            foreach (var item in list)
            {
                value = item.Process(value);
            }
            return value;
        }

        private void CalculatePositionAndRotation()
        {
            // Calculate final Position & Rotation
            _lookAtPosition = LookAt.position;

            var pivotRotation =
                Quaternion.Euler(_pitch, Yaw, 0); // Add roll later so offset can be calculated without it.
            var pivotPosition = Pivot.transform.position;

            _position = pivotPosition + (pivotRotation * new Vector3(0, 0, -Distance));

            _rotation = Quaternion.LookRotation((_lookAtPosition - _position).normalized,
                pivotRotation * Vector3.up);

            var offsetVector = _rotation * new Vector3(OffsetLateral, OffsetVertical, 0);
            _position += offsetVector;

            _rotation *= Quaternion.Euler(0, 0, _roll); // Finally add roll to the camera rotation.
        }

        private SortedSet<CameraProcessor<Pitch>> _pitchProcessors 
            = new SortedSet<CameraProcessor<Pitch>>(Comparer<CameraProcessor<Pitch>>.Create((x,y) => x.Order.CompareTo(y.Order)));
        private SortedSet<CameraProcessor<Yaw>> _yawProcessors 
            = new SortedSet<CameraProcessor<Yaw>>(Comparer<CameraProcessor<Yaw>>.Create((x,y) => x.Order.CompareTo(y.Order)));
        private SortedSet<CameraProcessor<Roll>> _rollProcessors
            = new SortedSet<CameraProcessor<Roll>>(Comparer<CameraProcessor<Roll>>.Create((x,y) => x.Order.CompareTo(y.Order)));
        private SortedSet<CameraProcessor<Distance>> _distanceProcessors
            = new SortedSet<CameraProcessor<Distance>>(Comparer<CameraProcessor<Distance>>.Create((x,y) => x.Order.CompareTo(y.Order)));
        private SortedSet<CameraProcessor<OffsetLateral>> _offsetLProcessors
            = new SortedSet<CameraProcessor<OffsetLateral>>(Comparer<CameraProcessor<OffsetLateral>>.Create((x,y) => x.Order.CompareTo(y.Order)));
        private SortedSet<CameraProcessor<OffsetVertical>> _offsetVProcessors
            = new SortedSet<CameraProcessor<OffsetVertical>>(Comparer<CameraProcessor<OffsetVertical>>.Create((x,y) => x.Order.CompareTo(y.Order)));
        private SortedSet<CameraProcessor<FieldOfView>> _fovProcessors
            = new SortedSet<CameraProcessor<FieldOfView>>(Comparer<CameraProcessor<FieldOfView>>.Create((x,y) => x.Order.CompareTo(y.Order)));
        private SortedSet<CameraPositionPostProcessor> _positionPostProcessors
            = new SortedSet<CameraPositionPostProcessor>(Comparer<CameraPositionPostProcessor>.Create((x,y) => x.Order.CompareTo(y.Order)));
        private SortedSet<CameraRotationPostProcessor> _rotationPostProcessors
            = new SortedSet<CameraRotationPostProcessor>(Comparer<CameraRotationPostProcessor>.Create((x,y) => x.Order.CompareTo(y.Order)));
        
        public bool Register(CameraProcessor<Pitch> processor) => _pitchProcessors.Add(processor);
        public bool Register(CameraProcessor<Yaw> processor) => _yawProcessors.Add(processor);
        public bool Register(CameraProcessor<Roll> processor) => _rollProcessors.Add(processor);
        public bool Register(CameraProcessor<Distance> processor) => _distanceProcessors.Add(processor);
        public bool Register(CameraProcessor<OffsetLateral> processor) => _offsetLProcessors.Add(processor);
        public bool Register(CameraProcessor<OffsetVertical> processor) => _offsetVProcessors.Add(processor);
        public bool Register(CameraProcessor<FieldOfView> processsor) => _fovProcessors.Add(processsor);
        public bool Register(CameraPositionPostProcessor processor) => _positionPostProcessors.Add(processor);
        public bool Register(CameraRotationPostProcessor processor) => _rotationPostProcessors.Add(processor);

        public bool Unregister(CameraProcessor<Pitch> processor) => _pitchProcessors.Remove(processor);
        public bool Unregister(CameraProcessor<Yaw> processor) => _yawProcessors.Remove(processor);
        public bool Unregister(CameraProcessor<Roll> processor) => _rollProcessors.Remove(processor);
        public bool Unregister(CameraProcessor<Distance> processor) => _distanceProcessors.Remove(processor);
        public bool Unregister(CameraProcessor<OffsetLateral> processor) => _offsetLProcessors.Remove(processor);
        public bool Unregister(CameraProcessor<OffsetVertical> processor) => _offsetVProcessors.Remove(processor);
        public bool Unregister(CameraProcessor<FieldOfView> processor) => _fovProcessors.Remove(processor);
        public bool Unregister(CameraPositionPostProcessor processor) => _positionPostProcessors.Remove(processor);
        public bool Unregister(CameraRotationPostProcessor processor) => _rotationPostProcessors.Remove(processor);
    }

    /// <summary>
    /// Allows a camera value (e.g. yaw, pitch, FoV) to be processed. Values are processed in the defined order.
    /// If the order is changed during runtime, the processor should be unregistered and then registered again with the ThirdPersonCamera.
    /// </summary>
    /// <typeparam name="T">One of the interface types defined in CameraValues.cs</typeparam>
    public interface CameraProcessor<T> where T : CameraValue
    {
        float Process(float value);
        int Order { get; }
    }

    /// <summary>
    /// Allows camera position to be modified directly after base position has been calculated, but before it is applied to the transform.
    /// </summary>
    public interface CameraPositionPostProcessor
    {
        Vector3 Process(Vector3 value);
        int Order { get; }
    }
    /// <summary>
    /// Allows camera rotation to be modified directly after base rotation has been calculated, but before it is applied to the transform.
    /// </summary>
    public interface CameraRotationPostProcessor
    {
        Quaternion Process(Quaternion value);
        int Order { get; }
    }
}

