<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="BasicData.Web.UI_BasicData.WorkingTeamAndShift.Edit" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/jquery.utility.js" charset="utf-8"></script>

    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="../js/page/WorkingTeamAndShift/Edit.js" charset="utf-8"></script>

    <title>班次班组定义</title>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false" style="padding: 5px;">
        <div data-options="region:'west',border:false,collapsible:false" style="width: 230px;">
            <uc1:OrganisationTree runat="server" ID="OrganisationTree" />
        </div>
        <div data-options="region:'center',border:false,collapsible:false" style="padding-left: 10px;">
            <table>
                <tr>
                    <td>当前产线名称:</td>
                    <td><input id="organizationName" class="easyui-textbox" style="width:100px"/></td>
                    <%--<td><a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok',plain:true" onclick="loadGridData('first')">查询</a></td>--%>
                </tr>
            </table>
            <div style="height: 5px"></div>
            <table id="dg_shift" class="easyui-datagrid" title="班次时间设置" style="width: 100%; height: auto">
            </table>
            <div style="height: 50px;"></div>
            <table id="dg_workingteam" class="easyui-datagrid" title="班组设置" style="width: 100%; height: auto">
            </table>
        </div>

        <div id="tb_shift" style="padding: 5px; height: auto">
            <a id="id_shiftEdit" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true" onclick="shiftEditItem()">编辑</a>
            <a id="id_shiftReset" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="shiftReload()">重置</a>
            <a id="id_shiftSave" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="shiftSave()">保存</a>
        </div>

        <div id="shiftEditDialog" class="easyui-dialog" title="编辑" data-options="iconCls:'icon-save',closed:true,buttons:'#shiftEditDialog-buttons'" style="width: 400px; height: 220px; padding: 10px">
            <table>
                <tr>
                    <td>班次名称</td>
                    <td>
                        <input id="name" style="width: 100px;" readonly="true" /></td>
                </tr>
                <tr>
                    <td>启用时间</td>
                    <td>
                        <input id="startTime" class="easyui-timespinner" data-options="showSeconds:false" style="width: 100px;" required="required" />
                    </td>
                </tr>
                <tr>
                    <td>停用时间</td>
                    <td>
                        <input id="endTime" class="easyui-timespinner" data-options="showSeconds:false" style="width: 100px;" />
                    </td>
                </tr>
                <tr>
                    <td>描述</td>
                    <td>
                        <input id="description" class="easyui-textbox" style="width: 100px;" />
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
        <div id="shiftEditDialog-buttons">
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="shiftSaveEditDialog()">保存</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript:$('#shiftEditDialog').dialog('close')">取消</a>
        </div>

        <div id="tb_workingteam" style="padding: 5px; height: auto">
            <a id="id_workingTeamEdit" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true" onclick="shifteditItem()">编辑</a>
            <a id="id_workingTeamReset" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="workingteamReload()">重置</a>
            <a id="id_workingTeamSave" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="workingteamSave()">保存</a>
        </div>

        <div id="workingteamEditDialog" class="easyui-dialog" title="编辑" data-options="iconCls:'icon-save',closed:true,buttons:'#workingteamEditDialog-buttons'" style="width: 400px; height: 200px; padding: 10px">
            <table>
                <tr>
                    <td>班次名称</td>
                    <td>
                        <input id="workingteamName" style="width: 100px;" readonly="true" /></td>
                </tr>
                <tr>
                    <td>负责人</td>
                    <td>
                        <input id="chargeMan" style="width: 100px;" />
                    </td>
                </tr>
                <tr>
                    <td>备注</td>
                    <td>
                        <input id="remarks" style="width: 100px;" />
                    </td>
                </tr>
                <tr>
                    <td>启用标志</td>
                    <td>
                        <input id="radio1" type="radio" name="radiobutton" value="True" />启用
                    <input id="radio2" type="radio" name="radiobutton" value="False" />停用
                    </td>
                </tr>
            </table>
        </div>

        <div id="workingteamEditDialog-buttons">
            <a  href="javascript:void(0)" class="easyui-linkbutton" onclick="workingteamSaveEditDialog()">保存</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript:$('#workingteamEditDialog').dialog('close')">取消</a>
        </div>
    </div>

</body>
</html>
