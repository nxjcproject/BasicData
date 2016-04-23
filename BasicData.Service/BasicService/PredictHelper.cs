using BasicData.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicData.Service.BasicService
{
    public class PredictHelper
    {
        private static string connectionString;
        private static TZHelper tzHelper;
        static PredictHelper()
        {
            connectionString = ConnectionStringFactory.NXJCConnectionString;
            tzHelper = new TZHelper(connectionString);
        }
        public static DataTable GetClinkerData(string organizeID, string v_begin, string v_end)
        {
            DataTable temp;
            temp = tzHelper.CreateTableStructure("report_ClinkerYearlyProcessEnergyConsumption");
            DataTable temp2;
            DateTime v_date = DateTime.Parse(v_begin);
            string v_date_string;
            while (v_date <= DateTime.Parse(v_end))
            {
                v_date_string = v_date.ToString("yyyy-MM");
                DataRow row = temp.NewRow();
                row["vDate"] = v_date_string.Substring(5, 2);
                temp2 = tzHelper.GetReportData("tz_Report", organizeID, v_date_string, "table_ClinkerMonthlyOutput", "vDate='合计'");
                foreach (DataRow dr in temp2.Rows)
                {
                    row["Consumption_CoalDust_Monthly"] = Convert.ToInt64(dr["AmounttoCoalDustConsumptionSum"]);//用煤量
                    row["Output_RawBatch_Monthly"] = Convert.ToInt64(dr["RawBatchProductionSum"]);
                    row["Output_Clinker_Monthly"] = Convert.ToInt64(dr["ClinkerProductionSum"]);
                    row["Output_CoalDust_Monthly"] = Convert.ToInt64(dr["CoalDustProductionSum"]);
                    row["Output_Cogeneration_Monthly"] = Convert.ToInt64(dr["PowerGenerationSum"]);
                }
                temp2 = tzHelper.GetReportData("tz_Report", organizeID, v_date_string, "table_ClinkerMonthlyElectricity_sum", "vDate='合计'");
                foreach (DataRow dr in temp2.Rows)//本月电量并入目标表
                {
                    row["Electricity_RawBatch_Monthly"] = Convert.ToInt64(dr["AmounttoRawBatchPreparationSum"]);
                    row["Electricity_RawBatchMil_Monthly"] = Convert.ToInt64(dr["RawBatchGrindingSum"]);
                    row["Electricity_Clinker_Monthly"] = Convert.ToInt64(dr["AmounttoFiringSystemSum"]);
                    row["Electricity_CoalDust_Monthly"] = Convert.ToInt64(dr["CoalMillSystemSum"]);
                }
                temp.Rows.Add(row);
                v_date = v_date.AddMonths(1);

            }
            foreach (DataRow dr in temp.Rows)
            {
                dr["ElectricityConsumption_RawBatch_Monthly"] = MyToDecimal(dr["Output_RawBatch_Monthly"]) != 0 ? MyToDecimal(dr["Electricity_RawBatch_Monthly"]) / MyToDecimal(dr["Output_RawBatch_Monthly"]) : 0;
                dr["ElectricityConsumption_RawBatchMill_Monthly"] = MyToDecimal(dr["Output_RawBatch_Monthly"]) != 0 ? MyToDecimal(dr["Electricity_RawBatchMil_Monthly"]) / MyToDecimal(dr["Output_RawBatch_Monthly"]) : 0;
                dr["ElectricityConsumption_Clinker_Monthly"] = MyToDecimal(dr["Output_Clinker_Monthly"]) != 0 ? MyToDecimal(dr["Electricity_Clinker_Monthly"]) / MyToDecimal(dr["Output_Clinker_Monthly"]) : 0;
                dr["ElectricityConsumption_CoalDust_Monthly"] = MyToDecimal(dr["Output_CoalDust_Monthly"]) != 0 ? MyToDecimal(dr["Electricity_CoalDust_Monthly"]) / MyToDecimal(dr["Output_CoalDust_Monthly"]) : 0;
                dr["ComprehensiveElectricityConsumption_Monthly"] = MyToDecimal(dr["Output_Clinker_Monthly"]) != 0 ? (MyToDecimal(dr["Electricity_RawBatch_Monthly"]) + MyToDecimal(dr["Electricity_Clinker_Monthly"])) / MyToDecimal(dr["Output_Clinker_Monthly"]) : 0;
                dr["ComprehensiveCoalConsumption_Monthly"] = MyToDecimal(dr["Output_Clinker_Monthly"]) != 0 ? MyToDecimal(dr["Consumption_CoalDust_Monthly"]) / MyToDecimal(dr["Output_Clinker_Monthly"]) : 0;
            }
            return temp;
        }

        public static DataTable GetCementMillData(string organizeID, string v_begin, string v_end)
        {
            DataTable temp;
            temp = tzHelper.CreateTableStructure("report_CementMilYearlyEnergyConsumption");
            DataTable temp2;
            DateTime v_date = DateTime.Parse(v_begin);
            string v_date_string;
            while (v_date <= DateTime.Parse(v_end))
            {
                v_date_string = v_date.ToString("yyyy-MM");
                DataRow row = temp.NewRow();
                row["vDate"] = v_date_string.Substring(5, 2);
                temp2 = tzHelper.GetReportData("tz_Report", organizeID, v_date_string, "table_CementMillMonthlyOutput", "vDate='合计'");
                foreach (DataRow dr in temp2.Rows)
                {
                    //row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Output_Cement_Monthly"] = dr["CementProductionSum"];//水泥制备
                }
                temp2 = tzHelper.GetReportData("tz_Report", organizeID, v_date_string, "table_CementMillMonthlyElectricity_sum", "vDate='合计'");
                foreach (DataRow dr in temp2.Rows)//本月电量并入目标表
                {
                    row["Electricity_Cement_Monthly"] = Convert.ToInt64(dr["AmounttoCementPreparationSum"]);
                    row["Electricity_CementGrinding_Monthly"] = Convert.ToInt64(dr["CementGrindingSum"]);
                    row["Electricity_AdmixturePreparation_Monthly"] = Convert.ToInt64(dr["AdmixturePreparationSum"]);
                    row["Electricity_BagsBulk_Monthly"] = Convert.ToInt64(dr["AmounttoCementPackagingSum"]);
                }
                temp.Rows.Add(row);
                v_date = v_date.AddMonths(1);

            }
            foreach (DataRow dr in temp.Rows)
            {
                dr["ElectricityConsumption_Cement_Monthly"] = MyToDecimal(dr["Output_Cement_Monthly"]) != 0 ? MyToDecimal(dr["Electricity_Cement_Monthly"]) / MyToDecimal(dr["Output_Cement_Monthly"]) : 0;
                dr["ElectricityConsumption_CementGrinding_Monthly"] = MyToDecimal(dr["Output_Cement_Monthly"]) != 0 ? MyToDecimal(dr["Electricity_CementGrinding_Monthly"]) / MyToDecimal(dr["Output_Cement_Monthly"]) : 0;
                dr["ComprehensiveElectricityConsumption_Monthly"]
                        = MyToDecimal(dr["Output_Cement_Monthly"]) != 0 ? (MyToDecimal(dr["Electricity_Cement_Monthly"])
                        + MyToDecimal(dr["Electricity_AdmixturePreparation_Monthly"])
                        + MyToDecimal(dr["Electricity_BagsBulk_Monthly"]))
                        / MyToDecimal(dr["Output_Cement_Monthly"]) : 0;
            }
            return temp;
        }

        public static decimal MyToDecimal(object obj)
        {
            if (obj is DBNull)
            {
                obj = 0;
                return Convert.ToDecimal(obj);
            }
            else
            {
                return Convert.ToDecimal(obj);
            }
        }
    }
}
