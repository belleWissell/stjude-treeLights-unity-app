using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AAMVC.Unity
{
    public class DrawGrid : MonoBehaviour
    {
        // ****************************************************************************
        // marnold drawing lines in GL adopted from:
        // https://github.com/UnityCommunity/UnityLibrary/blob/master/Assets/Scripts/Helpers/DrawGLLine.cs
        // ****************************************************************************
 
        public Vector3 gridCornerLL;
        public Vector3 gridCornerUR;
        public int numberOfGridSquaresX;
        public int numberOfGridSquaresY;
        public int numberOfGridSquaresZ;
        public  Color lineColor = Color.red;

        public bool isXYPlane = false;
        public bool isYZPlane = false;
        public bool isXZPlane = false;

        Material lineMaterial;
        public ApplicationControl AppController;

        private bool doDrawGrid = true;

        // ****************************************************************************
        // GRID geometry
        private static int maxNumberOfPoints = 200;

        private Vector3[] arrayOfLineVerts= new Vector3[maxNumberOfPoints];
        private int actualNumberOfGridLinesToDraw = 0;
        //private float gridW = 2000f;
        //private float gridH = 2000f;
        private int gridLineCount = 20;

        void Awake()
        {
            if (AppController == null)
                AppController = GetComponent<ApplicationControl>();
            // must be called before trying to draw lines..
            CreateLineMaterial();
        }

        // Start is called before the first frame update
        void Start()
        {
            CreateGridPoints();
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        // cannot call this on update, line wont be visible then.. and if used OnPostRender() thats works when attached to camera only
        void OnRenderObject()
        {
            int i;
            
            if (doDrawGrid)
            {
                lineMaterial.SetPass(0);

                GL.PushMatrix();
                GL.MultMatrix(transform.localToWorldMatrix);

                GL.Begin(GL.LINES);
                GL.Color(lineColor);
                // start line from transform position
                //GL.Vertex(transform.position);
                // end line 100 units forward from transform position
                //GL.Vertex(transform.position + transform.forward * 10);

                for (i = 0; i < actualNumberOfGridLinesToDraw; ++i)
                {
                    GL.Vertex(arrayOfLineVerts[i*2]);
                    GL.Vertex(arrayOfLineVerts[(i*2) + 1]);
                }
                GL.End();
                GL.PopMatrix();
            }
        }


        void CreateGridPoints()
        {
            float gridW = gridCornerUR.x - gridCornerLL.x;
            float gridH = gridCornerUR.y - gridCornerLL.y;
            float gridD = gridCornerUR.z - gridCornerLL.z;

            if (isXYPlane)
                numberOfGridSquaresZ = 1;
            else if (isXZPlane)
                numberOfGridSquaresY = 1;
            else if (isYZPlane)
                numberOfGridSquaresX = 1;
            
            float eachLineStart_x;
            float eachLineEnd_x;
            float delta_x = gridW / numberOfGridSquaresX;
            
            float eachLineStart_y;
            float eachLineEnd_y;
            float delta_y = gridH / numberOfGridSquaresY;

            float eachLineStart_z;
            float eachLineEnd_z;
            float delta_z = gridD / numberOfGridSquaresZ;

            /*
            if (isXYPlane)
            {
                delta_x = gridW / numberOfGridSquaresH;
                delta_y = gridH / numberOfGridSquaresV;
                delta_z = 0;
            }
            else if (isXZPlane)
            {
                delta_x = gridW / numberOfGridSquaresH;
                delta_y = 0;
                delta_z = gridD / numberOfGridSquaresV;
            }
            else if (isYZPlane)
            {
                delta_x = 0;
                delta_y = gridW / numberOfGridSquaresH;
                delta_z = gridD / numberOfGridSquaresV;
            }
            */
            
            int i;

            int runningVertCounter = 0;

            if (isXYPlane)
            {
                // horizLines (x axis)
                for (i = 0; i <= numberOfGridSquaresY; ++i)
                {
                    eachLineStart_x = gridCornerLL.x;
                    eachLineStart_y = gridCornerLL.y + (delta_y * i);
                    eachLineStart_z = gridCornerLL.z + (delta_z * i);

                    eachLineEnd_x = gridCornerUR.x;
                    eachLineEnd_y = gridCornerLL.y + (delta_y * i);
                    eachLineEnd_z = gridCornerLL.z + (delta_z * i);

                    arrayOfLineVerts[runningVertCounter] =
                        new Vector3(eachLineStart_x, eachLineStart_y, eachLineStart_z);
                    runningVertCounter += 1;
                    arrayOfLineVerts[runningVertCounter] = new Vector3(eachLineEnd_x, eachLineEnd_y, eachLineEnd_z);
                    runningVertCounter += 1;

                    actualNumberOfGridLinesToDraw += 1;
                }

                //vert lines (y axis)
                for (i = 0; i <= numberOfGridSquaresX; ++i)
                {
                    eachLineStart_x = gridCornerLL.x + (delta_x * i);
                    eachLineStart_y = gridCornerLL.y;
                    eachLineStart_z = gridCornerLL.z + (delta_z * i);

                    eachLineEnd_x = gridCornerLL.x + (delta_x * i);
                    eachLineEnd_y = gridCornerUR.y;
                    eachLineEnd_z = gridCornerLL.z + (delta_z * i);
                    
                    arrayOfLineVerts[runningVertCounter] = new Vector3(eachLineStart_x, eachLineStart_y, eachLineStart_z);
                    runningVertCounter += 1;
                    arrayOfLineVerts[runningVertCounter] = new Vector3(eachLineEnd_x, eachLineEnd_y, eachLineEnd_z);
                    runningVertCounter += 1;

                    actualNumberOfGridLinesToDraw += 1;
                }
            }
            else if (isXZPlane)
            {
                // horizLines (X axis)
                for (i = 0; i <= numberOfGridSquaresZ; ++i)
                {
                    eachLineStart_x = gridCornerLL.x;
                    eachLineStart_y = gridCornerLL.y + (delta_y * i);
                    eachLineStart_z = gridCornerLL.z + (delta_z * i);

                    eachLineEnd_x = gridCornerUR.x;
                    eachLineEnd_y = gridCornerLL.y + (delta_y * i);
                    eachLineEnd_z = gridCornerLL.z + (delta_z * i);
                    
                    arrayOfLineVerts[runningVertCounter] = new Vector3(eachLineStart_x, eachLineStart_y, eachLineStart_z);
                    runningVertCounter += 1;
                    arrayOfLineVerts[runningVertCounter] = new Vector3(eachLineEnd_x, eachLineEnd_y, eachLineEnd_z);
                    runningVertCounter += 1;

                    actualNumberOfGridLinesToDraw += 1;
                }
                
                //vert lines (Z axis)
                for (i = 0; i <= numberOfGridSquaresX; ++i)
                {
                    eachLineStart_x = gridCornerLL.x + (delta_x * i);
                    eachLineStart_y = gridCornerLL.y + (delta_y * i);
                    eachLineStart_z = gridCornerLL.z;

                    eachLineEnd_x = gridCornerLL.x + (delta_x * i);
                    eachLineEnd_y = gridCornerLL.y + (delta_y * i);
                    eachLineEnd_z = gridCornerUR.z;
                    
                    arrayOfLineVerts[runningVertCounter] = new Vector3(eachLineStart_x, eachLineStart_y, eachLineStart_z);
                    runningVertCounter += 1;
                    arrayOfLineVerts[runningVertCounter] = new Vector3(eachLineEnd_x, eachLineEnd_y, eachLineEnd_z);
                    runningVertCounter += 1;

                    actualNumberOfGridLinesToDraw += 1;
                }
            }
            else if (isYZPlane)
            {
                // horizLines (Y axis)
                for (i = 0; i <= numberOfGridSquaresZ; ++i)
                {
                    eachLineStart_x = gridCornerLL.x + (delta_x * i);
                    eachLineStart_y = gridCornerLL.y;
                    eachLineStart_z = gridCornerLL.z + (delta_z * i);

                    eachLineEnd_x = gridCornerLL.x + (delta_x * i);
                    eachLineEnd_y = gridCornerUR.y;
                    eachLineEnd_z = gridCornerLL.z + (delta_z * i);
                    
                    arrayOfLineVerts[runningVertCounter] = new Vector3(eachLineStart_x, eachLineStart_y, eachLineStart_z);
                    runningVertCounter += 1;
                    arrayOfLineVerts[runningVertCounter] = new Vector3(eachLineEnd_x, eachLineEnd_y, eachLineEnd_z);
                    runningVertCounter += 1;

                    actualNumberOfGridLinesToDraw += 1;
                }
                
                //vert lines (Z axis)
                for (i = 0; i <= numberOfGridSquaresY; ++i)
                {
                    eachLineStart_x = gridCornerLL.x + (delta_x * i);
                    eachLineStart_y = gridCornerLL.y + (delta_y * i);
                    eachLineStart_z = gridCornerLL.z;

                    eachLineEnd_x = gridCornerLL.x + (delta_x * i);
                    eachLineEnd_y = gridCornerLL.y + (delta_y * i);
                    eachLineEnd_z = gridCornerUR.z;
                    
                    arrayOfLineVerts[runningVertCounter] = new Vector3(eachLineStart_x, eachLineStart_y, eachLineStart_z);
                    runningVertCounter += 1;
                    arrayOfLineVerts[runningVertCounter] = new Vector3(eachLineEnd_x, eachLineEnd_y, eachLineEnd_z);
                    runningVertCounter += 1;

                    actualNumberOfGridLinesToDraw += 1;
                }
            }
            
            Debug.Log("[DRAWGRID] points processed");
        }
        
        void CreateLineMaterial()
        {
            // Unity has a built-in shader that is useful for drawing simple colored things
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }

    }
}
