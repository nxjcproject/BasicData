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
    public class PurchaseSalesResult
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
	            from plan_PurchaseSalesPlan_Template A 
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
        public static DataTable GetPurchaseSalesPlanInfo(string myOrganizationId, string myType, string myPlanType, string myPlanYear)
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
                            A.QuotasName as QuotasName,
                            A.VariableId as VariableId, 
				            A.DisplayIndex as TemplateIndex
                            from plan_PurchaseSalesPlan_Template A
                            where A.Type = @Type
                            and (A.OrganizationID is null or A.OrganizationID = @OrganizationID)) M
	            left join (select C.*
				            from plan_PurchaseSalesYearlyPlan C, tz_Plan D
				            where C.KeyID = D.KeyID
				            and D.OrganizationID=@OrganizationID
				            and D.Date=@Date
                            and D.PlanType = @PlanType) N on M.VariableId = N.VariableId and M.QuotasID = N.QuotasID
	            order by M.TemplateIndex";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@Type", myType), 
                                                  new SqlParameter("@OrganizationID", myOrganizationId), 
                                                  new SqlParameter("@PlanType", myPlanType), 
                                                  new SqlParameter("@Date", myPlanYear) };
                DataTable m_Result = _dataFactory.Query(m_Sql, m_Parameters);
                return m_Result;
            }
            catch (Exception e)
            {
                var m_e = e;
                return null;
            }
        }
        public static DataTable GetPurchaseSalesResultInfo(string myOrganizationId, string myType, string myPlanYear)
        {
            int m_PlanYear = Int32.Parse(myPlanYear);
            string m_Sql = @"Select M.OrganizationID, M.VariableId, convert(varchar(7),M.EndTime,120) as EndTime, sum(M.Value) as Value from
                                    (SELECT  A.BillNO
                                        ,case when A.sales_gblx = 'RD' then -A.Suttle else A.Suttle end AS Value
                                        ,A.OrganizationID
                                        ,A.ggxh AS Specs 
                                        ,CASE WHEN A.weightdate > A.lightdate THEN A.lightdate ELSE A.weightdate END AS StartTime
                                        ,CASE WHEN A.weightdate > A.lightdate THEN A.weightdate ELSE A.lightdate END AS EndTime
                                        ,CASE WHEN A.Type = '0' THEN 'Purchase' WHEN A.Type = '3' THEN 'Sales' else 'Translate' end  as Type
                                        ,NULL AS VariableSpecs
                                        ,B.VariableId
                                    FROM extern_interface.dbo.WB_WeightNYGL A, dbo.inventory_MaterialContrast B, plan_PurchaseSalesPlan_Template C
                                    where A.OrganizationID = '{0}' 
                                          and A.Type in ('0','3')
                                          and ((A.weightdate > A.lightdate and A.weightdate >= '{1}' and A.weightdate < '{2}')
                                             or (A.weightdate <= A.lightdate and A.lightdate >= '{1}' and A.lightdate < '{2}'))
                                          and A.Material = B.MaterialID
                                          and B.VariableId = C.VariableId
                                          and (C.OrganizationID is null or C.OrganizationID = '{0}')) M
                                where M.Type = '{3}'
                                group by M.OrganizationID, M.VariableId, convert(varchar(7),M.EndTime,120)
                                order by M.OrganizationID, M.VariableId";
            m_Sql = string.Format(m_Sql, myOrganizationId, myPlanYear + "-01-01 00:00:00", (m_PlanYear + 1).ToString("0000") + "-01-01 00:00:00", myType);
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                if (m_Result != null)
                {
                    DataTable m_PurchaseSalesResultTable = GetPurchaseSalesResultInfo(myPlanYear, m_Result);
                    return m_PurchaseSalesResultTable;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        private static DataTable GetPurchaseSalesResultInfo(string myPlanYear, DataTable myPurchaseSalesResultTable)
        {
            List<string> m_VariableIdArray = new List<string>();
            string m_VariableIdTemp = "";

            DataTable m_PurchaseSalesResultTable = new DataTable();
            m_PurchaseSalesResultTable.Columns.Add("VariableId", typeof(string));
            for (int i = 1; i <= 12; i++)
            {
                m_PurchaseSalesResultTable.Columns.Add("Month" + i.ToString("00"), typeof(decimal));
            }
            for (int i = 0; i < myPurchaseSalesResultTable.Rows.Count; i++)
            {
                if (m_VariableIdTemp != myPurchaseSalesResultTable.Rows[i]["VariableId"].ToString())
                {
                    m_VariableIdTemp = myPurchaseSalesResultTable.Rows[i]["VariableId"].ToString();
                    m_VariableIdArray.Add(m_VariableIdTemp);
                }
            }
            for (int i = 0; i < m_VariableIdArray.Count; i++)
            {
                DataRow m_NewDataRowTemp = m_PurchaseSalesResultTable.NewRow();
                DataRow[] m_SelectDataRows = myPurchaseSalesResultTable.Select(string.Format("VariableId = '{0}'", m_VariableIdArray[i]));
                for (int j = 0; j < m_SelectDataRows.Length; j++)
                {
                    string m_EndTimeTemp = m_SelectDataRows[j]["EndTime"].ToString();
                    string m_ColumnName = "Month" + m_EndTimeTemp.Substring(5);
                    m_NewDataRowTemp[m_ColumnName] = m_SelectDataRows[j]["Value"];
                }
                m_PurchaseSalesResultTable.Rows.Add(m_NewDataRowTemp);
            }
            return m_PurchaseSalesResultTable;
        }
    }
}
