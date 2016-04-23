using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web.UI_BasicData.EnergyDataManualInput
{
    public partial class EnergyDataManualInput : WebStyleBaseForEnergy.webStyleBase
    {
        private static char[] CRUD;
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();

#if DEBUG
            ////////////////////调试用,自定义的数据授权
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
            mPageOpPermission = "1011";
#elif RELEASE
#endif
            this.OrganisationTree.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
            this.OrganisationTree.PageName = "EnergyDataManualInput.aspx";                                     //向web用户控件传递当前调用的页面名称
            this.OrganisationTree.LeveDepth = 5;
            //控制网页的增删改查权限
            //低位共四位：查看、增加、修改、删除
            CRUD = mPageOpPermission.ToArray();
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
        public static string GetEnergyDataManualInputData(string organizationId)
        {
            DataTable dt = BasicData.Service.EnergyDataManualInput.EnergyDataManualInputService.GetEnergyDataManualInput(organizationId);
            string result = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(dt);
            return result;
        }
        [WebMethod]
        public static string GetVariableNameData()
        {           
            DataTable dt = BasicData.Service.EnergyDataManualInput.EnergyDataManualInputService.GetVariableNameData();
            string result = EasyUIJsonParser.ComboboxJsonParser.DataTableToJson(dt);
            return result;
        }
        [WebMethod]
        public static string AddEnergyDataManualInputData(string maddData)
        {
            if (CRUD[1] == '1')
            {
                int result = BasicData.Service.EnergyDataManualInput.EnergyDataManualInputService.AddEnergyDataManualInput(maddData);

                if (result == 1)
                    return "1";
                else if (result == -2)
                    return "-2";
                else
                    return "-1";
            }
            else
            {
                //没有增加权限
                return "noright";
            }
        }
        [WebMethod]
        public static string DeleteEnergyDataManualInputData(string id)
        {
            if (CRUD[3] == '1')
            {
                int result = BasicData.Service.EnergyDataManualInput.EnergyDataManualInputService.DeleteEnergyDataManualInput(id);

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
        public static string EditEnergyDataManualInputData(string editData)
        {
            if (CRUD[2] == '1')
            {
                int result = BasicData.Service.EnergyDataManualInput.EnergyDataManualInputService.EditEnergyDataManualInput(editData);

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
    }
}