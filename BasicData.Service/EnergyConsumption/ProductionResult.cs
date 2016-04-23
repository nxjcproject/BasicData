using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Transactions;
using BasicData.Infrastructure.Configuration;
using BasicData.Service.BasicService;
using SqlServerDataAdapter;


namespace BasicData.Service.EnergyConsumption
{
    public class ProductionResult
    {
        private static readonly string _connStr = ConnectionStringFactory.NXJCConnectionString;
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connStr);
        private static readonly BasicDataHelper _dataHelper = new BasicDataHelper(_connStr);

        private const string MaterialWeight = "MaterialWeight";
        private const string EquipmentUtilization = "EquipmentUtilization";

        public static DataTable GetEquipmentInfo(string myOrganizationId)
        {
            string m_Sql = @"Select distinct A.EquipmentCommonId as EquipmentCommonId, 
                B.Name as EquipmentCommonName
	            from equipment_EquipmentDetail A, equipment_EquipmentCommonInfo B
                where A.OrganizationId = @OrganizationID
                and A.Enabled = 1
                and A.EquipmentCommonId = B.EquipmentCommonId";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@OrganizationID", myOrganizationId) };
                DataTable m_Result = _dataFactory.Query(m_Sql, m_Parameters);
                return m_Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static DataTable GetQuotasInfo(string myEquipmentCommonId)
        {
            string m_Sql = @"Select A.QuotasID as QuotasId, 
                A.QuotasName as QuotasName,
                A.Type as Type
	            from plan_ProductionPlan_Template A 
                where A.EquipmentCommonId = @EquipmentCommonId
                order by DisplayIndex";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@EquipmentCommonId", myEquipmentCommonId) };
                DataTable m_Result = _dataFactory.Query(m_Sql, m_Parameters);
                return m_Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        /// <summary>
        /// 根据产线Id，产线类型和年份获得年计划
        /// </summary>
        /// <param name="myProductionLineType">产线类型</param>
        /// <param name="myOrganizationId">产线ID</param>
        /// <param name="myPlanYear">年计划的年份</param>
        /// <returns></returns>
        public static DataTable GetProductionPlanInfo(string myProductionQuotasId, string myOrganizationId, string myPlanYear, string myEquipmentCommonId)
        {
            string m_Sql = @"Select M.QuotasID, 
                M.EquipmentId, 
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
                            B.EquipmentId as EquipmentId,  
                            B.EquipmentName + A.QuotasName as QuotasName,
				            A.DisplayIndex as TemplateIndex,
				            B.DisplayIndex as EquipmentIndex   
                            from plan_ProductionPlan_Template A,
                            equipment_EquipmentDetail B            
                            where A.QuotasID = @QuotasId
                            and A.EquipmentCommonId = @EquipmentCommonId
                            and (A.OrganizationID is null or A.OrganizationID = @OrganizationID)
                            and B.OrganizationID = @OrganizationID
                            and A.EquipmentCommonId = B.EquipmentCommonId) M
	            left join (select C.*
				            from plan_ProductionYearlyPlan C, tz_Plan D
				            where C.KeyID = D.KeyID
				            and D.OrganizationID=@OrganizationID
				            and D.Date=@Date) N on M.EquipmentId = N.EquipmentId and M.QuotasID = N.QuotasID
	            order by M.EquipmentIndex, M.TemplateIndex";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@QuotasId", myProductionQuotasId), 
                                                  new SqlParameter("@OrganizationID", myOrganizationId), 
                                                  new SqlParameter("@EquipmentCommonId", myEquipmentCommonId), 
                                                  new SqlParameter("@Date", myPlanYear) };
                DataTable m_Result = _dataFactory.Query(m_Sql, m_Parameters);
                return m_Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static DataTable GetProductionResultInfo(string myProductionQuotasId, string myQuotasType, string myOrganizationId, string myPlanYear, string myEquipmentCommonId)
        {
            if (myQuotasType == MaterialWeight && myProductionQuotasId.Contains("台时产量"))   //台时产量
            {
                DataTable m_MachineHourCapacityTable = RunIndicators.EquipmentRunIndicators.GetMachineHourCapacityPerMonth(myProductionQuotasId, myOrganizationId, myPlanYear, myEquipmentCommonId, _dataFactory);
                return m_MachineHourCapacityTable;
            }
            else if (myQuotasType == MaterialWeight)                 //产量
            {
                DataTable m_WeightTable = RunIndicators.MaterialWeightResult.GetMaterialWeightResultPerMonth(myProductionQuotasId, myOrganizationId, myPlanYear, myEquipmentCommonId, _dataFactory);
                return m_WeightTable;
            }
            else if (myQuotasType == EquipmentUtilization)           //设备利用
            {
                DataTable m_RunTimeTable = RunIndicators.EquipmentRunIndicators.GetEquipmentUtilizationPerMonth(myProductionQuotasId, myOrganizationId, myPlanYear, myEquipmentCommonId, _dataFactory);
                return m_RunTimeTable;
            }
            else
            {
                return null;
            }
        }

    }
}
