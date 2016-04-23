var organizationId = '';

$(document).ready(function () {
    var data = $('#organisationTree').tree('getData');
    $('#organisationTree').tree('getData');
    initPageAuthority();
});
//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "KpiList.aspx/AuthorityControl",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            var authArray = msg.d;
            //增加
            if (authArray[1] == '0') {
                $("#add_list").linkbutton('disable');
                $("#add_detail").linkbutton('disable');
            }
            //修改
            //if (authArray[2] == '0') {
            //    $("#edit").linkbutton('disable');
            //}
            //删除
            if (authArray[3] == '0') {
                $("#delete_list").linkbutton('disable');
                $("#delete_detail").linkbutton('disable');
            }
        }
    });
}
// KPI引领双击事件
function OnKpiListDblClicked(index, row) {
    $("#txtCurrentKpi").textbox('setValue', row.StandardName);
    $('#txtCurrentKeyId').val(row.KeyId);
    $('#drpDisplayType').combobox('select', 'All');
    $('#dgKpiDetail').datagrid({ data: [] });

    QueryDetail();
}

// KPI详细双击
function OnDgKpiDetailClicked(index, row) {
    EditKpiDetail();
}

// 统计方式改变时
function OnStatisticalMethodChanged(rec) {
    $("#txtCurrentKpi").textbox('setValue', '');
    $('#txtCurrentKeyId').val('');
    $('#dgKpiList').datagrid({ data: [] });
    $('#dgKpiDetail').datagrid({ data: [] });

    closePnlOrganization();
}

// 显示类别改变时
function OnDisplayTypeChanged(rec) {
    if (rec.value == 'All' || rec.value == 'Public') {
        closePnlOrganization();
        QueryDetail();
    }
    else {
        openPnlOrganization();
        $('#dgKpiDetail').datagrid({ data: [] });
    }
}


// 双击组织机构时
function onOrganisationTreeClick(node) {
    organizationId = node.OrganizationId;
    $('#dgKpiDetail').datagrid({ data: [] });
    QueryDetail();
}

function closePnlOrganization() {
    $('#pnlOrganization').panel('close');
    $('#mainLayout').layout('resize');
}

function openPnlOrganization() {
    $('#pnlOrganization').panel('open');
    $('#mainLayout').layout('resize');
}

// 查询KPI列表
function QueryList() {
    var statisticalMethod = $('#drpStatisticalMethod').combobox('getValue');
    var queryUrl = 'KpiList.aspx/GetKpiList';
    var dataToSend = '{statisticalMethod: "' + statisticalMethod + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#dgKpiList').datagrid({
                data: jQuery.parseJSON(msg.d)
            });
        }
    });
}

// 创建Kpi列表
function CreateKpiList() {
    $('#dlgKpiListEditor').dialog('open');
}

function KpiListSave() {
    var standardId = $('#drpStandard').combobox('getValue');
    var statisticalMethod = $('#drpStatisticalMethod').combobox('getValue');
    var displayIndex = $('#txtDisplayIndex').textbox('getText');

    var queryUrl = 'KpiList.aspx/CreateKpiList';
    var dataToSend = '{standardId:"' + standardId + '", statisticalMethod:"' + statisticalMethod + '", displayIndex:' + displayIndex + '}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', '添加成功。', 'info');

            // 添加成功后刷新
            $('#dlgKpiListEditor').dialog('close');
            QueryList();
            $('#dgKpiDetail').datagrid({ data: [] });
        }
    });
}

// 删除KPI列表
function DeleteList() {
    var row = $('#dgKpiList').datagrid('getSelected');
    if (row == undefined || row == null) {
        $.messager.alert('提示', '请选择需要删除的行。', 'info');
        return;
    }

    $.messager.confirm('提示', '确认删除：' + row.StandardName + '？', function (r) {
        if (r) {
            KpiDelete(row.KeyId);
        }
    });
}

function KpiDelete(keyId) {
    var queryUrl = 'KpiList.aspx/DeleteKpiList';
    var dataToSend = '{keyId: "' + keyId + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', '删除成功。', 'info');

            // 删除成功后刷新
            QueryList();
            $('#dgKpiDetail').datagrid({ data: [] });
        }
    });
}

// 查询KPI详细
function QueryDetail() {
    var keyId = $('#txtCurrentKeyId').val();

    if (keyId == '') {
        $.messager.alert('提示', '请先选择KPI指标', 'info');
        return;
    }

    var displayType = $('#drpDisplayType').combobox('getValue');

    var queryUrl;
    var dataToSend;

    if (displayType == 'All') {
        queryUrl = 'KpiList.aspx/GetKpiDetail_All';
        dataToSend = '{keyId: "' + keyId + '"}';
    }
    else if (displayType == 'Public') {
        queryUrl = 'KpiList.aspx/GetKpiDetail_Public';
        dataToSend = '{keyId: "' + keyId + '"}';
    }
    else if (displayType == 'Private') {
        queryUrl = 'KpiList.aspx/GetKpiDetail_Private';
        dataToSend = '{keyId: "' + keyId + '", organizationId: "' + organizationId + '"}';
    }

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#dgKpiDetail').datagrid({
                data: jQuery.parseJSON(msg.d)
            });
        }
    });
}

var kpiDetailSaveState = null;

// 编辑物料详细
function EditKpiDetail() {
    var row = $('#dgKpiDetail').datagrid('getSelected');
    if (row == undefined || row == null) {
        $.messager.alert('提示', '请选择需要编辑的行。', 'info');
        return;
    }

    // 设置保存按钮状态为更新
    kpiDetailSaveState = 'UPDATE';

    $('#dlgKpiDetailEditor').dialog('open');
    $('#txtName').textbox('setText', row.Name);
    $('#drpLevelType').combobox('select', row.LevelType);
    $('#txtVariableId').textbox('setText', row.VariableId);
    $('#drpValueType').combobox('select', row.ValueType);
    $('#drpUnit').combobox('select', row.Unit);
    $('#txtStandardValue').textbox('setText', row.StandardValue);
    $('#txtStandardLevel').textbox('setText', row.StandardLevel);
}

//添加物料详细
function AddKpiDetail() {
    kpiDetailSaveState = "ADD";

    $('#dlgKpiDetailEditor').dialog('open');
    $('#txtName').textbox('setText', '');
    $('#drpLevelType').combobox('select', 'Process');
    $('#txtVariableId').textbox('setText', '');
    $('#drpValueType').combobox('select', 'ElectricityConsumption');
    $('#drpUnit').combobox('select', 'kW·h/t');
    $('#txtStandardValue').textbox('setText', '');
    $('#txtStandardLevel').textbox('setText', '');
}

function KpiDetailSave() {
    if (kpiDetailSaveState == 'UPDATE') {
        KpiDetailSave_Update();
    }
    else if (kpiDetailSaveState == 'ADD') {
        KpiDetailSave_Add();
    }
}

function KpiDetailSave_Update() {
    var row = $('#dgKpiDetail').datagrid('getSelected');
    if (row == undefined || row == null) {
        $.messager.alert('提示', '请选择需要编辑的行。', 'info');
        return;
    }

    var standardItemId = row.StandardItemId;
    var organizationId = row.OrganizationID;
    var name = $('#txtName').textbox('getText');
    var levelType = $('#drpLevelType').combobox('getValue');
    var variableId = $('#txtVariableId').textbox('getText');
    var valueType = $('#drpValueType').combobox('getValue');
    var unit = $('#drpUnit').combobox('getValue');
    var standardValue = $('#txtStandardValue').textbox('getText');
    var standardLevel = $('#txtStandardLevel').textbox('getText');

    var queryUrl = 'KpiList.aspx/UpdateKpiDetail';
    var dataToSend = '{standardItemId: "' + standardItemId + '",name:"' + name + '",organizationId:"' + organizationId + '",levelType:"' + levelType + '",variableId:"' + variableId + '",valueType:"' + valueType + '",unit:"' + unit + '",standardValue:"' + standardValue + '",standardLevel:"' + standardLevel + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', '更新成功。', 'info');
            $('#dlgKpiDetailEditor').dialog('close');
            QueryDetail();
        }
    });
}

function KpiDetailSave_Add() {

    var keyId = $('#txtCurrentKeyId').val();

    var displayType = $('#drpDisplayType').combobox('getValue');
    if (displayType == 'All') {
        organizationId = '';
    }
    else if (displayType == 'Public') {
        organizationId = '';
    }

    var name = $('#txtName').textbox('getText');
    var levelType = $('#drpLevelType').combobox('getValue');
    var variableId = $('#txtVariableId').textbox('getText');
    var valueType = $('#drpValueType').combobox('getValue');
    var unit = $('#drpUnit').combobox('getValue');
    var standardValue = $('#txtStandardValue').textbox('getText');
    var standardLevel = $('#txtStandardLevel').textbox('getText');

    var queryUrl = 'KpiList.aspx/CreateKpiDetail';
    var dataToSend = '{keyId: "' + keyId + '",name:"' + name + '",organizationId:"' + organizationId + '",levelType:"' + levelType + '",variableId:"' + variableId + '",valueType:"' + valueType + '",unit:"' + unit + '",standardValue:"' + standardValue + '",standardLevel:"' + standardLevel + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', '添加成功。', 'info');
            $('#dlgKpiDetailEditor').dialog('close');
            QueryDetail();
        }
    });

}

// 删除KPI详细
function DeleteDetail() {
    var row = $('#dgKpiDetail').datagrid('getSelected');
    if (row == undefined || row == null) {
        $.messager.alert('提示', '请选择需要删除的行。', 'info');
        return;
    }

    $.messager.confirm('提示', '确认删除：' + row.Name + '？', function (r) {
        if (r) {
            KpiDetailDelete(row.StandardItemId);
        }
    });
}

function KpiDetailDelete(standardItemId) {
    var queryUrl = 'KpiList.aspx/DeleteKpiDetail';
    var dataToSend = '{standardItemId: "' + standardItemId + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', '删除成功。', 'info');

            // 删除成功后刷新
            QueryDetail();
        }
    });
}