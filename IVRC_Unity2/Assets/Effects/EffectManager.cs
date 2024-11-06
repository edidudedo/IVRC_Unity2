using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private VertexPaintTool vertexPaintTool;
    private GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        obj = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(obj.activeSelf + "" + vertexPaintTool.isPainted);
        if (!obj.activeSelf && vertexPaintTool.isPainted){
            obj.SetActive(true);
        }
    }
}
