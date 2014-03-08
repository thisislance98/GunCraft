using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
    public float speed = 10.0F;
    public float rotationSpeed = 100.0F;
	
	public float horizontalSpeed = 1.0F;
    public float verticalSpeed = 1.0F;
	
	void Start()
	{
		Debug.Log("hiding cursor");
		Screen.showCursor = false;
	}
	
    void Update() {
//		if (Input.GetMouseButtonDown(1))
//			Application.LoadLevel(0);
		
        float forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float right = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float up = Input.GetAxis("Up") * speed * Time.deltaTime;
		
		transform.Translate(right, up, forward);
		
		
		float h = horizontalSpeed * Input.GetAxis("Mouse X");
        float v = verticalSpeed * Input.GetAxis("Mouse Y");
//        transform.Rotate(v, h, 0);
        transform.Rotate(0, h, 0);
		transform.Rotate(v, 0, 0);
		Vector3 eulers = transform.eulerAngles;
		transform.rotation = Quaternion.Euler(eulers.x+v,eulers.y+h,0);
    }
}
