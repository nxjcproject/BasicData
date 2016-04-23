using EasyUIJsonParser;
using BasicData.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace BasicData.Service.KpiDefinition
{
    public static class KpiService
    {
        /// <summary>
        /// 获取KPI指标体系列表
        /// </summary>
        /// <param name="statisticalMethod"></param>
        /// <returns></returns>
        public static DataTable GetKpiList(string statisticalMethod)
        {
            DataTable result = new DataTable();
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT [A].*, [U].[USER_NAME] AS [UserName]
                                          FROM [analyse_KPI_Standard] AS [A] LEFT JOIN
                                               [IndustryEnergy_SH].[dbo].[users] AS [U] ON [A].[Creator] = [U].[USER_ID]
                                         WHERE [A].[StatisticalMethod] = @statisticalMethod AND
                                               [A].[Enabled] = 1
                                      ORDER BY [A].[DisplayIndex]";

                command.Parameters.Add(new SqlParameter("statisticalMethod", statisticalMethod));

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }
            }

            return result;
        }

        /// <summary>
        /// 创建KPI引领
        /// </summary>
        /// <param name="standardId"></param>
        /// <param name="statisticalMethod"></param>
        /// <param name="displayIndex"></param>
        /// <param name="creator"></param>
        public static void CreateKpiList(string standardId, string statisticalMethod, int displayIndex, string creator)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO [dbo].[analyse_KPI_Standard]
                                                   ([KeyId]
                                                   ,[StandardId]
                                                   ,[StandardName]
                                                   ,[StatisticalMethod]
                                                   ,[DisplayIndex]
                                                   ,[Version]
                                                   ,[Creator]
                                                   ,[CreateTime]
                                                   ,[Enabled])
                                             VALUES
                                                   (@keyId
                                                   ,@standardId
                                                   ,@standardName
                                                   ,@statisticalMethod
                                                   ,@displayIndex
                                                   ,@version
                                                   ,@creator
                                                   ,@createTime
                                                   ,@enabled)";

                command.Parameters.Add(new SqlParameter("keyId", Guid.NewGuid()));
                command.Parameters.Add(new SqlParameter("standardId", standardId));
                command.Parameters.Add(new SqlParameter("standardName", GetStandardName(standardId)));
                command.Parameters.Add(new SqlParameter("statisticalMethod", statisticalMethod));
                command.Parameters.Add(new SqlParameter("displayIndex", displayIndex));
                command.Parameters.Add(new SqlParameter("version", 1));
                command.Parameters.Add(new SqlParameter("creator", creator));
                command.Parameters.Add(new SqlParameter("createTime", DateTime.Now));
                command.Parameters.Add(new SqlParameter("enabled", 1));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取标准名称
        /// </summary>
        /// <param name="standardId"></param>
        /// <returns></returns>
        private static string GetStandardName(string standardId)
        {
            switch (standardId)
            {
                case "InternationalStandard":
                    return "国际标准";
                case "NationalStandard":
                    return "国家标准";
                case "IndustryStandard":
                    return "行业标准";
                case "EnterpriseStandard":
                    return "企业标准";
                default:
                    return standardId;
            }
        }

        /// <summary>
        /// 删除KPI引领
        /// </summary>
        /// <param name="keyId"></param>
        public static void DeleteKpiList(string keyId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (TransactionScope tsCope = new TransactionScope())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command_tz = connection.CreateCommand();
                    SqlCommand command_details = connection.CreateCommand();

                    command_tz.CommandText = @"DELETE FROM [dbo].[analyse_KPI_Standard]
                                             WHERE [KeyID] = @keyId";
                    command_tz.Parameters.Add(new SqlParameter("keyId", keyId));

                    command_details.CommandText = @"DELETE FROM [dbo].[analyse_KPI_Standard_Detail]
                                                  WHERE [KeyID] = @keyId";
                    command_details.Parameters.Add(new SqlParameter("keyId", keyId));

                    connection.Open();

                    command_details.ExecuteNonQuery();
                    command_tz.ExecuteNonQuery();
                }

                tsCope.Complete();
            }
        }

        /// <summary>
        /// 获取KPI详细
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public static DataTable GetKpiDetail(string keyId)
        {
            DataTable result = new DataTable();
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT [D].*, [O].[Name] AS [OrganizationName], [U].[USER_NAME] AS [UserName]
                                          FROM [analyse_KPI_Standard_Detail] AS [D] LEFT JOIN
                                               [system_Organization] AS [O] ON [D].[OrganizationID] = [O].[OrganizationID] LEFT JOIN
                                               [IndustryEnergy_SH].[dbo].[users] AS [U] ON [D].[Creator] = [U].[USER_ID]
                                         WHERE [D].[KeyId] = @keyId AND
                                               [D].[Enabled] = 1
                                      ORDER BY [D].[CreateTime]";

                command.Parameters.Add(new SqlParameter("keyId", keyId));

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取KPI详细
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public static DataTable GetKpiDetail_Public(string keyId)
        {
            DataTable result = new DataTable();
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT [D].*, [O].[Name] AS [OrganizationName], [U].[USER_NAME] AS [UserName]
                                          FROM [analyse_KPI_Standard_Detail] AS [D] LEFT JOIN
                                               [system_Organization] AS [O] ON [D].[OrganizationID] = [O].[OrganizationID] LEFT JOIN
                                               [IndustryEnergy_SH].[dbo].[users] AS [U] ON [D].[Creator] = [U].[USER_ID]
                                         WHERE [D].[KeyId] = @keyId AND
                                               [D].[Enabled] = 1 AND
                                               ([D].[OrganizationID] = '' OR [D].[OrganizationID] IS NULL)
                                      ORDER BY [D].[CreateTime]";

                command.Parameters.Add(new SqlParameter("keyId", keyId));

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取KPI详细
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public static DataTable GetKpiDetail_Private(string keyId, string organizationId)
        {
            DataTable result = new DataTable();
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT [D].*, [O].[Name] AS [OrganizationName], [U].[USER_NAME] AS [UserName]
                                          FROM [analyse_KPI_Standard_Detail] AS [D] LEFT JOIN
                                               [system_Organization] AS [O] ON [D].[OrganizationID] = [O].[OrganizationID] LEFT JOIN
                                               [IndustryEnergy_SH].[dbo].[users] AS [U] ON [D].[Creator] = [U].[USER_ID]
                                         WHERE [D].[KeyId] = @keyId AND
                                               [D].[Enabled] = 1 AND
                                               [D].[OrganizationID] = @organizationId
                                      ORDER BY [D].[CreateTime]";

                command.Parameters.Add(new SqlParameter("keyId", keyId));
                command.Parameters.Add(new SqlParameter("organizationId", organizationId));

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }
            }

            return result;
        }

        /// <summary>
        /// 创建KPI详细
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="name"></param>
        /// <param name="organizationId"></param>
        /// <param name="levelType"></param>
        /// <param name="variableId"></param>
        /// <param name="valueType"></param>
        /// <param name="unit"></param>
        /// <param name="standardValue"></param>
        /// <param name="standardLevel"></param>
        /// <param name="creator"></param>
        public static void CreateKpiDetail(string keyId, string name, string organizationId, string levelType, string variableId, string valueType, string unit, decimal standardValue, int standardLevel, string creator)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO [dbo].[analyse_KPI_Standard_Detail]
                                                   ([StandardItemId]
                                                   ,[KeyId]
                                                   ,[Name]
                                                   ,[OrganizationID]
                                                   ,[LevelType]
                                                   ,[VariableId]
                                                   ,[ValueType]
                                                   ,[Unit]
                                                   ,[StandardValue]
                                                   ,[StandardLevel]
                                                   ,[Creator]
                                                   ,[CreateTime]
                                                   ,[Enabled])
                                             VALUES
                                                   (@standardItemId
                                                   ,@keyId
                                                   ,@name
                                                   ,@organizationID
                                                   ,@levelType
                                                   ,@variableId
                                                   ,@valueType
                                                   ,@unit
                                                   ,@standardValue
                                                   ,@standardLevel
                                                   ,@creator
                                                   ,@createTime
                                                   ,@enabled)";

                command.Parameters.Add(new SqlParameter("standardItemId", Guid.NewGuid()));
                command.Parameters.Add(new SqlParameter("keyId", keyId));
                command.Parameters.Add(new SqlParameter("name", name));
                if (string.IsNullOrWhiteSpace(organizationId))
                    command.Parameters.Add(new SqlParameter("organizationID", DBNull.Value));
                else
                    command.Parameters.Add(new SqlParameter("organizationID", organizationId));
                command.Parameters.Add(new SqlParameter("levelType", levelType));
                command.Parameters.Add(new SqlParameter("variableId", variableId));
                command.Parameters.Add(new SqlParameter("valueType", valueType));
                command.Parameters.Add(new SqlParameter("unit", unit));
                command.Parameters.Add(new SqlParameter("standardValue", standardValue));
                command.Parameters.Add(new SqlParameter("standardLevel", standardLevel));
                command.Parameters.Add(new SqlParameter("creator", creator));
                command.Parameters.Add(new SqlParameter("createTime", DateTime.Now));
                command.Parameters.Add(new SqlParameter("enabled", 1));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 更新KPI详细
        /// </summary>
        /// <param name="standardItemId"></param>
        /// <param name="name"></param>
        /// <param name="organizationId"></param>
        /// <param name="levelType"></param>
        /// <param name="variableId"></param>
        /// <param name="valueType"></param>
        /// <param name="unit"></param>
        /// <param name="standardValue"></param>
        /// <param name="standardLevel"></param>
        /// <param name="creator"></param>
        public static void UpdateKpiDetail(string standardItemId, string name, string organizationId, string levelType, string variableId, string valueType, string unit, decimal standardValue, int standardLevel, string creator)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"UPDATE [dbo].[analyse_KPI_Standard_Detail]
                                           SET [StandardItemId] = @standardItemId
                                              ,[Name] = @name
                                              ,[OrganizationID] = @organizationID
                                              ,[LevelType] = @levelType
                                              ,[VariableId] = @variableId
                                              ,[ValueType] = @valueType
                                              ,[Unit] = @unit
                                              ,[StandardValue] = @standardValue
                                              ,[StandardLevel] = @standardLevel
                                              ,[Creator] = @creator
                                              ,[CreateTime] = @createTime
                                         WHERE [StandardItemId] = @standardItemId";

                command.Parameters.Add(new SqlParameter("standardItemId", standardItemId));
                command.Parameters.Add(new SqlParameter("name", name));
                if (string.IsNullOrWhiteSpace(organizationId))
                    command.Parameters.Add(new SqlParameter("organizationID", DBNull.Value));
                else
                    command.Parameters.Add(new SqlParameter("organizationID", organizationId));
                command.Parameters.Add(new SqlParameter("levelType", levelType));
                command.Parameters.Add(new SqlParameter("variableId", variableId));
                command.Parameters.Add(new SqlParameter("valueType", valueType));
                command.Parameters.Add(new SqlParameter("unit", unit));
                command.Parameters.Add(new SqlParameter("standardValue", standardValue));
                command.Parameters.Add(new SqlParameter("standardLevel", standardLevel));
                command.Parameters.Add(new SqlParameter("creator", creator));
                command.Parameters.Add(new SqlParameter("createTime", DateTime.Now));
                command.Parameters.Add(new SqlParameter("enabled", 1));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 删除KPI详细
        /// </summary>
        /// <param name="standardItemId"></param>
        public static void DeleteKpiDetail(string standardItemId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"DELETE FROM [analyse_KPI_Standard_Detail]
                                              WHERE [StandardItemId] = @standardItemId";

                command.Parameters.Add(new SqlParameter("standardItemId", standardItemId));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
