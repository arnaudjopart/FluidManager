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

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mousePosition, Vector3.forward*11, out hit))
            {
                string name = hit.collider.gameObject.name;
                int number = int.Parse(name);
                //print(number);
                m_listOfIndexToFlood.Add(number);

            }
            

        }
        if( Input.GetMouseButtonDown( 1 ) )
        {
            CreateMap();
        }
        bool isAllowedToGoNextStep = Time.time > m_nextStepStartTimer+m_timeBetweenSteps;
        if( m_isFillingAuto && isAllowedToGoNextStep )
        {
            m_nextStepStartTimer = Time.time;
            UpdateMap();
        }
        else
        {

        }
    }

    public void StartPropagation(Vector2 _positionOfStart)
    {
        //m_listOfPositionToFlood.Add( _positionOfStart );
    }
    #endregion

    #region Utils

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

    private void CreateMap()
    {
        ClearMap();

        m_length = m_width * m_height;
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
                cube.GetComponent<MeshRenderer>().material = m_materialList[ randomValue ];
                cube.transform.SetParent( m_transform, false );
                m_listOfCubes.Add(index, cube);
            }
        }
    }

    private void UpdateMap()
    {
        bool isFlooding = m_listOfIndexToFlood.Count > 0;

        if (isFlooding)
        {
            int currentIndex = m_listOfIndexToFlood[0];
            //print("check "+currentIndex);
            
            GameObject cube;

            if (m_listOfCubes.TryGetValue(currentIndex, out cube))
            {
                cube.GetComponent<MeshRenderer>().material = m_materialList[1];
                m_map1D[ currentIndex ] = 1;
            }else
            {
                Debug.LogError( "No Cube found" );
            }
            m_listOfIndexToFlood.RemoveAt( 0 );
            m_listOfIndexAlreadyFlooded.Add( currentIndex );
            GetNeighBours( currentIndex );


            if( m_listOfIndexToFlood.Count > 0 )
            {
                GameObject nextCube;

                if( m_listOfCubes.TryGetValue( m_listOfIndexToFlood[ 0 ], out nextCube ) )
                {
                    nextCube.GetComponent<MeshRenderer>().material = m_materialList[ 3 ];
                }
            } 

        }
       
    }

    private void GetNeighBours(int _index)
    {
        List<int> neigborsIndexList = GenerateNeighbors(_index);

        foreach (int index in neigborsIndexList )
        {
            
            bool isRock = m_map1D[ index ] ==2;

            if( !isRock )
            {
                //print( "test" );
                bool isAlreadyFlooded = CheckIfIndexIsInList(index,m_listOfIndexAlreadyFlooded);
                bool isAlreadyWaitingForCheck = CheckIfIndexIsInList(index,m_listOfIndexToFlood);

                if( !isAlreadyFlooded && !isAlreadyWaitingForCheck )
                {
                    m_listOfIndexToFlood.Add( index );
                    //print( "add " + index );
                    GameObject nextCube;

                    if( m_listOfCubes.TryGetValue( index, out nextCube ) )
                    {
                        nextCube.GetComponent<MeshRenderer>().material = m_materialList[ 4 ];
                    }
                }
            }             

        }
    }

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
    private List<int> GenerateNeighbors(int _i)
    {
        List<int> list = new List<int>();

        bool thereIsANeighborUPHere = false;
        bool thereIsANeighborOnRight = false;
        bool thereIsANeighborDownHere = false;
        bool thereIsANeighborOnleft = false;

        // neighbor UP
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

    #endregion

    #region Private Members
    private int m_length; 
    private int [] m_map1D;
    private float m_nextStepStartTimer;
    private Transform m_transform;
    private List<int> m_listOfIndexToFlood = new List<int>();
    private List<int> m_listOfIndexAlreadyFlooded = new List<int>();
    private Dictionary<int, GameObject> m_listOfCubes = new Dictionary<int, GameObject>();
    
    #endregion
}
