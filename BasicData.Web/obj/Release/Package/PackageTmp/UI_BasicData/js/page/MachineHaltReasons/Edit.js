//////////////////////////////////////////////////////////////////////
//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "Edit.aspx/AuthorityControl",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            var authArray = msg.d;
            //增加
            if (authArray[1] == '0') {
                var itemEl = $('#add')[0];
                $("#mm").menu('disableItem', itemEl);
                itemEl = $('#addReason')[0];
                $("#mm").menu('disableItem', itemEl);
                $("#addOneReason").linkbutton('disable');
            }
            //修改
            if (authArray[2] == '0') {
                $("#save").linkbutton('disable');
            }
            //删除
            if (authArray[3] == '0') {
                var itemEl = $('#remove')[0];
                $("#mm").menu('disableItem', itemEl);
            }
        }
    });
}
// 右键菜单
function onContextMenu(e, row) {
    e.preventDefault();
    $(this).treegrid('select', row.MachineHaltReasonID);
    $('#mm').menu('show', {
        left: e.pageX,
        top: e.pageY
    });
}

// 添加根节点
function appendRoot() {
    GetReasonRow("RootNode");
}

//添加子节点
function append() {
    GetReasonRow("ChildNode");
}
//创建设备故障原因ID
function GetReasonRow(Flag) {
    var queryUrl = 'Edit.aspx/GetReasonItemID';
    $.ajax({
        type: "POST",
        url: queryUrl,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_Msg = msg.d;
            if (m_Msg != null && m_Msg != undefined && m_Msg != "") {
                if (Flag == "RootNode") {           //添加根节点
                    var levelCode = getAppendRootLevelCode();
                    $('#tgMachineHaltReasonsEditor').treegrid('append', {
                        data: [{
                            ReasonItemID: m_Msg,
                            MachineHaltReasonID: levelCode,
                            ReasonText: '新增错误原因',
                            ReasonStatisticsTypeId: '',
                            Remarks: '',
                            Enabled: 'True'
                        }]
                    })
                }
                else if (Flag == "ChildNode") {
                    var node = $('#tgMachineHaltReasonsEditor').treegrid('getSelected');
                    if (node.MachineHaltReasonID.length == 7) {
                        $.messager.alert('错误', '最多只能有三级停机原因！');
                        return;
                    }
                    var levelCode = getAppendLevelCode(node.MachineHaltReasonID);
                    $('#tgMachineHaltReasonsEditor').treegrid('append', {
                        parent: node.MachineHaltReasonID,
                        data: [{
                            ReasonItemID: m_Msg,
                            MachineHaltReasonID: levelCode,
                            ReasonText: '新增错误原因',
                            ReasonStatisticsTypeId: '',
                            Remarks: '',
                            Enabled: 'True'
                        }]
                    })
                }
            }
            else {
                $.messager.alert('错误', '无法创建新设备故障原因！');
            }
        },
        error: function () {
            $.messager.alert('错误', '无法创建新设备故障原因！');
        }
    });
}
// 删除节点
function removeIt() {
    var node = $('#tgMachineHaltReasonsEditor').treegrid('getSelected');
    if (node) {
        $('#tgMachineHaltReasonsEditor').treegrid('remove', node.MachineHaltReasonID);
    }
}

// 收起节点
function collapse() {
    var node = $('#tgMachineHaltReasonsEditor').treegrid('getSelected');
    if (node) {
        $('#tgMachineHaltReasonsEditor').treegrid('collapse', node.MachineHaltReasonID);
    }
}

// 展开节点
function expand() {
    var node = $('#tgMachineHaltReasonsEditor').treegrid('getSelected');
    if (node) {
        $('#tgMachineHaltReasonsEditor').treegrid('expand', node.MachineHaltReasonID);
    }
}

////////////////////////////////////////////////////////////////////////////////

// 生成根节点ID
function getAppendRootLevelCode() {
    var rows = $('#tgMachineHaltReasonsEditor').treegrid('getRoots');
    if (rows.length == 0) {
        return 'E01';
    }
    else {
        var maxCode = 0;
        for (var i = 0; i < rows.length; i++) {
            var temp = rows[i].MachineHaltReasonID;
            if (temp.length != 3)
                continue;
            var p = parseInt(temp.substring(1, temp.length));
            if (p > maxCode)
                maxCode = p;
        }
        maxCode = maxCode + 1;
        if (maxCode.toString().length % 2 == 1)
            return 'E0' + maxCode;
        else
            return 'E' + maxCode;
    }
}

// 生成子节点ID
function getAppendLevelCode(parentId) {
    var rows = $('#tgMachineHaltReasonsEditor').treegrid('getChildren', parentId);

    if (rows.length == 0) {
        return parentId + '01';
    }
    else {
        var maxCode = 0;
        for (var i = 0; i < rows.length; i++) {
            var temp = rows[i].MachineHaltReasonID;
            if (temp.length != parentId.length + 2)
                continue;
            var p = parseInt(temp.substring(1, temp.length));
            if (p > maxCode)
                maxCode = p;
        }
        maxCode = maxCode + 1;
        if (maxCode.toString().length % 2 == 1)
            return 'E0' + maxCode;
        else
            return 'E' + maxCode;
    }
}

///////////////////////////////////////////////////////////

// 当前编辑行ID
var editingId;

// 编辑
function edit() {
    if (editingId != undefined) {
        $('#tgMachineHaltReasonsEditor').treegrid('select', editingId);
        return;
    }
    var row = $('#tgMachineHaltReasonsEditor').treegrid('getSelected');
    if (row) {
        editingId = row.MachineHaltReasonID;
        $('#tgMachineHaltReasonsEditor').treegrid('beginEdit', editingId);
    }
}

// 保存
function save() {
    if (editingId != undefined) {
        var t = $('#tgMachineHaltReasonsEditor');
        t.treegrid('endEdit', editingId);
        editingId = undefined;
    }
}

// 取消编辑
function cancel() {
    if (editingId != undefined) {
        $('#tgMachineHaltReasonsEditor').treegrid('cancelEdit', editingId);
        editingId = undefined;
    }
}

// tg单击处理
function clickRow(row) {
    save();
}

// tg双击处理
function dblClickRow(row) {
    // 首先进入编辑模式
    edit();
}

/////////////////////////////////////////////////////////////

// 获取所有停机原因
function loadMachineHaltReasons() {
    var queryUrl = 'Edit.aspx/GetMachineHaltReasonsWithTreeGridFormat';

    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });

    $.ajax({
        type: "POST",
        url: queryUrl,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            initializeMachineHaltReasonsEditor(jQuery.parseJSON(msg.d));
            $.messager.progress('close');
        },
        error: function () {
            $.messager.progress('close');
            $.messager.alert('错误', '数据载入失败！');
        }
    });
}

// 初始停机原因编辑器
function initializeMachineHaltReasonsEditor(jsonData) {
    $('#tgMachineHaltReasonsEditor').treegrid({
        data: jsonData,
        dataType: "json"
    });
}

///////////////////////////////////////////////////////////////////////


// 保存停机原因
function saveMachineHaltReasons() {
    var queryUrl = 'Edit.aspx/SaveMachineHaltReasons';
    save();
    var dataToSend = '{"json":\'' + JSON.stringify($('#tgMachineHaltReasonsEditor').treegrid('getData')) + '\'}';
    //var m_OrganizationId = "";
    //var dataToSend = '{organizationId: "' + m_OrganizationId + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('信息', '停机原因保存成功！', 'info');
        }
    });
}
function InitializeTreeGrid(myReasonStatisticsTypeData) {
    var m_Data = { "rows": [], "total": 0 };
    grid = $('#tgMachineHaltReasonsEditor').treegrid({
        iconCls: 'icon-edit',
        rownumbers: true,
        animate: true,
        data: m_Data.rows,
        collapsible: true,
        fitColumns: true,
        idField: 'MachineHaltReasonID',
        treeField: 'ReasonText',
        onContextMenu: onContextMenu,
        onClickRow: clickRow,
        onDblClickRow: dblClickRow,
        columns: [[{
            width: 100,
            title: '错误标识',
            field: 'ReasonItemID'
        }, {
            width: 50,
            title: '错误码',
            field: 'MachineHaltReasonID'
        }, {
            width: 100,
            title: '停机原因',
            field: 'ReasonText',
            editor: 'text'
        }, {
            width: 100,
            title: '原因类别',
            field: 'ReasonStatisticsTypeId',
            formatter: function (value, row) {
                var m_ReasonStatisticsTypeId = row.ReasonStatisticsTypeId || value;
                for (var i = 0; i < myReasonStatisticsTypeData.length; i++) {
                    if (myReasonStatisticsTypeData[i]["ReasonStatisticsTypeId"] == m_ReasonStatisticsTypeId) {
                        return myReasonStatisticsTypeData[i]["Name"];
                    }
                }
                return "";
            },
            editor: {
                type: 'combobox',
                options: {
                    valueField: 'ReasonStatisticsTypeId',
                    textField: 'Name',
                    data: myReasonStatisticsTypeData,
                    panelHeight: 'auto'
                }
            }
        }, {
            width: 100,
            title: '备注',
            field: 'Remarks',
            editor: 'text'
        }, {
            width: 100,
            title: '是否可用',
            field: 'Enabled',
            editor: 'text'
        }]]
    });
}

//获得停机类型数据
function LoadReasonStatisticsTypeData() {
    var queryUrl = 'Edit.aspx/GetReasonStatisticsTypeData';
    var dataToSend = '';
    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                InitializeTreeGrid(m_MsgData.rows);
                initPageAuthority();
                loadMachineHaltReasons();
            }
        }
    });
}
$(document).ready(function () {
    LoadReasonStatisticsTypeData();
    ////InitializeTreeGrid(m_MsgData.rows);
    //InitializeTreeGrid("");
    //initPageAuthority();
    //loadMachineHaltReasons();
});
