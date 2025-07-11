using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Light2DBase)), DefaultExecutionOrder(-1)]
	public sealed class EffectsController : StateController
	{
		private static EffectsController _instance;
		private List<Light2DBase> _lightsStack;
		private bool _canHitStop = true;
		[SerializeField, Tooltip("The renderer of the image.")] private ImageRenderer _imageRenderer;
		private new void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			base.Awake();
			this._lightsStack = new List<Light2DBase>() { this.GetComponent<Light2DBase>() };
		}
		public static void SetHitStop(float stopTime, float slowTime)
		{
			if (_instance._canHitStop)
				_instance.StartCoroutine(HitStop());
			IEnumerator HitStop()
			{
				_instance._canHitStop = false;
				Time.timeScale = slowTime;
				yield return new WaitForSecondsRealtime(stopTime);
				_instance._canHitStop = true;
				Time.timeScale = 1f;
			}
		}
		public static void OnGlobalLight(Light2DBase globalLight)
		{
			if (_instance._lightsStack.Contains(globalLight))
				return;
			foreach (Light2DBase light in _instance._lightsStack)
				light.enabled = false;
			globalLight.enabled = true;
			_instance._lightsStack.Add(globalLight);
		}
		public static void OffGlobalLight(Light2DBase globalLight)
		{
			if (!_instance._lightsStack.Contains(globalLight))
				return;
			foreach (Light2DBase light in _instance._lightsStack)
				light.enabled = false;
			_instance._lightsStack.Remove(globalLight);
			_instance._lightsStack[^1].enabled = true;
		}
		public static IImagePool CreateImageRenderer<Components>(Components components) where Components : class, IImageComponents
		{
			MonoBehaviour rendererObject = components as MonoBehaviour;
			ImageRenderer image = Instantiate(_instance._imageRenderer);
			image.transform.SetParent(rendererObject.transform, false);
			image.transform.localPosition = components.ImageOffset;
			SpriteRenderer spriteRenderer = image.GetComponent<SpriteRenderer>();
			spriteRenderer.sprite = components.Image;
			return image;
		}
	};
};
