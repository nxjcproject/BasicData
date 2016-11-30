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
    public partial class EnergyConsumptionResult : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc", "zc_nxjc_ychc" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "EnergyConsumptionPlan.aspx";                                     //向web用户控件传递当前调用的页面名称
            }
        }

        [WebMethod]
        public static string GetEnergyInfo(string myOrganizationId, string myPlanYear)
        {
            string m_PlanType = "Energy";
            string[] m_ColumnText = new string[] { "指标项ID", "主要设备ID", "指标项目名称", "类别", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月", "年度合计", "备注" };
            int[] m_ColumnWidth = new int[] { 180, 180, 180, 60, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 100, 180 };

            DataTable m_EnergyPlanInfo = BasicData.Service.EnergyConsumption.EnergyConsumptionResult.GetEnergyPlanInfo(myOrganizationId, myPlanYear, m_PlanType);
            if (m_EnergyPlanInfo != null)
            {
                DataTable m_EnergyResultTable = BasicData.Service.EnergyConsumption.EnergyConsumptionResult.GetEnergyResultInfo(myOrganizationId, myPlanYear, m_PlanType, m_EnergyPlanInfo);

                int m_TableRowCount = m_EnergyPlanInfo.Rows.Count;
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
                    DataRow m_DataRow = m_EnergyPlanInfo.NewRow();

                    bool m_ContainEnergyResultTemp = false;
                    if (m_EnergyResultTable != null)
                    {
                        for (int j = 0; j < m_EnergyResultTable.Rows.Count; j++)
                        {
                            if (m_EnergyResultTable.Rows[j]["QuotasID"].ToString() == m_EnergyPlanInfo.Rows[2 * i]["QuotasID"].ToString())
                            {
                                m_DataRow[16] = 0;
                                m_DataRow[0] = m_EnergyResultTable.Rows[j][0];
                                m_DataRow[1] = m_EnergyResultTable.Rows[j][1];
                                m_DataRow[2] = m_EnergyResultTable.Rows[j][2];
                                m_DataRow[3] = m_EnergyResultTable.Rows[j][3];
                                for (int z = 0; z < 12; z++)
                                {
                                    decimal m_ValueTemp = m_EnergyResultTable.Rows[j][z + 4] != DBNull.Value ? (decimal)m_EnergyResultTable.Rows[j][z + 4] : 0.0m;
                                    m_DataRow[z + 4] = m_EnergyResultTable.Rows[j][z + 4];
                                    m_DataRow[16] = m_ValueTemp + (decimal)m_DataRow[16];
                                }
                                m_ContainEnergyResultTemp = true;
                                break;
                            }
                        }
                    }
                    if (m_ContainEnergyResultTemp == false)
                    {
                        m_DataRow[16] = 0;
                        for (int z = 0; z < 12; z++)
                        {
                            m_DataRow[z + 4] = 0;
                        }
                    }

                    m_EnergyPlanInfo.Rows.InsertAt(m_DataRow, i * 2 + 1);
                }
            }
            string m_Rows = EasyUIJsonParser.DataGridJsonParser.GetDataRowJson(m_EnergyPlanInfo);
            StringBuilder m_Columns = new StringBuilder();
            if (m_Rows == "")
            {
                m_Rows = "\"rows\":[],\"total\":0";
            }
            m_Columns.Append("\"columns\":[");
            for (int i = 0; i < m_EnergyPlanInfo.Columns.Count; i++)
            {
                m_Columns.Append("{");
                m_Columns.Append("\"width\":\"" + m_ColumnWidth[i] + "\"");
                m_Columns.Append(", \"title\":\"" + m_ColumnText[i] + "\"");
                m_Columns.Append(", \"field\":\"" + m_EnergyPlanInfo.Columns[i].ColumnName.ToString() + "\"");
                if (i == 16)   //屏蔽合计
                {
                    m_Columns.Append(", \"hidden\":true");
                }
                m_Columns.Append("}");
                if (i < m_EnergyPlanInfo.Columns.Count - 1)
                {
                    m_Columns.Append(",");
                }
            }
            m_Columns.Append("]");

            return "{" + m_Rows + "," + m_Columns + "}";
        }
    }
}
