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
    public int Weight
    {
        get { return m_weight; }
        set { m_weight = value; }
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

    private int m_weight; 
    private Material m_originalMaterial;
    private int m_origin;

    #endregion

}
