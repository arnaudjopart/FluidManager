using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cube : MonoBehaviour {

    #region Public And Protected Members
        
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

    public void SetOrigin(int _origin)
    {
        m_origin = _origin;
    }
    public int GetOrigin()
    {
        return m_origin;
    }
    #endregion

    #region utils

    #endregion
    // Use this for initialization

    #region Private Members

    private Vector2 m_positionInGrid;
    private List<Material> m_listOfMaterial;
    private MeshRenderer m_meshRenderer;
    private FluidPropagation m_map;
    private int m_origin;
    #endregion

}
