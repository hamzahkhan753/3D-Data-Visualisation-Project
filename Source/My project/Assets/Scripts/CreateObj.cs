using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Assets.Data_Classes;
using CsvHelper;
using SFB;
using TMPro;
using UnityEngine;
using UniUI = UnityEngine.UI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Unity.VisualScripting;
using CsvHelper.TypeConversion;
using static Assets.Data_Classes.Point_Data;
using CsvHelper.Configuration;

public class CreateObj : MonoBehaviour
{
    #region Variables

    #region Spatial Objects
    [Header("Data Space Variables")]
    /// <summary>
    /// Reference to the object at the centre (0, 0, 0) of the cubic space representing the 3 axes.
    /// </summary>
    public GameObject CubicCentre;

    /// <summary>
    /// Reference to Datapoint Model Prefab
    /// </summary>
    public GameObject DPModel;

    /// <summary>
    /// Reference to the X Axis label.
    /// </summary>
    public TextMeshProUGUI X_Text;

    /// <summary>
    /// Reference to the Y Axis label.
    /// </summary>
    public TextMeshProUGUI Y_Text;

    /// <summary>
    /// Reference to the Z Axis label.
    /// </summary>
    public TextMeshProUGUI Z_Text;

    #endregion

    #region Canvases
    [Header("Canvas References")]
    /// <summary>
    /// Reference to Main UI Canvas.
    /// </summary>
    public Canvas MainCanvas;

    /// <summary>
    /// Reference to the Canvas with Data Point info on it (Blue).
    /// </summary>
    public Canvas DataControlCanvas;

    /// <summary>
    /// Reference to the Canvas with Data Load Controls on it (Orange).
    /// </summary>
    public Canvas DataLoadCanvas;

    /// <summary>
    /// Reference to Class Control Canvas (Green).
    /// </summary>
    public Canvas ClassControlCanvas;
    #endregion

    #region Data Control
    [Header("Data Point Info Variables")]

    /// <summary>
    /// Reference to the TextMeshPro at the top of the Blue Panel.
    /// </summary>
    public TextMeshProUGUI DataInfoTitle;

    /// <summary>
    /// Reference to the TextMeshPro that will show the Class ID.
    /// </summary>
    public TextMeshProUGUI DataClassInfo;

    /// <summary>
    /// Reference to Line Graph used to visualise Point data.
    /// </summary>
    public Window_Graph LineGraph;

    /// <summary>
    /// Reference to the scroll rect object in our DC Content.
    /// </summary>
    public ScrollRect DataScrollView;

    /// <summary>
    /// Reference to the Content parent that our data control panel scrolls through.
    /// </summary>
    public GameObject DataControlContent;

    /// <summary>
    /// Reference to the button that swaps between data control and axis control
    /// </summary>
    public UniUI.Button DataVisualisationButton;
    #endregion

    #region Data Load
    [Header("Data Access Variables")]
    /// <summary>
    /// Reference to Content object in Headers List Scroll View.
    /// </summary>
    public GameObject HeaderCheckboxContent;
    /// <summary>
    /// Reference to Prefab layout of Toggle object.
    /// </summary>
    public GameObject HeaderTogglePrefab;
    /// <summary>
    /// True when a new CSV file is loaded, false when not.
    /// </summary>
    public bool CSVDataLoaded;
    #endregion

    #region Class Controls
    [Header("Classwide Control Variables")]
    /// <summary>
    /// Reference to the button that swaps between data control and Classwide control
    /// </summary>
    public UniUI.Button ClassControlButton;
    /// <summary>
    /// Reference to Class View Controller
    /// </summary>
    public ClassManager ClassControlObj;
    /// <summary>
    /// Reference to Grayscale Controller for events and passing on to other classes
    /// </summary>
    public GraphViewControlScript GraphViewControl;
    #endregion

    #endregion

    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        if (DataVisualisationButton.enabled)
        {
            DataVisualisationButton.enabled = false;
        }
        if (ClassControlButton.enabled)
        {
            ClassControlButton.enabled = false;
        }
        if (GraphViewControl == null)
        {
            GraphViewControl = ClassControlObj.GetComponent<GraphViewControlScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Methods

    #region Data Spawn Methods
    public void SpawnData()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Comma Separated Value Files", "csv"),
            new ExtensionFilter("All Files", "*")
        };
        try
        {
            string filename = StandaloneFileBrowser.OpenFilePanel("Open CSV Data", "", extensions, false).FirstOrDefault();

            if (filename == null)
            {
                return;
            }

            string read = new StreamReader(filename).ReadLine();
            List<string> headers = read.Split(',').ToList();

            if (headers.Count < 5)
            {
                throw new ArgumentException("The data file does not have enough headers necessary for 3D visualisation.");
            }

            //DataControlCanvas.enabled = false;
            //DataLoadCanvas.enabled = true;

            TMP_Dropdown[] Data_Dropdowns = DataLoadCanvas.transform.GetChild(0).GetComponentsInChildren<TMP_Dropdown>();

            foreach (TMP_Dropdown dd in Data_Dropdowns)
            {
                dd.options.Clear();
                dd.AddOptions(headers);
                dd.value = 0;
            }

            // Clear toggle objects in scroll view so no duplicates or old data are loaded in
            List<GameObject> headerRemove = new List<GameObject>();
            foreach (Transform headerToggle in HeaderCheckboxContent.transform)
            {
                headerRemove.Add(headerToggle.gameObject);
            }
            foreach (GameObject toggleRemove in headerRemove)
            {
                Destroy(toggleRemove);
            }

            // Add toggle objects for all headers in Scrollview to select dimensions
            foreach (string head in headers)
            {
                GameObject headerToggle = Instantiate(HeaderTogglePrefab, HeaderCheckboxContent.transform);
                headerToggle.name = head;
                headerToggle.GetComponentInChildren<Text>().text = head;
            }

            //headers.RemoveRange(0, 3);
            DataLoadCanvas.transform.GetChild(1).name = filename;
            //DataLoadCanvas.transform.GetChild(1).AddComponent<DataPoint_Model>();
            //DataLoadCanvas.transform.GetChild(1).GetComponent<DataPoint_Model>().Band_Names = headers;
            CSVDataLoaded = true;
            Swap_To_Canvas(DataLoadCanvas);
        }
        catch (IOException ioe)
        {
            MessageBox.Show($"An error occurred whilst loading: {ioe.Message}. Please check your file and try again.", "Data Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void Continue_Data_Spawn()
    {
        // Early branch off from the usual spawn so we're reloading datapointss instead of adding more
        if (!CSVDataLoaded)
        {
            Reload_Data();
            return;
        }

        // Get the dropdowns from the canvas to ensure headers have been selected can be loaded correctly
        TMP_Dropdown[] Data_Dropdowns = DataLoadCanvas.transform.GetChild(0).GetComponentsInChildren<TMP_Dropdown>();
        bool headers_loaded = true;
        foreach (TMP_Dropdown dd in Data_Dropdowns)
        {
            headers_loaded &= !string.IsNullOrEmpty(dd.options[dd.value].text);
        }

        if (headers_loaded)
        {
            // Map variable setup
            string point_ID = Data_Dropdowns[0].options[Data_Dropdowns[0].value].text;
            string class_id = Data_Dropdowns[1].options[Data_Dropdowns[1].value].text;
            string x_band = Data_Dropdowns[2].options[Data_Dropdowns[2].value].text;
            string y_band = Data_Dropdowns[3].options[Data_Dropdowns[3].value].text;
            string z_band = Data_Dropdowns[4].options[Data_Dropdowns[4].value].text;

            /// If the Datapoint or Class ID is toggled as Visual Data, 
            /// or if any Axis Measurement is not selected as such, the data load is not continued.
            #region Data Selection Checks
            if (HeaderCheckboxContent.transform.Find(point_ID).GetComponent<UniUI.Toggle>().isOn)
            {
                MessageBox.Show($"The column \"{point_ID}\" has been selected in the grid of visual data, " +
                    $"but has been assigned as the Datapoint ID.\nPlease either assign the correct Datapoint ID column, " +
                    $"or unselect \"{point_ID}\" in the grid below.", "Invalid Datapoint ID Setup");
                return;
            }

            if (HeaderCheckboxContent.transform.Find(class_id).GetComponent<UniUI.Toggle>().isOn)
            {
                MessageBox.Show($"The column \"{class_id}\" has been selected in the grid of visual data, " +
                    $"but has been assigned as the Class ID.\nPlease either assign the correct Class ID column, " +
                    $"or unselect \"{class_id}\" in the grid below.", "Invalid Class ID Setup");
                return;
            }

            if (!HeaderCheckboxContent.transform.Find(x_band).GetComponent<UniUI.Toggle>().isOn)
            {
                MessageBox.Show($"The column \"{x_band}\" has not been selected in the grid of visual data, " +
                    $"but has been assigned as the X Axis measurement.\nPlease either assign the correct X Axis measurement, " +
                    $"or unselect \"{x_band}\" in the grid below.", "Invalid X Axis Tracking Setup");
                return;
            }

            if (!HeaderCheckboxContent.transform.Find(y_band).GetComponent<UniUI.Toggle>().isOn)
            {
                MessageBox.Show($"The column \"{y_band}\" has not been selected in the grid of visual data, " +
                    $"but has been assigned as the Y Axis measurement.\nPlease either assign the correct Y Axis measurement, " +
                    $"or unselect \"{y_band}\" in the grid below.", "Invalid Y Axis Tracking Setup");
                return;
            }

            if (!HeaderCheckboxContent.transform.Find(z_band).GetComponent<UniUI.Toggle>().isOn)
            {
                MessageBox.Show($"The column \"{z_band}\" has not been selected in the grid of visual data, " +
                    $"but has been assigned as the Z Axis measurement.\nPlease either assign the correct Z Axis measurement, " +
                    $"or unselect \"{z_band}\" in the grid below.", "Invalid Z Axis Tracking Setup");
                return;
            }
            #endregion

            // Load datapoint objects with filename recovered from cache object
            using (var reader = new StreamReader(DataLoadCanvas.transform.GetChild(1).name))
            {                
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // Set map classing to selected bands
                    PointMap pm = new PointMap(point_ID, class_id, x_band, y_band, z_band);
                    csv.Context.RegisterClassMap(pm);

                    // Read from CSV data
                    var datapoints = csv.GetRecords<DataPoint_Info>();
                    try
                    {
                        List<int> classNum = new List<int>();
                        foreach (DataPoint_Info point in datapoints)
                        {
                            // Create Datapoint Model and UI object
                            GameObject dataRep = Instantiate(DPModel, CubicCentre.transform);
                            //GameObject dataRep = instantDP.transform.GetChild(0).gameObject;

                            #region DP Material Setup
                            MeshCollider dpMeshCol = (MeshCollider)dataRep.AddComponent(typeof(MeshCollider));
                            dpMeshCol.convex = true;
                            dpMeshCol.isTrigger = true;

                            Material dataRepMat = dataRep.GetComponent<Renderer>().material;
                            //dataRepMat.SetFloat("_Mode", 3);
                            dataRepMat.SetOverrideTag("RenderType", "Transparent");
                            dataRepMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                            dataRepMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            dataRepMat.SetInt("_ZWrite", 0);
                            dataRepMat.DisableKeyword("_ALPHATEST_ON");
                            dataRepMat.EnableKeyword("_ALPHABLEND_ON");
                            dataRepMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            dataRepMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                            #endregion

                            #region Old DP UI Code
                            //GameObject dataContent = Instantiate(DatapointInfoPrefab, DataControlContent.transform);
                            //if (dataContent.name.Contains("X"))
                            //{
                            //    dataContent.name = dataContent.name.Replace("X", point.DataPoint_ID.ToString());
                            //}

                            // Rename child objects in UI
                            //foreach (Transform obj in dataContent.transform)
                            //{
                            //    if (obj.name.Contains("X"))
                            //    {
                            //        obj.name = obj.name.Replace("X", point.DataPoint_ID.ToString());
                            //    }
                            //}

                            // Edit Data Point ID and Class
                            //GameObject dcNameClass = dataContent.transform.GetChild(0).gameObject;
                            //TextMeshProUGUI dpIDCls = dcNameClass.GetComponent<TextMeshProUGUI>();
                            //dpIDCls.text = dpIDCls.text.Replace("X", point.DataPoint_ID.ToString()) + point.Class_ID.ToString();

                            // Add Axis Value Info
                            //dataContent.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{x_band}/{y_band}/{z_band} " +
                            //    $"Values: {point.X_Axis_Value}/{point.Y_Axis_Value}/{point.Z_Axis_Value}";

                            // Realign UI object to top middle
                            //topMiddle(dataContent);
                            #endregion

                            // Load headers from Header Toggles
                            List<string> headers = new List<string>();

                            foreach (UniUI.Toggle headToggle in HeaderCheckboxContent.GetComponentsInChildren<UniUI.Toggle>())
                            {
                                if (headToggle.isOn) headers.Add(headToggle.name);
                            }

                            if (headers.Count < 3)
                            {
                                MessageBox.Show("Not enough headers were selected in the checkbox section.", "Please Select Headers");
                                return;
                            }

                            // Add data point tracker to Model
                            dataRep.AddComponent<DataPoint_Model>().Create_Model(point, headers, GraphViewControl);
                            dataRep.GetComponent<DataPoint_Model>().LoadDatapoint += LoadDatapointInfo;
                            ///GameObject dcNameClass = new GameObject($"{dataContent.name} IDs");
                            ///dcNameClass.AddComponent<TextMeshProUGUI>();
                            ///dcNameClass.GetComponent<TextMeshProUGUI>().autoSizeTextContainer = true;
                            ///dcNameClass.GetComponent<TextMeshProUGUI>().fontSize = 32;
                            ///dcNameClass.GetComponent<TextMeshProUGUI>().enableWordWrapping = false;
                            ///dcNameClass.GetComponent<TextMeshProUGUI>().text = $"{dataContent.name} - Class {point.Class_ID}";
                            ///if (!classNum.Contains(point.Class_ID))
                            ///{
                            ///    classNum.Add(point.Class_ID);
                            ///}
                            ///dcNameClass.transform.SetParent(dataContent.transform);
                            ///
                            ///GameObject dcExtension = new GameObject($"{dataContent.name} Extended");
                            ///dcExtension.AddComponent<TextMeshProUGUI>();
                            ///dcExtension.GetComponent<TextMeshProUGUI>().autoSizeTextContainer = true;
                            ///dcExtension.GetComponent<TextMeshProUGUI>().fontSize = 24;
                            ///dcExtension.GetComponent<TextMeshProUGUI>().enableWordWrapping = false;
                            ///dcExtension.GetComponent<TextMeshProUGUI>().text = $"{x_band}/{y_band}/{z_band} Reflectances: " +
                            ///    $"{point.X_Value_Reflectance}/{point.Y_Value_Reflectance}/{point.Z_Value_Reflectance}";
                            ///
                            ///dcExtension.transform.SetParent(dataContent.transform);

                            // Add Datapoint Model to field and rescale the size
                            dataRep.transform.SetParent(CubicCentre.transform);
                            dataRep.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                            // ---------------------------------------------------------------------------------------------------
                            // When Global pallets are implemented, this needs to pass in a global pallet setting in the future
                            // ---------------------------------------------------------------------------------------------------
                            ColourPallet pallet = new ColourPallet();
                            if (point.Class_ID - 1 < pallet.Pallet.Length)
                            {
                                dataRep.GetComponent<Renderer>().material.color = pallet.Pallet[point.Class_ID - 1];
                                //dcNameClass.GetComponent<TextMeshProUGUI>().color = pallet.Pallet[point.Class_ID - 1];
                            }
                            else
                            {
                                dataRep.GetComponent<Renderer>().material.color = pallet.Pallet.Last();
                                //dcNameClass.GetComponent<TextMeshProUGUI>().color = pallet.Pallet.Last();
                            }

                            // Relocate datapoint to relational position
                            Vector3 pos = new Vector3(point.X_Axis_Value, point.Y_Axis_Value, point.Z_Axis_Value);
                            dataRep.transform.localPosition = pos;

                            // Note any new classes being loaded in from the dataset
                            if (!classNum.Contains(point.Class_ID))
                            {
                                classNum.Add(point.Class_ID);
                            }

                            string name = $"DP {point.DataPoint_ID}: CLS {point.Class_ID}";
                            dataRep.name = name;
                            //Destroy(instantDP);
                        }
                        if (classNum.Count > 0)
                        {
                            // ---------------------------------------------------------------------------------------------------
                            // This needs to pass in a global pallet setting in the future
                            // ---------------------------------------------------------------------------------------------------
                            ClassControlObj.UpdateClasses(classNum.Count, new ColourPallet(), GraphViewControl);
                        }
                    }
                    catch (TypeConverterException tce)
                    {
                        if (tce.MemberMapData.Index < 2)
                        {
                            MessageBox.Show("A data error has occurred! Please reconfirm that the datapoint and class ID" +
                                "headers selected relate to a column of integers in your datafile, and try again.", 
                                "Error Reading Selected Data and Class ID Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("A data error has occurred! Please ensure your data column bands are not integer " +
                                "values in your datafile and try again.", "Error Reading Selected X/Y/Z Axis Data", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        Swap_Data_Canvases();
                        return;
                    }

                }
            }

            // Load datapoint values with filename recovered from cache object
            using (var reader = new StreamReader(DataLoadCanvas.transform.GetChild(1).name))
            {
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    List<string> headers = new List<string>();

                    foreach (UniUI.Toggle headToggle in HeaderCheckboxContent.GetComponentsInChildren<UniUI.Toggle>())
                    {
                        if (headToggle.isOn) headers.Add(headToggle.name);
                    }

                    Point_Data test = new Point_Data();
                    while (headers.Count < test.Data_List.Count)
                    {
                        headers.Add(string.Empty);
                    }
                    DataMap dm = new DataMap(headers);
                    List<MemberMap> l = dm.MemberMaps.ToList();
                    csv.Context.RegisterClassMap(dm);

                    var datas = csv.GetRecords<Point_Data>();
                    List<Point_Data> p = datas.ToList();

                    foreach (Transform dp in CubicCentre.transform)
                    {                            
                        DataPoint_Model dpm;
                        if (dp.TryGetComponent(out dpm))
                        {
                            dpm.Band_Values = p[dpm.Point_ID - 1];
                        }
                    }
                }
            }

            // Load headers into axes when data all loaded correctly
            X_Text.text = $"X Axis: {x_band}";
            Y_Text.text = $"Y Axis: {y_band}";
            Z_Text.text = $"Z Axis: {z_band}";

            DataVisualisationButton.enabled = true;
            CSVDataLoaded = false;
            ClassControlButton.enabled = true;
            Swap_Data_Canvases();
            ScrollToTop();
        }
        else
        {

        }
    }
    #endregion

    #region Datapoint UI Event Handler
    internal void LoadDatapointInfo(DataPoint_Model.DPM_Info DPM)
    {
        DataInfoTitle.text = $"<u>Data Point ID:</u> {DPM.Point_ID}";
        DataClassInfo.text = $"<u>Class:</u> {DPM.Class_ID}";

        LineGraph.ShowDataGraph(DPM);
    }
    #endregion

    #region Scroll Control Methods
    public void ScrollToTop()
    {
        Canvas.ForceUpdateCanvases();

        DataControlContent.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        DataControlContent.GetComponent<ContentSizeFitter>().SetLayoutVertical();

        DataScrollView.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        DataScrollView.content.GetComponent<ContentSizeFitter>().SetLayoutVertical();

        DataScrollView.normalizedPosition = new Vector2(0, 1);
    }
    #endregion

    #region Data Reload Methods
    public void Reload_Data()
    {
        TMP_Dropdown[] Data_Dropdowns = DataLoadCanvas.transform.GetChild(0).GetComponentsInChildren<TMP_Dropdown>();
        bool headers_loaded = true;

        foreach (TMP_Dropdown dd in Data_Dropdowns)
        {
            headers_loaded &= !string.IsNullOrEmpty(dd.options[dd.value].text);
        }

        if (headers_loaded)
        {
            // Map variable setup
            string x_band = Data_Dropdowns[2].options[Data_Dropdowns[2].value].text;
            string y_band = Data_Dropdowns[3].options[Data_Dropdowns[3].value].text;
            string z_band = Data_Dropdowns[4].options[Data_Dropdowns[4].value].text;

            try
            {
                foreach (Transform child in CubicCentre.transform)
                {
                    DataPoint_Model dataRep;
                    if (child.TryGetComponent<DataPoint_Model>(out dataRep))
                    {
                        dataRep.Update_Axis_Value("x", x_band);
                        dataRep.Update_Axis_Value("y", y_band);
                        dataRep.Update_Axis_Value("z", z_band);

                        if (GraphViewControl.IsGrayscale)
                        {
                            dataRep.Swap_Grayscale_Mode(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Reloading Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load headers into axes when data all loaded correctly
            X_Text.text = $"X Axis: {x_band}";
            Y_Text.text = $"Y Axis: {y_band}";
            Z_Text.text = $"Z Axis: {z_band}";

            Swap_Data_Canvases();
        }
        else
        {

        }
    }
    #endregion

    #region Swap Canvas Methods
    public void Swap_Data_Canvases()
    {
        if (CubicCentre.transform.childCount > 1)
        {
            DataControlCanvas.enabled = !DataControlCanvas.enabled;
            DataLoadCanvas.enabled = !DataLoadCanvas.enabled;
        }
    }

    public void Swap_To_Canvas(Canvas canv)
    {
        foreach (Canvas enabledCanv in MainCanvas.GetComponentsInChildren<Canvas>())
        {
            if (enabledCanv != MainCanvas)
            {
                enabledCanv.enabled = false;
            }
        }
        if (canv == DataLoadCanvas && !CSVDataLoaded)
        {
            try
            {
                HeaderCheckboxContent.transform.parent.parent.gameObject.SetActive(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An exception has occurred: {ex.Message} If this crashes the program, please alert developers.", "Error Swapping to Data Load", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        else
        {
            try
            {
                HeaderCheckboxContent.transform.parent.parent.gameObject.SetActive(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An exception has occurred: {ex.Message} If this crashes the program, please alert developers.", "Error Swapping to Data Load", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        canv.enabled = true;
    }
    #endregion

    #endregion

    #region GameObject Anchor Presets
    //------------Top-------------------
    void topLeft(GameObject uiObject)
    {
        RectTransform uitransform = uiObject.GetComponent<RectTransform>();

        uitransform.anchorMin = new Vector2(0, 1);
        uitransform.anchorMax = new Vector2(0, 1);
        uitransform.pivot = new Vector2(0, 1);
    }

    void topMiddle(GameObject uiObject)
    {
        RectTransform uitransform = uiObject.GetComponent<RectTransform>();

        uitransform.anchorMin = new Vector2(0.5f, 1);
        uitransform.anchorMax = new Vector2(0.5f, 1);
        uitransform.pivot = new Vector2(0.5f, 1);
    }


    void topRight(GameObject uiObject)
    {
        RectTransform uitransform = uiObject.GetComponent<RectTransform>();

        uitransform.anchorMin = new Vector2(1, 1);
        uitransform.anchorMax = new Vector2(1, 1);
        uitransform.pivot = new Vector2(1, 1);
    }

    //------------Middle-------------------
    void middleLeft(GameObject uiObject)
    {
        RectTransform uitransform = uiObject.GetComponent<RectTransform>();

        uitransform.anchorMin = new Vector2(0, 0.5f);
        uitransform.anchorMax = new Vector2(0, 0.5f);
        uitransform.pivot = new Vector2(0, 0.5f);
    }

    void middle(GameObject uiObject)
    {
        RectTransform uitransform = uiObject.GetComponent<RectTransform>();

        uitransform.anchorMin = new Vector2(0.5f, 0.5f);
        uitransform.anchorMax = new Vector2(0.5f, 0.5f);
        uitransform.pivot = new Vector2(0.5f, 0.5f);
    }

    void middleRight(GameObject uiObject)
    {
        RectTransform uitransform = uiObject.GetComponent<RectTransform>();

        uitransform.anchorMin = new Vector2(1, 0.5f);
        uitransform.anchorMax = new Vector2(1, 0.5f);
        uitransform.pivot = new Vector2(1, 0.5f);
    }

    //------------Bottom-------------------
    void bottomLeft(GameObject uiObject)
    {
        RectTransform uitransform = uiObject.GetComponent<RectTransform>();

        uitransform.anchorMin = new Vector2(0, 0);
        uitransform.anchorMax = new Vector2(0, 0);
        uitransform.pivot = new Vector2(0, 0);
    }

    void bottomMiddle(GameObject uiObject)
    {
        RectTransform uitransform = uiObject.GetComponent<RectTransform>();

        uitransform.anchorMin = new Vector2(0.5f, 0);
        uitransform.anchorMax = new Vector2(0.5f, 0);
        uitransform.pivot = new Vector2(0.5f, 0);
    }

    void bottomRight(GameObject uiObject)
    {
        RectTransform uitransform = uiObject.GetComponent<RectTransform>();

        uitransform.anchorMin = new Vector2(1, 0);
        uitransform.anchorMax = new Vector2(1, 0);
        uitransform.pivot = new Vector2(1, 0);
    }
    #endregion
}