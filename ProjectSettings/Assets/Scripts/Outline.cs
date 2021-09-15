using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    Material mat;
    public float outlineThickness = 0.1f;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsGameActive && Time.time >= time)
		{
            mat.SetFloat("_OutlineThick", 0);
		}
    }


    public void Selected()
	{
        mat.SetFloat("_OutlineThick", outlineThickness);
        time = Time.time + 0.1f;
	}

}
