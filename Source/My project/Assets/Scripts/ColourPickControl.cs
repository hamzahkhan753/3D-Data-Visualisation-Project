using Assets.Data_Classes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class ColourPickControl : MonoBehaviour
{
    private GraphViewControlScript GraphViewControlCO;
    private FlexibleColorPicker ClassColourPick;
    private GameObject CubicCenter;
    private int _classID;
    private TextMeshProUGUI ClassTitle;

    internal Color StartColor;

    public int ClassID {  get { return _classID; } }

    internal bool colourPickStarted = false;
    public bool colourPickEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        if (GraphViewControlCO == null)
        {
            throw new MissingReferenceException("Grayscale Control object missing. Please check ColourPickControl instantiation.");
        }
        else
        {
            GraphViewControlCO.GrayscaleEvent += ChangeColourPickEnable;
        }
        ClassColourPick = gameObject.transform.parent.GetComponentInChildren<FlexibleColorPicker>();
        if (ClassColourPick != null)
        {
            ClassColourPick.color = StartColor;
            colourPickStarted = true;
        }
        ClassTitle = gameObject.transform.parent.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (colourPickStarted)
        {
            ClassColourPick.gameObject.SetActive(false);
            colourPickStarted = false;
        }
        if (CubicCenter != null && colourPickEnabled)
        {
            foreach (Transform dp in CubicCenter.transform)
            {
                if (dp.TryGetComponent<DataPoint_Model>(out var DPM) && DPM.Class_ID == ClassID)
                {
                    Renderer dpRend = dp.GetComponent<Renderer>();
                    Color newCol = new Color(ClassColourPick.color.r, ClassColourPick.color.g, ClassColourPick.color.b, 
                        dpRend.material.color.a);
                    dpRend.material.color = newCol;
                    ClassTitle.color = ClassColourPick.color;                    
                }
            }
        }
    }

    public void ChangeColourPickEnable(bool GrayscaleOn)
    {
        colourPickEnabled = !GrayscaleOn;
    }

    internal void InitialiseClassSettings(int classID, GameObject cubeCent, GraphViewControlScript grayscaleCO, Color startingColor)
    {
        _classID = classID;
        CubicCenter = cubeCent;
        GraphViewControlCO = grayscaleCO;
        StartColor = startingColor;
    }

    public void ToggleColourPickVisibility()
    {
        ClassColourPick.gameObject.SetActive(!ClassColourPick.gameObject.activeSelf);
        //gameObject.transform.parent.transform.
    }
}
