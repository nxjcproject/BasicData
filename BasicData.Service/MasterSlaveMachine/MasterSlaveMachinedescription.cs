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

namespace BasicData.Service.MasterSlaveMachine
{
    public class MasterSlaveMachinedescription
    {
        private static readonly string _connStr = ConnectionStringFactory.NXJCConnectionString;
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connStr);
        private static readonly BasicDataHelper _dataHelper = new BasicDataHelper(_connStr);

        public static DataTable GetMasterMachineInfo(string myDcsId)
        {
            string m_Sql = @"Select
                    A.ID as Id,  
                    A.OrganizationID as OrganizationId, 
                    B.Name as OrganizationName,  
                    A.VariableId as VariableId, 
                    A.VariableName as VariableName,
                    A.VariableDescription as VariableDescription,
                    A.DataBaseName as DataBaseName, 
                    A.TableName as TableName, 
                    A.Record as Record,
                    A.ValidValues as ValidValues,
                    A.Remarks as Remarks,
					A.KeyID as KeyId  
                    from system_MasterMachineDescription A 
                    left join system_Organization B on A.OrganizationID = B.OrganizationID
					where A.OrganizationID=@OrganizationId";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@OrganizationId", myDcsId) };
                DataTable m_Result = _dataFactory.Query(m_Sql, m_Parameters);
                return m_Result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static DataTable GetMasterMachineInfobyId(string myId)
        {
            string m_Sql = @"Select
                    A.ID as Id,  
                    A.OrganizationID as OrganizationId, 
                    B.Name as OrganizationName, 
                    A.VariableId as VariableId, 
                    A.VariableName as VariableName,
                    A.VariableDescription as VariableDescription,
                    A.DataBaseName as DataBaseName, 
                    A.TableName as TableName, 
                    A.Record as Record,
                    A.ValidValues as ValidValues,
                    A.Remarks as Remarks,
					A.KeyID as KeyId  
                    from system_MasterMachineDescription A 
                    left join system_Organization B on A.OrganizationID = B.OrganizationID
					where A.ID=@Id";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@Id", myId) };
                DataTable m_Result = _dataFactory.Query(m_Sql, m_Parameters);
                return m_Result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static int AddMasterMachineInfo(string myOrganizationId, string myVariableId, string myVariableName, string myVariableDescription, string myDataBaseName, string myTableName, string myRecord, string myValidValues, string myRemarks)
        {
            string m_Sql = @" Insert into system_MasterMachineDescription 
                ( OrganizationID, VariableId, VariableName, VariableDescription, DataBaseName, TableName, Record, ValidValues, Remarks) 
                values
                (@OrganizationID,@VariableId,@VariableName,@VariableDescription,@DataBaseName,@TableName,@Record,@ValidValues,@Remarks)";
            SqlParameter[] m_Parameters = { new SqlParameter("@OrganizationID", myOrganizationId),
                                          new SqlParameter("@VariableId", myVariableId),
                                          new SqlParameter("@VariableName", myVariableName),
                                          new SqlParameter("@VariableDescription", myVariableDescription),
                                          new SqlParameter("@DataBaseName", myDataBaseName),
                                          new SqlParameter("@TableName", myTableName),
                                          new SqlParameter("@Record", myRecord),
                                          new SqlParameter("@ValidValues", myValidValues),
                                          new SqlParameter("@Remarks", myRemarks)};
            try
            {
                return _dataFactory.ExecuteSQL(m_Sql, m_Parameters);
            }
            catch (Exception e)
            {
                var m_e = e;
                return -1;
            }
        }
        public static int ModifyMasterMachineInfo(string myId, string myOrganizationId, string myVariableId, string myVariableName, string myVariableDescription, string myDataBaseName, string myTableName, string myRecord, string myValidValues, string myRemarks)
        {
            string m_Sql = @"UPDATE system_MasterMachineDescription SET 
                            OrganizationID=@OrganizationID, 
                            VariableId=@VariableId,
                            VariableName=@VariableName, 
                            VariableDescription=@VariableDescription, 
                            DataBaseName=@DataBaseName,
                            TableName=@TableName, 
                            Record=@Record, 
                            ValidValues=@ValidValues, 
                            Remarks=@Remarks
                            where ID=@ID";
            SqlParameter[] m_Parameters = {new SqlParameter("@ID", myId),
                                          new SqlParameter("@OrganizationID", myOrganizationId),
                                          new SqlParameter("@VariableId", myVariableId),
                                          new SqlParameter("@VariableName", myVariableName),
                                          new SqlParameter("@VariableDescription", myVariableDescription),                       
                                          new SqlParameter("@DataBaseName", myDataBaseName),
                                          new SqlParameter("@TableName", myTableName),
                                          new SqlParameter("@Record", myRecord),
                                          new SqlParameter("@ValidValues", myValidValues),
                                          new SqlParameter("@Remarks", myRemarks)};
            try
            {
                return _dataFactory.ExecuteSQL(m_Sql, m_Parameters);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public static int DeleteMasterMachineInfo(string myId)
        {
            string m_Sql = @"DELETE FROM system_MasterMachineDescription where ID=@ID";
            SqlParameter[] m_Parameters = { new SqlParameter("@ID", myId) };
            try
            {
                return _dataFactory.ExecuteSQL(m_Sql, m_Parameters);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public static DataTable GetMainMachineInfo(List<string> myOrganizationIds)
        {
            List<string> m_LevelCodeList = _dataHelper.GetOrganisationLevelCodeById(myOrganizationIds);

            string m_Sql = @"Select 
                                A.OrganizationID as OrganizationID,
                                A.Name + B.Name as Name,
                                A.OrganizationID + '>' + B.VariableId as VariableId,
                                C.LevelCode +  SUBSTRING(B.LevelCode,2,LEN(B.LevelCode) - 1) as LevelCode,
                                B.LevelType as LevelType
                                from tz_Formula A, formula_FormulaDetail B, system_Organization C
                                where A.KeyID = B.KeyID
                                and A.OrganizationID in (C.OrganizationID)
                                and A.Type = 2 
                                and A.ENABLE = 1
                                and A.State = 0
                                {0}
                             union all
                             Select 
                                D.OrganizationID as OrganizationID, 
                                D.Name as Name,
                                '' as VariableId,
                                D.LevelCode as LevelCode, 
                                '' as LevelType 
                                from system_Organization D
                                where D.OrganizationID = D.OrganizationID 
                                {1}";
            if (m_LevelCodeList != null)
            {
                string m_ConditionTemp = "";   //" C.LevelCode like '{0}%' ";
                string m_ConditionTemp1 = "";   //" C.LevelCode like '{0}%' ";
                for (int i = 0; i < m_LevelCodeList.Count; i++)
                {
                    if (i == 0)
                    {
                        m_ConditionTemp = " and ( " + string.Format("C.LevelCode like '{0}%'", m_LevelCodeList[i]);
                        m_ConditionTemp1 = " and ( " + string.Format("D.LevelCode like '{0}%' or CHARINDEX(D.LevelCode, '{0}') > 0 ", m_LevelCodeList[i]);
                    }
                    else
                    {
                        m_ConditionTemp = m_ConditionTemp + " or " + string.Format("C.LevelCode like '{0}%'", m_LevelCodeList[i]);
                        m_ConditionTemp1 = m_ConditionTemp1 + " or " + string.Format("D.LevelCode like '{0}%' or CHARINDEX(D.LevelCode, '{0}') > 0", m_LevelCodeList[i]);
                    }
                }
                m_ConditionTemp = m_ConditionTemp + " )";
                m_ConditionTemp1 = m_ConditionTemp1 + " )";
                m_Sql = string.Format(m_Sql, m_ConditionTemp, m_ConditionTemp1);
            }
            else
            {
                m_Sql = string.Format(m_Sql, " and C.OrganizationID = ''", " and D.OrganizationID = ''");
            }

            try
            {
                return _dataFactory.Query(m_Sql);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DataTable GetSlaveMachineInfo(string myKeyId)
        {
            string m_Sql = @"Select
                    A.ID as Id,  
                    A.OrganizationID as OrganizationId, 
                    B.Name as OrganizationName, 
                    A.KeyID as KeyId, 
                    C.VariableDescription as KeyName, 
                    A.VariableName as VariableName,
                    A.VariableDescription as VariableDescription,
                    A.DataBaseName as DataBaseName, 
                    A.TableName as TableName, 
                    A.ValidValues as ValidValues,
                    A.TimeDelay as TimeDelay,
                    A.Remarks as Remarks
                    from system_SlaveMachineDescription A 
                    left join system_Organization B on A.OrganizationID = B.OrganizationID 
                    left join system_MasterMachineDescription C on A.KeyID = C.ID
					where A.KeyID=@KeyId";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@KeyId", myKeyId) };
                DataTable m_Result = _dataFactory.Query(m_Sql, m_Parameters);
                return m_Result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static DataTable GetSlaveMachineInfobyId(string myId)
        {
            string m_Sql = @"Select
                    A.ID as Id,  
                    A.OrganizationID as OrganizationId, 
                    B.Name as OrganizationName, 
                    A.KeyID as KeyId, 
                    C.VariableDescription as KeyName, 
                    A.VariableName as VariableName,
                    A.VariableDescription as VariableDescription,
                    A.DataBaseName as DataBaseName, 
                    A.TableName as TableName, 
                    A.ValidValues as ValidValues,
                    A.TimeDelay as TimeDelay,
                    A.Remarks as Remarks
                    from system_SlaveMachineDescription A 
                    left join system_Organization B on A.OrganizationID = B.OrganizationID 
                    left join system_MasterMachineDescription C on A.KeyID = C.ID
					where A.ID=@Id";
            try
            {
                SqlParameter[] m_Parameters = { new SqlParameter("@Id", myId) };
                DataTable m_Result = _dataFactory.Query(m_Sql, m_Parameters);
                return m_Result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static int AddSlaveMachineInfo(string myOrganizationId, string myKeyId, string myVariableName, string myVariableDescription, string myDataBaseName, string myTableName, string myValidValues, string myTimeDelay, string myRemarks)
        {
            string m_Sql = @" Insert into system_SlaveMachineDescription 
                ( OrganizationID, KeyID, VariableName, VariableDescription, DataBaseName, TableName, ValidValues, TimeDelay, Remarks) 
                values
                (@OrganizationID, @KeyID, @VariableName, @VariableDescription, @DataBaseName, @TableName, @ValidValues, @TimeDelay, @Remarks)";
            SqlParameter[] m_Parameters = { new SqlParameter("@OrganizationID", myOrganizationId),
                                          new SqlParameter("@KeyID", myKeyId),
                                          new SqlParameter("@VariableName", myVariableName),
                                          new SqlParameter("@VariableDescription", myVariableDescription),
                                          new SqlParameter("@DataBaseName", myDataBaseName),
                                          new SqlParameter("@TableName", myTableName),
                                          new SqlParameter("@ValidValues", myValidValues),
                                          new SqlParameter("@TimeDelay", myTimeDelay),
                                          new SqlParameter("@Remarks", myRemarks)};
            try
            {
                return _dataFactory.ExecuteSQL(m_Sql, m_Parameters);
            }
            catch (Exception e)
            {
                var m_e = e;
                return -1;
            }
        }
        public static int ModifySlaveMachineInfo(string myId, string myOrganizationId, string myKeyId, string myVariableName, string myVariableDescription, string myDataBaseName, string myTableName, string myValidValues, string myTimeDelay, string myRemarks)
        {
            string m_Sql = @"UPDATE system_SlaveMachineDescription SET              
                OrganizationID=@OrganizationID, 
                KeyID=@KeyID, 
                VariableName=@VariableName, 
                VariableDescription=@VariableDescription, 
                DataBaseName=@DataBaseName,
                TableName=@TableName,
                ValidValues=@ValidValues, 
                TimeDelay=@TimeDelay, 
                Remarks=@Remarks 
                where ID=@ID";
            SqlParameter[] m_Parameters = {new SqlParameter("@ID", myId),
                                          new SqlParameter("@OrganizationID", myOrganizationId),
                                          new SqlParameter("@KeyID", myKeyId),
                                          new SqlParameter("@VariableName", myVariableName),
                                          new SqlParameter("@VariableDescription", myVariableDescription),
                                          new SqlParameter("@DataBaseName", myDataBaseName),
                                          new SqlParameter("@TableName", myTableName),
                                          new SqlParameter("@ValidValues", myValidValues),
                                          new SqlParameter("@TimeDelay", myTimeDelay),
                                          new SqlParameter("@Remarks", myRemarks)};
            try
            {
                return _dataFactory.ExecuteSQL(m_Sql, m_Parameters);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public static int DeleteSlaveMachineInfo(string myId)
        {
            string m_Sql = @"DELETE FROM system_SlaveMachineDescription where ID=@ID";
            SqlParameter[] m_Parameters = { new SqlParameter("@ID", myId) };
            try
            {
                return _dataFactory.ExecuteSQL(m_Sql, m_Parameters);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public static int DeleteAllSlaveMachineInfoByKeyId(string myKeyId)
        {
            string m_Sql = @"DELETE FROM system_SlaveMachineDescription where KeyID=@KeyID";
            SqlParameter[] m_Parameters = { new SqlParameter("@KeyID", myKeyId) };
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
