using BasicData.Service.ShiftArrangement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web.UI_BasicData.ShiftArrangement
{
    public partial class ShiftArrangementEdit : WebStyleBaseForEnergy.webStyleBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
#if DEBUG
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_qtx_efc", "zc_nxjc_qtx_tys" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
            mPageOpPermission = "1111";
#elif RELEASE
#endif
            this.OrganisationTree.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
            this.OrganisationTree.PageName = "ShiftArrangementEdit.aspx";                                                       //向web用户控件传递当前调用的页面名称
            this.OrganisationTree.LeveDepth = 5;
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
        public static string GetData(string organizationId)
        {
            DataTable table = ShiftArrangementService.GetShiftArrangement(organizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }

        [WebMethod]
        public static string SaveData(string organizationId, string json)
        {
            if (mPageOpPermission.ToArray()[2] == '1')
            {
                int count = ShiftArrangementService.SaveShiftArrange(organizationId,json);
                if (count == -1)
                {
                    return "failure";
                }
                else
                {
                    return "success";
                }
            }
            else
            {
                return "noright";
            }
        }
    }
}