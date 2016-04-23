using BasicData.Service.StaffInfo;
using EasyUIJsonParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web.UI_BasicData.StaffInfo
{
    public partial class Edit : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                // 调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_qtx_tys" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                mPageOpPermission = "0000";
#endif
                this.OrganisationTree.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree.PageName = "Edit.aspx";
                this.OrganisationTree.LeveDepth = 5;
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
        public static string GetStaffInfoWithDataGridFormat(string organizationId, string searchName, string searchId, string searchTeamName)
        {
            DataTable dt = StaffInfoService.GetStaffInfo(organizationId, searchName, searchId, searchTeamName);

            return DataGridJsonParser.DataTableToJson(dt);
        }

        [WebMethod]
        public static string InsertStaffInfo(string organizationId, string staffId, string name, bool sex, string workingTeam, string phoneNumber, bool enabled)
        {
            if (mPageOpPermission.ToArray()[1] == '1')
            {
                if (string.IsNullOrWhiteSpace(organizationId))
                    return "保存失败，请先选择分厂。";
                if (string.IsNullOrWhiteSpace(staffId))
                    return "职工ID不可为空。";
                if (string.IsNullOrWhiteSpace(name))
                    return "请输入姓名";

                return StaffInfoService.InsertStaffInfo(organizationId, staffId, workingTeam, name, sex, phoneNumber);
            }
            else
            {
                return "该用户没有增加权限！";
            }
        }

        [WebMethod]
        public static string UpdateStaffInfo(string organizationId, string staffId, string name, bool sex, string workingTeam, string phoneNumber, bool enabled)
        {
            if (mPageOpPermission.ToArray()[2] == '1')
            {
                if (string.IsNullOrWhiteSpace(organizationId))
                    return "保存失败，请刷新后重试。";
                if (string.IsNullOrWhiteSpace(staffId))
                    return "职工ID不可为空。";
                if (string.IsNullOrWhiteSpace(name))
                    return "请输入姓名";

                return StaffInfoService.UpdateStaffInfo(organizationId, staffId, workingTeam, name, sex, phoneNumber, enabled);
            }
            else
            {
                return "该用户没有修改权限！";
            }
        }
    }
}