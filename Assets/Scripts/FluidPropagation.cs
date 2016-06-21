using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FluidPropagation : MonoBehaviour {


    #region Public And protected Members
    [Header("Size Of the Map")]
    public int m_width;
    public int m_height;

    [Header("Update Parameters")]
    public float m_timeBetweenSteps;
    public bool m_isFillingAuto = false;
    public int[] listOfStartFlood;
    public GameObject m_cubePrefab;
    public List<Material> m_materialList;

    public bool m_floodIsActive;
    #endregion

    #region Main Methods

    void Awake()
    {
        m_transform = GetComponent<Transform>();
    }

    void Start()
    {
        CreateMap();        
    }

    void Update()
    {

        ManageInput();
        
        bool isAllowedToGoNextStep = Time.time > m_nextStepStartTimer+m_timeBetweenSteps;
        if( m_isFillingAuto && isAllowedToGoNextStep )
        {
            m_nextStepStartTimer = Time.time;
            UpdateMap();
        }
    }   

    public void StartPropagation(Vector2 _positionOfStart)
    {
        //m_listOfPositionToFlood.Add( _positionOfStart );
    }
    #endregion

    #region Utils
    /// <summary>
    /// Manage the input controls
    /// </summary>
    private void ManageInput()
    {
        var d = Input.GetAxis("Mouse ScrollWheel");
        if( d > 0f )
        {
            // scroll up
            Camera.main.orthographicSize += .5f; 
        }
        else if( d < 0f )
        {
            // scroll down
            Camera.main.orthographicSize -= .5f;
        }
        if( Input.GetMouseButtonDown( 0 ) )
        {
            if( m_floodIsActive )
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition = Camera.main.ScreenToWorldPoint( mousePosition );
                RaycastHit hit;
                if( Physics.Raycast( mousePosition, Vector3.forward * 11, out hit ) )
                {
                    string name = hit.collider.gameObject.name;
                    int number = int.Parse(name);
                    m_listOfIndexToFlood.Add( number );
                }
            }else
            {
                if( m_PathOriginAndDestination.Count < 2 )
                {
                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition = Camera.main.ScreenToWorldPoint( mousePosition );
                    RaycastHit hit;
                    if( Physics.Raycast( mousePosition, Vector3.forward * 11, out hit ) )
                    {
                        string name = hit.collider.gameObject.name;
                        int number = int.Parse(name);
                        m_PathOriginAndDestination.Add( number );
                        GameObject cube = FindObjectInDictionary(number, m_listOfCubes);
                        
                        cube.GetComponent<Cube>().OriginalMaterial = cube.GetComponent<Cube>().m_meshRenderer.material;
                        cube.GetComponent<Cube>().m_meshRenderer.material = m_materialList[ 4 ];
                        

                    }
                    
                }
                if( m_PathOriginAndDestination.Count == 2 )
                {
                    m_destinationIndex = m_PathOriginAndDestination[ 1 ];
                    m_listOfIndexToFlood.Add( m_PathOriginAndDestination[0] );
                    
                }
            }
            

        }
        if( Input.GetMouseButtonDown( 1 ) )
        {
            ClearPath();
        }
        if( Input.GetKeyDown( KeyCode.Space ) )
        {
            CreateMap();
        }
    }
    private void ClearMap()
    {
        m_listOfIndexToFlood = new List<int>();
        m_listOfIndexAlreadyFlooded = new List<int>();
        m_listOfCubes = new Dictionary<int, GameObject>();

        foreach(Transform obj in m_transform )
        {
            GameObject.Destroy( obj.gameObject );
        }
    }
    /// <summary>
    /// This method creates the map
    /// </summary>
    
    private void ClearPath()
    {
        while( m_PathOriginAndDestination.Count > 0 )
        {
            GameObject cube = FindObjectInDictionary(m_PathOriginAndDestination[0],m_listOfCubes);
            cube.GetComponent<Cube>().m_meshRenderer.material = cube.GetComponent<Cube>().OriginalMaterial;
            m_PathOriginAndDestination.RemoveAt( 0 );
        }
            
        m_PathOriginAndDestination = new List<int>();
    }

    private void CreateMap()
    {
        ClearPath();
        ClearMap();
                
        m_length = m_width * m_height;

        //m_map2D = new int[ m_width, m_height ];
        m_map1D = new int[ m_length ];

        for( int x = 0; x < m_width; x++ )
        {
            for( int y = 0; y < m_height; y++ )
            {
                int randomValue = Random.Range(0,3);
                int index = x*m_height+y;
                
                m_map1D[ index ] = randomValue;

                Vector3 positionOfCube = new Vector3(-m_width*.5f+x,m_height*.5f-y,0);

                GameObject cube = Instantiate(m_cubePrefab, positionOfCube, Quaternion.identity) as GameObject;
                cube.name = index.ToString();
                Cube cubeScript = cube.GetComponent<Cube>();
                cubeScript.m_meshRenderer.material = m_materialList[ randomValue ];
                cubeScript.OriginalMaterial = m_materialList[ randomValue ];
                cubeScript.Weight = randomValue;
                cube.transform.SetParent( m_transform, false );
                m_listOfCubes.Add(index, cube);
            }
        }
    }

    private void UpdateMap()
    {
        m_isProcessing = m_listOfIndexToFlood.Count > 0;

        if ( m_isProcessing )
        {
            BreathFirstPathFinding();
            //DijkstraPathFinding();



        }
       
    }
    // TO tweak !
    private void DijkstraPathFinding()
    {
        int currentIndex = m_listOfIndexToFlood[0];
        
        bool destinationIsReached = currentIndex == m_destinationIndex;

        if( destinationIsReached )
        {
            GameObject cube = FindObjectInDictionary(currentIndex, m_listOfCubes);
            if( cube )
            {
                m_isProcessing = false;
                RetrievePath();
            }
        }
        else
        {
            GameObject cube = FindObjectInDictionary(currentIndex, m_listOfCubes);
            if( cube )
            {
                Cube cubeScript = cube.GetComponent<Cube>();
                if( m_floodIsActive )
                {
                    cubeScript.m_meshRenderer.material = m_materialList[ 1 ];
                    m_map1D[ currentIndex ] = 1;
                }
                else
                {
                    cubeScript.m_meshRenderer.material = cubeScript.OriginalMaterial;
                }
            }
            else
            {
                Debug.LogError( "No Cube found" );
            }
            m_listOfIndexToFlood.RemoveAt( 0 );
            m_listOfIndexAlreadyFlooded.Add( currentIndex );
            GetNeighBours( currentIndex );

            m_listOfIndexToFlood.Sort(SortByWeight);

            if( m_listOfIndexToFlood.Count > 0 )
            {
                GameObject nextCube = FindObjectInDictionary(m_listOfIndexToFlood[0], m_listOfCubes);
                nextCube.GetComponent<MeshRenderer>().material = m_materialList[ 3 ];

            }
        }

    }

    private void BreathFirstPathFinding()
    {
        int currentIndex = m_listOfIndexToFlood[0];

        bool destinationIsReached = currentIndex == m_destinationIndex;

        if( destinationIsReached )
        {
            GameObject cube = FindObjectInDictionary(currentIndex, m_listOfCubes);
            if( cube )
            {
                m_isProcessing = false;
                RetrievePath();
            }
        }
        else
        {
            GameObject cube = FindObjectInDictionary(currentIndex, m_listOfCubes);
            if( cube )
            {
                Cube cubeScript = cube.GetComponent<Cube>();
                if( m_floodIsActive )
                {
                    cubeScript.m_meshRenderer.material = m_materialList[ 1 ];
                    m_map1D[ currentIndex ] = 1;
                }
                else
                {
                    cubeScript.m_meshRenderer.material = cubeScript.OriginalMaterial;
                }
            }
            else
            {
                Debug.LogError( "No Cube found" );
            }
            m_listOfIndexToFlood.RemoveAt( 0 );
            m_listOfIndexAlreadyFlooded.Add( currentIndex );
            GetNeighBours( currentIndex );

            m_listOfIndexToFlood.Sort();

            if( m_listOfIndexToFlood.Count > 0 )
            {
                GameObject nextCube = FindObjectInDictionary(m_listOfIndexToFlood[0], m_listOfCubes);
                nextCube.GetComponent<MeshRenderer>().material = m_materialList[ 3 ];

            }
        }
    }
    /// <summary>
    /// This methods add the available neighbors of an index to the m_listOfIndexToFlood list
    /// </summary>
    /// <param name="_index"></param>
    private void GetNeighBours(int _index)
    {
        List<int> neigborsIndexList = GenerateNeighbors(_index);
        int origin = _index;
        foreach (int index in neigborsIndexList )
        {
            bool isRock = m_map1D[ index ] == 2;

            if( !isRock )
            {
                bool isAlreadyFlooded = CheckIfIndexIsInList(index,m_listOfIndexAlreadyFlooded);
                bool isAlreadyWaitingForCheck = CheckIfIndexIsInList(index,m_listOfIndexToFlood);

                if( !isAlreadyFlooded && !isAlreadyWaitingForCheck )
                {
                    m_listOfIndexToFlood.Add( index );

                    GameObject nextCube = FindObjectInDictionary(index, m_listOfCubes);
                    nextCube.GetComponent<MeshRenderer>().material = m_materialList[ 4 ];
                    nextCube.GetComponent<Cube>().SetOrigin(origin);
                }
            }             
        }
    }
    /// <summary>
    /// this method search for a specific index in a dictionary and return a gameobject
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_dictionary"></param>
    /// <returns></returns>
    private GameObject FindObjectInDictionary(int _index, Dictionary<int,GameObject> _dictionary)
    {
        GameObject nextCube;

        if( _dictionary.TryGetValue( _index, out nextCube ) )
        {
            return nextCube;
        }

        return null;
    }
    /// <summary>
    /// this method checks if en index appears in a list
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_list"></param>
    /// <returns></returns>
    private bool CheckIfIndexIsInList(int _index, List<int> _list)
    {
        bool isInList=false;

        foreach( int indexToCheck in _list )
        {
            if( _index == indexToCheck )
            {
                isInList = true;
                break;
            }
            else
            {
                isInList = false;
            }
        }
        return isInList;
    }
    /// <summary>
    /// This method generate a list of the neghbors indexes.
    /// </summary>
    /// <param name="_i"></param>
    /// <returns></returns>
    private List<int> GenerateNeighbors(int _i)
    {
        List<int> list = new List<int>();

        bool thereIsANeighborUPHere = false;
        bool thereIsANeighborOnRight = false;
        bool thereIsANeighborDownHere = false;
        bool thereIsANeighborOnleft = false;

        if( _i % m_height == 0 )
        {
            thereIsANeighborUPHere = false;
            thereIsANeighborOnRight = (_i + m_height)<m_length;
            thereIsANeighborDownHere = (_i + 1) % m_height > 0 && (_i+1)<m_length;
            thereIsANeighborOnleft = (_i - m_height)>=0;
        }
        else
        {
            thereIsANeighborUPHere = (_i - 1) % m_height >= 0 && (_i-1)>=0;
            thereIsANeighborOnRight = (_i + m_height)<m_length;
            thereIsANeighborDownHere = (_i + 1) % m_height > 0 && (_i+1)<m_length;
            thereIsANeighborOnleft = (_i - m_height)>=0;
        }       

        if( thereIsANeighborUPHere )
        {
            list.Add( _i - 1 );
        }
        if( thereIsANeighborOnRight )
        {
            list.Add( _i + m_height );
        }
        if( thereIsANeighborDownHere )
        {
            list.Add( _i + 1 );
        }
        if( thereIsANeighborOnleft )
        {
            list.Add(_i - m_height); 
        }
        return list;
    }
    private void RetrievePath()
    {
        m_PathSteps = new List<GameObject>();
        int security = 0;

        GameObject cube = FindObjectInDictionary(m_destinationIndex,m_listOfCubes);
        m_PathSteps.Add( cube );
        while( cube.GetComponent<Cube>().GetOrigin() !=0 )
        {
            int indexOfOrigin = cube.GetComponent<Cube>().GetOrigin();
            m_PathSteps.Add( cube );
            cube = FindObjectInDictionary( indexOfOrigin, m_listOfCubes );
            if(security>1000)
            {
                print( "security out" );
                break;
            }
        }
        print( "Found Path" );
        m_PathSteps.Add( FindObjectInDictionary( m_PathOriginAndDestination[ 0 ],m_listOfCubes ) );

        DrawPath();
    }
    private int SortByWeight(int a, int b)
    {
        return b - a;
    }
    private void DrawPath()
    {
        for (int i = m_PathSteps.Count-1; i > 0; i-- )
        {
            m_PathSteps[ i ].GetComponent<MeshRenderer>().material = m_materialList[ 3 ];
        }
    }
    #endregion

    #region Private Members

    private int m_length;
    private int m_destinationIndex;
    private int [] m_map1D;
    private bool m_isProcessing;
   // private int [,] m_map2D;
    private float m_nextStepStartTimer;
    private Transform m_transform;
    private List<int> m_listOfIndexToFlood = new List<int>();
    private List<int> m_listOfIndexAlreadyFlooded = new List<int>();
    private Dictionary<int, GameObject> m_listOfCubes = new Dictionary<int, GameObject>();
    private List<GameObject> m_PathSteps = new List<GameObject>();
    private List<int> m_PathOriginAndDestination = new List<int>();
    #endregion
}
