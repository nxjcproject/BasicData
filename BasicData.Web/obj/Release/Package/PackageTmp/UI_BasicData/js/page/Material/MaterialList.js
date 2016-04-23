var materialDetailSaveState = null;

function Query() {
    var organizationId = $('#organizationId').val();

    // 获取物料记录
    GetMaterialList(organizationId);
}

// 获取物料列表
function GetMaterialList(organizationId) {
    var queryUrl = 'MaterialList.aspx/GetMaterialList';
    var dataToSend = '{organizationId: "' + organizationId + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#dgMaterial').datagrid({
                data: jQuery.parseJSON(msg.d)
            });
        }
    });
}

// 创建物料列表
function CreateMaterialList() {
    var organizationId = $('#organizationId').val();
    if (organizationId == undefined || organizationId == null || organizationId == '') {
        $.messager.alert('提示', '请先选择组织机构。', 'info');
        return;
    }

    $('#dlgMaterialListEditor').dialog('open');
}

function MaterialListSave() {
    var organizationId = $('#organizationId').val();
    var name = $('#listName').textbox('getText');

    var queryUrl = 'MaterialList.aspx/CreateMaterialList';
    var dataToSend = '{organizationId: "' + organizationId + '",name:"' + name + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', '添加成功。', 'info');

            // 添加成功后刷新物料列表
            GetMaterialList(organizationId);
            $('#dlgMaterialListEditor').dialog('close');
            $('#dgMaterialDetail').datagrid({ data: [] });
        }
    });
}

// 删除物料列表
function DeleteMaterialList() {
    var row = $('#dgMaterial').datagrid('getSelected');
    if (row == undefined || row == null) {
        $.messager.alert('提示', '请选择需要删除的行。', 'info');
        return;
    }

    $.messager.confirm('提示', '确认删除物料列表：' + row.Name + '？', function (r) {
        if (r) {
            MaterialDelete(row.KeyID);
        }
    });
}


function MaterialDelete(keyId) {
    var queryUrl = 'MaterialList.aspx/DeleteMaterialList';
    var dataToSend = '{keyId: "' + keyId + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', '删除成功。', 'info');

            // 删除成功后刷新物料列表
            GetMaterialList($('#organizationId').val());
            $('#dgMaterialDetail').datagrid({ data: [] });
        }
    });
}


// 获取物料详细
function GetMaterialDetails(keyId) {
    var queryUrl = 'MaterialList.aspx/GetMaterialDetail';
    var dataToSend = '{keyId: "' + keyId + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#dgMaterialDetail').datagrid({
                data: jQuery.parseJSON(msg.d)
            });
        }
    });
}

// 编辑物料详细
function EditMaterialDetail() {
    var row = $('#dgMaterialDetail').datagrid('getSelected');
    if (row == undefined || row == null) {
        $.messager.alert('提示', '请选择需要编辑的行。', 'info');
        return;
    }

    // 设置保存按钮状态为更新
    materialDetailSaveState = 'UPDATE';

    $('#dlgMaterialDetailEditor').dialog('open');
    $('#detailName').textbox('setText', row.Name);
    $('#variableId').textbox('setText', row.VariableId);
    $('#type').combobox('select', row.Type);
    $('#unit').textbox('setText', row.Unit);
    $('#materialErpCode').textbox('setText', row.MaterialErpCode);
    $('#coefficient').textbox('setText', row.Coefficient);
    $('#tagTableName').textbox('setText', row.TagTableName);
    $('#formula').textbox('setText', row.Formula);
}

//添加物料详细
function AddMaterialDetail() {
    materialDetailSaveState = "ADD";

    $('#dlgMaterialDetailEditor').dialog('open');
    $('#detailName').textbox('setText', '');
    $('#variableId').textbox('setText', '');
    $('#type').combobox('select', 'Coal');
    $('#unit').textbox('setText', '');
    $('#materialErpCode').textbox('setText', '');
    $('#coefficient').textbox('setText', '');
    $('#tagTableName').textbox('setText', '');
    $('#formula').textbox('setText', '');
}

// 删除物料详细（单条）
function DeleteMaterialDetail() {
    var row = $('#dgMaterialDetail').datagrid('getSelected');
    if (row == undefined || row == null) {
        $.messager.alert('提示', '请选择需要删除的行。', 'info');
        return;
    }

    $.messager.confirm('提示', '确认删除物料：'+ row.Name +'？', function (r) {
        if (r) {
            MaterialDetailDelete(row.MaterialId);
        }
    });
}

function MaterialDetailSave() {
    if (materialDetailSaveState == 'UPDATE') {
        MaterialDetailSave_Update();
    }
    else if (materialDetailSaveState == 'ADD') {
        MaterialDetailSave_Add();
    }
}

function MaterialDetailSave_Add() {
    var keyId = $('#currentMaterialKeyId').val();
    var name = $('#detailName').textbox('getText');
    var variableId = $('#variableId').textbox('getText');
    var type = $('#type').combobox('getValue');
    var unit = $('#unit').textbox('getText');
    var materialErpCode = $('#materialErpCode').textbox('getText');
    var coefficient = $('#coefficient').textbox('getText');
    var tagTableName = $('#tagTableName').textbox('getText');
    var formula = $('#formula').textbox('getText');

    var queryUrl = 'MaterialList.aspx/CreateMaterialDetail';
    var dataToSend = '{keyId: "' + keyId + '",variableId:"' + variableId + '",name:"' + name + '",type:"' + type + '",unit:"' + unit + '",materialErpCode:"' + materialErpCode + '",tagTableName:"' + tagTableName + '",formula:"' + formula + '",coefficient:"' + coefficient + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', '添加成功。', 'info');

            // 删除成功后刷新物料详细表
            GetMaterialDetails($('#currentMaterialKeyId').val());
            $('#dlgMaterialDetailEditor').dialog('close');
        }
    });
}

function MaterialDetailSave_Update() {
    var materialId = $('#dgMaterialDetail').datagrid('getSelected').MaterialId;
    var name = $('#detailName').textbox('getText');
    var variableId = $('#variableId').textbox('getText');
    var type = $('#type').combobox('getValue');
    var unit = $('#unit').textbox('getText');
    var materialErpCode = $('#materialErpCode').textbox('getText');
    var coefficient = $('#coefficient').textbox('getText');
    var tagTableName = $('#tagTableName').textbox('getText');
    var formula = $('#formula').textbox('getText');

    var queryUrl = 'MaterialList.aspx/UpdateMaterialDetail';
    var dataToSend = '{materialId: "' + materialId + '",variableId:"' + variableId + '",name:"' + name + '",type:"' + type + '",unit:"' + unit + '",materialErpCode:"' + materialErpCode + '",tagTableName:"' + tagTableName + '",formula:"' + formula + '",coefficient:"' + coefficient + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', '更新成功。', 'info');

            // 删除成功后刷新物料详细表
            GetMaterialDetails($('#currentMaterialKeyId').val());
            $('#dlgMaterialDetailEditor').dialog('close');
        }
    });
}

function MaterialDetailDelete(materialId) {
    var queryUrl = 'MaterialList.aspx/DeleteMaterialDetail';
    var dataToSend = '{materialId: "' + materialId + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', '删除成功。', 'info');

            // 删除成功后刷新物料详细表
            GetMaterialDetails($('#currentMaterialKeyId').val());
        }
    });
}

// 物料列表操作列
function MaterialListOperateColumnFormatter(val, row) {
    return '<a href="HandoverLogDetail.aspx?keyId=' + row.KeyID.trim() + '">删除</a>';
}

// 物料详细操作列
function MaterialDetailOperateColumnFormatter(val, row) {
    return '<a href="javascript:void(0)" onclick="EditMaterialDetail();">修改</a>';
}

// 物料详细类型列格式化器
function TypeFormatter(value, row, index) {
    switch (value) {
        case 'Coal':
            return '煤粉';
        case 'Clinker':
            return '熟料';
        case 'Cement':
            return '水泥';
        default:
            return value;
    }
}

// 物料列表双击
function OnDgMaterialClicked(index, row) {
    var keyId = row.KeyID;

    GetMaterialDetails(keyId);
    $('#currentMaterialName').textbox('setText', row.Name);
    $('#currentMaterialKeyId').val(row.KeyID);
}

// 物料详细双击
function OnDgMaterialDetailClicked(index, row) {
    EditMaterialDetail();
}


// 获取双击组织机构时的组织机构信息
function onOrganisationTreeClick(node) {

    // 设置组织机构ID
    // organizationId为其它任何函数提供当前选中的组织机构ID

    $('#organizationId').val(node.OrganizationId);

    // 设置组织机构名称
    // 用于呈现，在界面上显示当前的组织机构名称

    $('#txtOrganization').textbox('setText', node.text);

    $('#dgMaterial').datagrid({ data: [] });
    $('#dgMaterialDetail').datagrid({ data: [] });

    Query();
}
