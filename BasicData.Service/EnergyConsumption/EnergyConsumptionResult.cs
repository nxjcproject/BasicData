using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using BasicData.Infrastructure.Configuration;
using BasicData.Service.BasicService;
using SqlServerDataAdapter;


namespace BasicData.Service.EnergyConsumption
{
    public class EnergyConsumptionResult
    {
        private static readonly string _connStr = ConnectionStringFactory.NXJCConnectionString;
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connStr);
        private static readonly BasicDataHelper _dataHelper = new BasicDataHelper(_connStr);


        public static string GetEnergyConsumptionResultInfo(string myPlanYear, string mySelectedLevelCode, ref DataTable temp_plan)
        {
            //////////////////////////////////////////////////////////////////
            //入口参数
            string v_date;
            v_date = myPlanYear;
            //返回错误串
            string v_string_error = ""; //缺计划的生产线名称  


            //（1）	生成层次结构（张迪完成，能源消耗月统计分析报表）→Temp
            string sqlQuery = @"select
                                OrganizationID,
                                Name, 
                                (case when Type ='熟料' then 1 when Type ='水泥磨' then 2 when Type ='余热发电' then 3 else -1 end) as Type
                                from System_Organization where System_Organization.LevelCode like '{0}%'";
            if (mySelectedLevelCode != "")
            {
                sqlQuery = string.Format(sqlQuery, mySelectedLevelCode);
            }
            else
            {
                sqlQuery = string.Format(sqlQuery, "NULL");
            }
            DataTable temp = _dataFactory.Query(sqlQuery);

            //临时变量
            int No1_Row, No2_Row, No3_Row;  //行号
            int No_Col = 0;     //“1月”的列号

            //（2）准备工作
            //??????????????????????????????????????????????????????????????????????????????????????????????????????
            //需要更改
            //
            //DataTable temp_plan = tzHelper.CreateTableStructure("？？？？？？？？？？？？？？？？？？");
            //DataTable temp_plan = _dataHelper.CreateTableStructure("temp_plan");
            //temp_plan = _dataHelper.CreateTableStructure("temp_plan");
            temp_plan = _dataHelper.CreateTempPlanTableStructure();
            for (int i = 0; i < temp_plan.Columns.Count; i++)
            {
                if (temp_plan.Columns[i].ColumnName == "January")
                {
                    No_Col = i; //January的列号
                    break;
                }
            }

            // （3）遍历Temp,获取各生产线产量与能耗计划
            DataTable temp1;
            DataRow temp_dr;
            foreach (DataRow dr in temp.Rows)
            {
                if (Convert.ToInt32(dr["Type"]) == 1 || Convert.ToInt32(dr["Type"]) == 2)
                {
                    //??????????????????????????????????????????????????????????????;
                    //需要更改
                    //
                    //temp1 = tzHelper.GetReportData("tz_Report", Convert.ToString(dr["OrganizationID"]), v_date, "????????????????????????????????????????????????");
                    //temp1 = _dataFactory.Query("select * from temp_plan where OrganizationID='" + Convert.ToString(dr["OrganizationID"]) + "'");
                    string m_Temp1Sql = @"select tz_Plan.OrganizationID as OrganizationID, 
                           (case when tz_Plan.ProductionLineType ='熟料' then 1 when tz_Plan.ProductionLineType ='水泥磨' then 2 when tz_Plan.ProductionLineType ='余热发电' then 3 else -1 end) as Type,
                           (case when tz_Plan.ProductionLineType ='熟料' then plan_EnergyConsumptionYearlyPlan.DisplayIndex + 10 when tz_Plan.ProductionLineType ='水泥磨' then plan_EnergyConsumptionYearlyPlan.DisplayIndex + 20 when tz_Plan.ProductionLineType ='余热发电' then plan_EnergyConsumptionYearlyPlan.DisplayIndex + 30 else -1 end) as IdSort, 
                            plan_EnergyConsumptionYearlyPlan.QuotasID as QuotasID, 
                            plan_EnergyConsumptionYearlyPlan.QuotasName as IndicatorName, 
                            plan_EnergyConsumptionYearlyPlan.January as January,
                            plan_EnergyConsumptionYearlyPlan.February as February,
                            plan_EnergyConsumptionYearlyPlan.March as March,
                            plan_EnergyConsumptionYearlyPlan.April as April,
                            plan_EnergyConsumptionYearlyPlan.May as May,
                            plan_EnergyConsumptionYearlyPlan.June as June,
                            plan_EnergyConsumptionYearlyPlan.July as July,
                            plan_EnergyConsumptionYearlyPlan.August as August,
                            plan_EnergyConsumptionYearlyPlan.September as September,
                            plan_EnergyConsumptionYearlyPlan.October as October,
                            plan_EnergyConsumptionYearlyPlan.November as November,
                            plan_EnergyConsumptionYearlyPlan.December as December, 
                            plan_EnergyConsumptionYearlyPlan.Totals as Totals, 
                            plan_EnergyConsumptionYearlyPlan.Remarks as Remarks 
                            from plan_EnergyConsumptionYearlyPlan, tz_Plan 
                            where plan_EnergyConsumptionYearlyPlan.KeyID = tz_Plan.KeyID 
                            and tz_Plan.Date = '{0}' and tz_Plan.OrganizationID='{1}'
                            order by plan_EnergyConsumptionYearlyPlan.DisplayIndex";
                    m_Temp1Sql = string.Format(m_Temp1Sql, myPlanYear, Convert.ToString(dr["OrganizationID"]));
                    temp1 = _dataFactory.Query(m_Temp1Sql);
                    if (temp1.Rows.Count > 0)  //存在计划
                    {
                        if (Convert.ToInt32(dr["Type"]) == 1)
                        {   //孰料
                            //从生产线信息表中获得生熟料比v_RawBatchClinker
                            double v_RatioRawBatchClinker;
                            DataTable temp_Organization = _dataFactory.Query("select * from System_Organization where OrganizationID='" + Convert.ToString(dr["OrganizationID"]) + "'");
                            if (temp_Organization.Rows.Count > 0) v_RatioRawBatchClinker = Convert.IsDBNull(temp_Organization.Rows[0]["RawToClinkerCoff"]) ?
                                0 : Convert.ToDouble(temp_Organization.Rows[0]["RawToClinkerCoff"]);
                            else v_RatioRawBatchClinker = 0;

                            //增加熟料耗电量 、耗煤量、生料磨耗电量、煤磨耗电量
                            No1_Row = EnergyConsumptionResultHelper.GetNoRow(temp1, "QuotasID", "熟料产量");
                            No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp1, "QuotasID", "熟料电耗");

                            temp_dr = temp1.NewRow();
                            temp_dr["IdSort"] = "99";
                            temp_dr["QuotasID"] = "生料产量";
                            for (int i = No_Col; i < No_Col + 13; i++)
                            {
                                temp_dr[i] = Convert.ToDouble(temp1.Rows[No1_Row][i]) * v_RatioRawBatchClinker;
                            }
                            temp1.Rows.Add(temp_dr);

                            temp_dr = temp1.NewRow();
                            temp_dr["IdSort"] = "99";
                            temp_dr["QuotasID"] = "熟料耗电量";
                            for (int i = No_Col; i < No_Col + 13; i++)
                            {  //1月到年度
                                temp_dr[i] = Convert.ToDouble(temp1.Rows[No1_Row][i]) * Convert.ToDouble(temp1.Rows[No2_Row][i]);
                            }
                            temp1.Rows.Add(temp_dr);

                            No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp1, "QuotasID", "熟料煤耗");
                            temp_dr = temp1.NewRow();
                            temp_dr["IdSort"] = "99";
                            temp_dr["QuotasID"] = "耗煤量";
                            for (int i = No_Col; i < No_Col + 13; i++)
                            {  //1月到年度
                                temp_dr[i] = Convert.ToDouble(temp1.Rows[No1_Row][i]) * Convert.ToDouble(temp1.Rows[No2_Row][i])/1000;
                            }
                            temp1.Rows.Add(temp_dr);

                            No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp1, "QuotasID", "生料磨电耗");
                            temp_dr = temp1.NewRow();
                            temp_dr["IdSort"] = "99";
                            temp_dr["QuotasID"] = "生料磨耗电量";
                            for (int i = No_Col; i < No_Col + 13; i++)
                            {  //1月到年度
                                temp_dr[i] = Convert.ToDouble(temp1.Rows[No1_Row][i]) * v_RatioRawBatchClinker * Convert.ToDouble(temp1.Rows[No2_Row][i]);
                            }
                            temp1.Rows.Add(temp_dr);

                            No1_Row = EnergyConsumptionResultHelper.GetNoRow(temp1, "QuotasID", "耗煤量");
                            No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp1, "QuotasID", "煤磨电耗");
                            temp_dr = temp1.NewRow();
                            temp_dr["IdSort"] = "99";
                            temp_dr["QuotasID"] = "煤磨耗电量";
                            for (int i = No_Col; i < No_Col + 13; i++)
                            {  //1月到年度
                                temp_dr[i] = Convert.ToDouble(temp1.Rows[No1_Row][i]) * Convert.ToDouble(temp1.Rows[No2_Row][i]);
                            }
                            temp1.Rows.Add(temp_dr);
                        }
                        else
                        {  //水泥磨
                            No1_Row = EnergyConsumptionResultHelper.GetNoRow(temp1, "QuotasID", "水泥产量");
                            No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp1, "QuotasID", "水泥电耗");
                            //增加水泥耗电量、水泥磨耗电量
                            temp_dr = temp1.NewRow();
                            temp_dr["IdSort"] = "99";
                            temp_dr["QuotasID"] = "水泥耗电量";
                            for (int i = No_Col; i < No_Col + 13; i++)
                            {  //1月到年度
                                temp_dr[i] = Convert.ToDouble(temp1.Rows[No1_Row][i]) * Convert.ToDouble(temp1.Rows[No2_Row][i]);
                            }
                            temp1.Rows.Add(temp_dr);

                            No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp1, "QuotasID", "水泥磨电耗");
                            temp_dr = temp1.NewRow();
                            temp_dr["IdSort"] = "99";
                            temp_dr["QuotasID"] = "水泥磨耗电量";
                            for (int i = No_Col; i < No_Col + 13; i++)
                            {  //1月到年度
                                temp_dr[i] = Convert.ToDouble(temp1.Rows[No1_Row][i]) * Convert.ToDouble(temp1.Rows[No2_Row][i]);
                            }
                            temp1.Rows.Add(temp_dr);
                        }
                    }
                    temp_plan.Merge(temp1);
                }
                else
                { //不存在计划，形成缺计划的生产线名称
                    if (v_string_error.Length == 0)
                    {
                        v_string_error = Convert.ToString(dr["Name"]);
                    }
                    else
                    {
                        v_string_error = v_string_error + "," + dr["Name"];
                    }
                }
            }

            //返回，无任何计划
            if (temp_plan.Rows.Count == 0)
            {
                //Return_error("无任何生产线的“生产线产量与能耗计划”！无法生成该信息…");
            }

            //合并计划
            temp = EnergyConsumptionResultHelper.MyTotalOn(temp_plan, "QuotasID", "January,February,March,April,May,June,July,August,September,October,November,December,Totals");
            if (EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "熟料产量") != -1)
            {  //存在熟料计划
                //计算吨熟料发电量
                No1_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "熟料产量");
                No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "发电量");
                No3_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "吨熟料发电量");
                for (int i = No_Col; i < No_Col + 13; i++)
                {  //1月到年度
                    temp.Rows[No3_Row][i] = Convert.ToDouble(temp.Rows[No1_Row][i]) > 0 ?
                        Convert.ToDouble(temp.Rows[No2_Row][i]) / Convert.ToDouble(temp.Rows[No1_Row][i]) : 0;
                }
                //计算熟料电耗
                No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "熟料耗电量");
                No3_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "熟料电耗");
                for (int i = No_Col; i < No_Col + 13; i++)
                {  //1月到年度
                    temp.Rows[No3_Row][i] = Convert.ToDouble(temp.Rows[No1_Row][i]) > 0 ?
                        Convert.ToDouble(temp.Rows[No2_Row][i]) / Convert.ToDouble(temp.Rows[No1_Row][i]) : 0;
                }
                //计算熟料煤耗
                No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "耗煤量");
                No3_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "熟料煤耗");
                for (int i = No_Col; i < No_Col + 13; i++)
                {  //1月到年度
                    temp.Rows[No3_Row][i] = Convert.ToDouble(temp.Rows[No1_Row][i]) > 0 ?
                        Convert.ToDouble(temp.Rows[No2_Row][i])*1000 / Convert.ToDouble(temp.Rows[No1_Row][i]) : 0;
                }
                //计算生料磨电耗
                No1_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "生料产量");
                No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "生料磨耗电量");
                No3_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "生料磨电耗");
                for (int i = No_Col; i < No_Col + 13; i++)
                {  //1月到年度
                    temp.Rows[No3_Row][i] = Convert.ToDouble(temp.Rows[No1_Row][i]) > 0 ?
                        Convert.ToDouble(temp.Rows[No2_Row][i]) / Convert.ToDouble(temp.Rows[No1_Row][i]) : 0;
                }
                //计算煤磨电耗
                No1_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "耗煤量");
                No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "煤磨耗电量");
                No3_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "煤磨电耗");
                for (int i = No_Col; i < No_Col + 13; i++)
                {  //1月到年度
                    temp.Rows[No3_Row][i] = Convert.ToDouble(temp.Rows[No1_Row][i]) > 0 ?
                        Convert.ToDouble(temp.Rows[No2_Row][i]) / Convert.ToDouble(temp.Rows[No1_Row][i]) : 0;
                }
            }//存在熟料计划

            if (EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "水泥产量") != -1)//存在水泥磨计划
            {
                //计算水泥电耗
                No1_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "水泥产量");
                No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "水泥耗电量");
                No3_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "水泥电耗");
                for (int i = No_Col; i < No_Col + 13; i++)
                {  //1月到年度
                    temp.Rows[No3_Row][i] = Convert.ToDouble(temp.Rows[No1_Row][i]) > 0 ?
                       Convert.ToDouble(temp.Rows[No2_Row][i]) / Convert.ToDouble(temp.Rows[No1_Row][i]) : 0;
                }
                //计算水泥磨电耗
                No2_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "水泥磨耗电量");
                No3_Row = EnergyConsumptionResultHelper.GetNoRow(temp, "QuotasID", "水泥磨电耗");
                for (int i = No_Col; i < No_Col + 13; i++)
                {  //1月到年度
                    temp.Rows[No3_Row][i] = Convert.ToDouble(temp.Rows[No1_Row][i]) > 0 ?
                       Convert.ToDouble(temp.Rows[No2_Row][i]) / Convert.ToDouble(temp.Rows[No1_Row][i]) : 0;
                }
            }//存在水泥磨计划

            //去掉增加的行
            if (temp_plan.Rows.Count > 0)
            {
                temp_plan = temp.Select("IdSort<>'99'").CopyToDataTable();

                //按照IdSort排序
                temp_plan = EnergyConsumptionResultHelper.SortTable(temp_plan, "IdSort");
            }
            //返回
            //计划不全
            if (v_string_error.Length != 0)
            {
                //myEnergyConsumptionResultInfo = temp_plan;
                return "由于以下生产线的“生产线产量与能耗计划”不存在：" + v_string_error + "！所以生成的结果不完整…";
                //Return(temp_plan);
            }
            else
            {
                return "";
            }

            //dataGridView1.DataSource = temp_plan;
        }
    }
}
