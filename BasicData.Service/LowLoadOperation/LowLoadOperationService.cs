using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using BasicData.Infrastructure.Configuration;
namespace BasicData.Service.LowLoadOperation
{
    public class LowLoadOperationService
    {
        public static DataTable LowLoadOperation(string mOrganizationId, string variableDesc)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = "";
            if (variableDesc !="")
            {
                  mySql = @"SELECT A.[ID]
                              ,A.[OrganizationID]
                              ,A.[VariableName]
                              ,A.[VariableDescription]
                              ,A.[RunTag]
                              ,A.[RunTagTableName]
                              ,A.[RunTagDataBaseName]
                              ,A.[Record]
                              ,A.[ValidValues]
                              ,A.[DelayTime]
                              ,A.[LoadTag]
                              ,A.[DCSLoadTag]
                              ,A.[DCSLoadTableName]
                              ,A.[DCSLoadDataBaseName]
                              ,A.[LoadTagType]
                              ,A.[LLoadLimit]
                              ,A.[Editor]
                              ,B.[USER_NAME]
                              ,A.[EditTime]
                              ,A.[Remark]
                          FROM [equipment_LowLoadOperationConfig] A,[IndustryEnergy_SH].[dbo].[users] B
                          where A.[OrganizationID]=@mOrganizationId
                                and A.[Editor]=B.[USER_ID]
                                and A.[VariableDescription] like '%' + @variableDesc + '%'
                                order by LoadRunTag";
            }
            else
            {
                mySql = @"SELECT  A.[ID]
                              ,A.[OrganizationID]
                              ,A.[VariableName]
                              ,A.[VariableDescription]
                              ,A.[RunTag]
                              ,A.[RunTagTableName]
                              ,A.[RunTagDataBaseName]
                              ,A.[Record]
                              ,A.[ValidValues]
                              ,A.[DelayTime]
                              ,A.[LoadTag]
                              ,A.[DCSLoadTag]
                              ,A.[DCSLoadTableName]
                              ,A.[DCSLoadDataBaseName]
                              ,A.[LoadTagType]
                              ,A.[LLoadLimit]
                              ,A.[Editor]
                              ,B.[USER_NAME]
                              ,A.[EditTime]
                              ,A.[Remark]
                          FROM [equipment_LowLoadOperationConfig] A,[IndustryEnergy_SH].[dbo].[users] B
                          where A.[OrganizationID]=@mOrganizationId
                                and A.[Editor]=B.[USER_ID]";
            }
            SqlParameter[] sqlParameter = {
                                              new SqlParameter("@mOrganizationId", mOrganizationId),
                                              new SqlParameter("@variableDesc", variableDesc)
                                          };
            DataTable table = dataFactory.Query(mySql, sqlParameter);
            return table;
        }
        public static DataTable GetMainMachineInfo(string myOrganizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_Sql = @"Select B.EquipmentCommonId as EquipmentId
                                  ,B.Name as Name
                                  ,'0' as EquipmentCommonId
                                  ,'' as VariableId
                                  ,'' as OrganizationId
                                  ,'' as OutputFormula
	                              ,B.DisplayIndex as DisplayIndex
                              FROM equipment_EquipmentCommonInfo B
                              where B.EquipmentCommonId in 
                              (select distinct A.EquipmentCommonId 
                              from equipment_EquipmentDetail A, analyse_KPI_OrganizationContrast C
                              where A.Enabled = 1 
                              and A.OrganizationID = C.FactoryOrganizationID
                              and C.OrganizationID = '{0}')
                              union all 
                              Select UPPER(A.EquipmentId) as EquipmentId
                                  ,A.EquipmentName as Name
                                  ,A.EquipmentCommonId as EquipmentCommonId
                                  ,A.VariableId as VariableId
                                  ,A.OrganizationId as OrganizationId
                                  ,A.OutputFormula as OutputFormula
	                              ,A.DisplayIndex as DisplayIndex
                              FROM equipment_EquipmentDetail A, analyse_KPI_OrganizationContrast C
                              where A.Enabled = 1
                              and A.OrganizationID = C.FactoryOrganizationID
                              and C.OrganizationID = '{0}'
                              order by DisplayIndex";
            m_Sql = string.Format(m_Sql, myOrganizationId);
            try
            {
                return dataFactory.Query(m_Sql);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static int AddLowLoadOperationConfig(string mOrganizationId, string m_VariableDesc, string m_RunTag, string MasterTableName, string MasterDataBaseName, string m_Record, string m_ValidValues, string m_DelayTime, string m_LoadTag, string m_DCSLoadTag, string s_MasterTableName, string s_MasterDataBaseName, string m_LoadTagType, string m_LLoadLimit, string m_UserId, string m_Remark)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"INSERT INTO [dbo].[equipment_LowLoadOperationConfig]
                                    ([ID]
                                    ,[OrganizationID]
                                    ,[VariableDescription]
                                    ,[RunTag]
                                    ,[RunTagTableName]
                                    ,[RunTagDataBaseName]
                                    ,[Record]
                                    ,[ValidValues]
                                    ,[DelayTime]
                                    ,[LoadTag]
                                    ,[DCSLoadTag]
                                    ,[DCSLoadTableName]
                                    ,[DCSLoadDataBaseName]
                                    ,[LoadTagType]
                                    ,[LLoadLimit]
                                    ,[Editor]
                                    ,[EditTime]
                                    ,[Remark])
                             VALUES
                                   (@mItemId
                                   ,@mOrganizationId
                                   ,@m_VariableDesc
                                   ,@m_RunTag
                                   ,@MasterTableName
                                   ,@MasterDataBaseName
                                   ,@m_Record
                                   ,@m_ValidValues
                                   ,@m_DelayTime
                                   ,@m_LoadTag
                                   ,@m_DCSLoadTag
                                   ,@s_MasterTableName
                                   ,@s_MasterDataBaseName
                                   ,@m_LoadTagType
                                   ,@m_LLoadLimit
                                   ,@m_UserId
                                   ,@m_EditTime
                                   ,@m_Remark)";
            SqlParameter[] para = { new SqlParameter("@mItemId",System.Guid.NewGuid().ToString()),
                                    new SqlParameter("@mOrganizationId",mOrganizationId),
                                    new SqlParameter("@m_VariableDesc", m_VariableDesc),
                                    new SqlParameter("@m_RunTag", m_RunTag),
                                    new SqlParameter("@MasterTableName",  MasterTableName),
                                    new SqlParameter("@MasterDataBaseName", MasterDataBaseName),
                                    new SqlParameter("@m_Record",m_Record),
                                    new SqlParameter("@m_ValidValues",m_ValidValues),
                                    new SqlParameter("@m_DelayTime", m_DelayTime),
                                    new SqlParameter("@m_LoadTag", m_LoadTag),
                                    new SqlParameter("@m_DCSLoadTag",  m_DCSLoadTag),
                                    new SqlParameter("@s_MasterTableName", s_MasterTableName),
                                    new SqlParameter("@s_MasterDataBaseName", s_MasterDataBaseName),
                                    new SqlParameter("@m_LoadTagType", m_LoadTagType),
                                    new SqlParameter("@m_LLoadLimit",  m_LLoadLimit),
                                    new SqlParameter("@m_UserId", m_UserId),
                                    new SqlParameter("@m_EditTime", System.DateTime.Now.ToString()),
                                    new SqlParameter("@m_Remark", m_Remark)
                                  };
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;
        }
        public static int EditLowLoadOperationConfig(string m_Id, string mOrganizationId, string m_VariableDesc, string m_RunTag, string MasterTableName, string MasterDataBaseName, string m_Record, string m_ValidValues, string m_DelayTime, string m_LoadTag, string m_DCSLoadTag, string s_MasterTableName, string s_MasterDataBaseName, string m_LoadTagType, string m_LLoadLimit, string m_UserId, string m_Remark)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"UPDATE [dbo].[equipment_LowLoadOperationConfig]
                           SET   [ID]=@mItemId
                                ,[OrganizationID]=@mOrganizationId
                                ,[VariableDescription]=@m_VariableDesc
                                ,[RunTag]=@m_RunTag
                                ,[RunTagTableName]=@MasterTableName
                                ,[RunTagDataBaseName]=@MasterDataBaseName
                                ,[Record]=@m_Record
                                ,[ValidValues]=@m_ValidValues
                                ,[DelayTime]=@m_DelayTime
                                ,[LoadTag]=@m_LoadTag
                                ,[DCSLoadTag]=@m_DCSLoadTag
                                ,[DCSLoadTableName]=@s_MasterTableName
                                ,[DCSLoadDataBaseName]=@s_MasterDataBaseName
                                ,[LoadTagType]=@m_LoadTagType
                                ,[LLoadLimit]=@m_LLoadLimit
                                ,[Editor]=@m_UserId
                                ,[EditTime]=@m_EditTime
                                ,[Remark]=@m_Remark
                         WHERE [ID] = @mItemId";
            SqlParameter[] para = { new SqlParameter("@mItemId",m_Id),
                                    new SqlParameter("@mOrganizationId",mOrganizationId),
                                    new SqlParameter("@m_VariableDesc", m_VariableDesc),
                                    new SqlParameter("@m_RunTag", m_RunTag),
                                    new SqlParameter("@MasterTableName",  MasterTableName),
                                    new SqlParameter("@MasterDataBaseName", MasterDataBaseName),
                                    new SqlParameter("@m_Record",m_Record),
                                    new SqlParameter("@m_ValidValues",m_ValidValues),
                                    new SqlParameter("@m_DelayTime", m_DelayTime),
                                    new SqlParameter("@m_LoadTag", m_LoadTag),
                                    new SqlParameter("@m_DCSLoadTag",  m_DCSLoadTag),
                                    new SqlParameter("@s_MasterTableName", s_MasterTableName),
                                    new SqlParameter("@s_MasterDataBaseName", s_MasterDataBaseName),
                                    new SqlParameter("@m_LoadTagType", m_LoadTagType),
                                    new SqlParameter("@m_LLoadLimit",  m_LLoadLimit),
                                    new SqlParameter("@m_UserId", m_UserId),
                                    new SqlParameter("@m_EditTime", System.DateTime.Now.ToString()),
                                    new SqlParameter("@m_Remark", m_Remark)
                                  };
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;
        }
        public static int deleteLowLoadOperationConfig(string m_Id)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"delete from [dbo].[equipment_LowLoadOperationConfig]
                         WHERE [ID] =@mItemId";
            SqlParameter para = new SqlParameter("@mItemId", m_Id);
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;
        }
    }
}
