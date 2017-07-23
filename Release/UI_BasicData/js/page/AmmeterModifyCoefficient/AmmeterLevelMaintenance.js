
var myData;

$(document).ready(function () {
    loadDataGrid('', 'first');
});

function loadDataGrid(data,type){
    if (type == "first") {
       // var c_data = queryAmmeterInfo();
        $('#CoefficientContainer').treegrid({
            singleSelect: true,
            rownumbers: true,
            columns: [[
                { field: 'LevelCode', title: '层次码', width: 160 },
                { field: 'AmmeterNumber', title: '电表编号', width: 90, editor: { type: 'text', options: { required: true } } },
                { field: 'ElectricRoom', title: '电气室', width: 100 },
                { field: 'AmmeterName', title: '电表名称', width: 120 }
            ]],
            idField: 'id',
            treeField: 'LevelCode',
            onContextMenu: onContextMenu,
            onClickRow: onClickRow
        });
    }
    else {
        $('#CoefficientContainer').treegrid('loadData', data);
    }
}

function onContextMenu(e, row) {
    e.preventDefault();
    $(this).treegrid('select', row.id);
    $('#MenuId').menu('show', {
        left: e.pageX,
        top: e.pageY
    });
}
//删除节点
function removeIt() {
    var node = $('#CoefficientContainer').treegrid('getSelected');
    if (node) {
        $('#CoefficientContainer').treegrid('remove', node.id);
    }
}
//增加节点
function append() {
    var maxValue = 0;
    var currentLevelCode = "";
    var node = $('#CoefficientContainer').treegrid('getSelected');
    var childrenRows = $('#CoefficientContainer').treegrid('getChildren',node.id);
    if (childrenRows.length == 0) {                      //没有子节点
        currentLevelCode = node.id + "01";
    }
    else {                                                //有子节点
        var childrenCount = 0;//第一次子节点的个数
        for (var i = 0; i < childrenRows.length; i++) {//遍历寻找最大节点
            if (childrenRows[i].id.length == node.id.length + 2) {
                childrenCount++;
                var t_num = childrenRows[i].id.slice(1);
                var p = Number(t_num);
                if (p > maxValue) {
                    maxValue = p;
                }
            }         
        }
        if (childrenCount == 0) {
            currentLevelCode = node.id + "01";
        } else {
            var s_levelcode = (maxValue + 1).toString();
            //补齐0
            if (s_levelcode.length % 2 != 0) {
                s_levelcode = "0" + s_levelcode;
            }
            currentLevelCode = node.id.slice(0, 1) + s_levelcode;
        }
    }
    $('#CoefficientContainer').treegrid('append', {
        parent: node.id,
        data: [{
                id:currentLevelCode,
                LevelCode: currentLevelCode,
                //AmmeterNumber: "",
                ElectricRoom: "",
                AmmeterName:""
        }]
    })
    $('#CoefficientContainer').treegrid('beginEdit', currentLevelCode);
    editingId = currentLevelCode;
}
var editingId;
function onClickRow(clickRow) {
    if (editingId != undefined) {
        var t = $('#CoefficientContainer');
        t.treegrid('endEdit', editingId);
        editingId = clickRow.id;
        $('#CoefficientContainer').treegrid('beginEdit', editingId);
        return;
    }
    var row = $('#CoefficientContainer').treegrid('getSelected');
    if (row) {
        editingId = row.id
        $('#CoefficientContainer').treegrid('beginEdit', editingId);
    }
}
function edit() {

}
function endEditing() {
    var t = $('#CoefficientContainer');
    if (editingId == undefined) { return true; }
    else {
        var selectRow = t.treegrid('select', editingId);
        if (selectRow.AmmeterNumber == "" || selectRow.AmmeterNumber == undefined) {//应校验电表编号格式是否以A开头
            alert("请输入电表编号");
            return false;
        }
        else {
            $('#CoefficientContainer').treegrid('beginEdit', editingId);
            editingId = undefined;
            return true;
        }
    }
}
function queryAmmeterInfo() {
    var organizationId = $('#organizationId').val();
    var sendData = "{organizationId:'" + organizationId + "'}";
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "AmmeterLevelMaintenance.aspx/GetAmmeterInfo",
        data: sendData,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            $.messager.progress('close');
            myData = JSON.parse(msg.d);
            return myData;
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        }
    })
}
//获取所有孩子节点
function getNextChildrens(treegridId) {
    var t = $("#" + treegridId);
    var node = t.treegrid('getSelected');
    var childrenRows = t.treegrid("getChildren",node.id);
    var nextChildrenRows=[];
    for (var i = 0; i < childrenRows.length; i++) {
        if (childrenRows[i].LevelCode.length == node.LevelCode.length + 2) {
            nextChildrenRows.push(childrenRows[i]);
        }
    }
    return nextChildrenRows;
}
//获取所有的子孙节点
function getAllDescendants(treegridId) {
    var rows = $("#" + treegridId).treegrid("getChildren", nodeId);
    return rows;
}

// 获取双击组织机构时的组织机构信息
function onOrganisationTreeClick(node) {

    // 设置组织机构ID
    // organizationId为其它任何函数提供当前选中的组织机构ID

    $('#organizationId').val(node.OrganizationId);

    // 设置组织机构名称
    // 用于呈现，在界面上显示当前的组织机构名称

    $('#txtOrganization').textbox('setText', node.text);
    queryAmmeterInfo();
    loadDataGrid(myData, "last");
}