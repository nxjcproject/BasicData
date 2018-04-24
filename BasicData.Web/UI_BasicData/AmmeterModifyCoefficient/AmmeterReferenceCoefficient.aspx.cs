using BasicData.Service.AmmeterModifyCoefficient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web.UI_BasicData.AmmeterModifyCoefficient
{
    public partial class AmmeterReferenceCoefficient : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_ychc_yfcf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "EquipmentAccountEdit.aspx";                                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
            }
        }
        [WebMethod]
        public static string GetData(string organizationId, string startTime, string endTime)
        {
            DataTable table = ModifyCoefficientService.ReferenceCoefficientCalculate(organizationId, startTime, endTime);
            string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "LevelCode");
            return json;
        }
    }
}