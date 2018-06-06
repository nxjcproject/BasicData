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
        // private static string nodeID = ""; 
        public static DataTable GetIndustryEnergy_SHRoles()
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            DataTable result;
            string querySql = "";
            querySql = "select [ROLE_ID] AS ID,[ROLE_NAME] AS Name from [IndustryEnergy_SH].[dbo].[roles]";
            result = dataFactory.Query(querySql);
            return result;
        }
        public static string GetPageID(string nodeID)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string SqlPage = @"SELECT [PAGE_ID]FROM [IndustryEnergy_SH].[dbo].[content] A where A.[NODE_ID]=@nodeID
                          ";
            SqlParameter mPara = new SqlParameter("nodeID", nodeID);
            DataTable dtPage = dataFactory.Query(SqlPage, mPara);
            string mPageId = "";
            if (dtPage.Rows.Count != 0)
            {
                mPageId = dtPage.Rows[0]["PAGE_ID"].ToString();
            }
            return mPageId;
        }
        //根据对照表角色分组显示变量

        public static DataTable GetSystemVariableTypeList(string nodeID, string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mPageId = GetPageID(nodeID);//获取page_ID
            DataTable resultTable = new DataTable();
            //            string mSql = @"(SELECT A.[VariableId],A.[VariableName] 
            //                               FROM system_EnergyDataManualInputContrast A,
            //                                    [IndustryEnergy_SH].[dbo].[content] B
            //                               WHERE 
            //                                    A.[GroupId]=B.[PAGE_ID] 
            //                                    AND B.[PAGE_ID]=@m_pageid 
            //                                    AND A.[Enabled]=1)
            //                               union 
            //                                  (SELECT [VariableId],[VariableName]
            //                                   FROM system_EnergyDataManualInputContrast
            //                                   WHERE 
            //                                        [GroupId] is NULL 
            //                                    AND [Enabled]=1)";
            string mSql = @"SELECT D.OrganizationID + A.[VariableId] as ItemId, A.[VariableId],D.Name + '_' +  A.[VariableName] as [VariableName],A.[TYPE], D.LevelCode, D.OrganizationID
                               FROM system_EnergyDataManualInputContrast A,
                                    [IndustryEnergy_SH].[dbo].[content] B, system_Organization C, system_Organization D
                               WHERE C.OrganizationID = @m_organizationId
								    AND D.LevelCode like C.LevelCode + '%'
									AND A.[Type] = D.[Type]
                                    AND A.[GroupId]=B.[PAGE_ID] 
                                    AND B.[PAGE_ID]=@m_pageid
                                    AND A.[Enabled]=1
                               union 
                                  SELECT C.OrganizationID + A.[VariableId] as ItemId, A.[VariableId], C.Name + '_' +  A.[VariableName] as [VariableName],A.[TYPE], C.LevelCode, C.OrganizationID
                                   FROM system_EnergyDataManualInputContrast A, system_Organization B, system_Organization C
                                   WHERE B.OrganizationID = @m_organizationId
								    AND C.LevelCode like B.LevelCode + '%'
									AND A.[Type] = C.[Type]
                                    AND A.[GroupId] is NULL 
                                    AND A.[Enabled]=1
                            order by [LevelCode], [Type], [VariableId]";
            SqlParameter[] para = new SqlParameter[]{new SqlParameter("m_pageid", mPageId)
                                                     ,new SqlParameter("m_organizationId", organizationId)};
            resultTable = dataFactory.Query(mSql, para);
            //}
            return resultTable;
        }
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
                string insertSql = @"insert into system_EnergyDataManualInputContrast (VariableId,VariableName,Type,Enabled,Creator,CreateTime,Remark,GroupId) 
                                values (@variableId,@variableName,@type,@enabled,@creator,@createTime,@remark,@role)";
                IList<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@variableId", addData.JsonPick("variableId")));
                parameters.Add(new SqlParameter("@variableName", addData.JsonPick("variableName")));
                parameters.Add(new SqlParameter("@type", addData.JsonPick("type")));
                parameters.Add(new SqlParameter("@enabled", addData.JsonPick("enabled")));
                parameters.Add(new SqlParameter("@creator", addData.JsonPick("creator")));
                parameters.Add(new SqlParameter("@createTime", addData.JsonPick("createTime")));
                parameters.Add(new SqlParameter("@remark", addData.JsonPick("remark")));
                parameters.Add(new SqlParameter("@role", addData.JsonPick("role")));
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
            SqlParameter[] parameters = { new SqlParameter("@variableId", variableId) };

            int result = _dataFactory.ExecuteSQL(deleteSql, parameters);
            return result;
        }

        public static int EditEnergyDataManualInputContrast(string editData)
        {
            int result = 0;
            string updateSql = @"update system_EnergyDataManualInputContrast set VariableName=@variableName,Enabled=@enabled,
                                Creator=@creator,CreateTime=@createTime,Remark=@remark,GroupId=@role,Type=@type where VariableId=@variableId";
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@variableId", editData.JsonPick("variableId")));
            parameters.Add(new SqlParameter("@variableName", editData.JsonPick("variableName")));
            parameters.Add(new SqlParameter("@enabled", editData.JsonPick("enabled")));
            parameters.Add(new SqlParameter("@creator", editData.JsonPick("creator")));
            parameters.Add(new SqlParameter("@createTime", editData.JsonPick("createTime")));
            parameters.Add(new SqlParameter("@remark", editData.JsonPick("remark")));
            parameters.Add(new SqlParameter("@role", editData.JsonPick("role")));
            parameters.Add(new SqlParameter("@type", editData.JsonPick("type")));

            result = _dataFactory.ExecuteSQL(updateSql, parameters.ToArray());

            return result;
        }

        //        public static DataTable GetEnergyDataManualInput(string organizationId)
        //        {
        //            DataTable result;
        //            string queryString;
        //            if (organizationId != "")
        //            {
        //                queryString = @"select A.DataItemId,A.OrganizationID,C.Name,A.VariableId,B.VariableName,A.DataValue,A.TimeStamp,A.UpdateCycle,A.Version,A.Remark
        //                                from system_EnergyDataManualInput as A 
        //                                left join system_EnergyDataManualInputContrast as B 
        //                                on A.VariableId=B.VariableId
        //                                left join system_Organization as C on A.OrganizationID=C.OrganizationID
        //                                where A.OrganizationID=@organizationId";
        //            }
        //            else
        //            {
        //                queryString = @"select A.DataItemId,A.OrganizationID,C.Name,A.VariableId,B.VariableName,A.DataValue,A.TimeStamp,A.UpdateCycle,A.Version,A.Remark
        //                                from system_EnergyDataManualInput as A 
        //                                left join system_EnergyDataManualInputContrast as B 
        //                                on A.VariableId=B.VariableId
        //                                left join system_Organization as C on A.OrganizationID=C.OrganizationID";
        //            }
        //            SqlParameter[] parameters = { new SqlParameter("@organizationId", organizationId) };
        //            result = _dataFactory.Query(queryString, parameters);

        //            return result;
        //        }
        //input表查询
        public static DataTable GetEnergyDataManualInput(string organizationId, string startTime, string endTime, string nodeID)
        {
            DataTable result;
            string queryString;
            string mPageId = GetPageID(nodeID);

            if (organizationId != "")
            {
                //                queryString = @"select * from (select A.DataItemId,A.OrganizationID,C.Name,A.VariableId,B.VariableName,A.DataValue,A.TimeStamp,A.UpdateCycle,A.Version,A.Remark
                //                                from system_EnergyDataManualInput as A 
                //                                left join system_EnergyDataManualInputContrast as B 
                //                                on A.VariableId=B.VariableId
                //                                left join system_Organization as C on A.OrganizationID=C.OrganizationID
                //                                where A.OrganizationID=@organizationId) as D where D.TimeStamp>@startTime and D.TimeStamp<@endTime order by D.TimeStamp desc,D.VariableName asc";

                queryString = @"select A.DataItemId,D.Name,A.VariableId,B.VariableName,A.DataValue,A.TimeStamp,A.UpdateCycle,A.Version,A.Remark
                             from system_EnergyDataManualInput as A,
                                              (SELECT A.[VariableId],A.[VariableName] 
                                              FROM system_EnergyDataManualInputContrast A,[IndustryEnergy_SH].[dbo].[content] B
                                              WHERE A.[GroupId]=B.[PAGE_ID] AND B.[PAGE_ID]=@m_pageid
                                              union SELECT [VariableId],[VariableName]
                                              FROM system_EnergyDataManualInputContrast WHERE [GroupId] is NULL)  
                              AS B,system_Organization as C, system_Organization as D where A.VariableId=B.VariableId 
                                           and A.OrganizationID=D.OrganizationID and C.OrganizationID=@organizationId and D.LevelCode like C.LevelCode + '%'
                                           and A.TimeStamp>@startTime and A.TimeStamp<@endTime order by A.TimeStamp desc,D.LevelCode,B.VariableName asc";
            }
            else
            {
                //                queryString = @"select *from (select A.DataItemId,A.OrganizationID,C.Name,A.VariableId,B.VariableName,A.DataValue,A.TimeStamp,A.UpdateCycle,A.Version,A.Remark
                //                               from system_EnergyDataManualInput as A 
                //                                left join system_EnergyDataManualInputContrast as B 
                //                               on A.VariableId=B.VariableId
                //                               left join system_Organization as C on A.OrganizationID=C.OrganizationID) AS D where D.TimeStamp>@startTime and D.TimeStamp<@endTime order by D.TimeStamp desc,D.VariableName asc";
                queryString = @"select A.DataItemId,D.Name,A.VariableId,B.VariableName,A.DataValue,A.TimeStamp,A.UpdateCycle,A.Version,A.Remark
                             from system_EnergyDataManualInput as A,
                                              (SELECT A.[VariableId],A.[VariableName] 
                                              FROM system_EnergyDataManualInputContrast A,[IndustryEnergy_SH].[dbo].[content] B
                                              WHERE A.[GroupId]=B.[PAGE_ID] AND B.[PAGE_ID]=@m_pageid
                                              union SELECT [VariableId],[VariableName]
                                              FROM system_EnergyDataManualInputContrast WHERE [GroupId] is NULL)  
                              AS B,system_Organization as C where A.VariableId=B.VariableId and A.OrganizationID=C.OrganizationID 
                                              and A.TimeStamp>@startTime and A.TimeStamp<@endTime order by A.TimeStamp desc,D.LevelCode,B.VariableName asc";
            }
            SqlParameter[] parameters = { new SqlParameter("@organizationId", organizationId), new SqlParameter("@startTime", startTime), new SqlParameter("@endTime", endTime), new SqlParameter("@m_pageid", mPageId) };
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
            if (CheckData(addData) > 0)
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
