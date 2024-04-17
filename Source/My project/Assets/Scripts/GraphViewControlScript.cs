using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UniUI = UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GraphViewControlScript : MonoBehaviour
{
    public delegate void GrayscaleEventManager(bool GrayscaleOn);
    public event GrayscaleEventManager GrayscaleEvent;

    [Header("Grayscale Mode Flag")]
    public bool IsGrayscale;

    [Header("Class Control Sliders")]
    public UniUI.Slider GrayscaleSlider;
    public UniUI.Slider AxisToggleSlider;

    [Header("Axis Label Objects")]
    public TextMeshProUGUI XAxisLabel;
    public TextMeshProUGUI YAxisLabel;
    public TextMeshProUGUI ZAxisLabel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Swap_Grayscale_View()
    {
        if (GrayscaleSlider != null)
        {
            switch (GrayscaleSlider.value)
            {
                case 0:
                    GrayscaleSlider.value = 1;
                    IsGrayscale = true;
                    break;
                case 1:
                    GrayscaleSlider.value = 0;
                    IsGrayscale = false;
                    break;
                default:
                    MessageBox.Show("There seems to be an error. Alert developers that the Grayscale Slider has an invalid value.",
                        "Grayscale Slider Value Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }
            GrayscaleEvent?.Invoke(IsGrayscale);
        }
        else
        {
            MessageBox.Show("There seems to be an error. Please alert developers that the Grayscale Slider couldn't be found.",
                "Grayscale Slider Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    public void Toggle_Axis_Labels()
    {
        if (AxisToggleSlider != null)
        {
            switch (AxisToggleSlider.value)
            {
                case 0:
                    AxisToggleSlider.value = 1;
                    if (XAxisLabel != null && YAxisLabel != null && ZAxisLabel != null)
                    {
                        XAxisLabel.enabled = true;
                        YAxisLabel.enabled = true;
                        ZAxisLabel.enabled = true;
                    }
                    break;
                case 1:
                    AxisToggleSlider.value = 0;
                    if (XAxisLabel != null && YAxisLabel != null && ZAxisLabel != null)
                    {
                        XAxisLabel.enabled = false;
                        YAxisLabel.enabled = false;
                        ZAxisLabel.enabled = false;
                    }
                    break;
                default:
                    MessageBox.Show("There seems to be an error. Alert developers that the Axis Toggle Slider has an invalid value.",
                        "Axis Toggle Slider Value Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }
        }
        else
        {
            MessageBox.Show("There seems to be an error. Please alert developers that the Axis Toggle Slider couldn't be found.",
                "Axis Toggle Slider Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

    }
}
