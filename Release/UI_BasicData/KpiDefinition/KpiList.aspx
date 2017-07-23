<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KpiList.aspx.cs" Inherits="BasicData.Web.UI_BasicData.KPIDefinition.KpiList" %>
<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>KPI指标体系定义</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/jquery.utility.js"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/UI_BasicData/js/page/KpiDefinition/KpiList.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false" style="padding: 5px;">
        <!-- 引领表开始 -->
        <div data-options="region:'west',collapsible:false" style="width: 330px;">
	        <table id="dgKpiList" class="easyui-datagrid" data-options="rownumbers:true,singleSelect:true,onDblClickRow:OnKpiListDblClicked,toolbar:'#tbKpiList',fill:true">
		        <thead>
			        <tr>
                        <th data-options="field:'KeyId',hidden:true">KeyId</th>
                        <th data-options="field:'StandardId',hidden:true">StandardId</th>
				        <th data-options="field:'StandardName',width:80">标准名称</th>
                        <th data-options="field:'UserName',width:80">创建人</th>
                        <th data-options="field:'CreateTime',width:140">创建时间</th>
			        </tr>
		        </thead>
	        </table>
            <!-- 引领表工具栏开始 -->
            <div id="tbKpiList" style="padding:2px 5px;">
                <table>
                    <tr>
                        <td>
                            <select id="drpStatisticalMethod" class="easyui-combobox" data-options="editable:false,panelHeight:'auto',onSelect:OnStatisticalMethodChanged">
                                <option value="ComprehensiveEnergyConsumption">综合能耗</option>
                                <option value="EntityEnergyConsumption">实物能耗</option>
                            </select>
                        </td>
                        <td><a href="javascript:void(0)" class="easyui-linkbutton" onclick="QueryList()" data-options="iconCls:'icon-search',plain:true">查询</a></td>
                        <td><div class="datagrid-btn-separator"></div></td>
                        <td><a id="add_list" href="javascript:void(0)" class="easyui-linkbutton" onclick="CreateKpiList()" data-options="iconCls:'icon-add',plain:true">添加</a></td>
                        <td><a id="delete_list" href="javascript:void(0)" class="easyui-linkbutton" onclick="DeleteList()" data-options="iconCls:'icon-remove',plain:true">删除</a></td>
                    </tr>
                </table>      
            </div>
            <!-- 引领表工具栏结束 -->
        </div>
        <!-- 引领表结束 -->
        <div data-options="region:'center',border:false">
            <div id="mainLayout" class="easyui-layout" data-options="fit:true,border:false">
                <div data-options="region:'north',border:true" style="height:41px;">
                    <div style="padding-top:3px;padding-left:8px;background-color:#FAFAFA">
                        <table>
                            <tr>
                                <td>
                                    当前指标：
                                    <input id="txtCurrentKpi" class="easyui-textbox" data-options="editable:false" style="width:150px;" />
                                    <input id="txtCurrentKeyId" type="text" value="" style="display:none" />
                                </td>
                                <td>
                                    <select id="drpDisplayType" class="easyui-combobox" data-options="editable:false,panelHeight:'auto',onSelect:OnDisplayTypeChanged">
                                        <option value="All">全部</option>
                                        <option value="Public">公共</option>
                                        <option value="Private">私有</option>
                                    </select>
                                </td>
                                <!--<td><a href="javascript:void(0)" class="easyui-linkbutton" onclick="QueryDetail()" data-options="iconCls:'icon-search',plain:true">查询</a></td>-->
                                <td><div class="datagrid-btn-separator"></div></td>
                                <td><a id="add_detail" href="javascript:void(0)" class="easyui-linkbutton" onclick="AddKpiDetail()" data-options="iconCls:'icon-add',plain:true">添加</a></td>
                                <td><a id="delete_detail" href="javascript:void(0)" class="easyui-linkbutton" onclick="DeleteDetail()" data-options="iconCls:'icon-remove',plain:true">删除</a></td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div id="pnlOrganization" class="easyui-panel queryPanel" data-options="region:'west',border:false,closed:true" style="width: 230px;">
                    <uc1:OrganisationTree runat="server" ID="OrganisationTree" />
                </div>
                <div data-options="region:'center'">
	                <table id="dgKpiDetail" class="easyui-datagrid" data-options="rownumbers:true,singleSelect:true,fill:true,onDblClickRow:OnDgKpiDetailClicked" style="width:100%">
		                <thead>
			                <tr>
                                <th data-options="field:'StandardItemId',hidden:true">StandardItemId</th>
                                <th data-options="field:'OrganizationID',hidden:true">OrganizationID</th>
                                <th data-options="field:'OrganizationName',width:80">组织机构名称</th>
				                <th data-options="field:'Name',width:100">项目名称</th>
                                <th data-options="field:'LevelType',width:80">类型</th>
                                <th data-options="field:'VariableId',width:80">变量</th>
                                <th data-options="field:'ValueType',width:80">值类型</th>
                                <th data-options="field:'Unit',width:80">单位</th>
                                <th data-options="field:'StandardValue',width:80">标准值</th>
                                <th data-options="field:'StandardLevel',width:80">标准层次</th>
                                <th data-options="field:'UserName',width:80">创建人</th>
                                <th data-options="field:'CreateTime',width:140">创建时间</th>
			                </tr>
		                </thead>
	                </table>
                </div>
            </div>
        </div>
    </div>
    <!-- KPI指标体系添加窗口开始 -->
    <div id="dlgKpiListEditor" class="easyui-dialog" title="KPI指标体系" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false,buttons:'#bbKpiListEditor'" style="width:400px;height:auto;padding:10px 60px 20px 60px">
	    <table>
	    	<tr>
	    		<td>指标体系：</td>
                <td>
                    <select id="drpStandard" class="easyui-combobox" data-options="editable:false,panelHeight:'auto'">
                        <option value="InternationalStandard">国际标准</option>
                        <option value="NationalStandard">国家标准</option>
                        <option value="IndustryStandard">行业标准</option>
                        <option value="EnterpriseStandard">企业标准</option> 
                    </select>
	    		</td>
	    	</tr>
	    	<tr>
	    		<td>显示顺序：</td>
	    		<td><input class="easyui-textbox" type="text" id="txtDisplayIndex" data-options="required:true" style="width:160px" /></td>
	    	</tr>
	    </table>
    </div>
	<div id="bbKpiListEditor">
	    <a id="save_list" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="KpiListSave()">保存</a>
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#dlgKpiListEditor').dialog('close');">取消</a>
	</div>
    <!-- KPI指标体系添加窗口结束 -->
    <!-- KPI详细添加窗口开始 -->
    <div id="dlgKpiDetailEditor" class="easyui-dialog" title="KPI指标详细" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false,buttons:'#bbKpiDetailEditor'" style="width:400px;height:auto;padding:10px 60px 20px 60px">
	    <table>
	    	<tr>
	    		<td>组织机构：</td>
                <td><input class="easyui-combobox" type="text" id="drpOrganization" data-options="required:true" style="width:160px" /></td>
	    	</tr>
	    	<tr>
	    		<td>项目名称：</td>
                <td><input class="easyui-textbox" type="text" id="txtName" data-options="required:true" style="width:160px" /></td>
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
	    		<td>变量ID：</td>
                <td><input class="easyui-textbox" type="text" id="txtVariableId" data-options="required:true" style="width:160px" /></td>
	    	</tr>
	    	<tr>
	    		<td>值类型：</td>
	    		<td>
                    <select id="drpValueType" class="easyui-combobox" data-options="editable:false,panelHeight:'auto'">
                        <option value="ElectricityConsumption">电耗</option>
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
                <td><input class="easyui-textbox" type="text" id="txtStandardValue" data-options="required:true" style="width:160px" /></td>
	    	</tr>
	    	<tr>
	    		<td>标准层次：</td>
                <td><input class="easyui-textbox" type="text" id="txtStandardLevel" data-options="required:true" style="width:160px" /></td>
	    	</tr>
	    </table>
    </div>
	<div id="bbKpiDetailEditor">
	    <a id="save_detail" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="KpiDetailSave()">保存</a>
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#dlgKpiDetailEditor').dialog('close');">取消</a>
	</div>
    <!-- KPI详细添加窗口结束 -->
    <form id="form1" runat="server"></form>
</body>
</html>
