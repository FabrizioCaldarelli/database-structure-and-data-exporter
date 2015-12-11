using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseStructureAndData.Classes
{
    public class RowDataItem
    {
        public Column Col { get; private set; }
        public Object Val { get; private set; }

        public RowDataItem(Column col, Object val)
        {
            Col = col;
            Val = val;
        }
    }

    public class RowData
    {
        public List<RowDataItem> Items { get; private set; }        

        public RowData()
        {
            Items = new List<RowDataItem>();
        }
    }


}
