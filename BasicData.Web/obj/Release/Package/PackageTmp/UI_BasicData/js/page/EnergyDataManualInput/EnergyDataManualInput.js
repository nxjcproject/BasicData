var nodeID;
$(function () {
    nodeID = $('#Hiddenfield_PageId').val();
    loadDataGrid("first");
    initPageAuthority();
    initDate();
});

var publicData = {
    organizationId: "",
    editIndex: "",
    editRow: {}
}

//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "EnergyDataManualInput.aspx/AuthorityControl",
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
//初始化时间框
function initDate() {
    var myDate = new Date()
    var nowDate = myDate.getFullYear() + '-' + (myDate.getMonth() + 1) + '-' + myDate.getDate() + " " + (myDate.getHours()) + ':' + (myDate.getMinutes()) + ':' + (myDate.getSeconds());
    var beforeMonthDate = myDate.getFullYear() + '-' + myDate.getMonth() + '-' + myDate.getDate() + " " + (myDate.getHours()) + ':' + (myDate.getMinutes()) + ':' + (myDate.getSeconds());
    $('#db_endTime').datetimebox({ value: nowDate, showSeconds: true });
    $('#db_startTime').datetimebox('setValue', beforeMonthDate);
}

function onOrganisationTreeClick(node) {
    publicData.organizationId = node.OrganizationId;
    $('#organizationName').textbox('setText', node.text);
}
function loadVaribleNameData() {
    var dataToServer = {
        nodeID: nodeID
    };
    $.ajax({
        type: "POST",
        // url: "EnergyDataManualInput.aspx/GetVariableNameData",
        url: "EnergyDataManualInput.aspx/SystemVariableTypeList",
        //data: '{nodeId:"' + nodeID + '"}',
        data: JSON.stringify(dataToServer),
        // data:"",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,//同步执行
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            // var comboboxValue = jQuery.parseJSON(msg.d);
            $('#addVariableName').combobox({
                data: m_MsgData.rows,
                valueField: 'VariableId',
                textField: 'VariableName'
            });
        },
        error: function () {
            alert('加载失败');
        }
    });
}

function query() {
    var startTime = $('#db_startTime').datetimebox('getValue');
    var endTime = $('#db_endTime').datetimebox('getValue');
    if (startTime == '' || endTime == '' || startTime > endTime || startTime == endTime) {
        $.messager.alert('提示', '请选择合适的开始时间和结束时间');
        return;
    }
    var dataToServer = {
        organizationId: publicData.organizationId,
        startTime: startTime,
        endTime: endTime,
        nodeID: nodeID
    };
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "EnergyDataManualInput.aspx/GetEnergyDataManualInputData",
        data: JSON.stringify(dataToServer),
        //     data: "{nodeID: '" + nodeID + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            m_MsgData = jQuery.parseJSON(msg.d);
            loadDataGrid("last", m_MsgData);
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        }
    });
}
function loadDataGrid(type, myData) {
    if (type == "first") {
        $('#dg').datagrid({
            //data: myData,
            iconCls: 'icon-edit', singleSelect: true, rownumbers: true, striped: true,// toolbar: '#tb',
            fit: true,
            toolbar: "#toolBar",
            columns: [[
                {
                    field: 'VariableName', title: '变量名称', width: '17%', align: 'center'
                },
                {
                    field: 'Name', title: '组织机构名称', width: '12%', align: 'center'
                },
                {
                    field: 'TimeStamp', title: '更新日期', width: '10%', align: 'center'
                },
                {
                    field: 'DataValue', title: '录入值', width: '16%', align: 'center'
                },
                {
                    field: 'UpdateCycle', title: '更新周期', width: '16%', align: 'center'
                },
                {
                    field: 'Version', title: '版本', width: '8%', align: 'center'
                },
                {
                    field: 'Remark', title: '备注', width: '20%', align: 'center'
                }
            ]]
        });
    }
    else {
        $('#dg').datagrid("loadData", myData);
    }
}
//添加按钮
function addItem() {
    if ($('#organizationName').textbox('getText') == '') {
        alert("请选择分厂！");
    }
    else {
        //  $('#addDialog').dialog('open');
        loadVaribleNameData();
        $('#addDialog').dialog('open');
    }
}
//添加对话框保存按钮
function saveAddDialog() {
    var addData = {};
    addData.organizationId = publicData.organizationId;
    addData.variableId = $('#addVariableName').combobox('getValue');
    addData.updateCycle = $('#addUpdateCycle').combobox('getValue');
    addData.dataValue = $('#addDataValue').numberbox('getValue');
    addData.timeStamp = $('#addTimeStamp').datebox('getValue');
    addData.remark = $('#addRemark').textbox('getText');

    if (addData.variableId != '' && addData.dataValue != '' && addData.timeStamp != '') {
        $.ajax({
            type: "POST",
            url: "EnergyDataManualInput.aspx/AddEnergyDataManualInputData",
            data: "{\"maddData\":'" + JSON.stringify(addData) + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d == '1') {
                    alert("添加成功！");
                    query();
                }
                else if (msg.d == '-2') {
                    alert("该数据已经提交,添加失败！");
                }
                else if (msg.d == "noright") {
                    alert("用户没有添加权限！");
                } else {
                    alert("添加失败！");
                }
            }
        });

        $('#addDialog').dialog('close');
        query();
    }
    else {
        alert('请输入必填项！');
    }
}
//删除数据按钮
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
                deleteData(row["DataItemId"]);
            }
        });
    }
}
function deleteData(id) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    $.ajax({
        type: "POST",
        url: "EnergyDataManualInput.aspx/DeleteEnergyDataManualInputData",
        data: "{id: '" + id + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == '1') {
                alert("删除成功！");
            }
            else if (msg.d == 'noright') {
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

        publicData.dataItemId = row['DataItemId'];
        $("#editVariableName").textbox("setText", row['VariableName']);
        $("#editUpdateCycle").textbox("setText", row['UpdateCycle']);
        $("#editDataValue").numberbox("setValue", row['DataValue']);
        $("#editTimeStamp").datebox("setValue", row['TimeStamp']);
        $("#editRemark").textbox("setText", row['Remark']);

        $('#editDialog').dialog('open');
    }
}
function saveEditDialog() {
    var editData = {};
    editData.dataItemId = publicData.dataItemId;
    editData.dataValue = $('#editDataValue').numberbox('getValue');
    editData.timeStamp = $('#editTimeStamp').datebox('getValue');
    editData.remark = $('#editRemark').textbox('getText');

    if (editData.dataValue != '' && editData.timeStamp != '') {
        $.ajax({
            type: "POST",
            url: "EnergyDataManualInput.aspx/EditEnergyDataManualInputData",
            data: "{\"editData\":'" + JSON.stringify(editData) + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d == '1') {
                    alert("修改成功！");
                }
                else if (msg.d == '-2') {
                    alert("该数据已经提交,修改失败！");
                }
                else if (msg.d == 'noright') {
                    alert("用户没有编辑权限！");
                }
                else {
                    alert("修改失败！");
                }
            }
        });

        $('#editDialog').dialog('close');
        query();
    }
    else {
        alert('请输入必填项！');
    }
}