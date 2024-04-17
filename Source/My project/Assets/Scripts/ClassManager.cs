using Assets.Data_Classes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassManager : MonoBehaviour
{
    public GameObject CubicCenter;

    public ColourPallet Pallet;
    public int ClassCount;

    public GameObject ClassControlLayout;
    public GameObject ClassControlContent;

    #region Start + Update
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    public void UpdateClasses(int classCount, ColourPallet pal, GraphViewControlScript GrayscaleCS)
    {
        // Load class count and pallet. If pallet does not have enough colours, create a default pallet
        ClassCount = classCount;
        if (pal.Pallet.Length >= ClassCount)
        {
            Pallet = pal;
        }
        else
        {
            Pallet = new ColourPallet();
        }

        for (int i = 1; i <= ClassCount; i++) 
        { 
            var controlUI = Instantiate(ClassControlLayout, ClassControlContent.transform);

            foreach (Transform obj in controlUI.transform)
            {
                // Rename objects in Scene Viewer
                if (obj.name.Contains("X"))
                {
                    obj.name = obj.name.Replace("X", i.ToString());
                }
                // Edit class title in UI (visible to user)
                if (obj.TryGetComponent<TextMeshProUGUI>(out var classTitle) && classTitle.text.Contains("X"))
                {
                    classTitle.text = classTitle.text.Replace("X", i.ToString());
                    classTitle.color = Pallet.Pallet[i - 1];
                }
                // Setup Alpha Slider controls
                if (obj.TryGetComponent<ClassSlider>(out var slider))
                {
                    //slider.onValueChanged.AddListener(ChangeAlphaOnValue);
                    slider.InitialiseClassControl(i, CubicCenter);
                }
                // Setup Colour Picker Button Controls
                if (obj.TryGetComponent<ColourPickControl>(out var colourPickControl))
                {
                    colourPickControl.InitialiseClassSettings(i, CubicCenter, GrayscaleCS, pal.Pallet[i - 1]);
                }
            }            
        }
    }
}
