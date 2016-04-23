using BasicData.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BasicData.Service.EquipmentAccount
{
    public static class EquipmentAccountService
    {
        /// <summary>
        /// 取出设备信息
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static DataTable GetEquipmentDatas(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT A.*,B.Name
                                FROM system_EquipmentAccount AS A,system_Organization AS B
                                WHERE A.OrganizationID=B.OrganizationID
                                AND B.LevelCode LIKE (select C.LevelCode from system_Organization as C where C.OrganizationID=@myOrganizationID)+'%'";
            SqlParameter parameter = new SqlParameter("myOrganizationID", organizationId);
            DataTable result = dataFactory.Query(mySql, parameter);
            return result;
        }
        
        public static DataTable GetOrganizationIds(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT A.OrganizationID,A.Name 
                                FROM system_Organization AS A 
                                WHERE A.LevelCode LIKE (select C.LevelCode from system_Organization as C where C.OrganizationID=@myOrganizationID)+'%'
                                AND (A.LevelType='Factory' OR A.LevelType='ProductionLine') ";
            SqlParameter parameter = new SqlParameter("myOrganizationID", organizationId);
            DataTable result = dataFactory.Query(mySql, parameter);
            return result;
        }
        /// <summary>
        /// 保存设备
        /// </summary>
        /// <param name="VariableId"></param>
        /// <param name="OrganizationID"></param>
        /// <param name="EquipmentName"></param>
        /// <param name="MonitorType"></param>
        /// <param name="PowerSupply"></param>
        /// <param name="VoltageGrade"></param>
        /// <param name="RatedCT"></param>
        /// <param name="AmmeterCode"></param>
        /// <param name="ActualCT"></param>
        /// <param name="Power"></param>
        /// <param name="Unit"></param>
        /// <param name="PowerSupplyPosition"></param>
        /// <param name="Remarks"></param>
        /// <returns></returns>
        public static string SaveEquipment(string VariableId, string OrganizationID, string EquipmentName, int MonitorType,string PowerSupply, string VoltageGrade, 
            string RatedCT, string AmmeterCode, string ActualCT, int Power, string Unit,string Current,string PowerSupplyPosition, string Remarks)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string myCheckSql = @"SELECT * FROM system_EquipmentAccount where VariableId=@VariableId AND OrganizationID=@OrganizationID";
            SqlParameter[] m_checkParameters={new SqlParameter("@VariableId",VariableId),
                                             new SqlParameter("@OrganizationID",OrganizationID)};
            DataTable table = dataFactory.Query(myCheckSql, m_checkParameters);
            if (table.Rows.Count > 0)
            {
                return "该设备已经存在";
            }
            string mySql = @"INSERT INTO system_EquipmentAccount 
                                   (EquipmentItemId, VariableId, OrganizationID, EquipmentName, MonitorType, PowerSupply, VoltageGrade, 
                                   RatedCT, AmmeterCode, ActualCT, [Power], Unit, [Current],PowerSupplyPosition, Remarks)
                            values(@EquipmentItemId, @VariableId, @OrganizationID, @EquipmentName, @MonitorType, @PowerSupply, @VoltageGrade, 
                                   @RatedCT, @AmmeterCode, @ActualCT, @Power, @Unit,@Current,@PowerSupplyPosition, @Remarks)";
            Guid EquipmentItemId = Guid.NewGuid();
            SqlParameter[] m_Parameters = {new SqlParameter("@EquipmentItemId", EquipmentItemId),
                                              new SqlParameter("@VariableId", VariableId),
                                          new SqlParameter("@OrganizationID", OrganizationID),
                                          new SqlParameter("@EquipmentName", EquipmentName),
                                          new SqlParameter("@MonitorType", MonitorType),
                                          new SqlParameter("@PowerSupply", PowerSupply),
                                          new SqlParameter("@VoltageGrade", VoltageGrade),
                                          new SqlParameter("@RatedCT", RatedCT),
                                          new SqlParameter("@AmmeterCode", AmmeterCode),
                                          new SqlParameter("@ActualCT", ActualCT),
                                          new SqlParameter("@Power", Power),
                                          new SqlParameter("@Unit", Unit),
                                          new SqlParameter("@Current", Unit),
                                          new SqlParameter("@PowerSupplyPosition", PowerSupplyPosition),
                                          new SqlParameter("@Remarks", Remarks)};
            int num=dataFactory.ExecuteSQL(mySql, m_Parameters);
            if (num == 0)
            {
                return "保存失败";
            }
            else
            {
                return "保存成功";
            }
        }
        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="variableId"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static string RemoveEquipment(string variableId, string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string removeSql = @"DELETE FROM system_EquipmentAccount WHERE VariableId=@VariableId AND OrganizationID=@OrganizationID";
            SqlParameter[] removeParameters = { new SqlParameter("VariableId", variableId), new SqlParameter("OrganizationID", organizationId) };
            int n = dataFactory.ExecuteSQL(removeSql, removeParameters);
            string removeFormulaSql = @"DELETE FROM formula_FormulaDetail  
                                        WHERE VariableId=@VariableId AND KeyID=(select KeyID from tz_Material where OrganizationID=@OrganizationID)";
            SqlParameter[] removeFormulaParameters = { new SqlParameter("VariableId", variableId), new SqlParameter("OrganizationID", organizationId) };
            int m = dataFactory.ExecuteSQL(removeFormulaSql, removeFormulaParameters);
            if (0 == n)
                return "删除设备失败";
            else
                return "删除成功";

            
        }
        /// <summary>
        /// 更新设备信息
        /// </summary>
        /// <param name="variableId"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static string UpdateEquipment(string variableId_old, string organizationId_old, string VariableId, string OrganizationID, string EquipmentName, int MonitorType, string PowerSupply, string VoltageGrade,
            string RatedCT, string AmmeterCode, string ActualCT, int Power, string Unit,string Current, string PowerSupplyPosition, string Remarks)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string updateSql = @"UPDATE system_EquipmentAccount 
                                    SET 
                                    VariableId=@VariableId, OrganizationID=@OrganizationID,EquipmentName=@EquipmentName,MonitorType=@MonitorType,
                                    PowerSupply=@PowerSupply,VoltageGrade=@VoltageGrade,RatedCT=@RatedCT,AmmeterCode=@AmmeterCode,ActualCT=@ActualCT,
                                    [Power]=@Power,Unit=@Unit,[Current]=@Current,PowerSupplyPosition=@PowerSupplyPosition,Remarks=@Remarks
                                    WHERE
                                    VariableId=@VariableId_old AND OrganizationID=@OrganizationID_old";
            SqlParameter[] m_Parameters = {new SqlParameter("@VariableId_old", variableId_old),
                                              new SqlParameter("@OrganizationID_old", organizationId_old),
                                              new SqlParameter("@VariableId", VariableId),
                                          new SqlParameter("@OrganizationID", OrganizationID),
                                          new SqlParameter("@EquipmentName", EquipmentName),
                                          new SqlParameter("@MonitorType", MonitorType),
                                          new SqlParameter("@PowerSupply", PowerSupply),
                                          new SqlParameter("@VoltageGrade", VoltageGrade),
                                          new SqlParameter("@RatedCT", RatedCT),
                                          new SqlParameter("@AmmeterCode", AmmeterCode),
                                          new SqlParameter("@ActualCT", ActualCT),
                                          new SqlParameter("@Power", Power),
                                          new SqlParameter("@Unit", Unit),
                                          new SqlParameter("@Current", Current),
                                          new SqlParameter("@PowerSupplyPosition", PowerSupplyPosition),
                                          new SqlParameter("@Remarks", Remarks)};
            int n = dataFactory.ExecuteSQL(updateSql, m_Parameters);
            if (0 == n)
                return "更新数据失败";
            else
                return "更新数据成功";
        }
    }
}
