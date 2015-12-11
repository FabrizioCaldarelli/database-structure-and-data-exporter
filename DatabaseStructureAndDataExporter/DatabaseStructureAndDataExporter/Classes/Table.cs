using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseStructureAndData.Classes
{
    public class Table
    {
        // Properties
        public String Name { get; private set;  }
        public Dictionary<String, Column> Columns { get; private set; }
        public List<RowData> Data { get; private set; }

        private Table(String tableName)
        {
            this.Name = tableName;
            this.Columns = null;
            this.Data = null;
        }

        public String exportDataToJson()
        {
            List<Dictionary<String, Object>> lst = new List<Dictionary<string, object>>();

            foreach(RowData rd in Data)
            {
                Dictionary<String, Object> d = new Dictionary<string, object>();
                foreach (RowDataItem rdi in rd.Items)
                {
                    d[rdi.Col.Name] = rdi.Val;
                }
                lst.Add(d);
            }

            return JsonConvert.SerializeObject(lst, Formatting.Indented);
        }

        public static String exportStructurCSharpBaseModel(String modelNamespace)
        {
            String strBase = String.Format(
                "using System;\n" +
                "using System.Collections.Generic;\n" +
                "using System.ComponentModel;\n" +
                "using System.Linq;\n" +
                "using System.Text;\n" +
                "using System.Threading.Tasks;\n" +
                "\n" +
                "namespace {0}\n" +
                "{{\n" +
                "   public class BaseModel : INotifyPropertyChanged\n" +
                "   {{\n" +
                "       public event PropertyChangedEventHandler PropertyChanged;\n" +
                "       protected void NotifyPropertyChanged(String propertyName)\n" +
                "       {{\n" +
                "           PropertyChangedEventHandler handler = PropertyChanged;\n" +
                "           if (null != handler) handler(this, new PropertyChangedEventArgs(propertyName));\n" +
                "       }}\n" +
                "   }}\n" +
                "}}\n"
                , modelNamespace);

            return strBase;
        }

        public String exportStructureToCSharpModel(String modelNamespace)
        {
            String strModel = null;

                List<String> righeMember = new List<string>();
                List<String> righeProperty = new List<string>();
                foreach (KeyValuePair<String, Column> kvp in Columns)
                {
                    righeMember.Add(kvp.Value.exportToCSharpMember());
                    righeProperty.Add(kvp.Value.exportToCSharpProperty());
                }

                String strRigheMember = String.Join("\n\t\t\t", righeMember);
                String strRigheProperty = String.Join("\n\t\t\t", righeProperty);

                strModel = String.Format(
                    "using System;\n" +
                    "using System.Collections.Generic;\n" +
                    "using System.Linq;\n" +
                    "using System.Text;\n" +
                    "using System.Threading.Tasks;\n" +
                    "\n" +
                    "namespace {0}\n" +
                    "{{\n" +
                    "   public class {1} : BaseModel\n" +
                    "   {{\n" +
                    "       // Members\n" +
                    "       {2}\n" +
                    "\n" +
                    "       // Properties\n" +
                    "       {3}\n" +
                    "   }}\n" +
                    "}}\n"
                    , modelNamespace, Name, strRigheMember, strRigheProperty);

            return strModel;
        }


        private void readStructure(SqlConnection connection)
        {
            this.Columns = new Dictionary<String,Column>();

            SqlCommand cmd = new SqlCommand(String.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'", Name), connection);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (reader != null)
                {
                    String cn = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
                    String dt = reader.GetString(reader.GetOrdinal("DATA_TYPE"));

                    Column c = new Column(cn, dt);
                    this.Columns[cn] = c;
                }
            }
            reader.Close();
            
        }

        private void readData(SqlConnection connection)
        {
            this.Data = new List<RowData>();

            SqlCommand cmd = new SqlCommand(String.Format("SELECT * FROM {0}", Name), connection);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (reader != null)
                {
                    RowData rd = new RowData();
                    for (int k = 0; k < reader.FieldCount; k++)
                    {
                        String cn = reader.GetName(k);
                        Object v = reader.GetValue(k);
                        rd.Items.Add(new RowDataItem(this.Columns[cn], v));
                    }
                    this.Data.Add(rd);
                }
            }
            reader.Close();

        }

        public static List<Table> listTablesWithStructureAndData(DatabaseParser databaseParser)
        {
            List<Table> lstTable = new List<Table>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM information_schema.tables", databaseParser.Connection);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (reader != null)
                {
                    String tc = reader.GetString(reader.GetOrdinal("TABLE_CATALOG"));
                    String tn = reader.GetString(reader.GetOrdinal("TABLE_NAME"));
                    String tt = reader.GetString(reader.GetOrdinal("TABLE_TYPE"));
                    if (
                        (tc.Equals(databaseParser.Database))
                        &&
                        (tt.Equals("BASE TABLE"))
                    )
                    {
                        if (tn.Equals("sysdiagrams")) continue;
                        lstTable.Add(new Table(tn));
                    }
                }
            }
            reader.Close();

            foreach(Table t in lstTable)
            {
                t.readStructure(databaseParser.Connection);
                t.readData(databaseParser.Connection);
            }

            return lstTable;
        }


    }
}
