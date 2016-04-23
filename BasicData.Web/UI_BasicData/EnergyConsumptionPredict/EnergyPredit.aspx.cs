using BasicData.Service.EnergyConsumptionPredict;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web.UI_BasicData.EnergyConsumptionPredict
{
    public partial class EnergyPredit : WebStyleBaseForEnergy.webStyleBase
    {
        private const string REPORT_TEMPLATE_PATH = "\\ReportHeaderTemplate\\report_CementMilMonthlyEnergyConsumption.xml";
        private static DataTable myDataTable;

        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx_efc"};
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "EnergyConsumptionPlan.aspx";                                     //向web用户控件传递当前调用的页面名称
            }
        }

        /// <summary>
        /// 获得报表数据并转换为json
        /// </summary>
        /// <returns>column的json字符串</returns>
        [WebMethod]
        public static string GetReportData(string OrganizationId,string leveCode)
        {
            //string ord = "34d9ae54-a558-4420-bbf7-e25b5ba4ec21";
            //string ord = "zc_nxjc_qtx_efc_clinker02";
            //myDataTable = ClinkerMonthlyPeakerValleyFlatElectricityConsumption.TableQuery("df863854-89ae-46e6-80e8-96f6db6471b4", "2014-10");
            //try
            //{
                EnergyPredict energyPredict = new EnergyPredict();
                string nowDate = DateTime.Now.ToString("yyyy-MM");
               // string nowDate = "2014-12";
                energyPredict.Get_Forecast_ProductionLineEnergy(OrganizationId,nowDate);
                if (leveCode.Length == 7)
                {
                    myDataTable = energyPredict.DT.Select("Name<>'所有生产线'").CopyToDataTable();
                }
                else
                {
                    myDataTable = energyPredict.DT.Select("Name='所有生产线'").CopyToDataTable();
                }
                string m_UserInfoJson = EasyUIJsonParser.DataGridJsonParser.GetDataRowJson(myDataTable);
                return "{" + m_UserInfoJson + "}";
            
            //}
            //catch 
            //{
            //    return "";
            //};
        }

        /// <summary>
        /// 获得报表数据并转换为json
        /// </summary>
        /// <returns>column的json字符串</returns>
        //[WebMethod]
        //public static string PrintFile()
        //{
        //    string[] m_TagData = new string[] { "10月份", "报表类型:日报表", "汇总人:某某某", "审批人:某某某" };
        //    string m_HtmlData = StatisticalReportHelper.CreatePrintHtmlTable(mFileRootPath +
        //        REPORT_TEMPLATE_PATH, myDataTable, m_TagData);
        //    return m_HtmlData;
        //}
    }
}