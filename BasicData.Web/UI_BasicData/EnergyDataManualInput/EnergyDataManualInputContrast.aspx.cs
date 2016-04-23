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
    public partial class EnergyDataManualInputContrast : WebStyleBaseForEnergy.webStyleBase
    {
        private static char[] CRUD; 
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            ////////////////////调试用,自定义的数据授权
            //List<string> m_DataValidIdItems = new List<string>() { "C41B1F47-A48A-495F-A890-0AABB2F3BFF7                            ", "43F1EA8C-FF77-4BC5-BACB-531DC56A2512                            " };
            //AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#if DEBUG
            ////////////////////调试用,自定义的数据授权
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
            mPageOpPermission = "0000";
            
#elif RELEASE
#endif
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
        public static string GetEnergyDataManualInputContrastData(string variableName)
        {
            DataTable dt = BasicData.Service.EnergyDataManualInput.EnergyDataManualInputService.GetEnergyDataManualInputContrast(variableName);
            string result = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(dt);
            return result;
        }

        [WebMethod]
        public static string AddEnergyDataManualInputContrastData(string maddData)
        {
            if (CRUD[1] == '1')
            {
                int result = BasicData.Service.EnergyDataManualInput.EnergyDataManualInputService.AddEnergyDataManualInputContrast(maddData);

                if (result == 1)
                    return "1";
                else if (result == -2)
                    return "-2";
                else
                    return "-1";
            }
            else
            {
                return "noright";
            }
        }

        [WebMethod]
        public static string DeleteEnergyDataManualInputContrastData(string variableId)
        {
            if (CRUD[3] == '1')
            {
                int result = BasicData.Service.EnergyDataManualInput.EnergyDataManualInputService.DeleteEnergyDataManualInputContrast(variableId);

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
        public static string EditEnergyDataManualInputContrastData(string editData)
        {
            if (CRUD[2] == '1')
            {
                int result = BasicData.Service.EnergyDataManualInput.EnergyDataManualInputService.EditEnergyDataManualInputContrast(editData);

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