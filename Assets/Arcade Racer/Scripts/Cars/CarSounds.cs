using UnityEngine;

namespace Arcade_Racer.Scripts.Cars
{
	public class CarSounds : MonoBehaviour
	{
		public bool toggleEngineSounds;
	
		public float minSpeed;
		public float maxSpeed;
		
		public float minPitch;
		public float maxPitch;

		private Rigidbody carRigidBody;
		private AudioSource carAudio;

		private float currentSpeed;
		private float pitchFromCar;

		private void Start()
		{
			carAudio = GetComponent<AudioSource>();
			carRigidBody = GetComponent<Rigidbody>();
		}

		private void Update()
		{
			if(toggleEngineSounds)
				EngineSound();
		}

		private void EngineSound()
		{
			currentSpeed = carRigidBody.velocity.magnitude;
			pitchFromCar = carRigidBody.velocity.magnitude / 50f;

			if(currentSpeed < minSpeed)
				carAudio.pitch = minPitch;

			if(currentSpeed > minSpeed && currentSpeed < maxSpeed)
				carAudio.pitch = minPitch + pitchFromCar;

			if(currentSpeed > maxSpeed)
				carAudio.pitch = maxPitch;
		}
	}
}