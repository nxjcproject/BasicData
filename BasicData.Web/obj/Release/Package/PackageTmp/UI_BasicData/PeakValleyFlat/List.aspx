<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="BasicData.Web.UI_BasicData.PeakValleyFlat.List" %>

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

    <script type="text/javascript" src="../js/page/PeakValleyFlat/List.js" charset="utf-8"></script>

    <title>峰谷平列表</title>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false" style="padding: 5px;">
        <div data-options="region:'west',border:false,collapsible:false" style="width: 230px;">
            <uc1:OrganisationTree runat="server" ID="OrganisationTree" />
        </div>
        <div data-options="region:'center',border:false,collapsible:false" style="padding-left: 10px;">
            <table>
                <tr>
                    <td>产线名称:</td>
                    <td><input id="organizationName" class="easyui-textbox" style="width:100px"/></td>
                    <td>启用时间:</td>
                    <td>
                        <input id="selectedDate" style="width: 100px;" />
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
        <a href="javascript:void(0)" id="editBtn" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true" onclick="editItem()">编辑</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="loadGridData('first')">刷新</a>
    </div>

    <div id="editDialog" class="easyui-dialog" title="编辑" data-options="iconCls:'icon-save',closed:true,buttons:'#editDialog-buttons'" style="width: 400px; height: 200px; padding: 10px">
        <table>
            <tr>
                <td>启用时间</td>
                <td>
                    <input id="startUsing" style="width: 100px;" />
                </td>
            </tr>
            <tr>
                <td>停用时间</td>
                <td>
                    <input id="endUsing" style="width: 100px;" />
                </td>
            </tr>
            <tr>
                <td>启用标志</td>
                <td>
                    <input id="radioTrue" type="radio" name="radiobutton" value="True" />启用
                    <input id="radioFalse" type="radio" name="radiobutton" value="False" />停用
                </td>
            </tr>
        </table>
    </div>
    <div id="editDialog-buttons">
        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="saveEditDialog()">保存</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript:$('#editDialog').dialog('close')">取消</a>
    </div>

    <div id="addDialog" class="easyui-dialog" title="编辑" data-options="iconCls:'icon-edit',closed:true,buttons:'#addDialog-buttons'" style="width: 800px; height: 400px; padding: 10px">
        <table id="adddg" class="easyui-datagrid" title="" style="width:100%;height:auto">
	    </table>

        <div id="addtb" style="padding:5px;height:auto">
            <a href="javascript:void(0)"  class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="addItemPeak()">添加</a>
	        <a href="javascript:void(0)"  class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="deleteTimeItem()">删除</a>
            <a href="javascript:void(0)"  class="easyui-linkbutton" data-options="iconCls:'icon-clear',plain:true" onclick="clearAllItems()">清空</a>
            <%--<a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-back',plain:true" onclick="javascript:window.history.go(-1);">返回</a>--%>
	        <%--<a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="$('#save').dialog('open')">保存</a>--%>
        </div>
    </div>
    <div id="addDialog-buttons">
        <%--<a href="javascript:void(0)" class="easyui-linkbutton" onclick="saveEditDialog()">保存</a>--%>
        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="saveItem()">保存</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript:$('#addDialog').dialog('close')">取消</a>
    </div>

    <div id="edit" class="easyui-dialog" title="添加峰谷平时间段" data-options="iconCls:'icon-save',closed:true,buttons:'#edit-buttons'" style="width:400px;height:200px;padding:10px">
        <table>
            <tr>
                <td style="width:30%;">起始时间</td>
                <td style="width:70%;">
                    <input id="startTime" class="easyui-timespinner" style="width:80px;" required="required" data-options="showSeconds:true,readonly:true" />
                </td>
            </tr>
            <tr>
                <td>终止时间</td>
                <td>
                    <input id="endTime" class="easyui-timespinner" style="width:80px;" required="required" data-options="showSeconds:true" />
                </td>
            </tr>
            <tr>
                <td>类型</td>
                <td>
                    <select id="type" class="easyui-combobox" style="width:200px;">
                        <option value="峰期">峰期</option>
                        <option value="谷期">谷期</option>
                        <option value="平期">平期</option>
                    </select>
                </td>
            </tr>
        </table>
    </div>
    <div id="edit-buttons">
		<a href="javascript:void(0)" class="easyui-linkbutton" onclick="saveAddDialog()">保存</a>
		<a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript:$('#edit').dialog('close')">取消</a>
	</div>

    <div id="detailDialog" class="easyui-dialog" title="编辑" data-options="iconCls:'icon-edit',closed:true," style="width: 800px; height: 400px; padding: 10px">
        <table id="detaildg" class="easyui-datagrid" title="" style="width:100%;height:auto">
	    </table>
    </div>
</body>
</html>
