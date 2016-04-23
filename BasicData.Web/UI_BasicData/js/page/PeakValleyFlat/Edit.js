$(function () {
    //loadGridData('first');
    pvfData.organizationId = $.getUrlParam('organizationId');
    InitializeGrid("");
    InitializePage();
});

var m_MsgData;

var pvfData = {
    organizationId:'',
    tzStartDate: '',
    dataDetail: []
}

//function loadGridData(myLoadType) {
//    //parent.$.messager.progress({ text: '数据加载中....' });
//    $.ajax({
//        type: "POST",
//        url: "",//"EditFengGuPing.asmx/GetFGPValueForGrid",
//        data: "{companyId: '1'}",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (msg) {
//            if (myLoadType == 'first') {
//                myLoadType = 'last';
//                m_MsgData = jQuery.parseJSON(msg.d);
//                InitializeGrid(m_MsgData);
//            }
//            else if (myLoadType == 'last') {
//                m_MsgData = jQuery.parseJSON(msg.d);
//                $('#dg').datagrid('loadData', m_MsgData);
//            }
//        }
//    });
//}

function InitializePage() {
    $('#startUsing').datebox({
        required: "true",
        formatter: function (date) {
            var y = date.getFullYear();
            var m = date.getMonth() + 1;
            var d = date.getDate();
            return y + "-" + (m < 10 ? ("0" + m) : m) + "-" + (d < 10 ? ("0" + d) : d);
        },
        parser: function (s) {
            if (!s) return new Date();
            var ss = (s.split('-'));
            var y = parseInt(ss[0], 10);
            var m = parseInt(ss[1], 10);
            var d = parseInt(ss[2], 10);
            if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
                return new Date(y, m - 1, d);
            } else {
                return new Date();
            }
        }
    });
}

function InitializeGrid(myData) {
    $('#dg').datagrid({
        data: myData,
        iconCls: 'icon-edit', singleSelect: true, rownumbers: true, striped: true, toolbar: '#tb',
        columns: [[
            { field: 'StartTime', title: '起始时间', width: '30%', align: 'center' },
            { field: 'EndTime', title: '终止时间', width: '30%', align: 'center' },
            { field: 'Type', title: '类型', width: '20%', align: 'center' },
        ]]
    });
}
//datagrid添加一行数据
function addItem() {
    var items = $('#dg').datagrid('getRows');
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
function saveEditDialog() {
    var startTime = $('#startTime').val();
    var endTime = $('#endTime').val();
    var type = $('#type').combobox('getValue');
    if (validateTime(startTime, endTime) == 0) {
        $('#dg').datagrid('appendRow', { 'StartTime': startTime, 'EndTime': endTime, 'Type': type });
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

//删除datagrid的一行数据
function deleteItem() {
    $.messager.defaults = { ok: "是", cancel: "否" };
    $.messager.confirm('提示', '确定要删除最后一行？', function (r) {
        if (r) {
            var index = $('#dg').datagrid('getRows').length - 1;
            $('#dg').datagrid('deleteRow', index);
        }
    });
}
function clearAllItems() {
    var items = $('#dg').datagrid('getRows');
    $.messager.defaults = { ok: "是", cancel: "否" };
    $.messager.confirm('提示', '确定要清除所有列？', function (r) {
        if (r) {
            var index = $('#dg').datagrid('loadData', []);
        }
    });
}

//将datagrid中的数据持久化到数据库
function saveItem() {
    var items = $('#dg').datagrid('getRows');
    if (items[items.length - 1].EndTime != "23:59:59") {
        alert('请完成24小时的定义！');
    }
    else {
        pvfData.tzStartDate = $('#startUsing').datetimebox('getValue');
        for (var i = 0; i < items.length; i++) {
            pvfData.dataDetail.push(items[i]);
        }

        $.ajax({
            type: "POST",
            url: "Edit.aspx/Save",
            data: "{myJsonData:'" + JSON.stringify(pvfData) + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d == "1") {
                    $('#save').dialog('close');
                    alert("更新成功!");
                } else {
                    $('#save').dialog('close');
                    alert("更新失败!");
                }
            }
        });
    }
}