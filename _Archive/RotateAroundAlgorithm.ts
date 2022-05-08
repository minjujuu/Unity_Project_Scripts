// https://answers.unity.com/questions/489350/rotatearound-without-transform.html

function RotateAround(center: Vector3, axis: Vector3, angle: float){
   var pos: Vector3 = transform.position;
   var rot: Quaternion = Quaternion.AngleAxis(angle, axis); // get the desired rotation
   var dir: Vector3 = pos - center; // find current direction relative to center
   dir = rot * dir; // rotate the direction
   transform.position = center + dir; // define new position
   // rotate object to keep looking at the center:
   var myRot: Quaternion = transform.rotation;
   transform.rotation *= Quaternion.Inverse(myRot) * rot * myRot;
 }
