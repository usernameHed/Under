/*
 * Created by Cyborg Assets
 */
using UnityEngine;

/// <summary>
/// Methods to get the camera pivots in world space
/// </summary>
public static class CameraExtensionGetPivots
{
	/// <summary>
	/// get the screen top center in world position
	/// </summary>
	/// <param name="distance from camera">distance from the camera on the z axis ( mainly affects 3D cameras)
	/// (enter 10 if you're using orthographic ( 2D ) camera (or default unity 2D project)</param>
	public static Vector3 GetTopPosition(this Camera myCamera,float distanceFromCamera){
		Vector3 topCenter = myCamera.ViewportToWorldPoint(new Vector3(0.5f,1,distanceFromCamera));
		return topCenter;
	}
	
	/// <summary>
	/// get the screen top right in world position
	/// </summary>
	/// <param name="distance from camera">distance from the camera on the z axis ( mainly affects 3D cameras)
	/// (enter 10 if you're using orthographic ( 2D ) camera (or default unity 2D project)</param>
	public static Vector3 GetTopRight(this Camera myCamera,float distanceFromCamera){
		Vector3 targetPoint = myCamera.ViewportToWorldPoint(new Vector3(1,1,distanceFromCamera));
		return targetPoint;
	}
	
	/// <summary>
	/// get the screen right center in world position
	/// </summary>
	/// <param name="distance from camera">distance from the camera on the z axis ( mainly affects 3D cameras)
	/// (enter 10 if you're using orthographic ( 2D ) camera (or default unity 2D project)</param>
	public static Vector3 GetRightPosition(this Camera myCamera,float distanceFromCamera){
		Vector3 targetPoint = myCamera.ViewportToWorldPoint(new Vector3(1,0.5f,distanceFromCamera));
		return targetPoint;
	}
	
	/// <summary>
	/// get the screen bottom Right in world position
	/// </summary>
	/// <param name="distance from camera">distance from the camera on the z axis ( mainly affects 3D cameras)
	/// (enter 10 if you're using orthographic ( 2D ) camera (or default unity 2D project)</param>
	public static Vector3 GetBottomRight(this Camera myCamera,float distanceFromCamera){
		Vector3 targetPoint = myCamera.ViewportToWorldPoint(new Vector3(1,0,distanceFromCamera));
		return targetPoint;
	}
	
	/// <summary>
	/// get the screen Bottom Center in world position
	/// </summary>
	/// <param name="distance from camera">distance from the camera on the z axis ( mainly affects 3D cameras)
	/// (enter 10 if you're using orthographic ( 2D ) camera (or default unity 2D project)</param>
	public static Vector3 GetBottomPosition(this Camera myCamera,float distanceFromCamera){
		Vector3 targetPoint = myCamera.ViewportToWorldPoint(new Vector3(0.5f,0,distanceFromCamera));
		return targetPoint;
	}
	
	/// <summary>
	/// get the screen Bottom left in world position
	/// </summary>
	/// <param name="distance from camera">distance from the camera on the z axis ( mainly affects 3D cameras)
	/// (enter 10 if you're using orthographic ( 2D ) camera (or default unity 2D project)</param>
	public static Vector3 GetBottomLeft(this Camera myCamera,float distanceFromCamera){
		Vector3 targetPoint = myCamera.ViewportToWorldPoint(new Vector3(0,0,distanceFromCamera));
		return targetPoint;
	}
	
	/// <summary>
	/// get the screen Left Center in world position
	/// </summary>
	/// <param name="distance from camera">distance from the camera on the z axis ( mainly affects 3D cameras)
	/// (enter 10 if you're using orthographic ( 2D ) camera (or default unity 2D project)</param>
	public static Vector3 GetLeftPosition(this Camera myCamera,float distanceFromCamera){
		Vector3 targetPoint = myCamera.ViewportToWorldPoint(new Vector3(0,0.5f,distanceFromCamera));
		return targetPoint;
	}
	
	/// <summary>
	/// get the screen Top Left in world position
	/// </summary>
	/// <param name="distance from camera">distance from the camera on the z axis ( mainly affects 3D cameras)
	/// (enter 10 if you're using orthographic ( 2D ) camera (or default unity 2D project)</param>
	public static Vector3 GetTopLeft(this Camera myCamera,float distanceFromCamera){
		Vector3 targetPoint = myCamera.ViewportToWorldPoint(new Vector3(0f,1f,distanceFromCamera));
		return targetPoint;
	}
	
	/// <summary>
	/// get the screen center in world position
	/// </summary>
	/// <param name="distance from camera">distance from the camera on the z axis ( mainly affects 3D cameras)
	/// (enter 10 if you're using orthographic ( 2D ) camera (or default unity 2D project)</param>
	public static Vector3 GetCenter(this Camera myCamera,float distanceFromCamera){
		Vector3 targetPoint = myCamera.ViewportToWorldPoint(new Vector3(0.5f,0.5f,distanceFromCamera));
		return targetPoint;
	}
	
	
}
