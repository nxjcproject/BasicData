<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MasterSlaveMachinedescription.aspx.cs" Inherits="BasicData.Web.UI_BasicData.MasterSlaveMachine.MasterSlaveMachinedescription" %>

<%@ Register Src="~/UI_WebUserControls/TagsSelector/TagsSelector_Dcs.ascx" TagPrefix="uc1" TagName="TagsSelector_Dcs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>主从设备设置</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="../js/page/MasterSlaveMachine/MasterSlaveMachinedescription.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div id="toolbar_MasterMachineInfo" style="display: none;">
            <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>选择组织机构</td>
                                <td style="width: 200px;">
                                    <input id="Combobox_DCSF" class="easyui-combotree" style="width: 180px;" /></td>
                                <td>
                                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                        onclick="QueryMasterMachineInfoFun();">查询</a>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td><a id="id_add" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="AddMasterMachineFun();">添加</a>
                                </td>
                                <td>
                                    <div class="datagrid-btn-separator"></div>
                                </td>
                                <td>
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="RefreshMasterMachineFun();">刷新</a>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'west',collapsible:false" style="width: 700px;">
            <table id="grid_MasterMachineInfo" data-options="fit:true,border:false"></table>
        </div>
        <div id="toolbar_SlaveMachineInfo" style="display: none;">
            <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>当前主机名</td>
                                <td style="width: 200px;">
                                    <input id="Text_SelectMasterMachine" style="width: 180px;" readonly="readonly" /></td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td><a id="id_deleteAll" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true"
                                                onclick="RemoveAllSlaveMachineFun();">全部删除</a>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'center',border:false,collapsible:false" style="padding-left: 10px;">
            <table id="grid_SlaveMachineInfo" data-options="fit:true,border:true"></table>
        </div>
    </div>

    <!-------------------------------添加主机设备弹出对话框---------------------------------->
    <div id="dlg_AddMasterMachine" class="easyui-dialog" data-options="iconCls:'icon-save',resizable:false,modal:true">
        <fieldset>
            <legend>主机设备</legend>
            <table class="table" style="width: 100%;">
                <tr>
                    <th style="width: 80px; height: 30px;">变量名称</th>
                    <td style="width: 150px;">
                        <input id="TextBox_MasterVariableName" class="easyui-textbox" data-options="required:true,missingMessage:'不能为空', editable:false" style="width: 180px;" /></td>
                    <th style="width: 80px;">变量描述</th>
                    <td colspan="3">
                        <input id="TextBox_MasterVariableDescription" data-options="buttonText:'标签',buttonIcon:'icon-search',prompt:'查找DCS标签……',editable:false,onClickButton:function(){GetDcsTagsFun(1);}" class="easyui-textbox" style="width: 240px;" /></td>
                </tr>
                <tr>
                    <th style="height: 30px;">设备名称</th>
                    <td>
                        <input id="Commbox_EquipmentId" class="easyui-combotree" style="width: 180px;" />
                    </td>
                    <th style="width: 80px;">停机信息</th>
                    <td style="width: 70px;">
                        <input id="Checkbox_MasterRecord" type="checkbox" value="1" />是否记录</td>
                    <th style="width: 70px;">停机状态位</th>
                    <td>
                        <input type="radio" name="SelectRadio_MasterValidValues" id="Radio_MasterValidValueOn" value="1" />1
                        <input type="radio" name="SelectRadio_MasterValidValues" id="Radio_MasterValidValueOff" value="0" checked="checked" />0
                    </td>
                </tr>
                <tr>
                    <th style="height: 30px;">备注</th>
                    <td colspan="5">
                        <textarea id="TextBox_MasterRemark" cols="20" name="S1" style="width: 480px; height: 100px;" draggable="false"></textarea></td>
                </tr>
            </table>
        </fieldset>
    </div>
    <div id="buttons_AddMasterMachine">
        <table cellpadding="0" cellspacing="0" style="width: 100%">
            <tr>
                <td style="text-align: right">
                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="javascript:SaveMasterMachine();">保存</a>
                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="javascript:$('#dlg_AddMasterMachine').dialog('close');">取消</a>
                </td>
            </tr>
        </table>
    </div>

    <!-------------------------------添加从机设备弹出对话框---------------------------------->
    <div id="dlg_AddSlaveMachine" class="easyui-dialog" data-options="iconCls:'icon-save',resizable:false,modal:true">
        <fieldset>
            <legend>从机设备</legend>
            <table class="table" style="width: 100%;">
                <tr>
                    <th style="width: 180px; height: 30px;">变量名称</th>
                    <td style="width: 150px;">
                        <input id="TextBox_SlaveVariableName" class="easyui-textbox" data-options="required:true,missingMessage:'不能为空', editable:false" style="width: 140px;" /></td>
                    <th style="width: 110px;">变量描述</th>
                    <td>
                        <input id="TextBox_SlaveVariableDescription" data-options="buttonText:'标签',buttonIcon:'icon-search',prompt:'查找DCS标签……',editable:false,onClickButton:function(){GetDcsTagsFun(2);}" class="easyui-textbox" style="width: 180px;" /></td>
                </tr>
                <tr>
                    <th style="height: 30px;">允许时间(秒)</th>
                    <td>
                        <input id="Text_TimeDelay" class="easyui-numberspinner" data-options="min:1,max:9999,editable:true" style="width: 140px;" /><%--<input id="Text_TimeDelay" class="easyui-timespinner" data-options="value:'00:05', min:'00:01',max:'23:59'" style="width: 140px;"/>--%>
                    </td>
                    <th>停机状态位</th>
                    <td>
                        <input type="radio" name="SelectRadio_SlaveValidValues" id="Radio_SlaveValidValueOn" value="1" />1
                            <input type="radio" name="SelectRadio_SlaveValidValues" id="Radio_SlaveValidValueOff" value="0" checked="checked" />0
                    </td>
                </tr>
                <tr>
                    <th>备注</th>
                    <td colspan="3">
                        <textarea id="TextBox_SlaveRemark" cols="20" name="S1" style="width: 460px; height: 100px;" draggable="false"></textarea>
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <div id="buttons_AddSlaveMachine">
        <table cellpadding="0" cellspacing="0" style="width: 100%">
            <tr>
                <td style="text-align: right">
                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="javascript:SaveSlaveMachine();">保存</a>
                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="javascript:$('#dlg_AddSlaveMachine').dialog('close');">取消</a>
                </td>
            </tr>
        </table>
    </div>
    <!---------------------------------弹出对话框选择DCS标签--------------------------------->
    <form id="formMasterSlaveMachineDescripition" runat="server">
        <div id="dlg_SelectDcsTags" class="easyui-dialog" data-options="iconCls:'icon-search',resizable:false,modal:true">
            <uc1:TagsSelector_Dcs ID="TagsSelector_DcsTags" runat="server" />
        </div>
        <div>
            <asp:HiddenField ID="HiddenField_PageName" runat="server" />
            <asp:HiddenField ID="HiddenField_MasterMachineId" runat="server" />
            <asp:HiddenField ID="HiddenField_KeyId" runat="server" />
            <asp:HiddenField ID="HiddenField_SlaveMachineId" runat="server" />
            >
        </div>
    </form>
</body>
</html>
