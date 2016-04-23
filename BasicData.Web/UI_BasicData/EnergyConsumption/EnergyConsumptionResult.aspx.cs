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
        public static string GetEnergyConsumptionInfo(string myLevelCode, string myPlanYear)
        {
            DataTable m_EnergyConsumptionResultInfo = null;
            string m_WarningMessage = BasicData.Service.EnergyConsumption.EnergyConsumptionResult.GetEnergyConsumptionResultInfo(myPlanYear, myLevelCode, ref m_EnergyConsumptionResultInfo);
            string aa = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(null);
            if (m_EnergyConsumptionResultInfo != null)
            {
                EasyUIJsonParser.Model_DataTableColumns[] m_Model_DataTableColumns = new EasyUIJsonParser.Model_DataTableColumns[m_EnergyConsumptionResultInfo.Columns.Count];
                for (int i = 0; i < m_EnergyConsumptionResultInfo.Columns.Count; i++)
                {
                    EasyUIJsonParser.Model_DataTableColumns m_Model_DataTableColumnItem = new EasyUIJsonParser.Model_DataTableColumns();
                    m_Model_DataTableColumnItem.ColumnAlign = EasyUIJsonParser.AlignText.right.ToString();
                    m_Model_DataTableColumnItem.ColumnHeaderAlign = EasyUIJsonParser.AlignText.center.ToString();
                    m_Model_DataTableColumnItem.ColumnText = m_EnergyConsumptionResultInfo.Columns[i].ColumnName;
                    m_Model_DataTableColumnItem.ColumnWidth = 150;
                    m_Model_DataTableColumns[i] = m_Model_DataTableColumnItem;
                }
                string ReturnJson = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_EnergyConsumptionResultInfo);
                //string m_Columns = EasyUIJsonParser.DataGridJsonParser.GetColumnsJson(m_EnergyConsumptionResultInfo, m_Model_DataTableColumns);
                return ReturnJson;
            }
            else
            {
                return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(null);
            }

            //string m_Columns = EasyUIJsonParser.DataGridJsonParser.GetColumnsJson(
            
        }
    }
}