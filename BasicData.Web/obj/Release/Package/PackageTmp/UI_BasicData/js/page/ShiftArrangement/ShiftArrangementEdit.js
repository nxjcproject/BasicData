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
            { field: 'WorkingTeam', title: '班组', width: 50 },
            { field: 'ShiftDate', title: '首次白班日期', width: 100, editor: { type: 'datebox' } },
        ]],
        rownumbers: true,
        singleSelect: true,
        toolbar: '#tb',
        onClickCell: onClickCell
    });
}
//function endEditing() {
//    if (editIndex == undefined) { return true }
//    if ($('#dg').datagrid('validateRow', editIndex)) {
//        var ed = $('#dg').datagrid('getEditor', { index: editIndex, field: 'ShiftDate' });
//        var n_date = $(ed.target).datebox('getValue');
//        //var year = n_date.getFullYear();
//        //var month = n_date.getMonth() + 1;
//        //var day = n_date.getDate();
//        $('#dg').datagrid('getRows')[editIndex]['ShiftDate'] = n_date;//year+"-"+month+"-"+day;
//        $('#dg').datagrid('endEdit', editIndex);
//        editIndex = undefined;
//        return true;
//    } else {
//        return false;
//    }
//}
//function onClickRow(index) {
//    //var m_data = $('#dg').datagrid('getData');
//    if (index == 1 && $('#dg').datagrid('getData').rows[0]["ShiftDate"] == "") {
//        alert('请先编辑A班时间');
//        return;
//    }
//    if (index == 2 && $('#dg').datagrid('getData').rows[1]["ShiftDate"] == "") {
//        alert('请先编辑B班时间');
//        return;
//    }
//    if (index == 3 && $('#dg').datagrid('getData').rows[2]["ShiftDate"] == "") {
//        alert('请先编辑C班时间');
//        return;
//    }
//    //var shiftDate = m_data.rows[0]["ShiftDate"];
//    if (editIndex != index) {
//        if (endEditing()) {
//            $('#dg').datagrid('selectRow', index)
//                    .datagrid('beginEdit', index);
//            editIndex = index;
//        } else {
//            $('#dg').datagrid('selectRow', editIndex);
//        }
//    }
//}

$().extend($.fn.datagrid.methods, {                     //扩展新的方法  editCell
    editCell: function (jq, param) {
        return jq.each(function () {
            var opts = $(this).datagrid('options');     //获取datagrid的属性
            var fields = $(this).datagrid('getColumnFields', true).concat($(this).datagrid('getColumnFields'));  //返回列字段，如果设置了frozen属性为true，将返回固定列的字段名
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);  //返回特定列的属性
                col.editor1 = col.editor;     //获取列属性的编辑
                if (fields[i] != param.field) {
                    col.editor = null;
                }
            }
            $(this).datagrid('beginEdit', param.index);    // 获取行
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor = col.editor1;
            }
        });
    }
});
var editIndex = undefined;      //重置编辑索引行
function endEditing() {
    if (editIndex == undefined) { return 'true' }     //返回真允许编辑
    if ($("#dg").datagrid('validateRow', editIndex)) {
        $("#dg").datagrid('endEdit', editIndex);
        editIndex = undefined;
        return 'true';
    } else {
        return 'false';
    }
}
function onClickCell(index, field) {
    if (endEditing()) {
        $("#dg").datagrid('selectRow', index)
                .datagrid('editCell', { index: index, field: field });
        editIndex = index;
    }
    if (index == 1 && $('#dg').datagrid('getData').rows[0]["ShiftDate"] == "") {
        alert('请先编辑A班时间');
        return;
    }
    if (index == 2 && $('#dg').datagrid('getData').rows[1]["ShiftDate"] == "") {
        alert('请先编辑B班时间');
        return;
    }
    if (index == 3 && $('#dg').datagrid('getData').rows[2]["ShiftDate"] == "") {
        alert('请先编辑C班时间');
        return;
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
            if (m_msg.total==0) {
                $.messager.alert('提示','没有班次信息');
            }
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
    var organizationId = $('#organizationId').val();
    //for (var i = 0; i < data.total; i++) {
    //    if(data.rows[i]["ShiftDate"]==""){
    //        data.rows[i]["ShiftDate"] = "NULL";
    //    }
    //}
    var json = JSON.stringify(data);
    $.ajax({
        type: "POST",
        url: "ShiftArrangementEdit.aspx/SaveData",
        data: "{organizationId:'"+ organizationId +"',json:'" + json + "'}",
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