using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using EasyUIJsonParser;
using BasicData.Infrastructure.Configuration;
using SqlServerDataAdapter;

namespace BasicData.Service.EnergyDataManualInput
{
    public class EnergyDataManualInputService
    {
        private readonly static string _connStr = ConnectionStringFactory.NXJCConnectionString;
        private static ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connStr);

        public static DataTable GetEnergyDataManualInputContrast(string variableName)
        {
            DataTable result;

            string querySql = "";
            IList<SqlParameter> parameters = new List<SqlParameter>();
            
            if (variableName == "")
            {
                querySql = "select * from system_EnergyDataManualInputContrast";
            }
            else
            {
                querySql = "select * from system_EnergyDataManualInputContrast where VariableName like @variableName";
                parameters.Add(new SqlParameter("@variableName", "%" + variableName + "%"));
            }

            result = _dataFactory.Query(querySql, parameters.ToArray());

            return result;
        }

        public static int AddEnergyDataManualInputContrast(string addData)
        {
            int result = 0;

            string testSql = @"select * from system_EnergyDataManualInputContrast where VariableId=@variableId";
            SqlParameter[] testParameters = { new SqlParameter("@variableId", addData.JsonPick("variableId")) };
            DataTable testTable = _dataFactory.Query(testSql, testParameters);

            if (testTable.Rows.Count == 0)
            {
                string insertSql = @"insert into system_EnergyDataManualInputContrast (VariableId,VariableName,Type,Enabled,Creator,CreateTime,Remark) 
                                values (@variableId,@variableName,@type,@enabled,@creator,@createTime,@remark)";
                IList<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@variableId", addData.JsonPick("variableId")));
                parameters.Add(new SqlParameter("@variableName", addData.JsonPick("variableName")));
                parameters.Add(new SqlParameter("@type", addData.JsonPick("type")));
                parameters.Add(new SqlParameter("@enabled", addData.JsonPick("enabled")));
                parameters.Add(new SqlParameter("@creator", addData.JsonPick("creator")));
                parameters.Add(new SqlParameter("@createTime", addData.JsonPick("createTime")));
                parameters.Add(new SqlParameter("@remark", addData.JsonPick("remark")));

                result = _dataFactory.ExecuteSQL(insertSql, parameters.ToArray());
            }
            else
            {
                result = -2; //ID重复
            }

            return result;
        }

        public static int DeleteEnergyDataManualInputContrast(string variableId)
        {
            string deleteSql = @"delete from system_EnergyDataManualInputContrast where VariableId=@variableId";
            SqlParameter[] parameters = {new SqlParameter("@variableId",variableId)};

            int result = _dataFactory.ExecuteSQL(deleteSql, parameters);
            return result;
        }

        public static int EditEnergyDataManualInputContrast(string editData)
        {
            int result = 0;
            string updateSql = @"update system_EnergyDataManualInputContrast set VariableName=@variableName,Enabled=@enabled,
                                Creator=@creator,CreateTime=@createTime,Remark=@remark where VariableId=@variableId";
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@variableId", editData.JsonPick("variableId")));
            parameters.Add(new SqlParameter("@variableName", editData.JsonPick("variableName")));
            parameters.Add(new SqlParameter("@enabled", editData.JsonPick("enabled")));
            parameters.Add(new SqlParameter("@creator", editData.JsonPick("creator")));
            parameters.Add(new SqlParameter("@createTime", editData.JsonPick("createTime")));
            parameters.Add(new SqlParameter("@remark", editData.JsonPick("remark")));

            result = _dataFactory.ExecuteSQL(updateSql, parameters.ToArray());

            return result;
        }

        public static DataTable GetEnergyDataManualInput(string organizationId)
        {
            DataTable result;
            string queryString;
            if (organizationId != "")
            {
                queryString = @"select A.DataItemId,A.OrganizationID,C.Name,A.VariableId,B.VariableName,A.DataValue,A.TimeStamp,A.UpdateCycle,A.Version,A.Remark
                                from system_EnergyDataManualInput as A 
                                left join system_EnergyDataManualInputContrast as B 
                                on A.VariableId=B.VariableId
                                left join system_Organization as C on A.OrganizationID=C.OrganizationID
                                where A.OrganizationID=@organizationId";
            }
            else
            {
                queryString = @"select A.DataItemId,A.OrganizationID,C.Name,A.VariableId,B.VariableName,A.DataValue,A.TimeStamp,A.UpdateCycle,A.Version,A.Remark
                                from system_EnergyDataManualInput as A 
                                left join system_EnergyDataManualInputContrast as B 
                                on A.VariableId=B.VariableId
                                left join system_Organization as C on A.OrganizationID=C.OrganizationID";
            }
            SqlParameter[] parameters = { new SqlParameter("@organizationId", organizationId) };
            result = _dataFactory.Query(queryString, parameters);

            return result;
        }

        public static DataTable GetVariableNameData()
        {
            DataTable result;

            string queryString = @"select VariableId,VariableName from system_EnergyDataManualInputContrast";
            result = _dataFactory.Query(queryString);

            return result;
        }

        public static int AddEnergyDataManualInput(string addData)
        {
            int result = 0;

//            string testSql = @"select * 
//                                from system_EnergyDataManualInput A,system_Organization B,
//                                (select LevelCode from system_Organization where OrganizationID=@organizationId) C
//                                where A.OrganizationID=B.OrganizationID
//                                and B.LevelCode like C.LevelCode+'%'
//                                and A.TimeStamp = @datetime
//                                and A.VariableId=@variableId
//                                and A.UpdateCycle=@updateCycle";
//            string updateCycle = addData.JsonPick("updateCycle");
//            string organizationId = addData.JsonPick("organizationId");
//            string variableId = addData.JsonPick("variableId");
//            string timeStamp = addData.JsonPick("timeStamp");
//            if (updateCycle != "day")
//            {
//                string[] datetimearry=timeStamp.Split('-');
//                timeStamp = datetimearry[0] + "-" + datetimearry[1];
//            }
//           // string[] datetimearry = addData.JsonPick("timeStamp").Split('-');
//            SqlParameter[] testparameters = { new SqlParameter("datetime",timeStamp),
//                                            new SqlParameter("updateCycle",updateCycle),
//                                            new SqlParameter("organizationId",organizationId),
//                                            new SqlParameter("variableId",variableId)};
//            DataTable testTable = _dataFactory.Query(testSql, testparameters);

            string updateCycle = addData.JsonPick("updateCycle");
            string organizationId = addData.JsonPick("organizationId");
            string variableId = addData.JsonPick("variableId");
            string timeStamp = addData.JsonPick("timeStamp");
            if (updateCycle != "day")
            {
                string[] datetimearry = timeStamp.Split('-');
                timeStamp = datetimearry[0] + "-" + datetimearry[1];
            }
            if (CheckData(addData)> 0)
            {
                return -2;
            }
            else
            {
                string insertSql = @"insert into system_EnergyDataManualInput (DataItemId,VariableId,OrganizationID,TimeStamp,DataValue,UpdateCycle,Version,Remark) 
                                values (@dataItemId,@variableId,@organizationID,@timeStamp,@dataValue,@updateCycle,@version,@remark)";
                IList<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@dataItemId", Guid.NewGuid()));
                parameters.Add(new SqlParameter("@variableId", variableId));
                parameters.Add(new SqlParameter("@organizationID", organizationId));
                parameters.Add(new SqlParameter("@timeStamp", timeStamp));
                parameters.Add(new SqlParameter("@dataValue", addData.JsonPick("dataValue")));
                parameters.Add(new SqlParameter("@updateCycle", updateCycle));
                parameters.Add(new SqlParameter("@version", 2));
                parameters.Add(new SqlParameter("@remark", addData.JsonPick("remark")));

                result = _dataFactory.ExecuteSQL(insertSql, parameters.ToArray());

                return result;
            }
        }

        public static int DeleteEnergyDataManualInput(string id)
        {
            int result = 0;
            string deleteString = @"delete from system_EnergyDataManualInput where DataItemId=@dataItemId";
            SqlParameter[] parameters = { new SqlParameter("@dataItemId", id) };

            result = _dataFactory.ExecuteSQL(deleteString, parameters);

            return result;
        }

        public static int EditEnergyDataManualInput(string editData)
        {
            int result = 0;
            if (CheckData(editData) > 0)
            {
                return -2;
            }
            string updateCycle = editData.JsonPick("updateCycle");
            string organizationId = editData.JsonPick("organizationId");
            string variableId = editData.JsonPick("variableId");
            string timeStamp = editData.JsonPick("timeStamp");
            if (updateCycle != "day")
            {
                string[] datetimearry = timeStamp.Split('-');
                timeStamp = datetimearry[0] + "-" + datetimearry[1];
            }
            string updateSql = @"update system_EnergyDataManualInput set DataValue=@dataValue,
                               TimeStamp=@timeStamp,Remark=@remark where DataItemId=@dataItemId";
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@dataItemId", editData.JsonPick("dataItemId")));
            parameters.Add(new SqlParameter("@dataValue", editData.JsonPick("dataValue")));
            parameters.Add(new SqlParameter("@timeStamp", timeStamp));
            parameters.Add(new SqlParameter("@remark", editData.JsonPick("remark")));

            result = _dataFactory.ExecuteSQL(updateSql, parameters.ToArray());

            return result;
        }

        private static int CheckData(string myData)
        {
            string testSql = @"select * 
                                from system_EnergyDataManualInput A,system_Organization B,
                                (select LevelCode from system_Organization where OrganizationID=@organizationId) C
                                where A.OrganizationID=B.OrganizationID
                                and B.LevelCode like C.LevelCode+'%'
                                and A.TimeStamp = @datetime
                                and A.VariableId=@variableId
                                and A.UpdateCycle=@updateCycle";
            string updateCycle = myData.JsonPick("updateCycle");
            string organizationId = myData.JsonPick("organizationId");
            string variableId = myData.JsonPick("variableId");
            string timeStamp = myData.JsonPick("timeStamp");
            if (updateCycle != "day")
            {
                string[] datetimearry = timeStamp.Split('-');
                timeStamp = datetimearry[0] + "-" + datetimearry[1];
            }
            // string[] datetimearry = addData.JsonPick("timeStamp").Split('-');
            SqlParameter[] testparameters = { new SqlParameter("datetime",timeStamp),
                                            new SqlParameter("updateCycle",updateCycle),
                                            new SqlParameter("organizationId",organizationId),
                                            new SqlParameter("variableId",variableId)};
            DataTable testTable = _dataFactory.Query(testSql, testparameters);
            return testTable.Rows.Count;
        }
    }
}
