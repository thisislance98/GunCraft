//var scale = 10.0;
var speed = 1.0;
var windDirection : int = 0;
var globalScale = 20;
var randomize = false;
var scaleX = 0.01;
var scaleY = 0.01;
var scaleZ = 0.02;
var waveX = true;
var waveY = true;
var waveZ = true;

private var baseHeight : Vector3[];
 
function Start () {
	if(randomize == true) {
	globalScale -= Random.Range(0, globalScale/2);
	speed += Random.Range(-speed/4, speed/4);
	}
	transform.localEulerAngles.y = windDirection;
}

function Update () {
		if(transform.gameObject.isStatic == false) {
		
			//Wind
			transform.localEulerAngles.y = Mathf.Lerp (transform.localEulerAngles.y, windDirection, Time.deltaTime);
		
        var mesh : Mesh = GetComponent(MeshFilter).mesh;
 
        if (baseHeight == null)
                baseHeight = mesh.vertices;
 
        var vertices = new Vector3[baseHeight.Length];
        for (var i=0;i<vertices.Length;i++)
        {
                var vertex = baseHeight[i];
                if(waveY == true) {
                vertex.y += Mathf.Sin(Time.time * speed+ baseHeight[i].x*globalScale + baseHeight[i].y*globalScale + baseHeight[i].z*globalScale) * scaleY;
                }
                if(waveZ == true) {
                vertex.z += Mathf.Sin(Time.time * speed+ baseHeight[i].x*globalScale + baseHeight[i].y*globalScale + baseHeight[i].z*globalScale) * scaleZ;
                }
                if(waveX == true) {
                vertex.x += Mathf.Sin(Time.time * speed+ baseHeight[i].x*globalScale + baseHeight[i].y*globalScale + baseHeight[i].z*globalScale) * scaleX;
                }
                
                vertices[i] = vertex;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        }
}