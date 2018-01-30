using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
edit:
https://www.draw.io/#G0Byzet-SVq6ipYmw1ZnBGSTg1N3M
see:
https://drive.google.com/file/d/0Byzet-SVq6ipMG9BOHV6UTBPTHM/view
*/
/// <summary>
/// Pour tout les objets qui collide avec la caméra,
/// attacher ce script à l'objet, puis attacher le script CollideWithWalls
/// à un objet enfant de l'objet parent cible.
/// </summary>

public class BoolCloseToWalls : MonoBehaviour {

    public bool isCloseToWalls = false;
}
