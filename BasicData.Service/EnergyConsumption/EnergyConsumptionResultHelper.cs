using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace BasicData.Service.EnergyConsumption
{
    public class EnergyConsumptionResultHelper
    {
        //GetNoRow

        public static int GetNoRow(DataTable _table, string _fieldName, string _str)
        {
            string sql = string.Format("{0}='{1}'", _fieldName, _str);
            DataRow[] rows = _table.Select(sql);
            int result = rows.Count() > 0 ? _table.Rows.IndexOf(rows[0]) : -1;
            return result;
        }
        /// <summary>
        /// 按照sortStr在totalStr上合计。
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sortStr"></param>
        /// <param name="totalStr"></param>
        /// <returns></returns>
        public static DataTable MyTotalOn(DataTable table, string sortStr, string totalStr)//字符串sortStr,totalStr应用英文逗号分隔
        {
            sortStr = sortStr.Replace(" ", "");
            totalStr = totalStr.Replace(" ","");
            string[] SortArray = sortStr.Split(',');
            string[] TotalArray = totalStr.Split(',');
            DataTable Table = SortTable(table, SortArray);
            int n = SortArray.Length;//需要排序列的个数
            int m = TotalArray.Length;//需要合计列的个数
            int RowsNum = Table.Rows.Count;
            string Compare = "shshjs";
            DataTable[] T = new DataTable[n];
            for (int a = 0; a < n; a++)
            {
                T[a] = Table.Clone();
            }
            for (int t = 0; t < n; t++)
            {
                string str;
                int j = 0;
                int k = 0;
                int p = 0;
                foreach (DataRow dr1 in Table.Rows)
                {
                    p++;
                    str = dr1[SortArray[n - 1 - t]].ToString().Trim();//去掉前后的空格
                    //string str44 = dr1[1].ToString();
                    if (str != Compare)//不同的情况下
                    {
                        Compare = str;
                        DataRow row = dr1;
                        k++;//新生产表的行数
                        j = k - 1;//新表最后一行的索引号
                        for (int i = 0; i < t; i++)
                        {
                            //row[n-i-1] = null; 
                            row[SortArray[n - 1 - i]] = null;
                        }
                        T[t].Rows.Add(row.ItemArray);
                    }
                    else//相同的情况下
                    {

                        DataRow row = T[t].NewRow();
                        for (int i = 0; i < m; i++)
                        {
                            row = T[t].Rows[k - 1];//T1的最后一行
                            if (T[t].Rows[k - 1][TotalArray[i]] is DBNull)//数据库中的空在程序中为DBNull
                            {
                                T[t].Rows[k - 1][TotalArray[i]] = 0;
                            }
                            if (dr1[TotalArray[i]] is DBNull)
                            {
                                dr1[TotalArray[i]] = 0;
                            }
                            T[t].Rows[k - 1][TotalArray[i]] = Convert.ToDouble(T[t].Rows[k - 1][TotalArray[i]]) + Convert.ToDouble(dr1[TotalArray[i]]);
                        }
                    }
                }

            }
            for (int i = 0; i < n - 1; i++)
            {
                //T[n-1].Merge(T[n-i-2]);
                T[0].Merge(T[i + 1]);
            }
            //return T[0];
            //MyTableSort mts = new MyTableSort(T[0]);
            return SortTable(T[0], SortArray);
        }
        /// <summary>
        /// 以默认升序方式排列
        /// </summary>
        /// <param name="OrderBy"></param>
        /// <returns></returns>
        public static DataTable SortTable(DataTable _oldTable, params string[] OrderBy)
        {
            DataTable oldTable = _oldTable;
            DataView view = new DataView(oldTable);
            StringBuilder orderByStr = new StringBuilder();
            int i = 0;
            int n = OrderBy.Length;
            foreach (string str in OrderBy)
            {
                i++;
                orderByStr.Append(str);
                if (i < n)
                {
                    orderByStr.Append(",");
                }
            }
            string strn = orderByStr.ToString();
            view.Sort = strn;
            DataTable NewTable = view.ToTable();
            return NewTable;

        }
        /// <summary>
        /// 以可选的升序或降序方式排列
        /// </summary>
        /// <param name="px"></param>
        /// <param name="OrderBy"></param>
        /// <returns></returns>
        public static DataTable SortTable(DataTable oldTable, string px, params string[] OrderBy)
        {
            DataView view = new DataView(oldTable);
            StringBuilder orderByStr = new StringBuilder();
            int i = 0;
            int n = OrderBy.Length;
            // orderByStr.Append("State "+px+",");
            foreach (string str in OrderBy)
            {
                i++;
                orderByStr.Append(str);
                if (i < n)
                {
                    orderByStr.Append(",");
                }
            }
            orderByStr.Append(" " + px);
            string strn = orderByStr.ToString();
            view.Sort = strn;
            DataTable NewTable = view.ToTable();
            return NewTable;

        }
    }
}
