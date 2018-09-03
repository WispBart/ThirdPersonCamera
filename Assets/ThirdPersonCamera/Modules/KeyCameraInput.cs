using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wispfire.Cameras.ThirdPerson.Modules
{
	[RequireComponent(typeof(ThirdPersonCamera))]
	public class KeyCameraInput : MonoBehaviour, CameraProcessor<Yaw>, CameraProcessor<Pitch>
	{
		[SerializeField] private KeyCode _yawNegative = KeyCode.A;
		[SerializeField] private KeyCode _yawPositive = KeyCode.D;
		public bool InvertX;
		[SerializeField] private float _multiplyYawInput = 1;

		[SerializeField] private KeyCode _pitchNegative = KeyCode.S;
		[SerializeField] private KeyCode _pitchPositive = KeyCode.W;
		public bool InvertY;
		[SerializeField] private float _multiplyPitchInput = 1;
		
		private ThirdPersonCamera _camera;

		void Awake() => _camera = GetComponent<ThirdPersonCamera>();


		float CameraProcessor<Yaw>.Process(float value)
		{
			var yaw = (Input.GetKey(_yawPositive) ? 1 : 0) * _multiplyYawInput;
			yaw -= (Input.GetKey(_yawNegative) ? 1 : 0) * _multiplyYawInput;

			return InvertX ? value + yaw : value - yaw;
		}

		int CameraProcessor<Yaw>.Order => -10;
		int CameraProcessor<Pitch>.Order => -10;

		float CameraProcessor<Pitch>.Process(float value)
		{
			var pitch = (Input.GetKey(_pitchPositive) ? 1 : 0) * _multiplyPitchInput;
			pitch -= (Input.GetKey(_pitchNegative) ? 1 : 0) * _multiplyPitchInput;

			return InvertX ? value + pitch : value - pitch;		
		}

		private void OnEnable()
		{
			_camera.Register(this as CameraProcessor<Pitch>);
			_camera.Register(this as CameraProcessor<Yaw>);		
		}
		private void OnDisable()
		{
			_camera.Unregister(this as CameraProcessor<Pitch>);
			_camera.Unregister(this as CameraProcessor<Yaw>);		
		}
	}
}
