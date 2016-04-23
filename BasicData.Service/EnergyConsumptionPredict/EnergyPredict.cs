using BasicData.Infrastructure.Configuration;
using BasicData.Service.BasicService;
using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicData.Service.EnergyConsumptionPredict
{
    /// <summary>
    /// 能源预测
    /// </summary>
    public class EnergyPredict
    {
        string v_string_error = "";
        private TZHelper tzHelper;
        private static string _connectionString;
        private ISqlServerDataFactory _dataFactory;
        private DataTable _dt;

        public DataTable DT
        {
            get { return _dt; }
            set { _dt = value; }
        }
        public string String_Error
        {
            get { return v_string_error; }
            set { v_string_error = value; }
        }
        public EnergyPredict()
        {
            _connectionString = ConnectionStringFactory.NXJCConnectionString;
            _dataFactory = new SqlServerDataFactory(_connectionString);
            tzHelper = new TZHelper(_connectionString);
        }
        /// <summary>
        /// 能源预测
        /// </summary>
        /// <param name="v_OrganizationID"></param>
        /// <param name="nowDate">"yyyy-MM"</param>
        /// <returns></returns>
        public DataTable Get_Forecast_ProductionLineEnergy(string v_OrganizationID,string nowDate)
        {
            //string nowDate = DateTime.Now.ToString("yyyy-MM");
            string v_begin = DateTime.Parse(nowDate).AddMonths(-12).ToString("yyyy-MM");
            string v_end = DateTime.Parse(nowDate).AddMonths(-1).ToString("yyyy-MM");

            DataTable temp = GetTempTable(v_OrganizationID);


            DataTable Result_Forecast_ProductionLine;//存放各生产线结果
            Result_Forecast_ProductionLine = _dataFactory.Query("select * from forecast_ProductionLineEnergyConsumptionTemplate where Type ='*********'");
            DataTable Result_Forecast_all = Result_Forecast_ProductionLine.Clone();//存放整体预测结果
            int No_Col = Result_Forecast_all.Columns.IndexOf("January");
            DataRow temp_dr;
            foreach (DataRow dr in temp.Rows)
            {
                if (dr["Type"].ToString().Trim() == "熟料" || dr["Type"].ToString().Trim() == "水泥磨")
                {
                    Forecast forecastTemp = new Forecast();
                    forecastTemp.Get_Forecast_ProductionLineEnergy(dr["OrganizationID"].ToString().Trim(), dr["Type"].ToString().Trim(), v_begin, v_end);
                    v_string_error = v_string_error + forecastTemp.String_Error;
                    DataTable temp_result = forecastTemp.DT;
                    foreach (DataRow dr_temp_result in temp_result.Rows)//遍历temp_result,替换OrganizationID，Name
                    {
                        dr_temp_result["OrganizationID"] = dr["OrganizationID"];
                        dr_temp_result["Name"] = dr["Name"];      //-- 生产机构名称
                    }
                    Result_Forecast_ProductionLine.Merge(temp_result);  //存放各生产线结果
                    if (dr["Type"].ToString().Trim() == "熟料")  //熟料
                    {
                        DataTable temp_ProductionLineInformation = _dataFactory.Query("select * from system_Organization where OrganizationID='" + dr["OrganizationID"].ToString() + "'");
                        decimal v_RawBatchClinker = temp_ProductionLineInformation.Rows.Count > 0 ? MyToDecimal(temp_ProductionLineInformation.Rows[0]["RawToClinkerCoff"]) : 0;
                        int No1_Row = ReportHelper.GetNoRow(temp_result, "QuotasID", "熟料产量");
                        temp_dr = temp_result.NewRow();
                        temp_dr["IdSort"] = 99;
                        temp_dr["QuotasID"] = "生料磨产量";
                        //if(No1_Row!=-1)
                        for (int i = No_Col; i < No_Col + 13; i++)
                        {  //1月到年度
                            temp_dr[i] =No1_Row==-1?0: MyToDecimal(temp_result.Rows[No1_Row][i]) * v_RawBatchClinker;
                        }
                        temp_result.Rows.Add(temp_dr);
                        int No2_Row = ReportHelper.GetNoRow(temp_result, "QuotasID", "生料磨电耗");//要注意
                        temp_dr = temp_result.NewRow();
                        temp_dr["IdSort"] = 99;
                        temp_dr["QuotasID"] = "生料磨电量";
                        for (int i = No_Col; i < No_Col + 13; i++)//1月到年度
                        {
                            temp_dr[i] =(No1_Row==-1||No2_Row==-1)?0: MyToDecimal(temp_result.Rows[No1_Row][i]) * v_RawBatchClinker * MyToDecimal(temp_result.Rows[No2_Row][i]);
                        }
                        temp_result.Rows.Add(temp_dr);
                        //...
                        //-------------临时添加使用-------
                        No2_Row = ReportHelper.GetNoRow(temp_result, "QuotasID", "熟料煤耗");//要注意
                        DataRow rowTest1 = temp_result.NewRow();
                        rowTest1["IdSort"] = 99;
                        rowTest1["QuotasID"] = "煤磨产量";//煤磨产量=熟料煤耗*熟料熟料产量
                        for (int i = No_Col; i < No_Col + 13; i++)//1月到年度
                        {
                            rowTest1[i] = (No1_Row == -1 || No2_Row == -1) ? 0 : MyToDecimal(temp_result.Rows[No1_Row][i]) * MyToDecimal(temp_result.Rows[No2_Row][i]) / 1000;
                        }
                        temp_result.Rows.Add(rowTest1);

                        No1_Row = ReportHelper.GetNoRow(temp_result, "QuotasID", "煤磨产量");
                        No2_Row = ReportHelper.GetNoRow(temp_result, "QuotasID", "煤磨电耗");
                        DataRow rowTest2 = temp_result.NewRow();
                        rowTest2["IdSort"] = 99;
                        rowTest2["QuotasID"] = "煤磨电量";
                        for (int i = No_Col; i < No_Col + 13; i++)//1月到年度
                        {
                            rowTest2[i] = (No1_Row == -1 || No2_Row == -1) ? 0 : MyToDecimal(temp_result.Rows[No1_Row][i]) * MyToDecimal(temp_result.Rows[No2_Row][i]);
                        }
                        temp_result.Rows.Add(rowTest2);
                    }
                        //-------------------------------
                        if (dr["Type"].ToString().Trim() == "水泥磨")  //水泥磨
                        {
                            int No1_Row = ReportHelper.GetNoRow(temp_result, "QuotasID", "水泥产量");
                            int No2_Row = ReportHelper.GetNoRow(temp_result, "QuotasID", "水泥磨电耗");
                            temp_dr = temp_result.NewRow();
                            temp_dr["IdSort"] = 99;
                            temp_dr["QuotasID"] = "水泥磨电量";
                            for (int i = No_Col; i < No_Col + 13; i++)
                            {  //1月到年度
                                temp_dr[i] = (No1_Row == -1 || No2_Row == -1) ? 0 : MyToDecimal(temp_result.Rows[No1_Row][i]) * MyToDecimal(temp_result.Rows[No2_Row][i]);
                            }
                            temp_result.Rows.Add(temp_dr);
                        }
                        Result_Forecast_all.Merge(temp_result);  // Result_Forecast_all存放整体预测结果
                    
                }//遍历Temp,获取各生产线预测
            }
            temp = ReportHelper.MyTotalOn(Result_Forecast_all, "QuotasID", "January,February,March,April,May,June,July,August,September,October,November,December,Totals");
            if (ReportHelper.GetNoRow(temp, "QuotasID", "熟料产量") != -1)  //存在熟料预测结果
            {//计算熟料电耗
                int No1_Row = ReportHelper.GetNoRow(temp, "QuotasID", "熟料产量");
                int No2_Row = ReportHelper.GetNoRow(temp, "QuotasID", "电力消耗量");
                int No3_Row = ReportHelper.GetNoRow(temp, "QuotasID", "熟料电耗");
                if (No3_Row != -1)
                {
                    for (int i = No_Col; i < No_Col + 13; i++)
                    {
                        temp.Rows[No3_Row][i] =(No1_Row == -1 || No2_Row == -1)|| MyToDecimal(temp.Rows[No1_Row][i]) == 0 ? 0 : MyToDecimal(temp.Rows[No2_Row][i]) / MyToDecimal(temp.Rows[No1_Row][i]);
                    }
                }
                //计算熟料煤耗
                No2_Row = ReportHelper.GetNoRow(temp, "QuotasID", "原煤消耗量");
                No3_Row = ReportHelper.GetNoRow(temp, "QuotasID", "熟料煤耗");
                if (No3_Row != -1)
                {
                    for (int i = No_Col; i < No_Col + 13; i++)
                    {
                        temp.Rows[No3_Row][i] =(No1_Row == -1 || No2_Row == -1)|| MyToDecimal(temp.Rows[No1_Row][i]) == 0 ? 0 : MyToDecimal(temp.Rows[No2_Row][i]) / MyToDecimal(temp.Rows[No1_Row][i]);
                    }
                }
                //计算生料磨电耗
                No1_Row = ReportHelper.GetNoRow(temp, "QuotasID", "生料磨产量");
                No2_Row = ReportHelper.GetNoRow(temp, "QuotasID", "生料磨电量");
                No3_Row = ReportHelper.GetNoRow(temp, "QuotasID", "生料磨电耗");
                if (No3_Row != -1)
                {
                    for (int i = No_Col; i < No_Col + 13; i++)
                    {
                        temp.Rows[No3_Row][i] = (No1_Row == -1 || No2_Row == -1)||MyToDecimal(temp.Rows[No1_Row][i]) == 0 ? 0 : MyToDecimal(temp.Rows[No2_Row][i]) / MyToDecimal(temp.Rows[No1_Row][i]);
                    }
                }
                //计算煤磨电耗
                No1_Row = ReportHelper.GetNoRow(temp, "QuotasID", "煤磨产量");
                No2_Row = ReportHelper.GetNoRow(temp, "QuotasID", "煤磨电量");
                No3_Row = ReportHelper.GetNoRow(temp, "QuotasID", "煤磨电耗");
                if (No3_Row != -1)
                {
                    for (int i = No_Col; i < No_Col + 13; i++)
                    {
                        temp.Rows[No3_Row][i] =(No1_Row == -1 || No2_Row == -1) || MyToDecimal(temp.Rows[No1_Row][i]) == 0? 0 : MyToDecimal(temp.Rows[No2_Row][i]) / MyToDecimal(temp.Rows[No1_Row][i]);
                    }
                }
            }//熟料
            if (ReportHelper.GetNoRow(temp, "QuotasID", "水泥产量") != -1)//存在水泥磨预测结果
            {
                //计算水泥电耗
                int No1_Row = ReportHelper.GetNoRow(temp, "QuotasID", "水泥产量");
                int No2_Row = ReportHelper.GetNoRow(temp, "QuotasID", "水泥电力消耗量");
                int No3_Row = ReportHelper.GetNoRow(temp, "QuotasID", "水泥电耗");
                if (No3_Row!=-1)
                {
                    for (int i = No_Col; i < No_Col + 13; i++)
                    {
                        temp.Rows[No3_Row][i] =(No1_Row == -1 || No2_Row == -1)|| MyToDecimal(temp.Rows[No1_Row][i]) == 0  ? 0 : MyToDecimal(temp.Rows[No2_Row][i]) / MyToDecimal(temp.Rows[No1_Row][i]);
                    }
                }
                No1_Row = ReportHelper.GetNoRow(temp, "QuotasID", "水泥产量");
                No2_Row = ReportHelper.GetNoRow(temp, "QuotasID", "水泥磨电量");
                No3_Row = ReportHelper.GetNoRow(temp, "QuotasID", "水泥磨电耗");
                if (No3_Row != -1)
                {
                    for (int i = No_Col; i < No_Col + 13; i++)
                    {
                        temp.Rows[No3_Row][i] = (No1_Row == -1 || No2_Row == -1)||MyToDecimal(temp.Rows[No1_Row][i]) == 0 ? 0 : MyToDecimal(temp.Rows[No2_Row][i]) / MyToDecimal(temp.Rows[No1_Row][i]);
                    }
                }
            }//水泥磨

            //存在熟料预测结果和水泥预测结果,则合并用电量，形成“总用电量”
            if (ReportHelper.GetNoRow(temp, "QuotasID", "熟料产量") != -1 && ReportHelper.GetNoRow(temp, "QuotasID", "水泥产量") != -1)
            {
                int No1_Row = ReportHelper.GetNoRow(temp, "QuotasID", "电力消耗量");
                int No2_Row = ReportHelper.GetNoRow(temp, "QuotasID", "水泥电力消耗量");
                temp_dr = temp.NewRow();
                temp_dr["IdSort"] = 0;        //“预测总用电量（kwh）排最前面
                temp_dr["QuotasID"] = "总用电量";
                temp_dr["IndicatorName"] = "预测总用电量（kwh）";
                for (int i = No_Col; i < No_Col + 13; i++)
                {  //1月到年度
                    temp_dr[i] =(No1_Row == -1 || No2_Row == -1) ? 0: MyToDecimal(temp.Rows[No1_Row][i]) + MyToDecimal(temp.Rows[No2_Row][i]);
                }
                temp.Rows.Add(temp_dr);
            }
            //去掉增加的行
            //DataRow[] drs = temp.Select("IdSort<>99");
            Result_Forecast_all.Clear();
            // Result_Forecast_all.Rows.Add(drs);
            DataRow[] tempTows = temp.Select("IdSort<>99");
            if (tempTows.Count() > 0)
            {
                Result_Forecast_all = tempTows.CopyToDataTable();
                foreach (DataRow drow in Result_Forecast_all.Rows)
                {
                    //替换OrganizationID，Name
                    drow["OrganizationID"] = v_OrganizationID;
                    drow["Name"] = "所有生产线";      //-- 生产机构名称
                }
            }
            Result_Forecast_all.Merge(Result_Forecast_ProductionLine);
            _dt = Result_Forecast_all;

            return Result_Forecast_all;
        }
        private DataTable GetTempTable(string _organizationID)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                //string sql = "SELECT * FROM system_Organization WHERE OrganizationID=@id";
                //string sql = "SELECT * FROM system_Organization WHERE OrganizationID LIKE '" + _organizationID + "%'";
                string sql = "SELECT B.* FROM system_Organization AS A, system_Organization AS B WHERE A.OrganizationID=@organizationID AND B.LevelCode LIKE A.LevelCode + '%'";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("organizationID", _organizationID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            DataTable result = ds.Tables[0];
            return result;
        }
        public static decimal MyToDecimal(object obj)
        {
            if (obj is DBNull)
            {
                obj = 0;
                return Convert.ToDecimal(obj);
            }
            else
            {
                return Convert.ToDecimal(obj);
            }
        }
    }
}
