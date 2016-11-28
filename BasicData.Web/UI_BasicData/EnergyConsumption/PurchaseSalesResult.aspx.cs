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
    public partial class PurchaseSalesResult : WebStyleBaseForEnergy.webStyleBase
    {
        private const string myPlanType = "PurchaseSales";
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_ychc", "zc_nxjc_byc","zc_nxjc_tsc_tsf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                mPageOpPermission = "1111";
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "PurchaseSalesResult.aspx";                                     //向web用户控件传递当前调用的页面名称
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
        public static string GetPurchaseSalesResultInfo(string myOrganizationId, string myType, string myPlanYear)
        {
            string[] m_ColumnText = new string[] { "指标项ID", "变量ID", "指标项目名称", "类别", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月", "年度合计", "备注" };
            int[] m_ColumnWidth = new int[] { 180, 180, 180, 100, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 100, 180 };

            DataTable m_PurchaseSalesPlanInfo = BasicData.Service.EnergyConsumption.PurchaseSalesResult.GetPurchaseSalesPlanInfo(myOrganizationId, myType, myPlanType, myPlanYear);
            if (m_PurchaseSalesPlanInfo != null)
            {
                DataTable m_PurchaseSalesResultTable = BasicData.Service.EnergyConsumption.PurchaseSalesResult.GetPurchaseSalesResultInfo(myOrganizationId, myType, myPlanYear);

                int m_TableRowCount = m_PurchaseSalesPlanInfo.Rows.Count;
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
                    DataRow m_DataRow = m_PurchaseSalesPlanInfo.NewRow();
                    m_DataRow[3] = "实绩";
                    bool m_ContainPurchaseSalesResultTemp = false;
                    if (m_PurchaseSalesResultTable != null)
                    {
                        for (int j = 0; j < m_PurchaseSalesResultTable.Rows.Count; j++)
                        {
                            if (m_PurchaseSalesResultTable.Rows[j]["VariableId"].ToString() == m_PurchaseSalesPlanInfo.Rows[i]["VariableId"].ToString())
                            {
                                m_DataRow[16] = 0;
                                for (int z = 0; z < 12; z++)
                                {
                                    decimal m_ValueTemp = m_PurchaseSalesResultTable.Rows[j][z + 1] != DBNull.Value ? (decimal)m_PurchaseSalesResultTable.Rows[j][z + 1] : 0.0m;
                                    m_DataRow[z + 4] = m_PurchaseSalesResultTable.Rows[j][z + 1];
                                    m_DataRow[16] = m_ValueTemp + (decimal)m_DataRow[16];
                                }
                                m_ContainPurchaseSalesResultTemp = true;
                                break;
                            }
                        }
                    }
                    if (m_ContainPurchaseSalesResultTemp == false)
                    {
                        m_DataRow[16] = 0;
                        for (int z = 0; z < 12; z++)
                        {
                            m_DataRow[z + 4] = 0;
                        }
                    }

                    m_PurchaseSalesPlanInfo.Rows.InsertAt(m_DataRow, i * 2 + 1);
                }
            }
            string m_Rows = EasyUIJsonParser.DataGridJsonParser.GetDataRowJson(m_PurchaseSalesPlanInfo);
            StringBuilder m_Columns = new StringBuilder();
            if (m_Rows == "")
            {
                m_Rows = "\"rows\":[],\"total\":0";
            }
            m_Columns.Append("\"columns\":[");
            for (int i = 0; i < m_PurchaseSalesPlanInfo.Columns.Count; i++)
            {
                m_Columns.Append("{");
                m_Columns.Append("\"width\":\"" + m_ColumnWidth[i] + "\"");
                m_Columns.Append(", \"title\":\"" + m_ColumnText[i] + "\"");
                m_Columns.Append(", \"field\":\"" + m_PurchaseSalesPlanInfo.Columns[i].ColumnName.ToString() + "\"");
                if (i == 16)   //屏蔽合计
                {
                    m_Columns.Append(", \"hidden\":true");
                }
                m_Columns.Append("}");
                if (i < m_PurchaseSalesPlanInfo.Columns.Count - 1)
                {
                    m_Columns.Append(",");
                }
            }
            m_Columns.Append("]");

            return "{" + m_Rows + "," + m_Columns + "}";
        }
    }
}