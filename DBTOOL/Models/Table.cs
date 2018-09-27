using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBTOOL.Models
{
    class Table
    {
        private String tableName;

        public String TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        private String tableMessage;

        public String TableMessage
        {
            get { return tableMessage; }
            set { tableMessage = value; }
        }

    }
}
