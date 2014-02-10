var runSpeedScale = 1.0;
var walkSpeedScale = 1.0;

var transformVelocity = Vector3.zero;
var currentSpeed = 0.0;
var lastSpeed = 0.0;

var isFalling = false;
var verticalFallingSpeed = -10;

var walkSpeed = 3.0;

private var prevPos = Vector3.zero; //Vector3 type


function Awake ()
{
	prevPos = transform.position;

	// By default loop all animations
	animation.wrapMode = WrapMode.Loop;

	// We are in full control here - don't let any other animations play when we start
	animation.Stop();
	animation.Play("idle");
}

function Update ()
{
	if (Time.deltaTime == 0)//deltaTime can be 0 when game pauses
	{
		return;
	}
	transformVelocity = (transform.position - prevPos) / Time.deltaTime;
	prevPos = transform.position;

	var horizontalVelocity = transformVelocity;// currentVelocity;
	horizontalVelocity.y = 0;
	
	currentSpeed = (horizontalVelocity.magnitude + lastSpeed) / 2;
	lastSpeed = currentSpeed;

	if (transformVelocity.y < verticalFallingSpeed)
	{
		//Debug.Log("Falling!");
		animation.CrossFade ("jump");
		isFalling = true;
	}
	else
	{	
		if (isFalling == true) 
		{
			animation.Play("jump");	
			isFalling = false;
		}
		else if (currentSpeed > walkSpeed + 2.0)
		{
			animation.CrossFade("run"); 
		}
		else if (currentSpeed > 0.1)
		{
			animation.CrossFade("walk", 0.5);
		}
		// Fade out walk and run
		else
		{
			animation.CrossFade("idle", 0.5);
		}
	}		
	animation["run"].normalizedSpeed = runSpeedScale;
	animation["walk"].normalizedSpeed = walkSpeedScale;
	
}


@script RequireComponent(Animation)