var m_KeyId="";
var m_Consumpation = "";
var m_StandardType = "";
var m_InOutSide = "";
$(document).ready(function () {
    //initPageAuthority();
    LoadIndexDatagrid("first")
    LoadStandardType();


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
        url: "KpiList.aspx/AuthorityControl",
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
function LoadIndexDatagrid(type) {
    if (type == "first") {
        $("#dgrid_Index").datagrid({
            data: [],
            rownumbers: true,
            toolbar: "#toolbar_KPIndexEnergyInfo",
            singleSelect:true,
            fit:true,
            columns: [[
                { field: 'StandardItemId', title: '组织机构名称', width: 140, hidden: true },
                { field: 'OrganizationName', title: '组织机构名称', width: 140 },
                { field: 'Name', title: '项目名称', width: 120 },
                {field: 'LevelType', title: '类型', width: 100,
                    formatter: function (value, row, index) {
                        if (value == "ProductionLine") {
                            return "生产线";
                        } else if (value == "Process") { return "工序";} else {
                            return "错误！";
                        }
                    } },
                {
                    field: 'VariableName', title: '变量', width: 100               
                },
                {
                    field: 'ValueType', title: '值类型', width: 60,
                    formatter: function (value, row, index) {
                        if (value == "EnergyConsumption") {
                            return "能耗";
                        } else if (value == "ElectricityConsumption") {
                            return "电耗";
                        } else if (value == "CoalConsumption") {
                            return "煤耗";
                        } else { return "错误！"; }
                    }
                },
                { field: 'Unit', title: '单位', width: 80 },
                { field: 'StandardValue', title: '标准值', width: 80 },
                { field: 'StandardLevel', title: '标准层次', width: 80,align: 'right' },
                { field: 'Cteator', title: '创建人', width: 80, align: 'right' },
                { field: 'CreateTime', title: '创建时间', width: 140, align: 'right' }
            ]]
        });
    } else if (type == "second") {
        //$("#dgrid_Index").datagrid([data:]);
    }
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
                    var m_StandardId = param.value;
                    //$.messager.alert('', m_StandardId);
                    LoadConsumpation(m_StandardId);
                }
            });           
            $('#txtStandardLevel').combobox({
                data: m_MsgData.rows,
                valueField: 'value',
                textField: 'StandardName',
                onSelect: function (param) {
                    var m_StandardLevelId = param.value;
                }
            });
        },
        error: function () {
            $.messager.alert('提示', '标准类别加载失败！');
        }
    });
}
function LoadConsumpation(StandardId)
{
    $.ajax({
        type: "POST",
        url: "KPIndexEnergy.aspx/GetConsumpationType",
        data: "{myStandardId:'" + StandardId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            //InitializeEnergyConsumptionGrid(m_GridCommonName, m_MsgData);
            $('#cbox_Consumpation').combobox({
                data: m_MsgData.rows,
                valueField: 'id',
                textField: 'Consumpation',  
                onSelect: function (param) {
                    m_KeyId = param.KeyId;
                    m_Consumpation = param.Consumpation;
                }
            });
        },
        error: function () {
            $.messager.alert('提示', '能耗类别加载失败！');
        }
    });
}
function LoadIndex_Clinck() {
    var mConsumpation = m_Consumpation;
    var mKeyId = m_KeyId;
    var mStandardType = "Energy";
    var mInOutSide = m_InOutSide;
    if (mConsumpation == "" || mKeyId == "" || mStandardType == "" || mInOutSide == "") {
        $.messager.alert('提示', '请选择未选项！');
    }
    $.ajax({
        type: "POST",
        url: "KPIndexEnergy.aspx/GetIndexData",
        data: "{myConsumpation:'" + mConsumpation + "',myKeyId:'"+mKeyId+"',myStandardType:'"+mStandardType+"',myInOutSide:'"+mInOutSide+"'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $("#dgrid_Index").datagrid('loadData',m_MsgData);
            //$.messager.alert('提示','加载成功');
        }
    })
}
function RefreshRecordDataFun()
{ LoadIndex_Clinck();}
/////添加/////////
var m_OrganizationId = "";
var m_VariableId = "";


function CreateKPIndexEnergy() {       
    $("#dlgKPIndexEnergyDetailEditor").dialog('open');
    //$("input[name=adminFlag]").change(function () {
    //    var m_radValue = $("input[name=adminFlag]").value;
    //    if (m_radValue == "0") {
    //        $("#KPIndexEnergyOrganization").combobox({ disabled: true });
    //    } else if (m_radValue == "1") { $("#KPIndexEnergyOrganization").combobox({ disabled: false }); }
    //});
    //初始化//
    $("input[name=adminFlag]").get(0).checked = false;
    $("input[name=adminFlag]").get(1).checked = false;
    $("#KPIndexEnergyOrganization").combotree({ disabled: true });
    LoadKPIndexEnergyOrganizationData("first");
}
function radioYes() {
    $("#KPIndexEnergyOrganization").combotree({ disabled: false });
}
function radioNo() {
    $("#KPIndexEnergyOrganization").combotree({ disabled: true });
    m_OrganizationId = "";
    $.ajax({
        type: "POST",
        url: "KPIndexEnergy.aspx/GetObjectNameList",
        data:"",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $("#txtName").combobox({
                textField: 'Name',
                valueField: 'VariableId',
                data: m_MsgData.rows,
                onSelect: function (para) {                 
                    m_VariableId = para.VariableId;
                }
            });
        }
    })

}
function LoadKPIndexEnergyOrganizationData(myLoadType) {
    $.ajax({
        type: "POST",
        url: "KPIndexEnergy.aspx/GetKPIndexOrganizationData",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == 'first') {
                InitializeComboTree(m_MsgData);
            }
            else if (myLoadType == 'last') {
                $('#KPIndexEnergyOrganization').combotree('loadData', m_MsgData);
                //$('#ComboTree_SlaveDcs').combotree('loadData', m_MsgData);
                //$('#Combobox_DCSF').combotree('loadData', m_MsgData);
            }
        }
    });
}
function InitializeComboTree(myData) {
    $('#KPIndexEnergyOrganization').combotree({
        data: myData,
        dataType: "json",
        valueField: 'id',
        textField: 'text',
        required: false,
        panelHeight: 'auto',
        editable: false,
        onSelect: function (para) {
            m_OrganizationId = para.id;
            LoadNameList(m_OrganizationId);
        }
    });
}
function LoadNameList(OrganizationId) {
    $.ajax({
        type: "POST",
        url: "KPIndexEnergy.aspx/GetNameListData",
        data: "{myOrganizationId:'" + OrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $("#txtName").combobox({
                textField: 'Name',
                valueField: 'id',
                data: m_MsgData.rows,
                onSelect: function (para) {
                   m_VariableId=para.VariableId;
                }
            });
        }
    });
}
function KPIndexEnergyDetailSave() {
    var txt_ObjName = $('#txtObjName').val();   //项目名称
    var txt_StandardLevel = $("#txtStandardLevel").combobox('getValue');//标准层次
    var txt_EnergyType = $("#drpEnerygType").combobox('getValue');//能耗类型
   var txt_OrganizationId = m_OrganizationId;
   var txt_VariableId = m_VariableId;
   var txt_Name=$("#txtName").combobox('getText'); //项目
   var txt_LevelType = $("#drpLevelType").combobox('getValue');//类型
   var txt_ValueType = $('#drpValueType').combobox('getValue');//值类型
   var type = "Energy";
   var txt_Unit = $('#drpUnit').combobox('getText');//单位
   var txt_StandardValue = $("#txtStandardValue").val();//标准值

   var txt_Level = $("#txtLevel").combobox('getText');
   var txt_Creator = $("#txtCreator").val();//创建人
   if (txt_ObjName == "" || txt_StandardLevel == "" || txt_EnergyType == "" || txt_Name == "" || txt_Creator == "") {
       $.messager.alert('提示', '请填入空值！');
   } else {               //将添加的数据写入数据库
       //var queryData="{mStandardLevel:'InternationalStandard',mEnergyType:'Comprehensive',mObjName:'test',mOrganizationId:'',mLevelType:'Process',mVariableId:'cementGrind',mValueType:'ElectricityConsumption',mtype:'Energy',mUnit:'kW·h/t',mStandardValue:'1',mLevel:'1',mCreator:'a'}";
       var queryData = "{mStandardLevel:'" + txt_StandardLevel + "',mEnergyType:'" + txt_EnergyType + "',mObjName:'" + txt_ObjName + "',mOrganizationId:'" + txt_OrganizationId + "',mLevelType:'" + txt_LevelType + "',mVariableId:'" + txt_VariableId + "',mValueType:'" + txt_ValueType + "',mtype:'" + type + "',mUnit:'" + txt_Unit + "',mStandardValue:'" + txt_StandardValue + "',mLevel:'" + txt_Level + "',mCreator:'" + txt_Creator + "'}";  // string mLevel, string mCreator
       $.ajax({
           type: "POST",
           url: "KPIndexEnergy.aspx/AddKPIndexEnergy",
           data: queryData,
           contentType: "application/json; charset=utf-8",
           dataType: "json",
           success: function (msg) {
               $("#dlgKPIndexEnergyDetailEditor").dialog('close');
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
function DeleteKPIndexEnergy()
{
    var row = $("#dgrid_Index").datagrid('getSelected');
    if (row == undefined || row == null) {
        $.messager.alert('提示', '请选择需要删除的行。', 'info');
        return;
    }

    $.messager.confirm('提示', '确认删除：' + row.Name + '？', function (r) {
        if (r) {
            KPIndexEnergyDelete(row.StandardItemId);
        }
    });
}
function KPIndexEnergyDelete(StandardItemId)
{
    var queryUrl = 'KPIndexEnergy.aspx/DeleteKPIndexData';
    var dataToSend = '{standardItemId: "' + StandardItemId + '"}';
    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert('提示', '删除成功。', 'info');

            // 删除成功后刷新
            RefreshRecordDataFun();
        }
    });
}