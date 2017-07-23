var MasterMachineOpType;               //主机设备操作类型,1添加,2修改
var SlaveMachineOpType;                //从机设备操作类型,1添加,2修改
var MasterOrganizationId;              //主机组织机构代码,用于接收Web控件传递来的信息
var MasterDataBaseName;                //主机变量数据库名称
var MasterTableName;                   //主机变量表名
var SlaveOrganizationId;               //从机组织机构代码,用于接收Web控件传递来的信息
var SlaveDataBaseName;                 //从机变量数据库名称
var SlaveTableName;                    //从机变量表名
var CurrentMachineEditFoucs            //当前编辑的是主机还是从机1表示主机，2表示从机
var PageOpPermission;//页面操作权限控制
$(function () {
    LoadDcsOrganizationData("first");
    LoadMainDataGrid("first");
    initPageAuthority();
    LoadSelectDcsTagsDialog();
    InitializingMasterMachineVariableCommbox({ "rows": [], "total": 0 });
    //LoadMasterMachineVariables();
});
function onOrganisationTreeClick(node) {
    $('#organizationName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
    mOrganizationId = node.OrganizationId;
}
//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "LowLoadOperation.aspx/AuthorityControl",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            PageOpPermission = msg.d;
            //增加
            if (PageOpPermission[1] == '0') {
                $("#id_add").linkbutton('disable');
            }
            //修改
            //if (authArray[2] == '0') {
            //    $("#edit").linkbutton('disable');
            //}
            //删除
            if (PageOpPermission[3] == '0') {
                $("#id_deleteAll").linkbutton('disable');
            }
        }
    });
}
function LoadMainDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_Main').datagrid({
            columns: [[
                  { field: 'ItemId', title: '标识列', width: 100, hidden: true },
                  //{ field: 'Name', title: '考核分组', width: 100 },
                  //{ field: 'CycleType', title: '考核周期', width: 60 },
                  { field: 'VariableDescription', title: '名称', width: 100, align: 'left' },
                  { field: 'RunTag', title: '运行标签', width: 90, align: 'left' },
                  { field: 'DelayTime', title: '延迟时间', width: 60, align: 'left' },
                  { field: 'LoadTag', title: '负载标签', width: 90, align: 'left' },
                  { field: 'DCSLoadTag', title: 'DCS负载标签', width: 90, align: 'left' },
                  { field: 'LoadTagType', title: '负载类型', width: 60, align: 'left' },
                  { field: 'LLoadLimit', title: '负载下限', width: 60, align: 'left' },
                  { field: 'Record', title: '停机信息', width: 60, align: 'left' },
                  { field: 'ValidValues', title: '有效值', width: 60, align: 'left' },
                  { field: 'Editor', title: '编辑人', width: 65, align: 'left' },
                  { field: 'EditTime', title: '编辑时间', width: 100, align: 'left' },
                  { field: 'Remark', title: '备注', width: 100, align: 'left' },
                  {
                      field: 'edit', title: '编辑', width: 100, formatter: function (value, row, index) {
                          var str = "";
                          str = '<a href="#" onclick="editFun(true,\'' + row.ID + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png" title="编辑页面" onclick="editFun(true,\'' + row.ID + '\')"/>编辑</a>';
                          str = str + '<a href="#" onclick="deleteFun(\'' + row.ID + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面"  onclick="deleteFun(\'' + row.ID + '\')"/>删除</a>';
                          //str = str + '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_deleteFun.png" title="删除页面" onclick="deleteFunPageFun(\'' + row.id + '\');"/>删除';
                          return str;
                      }
                  }
            ]],
            fit: true,
            toolbar: "#toorBar",
            idField: 'ID',
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        });
    }
    else {
        $('#grid_Main').datagrid('loadData', myData);
    }
}
////////////////////////////////////////////////初始化Combotree///////////////////////////////////////////
function LoadDcsOrganizationData(type) {
    $.ajax({
        type: "POST",
        url: "LowLoadOperation.aspx/GetDcsOrganization",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (type == 'first') {
                InitializeComboTree(m_MsgData);
            }
            else if (type == 'last') {
                $('#Combobox_DCSF').combotree('loadData', m_MsgData);
            }
        }
    });
}
function LoadSelectDcsTagsDialog() {
    $('#dlg_SelectDcsTags').dialog({
        title: '选择DCS标签',
        width: 750,
        height: 460,
        closed: true,
        cache: false,
        modal: true
    });
}
//////////////////////////////////初始化基础数据//////////////////////////////////////////
//function InitializeComboTree(myData) {
//    $('#Combobox_DCSF').combotree({
//        data: myData,
//        dataType: "json",
//        valueField: 'id',
//        textField: 'text',
//        required: false,
//        panelHeight: '500',
//        editable: false
//    });
//}
//////////////获得Tag信息，调用Web控件时必须定义该方法才能有效的传递双击事件
var s_s_MasterOrganizationId;
var s_MasterDataBaseName = '';
var s_MasterTableName = '';
function GetTagInfo(myRowData, myDcsDataBaseName, myDcsOrganizationId) {
    //alert(myRowData.VariableName + "," + myRowData.VariableDescription + "," + myRowData.TableName + "," + myRowData.FieldName);
    if (CurrentMachineEditFoucs == 1) {
        $('#TextBox_MasterVariableName').textbox('setText', myRowData.VariableName);
        $('#TextBox_MasterVariableDescription').textbox('setText', myRowData.VariableDescription);
        MasterOrganizationId = myDcsOrganizationId;
        MasterDataBaseName = myDcsDataBaseName;
        MasterTableName = myRowData.TableName;
    }
    if (CurrentMachineEditFoucs==3) {
        $('#s_TextBox_MasterVariableName').textbox('setText', myRowData.VariableName);
        $('#s_TextBox_MasterVariableDescription').textbox('setText', myRowData.VariableDescription);
        s_MasterOrganizationId = myDcsOrganizationId;
        s_MasterDataBaseName = myDcsDataBaseName;
        s_MasterTableName = myRowData.TableName;
    }
    else if (CurrentMachineEditFoucs == 2) {
        $('#TextBox_SlaveVariableName').textbox('setText', myRowData.VariableName);
        $('#TextBox_SlaveVariableDescription').textbox('setText', myRowData.VariableDescription);
        SlaveOrganizationId = myDcsOrganizationId;
        SlaveDataBaseName = myDcsDataBaseName;
        SlaveTableName = myRowData.TableName;
    }
}
function GetDcsTagsFun(myCurrentMachineEditFoucs) {
    CurrentMachineEditFoucs = myCurrentMachineEditFoucs;
    $('#dlg_SelectDcsTags').dialog('open');
}
//初始化主机设备标签列表
//function LoadMasterMachineVariables() {
//    var m_OrganizationId = $('#Combobox_DCSF').combotree("getValue");
//    if (m_OrganizationId != undefined && m_OrganizationId != null & m_OrganizationId != "") {
//        $.ajax({
//            type: "POST",
//            url: "LowLoadOperation.aspx/GetMasterMachineEquipment",
//            data: "{myOrganizationId:'" + m_OrganizationId + "'}",
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            success: function (msg) {
//                var m_Data = jQuery.parseJSON(msg.d)
//                if (m_Data != null && m_Data != undefined) {
//                    //InitializingMasterMachineVariableCommbox(m_Data);
//                    $('#Commbox_EquipmentId').combotree('loadData', m_Data);
//                }
//            }
//        });
//    }

//    //$('#Commbox_EquipmentId');
//}
function InitializingMasterMachineVariableCommbox(myData) {
    $('#Commbox_EquipmentId').combotree({
        data: myData,
        dataType: "json",
        valueField: 'id',
        textField: 'text',
        required: false,
        panelHeight: 200,   //'auto',
        editable: false,
        onLoadSuccess: function () {
            $("#Commbox_EquipmentId").combotree('tree').tree("collapseAll");
        },
        onSelect: function (node) {
            //返回树对象  
            var tree = $(this).tree;
            //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
            var isLeaf = tree('isLeaf', node.target);
            if (!isLeaf) {
                //清除选中  
                $('#Commbox_EquipmentId').combotree('clear');
                alert("必须选择到具体设备!");
            }
        }
    });

}
/////////////////////////////查询主设备/////////////////////////
//function QueryMasterMachineInfoFun() {
//    var m_SelectDcs = $('#Combobox_DCSF').combotree("getValue");
//    if (m_SelectDcs != "" && m_SelectDcs != null && m_SelectDcs != undefined) {
//        LoadMaterMachineData('last', m_SelectDcs);
//        LoadMasterMachineVariables();
//    }
//    else {
//        alert('请选择有效的DCS!');
//    }
//}
//function RefreshMasterMachineFun() {
//    QueryMasterMachineInfoFun();
//}
function Query() {
    var variableDesc = $('#variable').textbox('getText');
    $.ajax({
        type: "POST",
        url: "LowLoadOperation.aspx/GetLowLoadOperation",
        data: "{mOrganizationId:'" + mOrganizationId + "',variableDesc:'" + variableDesc + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData.total == 0) {
                LoadMainDataGrid("last", []);
                $.messager.alert('提示', '没有查询到记录！');
            } else {
                LoadMainDataGrid("last", myData);
            }
        },
        error: function () {
            $.messager.progress('close');
            $("#grid_Main").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    });
}
function refresh() {
    Query();
}
function addFun() {
    editFun(false);
}
var mType = '';
var m_Id = '';
function editFun(IsEdit, editContrastId) {
    if (IsEdit) {
        if (PageOpPermission[2] == "0") {
            $.messager.alert("提示", "该用户没有编辑权限！");
            return;
        }
        IsAdd = false;
        $('#grid_Main').datagrid('selectRecord', editContrastId);
        var data = $('#grid_Main').datagrid('getSelected');
        $('#Checkbox_MasterRecord').attr('checked', true);
        $('#variableDecs').textbox('setText', data.VariableDescription);
        $('#Text1').textbox('setText', data.DelayTime);
        if (data.LoadTagType == 'current') {
            mType = '电流';
        }
        if (data.LoadTagType == 'power') {
            mType = '功率';
        }
        $('#Text2').combobox('setText', mType);
        $('#Text5').textbox('setText', data.LoadTag);
        $('#TextBox_MasterVariableName').textbox('setText', data.RunTag);
        $('#s_TextBox_MasterVariableName').textbox('setText', data.DCSLoadTag);
        $('#Text7').numberbox('setText', data.LLoadLimit);
        $('#TextBox_MasterRemark').attr('value', data.Remark);
        MasterDataBaseName = data.RunTagDataBaseName;
        MasterTableName = data.RunTagTableName;
        s_MasterDataBaseName = data.DCSLoadDataBaseName;
        s_MasterTableName = data.DCSLoadTableName;
        m_Id = data.ID;
    }
    else {
        IsAdd = true;
        $('#Checkbox_MasterRecord').attr('checked', true);
        $('#variableDecs').textbox('clear');
        $('#Text1').textbox('clear');
        $('#Text2').combobox('clear');
        $('#Text5').textbox('clear');
        $('#s_TextBox_MasterVariableName').textbox('clear');
        $('#TextBox_MasterVariableName').textbox('clear');
        $('#s_TextBox_MasterVariableDescription').textbox('clear');
        $('#TextBox_MasterVariableDescription').textbox('clear');       
        $('#Text7').numberbox('clear');
        $('#TextBox_MasterRemark').attr('value', '');
        if (mOrganizationId == "" && mOrganizationId == undefined) {
            $.messager.alert('提示', '请选择组织机构！');
        }
    }
    $('#AddandEditor').window('open');
}
function save() {
    var m_VariableDesc = $('#variableDecs').textbox('getText');
    var m_RunTag = $('#TextBox_MasterVariableName').textbox('getText');
    var m_Record = $('#Checkbox_MasterRecord').prop('checked');
    var m_ValidValues = $("input[name='SelectRadio_MasterValidValues']:checked").val();
    var m_DelayTime = $('#Text1').numberbox('getText');
    var m_LoadTag = $('#Text5').textbox('getText');
    var m_DCSLoadTag = $('#s_TextBox_MasterVariableName').textbox('getText');
    var m_LoadTagType = $('#Text2').combobox('getValue');
    var m_LLoadLimit = $('#Text7').numberbox('getText');
    var m_Remark = $('#TextBox_MasterRemark').val();
    if (m_LoadTag == '' && m_DCSLoadTag=='') {
        $.messager.alert('提示', '负载标签与DCS标签至少填一项!');
        rerurn;
    }
    if (m_VariableDesc == "") {
        $.messager.alert('提示', '请填写必填项!');
        return;
    }
    else {
        var mUrl = "";
        var mdata = "";
        if (IsAdd) {
            mUrl = "LowLoadOperation.aspx/AddLowLoadOperation";
            mdata = "{mOrganizationId:'" + mOrganizationId + "',m_VariableDesc:'" + m_VariableDesc + "',m_RunTag:'" + m_RunTag + "',MasterTableName:'" + MasterTableName + "',MasterDataBaseName:'"
                + MasterDataBaseName + "',m_Record:'" + m_Record + "',m_ValidValues:'" + m_ValidValues + "',m_DelayTime:'" + m_DelayTime + "',m_LoadTag:'" + m_LoadTag + "',m_DCSLoadTag:'"
                + m_DCSLoadTag + "',s_MasterTableName:'" + s_MasterTableName + "',s_MasterDataBaseName:'" + s_MasterDataBaseName + "',m_LoadTagType:'" + m_LoadTagType + "',m_LLoadLimit:'" + m_LLoadLimit + "',m_Remark:'" + m_Remark + "'}";
        } else if (IsAdd == false) {
            mUrl = "LowLoadOperation.aspx/EditLowLoadOperation";
            mdata = "{m_Id:'" + m_Id + "',mOrganizationId:'" + mOrganizationId + "',m_VariableDesc:'" + m_VariableDesc + "',m_RunTag:'" + m_RunTag + "',MasterTableName:'" + MasterTableName + "',MasterDataBaseName:'"
                + MasterDataBaseName + "',m_Record:'" + m_Record + "',m_ValidValues:'" + m_ValidValues + "',m_DelayTime:'" + m_DelayTime + "',m_LoadTag:'" + m_LoadTag + "',m_DCSLoadTag:'"
                + m_DCSLoadTag + "',s_MasterTableName:'" + s_MasterTableName + "',s_MasterDataBaseName:'" + s_MasterDataBaseName + "',m_LoadTagType:'" + m_LoadTagType + "',m_LLoadLimit:'" + m_LLoadLimit + "',m_Remark:'" + m_Remark + "'}";
        }
        $.ajax({
            type: "POST",
            url: mUrl,
            data: mdata,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var myData = msg.d;
                if (myData == 1) {
                    $.messager.alert('提示', '操作成功！');
                    $('#AddandEditor').window('close');
                    refresh();
                }
                else {
                    $.messager.alert('提示', '操作失败！');
                    refresh();
                }
            },
            error: function () {
                $.messager.alert('提示', '操作失败！');
                refresh();
            }
        });
    }
}
function deleteFun(deleteFunContrastId) {
    if (PageOpPermission[3] == "0") {
        $.messager.alert("提示", "该用户没有删除权限！");
        return;
    }
    $.messager.confirm('提示', '确定要删除吗？', function (r) {
        if (r) {
            $('#grid_Main').datagrid('selectRecord', deleteFunContrastId);
            var data = $('#grid_Main').datagrid('getSelected');
            m_Id = data.ID;
            $.ajax({
                type: "POST",
                url: "LowLoadOperation.aspx/DeleteLowLoadOperation",
                data: "{m_Id:'" + m_Id + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var myData = msg.d;
                    if (myData == 1) {
                        $.messager.alert('提示', '删除成功！');
                        $('#AddandEditor').window('close');
                        refresh();
                    }
                    else {
                        $.messager.alert('提示', '操作失败！');
                        refresh();
                    }
                },
                error: function () {
                    $.messager.alert('提示', '操作失败！');
                    $('#AddandEditor').window('close');
                    refresh();
                }
            });
        }
    })
}