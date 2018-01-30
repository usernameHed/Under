/*
 * Created by Cyborg Assets
 * Date: 9/14/2016
 * Time: 1:12 PM
 */
using System;
using UnityEngine;

	/// <summary>
	/// Description of CameraExtensionUtils.
	/// </summary>
	public static class CameraExtensionUtils
	{
		/// <summary>
		/// returns the perpendicular distance from the camera to the target object plane
		/// </summary>
		public static float GetDistance(this Camera targetCamera,Transform _targetObject){
			//transform the object into the camera local space
			Vector3 position= _targetObject.position;
			return targetCamera.GetDistance(position);
		}
		/// <summary>
		/// returns the perpendicular distance from the camera to the target point plane
		/// </summary>
		public static float GetDistance(this Camera targetCamera,Vector3 _targetPoint){
			//transform the object into the camera local space
			Vector3 localPosition= targetCamera.transform.InverseTransformPoint(_targetPoint);
			//now the distance relative to the camera is the z coordinate value
			float _distance=localPosition.z;
			return _distance;
		}
		
		/// <summary>
		/// get the screen size in world units
		/// - returns Vector2
		/// - x = width
		/// - y = height
		/// </summary>
		/// <param name="distance from camera">distance from the camera on the z axis ( mainly affects 3D cameras)
		/// (enter 10 if you're using 2D orthographic camera)</param>
		public static Vector2 GetSizeInWorldSpace(this Camera myCamera,float distanceFromCamera){
			Vector2 size;
			if(myCamera.orthographic){
				float height = 2f * myCamera.orthographicSize;
				float width = height * myCamera.aspect;
				size=  new Vector2(width,height);
			}else{
				float frustumHeight = 2.0f * distanceFromCamera * Mathf.Tan(myCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
				float frustumWidth = frustumHeight * myCamera.aspect;
				size=new Vector2(frustumWidth,frustumHeight);
			}
			
			return size;
			
		}
	}
