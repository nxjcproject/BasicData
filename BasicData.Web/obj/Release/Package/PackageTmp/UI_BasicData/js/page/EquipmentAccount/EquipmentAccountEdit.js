var m_VariableId = '';
var m_OrganizationID = '';
var m_EquipmentName = '';
var m_MonitorType = 0;
var m_PowerSupply = '';
var m_VoltageGrade = '';
var m_RatedCT = '';
var m_AmmeterCode = '';
var m_ActualCT = '';
var m_Power = 0;
var m_Unit = '';
var m_PowerSupplyPosition = '';
var m_Remarks = '';


$(function () {
    InitializeGrid();
    initPageAuthority();
});
//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "EquipmentAccountEdit.aspx/AuthorityControl",
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
                $("#saveEditDlg").linkbutton('disable');
                var itemEl = $('#mm_edit')[0];
                $("#RightMenu").menu('disableItem', itemEl);
            }
            //删除
            if (authArray[3] == '0') {
                $("#delete").linkbutton('disable');
                var itemEl = $('#mm_delete')[0];
                $("#RightMenu").menu('disableItem', itemEl);
            }
        }
    });
}

function loadGridData(myLoadType) {
    var m_OrganizationId = $("#organizationId").val();
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "EquipmentAccountEdit.aspx/GetEquipmentsInfo",
        data: "{organizationId:'" + m_OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            if (myLoadType == 'first') {
                m_MsgData = jQuery.parseJSON(msg.d);
                InitializeGrid(m_MsgData);
            }
            else if (myLoadType == 'last') {
                m_MsgData = jQuery.parseJSON(msg.d);
                $('#equipmentAccount_Info').datagrid('loadData', m_MsgData['rows']);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function () {
            $.messager.progress('close');
            handleError
        }
    });
}

function InitializeGrid(myData) {
    $('#equipmentAccount_Info').datagrid({
        data:myData,
        columns: [[
            { field: 'EquipmentName', title: '设备名称', width: 110 },
            { field: 'VariableId', title: '变量ID', width: 200 },            
            //{ field: 'OrganizationID', title: '组织机构ID', width: 200 },
            { field: 'Name', title: '产线名称', width: 70 },
            { field: 'OrganizationID', title: '组织机构ID', width: 70, hidden: true },
            { field: 'MonitorType', title: '监控点类别', width: 80 },
            { field: 'PowerSupply', title: '设备工序电源取自位置', width: 130 },
            { field: 'VoltageGrade', title: '电压等级', width: 60 },
            { field: 'RatedCT', title: '互感器变比', width: 70 },
            { field: 'AmmeterCode', title: '电表编号', width: 100 },
            { field: 'ActualCT', title: '实际变比', width: 60 },
            { field: 'Power', title: '设备功率', width: 60 },            
            { field: 'Unit', title: '单位', width: 40 },
            { field: 'Current', title: '额定电流', width: 60 },
            { field: 'PowerSupplyPosition', title: '计量供电电源安装位置', width: 100 },
            { field: 'Remarks', title: '备注', width: 100 }
        ]],
        rownumbers: true,
        singleSelect: true,

    });
}

function handleError() {
    $('#equipmentAccount_Info').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

function onOrganisationTreeClick(node) {
    $('#productLineName').textbox('setText', node.text); //组织机构的名字
    $('#organizationId').val(node.OrganizationId);//用一个隐藏控件传递organizationId的值OrganizationId
}

function QueryReportFun() {
    var organizationID = $('#organizationId').val();
    if (organizationID == "") {
        $.messager.alert('警告', '请选择生产线');
        return;
    }   
    loadGridData('last');
    // loadGridData('first');
}

function AddEquipment() {
    
    var m_OrganizationId = $("#organizationId").val();
    if(m_OrganizationId==""||m_OrganizationId==undefined)
    {
        $.messager.alert('警告','请先选择生产线！');
        return;
    }
    $('#AddDialog').dialog('open');
    //清空数据
    m_VariableId = $('#VariableId_edit').textbox('setValue', '');//validatebox('getValue');
    m_OrganizationID = $('#OrganizationID_edit').textbox('setValue', '');
    m_EquipmentName = $('#EquipmentName_edit').textbox('setValue', '');
    m_MonitorType = $('#MonitorType_edit').textbox('setValue', '');
    m_PowerSupply = $('#PowerSupply_edit').textbox('setValue', '');
    m_VoltageGrade = $('#VoltageGrade_edit').textbox('setValue', '');
    m_RatedCT = $('#RatedCT_edit').textbox('setValue', '');
    m_AmmeterCode = $('#AmmeterCode_edit').textbox('setValue', '');
    m_ActualCT = $('#ActualCT_edit').textbox('setValue', '');
    m_Power = $('#Power_edit').textbox('setValue', '');
    m_Unit = $('#Unit_edit').textbox('setValue', '');
    m_Current = $('#Current_edit').textbox('setValue', '');
    m_PowerSupplyPosition = $('#PowerSupplyPosition_edit').textbox('setValue', '');
    m_Remarks = $('#Remarks_edit').val('');


    $.ajax({
        type: "POST",
        url: "EquipmentAccountEdit.aspx/GetOrganizationIdInfo",
        data: "{organizationId:'" + m_OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
                m_MsgData = jQuery.parseJSON(msg.d);
                $('#OrganizationID').combobox({
                    data: m_MsgData,
                    valueField: 'OrganizationID',
                    textField: 'Name'
                });
        },
    });

}

function SaveEquipment() {
    //获取参数值
    m_VariableId = $('#VariableId').val();//validatebox('getValue');
    m_OrganizationID = $('#OrganizationID').combo('getValue');
    m_EquipmentName = $('#EquipmentName').textbox('getValue');
    m_MonitorType = $('#MonitorType').textbox('getValue');
    m_PowerSupply = $('#PowerSupply').textbox('getValue');
    m_VoltageGrade = $('#VoltageGrade').textbox('getValue');
    m_RatedCT = $('#RatedCT').textbox('getValue');
    m_AmmeterCode = $('#AmmeterCode').textbox('getValue');
    m_ActualCT = $('#ActualCT').textbox('getValue');
    m_Power = $('#Power').textbox('getValue');
    m_Unit = $('#Unit').textbox('getValue');
    m_Current = $('#Current').textbox('getValue');
    m_PowerSupplyPosition = $('#PowerSupplyPosition').textbox('getValue');
    m_Remarks = $('#Remarks').val();
    //要传入的参数
    if (m_MonitorType == '' || m_MonitorType == undefined)
        m_MonitorType =Number(0);
    if (m_Power == '' || m_Power == undefined)
        m_Power = Number(0);
    if (m_VariableId == "" || m_VariableId == undefined || m_OrganizationID == "" || m_OrganizationID == undefined || m_EquipmentName == "" || m_EquipmentName == undefined) {
        $.messager.alert('提示', '“变量ID”、“组织机构ID”和“设备名称”不允许为空！');
        return;
    }
    if (isNaN(m_MonitorType) || isNaN(m_Power)) {
        $.messager.alert('提示', '“监控点类别”和“设备功率”应输入数值类型！');
        return;
    }

    var m_Datas = "{VariableId:'" + m_VariableId + "',OrganizationID:'" + m_OrganizationID + "',EquipmentName:'" + m_EquipmentName + "',MonitorType:'" + m_MonitorType +
        "',PowerSupply:'" + m_PowerSupply + "',VoltageGrade:'" + m_VoltageGrade + "',RatedCT:'" + m_RatedCT + "',AmmeterCode:'" + m_AmmeterCode + "',ActualCT:'" +
        m_ActualCT + "',Power:'" + m_Power + "',Unit:'" + m_Unit + "',Current:'" + m_Current + "',PowerSupplyPosition:'" + m_PowerSupplyPosition + "',Remarks:'" + m_Remarks + "'}";
    $.ajax({
        type: "POST",
        url: "EquipmentAccountEdit.aspx/SaveEquipmentInfo",
        data: m_Datas,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', msg.d);
            loadGridData("last");
            $('#AddDialog').dialog('close');
        },
        error: function () { $.messager.alert('警告', '保存数据失败！') }
    });
}

//删除设备
function RemoveEquipment() {
    var selectedRow = $('#equipmentAccount_Info').datagrid('getSelected');
    if (selectedRow == undefined)
        $.messager.alert("提示","请先选择一行数据");
    var m_VariableId = selectedRow.VariableId;
    var m_OrganizationID = selectedRow.OrganizationID;
    $.ajax({
        type: "POST",
        url: "EquipmentAccountEdit.aspx/RemoveEquipmentInfo",
        data: "{variableId:'"+m_VariableId+"',organizationId:'"+m_OrganizationID+"'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {       
            $.messager.alert('提示', msg.d);
            loadGridData("last");
        },
        error: function () { $.messager.alert('警告', '删除设备失败！') }
    });
}

//编辑设备信息
function EditEquipment() {
    //编辑前的数据
    var selectedRow = $('#equipmentAccount_Info').datagrid('getSelected');
    if (selectedRow == undefined)
        $.messager.alert("提示", "请先选择一行数据");
    var VariableId_old = selectedRow.VariableId;
    var OrganizationName_old = selectedRow.Name;
    var OrganizationId_old = selectedRow.OrganizationID;
    var EquipmentName_old = selectedRow.EquipmentName;
    var MonitorType_old = selectedRow.MonitorType;
    var PowerSupply_old = selectedRow.PowerSupply;
    var VoltageGrade_old = selectedRow.VoltageGrade;
    var RatedCT_old = selectedRow.RatedCT;
    var AmmeterCode_old = selectedRow.AmmeterCode;
    var ActualCT_old = selectedRow.ActualCT;
    var Power_old = selectedRow.Power;
    var Unit_old = selectedRow.Unit;
    var Current_old = selectedRow.Current;
    var PowerSupplyPosition_old = selectedRow.PowerSupplyPosition;
    var Remarks_old = selectedRow.Remarks;
    //打开修改对话框
    $('#EditDialog').dialog('open');

    m_VariableId = $('#VariableId_edit').textbox('setValue', VariableId_old);//validatebox('getValue');
    //m_OrganizationID = $('#OrganizationID_edit').textbox('setValue',OrganizationID_old);
    m_EquipmentName = $('#EquipmentName_edit').textbox('setValue', EquipmentName_old);
    m_MonitorType = $('#MonitorType_edit').textbox('setValue', MonitorType_old);
    m_PowerSupply = $('#PowerSupply_edit').textbox('setValue', PowerSupply_old);
    m_VoltageGrade = $('#VoltageGrade_edit').textbox('setValue', VoltageGrade_old);
    m_RatedCT = $('#RatedCT_edit').textbox('setValue', RatedCT_old);
    m_AmmeterCode = $('#AmmeterCode_edit').textbox('setValue', AmmeterCode_old);
    m_ActualCT = $('#ActualCT_edit').textbox('setValue', ActualCT_old);
    m_Power = $('#Power_edit').textbox('setValue', Power_old);
    m_Unit = $('#Unit_edit').textbox('setValue', Unit_old);
    m_Current = $('#Current_edit').textbox('setValue', Current_old);
    m_PowerSupplyPosition = $('#PowerSupplyPosition_edit').textbox('setValue', PowerSupplyPosition_old);
    m_Remarks = $('#Remarks_edit').val(Remarks_old);


    var m_OrganizationId = $("#organizationId").val();
    $.ajax({
        type: "POST",
        url: "EquipmentAccountEdit.aspx/GetOrganizationIdInfo",
        data: "{organizationId:'" + m_OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            $('#OrganizationID_edit').combobox({
                data: m_MsgData,
                valueField: 'OrganizationID',
                textField: 'Name',
            });
            $('#OrganizationID_edit').combobox('setText', OrganizationName_old);
            $('#OrganizationID_edit').combobox('setValue', OrganizationId_old);
        },
    });
    
}


function SaveEditEquipment() {

    var selectedRow = $('#equipmentAccount_Info').datagrid('getSelected');
    var VariableId_old = selectedRow.VariableId;
    var OrganizationID_old = selectedRow.OrganizationID;
    //获取参数值
    m_VariableId = $('#VariableId_edit').val();
    m_OrganizationID = $('#OrganizationID_edit').combobox('getValue');
    m_EquipmentName = $('#EquipmentName_edit').textbox('getValue');
    m_MonitorType = $('#MonitorType_edit').textbox('getValue');
    m_PowerSupply = $('#PowerSupply_edit').textbox('getValue');
    m_VoltageGrade = $('#VoltageGrade_edit').textbox('getValue');
    m_RatedCT = $('#RatedCT_edit').textbox('getValue');
    m_AmmeterCode = $('#AmmeterCode_edit').textbox('getValue');
    m_ActualCT = $('#ActualCT_edit').textbox('getValue');
    m_Power = $('#Power_edit').textbox('getValue');
    m_Unit = $('#Unit_edit').textbox('getValue');
    m_Current = $('#Current_edit').textbox('getValue');
    m_PowerSupplyPosition = $('#PowerSupplyPosition_edit').textbox('getValue');
    m_Remarks = $('#Remarks_edit').val();


    //要传入的参数
    if (m_MonitorType == '' || m_MonitorType == undefined)
        m_MonitorType = Number(0);
    if (m_Power == '' || m_Power == undefined)
        m_Power = Number(0);
    if (m_VariableId == "" || m_VariableId == undefined || m_OrganizationID == "" || m_OrganizationID == undefined || m_EquipmentName == "" || m_EquipmentName == undefined) {
        $.messager.alert('提示', '“变量ID”、“组织机构ID”和“设备名称”不允许为空！');
        return;
    }
    if (isNaN(m_MonitorType) || isNaN(m_Power)) {
        $.messager.alert('提示', '“监控点类别”和“设备功率”应输入数值类型！');
        return;
    }

    var m_Datas = "{variableId_old:'" + VariableId_old + "',organizationId_old:'" + OrganizationID_old + "',VariableId:'" + m_VariableId + "',OrganizationID:'" + m_OrganizationID + "',EquipmentName:'" + m_EquipmentName + "',MonitorType:'" + m_MonitorType +
        "',PowerSupply:'" + m_PowerSupply + "',VoltageGrade:'" + m_VoltageGrade + "',RatedCT:'" + m_RatedCT + "',AmmeterCode:'" + m_AmmeterCode + "',ActualCT:'" +
        m_ActualCT + "',Power:'" + m_Power + "',Unit:'" + m_Unit + "',Current:'" + m_Current + "',PowerSupplyPosition:'" + m_PowerSupplyPosition + "',Remarks:'" + m_Remarks + "'}";
    $.ajax({
        type: "POST",
        url: "EquipmentAccountEdit.aspx/EditEquipmentInfo",
        data: m_Datas,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', msg.d);
            loadGridData("last");
            $('#EditDialog').dialog('close');
        },
        error: function () { $.messager.alert('警告', '保存数据失败！') }
    });
}


function onRowContextMenu(e, rowIndex, rowData) {
    e.preventDefault();
    $(this).datagrid('selectRow', rowIndex);
    $('#RightMenu').menu('show', {
        left: e.pageX,
        top: e.pageY
    });
}