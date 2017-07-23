
$(document).ready(function () {
    //LoadProductionType('first');
    //loadOrganisationTree('first');

    //$('#TextBox_OrganizationId').textbox('hide');
    SetYearValue();
    LoadPurchaseSalesResultData('first');
    initPageAuthority();
});
//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "PurchaseSalesResult.aspx/AuthorityControl",
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
}
function SetYearValue() {
    var m_PlanYear = new Date().getFullYear();
    $('#numberspinner_PlanYear').numberspinner('setValue', m_PlanYear);
}
function QueryPurchaseSalesResultInfoFun() {

    var m_OrganizationId = $('#TextBox_OrganizationId').val();
    var m_PlanYear = $('#numberspinner_PlanYear').numberspinner('getValue');
    //var m_PurchaseSalesResultType = $('#drpDisplayType').combobox('select');
    if (m_OrganizationId != "") {
        LoadPurchaseSalesResultData('last');
    }
    else {
        alert("请选择正确的产线!");
    }
}

function LoadPurchaseSalesResultData(myLoadType) {
    $.messager.progress({
        title: 'Please waiting',
        msg: 'Loading data...'
    });

    var m_OrganizationId = $('#TextBox_OrganizationId').val();
    var m_PlanYear = $('#numberspinner_PlanYear').numberspinner('getValue');
    var m_Type = $('#drpDisplayType').combobox('getValue');
    var m_GridCommonName = "PurchaseSalesResultInfo";
    $.ajax({
        type: "POST",
        url: "PurchaseSalesResult.aspx/GetPurchaseSalesResultInfo",
        data: "{myOrganizationId:'" + m_OrganizationId + "',myType:'" + m_Type + "',myPlanYear:'" + m_PlanYear + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == 'first') {
                InitializePurchaseSalesResultGrid(m_GridCommonName, m_MsgData);
            }
            else if (myLoadType == 'last') {
                $('#grid_' + m_GridCommonName).datagrid('loadData', m_MsgData);
            }
            if (m_MsgData != null && m_MsgData != undefined) {
                for (var i = 0; i < m_MsgData.rows.length / 2; i++) {
                    $('#grid_PurchaseSalesResultInfo').datagrid('mergeCells', {
                        index: i * 2,
                        field: 'QuotasName',
                        rowspan: 2
                    });
                }
            }
            $.messager.progress('close');
        },
        error: function (msg) {
            $.messager.progress('close');
        }
    });
}
function RefreshPurchaseSalesResultFun() {
    QueryPurchaseSalesResultInfoFun();
}

//////////////////////////////////初始化基础数据//////////////////////////////////////////
function InitializePurchaseSalesResultGrid(myGridId, myData) {

    var m_FrozenColumns = myData['columns'].splice(0, 4);
    m_FrozenColumns[0]["hidden"] = true;
    m_FrozenColumns[1]["hidden"] = true;
    $('#grid_' + myGridId).datagrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        idField: m_FrozenColumns[0].field,
        frozenColumns: [m_FrozenColumns],
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