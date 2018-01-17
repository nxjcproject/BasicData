using BasicData.Service.EquipmentAccount;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web.UI_BasicData.EquipmentAccount
{
    public partial class EquipmentAccountEdit : WebStyleBaseForEnergy.webStyleBase
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx_tys" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                mPageOpPermission = "1111";
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "EquipmentAccountEdit.aspx";                                     //向web用户控件传递当前调用的页面名称
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
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetEquipmentsInfo(string organizationId)
        {
            DataTable table = EquipmentAccountService.GetEquipmentDatas(organizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetOrganizationIdInfo(string organizationId)
        {
            DataTable table = EquipmentAccountService.GetOrganizationIds(organizationId);
            string json = EasyUIJsonParser.ComboboxJsonParser.DataTableToJson(table);
            return json;
        }
        /// <summary>
        /// 保存设备
        /// </summary>
        /// <param name="VariableId"></param>
        /// <param name="OrganizationID"></param>
        /// <param name="EquipmentName"></param>
        /// <param name="MonitorType"></param>
        /// <param name="PowerSupply"></param>
        /// <param name="VoltageGrade"></param>
        /// <param name="RatedCT"></param>
        /// <param name="AmmeterCode"></param>
        /// <param name="ActualCT"></param>
        /// <param name="Power"></param>
        /// <param name="Unit"></param>
        /// <param name="PowerSupplyPosition"></param>
        /// <param name="Remarks"></param>
        /// <returns></returns>
        [WebMethod]
        public static string SaveEquipmentInfo(string VariableId, string OrganizationID, string EquipmentName, int MonitorType,string PowerSupply, string VoltageGrade, 
            string RatedCT, string AmmeterCode, string ActualCT, string Power, string Unit,string Current,string PowerSupplyPosition, string Remarks)
        {
            if (mPageOpPermission.ToArray()[1] == '1')
            {
                string message = EquipmentAccountService.SaveEquipment(VariableId, OrganizationID, EquipmentName, MonitorType, PowerSupply, VoltageGrade, RatedCT, AmmeterCode, ActualCT,
                    Power, Unit, Current, PowerSupplyPosition, Remarks);
                return message;
            }
            else
            {
                return "用户没有修改权限！";
            }
        }
        /// <summary>
        /// 删除设备信息
        /// </summary>
        /// <param name="variableId"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [WebMethod]
        public static string RemoveEquipmentInfo(string variableId, string organizationId)
        {
            if (mPageOpPermission.ToArray()[3] == '1')
            {
                return EquipmentAccountService.RemoveEquipment(variableId, organizationId);
            }
            else
            {
                return "用户没有删除权限！";
            }
        }
        /// <summary>
        /// 编辑设备
        /// </summary>
        /// <param name="variableId"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [WebMethod]
        public static string EditEquipmentInfo(string variableId_old, string organizationId_old, string VariableId, string OrganizationID, string EquipmentName, int MonitorType, string PowerSupply, string VoltageGrade,
            string RatedCT, string AmmeterCode, string ActualCT, string Power, string Unit, string Current,string PowerSupplyPosition, string Remarks)
        {
            if (mPageOpPermission.ToArray()[2] == '1')
            {
                return EquipmentAccountService.UpdateEquipment(variableId_old, organizationId_old, VariableId, OrganizationID, EquipmentName, MonitorType, PowerSupply,
                    VoltageGrade, RatedCT, AmmeterCode, ActualCT, Power, Unit, Current, PowerSupplyPosition, Remarks);
            }
            else
            {
                return "用户没有编辑权限！";
            }
        }
    }
}