using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Data_Classes
{
    public class DataPoint_Model : MonoBehaviour
    {
        #region Variables
        public int Point_ID;
        public int Class_ID;
        public float X_Axis_Value;
        public float Y_Axis_Value;
        public float Z_Axis_Value;

        public List<string> Band_Names;
        public Point_Data Band_Values;
                
        private GraphViewControlScript GraphCO;

        #endregion

        /// <summary>
        /// Public Class used to send Data Point values to UI.
        /// </summary>
        public class DPM_Info
        {
            public int Point_ID;
            public int Class_ID;
            public float X_Axis_Value;
            public float Y_Axis_Value;
            public float Z_Axis_Value;

            public List<string> Band_Names;
            public Point_Data Band_Values;
            public List<int> Band_Order;

            public DPM_Info(DataPoint_Model dataPoint_Model)
            {
                Point_ID = dataPoint_Model.Point_ID;
                Class_ID = dataPoint_Model.Class_ID;
                X_Axis_Value = dataPoint_Model.X_Axis_Value;
                Y_Axis_Value = dataPoint_Model.Y_Axis_Value;
                Z_Axis_Value = dataPoint_Model.Z_Axis_Value;
                Band_Names = dataPoint_Model.Band_Names;
                Band_Values = dataPoint_Model.Band_Values;
                Band_Order = new List<int> { 3, 0, 4, 5, 1, 6, 7, 8, 2, 9 };
            }
        }

        internal delegate void LoadDataIntoUIEvent(DPM_Info DPM);
        internal event LoadDataIntoUIEvent LoadDatapoint;

        void Start()
        {

        }

        public void Create_Model(DataPoint_Info dp, List<string> band_names, GraphViewControlScript GCS)
        {
            Point_ID = dp.DataPoint_ID;
            Class_ID = dp.Class_ID;
            X_Axis_Value = dp.X_Axis_Value;
            Y_Axis_Value = dp.Y_Axis_Value;
            Z_Axis_Value = dp.Z_Axis_Value;
            Band_Names = band_names;
            GraphCO = GCS;

            if (GraphCO != null)
            {
                GraphCO.GrayscaleEvent += Swap_Grayscale_Mode;
            }
        }

        public void Update_Axis_Value(string dimension, string new_band)
        {
            if (Band_Names.Contains(new_band))
            {
                switch (dimension.ToUpper())
                {
                    case "X":
                        X_Axis_Value = Band_Values.Data_List[Band_Names.IndexOf(new_band)];
                        break;
                    case "Y":
                        Y_Axis_Value = Band_Values.Data_List[Band_Names.IndexOf(new_band)];
                        break;
                    case "Z":
                        Z_Axis_Value = Band_Values.Data_List[Band_Names.IndexOf(new_band)];
                        break;
                    default:
                        throw new Exception($"Dimension \"{dimension}\" is not recognised.");
                }

                gameObject.transform.localPosition = new Vector3(X_Axis_Value, Y_Axis_Value, Z_Axis_Value);
            }
            else
            {
                throw new Exception($"Specified Band \"{new_band}\" not found for this data point.");
            }
        }

        public void Swap_Grayscale_Mode(bool GrayscaleActive)
        {
            if (GrayscaleActive)
            {
                if (TryGetComponent<Renderer>(out var rend))
                {
                    Material mat = rend.material;
                    if (mat != null)
                    {
                        Color grayCol = new Color(Math.Abs(X_Axis_Value), Math.Abs(Y_Axis_Value), Math.Abs(Z_Axis_Value));
                        mat.color = grayCol;
                    }
                }
            }
            else
            {

            }
        }

        private void OnMouseOver()
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                transform.Rotate(transform.up);
            }
        }

        private void OnMouseUpAsButton()
        {
            DPM_Info thisCopy = new DPM_Info(this);
            LoadDatapoint?.Invoke(thisCopy);
        }

        private void OnMouseExit()
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                transform.eulerAngles = Vector3.zero;
            }
        }
    }

    public class Point_Data
    {
        #region Properties
        public int fid { get; set; }
        public float Value_1 { get; set; }
        public float Value_2 { get; set; }
        public float Value_3 { get; set; }
        public float Value_4 { get; set; }
        public float Value_5 { get; set; }
        public float Value_6 { get; set; }
        public float Value_7 { get; set; }
        public float Value_8 { get; set; }
        public float Value_9 { get; set; }
        public float Value_10 { get; set; }
        public float Value_11 { get; set; }
        public float Value_12 { get; set; }
        public float Value_13 { get; set; }

        public List<float> Data_List
        {
            get
            {
                List<float> l = new List<float>
                {
                    Value_1,
                    Value_2,
                    Value_3,
                    Value_4,
                    Value_5,
                    Value_6,
                    Value_7,
                    Value_8,
                    Value_9,
                    Value_10,
                    Value_11,
                    Value_12,
                    Value_13
                };

                return l;
            }
        }
        #endregion

        #region Constructors (Recursive)
        public Point_Data()
        {

        }

        public Point_Data(float ref_1, float ref_2, float ref_3, float ref_4, float ref_5, float ref_6, float ref_7, float ref_8,
            float ref_9, float ref_10, float ref_11, float ref_12, float ref_13) :
            this(ref_1, ref_2, ref_3, ref_4, ref_5, ref_6, ref_7, ref_8, ref_9, ref_10, ref_11, ref_12)
        {
            Value_13 = ref_13;
        }

        public Point_Data(float ref_1, float ref_2, float ref_3, float ref_4, float ref_5, float ref_6, float ref_7, float ref_8,
            float ref_9, float ref_10, float ref_11, float ref_12) :
            this(ref_1, ref_2, ref_3, ref_4, ref_5, ref_6, ref_7, ref_8, ref_9, ref_10, ref_11)
        {
            Value_12 = ref_12;
        }

        public Point_Data(float ref_1, float ref_2, float ref_3, float ref_4, float ref_5, float ref_6, float ref_7, float ref_8,
            float ref_9, float ref_10, float ref_11) :
            this(ref_1, ref_2, ref_3, ref_4, ref_5, ref_6, ref_7, ref_8, ref_9, ref_10)
        {
            Value_11 = ref_11;
        }

        public Point_Data(float ref_1, float ref_2, float ref_3, float ref_4, float ref_5, float ref_6, float ref_7, float ref_8,
            float ref_9, float ref_10) : this(ref_1, ref_2, ref_3, ref_4, ref_5, ref_6, ref_7, ref_8, ref_9)
        {
            Value_10 = ref_10;
        }

        public Point_Data(float ref_1, float ref_2, float ref_3, float ref_4, float ref_5, float ref_6, float ref_7, float ref_8,
            float ref_9) : this(ref_1, ref_2, ref_3, ref_4, ref_5, ref_6, ref_7, ref_8)
        {
            Value_9 = ref_9;
        }

        public Point_Data(float ref_1, float ref_2, float ref_3, float ref_4, float ref_5, float ref_6, float ref_7,
            float ref_8) : this(ref_1, ref_2, ref_3, ref_4, ref_5, ref_6, ref_7)
        {
            Value_8 = ref_8;
        }

        public Point_Data(float ref_1, float ref_2, float ref_3, float ref_4, float ref_5, float ref_6, float ref_7) :
            this(ref_1, ref_2, ref_3, ref_4, ref_5, ref_6)
        {
            Value_7 = ref_7;
        }

        public Point_Data(float ref_1, float ref_2, float ref_3, float ref_4, float ref_5, float ref_6) :
            this(ref_1, ref_2, ref_3, ref_4, ref_5)
        {
            Value_6 = ref_6;
        }

        public Point_Data(float ref_1, float ref_2, float ref_3, float ref_4, float ref_5) : this(ref_1, ref_2, ref_3, ref_4)
        {
            Value_5 = ref_5;
        }

        public Point_Data(float ref_1, float ref_2, float ref_3, float ref_4) : this(ref_1, ref_2, ref_3)
        {
            Value_4 = ref_4;
        }

        public Point_Data(float ref_1, float ref_2, float ref_3) : this(ref_1, ref_2)
        {
            Value_3 = ref_3;
        }

        public Point_Data(float ref_1, float ref_2)
        {
            Value_2 = ref_2;
            Value_1 = ref_1;
        }
        #endregion

        public sealed class DataMap : ClassMap<Point_Data>
        {
            #region Constructors (Recursive)
            private DataMap(string name_1, string name_2, string name_3, string name_4, string name_5, string name_6, string name_7, string name_8,
                string name_9, string name_10, string name_11, string name_12, string name_13) :
                this(name_1, name_2, name_3, name_4, name_5, name_6, name_7, name_8, name_9, name_10, name_11, name_12)
            {
                if (!string.IsNullOrEmpty(name_13))
                {
                    Map(m => m.Value_13).Name(name_13);
                }
                else
                {
                    Map(m => m.Value_13).Ignore(true);
                }
            }

            private DataMap(string name_1, string name_2, string name_3, string name_4, string name_5, string name_6, string name_7, string name_8,
                string name_9, string name_10, string name_11, string name_12) :
                this(name_1, name_2, name_3, name_4, name_5, name_6, name_7, name_8, name_9, name_10, name_11)
            {
                if (!string.IsNullOrEmpty(name_12))
                {
                    Map(m => m.Value_12).Name(name_12);
                }
                else
                {
                    Map(m => m.Value_12).Ignore(true);
                }
            }

            private DataMap(string name_1, string name_2, string name_3, string name_4, string name_5, string name_6, string name_7, string name_8,
                string name_9, string name_10, string name_11) :
                this(name_1, name_2, name_3, name_4, name_5, name_6, name_7, name_8, name_9, name_10)
            {
                if (!string.IsNullOrEmpty(name_11))
                {
                    Map(m => m.Value_11).Name(name_11);
                }
                else
                {
                    Map(m => m.Value_11).Ignore(true);
                }
            }

            private DataMap(string name_1, string name_2, string name_3, string name_4, string name_5, string name_6, string name_7, string name_8,
                string name_9, string name_10) : this(name_1, name_2, name_3, name_4, name_5, name_6, name_7, name_8, name_9)
            {
                if (!string.IsNullOrEmpty(name_10))
                {
                    Map(m => m.Value_10).Name(name_10);
                }
                else
                {
                    Map(m => m.Value_10).Ignore(true);
                }
            }

            private DataMap(string name_1, string name_2, string name_3, string name_4, string name_5, string name_6, string name_7, string name_8,
                string name_9) : this(name_1, name_2, name_3, name_4, name_5, name_6, name_7, name_8)
            {
                if (!string.IsNullOrEmpty(name_9))
                {
                    Map(m => m.Value_9).Name(name_9);
                }
                else
                {
                    Map(m => m.Value_9).Ignore(true);
                }
            }

            private DataMap(string name_1, string name_2, string name_3, string name_4, string name_5, string name_6, string name_7,
                string name_8) : this(name_1, name_2, name_3, name_4, name_5, name_6, name_7)
            {
                if (!string.IsNullOrEmpty(name_8))
                {
                    Map(m => m.Value_8).Name(name_8);
                }
                else
                {
                    Map(m => m.Value_8).Ignore(true);
                }
            }

            private DataMap(string name_1, string name_2, string name_3, string name_4, string name_5, string name_6, string name_7) :
                this(name_1, name_2, name_3, name_4, name_5, name_6)
            {
                if (!string.IsNullOrEmpty(name_7))
                {
                    Map(m => m.Value_7).Name(name_7);
                }
                else
                {
                    Map(m => m.Value_7).Ignore(true);
                }
            }

            private DataMap(string name_1, string name_2, string name_3, string name_4, string name_5, string name_6) :
                this(name_1, name_2, name_3, name_4, name_5)
            {
                if (!string.IsNullOrEmpty(name_6))
                {
                    Map(m => m.Value_6).Name(name_6);
                }
                else
                {
                    Map(m => m.Value_6).Ignore(true);
                }
            }

            private DataMap(string name_1, string name_2, string name_3, string name_4, string name_5) : this(name_1, name_2, name_3, name_4)
            {
                if (!string.IsNullOrEmpty(name_5))
                {
                    Map(m => m.Value_5).Name(name_5);
                }
                else
                {
                    Map(m => m.Value_5).Ignore(true);
                }
            }

            private DataMap(string name_1, string name_2, string name_3, string name_4) : this(name_1, name_2, name_3)
            {
                if (!string.IsNullOrEmpty(name_4))
                {
                    Map(m => m.Value_4).Name(name_4);
                }
                else
                {
                    Map(m => m.Value_4).Ignore(true);
                }
            }

            private DataMap(string name_1, string name_2, string name_3)
            {
                if (!string.IsNullOrEmpty(name_3))
                {
                    Map(m => m.Value_3).Name(name_3);
                }
                else
                {
                    Map(m => m.Value_3).Ignore(true);
                }

                if (!string.IsNullOrEmpty(name_2))
                {
                    Map(m => m.Value_2).Name(name_2);
                }
                else
                {
                    Map(m => m.Value_2).Ignore(true);
                }

                if (!string.IsNullOrEmpty(name_1))
                {
                    Map(m => m.Value_1).Name(name_1);
                }
                else
                {
                    Map(m => m.Value_1).Ignore(true);
                }
            }
            #endregion

            public DataMap(List<string> headers)
            {
                Point_Data test = new Point_Data();
                if (headers.Count == test.Data_List.Count)
                {
                    DataMap dm = new DataMap(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6],
                        headers[7], headers[8], headers[9], headers[10], headers[11], headers[12]);

                    Map(m => m.fid).Name("fid");

                    MemberMaps.AddRange(dm.MemberMaps);
                    ParameterMaps.AddRange(dm.ParameterMaps);
                    ReferenceMaps.AddRange(dm.ReferenceMaps);
                }
                else
                {
                    string heads = "";
                    foreach (string header in headers)
                    {
                        if (!string.IsNullOrEmpty(header))
                        {
                            heads += $"{header}, ";
                        }
                    }
                    heads = heads.Remove(heads.Length - 3, heads.Length - 1);   
                    throw new ArgumentException($"The headers were not loaded correctly. Headers were: {heads}");
                }
            }
        }
    }
}
