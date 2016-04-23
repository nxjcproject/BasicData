
var editIndex = undefined;
$(function () {
    initDatagrid();
    initPageAuthority();
});
//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "ShiftArrangementEdit.aspx/AuthorityControl",
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
function initDatagrid() {
    $('#dg').datagrid({
        //data: myData,
        columns: [[
            { field: 'WorkingTeam', title: '班组', width: 130 },
            { field: 'ShiftDate', title: '首次白班日期', width: 180, editor: { type: 'datebox' } },
        ]],
        rownumbers: true,
        singleSelect: true,
        toolbar: '#tb',
        onClickRow: onClickRow
    });
}
function endEditing() {
    if (editIndex == undefined) { return true }
    if ($('#dg').datagrid('validateRow', editIndex)) {
        var ed = $('#dg').datagrid('getEditor', { index: editIndex, field: 'ShiftDate' });
        var n_date = $(ed.target).datebox('getValue');
        //var year = n_date.getFullYear();
        //var month = n_date.getMonth() + 1;
        //var day = n_date.getDate();
        $('#dg').datagrid('getRows')[editIndex]['ShiftDate'] = n_date;//year+"-"+month+"-"+day;
        $('#dg').datagrid('endEdit', editIndex);
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}

function onClickRow(index) {
    if (editIndex != index) {
        if (endEditing()) {
            $('#dg').datagrid('selectRow', index)
                    .datagrid('beginEdit', index);
            editIndex = index;
        } else {
            $('#dg').datagrid('selectRow', editIndex);
        }
    }
}

function onOrganisationTreeClick(node) {
    $('#organizationName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);//用一个隐藏控件传递organizationId的值OrganizationId
    loadDataGrid();
}

function loadDataGrid() {
    var m_OrganizationId = $('#organizationId').val();
    $.ajax({
        type: "POST",
        url: "ShiftArrangementEdit.aspx/GetData",
        data: "{organizationId:'" + m_OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_msg = jQuery.parseJSON(msg.d);
            $('#dg').datagrid('loadData', m_msg);
        },
        error: handleError
    });
}

function handleError() {
    $('#dg').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}
function saveError() {
    $.messager.alert('失败', '保存数据失败');
}

function saveShiftArrangement() {
    $('#dg').datagrid('endEdit', editIndex);
    editIndex = undefined;
    $('#dg').datagrid('acceptChanges');
    var data = $('#dg').datagrid('getData');
    var json = JSON.stringify(data);
    $.ajax({
        type: "POST",
        url: "ShiftArrangementEdit.aspx/SaveData",
        data: "{json:'" + json + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_meg = msg.d;
            if (m_meg == "failure") {
                alert("数据更新失败");
            }
            if (m_meg == "success") {
                alert("数据更新成功");
            }
            if (m_meg == "noright") {
                alert("用户没有修改权限！");
            }
        },
        error: saveError
    });
}

//撤销
function reject() {
    $('#dg').datagrid('rejectChanges');
    editIndex = undefined;
}