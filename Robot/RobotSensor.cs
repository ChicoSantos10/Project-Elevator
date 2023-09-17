using UnityEngine;

namespace Robot
{
    public class RobotSensor : MonoBehaviour
    {
        [SerializeField] Player.PlayerController pController;
        [SerializeField] GameObject playerObject;
        [SerializeField] float visionRange;
        [SerializeField] float visionConeAngle;
        bool alerted;
        [SerializeField] Light myLight;
        [SerializeField] LayerMask obstacleLayer;
        GameObject player;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");

        }

        // Update is called once per frame
        void Update()
        {

            if (player != null && !player.GetComponent<Player.PlayerController>().dying)
            {
                
                if (Vector3.Distance(transform.parent.transform.position, player.transform.position) <= myLight.range)
                {
                    
                    Vector3 dirToTarget = (player.transform.position - transform.parent.transform.position).normalized;
                    float dstToTarget = Vector3.Distance(transform.parent.transform.position, player.transform.position);
                    if (Vector3.Angle(transform.forward, dirToTarget) <= myLight.innerSpotAngle / 2)
                    {
                        
                        if (!Physics.Raycast(transform.parent.transform.position, dirToTarget, dstToTarget, obstacleLayer))
                        {
                            Debug.DrawRay(transform.parent.transform.position, dirToTarget * dstToTarget, Color.yellow);
                            if (!alerted && !player.GetComponent<Player.PlayerController>().invincible)
                            {
                                Debug.LogWarning("Comentar a linha abaixo para o player não ser detectado");
                                player.GetComponent<Player.PlayerController>().Detected();
                                alerted = true;
                            }
                        }
                    }
                    else
                    {
                        alerted = false;
                    }
                }
                else
                {
                    alerted = false;
                }
            }

            


            
        }

        //Mesh CreateWedgeMesh()
        //{
        

        //    int segments = 10;

        //    int numTriangles = (segments * 4) + 2 + 2;
        //    int numVertices = numTriangles * 3;

        //    Vector3[] vertices = new Vector3[numVertices];
        //    int[] triangles = new int[numVertices];

        //    Vector3 bottomCenter = Vector3.zero;
        //    Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        //    Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        //    Vector3 topCenter = bottomCenter + Vector3.up * height;
        //    Vector3 topRight = bottomRight + Vector3.up * height;
        //    Vector3 topLeft = bottomLeft + Vector3.up * height;

        //    int vert = 0;

        //    //left side
        //    vertices[vert++] = bottomCenter;
        //    vertices[vert++] = bottomLeft;
        //    vertices[vert++] = topLeft;

        //    vertices[vert++] = topLeft;
        //    vertices[vert++] = topCenter;
        //    vertices[vert++] = bottomCenter;


        //    //right side
        //    vertices[vert++] = bottomCenter;
        //    vertices[vert++] = topCenter;
        //    vertices[vert++] = topRight;

        //    vertices[vert++] = topRight;
        //    vertices[vert++] = bottomRight;
        //    vertices[vert++] = bottomCenter;

        //    float currentAngle = -angle;
        //    float deltaAngle = (angle * 2) / segments;

        //    for (int i = 0; i < segments; i++)
        //    {

        //        bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
        //        bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

        //        topRight = bottomRight + Vector3.up * height;
        //        topLeft = bottomLeft + Vector3.up * height;


        //        //left side
        //        vertices[vert++] = bottomCenter;
        //        vertices[vert++] = bottomLeft;
        //        vertices[vert++] = topLeft;

        //        vertices[vert++] = topLeft;
        //        vertices[vert++] = topCenter;
        //        vertices[vert++] = bottomCenter;


        //        //right side
        //        vertices[vert++] = bottomCenter;
        //        vertices[vert++] = topCenter;
        //        vertices[vert++] = topRight;

        //        vertices[vert++] = topRight;
        //        vertices[vert++] = bottomRight;
        //        vertices[vert++] = bottomCenter;

        //        //far side
        //        vertices[vert++] = bottomLeft;
        //        vertices[vert++] = bottomRight;
        //        vertices[vert++] = topRight;

        //        vertices[vert++] = topRight;
        //        vertices[vert++] = topLeft;
        //        vertices[vert++] = bottomLeft;

        //        //top
        //        vertices[vert++] = topCenter;
        //        vertices[vert++] = topLeft;
        //        vertices[vert++] = topRight;


        //        //bottom
        //        vertices[vert++] = bottomCenter;
        //        vertices[vert++] = bottomRight;
        //        vertices[vert++] = bottomLeft;

        //        //RaycastHit2D raycasthit2D = Physics2D.Raycast(gameObject.transform.localPosition + transform.forward, GetVectorFromAngle(90), distance);
        //        //Debug.Log(raycasthit2D.point);
        //        //Debug.DrawLine(gameObject.transform.position, raycasthit2D.point, Color.red);

        //        //if (raycasthit2D.collider == null)
        //        //{
        //        //    Debug.Log("não collide");
        //        //}
        //        //else
        //        //{
        //        //    Debug.Log("está a colidir");
        //        //
        //        //    for (int j = 0; j < numVertices; j++)
        //        //    {
        //        //        if (vertices[j] == (GetVectorFromAngle(angle) * distance))
        //        //        {
        //        //            vertices[j] = raycasthit2D.point;
        //        //            vertices[j++] = raycasthit2D.point;
        //        //
        //        //        }
        //        //    }
        //        //}

        //        currentAngle += deltaAngle;

        //    }

        

        //    for (int i = 0; i < numVertices; i++)
        //    {
        //        triangles[i] = i;
        //    }

        //    mesh.Clear();
        //    mesh.vertices = vertices;
        //    mesh.triangles = triangles;
        //    mesh.RecalculateNormals();

        //    return mesh;

        //}



        //public static Vector3 GetVectorFromAngle(float angle)
        //{
        //    float angleRad = angle * (Mathf.PI/180f);
        //    return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        //}
    

    }
}
