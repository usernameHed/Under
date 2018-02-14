/*
 * Created By Cyborg Assets
 */
using UnityEngine;


	/// <summary>
	/// Methods to move the camera using it's pivots
	/// </summary>
	public static class CameraExtensionMoveByPivots
	{
	
	/// <returns>returns the offset the game object moved</returns>
	public static Vector3 MoveCenterTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera){
		Vector3 offset=MoveCenterTo(camera,targetPoint, distanceFromCamera, camera.gameObject);
		return offset;
	}
	///<returns>returns the offset the game object moved</returns>
	///<param name="topParent">moves the whole parent of the camera game object</param>
	public static Vector3 MoveCenterTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera,GameObject topParent){
		Vector3 topCenter=camera.GetCenter(distanceFromCamera);
		Vector3 offset=targetPoint-topCenter;
		topParent.transform.Translate(offset,Space.World);
		return offset;
	}
	
	
	/// <returns>returns the offset the game object moved</returns>
	public static Vector3 MoveTopCenterTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera){
		Vector3 offset=MoveTopCenterTo(camera,targetPoint, distanceFromCamera, camera.gameObject);
		return offset;
	}
	///<returns>returns the offset the game object moved</returns>
	///<param name="topParent">moves the whole parent of the camera game object</param>
	public static Vector3 MoveTopCenterTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera,GameObject topParent){
		Vector3 topCenter=camera.GetTopPosition(distanceFromCamera);
		Vector3 offset=targetPoint-topCenter;
		topParent.transform.Translate(offset,Space.World);
		return offset;
	}
	
	
	/// <returns>returns the offset the game object moved</returns>
	public static Vector3 MoveTopRightTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera){
		Vector3 offset=MoveTopRightTo(camera,targetPoint, distanceFromCamera, camera.gameObject);
		return offset;
	}
	///<returns>returns the offset the game object moved</returns>
	///<param name="topParent">moves the whole parent of the camera game object</param>
	public static Vector3 MoveTopRightTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera,GameObject topParent){
		Vector3 topCenter=camera.GetTopRight(distanceFromCamera);
		Vector3 offset=targetPoint-topCenter;
		topParent.transform.Translate(offset,Space.World);
		return offset;
	}
	
	
	/// <returns>returns the offset the game object moved</returns>
	public static Vector3 MoveRightCenterTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera){
		Vector3 offset=MoveRightCenterTo(camera,targetPoint, distanceFromCamera, camera.gameObject);
		return offset;
	}
	///<returns>returns the offset the game object moved</returns>
	///<param name="topParent">moves the whole parent of the camera game object</param>
	public static Vector3 MoveRightCenterTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera,GameObject topParent){
		Vector3 topCenter=camera.GetRightPosition(distanceFromCamera);
		Vector3 offset=targetPoint-topCenter;
		topParent.transform.Translate(offset,Space.World);
		return offset;
	}
	
	
	/// <returns>returns the offset the game object moved</returns>
	public static Vector3 MoveBottomRightTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera){
		Vector3 offset=MoveBottomRightTo(camera,targetPoint, distanceFromCamera, camera.gameObject);
		return offset;
	}
	///<returns>returns the offset the game object moved</returns>
	///<param name="topParent">moves the whole parent of the camera game object</param>
	public static Vector3 MoveBottomRightTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera,GameObject topParent){
		Vector3 topCenter=camera.GetBottomRight(distanceFromCamera);
		Vector3 offset=targetPoint-topCenter;
		topParent.transform.Translate(offset,Space.World);
		return offset;
	}
	
	
	
	/// <returns>returns the offset the game object moved</returns>
	public static Vector3 MoveBottomCenterTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera){
		Vector3 offset=MoveBottomCenterTo(camera,targetPoint, distanceFromCamera, camera.gameObject);
		return offset;
	}
	///<returns>returns the offset the game object moved</returns>
	///<param name="topParent">moves the whole parent of the camera game object</param>
	public static Vector3 MoveBottomCenterTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera,GameObject topParent){
		Vector3 topCenter=camera.GetBottomPosition(distanceFromCamera);
		Vector3 offset=targetPoint-topCenter;
		topParent.transform.Translate(offset,Space.World);
		return offset;
	}
	
	
	/// <returns>returns the offset the game object moved</returns>
	public static Vector3 MoveBottomLeftTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera){
		Vector3 offset=MoveBottomLeftTo(camera,targetPoint, distanceFromCamera, camera.gameObject);
		return offset;
	}
	///<returns>returns the offset the game object moved</returns>
	///<param name="topParent">moves the whole parent of the camera game object</param>
	public static Vector3 MoveBottomLeftTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera,GameObject topParent){
		Vector3 topCenter=camera.GetBottomLeft(distanceFromCamera);
		Vector3 offset=targetPoint-topCenter;
		topParent.transform.Translate(offset,Space.World);
		return offset;
	}
	
	
	/// <returns>returns the offset the game object moved</returns>
	public static Vector3 MoveLeftCenterTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera){
		Vector3 offset=MoveLeftCenterTo(camera,targetPoint, distanceFromCamera, camera.gameObject);
		return offset;
	}
	///<returns>returns the offset the game object moved</returns>
	///<param name="topParent">moves the whole parent of the camera game object</param>
	public static Vector3 MoveLeftCenterTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera,GameObject topParent){
		Vector3 topCenter=camera.GetLeftPosition(distanceFromCamera);
		Vector3 offset=targetPoint-topCenter;
		topParent.transform.Translate(offset,Space.World);
		return offset;
	}
	
	
	/// <returns>returns the offset the game object moved</returns>
	public static Vector3 MoveTopLeftTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera){
		Vector3 offset=MoveTopLeftTo(camera,targetPoint, distanceFromCamera, camera.gameObject);
		return offset;
	}
	///<returns>returns the offset the game object moved</returns>
	///<param name="topParent">moves the whole parent of the camera game object</param>
	public static Vector3 MoveTopLeftTo(this Camera camera,Vector3 targetPoint, float distanceFromCamera,GameObject topParent){
		Vector3 topCenter=camera.GetTopLeft(distanceFromCamera);
		Vector3 offset=targetPoint-topCenter;
		topParent.transform.Translate(offset,Space.World);
		return offset;
	}
	}
