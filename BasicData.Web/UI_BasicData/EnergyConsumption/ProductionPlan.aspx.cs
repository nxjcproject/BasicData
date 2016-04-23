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
    public partial class ProductionPlan : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_ychc" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                mPageOpPermission = "1111";
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "ProductionPlan.aspx";                                     //向web用户控件传递当前调用的页面名称
                HiddenField_PlanType.Value = "Production";    //HttpContext.Current.Request.QueryString["id"];
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
        public static string GetProductionPlanInfo(string myProductionPlanType, string myOrganizationId, string myPlanYear, string myPlanType)
        {
            string[] m_ColumnText = new string[] { "指标项ID", "主要设备ID", "指标项目名称", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月", "年度合计", "备注" };
            int[] m_ColumnWidth = new int[] { 180, 180, 180, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 100, 180 };
            string[] m_FormatString = new string[] { "", "", "", "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}",
                "\"type\":\"textbox\", \"options\":{\"validType\":\"length[0,200]\", \"invalidMessage\":\"不能超过200个字符!\"}" };

            DataTable m_ProductionPlanInfo = BasicData.Service.EnergyConsumption.ProductionPlan.GetProductionPlanInfo(myProductionPlanType, myOrganizationId, myPlanYear, myPlanType);

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
                if (m_FormatString[i] != "")
                {
                    m_Columns.Append(", \"editor\":{" + m_FormatString[i] + "}");
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
        [WebMethod]
        public static string SetProductionPlanInfo(string myOrganizationId, string myPlanYear, string myPlanType, string myProductionPlanType, string myDataGridData)
        {
            if (mPageOpPermission.ToArray()[2] == '1')
            {
                DataTable m_DataGridDataStruct = BasicData.Service.EnergyConsumption.ProductionPlan.CreateTableStructure("plan_ProductionYearlyPlan");
                //m_DataGridDataStruct.Columns.Remove("QuotasItemID");       //去掉ID列，此列的数据由数据库自动生成
                string[] m_DataGridDataGroup = EasyUIJsonParser.Utility.JsonPickArray(myDataGridData, "rows");
                DataTable m_DataGridData = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(m_DataGridDataGroup, m_DataGridDataStruct);
                if (m_DataGridData != null)
                {
                    for (int i = 0; i < m_DataGridData.Rows.Count; i++)
                    {
                        m_DataGridData.Rows[i]["Totals"] = 0;
                        for (int j = 0; j < 12; j++)
                        {
                            if (m_DataGridData.Rows[i][j + 6] != DBNull.Value)
                            {
                                m_DataGridData.Rows[i]["Totals"] = (decimal)m_DataGridData.Rows[i]["Totals"] + (decimal)m_DataGridData.Rows[i][j + 6];
                            }
                        }
                    }
                }
                ///////////////tz表里查找是否已经存在////////////////
                string m_KeyId = BasicData.Service.EnergyConsumption.ProductionPlan.GetKeyIdFromTz(myPlanYear, myOrganizationId, myPlanType);
                if (m_KeyId != "")               //表示计划已经存在
                {
                    BasicData.Service.EnergyConsumption.ProductionPlan.UpdateTzPlan(m_KeyId, mUserId);    //更新TZ引领表
                }
                else
                {
                    m_KeyId = Guid.NewGuid().ToString();
                    //添加TZ引领
                    BasicData.Service.EnergyConsumption.ProductionPlan.InsertTzPlan(m_KeyId, myOrganizationId, myPlanYear, mUserId, myPlanType);
                }

                for (int i = 0; i < m_DataGridData.Rows.Count; i++)
                {
                    string m_QuotasItemID = Guid.NewGuid().ToString();
                    m_DataGridData.Rows[i]["QuotasItemID"] = m_QuotasItemID;
                    m_DataGridData.Rows[i]["KeyID"] = m_KeyId;
                    m_DataGridData.Rows[i]["DisplayIndex"] = (i + 1).ToString();
                }
                BasicData.Service.EnergyConsumption.ProductionPlan.DeleteProductionPlanInfo(m_KeyId, myProductionPlanType);
                int ReturnValue = BasicData.Service.EnergyConsumption.ProductionPlan.SaveProductionPlanInfo("plan_ProductionYearlyPlan", m_DataGridData);
                ReturnValue = ReturnValue > 0 ? 1 : ReturnValue;
                return ReturnValue.ToString();
            }
            else
            {
                return "noright";
            }
        }
    }
}