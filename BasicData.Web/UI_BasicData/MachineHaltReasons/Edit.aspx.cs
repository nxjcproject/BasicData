using BasicData.Service.MachineHaltReasons;
using EasyUIJsonParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web.UI_BasicData.MachineHaltReasons
{
    public partial class Edit : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
#if DEBUG
               mPageOpPermission="1111";
#endif
        }

        /// <summary>
        /// 页面操作权限
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static char[] AuthorityControl()
        {
            return mPageOpPermission.ToArray();
        }

        [WebMethod]
        public static string GetMachineHaltReasonsWithTreeGridFormat()
        {
            DataTable dt = MachineHaltReasonsService.GetMachineHaltReasons();
            DataColumn parentIdColumn = new DataColumn("ParentID");
            dt.Columns.Add(parentIdColumn);

            foreach (DataRow row in dt.Rows)
            {
                string levelcode = row["MachineHaltReasonID"].ToString().Trim();
                if (levelcode.Length > 3)
                    row["ParentID"] = levelcode.Substring(0, levelcode.Length - 2);
            }
            return TreeGridJsonParser.DataTableToJson(dt, "MachineHaltReasonID", "ParentID", "ReasonItemID", "ReasonText", "ReasonStatisticsTypeId","Remarks", "Enabled");
        }

        [WebMethod]
        public static void SaveMachineHaltReasons(string json)
        {
            DataTable dt = TreeGridJsonParser.JsonToDataTable(json);

            MachineHaltReasonsService.SaveMachineHaltReasons(dt);
        }
        [WebMethod]
        public static string GetReasonItemID()
        {
            string m_ReasonItemId = System.Guid.NewGuid().ToString();
            return m_ReasonItemId;
        }
        [WebMethod]
        public static string GetReasonStatisticsTypeData()
        {
            string m_ReasonStatisticsTypeValue = MachineHaltReasonsService.GetReasonStatisticsTypeInfo();
            return m_ReasonStatisticsTypeValue;
        }
    }
}