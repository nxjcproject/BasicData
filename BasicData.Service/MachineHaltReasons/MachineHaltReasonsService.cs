using BasicData.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicData.Service.MachineHaltReasons
{
    public class MachineHaltReasonsService
    {
        /// <summary>
        /// 获取所有的停机原因
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMachineHaltReasons()
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Query query = new Query("system_MachineHaltReason");
            query.AddOrderByClause(new SqlServerDataAdapter.Infrastruction.OrderByClause("MachineHaltReasonID", false));

            return factory.Query(query);
        }

        /// <summary>
        /// 保存停机原因
        /// </summary>
        public static void SaveMachineHaltReasons(DataTable data)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Delete delete = new Delete("system_MachineHaltReason");
            factory.Remove(delete);
            Query query = new Query("system_MachineHaltReason");
            DataTable m_ReasonTable = factory.Query(query);
            if (query != null)
            {
                data.Columns.Add("OrganizationID", typeof(string));
                foreach (DataColumn Column in m_ReasonTable.Columns)
                {
                    int m_ColumnIndex = Column.Ordinal;
                    data.Columns[Column.ColumnName].SetOrdinal(m_ColumnIndex);
                }
            }

            factory.Save("system_MachineHaltReason", data);
        }
        public static string GetReasonStatisticsTypeInfo()
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(connectionString);
            string m_Sql = @"SELECT A.* from system_MachineHaltReasonStatisticsType A where A.Enabled = 1 order by A.LevelCode";
            m_Sql = string.Format(m_Sql);
            try
            {
                DataTable m_EquipmentCommonInfoTable = _dataFactory.Query(m_Sql);
                string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_EquipmentCommonInfoTable);
                return m_ReturnString;

            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }
    }
}
