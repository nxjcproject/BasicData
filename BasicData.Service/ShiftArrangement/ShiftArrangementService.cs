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
            string mySql = @"select A.OrganizationID,A.WorkingTeam,CONVERT(varchar(10),A.ShiftDate,20) as ShiftDate,A.UpdateDate
                                from system_ShiftArrangement A
                                where A.OrganizationID=@organizationId
                                order by WorkingTeam";
            SqlParameter parameter = new SqlParameter("organizationId",organizationId);
            table= dataFactory.Query(mySql, parameter);
            
            if (table.Rows.Count == 0)
            {
                IList<string> list = new List<string>();
                list.Add("A班");
                list.Add("B班");
                list.Add("C班");
                list.Add("D班");
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

        public static int SaveShiftArrange(string json)
        {
            int influenceNum = 0;
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            string mySql = @"update [system_ShiftArrangement] 
                                set [ShiftDate]=@shiftDate
                                , [UpdateDate]=GETDATE()
                                where [OrganizationID]=@organizationId
                                and [WorkingTeam]=@workingTeam";
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
