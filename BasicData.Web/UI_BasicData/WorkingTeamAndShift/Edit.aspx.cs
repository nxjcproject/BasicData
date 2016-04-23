using BasicData.Service.WorkingTeamAndShift;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using EasyUIJsonParser;

namespace BasicData.Web.UI_BasicData.WorkingTeamAndShift
{
    public partial class Edit : WebStyleBaseForEnergy.webStyleBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            ////////////////////调试用,自定义的数据授权
            //List<string> m_DataValidIdItems = new List<string>() { "C41B1F47-A48A-495F-A890-0AABB2F3BFF7                            ", "43F1EA8C-FF77-4BC5-BACB-531DC56A2512                            " };
            //AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
            this.OrganisationTree.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
            this.OrganisationTree.PageName = "Edit.aspx";                                                       //向web用户控件传递当前调用的页面名称
            this.OrganisationTree.LeveDepth = 5;
#if DEBUG
            ////////////////////调试用,自定义的数据授权
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
            //页面操作权限控制
            mPageOpPermission = "0100";
#endif
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
        public static string GetChargeManComboboxValue(string organizationId)
        {
            DataTable dt = WorkingTeamAndShiftService.GetStaffInfo(organizationId);
            string result = EasyUIJsonParser.ComboboxJsonParser.DataTableToJson(dt);
            return result;
        }

        [WebMethod]
        public static string SaveShifts(string organizationId,string shifts)
        {
            if (mPageOpPermission.ToArray()[2] == '1')
            {
                string[] sourceData = shifts.JsonPickArray("rows");
                int flag = WorkingTeamAndShiftService.SaveShiftInfo(organizationId, sourceData);

                if (flag == 1)
                    return "1";
                else
                    return "-1";
            }
            else
            {
                return "noright";
            }
        }

        [WebMethod]
        public static string SaveWorkingTeams(string organizationId,string workingTeams)
        {
            if (mPageOpPermission.ToArray()[2] == '1')
            {
                string[] souceData = workingTeams.JsonPickArray("rows");

                int result = WorkingTeamAndShiftService.SaveWorkingTeamInfo(organizationId, souceData);

                if (result != -1)
                    return "1";
                else
                    return "-1";
            }
            else
            {
                return "noright";
            }
        }

        [WebMethod]
        public static string QueryShifts(string organizationId)
        {
            DataTable sourcedt = WorkingTeamAndShiftService.QueryShiftsInfo(organizationId);
            string result = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(sourcedt);
            return result;
        }
        [WebMethod]
        public static string QueryWorkingTeam(string organizationId)
        {
            DataTable sourcedt = WorkingTeamAndShiftService.QueryWorkingTeamInfo(organizationId);
            string result = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(sourcedt);
            return result;
        }
    }
}