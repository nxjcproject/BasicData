<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnergyDataManualInputContrast.aspx.cs" Inherits="BasicData.Web.UI_BasicData.EnergyDataManualInput.EnergyDataManualInputContrast" %>

<%--<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>--%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/jquery.utility.js" charset="utf-8"></script>

    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script src="../js/page/EnergyDataManualInput/EnergyDataManualInputContrast.js"></script>

    <title></title>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false" style="padding: 5px;">
        <%--<div data-options="region:'west',border:false,collapsible:false" style="width: 230px;">
            <uc1:OrganisationTree runat="server" ID="OrganisationTree" />
        </div>--%>
        <div data-options="region:'center',border:false,collapsible:false" style="padding-left: 10px;">
            <table>
                <tr>
                    <td>变量名称:</td>
                    <td>
                        <input id="variableName" style="width: 100px;" />
                    </td>
                    <td><a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok',plain:true" onclick="loadGridData('first')">查询</a></td>
                </tr>
            </table>
            <table id="dg" class="easyui-datagrid" title="" style="width: 100%; height: auto">
            </table>
        </div>
    </div>

    <div id="tb" style="padding: 5px; height: auto">
        <a href="javascript:void(0)" id="add" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="addItem()">添加</a>
        <a href="javascript:void(0)" id="delete" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="deleteItem()">删除</a>
        <a href="javascript:void(0)" id="edit" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true" onclick="editItem()">编辑</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="loadGridData()">刷新</a>
    </div>

    <div id="addDialog" class="easyui-dialog" title="添加录入信息" data-options="iconCls:'icon-save',closed:true,buttons:'#addDialog-buttons'" style="width:550px;height:220px;padding:10px">
        <table style="border-spacing: 5px">
        <tr>
            <td style="width: 15%;">变量ID</td>
            <td style="width: 35%;">
                <input id="addVariableId" class="easyui-textbox" required="required" />
            </td>
            <td style="width: 15%;">变量名称</td>
            <td style="width: 35%;">
                <input id="addVariableName" class="easyui-textbox" required="required" />
            </td>
        </tr>
        <tr>
            <td style="width: 15%;">变量类别</td>
            <td>
                <select id="cbo_AddType" class="easyui-combobox" style="width: 147px;" data-option="readonly:true">
                    <option value="熟料">熟料</option>
                    <option value="分厂">分厂</option>
                </select>
            </td>
            <td style="width: 15%;">变量分组</td>
            <td style="width: 35%;">
                <input id="cbo_AddRole" class="easyui-combobox" data-option="required:required" />
            </td>
        </tr>
        <tr>
            <td>备注</td>
            <td colspan="3">
                <input id="addRemark" class="easyui-textbox" data-options="multiline:true" style="width: 300px;" />
            </td>
        </tr>
        <tr>
            <td>启用标志</td>
            <td>
                <input id="addradioTrue" type="radio" name="radiobutton" value="True" checked="checked" />启用                
                <input id="addradioFalse" type="radio" name="radiobutton" value="False" />停用
            </td>
        </tr>
    </table>
    </div>
    <div id="addDialog-buttons">
		<a href="javascript:void(0)" class="easyui-linkbutton" onclick="saveAddDialog()">保存</a>
		<a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript:$('#addDialog').dialog('close')">取消</a>
	</div>

    <div id="editDialog" class="easyui-dialog" title="编辑录入信息" data-options="iconCls:'icon-save',closed:true,buttons:'#editDialog-buttons'" style="width: 550px; height: 220px; padding: 10px">
        <table style="border-spacing: 5px">
            <tr>
                <td style="width: 15%;">变量ID</td>
                <td style="width: 35%;">
                    <input id="editVariableId" class="easyui-textbox" required="required" readonly="true" />
                </td>
                <td style="width: 15%;">变量名称</td>
                <td style="width: 35%;">
                    <input id="editVariableName" class="easyui-textbox" required="required" />
                </td>
            </tr>
            <tr>
                <td style="width: 15%;">变量类别</td>
                <td>
                    <select id="cbo_editType" class="easyui-combobox" style="width: 147px;" data-option="readonly:true">
                        <option value="熟料">熟料</option>
                        <option value="分厂">分厂</option>
                    </select>
                </td>
                <td style="width: 15%;">变量分组</td>
                <td style="width: 35%;">
                    <input id="cbo_editRole" class="easyui-combobox" data-option="required:required" />
                </td>
            </tr>
            <tr>
                <td>备注</td>
                <td colspan="3">
                    <input id="editRemark" class="easyui-textbox" data-options="multiline:true" style="width: 300px;" />
                </td>
            </tr>
            <tr>
                <td>启用标志</td>
                <td>
                    <input id="editradioTrue" type="radio" name="editradiobutton" value="True" checked="checked" />启用                 
                    <input id="editradioFalse" type="radio" name="editradiobutton" value="False" />停用
                </td>
            </tr>          
        </table>
    </div>
    <div id="editDialog-buttons">
		<a href="javascript:void(0)" class="easyui-linkbutton" onclick="saveEditDialog()">保存</a>
		<a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript:$('#editDialog').dialog('close')">取消</a>
	</div>
</body>
</html>
