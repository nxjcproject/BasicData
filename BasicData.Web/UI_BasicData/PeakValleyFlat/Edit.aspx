<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="BasicData.Web.UI_BasicData.PeakValleyFlat.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/jquery.utility.js" charset="utf-8"></script>

	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="../js/page/PeakValleyFlat/Edit.js" charset="utf-8"></script>

    <title>峰谷平分段定义</title>
</head>
<body>
    <table id="dg" class="easyui-datagrid" title="" style="width:100%;height:auto">
	</table>

    <div id="tb" style="padding:5px;height:auto">
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="addItem()">添加</a>
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="deleteItem()">删除</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-clear',plain:true" onclick="clearAllItems()">清空</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-back',plain:true" onclick="javascript:window.history.go(-1);">返回</a>
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="$('#save').dialog('open')">保存</a>
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
		<a href="javascript:void(0)" class="easyui-linkbutton" onclick="saveEditDialog()">保存</a>
		<a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript:$('#edit').dialog('close')">取消</a>
	</div>

    <div id="save" class="easyui-dialog" title="添加启用时间" data-options="iconCls:'icon-save',closed:true,buttons:'#save-buttons'" style="width:210px;height:150px;padding:10px">
        <table style="width:100%">
            <tr>
                <td style="width:45%;">起用时间:</td>
                <td style="width:55%;">
                    <input id="startUsing"  style="width:100px;" />
                </td>
            </tr>
        </table>
    </div>
    <div id="save-buttons">
		<a href="javascript:void(0)" class="easyui-linkbutton" onclick="saveItem()">保存</a>
		<a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript:$('#save').dialog('close')">取消</a>
	</div>

</body>
</html>
