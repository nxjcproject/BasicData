using BasicData.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BasicData.Service.AmmeterModifyCoefficient
{
    public class AmmeterLevelMaintenanceService
    {

        public static DataTable GetAmmeterLevelInfo(string organizationId)
        {
            string AmmeterDBName = "";
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string ammeterDBNameSql = @"select A.OrganizationID,B.MeterDatabase,B.DCSProcessDatabase
                                        from system_Organization A,system_Database B
                                        where A.DatabaseID=B.DatabaseID
                                        and A.OrganizationID=@organizationId";
            SqlParameter ammeterDBParameter = new SqlParameter("organizationId", organizationId);
            DataTable ammeterDBTable = dataFactory.Query(ammeterDBNameSql, ammeterDBParameter);
            if (ammeterDBTable.Rows.Count == 1)
            {
                AmmeterDBName = ammeterDBTable.Rows[0]["MeterDatabase"].ToString().Trim();
            }
            else
            {
                throw new Exception("无该组织机构对应的电表数据库");
            }
            string mySql = @"select A.OrganizationID,A.LevelCode,A.AmmeterNumber,A.Remarks,rtrim(ltrim(B.AmmeterName)) as AmmeterName,rtrim(ltrim(B.ElectricRoom)) as ElectricRoom
                                from [{0}].[dbo].[AmmeterModifyCoefficientReference] A 
                                left join [{0}].[dbo].[AmmeterContrast] B
                                on A.OrganizationID=B.OrganizationID
                                and A.AmmeterNumber=B.AmmeterNumber
                                and A.OrganizationID=@organizationId";
            SqlParameter parameter = new SqlParameter("organizationId", organizationId);
            DataTable table = dataFactory.Query(string.Format(mySql,AmmeterDBName), parameter);
            return table;
        }
    }
}
