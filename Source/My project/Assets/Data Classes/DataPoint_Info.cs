using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Data_Classes
{
    public class DataPoint_Info
    {
        public int DataPoint_ID { get; set; }
        public int Class_ID { get; set; }
        public float X_Axis_Value { get; set; }
        public float Y_Axis_Value { get; set; }
        public float Z_Axis_Value { get; set; }
    }

    public sealed class PointMap : ClassMap<DataPoint_Info>
    {
        public PointMap(string fid, string class_id, string x_name, string y_name, string z_name)
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.DataPoint_ID).Name(fid);
            Map(m => m.Class_ID).Name(class_id);
            Map(m => m.X_Axis_Value).Name(x_name);
            Map(m => m.Y_Axis_Value).Name(y_name);
            Map(m => m.Z_Axis_Value).Name(z_name);
        }
    }
}
