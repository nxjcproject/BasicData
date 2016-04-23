$(function () {
    InitializePage();
    //publicData.organizationId = $.getUrlParam('organizationId');

    loadGridData('first');
    initPageAuthority();
});

//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "List.aspx/AuthorityControl",
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
                $("#editBtn").linkbutton('disable');
            }
            //删除
            if (authArray[3] == '0') {
                $("#delete").linkbutton('disable');
            }
        }
    });
}
var publicData = {
    organizationId: "",
    editIndex: "",
    editRow: {}
}

var pvfData = {
    //organizationId: '',
    //tzStartDate: '',
    dataDetail: []
}

function onOrganisationTreeClick(node) {
    publicData.organizationId = node.OrganizationId;
    $('#organizationName').textbox('setText', node.text);
    //alert(publicData.organizationName);
    //InitializePage();
}
function InitializePage() {
    $('#startUsing').datebox({
        required: "true",
        //formatter: $.dateFormatter,
        //parser: $.dateParser
    });
    $('#endUsing').datebox({
        required: "true",
        //formatter: $.dateFormatter,
        //parser: $.dateParser
    });
    $('#selectedDate').datetimespinner({
        //required: "true",
        //value: $.dateFormatter(new Date()),
        selections: [[0, 4], [5, 7], [8, 10]],
        formatter: $.dateFormatter,
        parser: $.dateParser
    });
}

function loadGridData(myLoadType) {
    var selectedDate = $('#selectedDate').datetimespinner('getValue');

    $.ajax({
        type: "POST",
        url: "List.aspx/GetPVFList",
        data: "{organizationId:'" + publicData.organizationId + "', startUsing: '" + selectedDate + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (myLoadType == 'first') {
                myLoadType = 'last';
                m_MsgData = jQuery.parseJSON(msg.d);
                InitializeGrid(m_MsgData);
            }
            else if (myLoadType == 'last') {
                m_MsgData = jQuery.parseJSON(msg.d);
                $('#dg').datagrid('loadData', m_MsgData);
            }
        }
    });
}
function InitializeGrid(myData) {
    $('#dg').datagrid({
        data: myData,
        iconCls: 'icon-edit', singleSelect: true, rownumbers: true, striped: true, toolbar: '#tb',
        columns: [[
            {
                field: 'StartUsing', title: '启用时间', width: '30%', align: 'center',
                formatter: function (value) {
                    return value;//.substring(0, 10);
                }
            },
            {
                field: 'EndUsing', title: '停用时间', width: '30%', align: 'center',
                formatter: function (value) {
                    return value;//.substring(0, 10);
                }
            },
            {
                field: 'Flag', title: '启用标志', width: '20%', align: 'center',
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
                field: 'action', title: '操作', width: '14%', align: 'center',
                formatter: function (value, row, index) {
                    //var s = '<a href="Detail.aspx?keyid=' + row['KeyID'] + '">详细</a> ';
                    //detailItem(row['KeyID']);
                    var s = '<a href="javascript:void(0)" onclick="detailItem()">详细</a> ';
                    return s;
                }
            }
        ]]
    });
}

function detailItem(myKeyId) {
    var row = $("#dg").datagrid('getSelected');
    publicData.editRow = row;
    if (row == null) {
        alert('请选中目标行！');
    }
    else {
        //parent.$.messager.progress({ text: '数据加载中....' });
        var m_MsgData;
        $.ajax({
            type: "POST",
            url: "List.aspx/GetPVFDetail",
            data: "{keyId: '" + row['KeyID'] + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                m_MsgData = jQuery.parseJSON(msg.d);
                showIngrid(m_MsgData);
            }
        });
    }
}
function showIngrid(myData) {
    $('#detaildg').datagrid({
        data: myData,
        iconCls: 'icon-edit', singleSelect: true, rownumbers: true, striped: true,
        columns: [[
            { field: 'StartTime', title: '起始时间', width: '40%', align: 'center' },
            { field: 'EndTime', title: '终止时间', width: '40%', align: 'center' },
            { field: 'Type', title: '类型', width: '19%', align: 'center' },
        ]]
    });
    $('#detailDialog').dialog('open');
}

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

        //if (row['StartUsing'] != "") {
        //    var start = row['StartUsing'];//.substring(0, 10);
        //    $('#startUsing').datebox('setValue', start);
        //    //$('#startUsing').val(start);
        //}

        //if (row['EndUsing'] != "") {
        //    var end = row['EndUsing'];//.substring(0, 10);
        //    $('#endUsing').datebox('setValue', end);
        //    //$('#endUsing').val(end);
        //}
        if (row['Flag'] == 'true') {
            $("#radioTrue").attr("checked", "checked");
        }
        else {
            $("#radioFalse").attr("checked", "checked");
        }
        $('#editDialog').dialog('open');
    }
}
function saveEditDialog() {
    var startUsing = $('#startUsing').datebox('getValue');
    var endUsing = $('#endUsing').datebox('getValue');
    if (startUsing == "" || endUsing == "") {
        alert("请填写启停时间！");
    }
    else {
        var flag = $("input[name='radiobutton']:checked").val();
        $('#dg').datagrid('updateRow', {
            index: publicData.editIndex,
            row: {
                StartUsing: startUsing,
                EndUsing: endUsing,
                Flag: flag
            }
        });
        var id = publicData.editRow["ID"];
        $('#editDialog').dialog('close');
        editData(id, startUsing, endUsing, flag);
    }
}
function editData(id, startUsing, endUsing, flag) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    $.ajax({
        type: "POST",
        url: "List.aspx/UpdatePVFList",
        data: "{id:'" + id + "',startUsing: '" + startUsing + "',endUsing: '" + endUsing + "',flag:'" + flag + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == '1') {
                alert("修改成功！");
            } else if (msg.d =="noright" ) {
                alert("用户没有修改权限！");
            }
            else {
                alert("修改失败！");
            }
        }
    });
}

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
                deleteData(row["KeyID"]);
            }
        });
    }
}
function deleteData(keyId) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    $.ajax({
        type: "POST",
        url: "List.aspx/DeletePVFList",
        data: "{keyId: '" + keyId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == '1') {
                alert("删除成功！");
            }
            else if(msg.d=='noright'){
                alert("用户没有删除权限！");
            }
            else {
                alert("删除失败！");
            }
        }
    });
}
function addItem() {
    if (publicData.organizationId == "") {
        alert("请选择生产线！");
    }
    else {
        //window.location.href = "Edit.aspx?organizationId=" + publicData.organizationId;
        $('#adddg').datagrid({
            data: '',
            iconCls: 'icon-edit', singleSelect: true, rownumbers: true, striped: true, toolbar: '#addtb',
            columns: [[
                { field: 'StartTime', title: '起始时间', width: '40%', align: 'center' },
                { field: 'EndTime', title: '终止时间', width: '40%', align: 'center' },
                { field: 'Type', title: '类型', width: '19%', align: 'center' },
            ]]
        });
        $('#addDialog').dialog('open');
    }
}

//datagrid添加一行数据
function addItemPeak() {
    var items = $('#adddg').datagrid('getRows');
    if (items.length > 0) {
        var startTime = items[items.length - 1].EndTime;
        $('#startTime').timespinner('setValue', startTime);
        $('#endTime').timespinner('setValue', '23:59:59');
    }
    else {
        $('#startTime').timespinner('setValue', '00:00:00');
        $('#endTime').timespinner('setValue', '23:59:59');
    }
    $('#edit').dialog('open');
}

//保存对话框信息到datagrid
function saveAddDialog() {
    var startTime = $('#startTime').val();
    var endTime = $('#endTime').val();
    var type = $('#type').combobox('getValue');
    if (validateTime(startTime, endTime) == 0) {
        $('#adddg').datagrid('appendRow', { 'StartTime': startTime, 'EndTime': endTime, 'Type': type });
    }
    else {
        alert('起止时间格式不正确');
    }
    $('#edit').dialog('close');
}
//验证起始时间和终止时间的大小
function validateTime(startTime, endTime) {
    var start = startTime.split(":");
    var startToInt = parseInt(start[0]) * 3600 + parseInt(start[1]) * 60 + parseInt(start[2]);

    var end = endTime.split(":");
    var endToInt = parseInt(end[0]) * 3600 + parseInt(end[1]) * 60 + parseInt(end[2]);

    if (startToInt < endToInt) {
        return 0;
    }
    else {
        return -1;
    }
}

//将datagrid中的数据持久化到数据库
function saveItem() {
    var items = $('#adddg').datagrid('getRows');
    if (items[items.length - 1].EndTime != "23:59:59") {
        alert('请完成24小时的定义！');
    }
    else {
        //pvfData.organizationId = publicData.organizationId;
        //pvfData.tzStartDate = new Date(); //$('#startUsing').datetimebox('getValue');
        for (var i = 0; i < items.length; i++) {
            pvfData.dataDetail.push(items[i]);
        }

        $.ajax({
            type: "POST",
            url: "List.aspx/Save",
            data: "{organizationId:'" + publicData.organizationId + "',myJsonData:'" + JSON.stringify(pvfData) + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d == "1") {
                    $('#addDialog').dialog('close');
                    alert("更新成功!");
                    loadGridData('first');
                } else if (msg.d == "noright") {
                    alert("用户没有更新权限！");
                }
                else {
                    $('#addDialog').dialog('close');
                    alert("更新失败!");
                }
            }
        });
    }
}
//删除datagrid的一行数据
function deleteTimeItem() {
    $.messager.defaults = { ok: "是", cancel: "否" };
    $.messager.confirm('提示', '确定要删除最后一行？', function (r) {
        if (r) {
            var index = $('#adddg').datagrid('getRows').length - 1;
            $('#adddg').datagrid('deleteRow', index);
        }
    });
}
function clearAllItems() {
    var items = $('#adddg').datagrid('getRows');
    $.messager.defaults = { ok: "是", cancel: "否" };
    $.messager.confirm('提示', '确定要清除所有列？', function (r) {
        if (r) {
            var index = $('#adddg').datagrid('loadData', []);
        }
    });
}