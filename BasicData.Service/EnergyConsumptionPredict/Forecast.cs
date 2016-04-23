using BasicData.Infrastructure.Configuration;
using BasicData.Service.BasicService;
using SqlServerDataAdapter;
using SqlServerDataAdapter.Infrastruction;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicData.Service.EnergyConsumptionPredict
{
    /// <summary>
    /// 生产线与能源预测
    /// </summary>
    public class Forecast
    {
        private TZHelper tzHelper;
        private static string _connectionString;
        private ISqlServerDataFactory _dataFactory;
        private DataTable _dt;
        private string _string_error;
        private Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        public Forecast()
        {
            _connectionString = ConnectionStringFactory.NXJCConnectionString;
            _dataFactory = new SqlServerDataFactory(_connectionString);
            tzHelper = new TZHelper(_connectionString);
            _dictionary = InitDictionary();//初始化字典
        }

        public DataTable DT
        {
            get { return _dt; }
            set { _dt = value; }
        }
        public string String_Error
        {
            get { return _string_error; }
            set { _string_error = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_OrganizationID"></param>
        /// <param name="v_type">1:熟料，2：水泥磨</param>
        /// <param name="v_begin"></param>
        /// <param name="v_end"></param>
        /// <returns></returns>
        public DataTable Get_Forecast_ProductionLineEnergy(string v_OrganizationID, string v_type, string v_begin, string v_end)
        {
            _string_error = "";//错误信息串
            //string year = DateTime.Now.ToString("yyyy");
            string year = v_end.Substring(0, 4);
            DataTable temp = new DataTable();
            if (v_type == "熟料") //熟料
            {
                temp = PredictHelper.GetClinkerData(v_OrganizationID, v_begin, v_end);
            }
            if (v_type == "水泥磨") //水泥磨
            {
                temp = PredictHelper.GetCementMillData(v_OrganizationID, v_begin, v_end);
            }
            AddAverageRow(temp, v_type);//增加均值行
            CheckData(temp, v_type);//数据残缺度检查
            Query query = new Query("forecast_ProductionLineEnergyConsumptionReference");
            query.AddCriterion("OrganizationID", v_OrganizationID, CriteriaOperator.Equal);
            DataTable temp_reference = _dataFactory.Query(query);
            DataTable temp_history = temp_reference.Copy();
            CrosswiseToLengthways(temp, temp_history, v_type);
            Predict(temp_history, temp_reference);//第四步预测
            DataTable temp_plan = tzHelper.GetReportData("tz_Plan", v_OrganizationID, year, "plan_EnergyConsumptionYearlyPlan");
            if (temp_plan.Rows.Count == 0)
            {
                _string_error = _string_error + "生产线：" + v_OrganizationID + "的产量计划不存在！";
            }
            DataTable temp_result = new DataTable();
            if (v_type == "熟料")
            {   //熟料
                temp_result = _dataFactory.Query("select * from forecast_ProductionLineEnergyConsumptionTemplate where Type ='熟料'");
            }  // 熟料
            if (v_type == "水泥磨")
            {   //水泥磨
                temp_result = _dataFactory.Query("select * from forecast_ProductionLineEnergyConsumptionTemplate where Type ='水泥磨'");
            }  //水泥磨

            Calculat(v_OrganizationID, temp_plan, temp_result, temp_history, v_type);


            _dt = temp_result;
            return temp_result;
        }

        /// <summary>
        /// 增加均值行
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="v_type"></param>
        private void AddAverageRow(DataTable temp, string v_type)
        {
            DataRow dr = temp.NewRow();
            dr["vDate"] = "均值";
            if (v_type == "熟料")
            {
                dr["ElectricityConsumption_RawBatch_Monthly"]
                           = MyToDecimal(temp.Compute("sum(Output_RawBatch_Monthly)", "true")) != 0 ?
                           MyToDecimal(temp.Compute("sum(Electricity_RawBatch_Monthly)", "true"))
                           / MyToDecimal(temp.Compute("sum(Output_RawBatch_Monthly)", "true")) : 0;     //生料电耗
                dr["ElectricityConsumption_RawBatchMill_Monthly"]
                           = MyToDecimal(temp.Compute("Sum(Output_RawBatch_Monthly)", "true")) != 0 ?
                           MyToDecimal(temp.Compute("Sum(Electricity_RawBatchMil_Monthly)", "true"))
                           / MyToDecimal(temp.Compute("Sum(Output_RawBatch_Monthly)", "true")) : 0;	//生料磨电耗
                dr["ElectricityConsumption_Clinker_Monthly"]
                           = MyToDecimal(temp.Compute("Sum(Output_Clinker_Monthly)", "true")) != 0 ?
                           MyToDecimal(temp.Compute("Sum(Electricity_Clinker_Monthly)", "true"))
                           / MyToDecimal(temp.Compute("Sum(Output_Clinker_Monthly)", "true")) : 0; 	//熟料烧成电耗
                dr["ElectricityConsumption_CoalDust_Monthly"]
                           = MyToDecimal(temp.Compute("Sum(Output_CoalDust_Monthly)", "true")) != 0 ?
                           MyToDecimal(temp.Compute("Sum(Electricity_CoalDust_Monthly)", "true"))
                           / MyToDecimal(temp.Compute("Sum(Output_CoalDust_Monthly)", "true")) : 0; 	//煤粉制备电耗
                dr["ComprehensiveElectricityConsumption_Monthly"]
                           = MyToDecimal(temp.Compute("Sum(Output_Clinker_Monthly)", "true")) != 0 ?
                           (MyToDecimal(temp.Compute("Sum(Electricity_RawBatch_Monthly)", "true")) + MyToDecimal(temp.Compute("Sum(Electricity_Clinker_Monthly)", "true")))
                           / MyToDecimal(temp.Compute("Sum(Output_Clinker_Monthly)", "true")) : 0; 	//熟料综合电耗
                dr["ComprehensiveCoalConsumption_Monthly"]
                           = MyToDecimal(temp.Compute("Sum(Output_Clinker_Monthly)", "true")) != 0 ?
                           MyToDecimal(temp.Compute("Sum(Consumption_CoalDust_Monthly)", "true"))
                           / MyToDecimal(temp.Compute("Sum(Output_Clinker_Monthly)", "true")) : 0; 	//熟料实物煤耗
            }//熟料能耗求均值
            //求水泥制备电耗、水泥磨电耗、袋装与散装电耗、综合电耗电耗
            //增加一行，存放平均值
            if (v_type == "水泥磨")
            {
                dr["ElectricityConsumption_Cement_Monthly"]
                           = MyToDecimal(temp.Compute("sum(Output_Cement_Monthly)", "true")) != 0 ?
                           MyToDecimal(temp.Compute("sum(Electricity_Cement_Monthly)", "true"))
                           / MyToDecimal(temp.Compute("sum(Output_Cement_Monthly)", "true")) : 0;     //水泥制备电耗
                dr["ElectricityConsumption_CementGrinding_Monthly"]
                           = MyToDecimal(temp.Compute("sum(Output_Cement_Monthly)", "true")) != 0 ?
                           MyToDecimal(temp.Compute("sum(Electricity_CementGrinding_Monthly)", "true"))
                           / MyToDecimal(temp.Compute("sum(Output_Cement_Monthly)", "true")) : 0;	//水泥磨电耗
                dr["ElectricityConsumption_BagsBulk_Monthly"]
                           = MyToDecimal(temp.Compute("sum(Output_BagsBulk_Monthly)", "true")) != 0 ?
                           MyToDecimal(temp.Compute("sum(Electricity_BagsBulk_Monthly)", "true"))
                           / MyToDecimal(temp.Compute("sum(Output_BagsBulk_Monthly)", "true")) : 0;     //袋装与散装电耗
                dr["ComprehensiveElectricityConsumption_Monthly"]
                           = MyToDecimal(temp.Compute("sum(Output_Cement_Monthly)", "true")) != 0 ?
                           MyToDecimal(temp.Compute("sum(Electricity_Cement_Monthly+ Electricity_BagsBulk_Monthly)", "true"))
                           / MyToDecimal(temp.Compute("sum(Output_Cement_Monthly)", "true")) : 0; 	//综合电耗
            }
            temp.Rows.Add(dr);
        }

        /// <summary>
        /// 数据残缺度检查
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="v_type"></param>
        private void CheckData(DataTable temp, string v_type)
        {
            for (int i = 0; i < 12; i++)
            {  //1-12月
                int f_not_exist = 0;
                int f_error = 0;
                if (v_type == "熟料")
                {   //熟料能耗年统计 
                    int[] clinker_not_exist = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    int[] clinker_error = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                    if (MyToDecimal(temp.Rows[i]["ElectricityConsumption_RawBatch_Monthly"]) == 0)
                    { //生料电耗0
                        f_not_exist = 1;
                        temp.Rows[i]["ElectricityConsumption_RawBatch_Monthly"] = MyToDecimal(temp.Rows[12]["ElectricityConsumption_RawBatch_Monthly"]);
                    }
                    else
                    {
                        decimal diverge = new decimal();
                        diverge = Math.Abs(MyToDecimal(temp.Rows[i]["ElectricityConsumption_RawBatch_Monthly"])
                                        - MyToDecimal(temp.Rows[12]["ElectricityConsumption_RawBatch_Monthly"]))
                                        / MyToDecimal(temp.Rows[12]["ElectricityConsumption_RawBatch_Monthly"]);//求偏离
                        if (diverge >= (decimal)0.20)
                        {
                            f_error = 1;
                            temp.Rows[i]["ElectricityConsumption_RawBatch_Monthly"] = temp.Rows[12]["ElectricityConsumption_RawBatch_Monthly"];
                        }
                    }
                    if (f_not_exist == 1) { clinker_not_exist[0] = clinker_not_exist[0] + 1; }
                    if (f_error == 1) { clinker_error[0] = clinker_error[0] + 1; }
                    ////生料电耗0

                    if (MyToDecimal(temp.Rows[i]["ElectricityConsumption_RawBatchMill_Monthly"]) == 0)
                    { //生料磨电耗1
                        f_not_exist = 1;
                        temp.Rows[i]["ElectricityConsumption_RawBatchMill_Monthly"] = MyToDecimal(temp.Rows[12]["ElectricityConsumption_RawBatchMill_Monthly"]);
                    }
                    else
                    {
                        decimal diverge = new decimal();
                        diverge = Math.Abs(MyToDecimal(temp.Rows[i]["ElectricityConsumption_RawBatchMill_Monthly"])
                                        - MyToDecimal(temp.Rows[12]["ElectricityConsumption_RawBatchMill_Monthly"]))
                                        / MyToDecimal(temp.Rows[12]["ElectricityConsumption_RawBatchMill_Monthly"]);//求偏离
                        if (diverge >= (decimal)0.20)
                        {
                            f_error = 1;
                            temp.Rows[i]["ElectricityConsumption_RawBatchMill_Monthly"] = temp.Rows[12]["ElectricityConsumption_RawBatchMill_Monthly"];
                        }
                    }
                    if (f_not_exist == 1) { clinker_not_exist[1] = clinker_not_exist[1] + 1; }
                    if (f_error == 1) { clinker_error[1] = clinker_error[1] + 1; }
                    //生料磨电耗1

                    if (MyToDecimal(temp.Rows[i]["ElectricityConsumption_Clinker_Monthly"]) == 0)
                    { //熟料烧成电耗2
                        f_not_exist = 1;
                        temp.Rows[i]["ElectricityConsumption_Clinker_Monthly"] = MyToDecimal(temp.Rows[12]["ElectricityConsumption_Clinker_Monthly"]);
                    }
                    else
                    {
                        decimal diverge = new decimal();
                        diverge = Math.Abs(MyToDecimal(temp.Rows[i]["ElectricityConsumption_Clinker_Monthly"])
                                        - MyToDecimal(temp.Rows[12]["ElectricityConsumption_Clinker_Monthly"]))
                                        / MyToDecimal(temp.Rows[12]["ElectricityConsumption_Clinker_Monthly"]);//求偏离
                        if (diverge >= (decimal)0.20)
                        {
                            f_error = 1;
                            temp.Rows[i]["ElectricityConsumption_Clinker_Monthly"] = temp.Rows[12]["ElectricityConsumption_Clinker_Monthly"];
                        }
                    }
                    if (f_not_exist == 1) { clinker_not_exist[2] = clinker_not_exist[2] + 1; }
                    if (f_error == 1) { clinker_error[2] = clinker_error[2] + 1; }
                    //熟料烧成电耗2

                    if (MyToDecimal(temp.Rows[i]["ElectricityConsumption_CoalDust_Monthly"]) == 0)
                    { //煤粉制备电耗3
                        f_not_exist = 1;
                        temp.Rows[i]["ElectricityConsumption_CoalDust_Monthly"] = MyToDecimal(temp.Rows[12]["ElectricityConsumption_CoalDust_Monthly"]);
                    }
                    else
                    {
                        decimal diverge = new decimal();
                        diverge = Math.Abs(MyToDecimal(temp.Rows[i]["ElectricityConsumption_CoalDust_Monthly"])
                                        - MyToDecimal(temp.Rows[12]["ElectricityConsumption_CoalDust_Monthly"]))
                                        / MyToDecimal(temp.Rows[12]["ElectricityConsumption_CoalDust_Monthly"]);//求偏离
                        if (diverge >= (decimal)0.20)
                        {
                            f_error = 1;
                            temp.Rows[i]["ElectricityConsumption_CoalDust_Monthly"] = temp.Rows[12]["ElectricityConsumption_CoalDust_Monthly"];
                        }
                    }
                    if (f_not_exist == 1) { clinker_not_exist[3] = clinker_not_exist[3] + 1; }
                    if (f_error == 1) { clinker_error[3] = clinker_error[3] + 1; }
                    //煤粉制备电耗3

                    if (MyToDecimal(temp.Rows[i]["ComprehensiveElectricityConsumption_Monthly"]) == 0)
                    { //综合电耗4
                        f_not_exist = 1;
                        temp.Rows[i]["ComprehensiveElectricityConsumption_Monthly"] = MyToDecimal(temp.Rows[12]["ComprehensiveElectricityConsumption_Monthly"]);
                    }
                    else
                    {
                        decimal diverge = new decimal();
                        diverge = Math.Abs(MyToDecimal(temp.Rows[i]["ComprehensiveElectricityConsumption_Monthly"])
                                        - MyToDecimal(temp.Rows[12]["ComprehensiveElectricityConsumption_Monthly"]))
                                        / MyToDecimal(temp.Rows[12]["ComprehensiveElectricityConsumption_Monthly"]);//求偏离
                        if (diverge >= (decimal)0.20)
                        {
                            f_error = 1;
                            temp.Rows[i]["ComprehensiveElectricityConsumption_Monthly"] = temp.Rows[12]["ComprehensiveElectricityConsumption_Monthly"];
                        }
                    }
                    if (f_not_exist == 1) { clinker_not_exist[4] = clinker_not_exist[4] + 1; }
                    if (f_error == 1) { clinker_error[4] = clinker_error[4] + 1; }
                    //综合电耗4

                    if (MyToDecimal(temp.Rows[i]["ComprehensiveCoalConsumption_Monthly"]) == 0)
                    { //实物电耗5
                        f_not_exist = 1;
                        temp.Rows[i]["ComprehensiveCoalConsumption_Monthly"] = MyToDecimal(temp.Rows[12]["ComprehensiveCoalConsumption_Monthly"]);
                    }
                    else
                    {
                        decimal diverge = new decimal();
                        diverge = Math.Abs(MyToDecimal(temp.Rows[i]["ComprehensiveCoalConsumption_Monthly"])
                                        - MyToDecimal(temp.Rows[12]["ComprehensiveCoalConsumption_Monthly"]))
                                        / MyToDecimal(temp.Rows[12]["ComprehensiveCoalConsumption_Monthly"]);//求偏离
                        if (diverge >= (decimal)0.20)
                        {
                            f_error = 1;
                            temp.Rows[i]["ComprehensiveCoalConsumption_Monthly"] = temp.Rows[12]["ComprehensiveCoalConsumption_Monthly"];
                        }
                    }
                    if (f_not_exist == 1) { clinker_not_exist[5] = clinker_not_exist[5] + 1; }
                    if (f_error == 1) { clinker_error[5] = clinker_error[5] + 1; }
                    //实物电耗5
                }


                if (v_type == "水泥磨")
                {   //水泥磨能耗年统计
                    int[] cementMil_not_exist = new int[4] { 0, 0, 0, 0 };
                    int[] cementMil_error = new int[4] { 0, 0, 0, 0 };

                    if (MyToDecimal(temp.Rows[i]["ElectricityConsumption_Cement_Monthly"]) == 0)
                    { //水泥制备电耗0
                        f_not_exist = 1;
                        temp.Rows[i]["ElectricityConsumption_Cement_Monthly"] = MyToDecimal(temp.Rows[12]["ElectricityConsumption_Cement_Monthly"]);
                    }
                    else
                    {
                        decimal diverge = new decimal();
                        diverge = Math.Abs(MyToDecimal(temp.Rows[i]["ElectricityConsumption_Cement_Monthly"])
                                        - MyToDecimal(temp.Rows[12]["ElectricityConsumption_Cement_Monthly"]))
                                        / MyToDecimal(temp.Rows[12]["ElectricityConsumption_Cement_Monthly"]);//求偏离
                        if (diverge >= (decimal)0.20)
                        {
                            f_error = 1;
                            temp.Rows[i]["ElectricityConsumption_Cement_Monthly"] = temp.Rows[12]["ElectricityConsumption_Cement_Monthly"];
                        }
                    }
                    if (f_not_exist == 1) { cementMil_not_exist[0] = cementMil_not_exist[0] + 1; }
                    if (f_error == 1) { cementMil_error[0] = cementMil_error[0] + 1; }

                    if (MyToDecimal(temp.Rows[i]["ElectricityConsumption_CementGrinding_Monthly"]) == 0)
                    { //水泥磨电耗1
                        f_not_exist = 1;
                        temp.Rows[i]["ElectricityConsumption_CementGrinding_Monthly"] = MyToDecimal(temp.Rows[12]["ElectricityConsumption_CementGrinding_Monthly"]);
                    }
                    else
                    {
                        decimal diverge = new decimal();
                        diverge = Math.Abs(MyToDecimal(temp.Rows[i]["ElectricityConsumption_CementGrinding_Monthly"])
                                        - MyToDecimal(temp.Rows[12]["ElectricityConsumption_CementGrinding_Monthly"]))
                                        / MyToDecimal(temp.Rows[12]["ElectricityConsumption_CementGrinding_Monthly"]);//求偏离
                        if (diverge >= (decimal)0.20)
                        {
                            f_error = 1;
                            temp.Rows[i]["ElectricityConsumption_CementGrinding_Monthly"] = temp.Rows[12]["ElectricityConsumption_CementGrinding_Monthly"];
                        }
                    }
                    if (f_not_exist == 1) { cementMil_not_exist[1] = cementMil_not_exist[1] + 1; }
                    if (f_error == 1) { cementMil_error[1] = cementMil_error[1] + 1; }

                    if (MyToDecimal(temp.Rows[i]["ElectricityConsumption_BagsBulk_Monthly"]) == 0)
                    { //袋装与散装电耗2
                        f_not_exist = 1;
                        temp.Rows[i]["ElectricityConsumption_BagsBulk_Monthly"] = MyToDecimal(temp.Rows[12]["ElectricityConsumption_BagsBulk_Monthly"]);
                    }
                    else
                    {
                        decimal diverge = new decimal();
                        diverge = Math.Abs(MyToDecimal(temp.Rows[i]["ElectricityConsumption_BagsBulk_Monthly"])
                                        - MyToDecimal(temp.Rows[12]["ElectricityConsumption_BagsBulk_Monthly"]))
                                        / MyToDecimal(temp.Rows[12]["ElectricityConsumption_BagsBulk_Monthly"]);//求偏离
                        if (diverge >= (decimal)0.20)
                        {
                            f_error = 1;
                            temp.Rows[i]["ElectricityConsumption_BagsBulk_Monthly"] = temp.Rows[12]["ElectricityConsumption_BagsBulk_Monthly"];
                        }
                    }
                    if (f_not_exist == 1) { cementMil_not_exist[2] = cementMil_not_exist[2] + 1; }
                    if (f_error == 1) { cementMil_error[2] = cementMil_error[2] + 1; }

                    if (MyToDecimal(temp.Rows[i]["ComprehensiveElectricityConsumption_Monthly"]) == 0)
                    { //综合电耗3
                        f_not_exist = 1;
                        temp.Rows[i]["ComprehensiveElectricityConsumption_Monthly"] = MyToDecimal(temp.Rows[12]["ComprehensiveElectricityConsumption_Monthly"]);
                    }
                    else
                    {
                        decimal diverge = new decimal();
                        diverge = Math.Abs(MyToDecimal(temp.Rows[i]["ComprehensiveElectricityConsumption_Monthly"])
                                        - MyToDecimal(temp.Rows[12]["ComprehensiveElectricityConsumption_Monthly"]))
                                        / MyToDecimal(temp.Rows[12]["ComprehensiveElectricityConsumption_Monthly"]);//求偏离
                        if (diverge >= (decimal)0.20)
                        {
                            f_error = 1;
                            temp.Rows[i]["ComprehensiveElectricityConsumption_Monthly"] = temp.Rows[12]["ComprehensiveElectricityConsumption_Monthly"];
                        }
                    }
                    if (f_not_exist == 1) { cementMil_not_exist[2] = cementMil_not_exist[2] + 1; }
                    if (f_error == 1) { cementMil_error[2] = cementMil_error[2] + 1; }
                }

                //Label1.Text = "v_not_exist = " + Convert.ToString(v_not_exist) + " and v_error = " + Convert.ToString(v_error);

            }
        }
        /// <summary>
        /// 横表转纵表
        /// </summary>
        /// <param name="temp">横表</param>
        /// <param name="temp_history">纵表</param>
        private void CrosswiseToLengthways(DataTable temp, DataTable temp_history, string v_type)
        {
            int firstMonth = temp_history.Columns.IndexOf("January");
            //if (1 == v_type)//是不是不用判断，根据组织机构ID就可以区分
            foreach (DataRow dr in temp_history.Rows)
            {
                string name = dr["QuotasID"].ToString().Trim();
                for (int i = firstMonth; i < firstMonth + 12; i++)
                {
                    foreach (DataRow row in temp.Rows)
                    {
                        dr[i] = row[_dictionary[name]];
                    }
                }
            }
        }

        /// <summary>
        /// 第四步预测
        /// </summary>
        /// <param name="temp_history"></param>
        /// <param name="temp_reference"></param>
        private void Predict(DataTable temp_history, DataTable temp_reference)
        {
            int No_Col = 0;     //“1月”的列号
            No_Col = temp_history.Columns.IndexOf("January");//找出第一个月所在的列号
            for (int i = 0; i < temp_history.Rows.Count; i++)
            {
                for (int j = No_Col; j < No_Col + 12; j++)
                {  //1月到12月
                    Decimal v_value = MyToDecimal(temp_reference.Rows[i][j]) - MyToDecimal(temp_history.Rows[i][j]);
                    if (Math.Abs(v_value) / MyToDecimal(temp_reference.Rows[i][j]) >= (decimal)0.10)
                    {                                               // 10% 以上，限幅10%
                        if (v_value < 0)
                        {
                            temp_history.Rows[i][j] = MyToDecimal(temp_reference.Rows[i][j]) * (decimal)1.10;
                        }
                        else
                        {
                            temp_history.Rows[i][j] = MyToDecimal(temp_reference.Rows[i][j]) * (decimal)0.90;
                        }
                    }
                    else
                    {//10%以下
                        temp_history.Rows[i][j] = MyToDecimal(temp_history.Rows[i][j]) + v_value * (decimal)0.50;
                    }
                }
            }

        }
        /// <summary>
        /// 第七步计算
        /// </summary>
        /// <param name="temp_plan"></param>
        /// <param name="temp_reuslt"></param>
        /// <param name="v_type"></param>
        private void Calculat(string v_OrganizationID, DataTable temp_plan, DataTable temp_Result, DataTable temp_history, string v_type)
        {
            int No_Col = temp_Result.Columns.IndexOf("January");//找出第一个月所在的列号
            int No1_Row, No2_Row, No3_Row;
            //复制计划产量
            if (v_type == "熟料") //熟料   
            {
                MyFirstMethod(temp_plan, temp_Result, "熟料产量", "熟料产量", No_Col);//复制计划产量
                //--------从temp_history中复制预测值-----
                MyFirstMethod(temp_history, temp_Result, "熟料电耗", "熟料电耗", No_Col);
                MyFirstMethod(temp_history, temp_Result, "熟料煤耗", "熟料煤耗", No_Col);
                MyFirstMethod(temp_history, temp_Result, "生料磨电耗", "生料磨电耗", No_Col);
                MyFirstMethod(temp_history, temp_Result, "煤磨电耗", "煤磨电耗", No_Col);
                //-------end---------
                //计算原煤消耗量、电力消耗量
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料产量");
                No2_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料煤耗");
                No3_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "原煤消耗量");
                if (No3_Row!=-1)
                {
                    for (int i = No_Col; i < No_Col + 12; i++)  //1月到12月
                    {
                        temp_Result.Rows[No3_Row][i] = (No1_Row == -1 || No2_Row == -1 )? 0 : (MyToDecimal(temp_Result.Rows[No1_Row][i]) * MyToDecimal(temp_Result.Rows[No2_Row][i])/ 1000);
                    }
                }
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料产量");
                No2_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料电耗");
                No3_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "电力消耗量");
                if (No3_Row != -1)
                {
                    for (int i = No_Col; i < No_Col + 12; i++)  //1月到12月
                    {
                        temp_Result.Rows[No3_Row][i] = (No1_Row == -1 || No2_Row == -1) ? 0 : (MyToDecimal(temp_Result.Rows[No1_Row][i]) * MyToDecimal(temp_Result.Rows[No2_Row][i]));
                    }
                }
            }
            if ("水泥磨" == v_type) //水泥磨
            {
                MyFirstMethod(temp_plan, temp_Result, "水泥产量", "水泥产量", No_Col);//复制计划产量
                //--------从temp_history中复制预测值-----
                MyFirstMethod(temp_history, temp_Result, "水泥电耗", "水泥电耗", No_Col);
                MyFirstMethod(temp_history, temp_Result, "水泥磨电耗", "水泥磨电耗", No_Col);
                //-------end---------
                //计算水泥电力消耗量
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "水泥产量");
                No2_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "水泥电耗");
                No3_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "水泥电力消耗量");
                if (No3_Row != -1)
                {
                    for (int i = No_Col; i < No_Col + 12; i++)  //1月到12月
                    {
                        temp_Result.Rows[No3_Row][i] = (No1_Row == -1 || No2_Row == -1 )? 0 : (MyToDecimal(temp_Result.Rows[No1_Row][i]) * MyToDecimal(temp_Result.Rows[No2_Row][i]));
                    }
                }
            }
            //计算年度值
            if ("熟料" == v_type)//熟料
            {
                //年度熟料产量
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料产量");               
                if (No1_Row != -1)
                {
                    temp_Result.Rows[No1_Row]["Totals"] = 0;
                    for (int j = No_Col; j < No_Col + 12; j++) //1月到12月
                    {
                        temp_Result.Rows[No1_Row]["Totals"] = MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]) + MyToDecimal(temp_Result.Rows[No1_Row][j]);
                    }
                }
                //年度原煤消耗量
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "原煤消耗量");              
                if (No1_Row != -1)
                {
                    temp_Result.Rows[No1_Row]["Totals"] = 0;
                    for (int j = No_Col; j < No_Col + 12; j++) //1月到12月
                    {
                        temp_Result.Rows[No1_Row]["Totals"] = MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]) + MyToDecimal(temp_Result.Rows[No1_Row][j]);
                    }
                }
                //年度电力消耗量
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "电力消耗量");
                
                if (No1_Row != -1)
                {
                    temp_Result.Rows[No1_Row]["Totals"] = 0;
                    for (int j = No_Col; j < No_Col + 12; j++) //1月到12月
                    {
                        temp_Result.Rows[No1_Row]["Totals"] = MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]) + MyToDecimal(temp_Result.Rows[No1_Row][j]);
                    }
                }
                //年度熟料电耗
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料产量");
                No2_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "电力消耗量");
                No3_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料电耗");
                if (No3_Row != -1)
                {
                    temp_Result.Rows[No3_Row]["Totals"] = (No1_Row == -1 || No2_Row == -1||MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]) == 0 ) ? 0 : (MyToDecimal(temp_Result.Rows[No2_Row]["Totals"]) / MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]));
                }
                //年度熟料煤耗
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料产量");
                No2_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "原煤消耗量");
                No3_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料煤耗");
                if (No3_Row != -1)
                {
                    temp_Result.Rows[No3_Row]["Totals"] = ( No1_Row == -1 || No2_Row == -1||MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]) == 0) ? 0 : (MyToDecimal(temp_Result.Rows[No2_Row]["Totals"]) * 1000 / MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]));
                }
                //年度生料磨电耗
                //从组织表中获得生熟料比v_RawBatchClinker
                DataTable temp_ProductionLineInformation = _dataFactory.Query("select * from system_Organization where OrganizationID='" + v_OrganizationID + "'");
                decimal v_RawBatchClinker = temp_ProductionLineInformation.Rows.Count > 0 ? MyToDecimal(temp_ProductionLineInformation.Rows[0]["RawToClinkerCoff"]) : 0;
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料产量");
                No2_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "生料磨电耗");
                decimal v_Output_RawBatchMill = 0;      //年度生料磨产量
                decimal v_ElectricityConsumption_RawBatchMill = 0;      //年度生料磨耗电量
                for (int j = No_Col; j < No_Col + 12; j++) //1月到12月
                {
                    v_Output_RawBatchMill =(No1_Row==-1)?0:( v_Output_RawBatchMill + MyToDecimal(temp_Result.Rows[No1_Row][j]) * v_RawBatchClinker);
                    v_ElectricityConsumption_RawBatchMill =( No1_Row == -1 || No2_Row == -1) ? 0 : (v_ElectricityConsumption_RawBatchMill + MyToDecimal(temp_Result.Rows[No1_Row][j]) * v_RawBatchClinker * MyToDecimal(temp_Result.Rows[No2_Row][j]));
                }
                temp_Result.Rows[No2_Row]["Totals"] =(v_Output_RawBatchMill==0)?0: v_ElectricityConsumption_RawBatchMill / v_Output_RawBatchMill;
                //年度煤磨电耗
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料产量");
                No2_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "熟料煤耗");
                No3_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "煤磨电耗");
                decimal v_Output_CoalMill = 0;       //年度煤磨产量
                decimal v_ElectricityConsumption_CoalMill = 0;      //年度煤磨耗电量
                for (int j = No_Col; j < No_Col + 12; j++)
                {  //1月到12月
                    v_Output_CoalMill = (No1_Row == -1 || No2_Row == -1) ? 0 : (v_Output_CoalMill + MyToDecimal(temp_Result.Rows[No1_Row][j]) * MyToDecimal(temp_Result.Rows[No2_Row][j]) / 1000);
                    v_ElectricityConsumption_CoalMill = (No1_Row == -1 || No2_Row == -1) ? 0 : (v_ElectricityConsumption_CoalMill + MyToDecimal(temp_Result.Rows[No1_Row][j]) * MyToDecimal(temp_Result.Rows[No2_Row][j]) / 1000 * MyToDecimal(temp_Result.Rows[No3_Row][j]));
                }
                if (No3_Row != -1)
                {

                   temp_Result.Rows[No3_Row]["Totals"] =v_Output_CoalMill==0?0: (v_ElectricityConsumption_CoalMill / v_Output_CoalMill);                    
                }
            }
            if ("水泥磨" == v_type)//水泥磨
            {
                //年度水泥产量
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "水泥产量");
               
                if (No1_Row != -1)
                {
                    temp_Result.Rows[No1_Row]["Totals"] = 0;
                    for (int j = No_Col; j < No_Col + 12; j++)  //1月到12月
                    {
                        temp_Result.Rows[No1_Row]["Totals"] = MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]) + MyToDecimal(temp_Result.Rows[No1_Row][j]);
                    }
                }
                //年度电力消耗量
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "水泥电力消耗量");
                if (No1_Row != -1)
                {
                    temp_Result.Rows[No1_Row]["Totals"] = 0;

                    for (int j = No_Col; j < No_Col + 12; j++) //1月到12月
                    {
                        temp_Result.Rows[No1_Row]["Totals"] = MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]) + MyToDecimal(temp_Result.Rows[No1_Row][j]);
                    }
                }
                //年度水泥电耗
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "水泥产量");
                No2_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "水泥电力消耗量");
                No3_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "水泥电耗");
                if (No3_Row != -1)
                {
                   temp_Result.Rows[No3_Row]["Totals"] = (No1_Row == -1 || No2_Row == -1||MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]) == 0) ? 0 : (MyToDecimal(temp_Result.Rows[No2_Row]["Totals"]) / MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]));
                }
                //年度水泥磨电耗
                No1_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "水泥产量");
                No2_Row = ReportHelper.GetNoRow(temp_Result, "QuotasID", "水泥磨电耗");
                decimal v_ElectricityConsumption = 0;      //年度水泥磨耗电量
                for (int j = No_Col; j < No_Col + 12; j++)//1月到12月
                {
                    v_ElectricityConsumption =(No1_Row==-1||No2_Row==-1)?0:( v_ElectricityConsumption + MyToDecimal(temp_Result.Rows[No1_Row][j]) * MyToDecimal(temp_Result.Rows[No2_Row][j]));
                }
                if (No2_Row != -1)
                {
                    temp_Result.Rows[No2_Row]["Totals"] = ( No1_Row == -1||MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]) == 0) ? 0 : (v_ElectricityConsumption / MyToDecimal(temp_Result.Rows[No1_Row]["Totals"]));                    
                }
            }
        }
        /// <summary>
        /// 字典初始化
        /// </summary>
        /// <param name="dict"></param>
        private Dictionary<string, string> InitDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("熟料电耗", "ComprehensiveElectricityConsumption_Monthly");
            dict.Add("熟料煤耗", "ComprehensiveCoalConsumption_Monthly");
            dict.Add("生料制备电耗", "ElectricityConsumption_RawBatch_Monthly");
            dict.Add("生料磨电耗", "ElectricityConsumption_RawBatchMill_Monthly");
            dict.Add("熟料烧成电耗", "ElectricityConsumption_Clinker_Monthly");
            dict.Add("煤磨电耗", "ElectricityConsumption_CoalDust_Monthly");
            dict.Add("水泥电耗", "ComprehensiveElectricityConsumption_Monthly");
            dict.Add("水泥制备电耗", "ElectricityConsumption_Cement_Monthly");
            dict.Add("水泥磨电耗", "ElectricityConsumption_CementGrinding_Monthly");
            return dict;
        }

        /// <summary>
        /// 此方法只是为了减少重复代码（将otherTable表中的数据导入resultTable表）
        /// </summary>
        /// <param name="otherTable">temp_plan表，或temp_history表</param>
        /// <param name="resultTable">resultTable：结果表</param>
        /// <param name="firsName"></param>
        /// <param name="secondName"></param>
        /// <param name="No_Col"></param>
        private void MyFirstMethod(DataTable otherTable, DataTable resultTable, string firsName, string secondName, int No_Col)
        {
            int otherTableJanuary = otherTable.Columns.IndexOf("January");
            int resultTableJanuary = resultTable.Columns.IndexOf("January");
            int No1_Row, No2_Row;//, No3_Row;
            No1_Row = ReportHelper.GetNoRow(otherTable, "QuotasID", firsName);
            No2_Row = ReportHelper.GetNoRow(resultTable, "QuotasID", secondName);
            if (No2_Row != -1)
            {
                for (int i = 0; i < 12; i++) //1月到年度
                {
                    resultTable.Rows[No2_Row][i + resultTableJanuary] =No1_Row==-1?0: otherTable.Rows[No1_Row][i + otherTableJanuary];
                }
            }
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
