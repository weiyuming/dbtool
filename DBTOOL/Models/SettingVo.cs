using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBTOOL.Models
{
    class SettingVo
    {
        private string tableSavePath;

        public string TableSavePath
        {
            get { return tableSavePath; }
            set { tableSavePath = value; }
        }


        private String userName;
        public String UserName
        {
            get { return userName; }
            set { userName = value; }
        }



        private String hour;

        public String Hour
        {
            get { return hour; }
            set { hour = value; }
        }


        private String min;

        public String Min
        {
            get { return min; }
            set { min = value; }
        }

        private int maxRow;

        public int MaxRow
        {
            get { return maxRow; }
            set { maxRow = value; }
        }

        

    }
}
