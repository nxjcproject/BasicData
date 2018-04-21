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
            int mWorkingTeamCount = -1;//记录未保存前工作组的个数，与保存的比较判断是增加还是删除了工作组
            StringBuilder mOldWorkingTeamSB = new StringBuilder();//未保存时的所有班组
            StringBuilder mWorkingTeamSB = new StringBuilder();//当前需保存的所有班组
            string mDelWorkingTeamSql = @"DELETE 
                                            FROM system_WorkingTeam 
                                           WHERE OrganizationID=@organizationId";
            
            DataTable dt = _dataHelper.CreateTableStructure("system_WorkingTeam");
            DataTable mSourceDt = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(json, dt);
            foreach (DataRow dr in mSourceDt.Rows)
            {
                dr["OrganizationID"] = organizationId;
                dr["ID"] = System.Guid.NewGuid().ToString();
            }

            //获取保存的所有工作班组
            for (int i = 0; i < mSourceDt.Rows.Count; i++)
            {
                if (i == 0)
                {
                    mWorkingTeamSB.Append("'" + mSourceDt.Rows[i]["Name"].ToString() + "'");
                }
                else {
                    mWorkingTeamSB.Append("," + "'" + mSourceDt.Rows[i]["Name"].ToString() + "'");
                }
            }
            //获取当前[system_WorkingTeam]的班数
            string mWorkingTeamCountSql = @"SELECT * FROM system_WorkingTeam WHERE OrganizationID=@organizationId";
            DataTable mWorkingTeamCountTable = _dataFactory.Query(mWorkingTeamCountSql, new SqlParameter("@organizationId", organizationId));
            mWorkingTeamCount = mWorkingTeamCountTable.Rows.Count;
            //获取当前所有的工作班组
            for (int i = 0; i < mWorkingTeamCountTable.Rows.Count; i++)
            {
                if (i == 0)
                {
                    mOldWorkingTeamSB.Append("'" + mWorkingTeamCountTable.Rows[i]["Name"].ToString() + "'");
                }
                else
                {
                    mOldWorkingTeamSB.Append("," + "'" + mWorkingTeamCountTable.Rows[i]["Name"].ToString() + "'");
                }           
            }
            //////////////此处开始修改//////////////////////////
            if (mWorkingTeamCount != -1)
            {
                //先保存system_WorkingTeam
                try {
                    _dataFactory.ExecuteSQL(mDelWorkingTeamSql, new SqlParameter("@organizationId", organizationId));
                    int affected = _dataFactory.Save("system_WorkingTeam", mSourceDt);
                    if (affected != -1){
                        result = 1;
                    }
                    else {
                        result = -1;
                    }                      
                }
                catch {
                    result = -1;
                }

                int mSourceDtCount = mSourceDt.Rows.Count;
                if (mSourceDtCount > mWorkingTeamCount) {
                    List<string> mAddWorkingTeam = new List<string>();
                    DataRow[] mAddWorkingTeamRows = mSourceDt.Select("Name not in (" + mOldWorkingTeamSB + ")", "Name");
                    for (int i = 0; i < mAddWorkingTeamRows.Length; i++)
                    {
                        mAddWorkingTeam.Add(mAddWorkingTeamRows[i]["Name"].ToString());
                    }
                    for (int i = 0; i < mAddWorkingTeam.Count; i++)
                    {
                        string mInsertSql = @"INSERT INTO system_ShiftArrangement
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
                        SqlParameter[] mInsertPara = { new SqlParameter("@shiftArrangementItemId", System.Guid.NewGuid().ToString()),
                                                       new SqlParameter("@organizationId",organizationId),
                                                       new SqlParameter("@workingTeam",mAddWorkingTeam[i]),
                                                       new SqlParameter("@shiftDate",DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"))};
                        try {
                            _dataFactory.ExecuteSQL(mInsertSql, mInsertPara);
                        }
                        catch {
                            result = -1;
                        }
                        
                    }                
                }
                else if (mSourceDtCount <= mWorkingTeamCount) {
                    string mDelShiftArrangementSql = @"DELETE 
                                                         FROM system_ShiftArrangement 
                                                        WHERE OrganizationID=@organizationId
                                                          and WorkingTeam NOT IN ({0})";
                    try {
                        _dataFactory.ExecuteSQL(string.Format(mDelShiftArrangementSql, mWorkingTeamSB), new SqlParameter("@organizationId", organizationId));
                    }
                    catch {
                        result = -1;
                    }                   
                }
            }
            else {
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
            string mWorkingTeamSql = @"SELECT * FROM system_WorkingTeam WHERE OrganizationID=@organizationId
                               order by Name";
            DataTable mWorkingTeamTable = _dataFactory.Query(mWorkingTeamSql, new SqlParameter("@organizationId", organizationId));         
            return mWorkingTeamTable;
        }
    }
}
