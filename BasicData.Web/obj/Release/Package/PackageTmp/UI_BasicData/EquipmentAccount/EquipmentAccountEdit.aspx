<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquipmentAccountEdit.aspx.cs" Inherits="BasicData.Web.UI_BasicData.EquipmentAccount.EquipmentAccountEdit" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>主要设备信息录入</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint.js" charset="utf-8"></script>

    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script>

    <script type="text/javascript" src="../js/page/EquipmentAccount/EquipmentAccountEdit.js" charset="utf-8"></script>
    <style type="text/css">
        .easyui-dialog td {
            padding: 5px;
            text-align: right;
        }
    </style>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false" style="padding: 5px;">
        <div data-options="region:'west',border:false,collapsible:false" style="width: 240px;">
            <uc1:OrganisationTree runat="server" ID="OrganisationTree_ProductionLine" />
        </div>
        <div data-options="region:'center',border:false">
            <table id="equipmentAccount_Info" class="easyui-datagrid" data-options="fit:true,border:false,toolbar:equipmentAccount_ToolBar,onDblClickRow:EditEquipment,onRowContextMenu:onRowContextMenu"></table>
        </div>
    </div>

    <div id="equipmentAccount_ToolBar">
        <div style="padding: 10px" id="PanelToolBar">
            <span>机构名称：</span><input id="productLineName" class="easyui-textbox" style="width: 180px;" readonly="true" />
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                onclick="QueryReportFun();">查询</a>
            <input id="organizationId" readonly="true" style="display: none;" />
        </div>
        <a href="#" class="easyui-linkbutton" id="edit" data-options="iconCls:'icon-edit',plain:true" onclick="javascript:EditEquipment()">编辑</a>
        <a href="#" class="easyui-linkbutton" id="add" data-options="iconCls:'icon-add',plain:true" onclick="javascript:AddEquipment()">增加设备</a>
        <a href="#" class="easyui-linkbutton" id="delete" data-options="iconCls:'icon-remove',plain:true" onclick="javascript:RemoveEquipment()">删除设备</a>
    </div>

    <%--添加设备对话框开始--%>
    <div id="AddDialog" class="easyui-dialog" title="添加设备" style="width: 770px; height: 340px; background-color: rgb(248, 246, 246)"
        data-options="iconCls:'icon-save',resizable:false,modal:true,closed: true,buttons:buttons_Add">
        <table style="padding: 10px;">
            <tr>
                <td>变量ID:</td>
                <td>
                    <input id="VariableId" class="easyui-validatebox" style="width: 120px;" data-options="required:true" />
                </td>
                <td>组织机构ID:</td>
                <td>
                    <select id="OrganizationID" class="easyui-combobox" style="width: 150px" data-options="panelHeight:'auto',required:true"></select></td>
                <td>设备名称:</td>
                <td>
                    <input id="EquipmentName" class="easyui-textbox" style="width: 120px;" data-options="required:true" /></td>
            </tr>
            <tr>
                <td>监控点类别:</td>
                <td>
                    <input id="MonitorType" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
                <td>电源取自位置:</td>
                <td>
                    <input id="PowerSupply" class="easyui-textbox" style="width: 150px;" data-option="required:true" /></td>
                <td>电压等级:</td>
                <td>
                    <input id="VoltageGrade" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
            </tr>
            <tr>
                <td>互感器变比:</td>
                <td>
                    <input id="RatedCT" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
                <td>电表编号:</td>
                <td>
                    <input id="AmmeterCode" class="easyui-textbox" style="width: 150px;" data-option="required:true" /></td>
                <td>实际变比:</td>
                <td>
                    <input id="ActualCT" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
            </tr>
            <tr>
                <td>设备功率:</td>
                <td>
                    <input id="Power" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
                <td>功率单位:</td>
                <td>
                    <input id="Unit" class="easyui-textbox" style="width: 150px;" data-option="required:true" /></td>
                <td>电源安装位置:</td>
                <td>
                    <input id="PowerSupplyPosition" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
            </tr>
            <tr>
                <td>额定电流:</td>
                <td>
                    <input id="Current" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
            </tr>
            <tr>
                <td rowspan="2">备注：</td>
                <td colspan="5" rowspan="2" style="text-align: left">
                    <textarea id="Remarks" style="width: 500px"></textarea>
                </td>
            </tr>
        </table>
    </div>
    <%--添加设备对话框结束--%>
    <div id="buttons_Add">
        <table style="width: 100%">
            <tr>
                <td style="text-align: right">
                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="javascript:SaveEquipment();">保存</a>
                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="javascript:$('#AddDialog').dialog('close');">取消</a>
                </td>
            </tr>
        </table>
    </div>

    <%--编辑设备对话框开始--%>
    <div id="EditDialog" class="easyui-dialog" title="编辑设备信息" style="width: 770px; height: 330px; background-color: rgb(248, 246, 246)"
        data-options="iconCls:'icon-edit',resizable:false,modal:true,closed: true,buttons:buttons_Edit">
        <%--<fieldset>--%>
        <%--<legend>主机设备</legend>--%>
        <table style="padding: 10px;">
            <tr>
                <td>变量ID:</td>
                <td>
                    <input id="VariableId_edit" class="easyui-textbox" style="width: 120px;" data-options="required:true" />
                </td>
                <td>组织机构ID:</td>
                <td>
                    <select id="OrganizationID_edit" class="easyui-combobox" style="width: 150px" data-options="panelHeight:'auto',required:true"></select></td>

                <td>设备名称:</td>
                <td>
                    <input id="EquipmentName_edit" class="easyui-textbox" style="width: 120px;" data-options="required:true" /></td>
            </tr>
            <tr>
                <td>监控点类别:</td>
                <td>
                    <input id="MonitorType_edit" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
                <td>电源取自位置:</td>
                <td>
                    <input id="PowerSupply_edit" class="easyui-textbox" style="width: 150px;" data-option="required:true" /></td>
                <td>电压等级:</td>
                <td>
                    <input id="VoltageGrade_edit" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
            </tr>
            <tr>
                <td>互感器变比:</td>
                <td>
                    <input id="RatedCT_edit" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
                <td>电表编号:</td>
                <td>
                    <input id="AmmeterCode_edit" class="easyui-textbox" style="width: 150px;" data-option="required:true" /></td>
                <td>实际变比:</td>
                <td>
                    <input id="ActualCT_edit" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
            </tr>
            <tr>
                <td>设备功率:</td>
                <td>
                    <input id="Power_edit" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
                <td>功率单位:</td>
                <td>
                    <input id="Unit_edit" class="easyui-textbox" style="width: 150px;" data-option="required:true" /></td>
                <td>电源安装位置:</td>
                <td>
                    <input id="PowerSupplyPosition_edit" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
            </tr>
            <tr>
                <td>额定电流:</td>
                <td>
                    <input id="Current_edit" class="easyui-textbox" style="width: 120px;" data-option="required:true" /></td>
            </tr>
            <tr>
                <td rowspan="2">备注：</td>
                <td colspan="5" rowspan="2" style="text-align: left">
                    <textarea id="Remarks_edit" style="width: 500px"></textarea>
                </td>
            </tr>
        </table>
        <%--</fieldset>--%>
    </div>
    <%--编辑设备对话框结束--%>
    <div id="buttons_Edit">
        <table cellpadding="0" cellspacing="0" style="width: 100%">
            <tr>
                <td style="text-align: right">
                    <a href="#" id="saveEditDlg" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="javascript:SaveEditEquipment();">保存修改</a>
                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="javascript:$('#EditDialog').dialog('close');">取消</a>
                </td>
            </tr>
        </table>
    </div>
    <!-- 右键菜单开始 -->
    <div id="RightMenu" class="easyui-menu" style="width: 120px;">
        <div id="mm_edit" onclick="EditEquipment()" data-options="iconCls:'icon-edit'">编辑</div>
        <div class="menu-sep"></div>
        <div id="mm_delete" onclick="RemoveEquipment()" data-options="iconCls:'icon-remove'">删除</div>
    </div>
    <!-- 右键菜单结束 -->
</body>
</html>
