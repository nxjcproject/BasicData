using EasyUIJsonParser;
using BasicData.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BasicData.Service.ShiftArrangement
{
    public class ShiftArrangementService
    {
        public static DataTable GetShiftArrangement(string organizationId)
        {
            DataTable table = new DataTable();
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.OrganizationID,A.WorkingTeam,CONVERT(varchar(10),A.ShiftDate,20) as ShiftDate,CONVERT(varchar(10),A.UpdateDate,20) as UpdateDate
                                from system_ShiftArrangement A
                                where A.OrganizationID=@organizationId
                                order by WorkingTeam";
            SqlParameter parameter = new SqlParameter("organizationId",organizationId);
            table= dataFactory.Query(mySql, parameter);
            string mSql = @"SELECT [ID]
                              ,[OrganizationID]
                              ,[Name]
                              ,[ChargeManID]
                              ,[Remarks]
                          FROM [system_WorkingTeam]
                           where [OrganizationID]=@organizationId";
            SqlParameter para = new SqlParameter("organizationId", organizationId);
            DataTable result = dataFactory.Query(mSql, para);
            if (table.Rows.Count<result.Rows.Count && table.Rows.Count!=0)
            {
                DataRow row = table.NewRow();
                row["OrganizationID"]=organizationId;
                row["WorkingTeam"]="常白";
                row["ShiftDate"]="";
                row["UpdateDate"]="";
                table.Rows.Add(row);
            }
            if (table.Rows.Count == 0)
            {
                IList<string> list = new List<string>();
                if (result.Rows.Count==4)
                {
                    list.Add("A班");
                    list.Add("B班");
                    list.Add("C班");
                    list.Add("D班");
                }
                if (result.Rows.Count==5)
                {
                    list.Add("A班");
                    list.Add("B班");
                    list.Add("C班");
                    list.Add("D班");
                    list.Add("常白");
                }               
                foreach (string t_item in list)
                {
                    string insertSQL = @"insert into [dbo].[system_ShiftArrangement] ([OrganizationID],[WorkingTeam],[UpdateDate])
                                        values ('{0}','{1}',GETDATE())";
                    dataFactory.ExecuteSQL(string.Format(insertSQL, organizationId, t_item));
                }
                SqlParameter parameterlast = new SqlParameter("organizationId", organizationId);
                table = dataFactory.Query(mySql, parameterlast);
            }
            return table;
        }

        public static int SaveShiftArrange(string mOrganizationId,string json)
        {
            int influenceNum = 0;
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mSql = @"delete from [system_ShiftArrangement] WHERE OrganizationID=@mOrganizationId";
            SqlParameter parameter = new SqlParameter("mOrganizationId", mOrganizationId);
            int dt = factory.ExecuteSQL(mSql, parameter);
//            string mySql = @"update [system_ShiftArrangement] 
//                                set [ShiftDate]=@shiftDate
//                                , [UpdateDate]=GETDATE()
//                                where [OrganizationID]=@organizationId
//                                and [WorkingTeam]=@workingTeam";
            string mySql = @"INSERT INTO [dbo].[system_ShiftArrangement] 
                                   ([ShiftArrangementItemId]
                                   ,[OrganizationID]
                                   ,[WorkingTeam]
                                   ,[ShiftDate]
                                   ,[UpdateDate])
                             VALUES
                                   (@shiftArrangementItemId
                                   ,@organizationId
                                   ,@workingTeam
                                   ,@shiftDate
                                   ,GETDATE())";
            using (SqlConnection con = new SqlConnection(connectionString))
            {   
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = mySql;
                string[] array = json.JsonPickArray("rows");
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                cmd.Transaction = transaction;
                try
                {                  
                    foreach (string item in array)
                    {
                        cmd.Parameters.Clear();
                        string shiftData = item.JsonPick("ShiftDate");
                        string organizationId = item.JsonPick("OrganizationID");
                        string workingTeam = item.JsonPick("WorkingTeam");
                        cmd.Parameters.Add(new SqlParameter("shiftArrangementItemId", System.Guid.NewGuid().ToString()));
                        cmd.Parameters.Add(new SqlParameter("shiftDate", shiftData));
                        cmd.Parameters.Add(new SqlParameter("organizationId", organizationId));
                        cmd.Parameters.Add(new SqlParameter("workingTeam", workingTeam));
                        influenceNum = cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
                finally
                {
                    con.Close();
                    transaction.Dispose();
                    con.Dispose();
                }
            }
            return influenceNum;
        }
    }
}
