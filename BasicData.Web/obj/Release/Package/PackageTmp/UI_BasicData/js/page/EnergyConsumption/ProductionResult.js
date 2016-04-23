var editIndex = undefined;
var PlanYearCurrentDataGrid;                //当前datagrid显示的计划年
var OrganizationIdCurrentDataGrid;            //当前datagrid显示的组织机构
var QuotasType;                               //计划类型
$(document).ready(function () {
    //LoadProductionType('first');
    //loadOrganisationTree('first');

    //$('#TextBox_OrganizationId').textbox('hide');
    SetYearValue();
    LoadEquipmentInfo('first');
    LoadQuotasInfo('first');
    LoadProductionResultData('first');
    initPageAuthority();
});
//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "ProductionResult.aspx/AuthorityControl",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            var authArray = msg.d;
            //增加
            //if (authArray[1] == '0') {
            //    $("#add").linkbutton('disable');
            //}
            //修改
            if (authArray[2] == '0') {
                $("#id_save").linkbutton('disable');
            }
            //删除
            //if (authArray[3] == '0') {
            //    $("#delete").linkbutton('disable');
            //}
        }
    });
}
function onOrganisationTreeClick(myNode) {
    //alert(myNode.text);
    $('#TextBox_OrganizationId').attr('value', myNode.OrganizationId);  //textbox('setText', myNode.OrganizationId);
    $('#TextBox_OrganizationText').textbox('setText', myNode.text);
    //$('#TextBox_OrganizationType').textbox('setText', myNode.OrganizationType);
    LoadEquipmentInfo('last');
}
function SetYearValue() {
    var m_PlanYear = new Date().getFullYear();
    $('#numberspinner_PlanYear').numberspinner('setValue', m_PlanYear);
}
function QueryProductionResultInfoFun() {

    var m_OrganizationId = $('#TextBox_OrganizationId').val();
    var m_PlanYear = $('#numberspinner_PlanYear').numberspinner('getValue');
    //var m_ProductionResultType = $('#drpDisplayType').combobox('select');
    PlanYearCurrentDataGrid = m_PlanYear;
    OrganizationIdCurrentDataGrid = m_OrganizationId;
    if (m_OrganizationId != "") {
        LoadProductionResultData('last');
    }
    else {
        alert("请选择正确的产线!");
    }
}
function InitializeQuotasInfo(myData) {
    $('#Combobox_Quotas').combobox({
        data: myData,
        dataType: "json",
        valueField: 'QuotasId',
        textField: 'QuotasName',
        panelHeight: 'auto',
        editable: false,
        onSelect:function(myRecord){
            QuotasType = myRecord.Type;
        }
    });
}
function InitializeEquipmentInfo(myData) {
    $('#Combobox_Equipment').combobox({
        data: myData,
        dataType: "json",
        valueField: 'EquipmentCommonId',
        textField: 'EquipmentCommonName',
        panelHeight: 'auto',
        editable: false,
        onSelect: function (myRecord) {
            $('#Combobox_Quotas').combobox('clear');
            LoadQuotasInfo('last');
        }
    });
}
function LoadQuotasInfo(myLoadType) {
    var m_EquipmentCommonId = $('#Combobox_Equipment').combobox('getValue');
    $.ajax({
        type: "POST",
        url: "ProductionResult.aspx/GetQuotasInfo",
        data: "{myEquipmentCommonId:'" + m_EquipmentCommonId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == 'first') {
                InitializeQuotasInfo(m_MsgData.rows);
            }
            else {
                $('#Combobox_Quotas').combobox('loadData', m_MsgData.rows);
            }
        }
    });
}
function LoadEquipmentInfo(myLoadType) {
    var m_OrganizationId = $('#TextBox_OrganizationId').val();
    $.ajax({
        type: "POST",
        url: "ProductionResult.aspx/GetEquipmentInfo",
        data: "{myOrganizationId:'" + m_OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == 'first') {
                InitializeEquipmentInfo(m_MsgData.rows);
            }
            else {
                $('#Combobox_Equipment').combobox('loadData', m_MsgData.rows);
            }
        }
    });
}
function LoadProductionResultData(myLoadType) {
    var m_OrganizationId = $('#TextBox_OrganizationId').val();
    var m_PlanYear = $('#numberspinner_PlanYear').numberspinner('getValue');
    var m_EquipmentCommonId = $('#Combobox_Equipment').combobox('getValue');
    var m_ProductionQuotasId = $('#Combobox_Quotas').combobox('getValue');
    
    var m_GridCommonName = "ProductionResultInfo";
    $.ajax({
        type: "POST",
        url: "ProductionResult.aspx/GetProductionResultInfo",
        data: "{myProductionQuotasId:'" + m_ProductionQuotasId + "',myQuotasType:'" + QuotasType + "',myOrganizationId:'" + m_OrganizationId + "',myPlanYear:'" + m_PlanYear + "',myEquipmentCommonId:'" + m_EquipmentCommonId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == 'first') {
                InitializeProductionResultGrid(m_GridCommonName, m_MsgData);
            }
            else if (myLoadType == 'last') {
                $('#grid_' + m_GridCommonName).datagrid('loadData', m_MsgData);
            }
            if (m_MsgData != null && m_MsgData != undefined) {
                for (var i = 0; i < m_MsgData.rows.length / 2; i++) {
                    $('#grid_ProductionResultInfo').datagrid('mergeCells', {
                        index: i * 2,
                        field: 'QuotasName',
                        rowspan: 2
                    });
                }
            }

        }
    });
}
function RefreshProductionResultFun() {
    QueryProductionResultInfoFun();
}

//////////////////////////////////初始化基础数据//////////////////////////////////////////
function InitializeProductionResultGrid(myGridId, myData) {

    var m_IdColumn = myData['columns'].splice(0, 2);
    $('#grid_' + myGridId).datagrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        idField: m_IdColumn[0].field,
        columns: [myData['columns']],
        //loadMsg: '',   //设置本身的提示消息为空 则就不会提示了的。这个设置很关键的
        rownumbers: true,
        //pagination: true,
        singleSelect: true,
        //idField: m_IdAndNameColumn[0].field,
        //pageSize: 20,
        //pageList: [20, 50, 100, 500],

        toolbar: '#toolbar_' + myGridId
    });
}

function MathSumColumnsValue(myDataGridId, editIndex) {
    var m_Columns = $('#' + myDataGridId).datagrid("options").columns;
    var m_Rows = $('#' + myDataGridId).datagrid("getRows");
    var m_SumValue = 0;
    for (var i = 0; i < 12; i++) {
        var m_Field = m_Columns[0][i + 1].field;
        var m_Value = Number(m_Rows[editIndex][m_Field]);
        if (m_Value != "" && m_Value != null && m_Value != undefined && m_Value != NaN) {
            m_SumValue = m_SumValue + m_Value;
        }
    }
    m_Rows[editIndex]["Totals"] = m_SumValue.toString();
    $('#' + myDataGridId).datagrid("refreshRow", editIndex);
}