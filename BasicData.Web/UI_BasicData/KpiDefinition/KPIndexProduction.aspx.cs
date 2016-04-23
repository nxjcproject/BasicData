using BasicData.Service.KpiDefinition;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web.UI_BasicData.KpiDefinition
{
    public partial class KPIndexProduction : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                mPageOpPermission = "0000";
#elif RELEASE
#endif
                //this.OrganisationTree.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                //this.OrganisationTree.PageName = "KPIndexEnergy.aspx";    

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
        public static string GetIndexData(string mStandardId, string myInOutSide)
        {
            DataTable table = ProdunctionService.GetProductionIndexData(mStandardId, myInOutSide);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        
        }
        [WebMethod]
        public static string GetKPIProductionOrganizationData()
        {
            string[] temArr={"熟料","水泥磨"};
            List<string> typeItems=new List<string>(temArr);
            DataTable m_DcsOrganization = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationTree(GetDataValidIdGroup("ProductionOrganization"), "", typeItems,7,true);
            return EasyUIJsonParser.TreeJsonParser.DataTableToJsonByLevelCodeWithIdColumn(m_DcsOrganization, "LevelCode", "OrganizationID", "Name");     
        }
         [WebMethod]
        public static string GetEquipmentCommonName()
        {
            DataTable table = ProdunctionService.GetEquipmentCommonNameList();
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
         [WebMethod]
         public static string GetValueTypeList() 
         {
             DataTable table =RunIndicators.RunIndicatorsItems.GetRunIndicatorsItemsTable();
             string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
             return json;       
         }
         [WebMethod]
         //public static string AddKPIndexProduction(string mStandardLevel, string mObjName, string mOrganizationId, string mLevelType, string mVariableId, string mValueType, float mUnit, string mStandardValue, string mLevel, string mCreator)  
         public static void AddKPIndexProduction(string mStandardLevel, string mObjName, string mOrganizationId, string mLevelType, string mVariableId, string mValueType, string  mUnit, float mStandardValue, int mLevel, string mCreator)
         {
             ProdunctionService.AddKPIndexProductionList(mStandardLevel, mObjName, mOrganizationId, mLevelType, mVariableId, mValueType, mUnit, mStandardValue, mLevel, mCreator);
         }
    }
}