<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KPIndexEnergy.aspx.cs" Inherits="BasicData.Web.UI_BasicData.KpiDefinition.KPIndex" %>
<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <title>能源KPI指标定义</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>
    
	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/jquery.utility.js"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript"src="../js/page/KpiDefinition/KPIndexEnergy.js"charset="utf-8"></script>
</head>
<body>
        <div class="easyui-layout" data-options="fit:true,border:false">
            <div id="toolbar_KPIndexEnergyInfo" data-options="region:'north'" style="height: 80px;display:none">
            <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>标准层次</td>
                                <td style="width: 110px;">
                                    <input id="cbox_Standard" class="easyui-combobox" data-options="panelHeight:'auto'" style="width: 100px;" />
                                </td>
                                  <td>能耗类型</td>
                                <td style="width: 110px;">
                                    <input id="cbox_Consumpation" class="easyui-combobox" style="width: 100px;"data-options="panelHeight:'auto'" />
                                </td>                             
                                <td style="width: 110px;">
                                    <input id="cbox_Insideoutside" class="easyui-combobox" style="width: 110px;"data-options="panelHeight:'auto'" />
                                </td> 
                               <td><a id="add_list" href="javascript:void(0)" class="easyui-linkbutton" onclick="CreateKPIndexEnergy()" data-options="iconCls:'icon-add',plain:true">添加</a></td>
                                <td><a id="delete_list" href="javascript:void(0)" class="easyui-linkbutton" onclick="DeleteKPIndexEnergy()" data-options="iconCls:'icon-remove',plain:true">删除</a></td>                          
                                <td>
                                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                        onclick=" LoadIndex_Clinck()">查询</a>
                                </td>
                                  <td>
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="RefreshRecordDataFun();">刷新</a>
                                 </td>
                                <%--<td>
                                    <input id="TextBox_OrganizationId" style="width: 10px; visibility: hidden;" />
                                </td>--%>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                           <%-- <tr>
                                 <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-printer',plain:true" onclick="PrintFileFun();">打印</a>
                                </td>
                                <td>
                                    <div class="datagrid-btn-separator"></div>
                                </td>
                                <td>
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="RefreshRecordDataFun();">刷新</a>
                                 </td>
                            </tr>--%>
                        </table>                               
                    </td>
                </tr>
            </table>
           </div>   
            <div data-options="region:'center',border:false,fit:true">               
                <table id="dgrid_Index"class="easyui-datagrid"></table>
            </div>
         </div>


     <!-- KPI详细添加窗口开始 -->
    <div id="dlgKPIndexEnergyDetailEditor" class="easyui-dialog" title="KPI指标添加" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false,buttons:'#bbKPIndexEnergyDetailEditor'" style="width:400px;height:auto;padding:10px 60px 20px 60px">
	    <table>
             <tr>
	    		<td>项目名称：</td>
                <td><input class="easyui-textbox" type="text" id="txtObjName" data-options="required:true" style="width:140px" /></td>
	    	</tr>

            <tr>
	    		<td>标准层次：</td>
                <td><input class="easyui-combobox" type="text" id="txtStandardLevel" data-options="panelHeight: 'auto',required:true" style="width:140px" /></td>
	    	</tr>    
            <tr>
	    		<td>能耗类型：</td>
                <td>   <select id="drpEnerygType" class="easyui-combobox" data-options="editable:false,panelHeight:'auto'">
                        <option value="Comprehensive">综合能耗</option>
                        <option value="Entity">标准能耗</option>
                    </select>
                </td>
	    	</tr>          
            <tr>
	    		<td>企业内是否执行：</td>
                <td>
                  <span class="radioSpan"> <%--style="width:30px"--%>
                    <input type="radio"name="adminFlag" value="1"onclick=" radioYes()"/>是<input type="radio" name="adminFlag" value="0" onclick="radioNo()"/>否
                     <label id="Lab_radio">  （请选择！）</label></span>
                 </td>
            </tr>           
	    	<tr>
	    		<td>组织机构：</td>
                <td><input class="easyui-combotree" type="text" id="KPIndexEnergyOrganization" data-options="panelHeight: 'auto',required:true" style="width:140px" /><label id="Lab_warning"></label></td>
	    	</tr>
	    	<tr>
	    		<td>项目：</td>
                <td><input class="easyui-combobox" type="text" id="txtName" data-options="panelHeight: 'auto',required:true" style="width:140px" /></td>
	    	</tr>
	    	<tr>
	    		<td>类型：</td>
	    		<td>
                    <select id="drpLevelType" class="easyui-combobox" data-options="editable:false,panelHeight:'auto'">
                        <option value="Process">工序</option>
                        <option value="ProductionLine">生产线</option>
                    </select>
	    		</td>
	    	</tr>	    	
	    	<tr>
	    		<td>值类型：</td>
	    		<td>
                    <select id="drpValueType" class="easyui-combobox" data-options="editable:false,panelHeight:'auto'">
                        <option value="ElectricityConsumption">电耗</option>
                        <option value="CoalConsumption">煤耗</option>
                        <option value="EnergyConsumption">能耗</option>
                    </select>
	    		</td>
	    	</tr>
	    	<tr>
	    		<td>单位：</td>
	    		<td>
                    <select id="drpUnit" class="easyui-combobox" data-options="editable:false,panelHeight:'auto'">
                        <option value="kW·h/t">kW·h/t</option>
                        <option value="kgce/t">kgce/t</option>
                    </select>
	    		</td>
	    	</tr>
	    	<tr>
	    		<td>标准值：</td>
                <td><input class="easyui-textbox" type="text" id="txtStandardValue" data-options="required:true" style="width:140px" /></td>
	    	</tr>	
            <tr>
	    		<td>标准层次：</td>
	    		<td>
                    <select id="txtLevel" class="easyui-combobox" data-options="editable:false,panelHeight:'auto'">
                        <option>1</option>
                        <option>2</option>
                        <option>3</option>
                    </select>
	    		</td>
	    	</tr> 
            <tr>
	    		<td>创建人：</td>
                <td><input class="easyui-textbox" type="text" id="txtCreator" data-options="required:true" style="width:140px" /></td>
	    	</tr>	   	
	    </table>
    </div>
	<div id="bbKPIndexEnergyDetailEditor">
	    <a id="save_detail" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="KPIndexEnergyDetailSave()">保存</a>
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#dlgKPIndexEnergyDetailEditor').dialog('close');">取消</a>
	</div>
    <form id="form1" runat="server">
    <div>  
    </div>
    </form>
</body>
</html>
