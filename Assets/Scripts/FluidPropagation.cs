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
        m_length = m_width * m_height;
        m_map1D = new int[ m_length ];
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
                print(number);
                m_listOfIndexToFlood.Add(number);

            }
        }
        bool isAllowedToGoNextStep = Time.time > m_nextStepStartTimer+m_timeBetweenSteps;
        if( m_isFillingAuto && isAllowedToGoNextStep )
        {
            print("Update");
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

    private void CreateMap()
    {
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
            print("check "+currentIndex);
            GetNeighBours(currentIndex);

            GameObject cube;

            if (m_listOfCubes.TryGetValue(currentIndex, out cube))
            {
                cube.GetComponent<MeshRenderer>().material = m_materialList[1];
            }
            m_listOfIndexAlreadyFlooded.Add(currentIndex);
            m_listOfIndexToFlood.RemoveAt(0);
            print("next : " + m_listOfIndexToFlood[0]);

            GameObject nextCube;

            if (m_listOfCubes.TryGetValue(m_listOfIndexToFlood[0], out nextCube))
            {
                nextCube.GetComponent<MeshRenderer>().material = m_materialList[3];
            }

        }
       
    }

    private void GetNeighBours(int _index)
    {
        int[] neigboursIndex = new int[4];

        int neighbourUp = _index - 1;
        int neighbourRight = _index + m_height;
        int neighbourDown = _index + 1;
        int neighbourLeft = _index - m_height;

        neigboursIndex[0] = neighbourUp;
        neigboursIndex[1] = neighbourRight;
        neigboursIndex[2] = neighbourDown;
        neigboursIndex[3] = neighbourLeft;

        foreach (int nb in neigboursIndex)
        {
            if (nb >= 0)
            {
                bool isAlreadyFlooded = false;

                foreach ( int indexToCheck in m_listOfIndexAlreadyFlooded)
                {
                    if (nb == indexToCheck)
                    {
                        isAlreadyFlooded = true;
                        break;
                    }
                    else
                    {
                        isAlreadyFlooded = false;
                    }
                }

                if (!isAlreadyFlooded)
                {
                    m_listOfIndexToFlood.Add(nb);
                    print("add " + nb);
                }

            }
        }
    }
    #endregion

    #region Private Members

    private int m_floodingIndex=0;
    
    private int m_length; 
    private int [] m_map1D;
    private float m_nextStepStartTimer;
    private Transform m_transform;
    private List<int> m_listOfIndexToFlood = new List<int>();
    private List<int> m_listOfIndexAlreadyFlooded = new List<int>();
    private Dictionary<int, GameObject> m_listOfCubes = new Dictionary<int, GameObject>();
    #endregion
}
