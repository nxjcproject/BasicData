using BasicData.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BasicData.Service.KpiDefinition
{
    public class KPIndexService
    {
        public static DataTable GetStandardTypeTable()
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select StandardName,DisplayIndex as id,StandardId as value from [dbo].[analyse_KPI_Standard]
                            group by StandardName,DisplayIndex,StandardId 
                            order by DisplayIndex";
            DataTable table = dataFactory.Query(mySql);
            return table;
        }
        public static DataTable GetObjectNameListTable() 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select distinct (case when B.LevelType = 'ProductionLine' then C.Type + B.Name else B.Name end) as Name, B.VariableId from tz_Formula A, formula_FormulaDetail B, system_Organization C
                                where A.ENABLE  = 1
                                and A.State = 0
                                and A.Type = 2
                                and A.KeyID = B.KeyID
                                and (B.LevelType = 'ProductionLine' or B.LevelType = 'Process')
                                and A.OrganizationID = C.OrganizationID
                                and C.LevelType = 'ProductionLine'
                                and C.LevelCode like 'O0201%'
                                order by B.VariableId";
            DataTable table = dataFactory.Query(mySql);
            return table;        
        }
        public static DataTable GetConsumpationTypeTable(string myStandardId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select CASE WHEN StatisticalMethod = 'Comprehensive' THEN '综合能耗'
                            WHEN StatisticalMethod = 'Entity' THEN '标准能耗'
                            ELSE '其他' END as Consumpation,KeyId, Row_Number() over ( order by StatisticalMethod ) as id 
                            from [dbo].[analyse_KPI_Standard]
                            where StandardId=@StandardId";
            SqlParameter Param = new SqlParameter("@StandardId", myStandardId);
            DataTable table = dataFactory.Query(mySql, Param);
            return table;
        }
        public static DataTable GetIndexDataTable(string myConsumpation, string mykeyId, string myStandardType, string myInOutSide)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = "";
            if (myInOutSide == "Inside")
            {
                mySql = @"select t3.Name as OrganizationName,t2.Name as VariableName,t1.*from [analyse_KPI_Standard_Detail] t1,	
                        (select distinct (case when B.LevelType = 'ProductionLine' then C.Type + B.Name else B.Name end) as Name, B.VariableId from tz_Formula A, formula_FormulaDetail B, system_Organization C
	                        where A.ENABLE  = 1
	                        and A.State = 0
	                        and A.Type = 2
	                        and A.KeyID = B.KeyID
	                        and (B.LevelType = 'ProductionLine' or B.LevelType = 'Process')
	                        and A.OrganizationID = C.OrganizationID
	                        and C.LevelType = 'ProductionLine'
	                        and C.Type <> '余热发电'
	                        and C.LevelCode like 'O0201%')t2
	                        ,[dbo].[system_Organization] t3	
                        where  t1.KeyId=@KeyId
	                        and t1.StandardType=@myStandardType
	                        and t1.Enabled=1
	                        and t1.VariableId=t2.VariableId
	                        and t1.OrganizationID=t3.OrganizationID";
            }
            else if (myInOutSide == "Outside")
            {
                mySql = @"select t2.Name as VariableName,t1.*from [analyse_KPI_Standard_Detail] t1,	
                        (select distinct (case when B.LevelType = 'ProductionLine' then C.Type + B.Name else B.Name end) as Name, B.VariableId from tz_Formula A, formula_FormulaDetail B, system_Organization C
	                        where A.ENABLE  = 1
	                        and A.State = 0
	                        and A.Type = 2
	                        and A.KeyID = B.KeyID
	                        and (B.LevelType = 'ProductionLine' or B.LevelType = 'Process')
	                        and A.OrganizationID = C.OrganizationID
	                        and C.LevelType = 'ProductionLine'
	                        and C.Type <> '余热发电'
	                        and C.LevelCode like 'O0201%')t2
                        where  t1.KeyId=@KeyId
	                        and t1.StandardType=@myStandardType
	                        and t1.Enabled=1
                            and t1.OrganizationID is null
	                        and t1.VariableId=t2.VariableId";
            }
            SqlParameter[] Param = { new SqlParameter("@KeyId", mykeyId), new SqlParameter("@myStandardType", myStandardType) };
            DataTable table = dataFactory.Query(mySql, Param);
            return table;
        }
        public static DataTable GetKPIndexOrganizationTable()
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select CASE WHEN StatisticalMethod = 'Comprehensive' THEN '综合能耗'
                            WHEN StatisticalMethod = 'Entity' THEN '标准能耗'
                            ELSE '其他' END as Consumpation,KeyId, Row_Number() over ( order by StatisticalMethod ) as id 
                            from [dbo].[analyse_KPI_Standard]
                            where StandardId=@StandardId";
            DataTable table = dataFactory.Query(mySql);
            return table;
        }
        public static DataTable GetNameListTable(string myOrganization)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mysql = @"select LevelCode from [dbo].[system_Organization]
                        where OrganizationID=@myOrganization";
            SqlParameter para = new SqlParameter("@myOrganization", myOrganization);
            DataTable t_LeverCode = dataFactory.Query(mysql, para);
            string myCode = t_LeverCode.Rows[0][0].ToString().Trim();
            string mySql = @"select distinct (case when B.LevelType = 'ProductionLine' then C.Type + B.Name else B.Name end) as Name, B.VariableId,
                row_number() over(order by B.VariableId) as id
                from tz_Formula A, formula_FormulaDetail B, system_Organization C
                where A.ENABLE  = 1
                and A.State = 0
                and A.Type = 2
                and A.KeyID = B.KeyID
                and (B.LevelType = 'ProductionLine' or B.LevelType = 'Process')
                and A.OrganizationID = C.OrganizationID
                and C.LevelType = 'ProductionLine'
                --and C.Type <> '余热发电'
                and C.LevelCode like '" + myCode + @"%'
                group by B.Name,B.VariableId,C.Type,B.LevelType
                order by B.VariableId";
            DataTable table = dataFactory.Query(mySql);
            return table;
        }
        public static void AddKPIList(string mStandardLevel, string mEnergyType, string mObjName, string mOrganizationId, string mLevelType, string mVariableId, string mValueType, string mtype, string mUnit, float mStandardValue, string mLevel, string mCreator)
        {
            //先获取KeyId
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mysql = @"select KeyId from [dbo].[analyse_KPI_Standard]
                                  where StandardId=@mStandardLevel
                                   and StatisticalMethod=@mEnergyType";
            SqlParameter[] para = {new SqlParameter("mStandardLevel", mStandardLevel),
                                  new SqlParameter("mEnergyType", mEnergyType)};
            DataTable t_KeyId = dataFactory.Query(mysql, para);
            string m_KeyId=t_KeyId.Rows[0][0].ToString().Trim();
            //写入数据库
            ISqlServerDataFactory SqldataFactory = new SqlServerDataFactory(connectionString);
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
                                           ,'Energy'
                                           ,@Unit
                                           ,@StandardValue
                                           ,@StandardLevel
                                           ,@Creator
                                           ,getdate()
                                           ,1)";

            SqlParameter[] Para = {new SqlParameter("@KeyId", m_KeyId),
                                  new SqlParameter("@Name", mObjName),
                                  new SqlParameter("@OrganizationID", mOrganizationId),
                                  new SqlParameter("@LevelType", mLevelType),
                                  new SqlParameter("@VariableId", mVariableId),
                                  new SqlParameter("@ValueType", mValueType),
                                  new SqlParameter("@StandardType", mtype),
                                  new SqlParameter("@Unit", mUnit),
                                  new SqlParameter("@StandardValue", mStandardValue),
                                  new SqlParameter("@StandardLevel", mLevel),
                                  new SqlParameter("@Creator", mCreator)};
            if (mOrganizationId=="")
            { Para[2] = new SqlParameter("@OrganizationID",DBNull.Value); }
            SqldataFactory.ExecuteSQL(mySql, Para);
        }
        //"{mStandardLevel:'InternationalStandard',mEnergyType:'Comprehensive',mObjName:'test',mOrganizationId:'zc_nxjc_byc_byf_cementmill01',mLevelType:'Process',
                                           //mVariableId:'cementGrind',mValueType:'ElectricityConsumption',mtype:'Energy',mUnit:'kW·h/t',mStandardValue:''}"
        public static void DeleteKPIndex(string standardItemId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string Deletesql = @"DELETE FROM [analyse_KPI_Standard_Detail]
                                              WHERE [StandardItemId] = @standardItemId";
            SqlParameter para = new SqlParameter("@standardItemId", standardItemId);
            dataFactory.ExecuteSQL(Deletesql, para);
        }
    }
}
