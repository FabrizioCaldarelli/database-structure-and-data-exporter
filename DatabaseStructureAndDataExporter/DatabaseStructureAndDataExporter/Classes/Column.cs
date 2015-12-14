using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseStructureAndData.Classes
{
    public class Column
    {
        public enum ColumnType
        {
            Integer,
            Float,
            String,
            Date,
            DateTime
        }

        // Properties
        public String Name { get; private set; }
        public ColumnType Type { get; private set; }

        public String TypeCShart
        {
            get
            {
                String s = null;
                switch (Type)
                {
                    case ColumnType.Integer:
                        s = "int";
                        break;
                    case ColumnType.Float:
                        s = "float";
                        break;
                    case ColumnType.String:
                        s = "string";
                        break;
                    case ColumnType.Date:
                        s = "DateTime";
                        break;
                    case ColumnType.DateTime:
                        s = "DateTime";
                        break;
                }
                return s;
            }
        }

        public Column(String name, String datatype)
        {
            Name = name;

            switch (datatype)
            {
                case "bit":
                case "smallint":
                case "tinyint":
                case "int":
                    Type = ColumnType.Integer;
                    break;
                case "float":
                case "decimal":
                case "numeric":
                    Type = ColumnType.Float;
                    break;
                case "varchar":
                case "nvarchar":
                case "text":
                    Type = ColumnType.String;
                    break;
                case "smalldatetime":
                case "datetime":
                case "date":
                    Type = ColumnType.DateTime;
                    break;
                case "money":
                    Type = ColumnType.String;
                    break;
                default:
                    throw new Exception("Column data type not found = " + datatype);
            }
        }

        

        public String exportToCSharpMember()
        {
            String s = String.Format("private {0} _{1};", TypeCShart, Name);
            return s;
        }
        public String exportToCSharpProperty()
        {
            String var = "_" + Name;
            String s = String.Format("public {0} {1} {{ get {{ return {2}; }} set {{ {3} = value; NotifyPropertyChanged(\"{4}\"); }} }}",
                TypeCShart, Name, var, var, Name);
            return s;
        }
    }
}
