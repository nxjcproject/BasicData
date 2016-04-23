$(function () {
    GetPageOpPermission();
});
function Query() {
    var organizationId = $('#organizationId').val();

    // 获取物料记录
    GetMaterialList(organizationId);
}
function GetPageOpPermission() {
    $.ajax({
        type: "POST",
        url: 'CoefficientUpdater.aspx/AuthorityControl',
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var authArray = msg.d;            
            //修改
            if (authArray[2] == '0') {
                $("#save").linkbutton('disable');
            }
        }
    });
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

// 保存物料系数
function SaveMaterialDetail() {
    var keyId = $('#currentMaterialKeyId').val();
    if (keyId == undefined || keyId == null || keyId == '') {
        $.messager.alert('提示', '请选择需要编辑的物料。', 'info');
        return;
    }

    $.messager.confirm('确认', '确认保存物料系数？', function (r) {
        if (r) {
            $('#dgMaterialDetail').datagrid('acceptChanges');
            var queryUrl = 'CoefficientUpdater.aspx/UpdateCoefficient';
            var dataToSend = '{keyId:"' + keyId + '",json:\'' + JSON.stringify($('#dgMaterialDetail').datagrid('getData')) + '\'}';

            $.ajax({
                type: "POST",
                url: queryUrl,
                data: dataToSend,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d == "success") {
                        $.messager.alert('提示', '物料系数保存成功。', 'info');
                        GetMaterialDetails(keyId);
                    }
                },
                error: function (msg) {
                    $.messager.alert('提示', '物料系数保存失败，错误原因：' + jQuery.parseJSON(msg.responseText).Message, 'error');
                }
            });

        }
    });
}


var EditIndex = undefined;

function EndEditing() {
    if (EditIndex == undefined) { return true }
    if ($('#dgMaterialDetail').datagrid('validateRow', EditIndex)) {
        // 结束编辑
        $('#dgMaterialDetail').datagrid('endEdit', EditIndex);
        EditIndex = undefined;
        return true;
    } else {
        return false;
    }
}

function OnClickRow(index) {
    if (EditIndex != index) {
        if (EndEditing()) {
            $('#dgMaterialDetail').datagrid('selectRow', index)
                    .datagrid('beginEdit', index);
            EditIndex = index;
        } else {
            $('#dgMaterialDetail').datagrid('selectRow', EditIndex);
        }
    }
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