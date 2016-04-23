using EasyUIJsonParser;
using BasicData.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace BasicData.Service.Material
{
    public static class MaterialService
    {
        /// <summary>
        /// 按组织机构ID获取所有的物料聚合根记录
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static DataTable GetMaterials(string organizationId)
        {
            DataTable result = new DataTable();
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT *
                                          FROM [tz_Material]
                                         WHERE [OrganizationID] LIKE @organizationId + '%' AND
	                                           [Enable] = 1";

                command.Parameters.Add(new SqlParameter("organizationId", organizationId));

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }
            }

            return result;
        }

        /// <summary>
        /// 创建物料列表
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="name"></param>
        public static void CreateMaterial(string organizationId, string name)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO [dbo].[tz_Material]
                                                   ([KeyID]
                                                   ,[OrganizationID]
                                                   ,[Name]
                                                   ,[CreatedDate]
                                                   ,[Type]
                                                   ,[Enable]
                                                   ,[State])
                                             VALUES
                                                   (@keyId
                                                   ,@organizationID
                                                   ,@name
                                                   ,@createdDate
                                                   ,2
                                                   ,1
                                                   ,0)";

                command.Parameters.Add(new SqlParameter("keyId", Guid.NewGuid()));
                command.Parameters.Add(new SqlParameter("organizationId", organizationId));
                command.Parameters.Add(new SqlParameter("name", name));
                command.Parameters.Add(new SqlParameter("createdDate", DateTime.Now));

                connection.Open();
                command.ExecuteNonQuery();
            }

        }
        
        /// <summary>
        /// 根据KeyID禁用物料聚合根
        /// </summary>
        /// <param name="keyId"></param>
        public static void DisableMaterials(string keyId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"UPDATE [dbo].[tz_Material]
                                           SET [Enable] = 0
                                         WHERE [KeyID] = @keyId AND
	                                           [Enable] = 1";

                command.Parameters.Add(new SqlParameter("keyId", keyId));
            }
        }

        /// <summary>
        /// 根据KeyID删除所有物料记录
        /// </summary>
        /// <param name="keyId"></param>
        public static void DeleteMaterials(string keyId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (TransactionScope tsCope = new TransactionScope())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command_tz = connection.CreateCommand();
                    SqlCommand command_details = connection.CreateCommand();

                    command_tz.CommandText = @"DELETE FROM [dbo].[tz_Material]
                                             WHERE [KeyID] = @keyId";
                    command_tz.Parameters.Add(new SqlParameter("keyId", keyId));

                    command_details.CommandText = @"DELETE FROM [dbo].[material_MaterialDetail]
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
        /// 根据KeyID获取物料详细
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public static DataTable GetMaterialDetail(string keyId)
        {
            DataTable result = new DataTable();
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT *
                                          FROM [material_MaterialDetail]
                                         WHERE [KeyID] = @keyId";

                command.Parameters.Add(new SqlParameter("keyId", keyId));

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }
            }

            return result;
        }

        /// <summary>
        /// 创建物料详细（单条）
        /// </summary>
        /// <param name="variableId"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="unit"></param>
        /// <param name="materialErpCode"></param>
        /// <param name="tagTableName"></param>
        /// <param name="formula"></param>
        /// <param name="coefficient"></param>
        public static void CreateMaterialDetail(string keyId,
                                                string variableId,
                                                string name,
                                                string type,
                                                string unit,
                                                string materialErpCode,
                                                string tagTableName,
                                                string formula,
                                                string coefficient)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO [dbo].[material_MaterialDetail]
                                                   ([MaterialId]
		                                           ,[VariableId]
                                                   ,[Name]
                                                   ,[KeyID]
                                                   ,[Type]
                                                   ,[Unit]
                                                   ,[MaterialErpCode]
                                                   ,[TagTableName]
                                                   ,[Formula]
                                                   ,[Coefficient])
                                             VALUES
                                                   (@materialId
                                                   ,@variableId
                                                   ,@name
                                                   ,@keyId
                                                   ,@type
                                                   ,@unit
                                                   ,@materialErpCode
                                                   ,@tagTableName
                                                   ,@formula
                                                   ,@coefficient)";

                command.Parameters.Add(new SqlParameter("materialId", Guid.NewGuid()));
                command.Parameters.Add(new SqlParameter("keyId", keyId));
                command.Parameters.Add(new SqlParameter("variableId", variableId));
                command.Parameters.Add(new SqlParameter("name", name));
                command.Parameters.Add(new SqlParameter("type", type));
                command.Parameters.Add(new SqlParameter("unit", unit));
                command.Parameters.Add(new SqlParameter("materialErpCode", materialErpCode));
                command.Parameters.Add(new SqlParameter("tagTableName", tagTableName));
                command.Parameters.Add(new SqlParameter("formula", formula));
                if (string.IsNullOrWhiteSpace(coefficient))
                    command.Parameters.Add(new SqlParameter("coefficient", DBNull.Value));
                else
                    command.Parameters.Add(new SqlParameter("coefficient", decimal.Parse(coefficient)));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 更新物料详细（单条）
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="variableId"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="unit"></param>
        /// <param name="materialErpCode"></param>
        /// <param name="tagTableName"></param>
        /// <param name="formula"></param>
        /// <param name="coefficient"></param>
        public static void UpdateMaterialDetail(string materialId,
                                                string variableId,
                                                string name,
                                                string type,
                                                string unit,
                                                string materialErpCode,
                                                string tagTableName,
                                                string formula,
                                                string coefficient)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"UPDATE [dbo].[material_MaterialDetail]
                                       SET [VariableId] = @variableId
                                          ,[Name] = @name
                                          ,[Type] = @type
                                          ,[Unit] = @unit
                                          ,[MaterialErpCode] = @materialErpCode
                                          ,[TagTableName] = @tagTableName
                                          ,[Formula] = @formula
                                          ,[Coefficient] = @coefficient
                                     WHERE [MaterialId] = @materialId";

                command.Parameters.Add(new SqlParameter("materialId", materialId));
                command.Parameters.Add(new SqlParameter("variableId", variableId));
                command.Parameters.Add(new SqlParameter("name", name));
                command.Parameters.Add(new SqlParameter("type", type));
                command.Parameters.Add(new SqlParameter("unit", unit));
                command.Parameters.Add(new SqlParameter("materialErpCode", materialErpCode));
                command.Parameters.Add(new SqlParameter("tagTableName", tagTableName));
                command.Parameters.Add(new SqlParameter("formula", formula));
                if (string.IsNullOrWhiteSpace(coefficient))
                    command.Parameters.Add(new SqlParameter("coefficient", DBNull.Value));
                else
                    command.Parameters.Add(new SqlParameter("coefficient", decimal.Parse(coefficient)));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 更新物料系数（单条）
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="coefficient"></param>
        public static void UpdateMaterialCoefficient(string materialId, string coefficient)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"UPDATE [dbo].[material_MaterialDetail]
                                       SET [Coefficient] = @coefficient
                                     WHERE [MaterialId] = @materialId";

                command.Parameters.Add(new SqlParameter("materialId", materialId));
                if (string.IsNullOrWhiteSpace(coefficient))
                    command.Parameters.Add(new SqlParameter("coefficient", DBNull.Value));
                else
                    command.Parameters.Add(new SqlParameter("coefficient", decimal.Parse(coefficient)));

                connection.Open();
                command.ExecuteNonQuery();
            }

        }

        /// <summary>
        /// 删除物料详细（单条）
        /// </summary>
        /// <param name="materialId"></param>
        public static void DeleteMaterialDetail(string materialId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"DELETE FROM [material_MaterialDetail]
                                              WHERE [MaterialId] = @materialId";

                command.Parameters.Add(new SqlParameter("materialId", materialId));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
 
        /// <summary>
        /// 更新物料详细
        /// </summary>
        /// <param name="json"></param>
        public static void UpdateMaterialDetailFromJson(string json)
        {
            string[] detailJsons = json.JsonPickArray("rows");

            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();

            command.CommandText = @"UPDATE [dbo].[material_MaterialDetail]
                                       SET [VariableId] = @variableId
                                          ,[Name] = @name
                                          ,[Type] = @type
                                          ,[Unit] = @unit
                                          ,[MaterialErpCode] = @materialErpCode
                                          ,[TagTableName] = @tagTableName
                                          ,[Formula] = @formula
                                          ,[Coefficient] = @coefficient
                                     WHERE [MaterialId] = @materialId";

            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                connection.Open();

                foreach (string detail in detailJsons)
                {
                    string materialId = detail.JsonPick("MaterialId");
                    string variableId = detail.JsonPick("VariableId");
                    string name = detail.JsonPick("Name");
                    string type = detail.JsonPick("Type");
                    string unit = detail.JsonPick("Unit");
                    string materialErpCode = detail.JsonPick("MaterialErpCode");
                    string tagTableName = detail.JsonPick("TagTableName");
                    string formula = detail.JsonPick("Formula");
                    string coefficient = detail.JsonPick("Coefficient");

                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("materialId", materialId));
                    command.Parameters.Add(new SqlParameter("variableId", variableId));
                    command.Parameters.Add(new SqlParameter("name", name));
                    command.Parameters.Add(new SqlParameter("type", type));
                    command.Parameters.Add(new SqlParameter("unit", unit));
                    command.Parameters.Add(new SqlParameter("materialErpCode", materialErpCode));
                    command.Parameters.Add(new SqlParameter("tagTableName", tagTableName));
                    command.Parameters.Add(new SqlParameter("formula", formula));
                    command.Parameters.Add(new SqlParameter("coefficient", coefficient));
                    command.ExecuteNonQuery();
                }
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
            finally
            {
                connection.Close();
                transaction.Dispose();
                connection.Dispose();
            }
        }

        /// <summary>
        /// 更新物料系数
        /// </summary>
        /// <param name="json"></param>
        public static void UpdateMaterialCoefficientFromJson(string json)
        {
            string[] detailJsons = json.JsonPickArray("rows");

            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (TransactionScope tsCope = new TransactionScope())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = connection.CreateCommand();

                    command.CommandText = @"UPDATE [dbo].[material_MaterialDetail]
                                       SET [Coefficient] = @coefficient
                                     WHERE [MaterialId] = @materialId";

                    connection.Open();

                    foreach (string detail in detailJsons)
                    {
                        string materialId = detail.JsonPick("MaterialId");
                        string coefficient = detail.JsonPick("Coefficient");

                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("materialId", materialId));
                        if (string.IsNullOrWhiteSpace(coefficient))
                            command.Parameters.Add(new SqlParameter("coefficient", DBNull.Value));
                        else
                            command.Parameters.Add(new SqlParameter("coefficient", decimal.Parse(coefficient)));
                        command.ExecuteNonQuery();
                    }
                }

                tsCope.Complete();
            }
        }
    }
}
