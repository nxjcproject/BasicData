using BasicData.Service.Material;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web.UI_BasicData.Material
{
    public partial class MaterialList : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
#if DEBUG
            // 调试用,自定义的数据授权
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx_efc", "zc_nxjc_byc" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#endif
            this.OrganisationTree.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
            this.OrganisationTree.PageName = "MaterialList.aspx";

        }


        /// <summary>
        /// 获取物料列表
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetMaterialList(string organizationId)
        {
            DataTable table = MaterialService.GetMaterials(organizationId);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
        }

        /// <summary>
        /// 创建物料列表
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="name"></param>
        [WebMethod]
        public static void CreateMaterialList(string organizationId, string name)
        {
            MaterialService.CreateMaterial(organizationId, name);
        }

        /// <summary>
        /// 删除物料列表
        /// </summary>
        /// <param name="keyId"></param>
        [WebMethod]
        public static void DeleteMaterialList(string keyId)
        {
            MaterialService.DeleteMaterials(keyId);
        }

        /// <summary>
        /// 获取物料详细
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetMaterialDetail(string keyId)
        {
            DataTable table = MaterialService.GetMaterialDetail(keyId);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
        }

        /// <summary>
        /// 创建物料详细（单条）
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="variableId"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="unit"></param>
        /// <param name="materialErpCode"></param>
        /// <param name="tagTableName"></param>
        /// <param name="formula"></param>
        /// <param name="coefficient"></param>
        [WebMethod]
        public static void CreateMaterialDetail(string keyId,
                                                string variableId,
                                                string name,
                                                string type,
                                                string unit,
                                                string materialErpCode,
                                                string tagTableName,
                                                string formula,
                                                string coefficient)
        {
            MaterialService.CreateMaterialDetail(keyId, variableId, name, type, unit, materialErpCode, tagTableName, formula, coefficient);
        }

        /// <summary>
        /// 更新物料详细（单条）
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="variableId"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="unit"></param>
        /// <param name="materialErpCode"></param>
        /// <param name="tagTableName"></param>
        /// <param name="formula"></param>
        /// <param name="coefficient"></param>
        [WebMethod]
        public static void UpdateMaterialDetail(string materialId,
                                                string variableId,
                                                string name,
                                                string type,
                                                string unit,
                                                string materialErpCode,
                                                string tagTableName,
                                                string formula,
                                                string coefficient)
        {
            MaterialService.UpdateMaterialDetail(materialId, variableId, name, type, unit, materialErpCode, tagTableName, formula, coefficient);
        }

        /// <summary>
        /// 删除物料详细（单条）
        /// </summary>
        /// <param name="materialId"></param>
        [WebMethod]
        public static void DeleteMaterialDetail(string materialId)
        {
            MaterialService.DeleteMaterialDetail(materialId);
        }
    }
}