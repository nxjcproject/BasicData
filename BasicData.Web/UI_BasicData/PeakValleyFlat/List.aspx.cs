using BasicData.Service.PeakValleyFlat;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using EasyUIJsonParser;

namespace BasicData.Web.UI_BasicData.PeakValleyFlat
{
    public partial class List : WebStyleBaseForEnergy.webStyleBase
    {
        private static char[] CRUD = mPageOpPermission.ToArray();
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            ////////////////////调试用,自定义的数据授权
            //List<string> m_DataValidIdItems = new List<string>() { "C41B1F47-A48A-495F-A890-0AABB2F3BFF7                            ", "43F1EA8C-FF77-4BC5-BACB-531DC56A2512                            " };
            //AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
            this.OrganisationTree.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
            this.OrganisationTree.PageName = "List.aspx";
#if DEBUG
            ////////////////////调试用,自定义的数据授权
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc" };
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
        public static string GetPVFList(string organizationId, string startUsing)
        {
            DataTable dt = PeakValleyFlatService.GetPVFList(organizationId, startUsing);
            string result = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(dt);
            return result;
        }

        [WebMethod]
        public static string DeletePVFList(string keyId)
        {
            if (CRUD[3] == '1')
            {
                int result = PeakValleyFlatService.DeletePVFData(keyId);

                if (result == 1)
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
        public static string UpdatePVFList(string id, string startUsing, string endUsing, string flag)
        {
            if (CRUD[2] == '1')
            {
                int result = PeakValleyFlatService.UpdatePVFData(id, startUsing, endUsing, flag);

                if (result == 1)
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
        public static string Save(string organizationId, string myJsonData)
        {
            if (CRUD[2] == '1')
            {
                string morganizationId = myJsonData.JsonPick("organizationId");
                //string startUsing = myJsonData.JsonPick("tzStartDate");
                string[] dataDetails = myJsonData.JsonPickArray("dataDetail");

                int result = PeakValleyFlatService.SavePVFData(organizationId, dataDetails);

                if (result == 1)
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
        public static string GetPVFDetail(string keyId)
        {
            DataTable dt = PeakValleyFlatService.GetPVFDetail(keyId);
            string result = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(dt);
            return result;
        }
    }
}