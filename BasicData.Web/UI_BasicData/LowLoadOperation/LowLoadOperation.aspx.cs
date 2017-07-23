using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using System.Web.UI.WebControls;
using BasicData.Service.LowLoadOperation;

namespace BasicData.Web.UI_BasicData.LowLoadOperation
{
    public partial class LowLoadOperation : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_qtx_tys", "zc_nxjc_klqc_klqf", "zc_nxjc_znc_znf"
                ,"zc_nxjc_ychc_yfcf","zc_nxjc_tsc_tsf","zc_nxjc_qtx_efc","zc_nxjc_ychc_ndf","zc_nxjc_szsc_szsf","zc_nxjc_ychc_lsf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                mPageOpPermission = "1111";
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "LowLoadOperation.aspx";   //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;

                this.TagsSelector_DcsTags.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.TagsSelector_DcsTags.PageName = "LowLoadOperation.aspx";                                     //向web用户控件传递当前调用的页面名称
            }
        }
        [WebMethod]
        public static char[] AuthorityControl()
        {
            return mPageOpPermission.ToArray();
        }
        [WebMethod]
        public static string GetLowLoadOperation(string mOrganizationId, string variableDesc)
        {
            DataTable table = LowLoadOperationService.LowLoadOperation(mOrganizationId, variableDesc);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetMasterMachineEquipment(string myOrganizationId)
        {
            //List<string> m_OrganizationIds = GetDataValidIdGroup("ProductionOrganization");
            DataTable m_MainMachineInfo = LowLoadOperationService.GetMainMachineInfo(myOrganizationId);
            string ReturnValue = EasyUIJsonParser.TreeJsonParser.DataTableToJson(m_MainMachineInfo, "EquipmentId", "Name", "EquipmentCommonId", "0", new string[] { "VariableId", "OrganizationId", "OutputFormula" });
            return ReturnValue;
        }
        [WebMethod]
        public static string GetDcsOrganization()
        {
            DataTable m_DcsOrganization = WebUserControls.Service.TagsSelector.TagsSelector_Dcs.GetDCSTagsDataBase(GetDataValidIdGroup("ProductionOrganization"), true);
            return EasyUIJsonParser.TreeJsonParser.DataTableToJsonByLevelCodeWithIdColumn(m_DcsOrganization, "LevelCode", "OrganizationID", "Name");
        }
        [WebMethod]
        public static int AddLowLoadOperation(string mOrganizationId, string m_VariableDesc, string m_RunTag, string MasterTableName, string MasterDataBaseName, string m_Record, string m_ValidValues, string m_DelayTime, string m_LoadTag, string m_DCSLoadTag, string s_MasterTableName, string s_MasterDataBaseName, string m_LoadTagType, string m_LLoadLimit, string m_Remark)
        {
            var m_UserId = mUserId;
            int result = LowLoadOperationService.AddLowLoadOperationConfig(mOrganizationId, m_VariableDesc, m_RunTag, MasterTableName, MasterDataBaseName, m_Record, m_ValidValues, m_DelayTime, m_LoadTag, m_DCSLoadTag, s_MasterTableName, s_MasterDataBaseName, m_LoadTagType, m_LLoadLimit, m_UserId, m_Remark);
            return result;
        }
        [WebMethod]
        public static int EditLowLoadOperation(string m_Id, string mOrganizationId, string m_VariableDesc, string m_RunTag, string MasterTableName, string MasterDataBaseName, string m_Record, string m_ValidValues, string m_DelayTime, string m_LoadTag, string m_DCSLoadTag, string s_MasterTableName, string s_MasterDataBaseName, string m_LoadTagType, string m_LLoadLimit, string m_Remark)
        {
            var m_UserId = mUserId;
            int result = LowLoadOperationService.EditLowLoadOperationConfig(m_Id, mOrganizationId, m_VariableDesc, m_RunTag, MasterTableName, MasterDataBaseName, m_Record, m_ValidValues, m_DelayTime, m_LoadTag, m_DCSLoadTag, s_MasterTableName, s_MasterDataBaseName, m_LoadTagType, m_LLoadLimit, m_UserId, m_Remark);
            return result;
        }
        [WebMethod]
        public static int deleteLowLoadOperation(string m_Id)
        {
            int result = LowLoadOperationService.deleteLowLoadOperationConfig(m_Id);
            return result;
        }
    }
}