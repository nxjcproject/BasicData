using BasicData.Service.KpiDefinition;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web.UI_BasicData.KPIDefinition
{
    public partial class KpiList : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
#if DEBUG
            // 调试用,自定义的数据授权
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx_efc", "zc_nxjc_byc" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
            mPageOpPermission = "0000";
#endif
            this.OrganisationTree.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
            this.OrganisationTree.PageName = "MaterialList.aspx";
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
        public static string GetKpiList(string statisticalMethod)
        {
            DataTable table = KpiService.GetKpiList(statisticalMethod);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
        }

        [WebMethod]
        public static string GetKpiDetail_All(string keyId)
        {
            DataTable table = KpiService.GetKpiDetail(keyId);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
        }

        [WebMethod]
        public static string GetKpiDetail_Public(string keyId)
        {
            DataTable table = KpiService.GetKpiDetail_Public(keyId);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
        }

        [WebMethod]
        public static string GetKpiDetail_Private(string keyId, string organizationId)
        {
            DataTable table = KpiService.GetKpiDetail_Private(keyId, organizationId);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
        }

        /// <summary>
        /// 创建KPI引领
        /// </summary>
        /// <param name="standardId"></param>
        /// <param name="statisticalMethod"></param>
        /// <param name="displayIndex"></param>
        /// <param name="creator"></param>
        [WebMethod]
        public static void CreateKpiList(string standardId, string statisticalMethod, int displayIndex)
        {
#if DEBUG
            WebStyleBaseForEnergy.webStyleBase.mUserId = "1";
#endif
            if (mPageOpPermission.ToArray()[1] == '1')
            {
                KpiService.CreateKpiList(standardId, statisticalMethod, displayIndex, WebStyleBaseForEnergy.webStyleBase.mUserId);
            }
        }

        [WebMethod]
        public static void DeleteKpiList(string keyId)
        {
            if (mPageOpPermission.ToArray()[3] == '1')
            {
                KpiService.DeleteKpiList(keyId);
            }
        }

        /// <summary>
        /// 创建KPI详细
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="name"></param>
        /// <param name="organizationId"></param>
        /// <param name="levelType"></param>
        /// <param name="variableId"></param>
        /// <param name="valueType"></param>
        /// <param name="unit"></param>
        /// <param name="standardValue"></param>
        /// <param name="standardLevel"></param>
        [WebMethod]
        public static void CreateKpiDetail(string keyId, string name, string organizationId, string levelType, string variableId, string valueType, string unit, decimal standardValue, int standardLevel)
        {
#if DEBUG
            WebStyleBaseForEnergy.webStyleBase.mUserId = "1";
#endif
            if (mPageOpPermission.ToArray()[1] == '1')
            {
                KpiService.CreateKpiDetail(keyId, name, organizationId, levelType, variableId, valueType, unit, standardValue, standardLevel, WebStyleBaseForEnergy.webStyleBase.mUserId);
            }
        }

        /// <summary>
        /// 更新KPI详细
        /// </summary>
        /// <param name="standardItemId"></param>
        /// <param name="name"></param>
        /// <param name="organizationId"></param>
        /// <param name="levelType"></param>
        /// <param name="variableId"></param>
        /// <param name="valueType"></param>
        /// <param name="unit"></param>
        /// <param name="standardValue"></param>
        /// <param name="standardLevel"></param>
        /// <param name="creator"></param>
        [WebMethod]
        public static void UpdateKpiDetail(string standardItemId, string name, string organizationId, string levelType, string variableId, string valueType, string unit, decimal standardValue, int standardLevel)
        {
            if (mPageOpPermission.ToArray()[2] == '1')
            {
                KpiService.UpdateKpiDetail(standardItemId, name, organizationId, levelType, variableId, valueType, unit, standardValue, standardLevel, WebStyleBaseForEnergy.webStyleBase.mUserId);
            }
        }

        [WebMethod]
        public static void DeleteKpiDetail(string standardItemId)
        {
            if (mPageOpPermission.ToArray()[3] == '1')
            {
                KpiService.DeleteKpiDetail(standardItemId);
            }
        }
    }
}