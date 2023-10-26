using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Arcade_Racer.Scripts.Cars
{
	public class CarLights : MonoBehaviour
	{
		public enum Side
		{
			Front,
			Back
		}

		[Serializable]
		public struct Light
		{
			public GameObject lightObj;
			public Material lightMat;
			public Side side;
		}
		
		public Toggle lightToggle;

		public bool isFrontLightOn;
		public bool isBackLightOn;

		public Color frontLightOnColor;
		public Color frontLightOffColor;
		public Color backLightOnColor;
		public Color backLightOffColor;

		public List<Light> lights;

		private void Start()
		{
			isFrontLightOn = lightToggle.isOn;
			isBackLightOn = false;
		}

		public void OperateFrontLights()
		{
			isFrontLightOn = !isFrontLightOn;

			if(isFrontLightOn)
			{
				// Turn on Lights
				foreach(Light l in lights)
				{
					if(l.side == Side.Front && l.lightObj.activeInHierarchy == false)
					{
						l.lightObj.SetActive(true);
						l.lightMat.color = frontLightOnColor;
					}
				}
				
				lightToggle.gameObject.GetComponent<Image>().color = Color.yellow;
			}
			else
			{
				// Turn off Lights
				foreach(Light l in lights)
				{
					if(l.side == Side.Front && l.lightObj.activeInHierarchy == true)
					{
						l.lightObj.SetActive(false);
						l.lightMat.color = frontLightOffColor;
					}
				}
				
				lightToggle.gameObject.GetComponent<Image>().color = Color.white;
			}
		}

		public void OperateBackLights()
		{
			if(isBackLightOn)
			{
				// Turn on Lights
				foreach(Light l in lights)
				{
					if(l.side == Side.Back && l.lightObj.activeInHierarchy == false)
					{
						l.lightObj.SetActive(true);
						l.lightMat.color = backLightOnColor;
					}
				}
			}
			else
			{
				foreach(Light l in lights)
				{
					if(l.side == Side.Back && l.lightObj.activeInHierarchy == true)
					{
						l.lightObj.SetActive(false);
						l.lightMat.color = backLightOffColor;
					}
				}
			}
		}
	}
}