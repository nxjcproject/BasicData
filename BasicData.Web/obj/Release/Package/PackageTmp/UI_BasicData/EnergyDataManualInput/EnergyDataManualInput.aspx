<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnergyDataManualInput.aspx.cs" Inherits="BasicData.Web.UI_BasicData.EnergyDataManualInput.EnergyDataManualInput" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>

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

    <script src="../js/page/EnergyDataManualInput/EnergyDataManualInput.js"></script>

    <title></title>
</head>
<body>
    <%-- 手动数据补录--%>
    <div class="easyui-layout" data-options="fit:true,border:false" style="padding: 5px;">
        <div data-options="region:'west',border:false,collapsible:false" style="width: 150px;">
            <uc1:OrganisationTree runat="server" ID="OrganisationTree" />
        </div>
        <div data-options="region:'center',border:false,collapsible:false">
            <div id="toolBar">
                <div  style="height: 25px; padding-top: 10px;padding-left:10px">
                    <span>组织机构</span>
                    <span>
                        <input id="organizationName" class="easyui-textbox" style="width: 100px"/></span>
                          <span>开始时间</span>
                    <span id="db_startTime" class="easyui-datetimebox" style="width:150px;" data-option="required:true"></span>
                    <span>结束时间</span>
                    <span id="db_endTime" class="easyui-datetimebox" style="width:150px;" data-option="required:true"></span>
                    <span><a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="query()">查询</a></span>
                </div>
                <div id="tb" style="padding: 5px; height: auto">
                    <a href="javascript:void(0)" id="add" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="addItem()">添加</a>
                    <a href="javascript:void(0)" id="delete" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="deleteItem()">删除</a>
                    <a href="javascript:void(0)" id="edit" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true" onclick="editItem()">编辑</a>
                    <a href="javascript:void(0)" id="query" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="query()">刷新</a>
                </div>
            </div>
            <table id="dg" class="easyui-datagrid" title="" style="width: 100%; height: auto">
            </table>
        </div>
    </div>



    <div id="addDialog" class="easyui-dialog" title="添加录入信息" data-options="iconCls:'icon-save',closed:true,buttons:'#addDialog-buttons'" style="width: 500px; height: 200px; padding: 10px">
        <table>
            <tr>
                <td style="width: 15%;">变量名称</td>
                <td style="width: 35%;">
                    <input id="addVariableName" class="easyui-combobox" style="width: 150px;" data-options="panelHeight:'auto'"/>
                </td>
                <td style="width: 15%;">更新周期</td>
                <td style="width: 35%;">
                    <select id="addUpdateCycle" class="easyui-combobox" data-options="panelHeight:'auto'">
                        <option value="day">天</option>
                        <option value="month">月</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td style="width: 15%;">录入值</td>
                <td style="width: 35%;">
                    <input id="addDataValue" class="easyui-numberbox" data-options="min:0,precision:4" required="required" />
                </td>
                <td style="width: 15%;">创建时间</td>
                <td style="width: 35%;">
                    <input id="addTimeStamp" class="easyui-datebox" required="required" />
                </td>
            </tr>
            <tr>
                <td>备注</td>
                <td colspan="3">
                    <input id="addRemark" class="easyui-textbox" data-options="multiline:true" style="width: 300px;" />
                </td>
            </tr>
        </table>
    </div>
    <div id="addDialog-buttons">
        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="saveAddDialog()">保存</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript:$('#addDialog').dialog('close')">取消</a>
    </div>

    <div id="editDialog" class="easyui-dialog" title="编辑录入信息" data-options="iconCls:'icon-save',closed:true,buttons:'#editDialog-buttons'" style="width: 500px; height: 200px; padding: 10px">
        <table>
            <tr>
                <td style="width: 15%;">变量名称</td>
                <td style="width: 35%;">
                    <input id="editVariableName" class="easyui-textbox" data-options="readonly:true" style="width: 150px;" />
                </td>
                <td style="width: 15%;">更新周期</td>
                <td style="width: 35%;">
                    <input id="editUpdateCycle" class="easyui-textbox" data-options="readonly:true" />
                </td>
            </tr>
            <tr>
                <td style="width: 15%;">录入值</td>
                <td style="width: 35%;">
                    <input id="editDataValue" class="easyui-numberbox" data-options="min:0,precision:4" required="required" />
                </td>
                <td style="width: 15%;">创建时间</td>
                <td style="width: 35%;">
                    <input id="editTimeStamp" class="easyui-datebox" required="required" />
                </td>
            </tr>
            <tr>
                <td>备注</td>
                <td colspan="3">
                    <input id="editRemark" class="easyui-textbox" data-options="multiline:true" style="width: 300px;" />
                </td>
            </tr>
        </table>
    </div>
    <div id="editDialog-buttons">
        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="saveEditDialog()">保存</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript:$('#editDialog').dialog('close')">取消</a>
    </div>
        <form id="form_EnergyConsumptionPlan" runat="server">
        <div>
            <asp:HiddenField ID="Hiddenfield_PageId" runat="server" />
        </div>
    </form>
</body>
</html>
