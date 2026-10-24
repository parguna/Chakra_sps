using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DeepIdentity.DataAccess
{
    public class CheckSqlLiteral
    {
        public static void SafelyEscape(DataSet _dataSet)
        {
            foreach(DataTable dtable in _dataSet.Tables)
            {
                foreach (DataRow dRow in dtable.Rows)
                {
                    for (int index = 0; index < dtable.Columns.Count; index++)
                    {
                        if (dRow[index].GetType().Name == "string")
                        {

                        }
                    }
                }
            }
        }
    }
}
