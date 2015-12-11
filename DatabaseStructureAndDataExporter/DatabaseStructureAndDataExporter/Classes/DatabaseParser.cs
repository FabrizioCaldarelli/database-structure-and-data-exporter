using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseStructureAndData.Classes
{
    public class DatabaseParser
    {
        // Properties
        public String Host { get; private set;  }
        public String Database { get; private set; }
        public SqlConnection Connection { get; private set; }

        public DatabaseParser(String host, String database)
        {
            this.Host = host;
            this.Database = database;
        }

        public List<Table> listTablesWithStructureAndData()
        {
            List<Table> lstTable = Table.listTablesWithStructureAndData(this);
            return lstTable;
        }

        public void connect()
        {
            this.Connection = new SqlConnection("server=" + Host + ";" +
                                                   "Trusted_Connection=yes;" +
                                                   "database=" + Database + "; " +
                                                   "connection timeout=30");
            Connection.Open();
        }

        public void disconnect()
        {
            Connection.Close();
            this.Connection = null;
        }
    }
}
