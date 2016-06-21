using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cube : MonoBehaviour {

    #region Public And Protected Members

    public Material OriginalMaterial
    {
        get { return m_originalMaterial; }
        set { m_originalMaterial = value; }
    }
    public int Cost
    {
        get { return m_cost; }
        set { m_cost = value; }

    }
    public int CostSoFar
    {
        get { return m_costSoFar; }
        set { m_costSoFar = value; }
    }
    [HideInInspector]
    public MeshRenderer m_meshRenderer;

    #endregion

    #region Main Methods
    void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        
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

    private int m_cost;
    private int m_costSoFar = 0;
    private Material m_originalMaterial;
    private int m_origin;

    #endregion

}
