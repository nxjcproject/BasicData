using BasicData.Service.PeakValleyFlat;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web.UI_BasicData.PeakValleyFlat
{
    public partial class Detail : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
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