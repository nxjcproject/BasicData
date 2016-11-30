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
        /// <summary>
        /// 根据产线Id，产线类型和年份获得年计划
        /// </summary>
        /// <param name="myProductionLineType">产线类型</param>
        /// <param name="myOrganizationId">产线ID</param>
        /// <param name="myPlanYear">年计划的年份</param>
        /// <returns></returns>
        public static DataTable GetEnergyPlanInfo(string myOrganizationId, string myPlanYear, string myPlanType)
        {
            string m_Sql = @"Select M.QuotasID, 
                M.VariableId, 
	            M.QuotasName,
                '计划' as Type, 
	            N.January as January,
	            N.February as February,
	            N.March as March,
	            N.April as April,
	            N.May as May,
	            N.June as June,
	            N.July as July,
	            N.August as August,
	            N.September as September,
	            N.October as October,
	            N.November as November,
	            N.December as December,
	            N.Totals as Totals,
	            N.Remarks as Remarks
	            from (Select
                            A.QuotasID as QuotasID,
                            A.VariableId as VariableId,  
                            A.QuotasName as QuotasName,
				            A.DisplayIndex as TemplateIndex
                            from plan_EnergyConsumptionPlan_Template A, system_Organization B
                            where A.PlanType = @PlanType
                            and rtrim(A.ProductionLineType) = rtrim(B.Type)
                            and B.OrganizationID = @OrganizationID) M
	            left join (select C.*
				            from plan_EnergyConsumptionYearlyPlan C, tz_Plan D
				            where C.KeyID = D.KeyID
				            and D.OrganizationID=@OrganizationID
                            and D.PlanType = @PlanType
				            and D.Date=@Date) N on M.QuotasID = N.QuotasID
	            order by M.TemplateIndex";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@OrganizationID", myOrganizationId), 
                                                  new SqlParameter("@PlanType", myPlanType), 
                                                  new SqlParameter("@Date", myPlanYear) };
                DataTable m_Result = _dataFactory.Query(m_Sql, m_Parameters);
                return m_Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static DataTable GetEnergyResultInfo(string myOrganizationId, string myPlanYear, string myPlanType, DataTable myEnergyPlanInfoTable)
        {
            DataTable m_EnergyPlanTemplateTable = GetEnergyPlanTemplate(myOrganizationId, myPlanType);
            DataTable m_BalanceEnergyTable = GetNormalEnergyResult(myOrganizationId, myPlanYear);
            DataTable m_EnergyResultTable = GetEnergyResult(m_EnergyPlanTemplateTable, m_BalanceEnergyTable, myOrganizationId, myPlanYear, myEnergyPlanInfoTable);
            return m_EnergyResultTable;
        }
        private static DataTable GetEnergyPlanTemplate(string myOrganizationId, string myPlanType)
        {
            string m_Sql = @"Select A.QuotasID
                                ,A.QuotasName
                                ,A.OrganizationID
                                ,A.DisplayIndex
                                ,A.ProductionLineType
                                ,A.PlanType
                                ,A.VariableId
                                ,A.ValueType
                                ,A.CaculateType
                                ,A.Denominator 
                                from plan_EnergyConsumptionPlan_Template A, system_Organization B
                                where A.PlanType = @PlanType
                                and A.ProductionLineType = B.Type
                                and B.OrganizationID = @OrganizationID";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@OrganizationID", myOrganizationId), 
                                                  new SqlParameter("@PlanType", myPlanType) };
                DataTable m_Result = _dataFactory.Query(m_Sql, m_Parameters);
                return m_Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        private static DataTable GetNormalEnergyResult(string myOrganizationId, string myPlanYear)
        {
            string m_StartTime = myPlanYear + "-01-01";
            string m_EndTime = myPlanYear + "-12-31";
            string m_Sql = @"SELECT B.VariableId
                                  ,SUBSTRING(A.TimeStamp,6,2) as MonthIndex
                                  ,B.OrganizationID
                                  ,B.ValueType
                                  ,sum(B.TotalPeakValleyFlatB) as Value
                              FROM  tz_Balance A, balance_Energy B
                              where A.StaticsCycle = 'day'
                              and A.TimeStamp >= @StartTime
                              and A.TimeStamp <= @EndTime
                              and B.ValueType in ('ElectricityQuantity','MaterialWeight')
                              and B.OrganizationID = @OrganizationID
                              and A.BalanceId = B.KeyId
                              group by B.OrganizationID, B.ValueType, B.VariableId, SUBSTRING(A.TimeStamp,6,2)
                              order by MonthIndex, B.ValueType";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@OrganizationID", myOrganizationId), 
                                                  new SqlParameter("@StartTime", m_StartTime),
                                                  new SqlParameter("@EndTime", m_EndTime) };
                DataTable m_Result = _dataFactory.Query(m_Sql, m_Parameters);
                return m_Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        private static DataTable GetEnergyResult(DataTable myEnergyPlanTemplateTable, DataTable myBalanceEnergyTable, string myOrganizationId, string myPlanYear, DataTable myEnergyPlanInfoTable)
        {
            int m_MonthCount = 12;
            DateTime m_CurrentDate = DateTime.Now;
            if (m_CurrentDate.Year > Int32.Parse(myPlanYear))
            {
                m_MonthCount = 12;
            }
            else if (m_CurrentDate.Year == Int32.Parse(myPlanYear))
            {
                m_MonthCount = m_CurrentDate.Month;
            }
            else
            {
                m_MonthCount = 0;
            }
            if (myEnergyPlanTemplateTable != null && myBalanceEnergyTable != null && myEnergyPlanInfoTable != null)
            {
                DataTable m_EnergyResultTable = myEnergyPlanInfoTable.Clone();
                for (int i = 0; i < myEnergyPlanTemplateTable.Rows.Count; i++)
                {
                    DataRow m_NewRowTemp = m_EnergyResultTable.NewRow();
                    m_NewRowTemp["QuotasID"] = myEnergyPlanTemplateTable.Rows[i]["QuotasID"].ToString();
                    m_NewRowTemp["VariableId"] = myEnergyPlanTemplateTable.Rows[i]["VariableId"].ToString();
                    m_NewRowTemp["QuotasName"] = myEnergyPlanTemplateTable.Rows[i]["QuotasName"].ToString();
                    m_NewRowTemp["Type"] = "实绩";
                    for (int j = 1; j <= m_MonthCount; j++)
                    {
                        if (myEnergyPlanTemplateTable.Rows[i]["ValueType"].ToString() == "ElectricityQuantity" && myEnergyPlanTemplateTable.Rows[i]["CaculateType"].ToString() == "Normal")   //计算电量
                        {
                            string m_ConditionElectricity = string.Format("VariableId = '{0}_ElectricityQuantity'and MonthIndex = '{1}' and ValueType = '{2}'"
                                , myEnergyPlanTemplateTable.Rows[i]["VariableId"].ToString(), j.ToString("00"), "ElectricityQuantity");
                            object m_ElectricityQuantity = myBalanceEnergyTable.Compute("sum(Value)", m_ConditionElectricity);
                            if (m_ElectricityQuantity != DBNull.Value)
                            {
                                m_NewRowTemp[j + 3] = (decimal)m_ElectricityQuantity;
                            }
                            else
                            {
                                m_NewRowTemp[j + 3] = 0.0m;
                            }
                        }
                        else if (myEnergyPlanTemplateTable.Rows[i]["ValueType"].ToString() == "ElectricityConsumption" && myEnergyPlanTemplateTable.Rows[i]["CaculateType"].ToString() == "Normal")   //计算电耗
                        {
                            string m_ConditionElectricity = string.Format("VariableId = '{0}_ElectricityQuantity'and MonthIndex = '{1}' and ValueType = '{2}'"
                                , myEnergyPlanTemplateTable.Rows[i]["VariableId"].ToString(), j.ToString("00"), "ElectricityQuantity");
                            string m_ConditionMaterialWeight = string.Format("VariableId = '{0}'and MonthIndex = '{1}' and ValueType = '{2}'"
                                , myEnergyPlanTemplateTable.Rows[i]["Denominator"].ToString(), j.ToString("00"), "MaterialWeight");
                            object m_ElectricityQuantity = myBalanceEnergyTable.Compute("sum(Value)", m_ConditionElectricity);
                            object m_MaterialWeight = myBalanceEnergyTable.Compute("sum(Value)", m_ConditionMaterialWeight);
                            if (m_ElectricityQuantity != DBNull.Value && m_MaterialWeight != DBNull.Value)
                            {
                                decimal m_MaterialWeightValue = (decimal)m_MaterialWeight;
                                m_NewRowTemp[j + 3] = m_MaterialWeightValue == 0.0m ? 0.0m : (decimal)m_ElectricityQuantity / m_MaterialWeightValue;
                            }
                            else
                            {
                                m_NewRowTemp[j + 3] = 0.0m;
                            }
                        }
                        else if (myEnergyPlanTemplateTable.Rows[i]["ValueType"].ToString() == "MaterialWeight" && myEnergyPlanTemplateTable.Rows[i]["CaculateType"].ToString() == "Normal")   //计算重量
                        {
                            object m_MaterialWeight = myBalanceEnergyTable.Compute("sum(Value)", string.Format("VariableId = '{0}' and MonthIndex = '{1}' and ValueType = '{2}'"
                                , myEnergyPlanTemplateTable.Rows[i]["VariableId"].ToString(), j.ToString("00"), "MaterialWeight"));
                            if (m_MaterialWeight != DBNull.Value)
                            {
                                m_NewRowTemp[j + 3] = (decimal)m_MaterialWeight;
                            }
                            else
                            {
                                m_NewRowTemp[j + 3] = 0.0m;
                            }
                        }
                        else if (myEnergyPlanTemplateTable.Rows[i]["CaculateType"].ToString() == "Comprehensive")   //计算综合电耗
                        {
                            DateTime m_MonthDate = new DateTime(Int32.Parse(myPlanYear), j, 1);
                            string m_VariableId = myEnergyPlanTemplateTable.Rows[i]["VariableId"].ToString() + "_" + myEnergyPlanTemplateTable.Rows[i]["ValueType"].ToString() + "_" +myEnergyPlanTemplateTable.Rows[i]["CaculateType"].ToString();
                            m_NewRowTemp[j + 3] = GetComprehensiveData(m_MonthDate.ToString("yyyy-MM-dd"), m_MonthDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"), myOrganizationId, m_VariableId);
                        }
                    }
                    m_EnergyResultTable.Rows.Add(m_NewRowTemp);
                }
                return m_EnergyResultTable;
            }
            else
            {
                return null;
            }
        }
        private static Decimal GetComprehensiveData(string myStartTime, string myEndTime, string myOrganizationId, string myVariableId)
        {
            ////计算综合电耗、煤耗
            decimal Value = 0.0m;
            AutoSetParameters.AutoGetEnergyConsumptionRuntime_V1 m_AutoGetEnergyConsumption_V1 = new AutoSetParameters.AutoGetEnergyConsumptionRuntime_V1(new SqlServerDataAdapter.SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString));
            DataTable m_OrganizationLevelCodeTable = GetOrganizationInfo(myOrganizationId);
            if (m_OrganizationLevelCodeTable != null && m_OrganizationLevelCodeTable.Rows.Count > 0)
            {
                string m_OrganizationLevelCode = m_OrganizationLevelCodeTable.Rows[0]["LevelCode"].ToString();

                if (myVariableId == "clinker_ElectricityConsumption_Comprehensive")              //熟料综合电耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day", myStartTime, myEndTime, m_OrganizationLevelCode).CaculateValue;
                }
                else if (myVariableId == "clinker_CoalConsumption_Comprehensive")              //熟料综合煤耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetClinkerCoalConsumptionWithFormula("day", myStartTime, myEndTime, m_OrganizationLevelCode).CaculateValue;
                }
                else if (myVariableId == "clinker_EnergyConsumption_Comprehensive")              //熟料能耗电耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetClinkerEnergyConsumptionWithFormula("day", myStartTime, myEndTime, m_OrganizationLevelCode).CaculateValue;
                }
                else if (myVariableId == "cementmill_ElectricityConsumption_Comprehensive")              //水泥综合电耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day", myStartTime, myEndTime, m_OrganizationLevelCode).CaculateValue;
                }
                else if (myVariableId == "cementmill_CoalConsumption_Comprehensive")                   //水泥综合煤耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetCementCoalConsumptionWithFormula("day", myStartTime, myEndTime, m_OrganizationLevelCode).CaculateValue;
                }
                else if (myVariableId == "cementmill_EnergyConsumption_Comprehensive")                //水泥综合能耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetCementEnergyConsumptionWithFormula("day", myStartTime, myEndTime, m_OrganizationLevelCode).CaculateValue;
                }
            }
            return Value;
        }
        private static DataTable GetOrganizationInfo(string myOrganizationId)
        {
            string _connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connectionString);

            string m_Sql = @"select A.LevelCode as LevelCode, A.LevelType as LevelType from system_Organization A
                     where A.OrganizationID = @OrganizationID";
            List<SqlParameter> m_Parameters = new List<SqlParameter>();
            m_Parameters.Add(new SqlParameter("@OrganizationID", myOrganizationId));
            DataTable table = _dataFactory.Query(m_Sql, m_Parameters.ToArray());
            return table;
        }
    }
}
