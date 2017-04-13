using BasicData.Infrastructure.Configuration;
using BasicData.Service.BasicService;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicData.Service.WorkingTeamAndShift
{
    public class WorkingTeamAndShiftService
    {
        private readonly static string _connStr = ConnectionStringFactory.NXJCConnectionString;
        private static ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connStr);
        private static BasicDataHelper _dataHelper = new BasicDataHelper(_connStr);

        public static DataTable GetStaffInfo(string organizationId)
        {
            string queryStr = @"SELECT StaffInfoID,WorkingTeamName,Name FROM system_StaffInfo WHERE OrganizationID=@organizationId";
            DataTable result = _dataFactory.Query(queryStr, new SqlParameter("@organizationId", organizationId));
            return result;
        }

        public static int SaveShiftInfo(string organizationId, string[] json)
        {
            int result;

            string deleteStr = @"DELETE FROM system_ShiftDescription WHERE OrganizationID=@organizationId";

            DataTable dt = _dataHelper.CreateTableStructure("system_ShiftDescription");
            DataTable sourceDt = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(json, dt);
            DateTime time = System.DateTime.Now;
            foreach (DataRow dr in sourceDt.Rows)
            {
                dr["OrganizationID"] = organizationId;
                dr["CreatedDate"] = time;
                dr["ShiftDescriptionID"] = System.Guid.NewGuid().ToString();
            }
            try
            {
                _dataFactory.ExecuteSQL(deleteStr, new SqlParameter("@organizationId", organizationId));
                int affected = _dataFactory.Save("system_ShiftDescription", sourceDt);
                if (affected != -1)
                    result = 1;
                else
                    result = -1;
            }
            catch
            {
                result = -1;
            }

            return result;
        }

        public static int SaveWorkingTeamInfo(string organizationId, string[] json)
        {
            int result;

            string deleteStr = @"DELETE FROM system_WorkingTeam WHERE OrganizationID=@organizationId";

            DataTable dt = _dataHelper.CreateTableStructure("system_WorkingTeam");
            DataTable sourceDt = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(json, dt);

            foreach (DataRow dr in sourceDt.Rows)
            {
                dr["OrganizationID"] = organizationId;
                dr["ID"] = System.Guid.NewGuid().ToString();
            }
            try
            {
                _dataFactory.ExecuteSQL(deleteStr, new SqlParameter("@organizationId", organizationId));
                int affected = _dataFactory.Save("system_WorkingTeam", sourceDt);
                if (affected != -1)
                    result = 1;
                else
                    result = -1;
            }
            catch
            {
                result = -1;
            }
            return result;
        }

        public static DataTable QueryShiftsInfo(string organizationId)
        {
            string queryStr = @"SELECT * FROM system_ShiftDescription WHERE OrganizationID=@organizationId
                              and [Shifts] IN ('甲班','乙班','丙班')
                              order by CHARINDEX(',' + CONVERT(VARCHAR(64),Shifts)+ ',',',甲班,乙班,丙班,')";
            DataTable dt = _dataFactory.Query(queryStr, new SqlParameter("@organizationId", organizationId));

            return dt;
        }

        public static DataTable QueryWorkingTeamInfo(string organizationId)
        {
            string queryStr = @"SELECT * FROM system_WorkingTeam WHERE OrganizationID=@organizationId
                               order by [Name]";
            DataTable dt = _dataFactory.Query(queryStr, new SqlParameter("@organizationId", organizationId));

            return dt;
        }
    }
}
