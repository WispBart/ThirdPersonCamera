using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Wispfire.Cameras.ThirdPerson
{
	[RequireComponent(typeof(ThirdPersonCamera))]
	public class ThirdPersonCMVirtualCamera : MonoBehaviour
	{
		public CinemachineVirtualCamera TargetCamera;
		private ThirdPersonCamera _thirdPersonCamera;

		private void Awake() => _thirdPersonCamera = GetComponent<ThirdPersonCamera>();
		
		void LateUpdate()
		{
			transform.position = _thirdPersonCamera.Position;
			transform.rotation = _thirdPersonCamera.Rotation;
			TargetCamera.m_Lens.FieldOfView = _thirdPersonCamera.FieldOfView;
		}
	}
}