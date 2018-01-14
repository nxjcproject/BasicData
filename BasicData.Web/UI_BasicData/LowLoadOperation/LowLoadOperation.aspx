<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LowLoadOperation.aspx.cs" Inherits="BasicData.Web.UI_BasicData.LowLoadOperation.LowLoadOperation" %>
<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>
<%@ Register Src="~/UI_WebUserControls/TagsSelector/TagsSelector_Dcs.ascx" TagPrefix="uc2" TagName="TagsSelector_Dcs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>设备低负荷配置</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script> 
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint.js" charset="utf-8"></script>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script> 

    <script type="text/javascript" src="../js/page/LowLoadOperation/LowLoadOperation.js" charset="utf-8"></script>
</head>
<body>
    <div id="cc" class="easyui-layout"data-options="fit:true,border:false" >    
         <div data-options="region:'west'" style="width: 150px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
          <div id="toorBar" title="" style="height:30px;padding:10px;">
            <div>
                <table>
                    <tr>
                        <td>组织机构:</td>
                        <td >                               
                            <input id="organizationName" class="easyui-textbox" readonly="readonly"style="width:80px" />  
                            <input id="organizationId" readonly="readonly" style="display: none;" />             
                        </td>
                        <td>名称：</td>
                        <td>
                            <input id="variable" class="easyui-textbox" style="width: 120px;" data-options="required:false" />
                        </td>
                        <td>
                            <a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Query()">查询</a>
                        </td>
                              <td style="width:40px"></td>
                       <td>
                            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="addFun()">添加</a>
                        </td>
                         <td>
                            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="refresh()">刷新</a>
                        </td>                   
                    </tr>
                </table>         
            </div>
	    </div>
         <div data-options="region:'center'" style="padding:5px;background:#eee;">
             <table id="grid_Main"class="easyui-datagrid"></table>
         </div>
        <!-------------------------------添加主机设备弹出对话框---------------------------------->
        <div id="AddandEditor" class="easyui-window" title="低负荷设备配置" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false" style="width:750px;height:390px;padding:10px 20px 10px 20px">
            <table class="table" style="width: 100%;">
                <tr>
                    <th style="width: 80px; height: 30px;">名称</th>
                    <td style="width: 150px;">
                        <input id="variableDecs" class="easyui-textbox" data-options="required:true,missingMessage:'不能为空', editable:true" style="width: 180px;" /></td>
                    <th style="width: 80px;">停机信息</th>
                    <td style="width: 70px;">
                        <input id="Checkbox_MasterRecord" type="checkbox" value="1" />是否记录</td>
                   <%-- <th style="width: 80px; height: 30px;">有效值</th>
                    <td style="width: 150px;">
                        <input id="Text3" class="easyui-numberbox" data-options="required:true,missingMessage:'不能为空', editable:true" style="width: 180px;" /></td>--%>
                </tr>
                <tr>
                    <th style="width: 80px; height: 30px;">运行信号</th>
                    <td style="width: 150px;">
                        <input id="TextBox_MasterVariableName" class="easyui-textbox" data-options="required:true,missingMessage:'不能为空', editable:false" style="width: 180px;" /></td>
                    <th style="width: 80px;">变量描述</th>
                    <td colspan="3">
                        <input id="TextBox_MasterVariableDescription" data-options="buttonText:'标签',buttonIcon:'icon-search',prompt:'查找运行标签……',editable:false,onClickButton:function(){GetDcsTagsFun(1);}" class="easyui-textbox" style="width: 240px;" /></td>
                </tr>
                <tr>
                    <th style="width: 80px; height: 30px;">DCS负载信号</th>
                    <td style="width: 150px;">
                        <input id="s_TextBox_MasterVariableName" class="easyui-textbox" data-options="required:false, editable:false" style="width: 180px;" /></td>
                    <th style="width: 80px;">变量描述</th>
                    <td colspan="3">
                        <input id="s_TextBox_MasterVariableDescription" data-options="buttonText:'标签',buttonIcon:'icon-search',prompt:'查找DCS标签……',editable:false,onClickButton:function(){GetDcsTagsFun(3);}" class="easyui-textbox" style="width: 240px;" /></td>
                </tr>
                <tr>
                    <th style="width: 80px; height: 30px;">延时时间(秒)</th>
                    <td style="width: 150px;">
                        <input id="Text1" class="easyui-numberbox" data-options="required:true,missingMessage:'不能为空', editable:true" style="width: 180px;" /></td>
                    <th style="width: 80px; height: 30px;">负载下限</th>
                    <td style="width: 150px;">
                        <input id="Text7" class="easyui-numberbox" data-options="required:true,missingMessage:'不能为空', editable:true" style="width: 180px;" />
                    </td>
                    <th style="width: 80px; display:none;">负载类型</th>
                    <td colspan="3" style="display:none;">
                        <select class="easyui-combobox" id="Text2" name="type" data-options="panelHeight:'auto',required:true,missingMessage:'不能为空'" style="width:160px">
                            <option value="current">电流</option>
                            <option value="power">功率</option>
	    			    </select>
                    </td>         
                </tr>
                <tr>
                    <th style="width: 80px; height: 30px; display:none;">负载标签</th>
                    <td style="width: 150px; display:none;">
                        <input id="Text5" class="easyui-textbox" data-options="required:false, editable:true" style="width: 180px;" />
                    </td>                             
                </tr>
                <tr>
                    <%--<th style="height: 30px;">设备名称</th>
                    <td>
                        <input id="Commbox_EquipmentId" class="easyui-combotree" style="width: 180px;" />
                    </td>--%>
                    
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
	            <div style="text-align:center;padding:5px;margin-left:-18px;">
	    	        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="save()">保存</a>
	    	        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#AddandEditor').window('close');">取消</a>
	            </div>
         </div>
    </div>
    <form id="formMasterSlaveMachineDescripition" runat="server">
        <div id="dlg_SelectDcsTags" class="easyui-dialog" data-options="iconCls:'icon-search',resizable:false,modal:true">
            <uc2:TagsSelector_Dcs ID="TagsSelector_DcsTags" runat="server" />
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