using Assets.Data_Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ClassSlider : MonoBehaviour
{
    private int _classID;
    public int ClassID { get { return _classID; } }

    private Slider _slider;

    private GameObject CubicCenter;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitialiseClassControl(int classId, GameObject cubicCent)
    {
        _classID = classId;
        CubicCenter = cubicCent;
        if (_slider == null)
        {
            _slider = GetComponent<Slider>();
        }        
        _slider.onValueChanged.AddListener(ChangeAlphaOnValue);
    }


    public void ChangeAlphaOnValue(float value)
    {
        ChangeAlpha(ClassID, value);
    }

    public void ChangeAlpha(int classID, float alphaVal)
    {
        foreach (Transform dp in CubicCenter.transform)
        {
            if (dp.TryGetComponent<DataPoint_Model>(out var DPM))
            {
                if (DPM.Class_ID == classID)
                {
                    Renderer dpRend = dp.GetComponent<Renderer>();
                    Color dpCol = dpRend.material.color;
                    Color newCol = new Color(dpCol.r, dpCol.g, dpCol.b, alphaVal / 255);
                    dpRend.material.color = newCol;
                }
            }
        }
    }
}
