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
    public class EnergyConsumptionPlan
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
        public static DataTable GetEnergyConsumptionInfo(string myProductionLineType, string myOrganizationId, string myPlanYear, string myPlanType)
        {
            string m_Sql = @"Select
                    A.QuotasID as QuotasID,  
                    A.QuotasName as QuotasName,
                    B.January as January,
                    B.February as February,
                    B.March as March,
                    B.April as April,
                    B.May as May,
                    B.June as June,
                    B.July as July,
                    B.August as August,
                    B.September as September,
                    B.October as October,
                    B.November as November,
                    B.December as December,
                    B.Totals as Totals,
                    B.Remarks as Remarks
                    from plan_EnergyConsumptionPlan_Template A 
                    left join 
					    (select C.*
							   from plan_EnergyConsumptionYearlyPlan C, tz_Plan D
							   where C.KeyID = D.KeyID
							   and D.OrganizationID=@OrganizationID
							   and D.Date=@Date
							   and D.ProductionLineType = @ProductionLineType
                               and D.PlanType = @PlanType) B on A.QuotasID = B.QuotasID
                    where A.ProductionLineType = @ProductionLineType
                    and (A.OrganizationID is null or A.OrganizationID = @OrganizationID)
					order by A.DisplayIndex";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@ProductionLineType", myProductionLineType), 
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
        public static DataTable CreateTableStructure(string myDataTableName)
        {
            try
            {
                return _dataHelper.CreateTableStructure(myDataTableName);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 保存年计划信息
        /// </summary>
        /// <param name="myDataTableName">数据表的名称</param>
        /// <param name="myDataTable">数据表内容</param>
        /// <returns></returns>
        public static int SaveEnergyConsumptionInfo(string myDataTableName, DataTable myDataTable)
        {
            return _dataFactory.Save(myDataTableName, myDataTable);
        }
        /// <summary>
        /// 根据KeyId删除能源计划信息
        /// </summary>
        /// <param name="myKeyId">KeyId</param>
        /// <returns>是否删除成功</returns>
        public static int DeleteEnergyConsumptionInfo(string myKeyId)
        {
            string m_Sql = @"DELETE FROM plan_EnergyConsumptionYearlyPlan where KeyId=@KeyId";
            SqlParameter[] m_Parameters = { new SqlParameter("@KeyId", myKeyId) };
            try
            {
                return _dataFactory.ExecuteSQL(m_Sql, m_Parameters);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 在tz表里获得年报的相关数据
        /// </summary>
        /// <param name="myPlanYear">年计划的年份</param>
        /// <param name="myOrganizationId">组织机构id</param>
        /// <param name="myProductionLineType">生产线类型</param>
        /// <returns>KeyId</returns>
        public static string GetKeyIdFromTz(string myPlanYear, string myOrganizationId, string myProductionLineType, string myPlanType)
        {
            string m_Sql = @"Select A.KeyID as KeyId FROM tz_Plan A where A.OrganizationID=@OrganizationID and A.Date=@Date and A.ProductionLineType=@ProductionLineType and A.PlanType = @PlanType";
            SqlParameter[] m_Parameters = { new SqlParameter("@OrganizationID", myOrganizationId), new SqlParameter("@Date", myPlanYear), new SqlParameter("@ProductionLineType", myProductionLineType), new SqlParameter("@PlanType", myPlanType)};
            try
            {
                DataTable m_DataTable = _dataFactory.Query(m_Sql, m_Parameters);
                if (m_DataTable != null)
                {
                    if (m_DataTable.Rows.Count > 0)
                    {
                        return m_DataTable.Rows[0]["KeyId"].ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static int InsertTzPlan(string myKeyId, string myOrganizationId, string myProductionLineType, string myPlanYear, string myModifierId, string myPlanType)
        {
            string m_Sql = @" Insert into tz_Plan 
                ( KeyID, OrganizationID, Date, ProductionLineType, PlanType, TableName, CreationDate, Version, ModifierID, Statue, Remarks) 
                values
                (@KeyID, @OrganizationID, @Date, @ProductionLineType, @PlanType, @TableName, @CreationDate, @Version, @ModifierID, @Statue, @Remarks)";
            SqlParameter[] m_Parameters = { new SqlParameter("@KeyID", myKeyId),
                                          new SqlParameter("@OrganizationID", myOrganizationId),
                                          new SqlParameter("@Date", myPlanYear),
                                          new SqlParameter("@ProductionLineType", myProductionLineType),
                                          new SqlParameter("@PlanType", myPlanType),
                                          new SqlParameter("@TableName", "plan_EnergyConsumptionYearlyPlan"),
                                          new SqlParameter("@CreationDate", DateTime.Now),
                                          new SqlParameter("@Version", DateTime.Now),
                                          new SqlParameter("@ModifierID", myModifierId),
                                          new SqlParameter("@Statue", 1),
                                          new SqlParameter("@Remarks", "")};
            try
            {
                return _dataFactory.ExecuteSQL(m_Sql, m_Parameters);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public static int UpdateTzPlan(string myKeyId, string myModifierId)
        {
            string m_Sql = @"UPDATE tz_Plan SET              
                ModifierID=@ModifierID, 
                Version=@Version 
                where KeyID=@KeyID";
            SqlParameter[] m_Parameters = {new SqlParameter("@KeyID", myKeyId),
                                          new SqlParameter("@ModifierID", myModifierId),
                                          new SqlParameter("@Version", DateTime.Now)};
            try
            {
                return _dataFactory.ExecuteSQL(m_Sql, m_Parameters);
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
