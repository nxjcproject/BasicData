$(function () {
    //InitializePage();
    loadGridData();
    initPageAuthority();
});

var publicData = {
    organizationId: "",
    editIndex: "",
    editRow: {}
}
function initAddRoleCbo(msgData) {
    $("#cbo_AddRole").combobox({
        data: msgData.rows,
        valueField: 'ID',
        textField: 'Name',
        panelHeight: 'auto'

    })

}
var selectedRole//修改窗口，存放选中行的角色ID值
function initEditRoleCbo(msgData) {
    $("#cbo_editRole").combobox({
        data: msgData.rows,
        valueField: 'ID',
        textField: 'Name',
        panelHeight: 'auto',

        onLoadSuccess: function () { //加载完成后,设置选中第一项
            var val = $("#cbo_editRole").combobox('getData');
            for (i = 0; i < val.length; i++) {
                if (val[i].ID == selectedRole) {
                    $('#cbo_editRole').combobox('setText', val[i].Name);
                }
            }//外边的for
        }//Success

    })
}
//加载添加弹窗中变量分组信息
function LoadAddRoleTable() {
    $.ajax({
        type: "POST",
        url: "EnergyDataManualInputContrast.aspx/GetRoles",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            MsgData = jQuery.parseJSON(msg.d);
            if (MsgData.rows.length == 0) {
                alert('角色加载失败');
            }
            initAddRoleCbo(MsgData);
        }
    })
}
//加载编辑画面变量分组信息
function LoadEditRoleTable() {
    $.ajax({
        type: "POST",
        url: "EnergyDataManualInputContrast.aspx/GetRoles",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            MsgData = jQuery.parseJSON(msg.d);
            if (MsgData.rows.length == 0) {
                alert('角色加载失败');
            }
            initEditRoleCbo(MsgData);
        }
    })
}
function LoadRolesTable(initcbo, valueField) {
    $.ajax({
        type: "POST",
        url: "EnergyDataManualInputContrast.aspx/GetRoles",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            MsgData = jQuery.parseJSON(msg.d);
            if (MsgData.rows.length == 0) {
                alert('角色加载失败');
            }
            initcbo(MsgData, valueField);
        }
    })
}

//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "EnergyDataManualInputContrast.aspx/AuthorityControl",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            var authArray = msg.d;
            //增加
            if (authArray[1] == '0') {
                $("#add").linkbutton('disable');
            }
            //修改
            if (authArray[2] == '0') {
                $("#edit").linkbutton('disable');
            }
            //删除
            if (authArray[3] == '0') {
                $("#delete").linkbutton('disable');
            }
        }
    });
}
function onOrganisationTreeClick(node) {
    publicData.organizationId = node.OrganizationId;
    $('#organizationName').textbox('setText', node.text);
}

function loadGridData() {
    var dataToServer = {
        variableName: $('#variableName').val()
    };
    $.ajax({
        type: "POST",
        url: "EnergyDataManualInputContrast.aspx/GetEnergyDataManualInputContrastData",
        data: JSON.stringify(dataToServer),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            InitializeGrid(m_MsgData);
        }
    });
}
function InitializeGrid(myData) {
    $('#dg').datagrid({
        data: myData,
        iconCls: 'icon-edit', singleSelect: true, rownumbers: true, striped: true, toolbar: '#tb',
        columns: [[
            {
                field: 'VariableName', title: '变量名称', width: '20%', align: 'center'
            },
            {
                field: 'Type', title: '变量类别', width: '12%', align: 'center'
            },
            {
                field: 'Enabled', title: '启用标志', width: '10%', align: 'center',
                formatter: function (value) {
                    if (value == true || value == "True")
                        return "启用";
                    else if (value == false || value == "False")
                        return "禁用";
                    else
                        return "";
                }
            },
            {
                field: 'Creator', title: '创建人', width: '18%', align: 'center', hidden: true
            },
             {
                 field: 'GroupId', title: '角色ID', width: '18%', align: 'center', hidden: false
             },
            {
                field: 'CreateTime', title: '创建时间', width: '16%', align: 'center', hidden: true
            },
            {
                field: 'Remark', title: '备注', width: '35%', align: 'center'
            }
        ]]
    });
}
//添加按钮
function addItem() {
    $('#addDialog').dialog('open');
    LoadAddRoleTable();
}
//添加对话框保存按钮
function saveAddDialog() {
    var addData = {};
    addData.variableId = $('#addVariableId').textbox('getText');
    addData.variableName = $('#addVariableName').textbox('getText');
    addData.enabled = $("input[name='radiobutton']:checked").val();
    addData.type = $('#cbo_AddType').combobox('getValue');
    //   addData.creator = $('#addCreator').textbox('getText');
    // addData.createTime = $('#addCreateTime').datetimebox('getValue');
    addData.remark = $('#addRemark').textbox('getText');
    addData.role = $('#cbo_AddRole').combobox('getValue');

    if (addData.variableId != '' && addData.variableName != '' && addData.creator != '' && addData.createTime != '') {
        if (addData.role == '') {
            alert('请输入录入角色');
            return;
        }
        $.ajax({
            type: "POST",
            url: "EnergyDataManualInputContrast.aspx/AddEnergyDataManualInputContrastData",
            data: "{\"maddData\":'" + JSON.stringify(addData) + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d == '1') {
                    alert("添加成功！");
                }
                else if (msg.d == '-2') {
                    alert("ID值重复，添加失败！");
                }
                else if (msg.d == "noright") {
                    alert("用户没有添加权限！");
                }
                else {
                    alert("添加失败！");
                }
            }
        });

        $('#addDialog').dialog('close');
        loadGridData();
    }
    else {
        alert('请输入必填项！');
    }
}
//删除数据
function deleteItem() {
    var row = $("#dg").datagrid('getSelected');
    if (row == null) {
        alert('请选中一行数据！');
    }
    else {
        var index = $("#dg").datagrid('getRowIndex', row);
        $.messager.defaults = { ok: "是", cancel: "否" };
        $.messager.confirm('提示', '确定要删除选中行？', function (r) {
            if (r) {
                $('#dg').datagrid('deleteRow', index);
                deleteData(row['VariableId']);
            }
        });
    }
}
//删除按钮
function deleteData(id) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    $.ajax({
        type: "POST",
        url: "EnergyDataManualInputContrast.aspx/DeleteEnergyDataManualInputContrastData",
        data: "{variableId: '" + id + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == '1') {
                alert("删除成功！");
            } else if (msg.d == "noright") {
                alert("用户没有删除权限！");
            }
            else {
                alert("删除失败！");
            }
        }
    });
}
//编辑按钮
function editItem() {
    var row = $("#dg").datagrid('getSelected');
    publicData.editRow = row;
    if (row == null) {
        alert('请选中一行数据！');
    }
    else {
        var index = $("#dg").datagrid('getRowIndex', row);
        publicData.editIndex = index;
        $('#dg').datagrid('selectRow', index);
        var row = $('#dg').datagrid('getSelected');
        //groupId = row['ROLE_NAME'];
        publicData.variableId = row['VariableId'];
        selectedRole = row['GroupId'];
        //      LoadRolesTable(initEditRoleCbo, groupId);
        $("#editVariableName").textbox("setText", row['VariableName']);
        //   $("#editCreator").textbox("setText", row['Creator']);
        //  $("#editCreateTime").datetimebox('setValue', row['CreateTime']);
        $("#editRemark").textbox("setText", row['Remark']);
        $("#editVariableId").textbox("setText", row['VariableId'])

        if (row['Enabled'] == 'True') {
            $("#editradioTrue").attr("checked", "checked");
        }
        else {
            $("#editradioFalse").attr("checked", "checked");
        }
        $('#editDialog').dialog('open');
        LoadEditRoleTable();

    }
}
//function saveEditDialog() {
//    var editData = {};
//  editData.variableId = publicData.variableId;
//    editData.variableName = $('#editVariableName').textbox('getText');
//    editData.enabled = $("input[name='editradiobutton']:checked").val();
//   // editData.creator = $('#editCreator').textbox('getText');
//   // editData.createTime = $('#editCreateTime').datetimebox('getValue');
//    editData.remark = $('#editRemark').textbox('getText');
//   editData.role = $('#cbo_editRole').combobox('getValue');

//    if (editData.variableName != '' && editData.creator != '' && editData.createTime != '') {
//        $.ajax({
//            type: "POST",
//            url: "EnergyDataManualInputContrast.aspx/EditEnergyDataManualInputContrastData",
//            data: "{\"editData\":'" + JSON.stringify(editData) + "'}",
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            success: function (msg) {
//                if (msg.d == '1') {
//                    alert("修改成功！");
//                }
//                else if (msg.d == "noright") {
//                    alert("用户没有修改权限！");
//                }
//                else {
//                    alert("修改失败！");
//                }
//            }
//        });

//        $('#editDialog').dialog('close');
//        loadGridData();
//    }
//    else {
//        alert('请输入必填项！');
//    }
//}
function saveEditDialog() {
    var editData = {};
    editData.variableId = publicData.variableId;
    editData.variableName = $("#editVariableName").textbox('getText');
    editData.enabled = $("input[name='editradiobutton']:checked").val();
    editData.type = $("#cbo_editType").combobox('getValue');

    //   editData.creator = $("#editCreator").textbox('getText');
    //  editData.createTime = $("#editCreateTime").datetimebox('getValue');
    editData.remark = $("#editRemark").textbox('getText');
    editData.role = $("#cbo_editRole").combobox('getValue');
    editData.role = $("#cbo_editType").combobox('getValue');
    if (editData.variableName != '' && editData.creator != '' && editData.createTime != '') {
        $.ajax({
            type: "POST",
            url: "EnergyDataManualInputContrast.aspx/EditEnergyDataManualInputContrastData",
            data: "{\"editData\":'" + JSON.stringify(editData) + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d == '1') {
                    alert("修改成功！");
                }
                else if (msg.d == "noright") {
                    alert("用户没有修改权限！");
                }
                else {
                    alert("修改失败！");
                }
            }
        });

        $('#editDialog').dialog('close');
        loadGridData();
    }
    else {
        alert('请输入必填项！');
    }
}