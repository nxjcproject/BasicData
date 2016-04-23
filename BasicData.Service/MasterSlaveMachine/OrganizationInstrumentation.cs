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

    public class OrganizationInstrumentation
    {
        private static readonly string _connStr = ConnectionStringFactory.NXJCConnectionString;
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connStr);
        private static readonly BasicDataHelper _dataHelper = new BasicDataHelper(_connStr);
        public static DataTable GetDcsOrganization(List<string> myOrganizations)
        {
             string m_Sql = @"Select 
                    A.OrganizationID as OrganizationId, 
                    A.Name as Name,
                    A.LevelCode as LevelCode,
					B.DCSProcessDatabase as DcsProcessDatabase  
                    from system_Organization_Instrumentation A 
					left join system_Database B on A.DatabaseID = B.DatabaseID 
					where A.Enabled = 1 
                    and len(A.LevelCode) <= 7 
                    and {0} ";
            string m_SqlConditionTemp = @" (A.LevelCode like '{0}%' 
                                       or CHARINDEX(A.LevelCode, '{0}') > 0) ";
            string m_SqlCondition = "";
            if (myOrganizations != null)
            {
                for (int i = 0; i < myOrganizations.Count; i++)
                {
                    if (i == 0)
                    {
                        m_SqlCondition = string.Format(m_SqlConditionTemp, myOrganizations[i]);
                    }
                    else
                    {
                        m_SqlCondition = m_SqlCondition + string.Format("or " + m_SqlConditionTemp, myOrganizations[i]);
                    }
                }
            }
            if (m_SqlCondition != "")
            {
                m_Sql = string.Format(m_Sql, "(" + m_SqlCondition + ")");
            }
            else
            {
                m_Sql = string.Format(m_Sql, "A.OrganizationID <> A.OrganizationID");
            }
            try
            {
                //SqlParameter[] m_Parameters = { new SqlParameter("@KeyId", myKeyId) };
                DataTable m_Result = _dataFactory.Query(m_Sql);
                return m_Result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
