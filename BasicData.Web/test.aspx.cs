using BasicData.Service.AmmeterModifyCoefficient;
using BasicData.Service.PeakValleyFlat;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasicData.Web
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //PeakValleyFlatService service = new PeakValleyFlatService();
            //DataTable dt = service.GetPVFList("123", "2014-10-08");
            //this.GridView1.DataSource = dt;
            //this.GridView1.DataBind();
            ModifyCoefficientService.ReferenceCoefficientCalculate("zc_nxjc_byc_byf", "2015-01-01", "2015-05-30");
        }
    }
}