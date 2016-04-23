using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BasicData.Service.KpiDefinition
{
    public class ProdunctionService
    {
        public static DataTable GetProductionIndexData(string mStandardId, string myInOutSide)
        {
            string mKeyId = GetKeyId(mStandardId);

            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory= new SqlServerDataFactory(connectionString);

            string mysql = "";
            if (myInOutSide == "Inside")
            {
                mysql = @"select C.Name as OrganizationName,B.Name as VariableName,A.* from [dbo].[analyse_KPI_Standard_Detail] A,[dbo].[equipment_EquipmentCommonInfo] B,[dbo].[system_Organization] C
                                where KeyId=@KeyId
                                and StandardType='Production'
                                and A.VariableId=B.EquipmentCommonId
                                and A.OrganizationID=C.OrganizationID";
            }
            else if (myInOutSide == "Outside")
            {
                mysql = @"select B.Name as VariableName,A.* from [dbo].[analyse_KPI_Standard_Detail] A,[dbo].[equipment_EquipmentCommonInfo] B
                            where KeyId=@KeyId
                            and StandardType='Production'
                            and A.VariableId=B.EquipmentCommonId
                            and A.OrganizationID is null";          
            }           
            SqlParameter param = new SqlParameter("@KeyId", mKeyId);
            DataTable table = dataFactory.Query(mysql, param);
            return table;
        }
        public static DataTable GetEquipmentCommonNameList()
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mysql = @"select EquipmentCommonId as VariableId,Name 
                          from [dbo].[equipment_EquipmentCommonInfo]";
            DataTable table = dataFactory.Query(mysql);
            return table;
        }
        public static void AddKPIndexProductionList(string mStandardLevel, string mObjName, string mOrganizationId, string mLevelType, string mVariableId,
            string mValueType, string mUnit, float mStandardValue, int mLevel, string mCreator) 
        {
            string mKeyId = GetKeyId(mStandardLevel);
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"INSERT INTO [dbo].[analyse_KPI_Standard_Detail]
                                           ([KeyId]
                                           ,[Name]
                                           ,[OrganizationID]
                                           ,[LevelType]
                                           ,[VariableId]
                                           ,[ValueType]
                                           ,[StandardType]
                                           ,[Unit]
                                           ,[StandardValue]
                                           ,[StandardLevel]
                                           ,[Creator]
                                           ,[CreateTime]
                                           ,[Enabled])
                                     VALUES
                                           (@KeyId
                                           ,@Name
                                           ,@OrganizationID
                                           ,@LevelType
                                           ,@VariableId
                                           ,@ValueType
                                           ,'Production'
                                           ,@Unit
                                           ,@StandardValue
                                           ,@StandardLevel
                                           ,@Creator
                                           ,getdate()
                                           ,1)";

            SqlParameter[] Para = {new SqlParameter("@KeyId", mKeyId),
                                  new SqlParameter("@Name", mObjName),
                                  new SqlParameter("@OrganizationID", mOrganizationId),
                                  new SqlParameter("@LevelType", mLevelType),
                                  new SqlParameter("@VariableId", mVariableId),
                                  new SqlParameter("@ValueType", mValueType),
                                  new SqlParameter("@Unit", mUnit),
                                  new SqlParameter("@StandardValue", mStandardValue),
                                  new SqlParameter("@StandardLevel", mLevel),
                                  new SqlParameter("@Creator", mCreator)};
            if (mOrganizationId == "")
            { Para[2] = new SqlParameter("@OrganizationID", DBNull.Value); }
            dataFactory.ExecuteSQL(mySql, Para);
        }
        private static string GetKeyId(string mStandardId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dF_KeyId = new SqlServerDataFactory(connectionString);
            string sql_KeyId = @"select KeyId from [dbo].[analyse_KPI_Standard]
                                 where [StandardId]=@StandardId
                                   and [StatisticalMethod]='Entity'";
            SqlParameter para_KeyId = new SqlParameter("@StandardId", mStandardId);
            DataTable t_Key = dF_KeyId.Query(sql_KeyId, para_KeyId);
            string str=t_Key.Rows[0][0].ToString().Trim();
            return str; 
        }
    }
}
