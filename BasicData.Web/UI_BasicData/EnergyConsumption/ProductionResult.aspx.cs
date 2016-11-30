using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Web.Services;
using WebStyleBaseForEnergy;

namespace BasicData.Web.UI_BasicData.EnergyConsumption
{
    public partial class ProductionResult : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_ychc","zc_nxjc_byc" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                mPageOpPermission = "1111";
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "ProductionResult.aspx";                                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
            }
        }
        /// <summary>
        /// 增删改查权限控制
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static char[] AuthorityControl()
        {
            return mPageOpPermission.ToArray();
        }
        [WebMethod]
        public static string GetEquipmentInfo(string myOrganizationId)
        {
            DataTable m_EquipmentInfo = BasicData.Service.EnergyConsumption.ProductionResult.GetEquipmentInfo(myOrganizationId);
            string m_EquipmentInfoString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_EquipmentInfo);
            return m_EquipmentInfoString;
        }
        [WebMethod]
        public static string GetQuotasInfo(string myEquipmentCommonId)
        {
            DataTable m_QuotasInfo = BasicData.Service.EnergyConsumption.ProductionResult.GetQuotasInfo(myEquipmentCommonId);
            string m_QuotasInfoString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_QuotasInfo);
            return m_QuotasInfoString;
        }
        [WebMethod]
        public static string GetProductionResultInfo(string myProductionQuotasId, string myQuotasType, string myOrganizationId, string myPlanYear, string myEquipmentCommonId)
        {
            string[] m_ColumnText = new string[] { "指标项ID", "主要设备ID", "指标项目名称", "类别", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月", "年度合计", "备注" };
            int[] m_ColumnWidth = new int[] { 180, 180, 180, 60, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 100, 180 };

            DataTable m_ProductionPlanInfo = BasicData.Service.EnergyConsumption.ProductionResult.GetProductionPlanInfo(myProductionQuotasId, myOrganizationId, myPlanYear, myEquipmentCommonId);
            if (m_ProductionPlanInfo != null)
            {
                DataTable m_ProductionResultTable = BasicData.Service.EnergyConsumption.ProductionResult.GetProductionResultInfo(myProductionQuotasId, myQuotasType, myOrganizationId, myPlanYear, myEquipmentCommonId);

                int m_TableRowCount = m_ProductionPlanInfo.Rows.Count;
                //int m_CurrentYear = Int32.Parse(myPlanYear);
                //int m_MaxMonth = 0;
                //if (m_CurrentYear < DateTime.Now.Year)
                //{
                //    m_MaxMonth = 12;
                //}
                //else if(m_CurrentYear == DateTime.Now.Year)
                //{
                //    m_MaxMonth = DateTime.Now.Month;
                //}
                for (int i = 0; i < m_TableRowCount; i++)
                {
                    DataRow m_DataRow = m_ProductionPlanInfo.NewRow();
                    m_DataRow[3] = "实绩";
                    bool m_ContainProductionResultTemp = false;
                    if (m_ProductionResultTable != null)
                    {
                        for (int j = 0; j < m_ProductionResultTable.Rows.Count; j++)
                        {
                            if (m_ProductionResultTable.Rows[j]["EquipmentId"].ToString() == m_ProductionPlanInfo.Rows[2*i]["EquipmentId"].ToString())
                            {
                                m_DataRow[16] = 0;
                                for (int z = 0; z < 12; z++)
                                {
                                    decimal m_ValueTemp = m_ProductionResultTable.Rows[j][z + 1] != DBNull.Value ? (decimal)m_ProductionResultTable.Rows[j][z + 1] : 0.0m;
                                    m_DataRow[z + 4] = m_ProductionResultTable.Rows[j][z + 1];
                                    m_DataRow[16] = m_ValueTemp + (decimal)m_DataRow[16];
                                }
                                m_ContainProductionResultTemp = true;
                                break;
                            }
                        }
                    }
                    if (m_ContainProductionResultTemp == false)
                    {
                        m_DataRow[16] = 0;
                        for (int z = 0; z < 12; z++)
                        {
                            m_DataRow[z + 4] = 0;
                        }
                    }

                    m_ProductionPlanInfo.Rows.InsertAt(m_DataRow, i * 2 + 1);
                }
            }
            string m_Rows = EasyUIJsonParser.DataGridJsonParser.GetDataRowJson(m_ProductionPlanInfo);
            StringBuilder m_Columns = new StringBuilder();
            if (m_Rows == "")
            {
                m_Rows = "\"rows\":[],\"total\":0";
            }
            m_Columns.Append("\"columns\":[");
            for (int i = 0; i < m_ProductionPlanInfo.Columns.Count; i++)
            {
                m_Columns.Append("{");
                m_Columns.Append("\"width\":\"" + m_ColumnWidth[i] + "\"");
                m_Columns.Append(", \"title\":\"" + m_ColumnText[i] + "\"");
                m_Columns.Append(", \"field\":\"" + m_ProductionPlanInfo.Columns[i].ColumnName.ToString() + "\"");
                if (i == 16)   //屏蔽合计
                {
                    m_Columns.Append(", \"hidden\":true");
                }
                m_Columns.Append("}");
                if (i < m_ProductionPlanInfo.Columns.Count - 1)
                {
                    m_Columns.Append(",");
                }
            }
            m_Columns.Append("]");

            return "{" + m_Rows + "," + m_Columns + "}";
        }
    }
}


/*
Select M.EquipmentID, M.ReasonID, sum(M.HaltLong)
 from (SELECT B.OrganizationID
      ,A.EquipmentID
	  ,DATEDIFF (hour, A.HaltTime, case when A.RecoverTime is null then getdate() else A.RecoverTime end) as HaltLong
      ,substring(A.ReasonID,1,3) as ReasonID
  FROM shift_MachineHaltLog A, system_Organization B, system_Organization C
  where A.HaltTime >= '2015-06-01'
  and A.HaltTime <= '2015-12-31'
  and (A.ReasonID like 'E01%' or A.ReasonID like 'E02%')
  and B.OrganizationID = 'zc_nxjc_qtx_efc'
  and C.LevelCode like B.LevelCode + '%'
  and A.OrganizationID = C.OrganizationID) M, equipment_EquipmentDetail N
  where M.OrganizationID = N.OrganizationID
  and M.EquipmentID = N.EquipmentID
  group by M.EquipmentID, M.ReasonID
 * 
 * 
 * 
 * 
 * 统计停机时间
 SELECT A.MachineHaltLogID
      ,B.OrganizationID
      ,A.EquipmentID
	  ,A.HaltTime
	  ,A.RecoverTime
	  ,substring(A.ReasonID,1,3) as ReasonID
	  ,(case when convert(varchar(7),A.HaltTime,20) < D.PreMonth then  CONVERT(datetime,D.PreMonth + '-01 00:00:00') else A.HaltTime end) as HaltTimeF
	  ,(case when convert(varchar(7),A.RecoverTime,20) > D.PreMonth then dateadd(day,-1, dateadd(month, 1, CONVERT(datetime,D.PreMonth + '-01 23:59:59'))) else A.RecoverTime end) as RecoverTimeF
	  ,DATEDIFF (hour, (case when convert(varchar(7),A.HaltTime,20) < D.PreMonth then  CONVERT(datetime,D.PreMonth + '-01 00:00:00') else A.HaltTime end)
	  ,(case when convert(varchar(7),A.RecoverTime,20) > D.PreMonth then dateadd(day,-1, dateadd(month, 1, CONVERT(datetime,D.PreMonth + '-01 23:59:59'))) else A.RecoverTime end)) as HaltLong
  FROM shift_MachineHaltLog A, system_Organization B, system_Organization C, 
  (select convert(varchar(7),dateadd(month,number,'2015-01-01'),120) as PreMonth
      from master..spt_values
      where type='P'
      and dateadd(month,number,'2015-01-01')<='2015-12-31') D
  where A.HaltTime >= '2015-06-01'
  and A.HaltTime <= '2015-12-31'
  and (A.ReasonID like 'E01%' or A.ReasonID like 'E02%')
  and B.OrganizationID = 'zc_nxjc_qtx_efc'
  and C.LevelCode like B.LevelCode + '%'
  and A.OrganizationID = C.OrganizationID
  and convert(varchar(7),A.HaltTime,20) <= D.PreMonth and convert(varchar(7),RecoverTime,20) >= D.PreMonth

*/