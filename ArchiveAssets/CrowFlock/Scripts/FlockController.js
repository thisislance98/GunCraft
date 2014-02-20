/****************************************
	FlockController v1.02						
	Copyright 2012 Unluck Software	
 	www.chemicalbliss.com
 	
 	v1.01
 	Flock can now be moved freely on the stage
 	
 	v1.02
 	Script is now pragma strict																																							
*****************************************/
#pragma strict
var _childPrefab:FlockChild;			// Assign prefab with FlockChild script attached
var _childAmount:int = 250;				// Number of objects
var _spawnSphere:float = 3;				// Range around the spawner waypoints will created
var _spawnSphereHeight:float = 1.5;		// Height of the spawn sphere
var _minSpeed:float = 6;				// minimum random speed
var _maxSpeed:float = 10;				// maximum random speed
var _minScale:float = .7;				// minimum random size
var _maxScale:float = 1;				// maximum random size
var _diveValue:float = 7;				// Dive depth
var _diveFrequency:float = 0.5;			// How often dive 1 = always 0 = never
var _minDamping:float = 1;				// Rotation tween damping, lower number = smooth/slow rotation (if this get stuck in a loop, increase this value)
var _maxDamping:float = 2;
var _waypointDistance:float = 1;		// How close this can get to waypoint before creating a new waypoint (also fixes stuck in a loop)
var _minAnimationSpeed:float = 2;
var _maxAnimationSpeed:float = 4;
var _randomPositionTimer:float = 10;	// *** 
var _positionSphere:int = 25;			// If _randomPositionTimer is bigger than zero the controller will be moved to a random position within this sphere
var _positionSphereHeight = 5;			// Overides height of sphere for more controll
var _childTriggerPos:boolean;			// Runs the random position function when a child reaches the controller
var _forceChildWaypoints:boolean;		// Forces all children to change waypoints when this changes position
var _forcedRandomDelay:float = 1.5;		// Random delay added before forcing new waypoint
private var _roamers:Array = new Array();
private var _posBuffer:Vector3;
private var _counter:float;


function Start () {
	_posBuffer = transform.position;
	for(var i:int=0;i<_childAmount;i++){
   			var obj : FlockChild = Instantiate(_childPrefab);
		    obj._spawner = this;
		   _roamers.push(obj);
		}
}

function Update () {
	if(_childAmount>= 0 && _childAmount < _roamers.length){
		Destroy((_roamers.pop() as GameObject).gameObject);
	}else if (_childAmount > _roamers.length){
		var obj : FlockChild = Instantiate(_childPrefab);
		obj._spawner = this;
		_roamers.push(obj);
	}
	if(_randomPositionTimer > 0){
		if(_counter < _randomPositionTimer){
			_counter+=Time.deltaTime;
		}else{
			randomPosition();			
		}
	}
}

function randomPosition () {
	_counter = 0;
	transform.position = Random.insideUnitSphere *_positionSphere + _posBuffer;
	transform.position.y = Random.Range(-_positionSphereHeight, _positionSphereHeight)+ _posBuffer.y;
	if(_forceChildWaypoints){
		for (var i:int = 0; i < _roamers.length; i++) 
		{
			if (_roamers[i] != null)
  		 		(_roamers[i] as FlockChild).Wander(Random.value*_forcedRandomDelay);
		}	
	}
}
