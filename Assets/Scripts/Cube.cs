using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cube : MonoBehaviour {

    #region Public And Protected Members
    public Material m_waterMaterial;
    #endregion

    #region Main Methods
    void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        ChangeToWater();
    }
    public void Initialise(List<Material> _listOfAvailableMaterials,FluidPropagation _map, int _x, int _y)
    {
        m_listOfMaterial = _listOfAvailableMaterials;
        m_map = _map;
        m_positionInGrid = new Vector2( _x, _y );
    }

    public void ChangeToWater()
    {
        m_meshRenderer.material = m_listOfMaterial[ 1 ];
        m_map.StartPropagation( m_positionInGrid );

    }
    #endregion

    #region utils

    #endregion
    // Use this for initialization

    #region Private Members

    private Vector2 m_positionInGrid;
    private int m_indexInList;
    private List<Material> m_listOfMaterial;
    private MeshRenderer m_meshRenderer;
    private FluidPropagation m_map;
    #endregion

}
