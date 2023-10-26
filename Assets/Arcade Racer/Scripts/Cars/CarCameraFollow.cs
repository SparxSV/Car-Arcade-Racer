using UnityEngine;

namespace Arcade_Racer.Scripts.Cars
{
	public class CarCameraFollow : MonoBehaviour
	{
		[Header("Smoothness")]
		public float moveSmoothness;
		public float rotSmoothness;

		public Vector3 moveOffset;
		public Vector3 rotOffset;

		public Transform carTarget;

		private void FixedUpdate() => FollowTarget();

		private void FollowTarget()
		{
			HandleMovement();
			HandleRotation();
		}
		
		private void HandleMovement()
		{
			Vector3 targetPos = carTarget.TransformPoint(moveOffset);
			transform.position = Vector3.Lerp(transform.position, targetPos, moveSmoothness * Time.deltaTime);
		}

		private void HandleRotation()
		{
			Vector3 direction = carTarget.position - transform.position;
			Quaternion rotation = Quaternion.LookRotation(direction + rotOffset, Vector3.up);
			
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotSmoothness * Time.deltaTime);
		}
	}
}