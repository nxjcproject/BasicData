//全局变量
var m_StandardId = "";
var m_InOutSide = "";

$(document).ready(function () {
    //initPageAuthority();
    LoadStandardType();
    LoadIndexDatagrid("first")

    //企业内执行，非企业内执行//
    $("#cbox_Insideoutside").combobox({
        textField: 'type',
        valueField: 'value',
        data: [{
            type: '企业内执行',
            value: 'Inside'
        }, {
            type: '非企业内执行',
            value: 'Outside'
        }],
        onSelect: function (para) {
            m_InOutSide = para.value;
        }
    });
});
//初始化页面的增删改查权限
function initPageAuthority() {
    $.ajax({
        type: "POST",
        url: "KPIndexProduction.aspx/AuthorityControl",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            var authArray = msg.d;
            //增加
            if (authArray[1] == '0') {
                $("#add_list").linkbutton('disable');
                $("#add_detail").linkbutton('disable');
            }
            //修改
            //if (authArray[2] == '0') {
            //    $("#edit").linkbutton('disable');
            //}
            //删除
            if (authArray[3] == '0') {
                $("#delete_list").linkbutton('disable');
                $("#delete_detail").linkbutton('disable');
            }
        }
    });
}
function LoadStandardType() {
    $.ajax({
        type: "POST",
        url: "KPIndexEnergy.aspx/GetStandardType",
        //data: "{myOrganizationId:'" + m_OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            //InitializeEnergyConsumptionGrid(m_GridCommonName, m_MsgData);
            $('#cbox_Standard').combobox({
                data: m_MsgData.rows,
                valueField: 'id',
                textField: 'StandardName',
                onSelect: function (param) {
                    m_StandardId = param.value;                 
                }
            });
            //添加中的combobox
            $('#txtStandardLevel').combobox({
                data: m_MsgData.rows,
                valueField: 'value',
                textField: 'StandardName'
            });
        },
        error: function () {
            $.messager.alert('提示', '标准层次加载失败！');
        }
    });
}
function LoadIndexDatagrid(type,mdata) {
    if (type == "first") {
        $("#dgrid_Index").datagrid({
            data: [],
            rownumbers: true,
            toolbar: "#toolbar_KPIndexInfo",
            singleSelect: true,
            fit: true,       
            columns: [[
                { field: 'StandardItemId', title: '组织机构名称', width: 140, hidden: true },
                { field: 'OrganizationName', title: '组织机构名称', width: 140 },
                { field: 'Name', title: '项目名称', width: 120 },
                {
                    field: 'LevelType', title: '类型', width: 100,
                    formatter: function (value, row, index) {
                        if (value == "ProductionLine") {
                            return "生产线";
                        } else if (value == "Process") { return "工序"; } else {
                            return "错误！";
                        }
                    }
                },
                {
                    field: 'VariableName', title: '变量', width: 100
                },
                {field: 'ValueType', title: '值类型', width: 150},
                { field: 'Unit', title: '单位', width: 80 },
                { field: 'StandardValue', title: '标准值', width: 80 },
                { field: 'StandardLevel', title: '标准层次', width: 80, align: 'right' },
                { field: 'Creator', title: '创建人', width: 80, align: 'right' },
                { field: 'CreateTime', title: '创建时间', width: 140, align: 'right' }
            ]]
        });
    } else if (type == "second") {
        $("#dgrid_Index").datagrid('loadData', mdata);
    }
}
//var m_StandardId = "";
//var m_InOutSide = "";
function LoadIndex_Clinck() {
    var mStandardId=m_StandardId;
    var mInOutSide = m_InOutSide;
    if (mStandardId == "" || mInOutSide == "") {
        $.messager.alert('提示', '请选择未选项！');
    }
    $.ajax({
        type: "POST",
        url: "KPIndexProduction.aspx/GetIndexData",
        data: "{mStandardId:'" + mStandardId+ "',myInOutSide:'" + mInOutSide + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData.total == 0)
            { $.messager.alert('提示','未查询到数据！','info');}
            LoadIndexDatagrid("second", m_MsgData);
        }
    })
}
function RefreshRecordDataFun()
{ LoadIndex_Clinck(); }


//添加
var m_OrganizationId = "";
var m_VariableId = "";
var m_IndicatorId ="";
var m_Unit ="";
function CreateKPIndex()
{  //打开
    $("#dlgKPIndexProductionEditor").dialog('open');
   //初始化
    $("input[name=adminFlag]").get(0).checked = false;
    $("input[name=adminFlag]").get(1).checked = false;
    $("#ProductionOrganization").combotree({ disabled: true });
    //加载combobox选项
    LoadKPIProductionOrganizationData("first");
    LoadEquipmentCommonName();
    LoadValueType();
}
function radioYes() {
    $("#ProductionOrganization").combotree({ disabled: false });
}
function radioNo() {
    $("#ProductionOrganization").combotree({ disabled: true });
    m_OrganizationId = "";
}
function LoadKPIProductionOrganizationData(myLoadType) {
    $.ajax({
        type: "POST",
        url: "KPIndexProduction.aspx/GetKPIProductionOrganizationData",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == 'first') {
                InitializeComboTree(m_MsgData);
            }
            else if (myLoadType == 'last') {
                $('#ProductionOrganization').combotree('loadData', m_MsgData);
                //$('#ComboTree_SlaveDcs').combotree('loadData', m_MsgData);
                //$('#Combobox_DCSF').combotree('loadData', m_MsgData);
            }
        }
    });
}
function InitializeComboTree(myData) {
    $('#ProductionOrganization').combotree({
        data: myData,
        dataType: "json",
        valueField: 'id',
        textField: 'text',
        required: false,
        panelHeight: 'auto',
        editable: false,
        onSelect: function (para) {
            m_OrganizationId = para.id;       
        }
    });
}
//txtEquipmentCommonName
function LoadEquipmentCommonName()
{
    $.ajax({
        type: "POST",
        url: "KPIndexProduction.aspx/GetEquipmentCommonName",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $("#txtEquipmentCommonName").combobox({
                textField: 'Name',
                valueField: 'VariableId',
                data: m_MsgData.rows,
                onSelect: function (para) {
                    m_VariableId = para.VariableId;
                }
            });
        }
    });
}
function LoadValueType()
{
    $.ajax({
        type: "POST",
        url: "KPIndexProduction.aspx/GetValueTypeList",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $("#txtValueType").combobox({
                textField: 'IndicatorId',
                valueField: 'Unit',
                data: m_MsgData.rows,
                onSelect: function (para) {
                    m_IndicatorId = para.IndicatorId;
                    m_Unit = para.Unit;                  
                    $("#txtUnit").textbox('setValue', m_Unit);
                }
            });
        }
    });
}
function KPIndexDetailSave()
{
    var txt_ObjName = $('#txtObjName').val();   //项目名称
    var txt_StandardLevelText = $("#txtStandardLevel").combobox('getText');
    var txt_StandardLevel = $("#txtStandardLevel").combobox('getValue');//标准层次

    var txt_OrganizationId = m_OrganizationId;

    var txt_Name = $("#txtEquipmentCommonName").combobox('getText'); //设备
    var txt_VariableId = m_VariableId;

    var txt_LevelType = $("#txtLevelType").combobox('getValue');//类型
    var txt_ValueType = $('#txtValueType').combobox('getText');//值类型
    var txt_Unit = m_Unit;//单位

    var txt_StandardValue = $("#txtStandardValue").val();//标准值
    var txt_Level = $("#txtLevel").combobox('getText'); //标准层次
    var txt_Creator = $("#txtCreator").val();//创建人
    if (txt_ObjName == "" || txt_StandardLevelText == "" || txt_Name == "" || txt_ValueType == "" || txt_StandardValue == "" || txt_Creator == "") {
        $.messager.alert('提示', '请填入空值！');
    } else {
        var queryData = "{mStandardLevel:'" + txt_StandardLevel + "',mObjName:'" + txt_ObjName + "',mOrganizationId:'" + txt_OrganizationId + "',mLevelType:'" + txt_LevelType + "',mVariableId:'" + txt_VariableId + "',mValueType:'" + txt_ValueType + "',mUnit:'" + txt_Unit + "',mStandardValue:'" + txt_StandardValue + "',mLevel:'" + txt_Level + "',mCreator:'" + txt_Creator + "'}";
        $.ajax({
            type: "POST",
            url: "KPIndexProduction.aspx/AddKPIndexProduction",
            data: queryData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                $("#dlgKPIndexProductionEditor").dialog('close');
                $.messager.alert('提示', '指标添加成功！');
                RefreshRecordDataFun();
            },
            error: function () {
                $.messager.alert('提示', '指标添加失败！');
            }
        });
    }
}
//删除操作
function DeleteKPIndex() {
    var row = $("#dgrid_Index").datagrid('getSelected');
    if (row == undefined || row == null) {
        $.messager.alert('提示', '请选择需要删除的行。', 'info');
        return;
    }

    $.messager.confirm('提示', '确认删除：' + row.Name + '？', function (r) {
        if (r) {
            KPIndexDelete(row.StandardItemId);
        }
    });
}
function KPIndexDelete(StandardItemId) {
    var queryUrl = 'KPIndexEnergy.aspx/DeleteKPIndexData';
    var dataToSend = '{standardItemId: "' + StandardItemId + '"}';
    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            // 删除成功后刷新
            RefreshRecordDataFun();
            $.messager.alert('提示', '删除成功！', 'info');
        }, error: function () {
            $.messager.alert('提示', '删除失败！', 'info');
        }
    });
}