using BasicData.Infrastructure.Configuration;
using SqlServerDataAdapter;
using SqlServerDataAdapter.Infrastruction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicData.Service.StaffInfo
{
    public class StaffInfoService
    {
        /// <summary>
        /// 获取职工信息
        /// </summary>
        /// <param name="organizationId">组织机构ID</param>
        /// <param name="searchName">检索关键字</param>
        /// <returns></returns>
        public static DataTable GetStaffInfo(string organizationId, string searchName = "", string searchId = "", string searchTeamName = "")
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            Query query = new Query("system_StaffInfo");
            query.AddCriterion("OrganizationID", organizationId, CriteriaOperator.Equal);
            // 添加检索关键字约束
            if (!string.IsNullOrWhiteSpace(searchName + searchId + searchTeamName))
            {
                searchName = "%" + searchName + "%";
                searchId = "%" + searchId + "%";
                searchTeamName = "%" + searchTeamName + "%";
                query.AddCriterion("Name", searchName, CriteriaOperator.Like);
                query.AddCriterion("StaffInfoID", searchId, CriteriaOperator.Like);
                query.AddCriterion("WorkingTeamName", searchTeamName, CriteriaOperator.Like);
            }

            // 添加排序（启用降序，工号升序）
            query.AddOrderByClause(new OrderByClause("Enabled", true));
            query.AddOrderByClause(new OrderByClause("StaffInfoID", false));

            DataTable m_StaffInfoTable = dataFactory.Query(query);
            DataRow[] m_dataRows = m_StaffInfoTable.Select("StaffType <> 'superior' or StaffType is null");
            DataTable m_StaffInfoTableNew = m_StaffInfoTable.Clone();
            for (int i = 0; i < m_dataRows.Length; i++)
            {
                m_StaffInfoTableNew.Rows.Add(m_dataRows[i].ItemArray);
            }
            return m_StaffInfoTableNew;
        }

        /// <summary>
        /// 添加职工信息
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="staffInfoId"></param>
        /// <param name="workingTeamName"></param>
        /// <param name="name"></param>
        /// <param name="sex"></param>
        /// <param name="phoneNumber"></param>
        /// <returns>添加结果</returns>
        public static string InsertStaffInfo(string organizationId, string staffInfoId, string workingTeamName, string name, bool sex, string phoneNumber)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            // 检查是否存在此职工
            Query query = new Query("system_StaffInfo");
            query.AddCriterion("OrganizationID", organizationId, CriteriaOperator.Equal);
            query.AddCriterion("staffInfoId", staffInfoId, CriteriaOperator.Equal);

            if (dataFactory.Query(query).Rows.Count > 0)
                return "添加失败，已存在此工号。";

            int executeResult = 0;

            // 添加记录
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO [dbo].[system_StaffInfo]
                                                   ([StaffInfoID]
                                                   ,[OrganizationID]
                                                   ,[WorkingTeamName]
                                                   ,[Name]
                                                   ,[Sex]
                                                   ,[PhoneNumber]
                                                   ,[Enabled])
                                             VALUES
                                                   (@staffInfoId
                                                   ,@organizationId
                                                   ,@workingTeamName
                                                   ,@name
                                                   ,@sex
                                                   ,@phoneNumber
                                                   ,1)";

                command.Parameters.Add(new SqlParameter("staffInfoId", staffInfoId));
                command.Parameters.Add(new SqlParameter("organizationId", organizationId));
                command.Parameters.Add(new SqlParameter("workingTeamName", workingTeamName));
                command.Parameters.Add(new SqlParameter("name", name));
                command.Parameters.Add(new SqlParameter("sex", sex));
                command.Parameters.Add(new SqlParameter("phoneNumber", phoneNumber));

                connection.Open();
                executeResult = command.ExecuteNonQuery();
            }

            if (executeResult == 1)
                return "添加成功。";
            else
                return "添加失败。";
        }

        /// <summary>
        /// 修改职工信息
        /// </summary>
        /// <param name="staffInfoId"></param>
        /// <param name="workingTeamName"></param>
        /// <param name="name"></param>
        /// <param name="sex"></param>
        /// <param name="phoneNumber"></param>
        /// <returns>修改结果</returns>
        public static string UpdateStaffInfo(string organizationId, string staffInfoItemId, string staffInfoId, string workingTeamName, string name, bool sex, string phoneNumber, bool enabled)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            int executeResult = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"UPDATE [dbo].[system_StaffInfo]
                                           SET [StaffInfoID] = @staffInfoId
                                              ,[WorkingTeamName] = @workingTeamName
                                              ,[Name] = @name
                                              ,[Sex] = @sex
                                              ,[PhoneNumber] = @phoneNumber
                                              ,[Enabled] = @enabled
                                         WHERE [StaffInfoItemId] = @StaffInfoItemId";

                command.Parameters.Add(new SqlParameter("staffInfoId", staffInfoId));
                command.Parameters.Add(new SqlParameter("organizationId", organizationId));
                command.Parameters.Add(new SqlParameter("workingTeamName", workingTeamName));
                command.Parameters.Add(new SqlParameter("name", name));
                command.Parameters.Add(new SqlParameter("sex", sex));
                command.Parameters.Add(new SqlParameter("phoneNumber", phoneNumber));
                command.Parameters.Add(new SqlParameter("enabled", enabled));
                command.Parameters.Add(new SqlParameter("StaffInfoItemId", staffInfoItemId));

                connection.Open();
                executeResult = command.ExecuteNonQuery();
            }

            if (executeResult == 1)
                return "修改成功。";
            else
                return "修改失败。";
        }
        public static string DeleteStaffInfo(string myStaffInfoItemId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_Sql = @"delete from [dbo].[system_StaffInfo] where [StaffInfoItemId] = '{0}'";
            m_Sql = string.Format(m_Sql, myStaffInfoItemId);
            int m_DeleteFlag = dataFactory.ExecuteSQL(m_Sql);
            if (m_DeleteFlag > 0)
            {
                return "删除成功！";
            }
            else if (m_DeleteFlag == 0)
            {
                return "该员工不存在！";
            }
            else
            {
                return "删除失败！";
            }
        }
    }
}
