using BasicData.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BasicData.Service.AmmeterModifyCoefficient
{
    public class ModifyCoefficientService
    {
        public static DataTable ReferenceCoefficientCalculate(string organizationId,string startTime,string endTime)
        {
            DataTable source = CreatBaseTable(organizationId, startTime, endTime);
            foreach (DataRow dr in source.Rows)
            {
                SetChild(dr, source);
            }
            return source;
        }
        /// <summary>
        /// 获得基础数据
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private static DataTable CreatBaseTable(string organizationId, string startTime, string endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string meterDatabase;
            string sql = @"select B.MeterDatabase 
                                from system_Organization A,system_Database B
                                where A.DatabaseID=B.DatabaseID
                                and A.OrganizationID=@organizationId";
            SqlParameter parameter = new SqlParameter("organizationId", organizationId);
            DataTable t_table = dataFactory.Query(sql, parameter);
            if (t_table.Rows.Count == 0)
            {
                throw new Exception("没有找到对应的分厂数据库");
            }
            else
            {
                meterDatabase = t_table.Rows[0]["MeterDatabase"].ToString().Trim();
            }
            string myFieldSql = @"select A.LevelCode,A.OrganizationID, A.AmmeterNumber,A.Remarks,B.AmmeterName, B.ElectricRoom, B.CommunicationProtocol, B.AmmeterAddress,
                                    B.CommPort, B.CT,B.PT, B.PowerFieldNameSave, B.Status, C.ElectricRoomName
                                    from {0}.dbo.AmmeterModifyCoefficientReference A 
                                    left join {0}.dbo.AmmeterContrast B on (A.OrganizationID=B.OrganizationID and A.AmmeterNumber=B.AmmeterNumber)
                                    left join [{0}].[dbo].[ElectricRoomContrast] C on B.ElectricRoom=C.ElectricRoom
                                    where B.[EnabledFlag]=1 or B.[EnabledFlag] is null";
            DataTable fieldTable = dataFactory.Query(string.Format(myFieldSql, meterDatabase));
            DataColumn column = new DataColumn("Value", typeof(decimal));
            column.DefaultValue = 0;
            fieldTable.Columns.Add(column);
            //子节点电量和
            DataColumn columnChildrenValue = new DataColumn("ChildrenValue", typeof(decimal));
            columnChildrenValue.DefaultValue = 0;
            fieldTable.Columns.Add(columnChildrenValue);
            //差值
            DataColumn columnDValue = new DataColumn("DValue", typeof(decimal));
            columnDValue.DefaultValue = 0;
            fieldTable.Columns.Add(columnDValue);
            //系数
            DataColumn columnRatio = new DataColumn("Ratio",typeof(decimal));
            columnRatio.DefaultValue = 1;
            fieldTable.Columns.Add(columnRatio);
            StringBuilder fieldBuilder = new StringBuilder();
            foreach (DataRow dr in fieldTable.Rows)
            {
                string t_formula = dr["AmmeterNumber"].ToString().Trim();
                string resultFormula = t_formula;
                IEnumerable<string> variableList = Regex.Split(t_formula, @"[+\-*/()]+")
                                                .Except((IEnumerable<string>)new string[] { "" })
                                                .Select(p => p = Regex.Replace(p, @"^([0-9]+)([\.]([0-9]+))?$", ""))
                                                .Except((IEnumerable<string>)new string[] { "" });
                foreach (string item in variableList)
                {
                    resultFormula = resultFormula.Replace(item, item.Trim() + "Energy");
                }
                fieldBuilder.Append("SUM(");
                fieldBuilder.Append(resultFormula);
                fieldBuilder.Append(") as '");
                fieldBuilder.Append(dr["AmmeterNumber"].ToString().Trim());
                fieldBuilder.Append("',");
            }
            fieldBuilder.Remove(fieldBuilder.Length - 1, 1);
            string myValueSql = @"select {0}
                                    from {1}.dbo.HistoryAmmeterIncrement A
                                    where CONVERT(varchar(10),A.vDate,20)>=@startTime
                                    and CONVERT(varchar(10),A.vDate,20)<=@endTime";
            SqlParameter[] parameters = { new SqlParameter("startTime", startTime), new SqlParameter("endTime", endTime) };
            DataTable valueTable = dataFactory.Query(string.Format(myValueSql, fieldBuilder.ToString(), meterDatabase), parameters);
            if (valueTable.Rows.Count == 1)
            {
                foreach (DataRow dr in fieldTable.Rows)
                {
                    string ammeterNum = dr["AmmeterNumber"].ToString().Trim();
                    dr["Value"] = valueTable.Rows[0][ammeterNum];
                }
            }
            return fieldTable;
        }
        private static void SetChild(DataRow currentRow,DataTable source)
        {
            string levelCode = currentRow["LevelCode"].ToString().Trim();
            int parentlength = levelCode.Length;
            DataRow[] rows = source.Select("LevelCode like '" + levelCode + "%' and " + "len(LevelCode)=" + (parentlength + 2));
            if (rows.Count() == 0)
            {
                currentRow["ChildrenValue"] = currentRow["Value"];
                currentRow["DValue"] = 0;
                return;
            }
            else
            {             
                decimal parentValue = currentRow["Value"] is DBNull ? 0 : Convert.ToDecimal(currentRow["Value"]);
                
                decimal childValue = 0;
                //计算子节点电量和
                foreach (DataRow dr in rows)
                {
                    SetChild(dr, source);//递归
                    childValue = childValue + (dr["Value"] is DBNull ? 0 : Convert.ToDecimal(dr["Value"]));
                }
                //设置子节点比例系数
                foreach (DataRow dr in rows)
                {
                    dr["Ratio"] = childValue == 0 ? 1 : (parentValue - childValue) / childValue+1;
                }
                currentRow["ChildrenValue"] = childValue;
                currentRow["DValue"] = parentValue - childValue;
            }
        }
    }
}
