using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroMenuScript : MonoBehaviour
{
    public Canvas MainMenuCanvas, MainHelpCanvas, IntroCanvas;

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

    public IntroMenuScript() 
    { 

    }

    public void GoToVisualisationScene()
    {
        SceneManager.LoadScene("3D Visual Scene");
        //SceneManager.UnloadSceneAsync("Introduction Scene");
    }

    public void GoToMainMenu()
    {
        DialogResult res = MessageBox.Show("Do you want to return to the main menu?\nYour current data visualisation will be lost.", "Exit Visualisation Screen?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (res == DialogResult.Yes)
        {
            SceneManager.LoadScene("Introduction Scene");
        }
    }

    public void ShowHelp()
    {
        if (MainHelpCanvas != null)
        {
            MainHelpCanvas.enabled = true;
            IntroCanvas.enabled = true;
        }
    }

    public void CloseHelp()
    {
        if (MainHelpCanvas != null)
        {
            MainHelpCanvas.enabled = false;
            foreach (Transform canvas in MainHelpCanvas.transform)
            {
                Canvas canv;
                if (canvas.TryGetComponent<Canvas>(out canv))
                {
                    canv.enabled = false;
                }
            }
        }
    }

    public void ChangeHelp(Canvas nextToShow)
    {
        if (nextToShow != null)
        {
            gameObject.transform.parent.parent.TryGetComponent<Canvas>(out Canvas thisCanv);

            if (thisCanv != null && nextToShow != null)
            {
                thisCanv.enabled = false;
                nextToShow.enabled = true;
            }
        }
    }

    public void Exit()
    {
        DialogResult res = MessageBox.Show("Do you want to exit the program?", "Exit Program?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (res == DialogResult.Yes)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
