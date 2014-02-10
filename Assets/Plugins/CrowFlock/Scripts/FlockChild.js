/**************************************									
	Copyright 2012 Unluck Software	
 	www.chemicalbliss.com													
***************************************/
#pragma strict
#pragma downcast
public var _spawner:FlockController;
private var _wayPoint : Vector3;
private var _speed= 10;
private var _dived:boolean =true;
private var _stuckCounter:float;	//prevents looping around a waypoint
private var _damping:float;
private var _isDead:boolean = false;

function Start(){
   Wander(0);
   var sc = Random.Range(_spawner._minScale, _spawner._maxScale);
   transform.localScale=Vector3(sc,sc,sc);
   transform.position = (Random.insideUnitSphere *_spawner._spawnSphere) + _spawner.transform.position;
   transform.position.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight*1.0) +_spawner.transform.position.y;
   for (var state : AnimationState in transform.FindChild("Model").animation) {
   	 	state.time = Random.value * state.length;
   }
}

function Update() {
   transform.position += transform.TransformDirection(Vector3.forward)*_speed*Time.deltaTime;
   
   if (_isDead)
   		transform.position += Vector3.down * _speed * Time.deltaTime;
   
    if((transform.position - _wayPoint).magnitude < _spawner._waypointDistance+_stuckCounter){
        Wander(0);	//create a new waypoint
        _stuckCounter=0;
    }else{
    	_stuckCounter+=Time.deltaTime;
    }
    var rotation = Quaternion.LookRotation(_wayPoint - transform.position);
	transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _damping);

	if(_spawner._childTriggerPos){
		if((transform.position - _spawner.transform.position).magnitude < 1){
			_spawner.randomPosition();
		}
	}
	
	if (transform.position.y < 0)
		Destroy(gameObject);
}

public function Damage(damage:float)
{
	_isDead = true;
	
	for (var state : AnimationState in transform.FindChild("Model").animation)
		state.speed = 0;
}

function Wander(delay:float){
	yield(WaitForSeconds(delay));
	_damping = Random.Range(_spawner._minDamping, _spawner._maxDamping);       
    _speed = Random.Range(_spawner._minSpeed, _spawner._maxSpeed);
    if(!_dived && _spawner._diveFrequency>0 && Random.value < _spawner._diveFrequency){
		for (var state : AnimationState in transform.FindChild("Model").animation) {
   	 	if(transform.position.y < _wayPoint.y +25){
   	 	state.speed = 0.1;
   	 	state.time = 0;
   	 	}
   	 	_wayPoint.x= transform.position.x + Random.Range(-1, 1);
    	_wayPoint.z=	transform.position.z + Random.Range(-1, 1);
    	_wayPoint.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight) +_spawner.transform.position.y;
    	_wayPoint.y -= _spawner._diveValue;
    	_dived = true;
		}
	}else{
		for (var state : AnimationState in transform.FindChild("Model").animation) {
    		if(_dived){
    			state.speed = Random.Range(_spawner._minAnimationSpeed, _spawner._maxAnimationSpeed);
    		}else{
    			state.speed = _spawner._maxAnimationSpeed;
    		}   
		}
		_wayPoint= (Random.insideUnitSphere *_spawner._spawnSphere) + _spawner.transform.position;
		_wayPoint.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight*1.0) +_spawner.transform.position.y;
		_dived = false;
	}
}