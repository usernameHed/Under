/*
 * Created by Cyborg Assets
 */
using UnityEngine;

namespace Assets.Cyborg_Assets.Camera_Get_Pivots.Examples
{
	/// <summary>
	/// Description of Distance.
	/// </summary>
	public class DistanceForWalls:MonoBehaviour
	{
		public static DistanceForWalls instance;
		
		public float distance=5;
		
		void Awake(){
			instance=this;
		}
    }
}
