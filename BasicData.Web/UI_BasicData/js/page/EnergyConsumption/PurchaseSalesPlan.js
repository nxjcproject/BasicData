﻿var editIndex = undefined;
var PlanYearCurrentDataGrid;                //当前datagrid显示的计划年
var OrganizationIdCurrentDataGrid = "";            //当前datagrid显示的组织机构
var PlanType;                                 //计划类型
$(document).ready(function () {
    //LoadProductionType('first');
    //loadOrganisationTree('first');
    PlanType = $('#HiddenField_PlanType').val();
    //$('#TextBox_OrganizationId').textbox('hide');
    SetYearValue();

    LoadPurchaseSalesPlanData('first');
    initPageAuthority();
});
//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "PurchaseSalesPlan.aspx/AuthorityControl",
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
function QueryPurchaseSalesPlanInfoFun() {
    endEditing();           //关闭正在编辑

    var m_OrganizationId = $('#TextBox_OrganizationId').val();
    var m_PlanYear = $('#numberspinner_PlanYear').numberspinner('getValue');
    //var m_PurchaseSalesPlanType = $('#drpDisplayType').combobox('select');
    PlanYearCurrentDataGrid = m_PlanYear;
    OrganizationIdCurrentDataGrid = m_OrganizationId;
    if (m_OrganizationId != "") {
        LoadPurchaseSalesPlanData('last');
    }
    else {
        alert("请选择正确的产线!");
    }
}
function LoadPurchaseSalesPlanData(myLoadType) {
    var m_OrganizationId = $('#TextBox_OrganizationId').val();
    var m_PlanYear = $('#numberspinner_PlanYear').numberspinner('getValue');
    var m_PurchaseSalesPlanType = $('#drpDisplayType').combobox('getValue');
    var m_GridCommonName = "PurchaseSalesPlanInfo";
    $.ajax({
        type: "POST",
        url: "PurchaseSalesPlan.aspx/GetPurchaseSalesPlanInfo",
        data: "{myPurchaseSalesPlanType:'" + m_PurchaseSalesPlanType + "',myOrganizationId:'" + m_OrganizationId + "',myPlanYear:'" + m_PlanYear + "',myPlanType:'" + PlanType + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == 'first') {
                InitializePurchaseSalesPlanGrid(m_GridCommonName, m_MsgData);
            }
            else if (myLoadType == 'last') {
                $('#grid_' + m_GridCommonName).datagrid('loadData', m_MsgData);
            }
        }
    });
}
function RefreshPurchaseSalesPlanFun() {
    QueryPurchaseSalesPlanInfoFun();
}
function SavePurchaseSalesPlanFun() {
    endEditing();           //关闭正在编辑
    var m_DataGridData = $('#grid_PurchaseSalesPlanInfo').datagrid('getData');
    var m_PurchaseSalesPlanType = $('#drpDisplayType').combobox('getValue');
    if (m_DataGridData['rows'].length > 0) {
        var m_DataGridDataJson = '{"rows":' + JSON.stringify(m_DataGridData['rows']) + '}';
        if (OrganizationIdCurrentDataGrid == "") {
            alert("请选择分厂!");
        }
        else {
            $.ajax({
                type: "POST",
                url: "PurchaseSalesPlan.aspx/SetPurchaseSalesPlanInfo",
                data: "{myOrganizationId:'" + OrganizationIdCurrentDataGrid + "',myPlanYear:'" + PlanYearCurrentDataGrid + "',myPlanType:'" + PlanType + "',myPurchaseSalesPlanType:'" + m_PurchaseSalesPlanType + "',myDataGridData:'" + m_DataGridDataJson + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var m_Msg = msg.d;
                    if (m_Msg == '1') {
                        alert('修改成功!');
                    }
                    else if (m_Msg == '0') {
                        alert('修改失败!');
                    }
                    else if (m_Msg == '-1') {
                        alert('数据库错误!');
                    }
                    else if (m_Msg == 'noright') {
                        alert("该用户没有修改保存权限！");
                    }
                    else {
                        alert(m_Msg);
                    }
                }
            });
        }
    }
    else {
        alert("没有任何数据需要保存!");
    }
}
//////////////////////////////////初始化基础数据//////////////////////////////////////////
function InitializePurchaseSalesPlanGrid(myGridId, myData) {

    var m_IdColumn = myData['columns'].splice(0, 2);
    var m_FrozenColumns = myData['columns'].splice(0, 1);
    $('#grid_' + myGridId).datagrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        idField: m_IdColumn[0].field,
        frozenColumns: [m_FrozenColumns],
        columns: [myData['columns']],
        //loadMsg: '',   //设置本身的提示消息为空 则就不会提示了的。这个设置很关键的
        rownumbers: true,
        //pagination: true,
        singleSelect: true,
        onClickCell: onClickCell,
        //idField: m_IdAndNameColumn[0].field,
        //pageSize: 20,
        //pageList: [20, 50, 100, 500],

        toolbar: '#toolbar_' + myGridId
    });
}
function endEditing() {
    if (editIndex == undefined) { return true }
    if ($('#grid_PurchaseSalesPlanInfo').datagrid('validateRow', editIndex)) {
        $('#grid_PurchaseSalesPlanInfo').datagrid('endEdit', editIndex);

        //MathSumColumnsValue('grid_PurchaseSalesPlanInfo', editIndex);             //计算合计列
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}

function onClickCell(index, field) {
    if (endEditing()) {
        //var m_Rows = $('#grid_PurchaseSalesPlanInfo').datagrid("getRows")
        //var m_Formula = m_Rows[index]["StatisticalFormula"];         //屏蔽根据行的内容进行计算的行，这些行自动改变值
        //if (m_Formula.indexOf("{Line|") == -1) {
        $('#grid_PurchaseSalesPlanInfo').datagrid('selectRow', index)
                        .datagrid('editCell', { index: index, field: field });
        editIndex = index;
        //
        //     SelectedTagValue = GetTagValue('grid_PurchaseSalesPlanInfo', index);
        //}
    }
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