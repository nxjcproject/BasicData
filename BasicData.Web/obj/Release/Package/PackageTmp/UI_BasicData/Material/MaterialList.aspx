<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaterialList.aspx.cs" Inherits="BasicData.Web.UI_BasicData.Material.MaterialList" %>
<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>物料信息列表</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/jquery.utility.js"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/UI_BasicData/js/page/Material/MaterialList.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false" style="padding: 5px;">
        <!-- 左侧目录树开始 -->
        <div data-options="region:'west',border:false,collapsible:false" style="width: 230px;">
            <uc1:OrganisationTree runat="server" ID="OrganisationTree" />
        </div>
        <div data-options="region:'center',border:false">
            <div class="easyui-layout" data-options="fit:true,border:false">
               <div class="easyui-panel queryPanel" data-options="region:'north', border:true, collapsible:false, split:false" style="height: 50px;">
                    <table>
                        <tr><td style="height:5px;"></td></tr>
                        <tr>
                            <td>组织机构：</td>
                            <td>
                                <input id="txtOrganization" class="easyui-textbox" data-options="editable:false" style="width: 150px;" /><input id="organizationId" readonly="true" style="display: none;" /></td>
                            <td><div class="datagrid-btn-separator"></div></td>
                            <td><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Query();">查询</a></td>
                        </tr>
                    </table>
                </div>
                <div data-options="region:'west'" title="物料信息列表" style="width:250px;">
	                <table id="dgMaterial" class="easyui-datagrid" data-options="rownumbers:true,singleSelect:true,toolbar:'#tbMaterialList',onDblClickRow:OnDgMaterialClicked" style="width:100%;height:100%;">
		                <thead>
			                <tr>
                                <th data-options="field:'KeyID',hidden:true">KeyID</th>
				                <th data-options="field:'Name',width:140">名称</th>
                                <th data-options="field:'CreatedDate',width:80">创建时间</th>
                                <!--<th data-options="field:'OperateColumn',formatter:MaterialListOperateColumnFormatter,width:80">操作</th>-->
			                </tr>
		                </thead>
	                </table>
                </div>
                <div data-options="region:'center'" title="物料详细">
	                <table id="dgMaterialDetail" class="easyui-datagrid" data-options="rownumbers:true,singleSelect:true,toolbar:'#tbMaterialDetail',onDblClickRow:OnDgMaterialDetailClicked" style="width:100%;height:100%;">
		                <thead>
			                <tr>
                                <th data-options="field:'MaterialId',hidden:true">MaterialId</th>
				                <th data-options="field:'Name',width:140">名称</th>
                                <th data-options="field:'VariableId',width:140">变量ID</th>
                                <th data-options="field:'Type',width:80,formatter:TypeFormatter">类型</th>
                                <th data-options="field:'Unit',width:40">单位</th>
                                <th data-options="field:'MaterialErpCode',width:80">ERP编码</th>
                                <th data-options="field:'TagTableName',width:80">表名</th>
                                <th data-options="field:'Formula',width:80">公式</th>
                                <th data-options="field:'Coefficient',width:80">系数</th>
                                <!--<th data-options="field:'OperateColumn',formatter:MaterialDetailOperateColumnFormatter,width:150">操作</th>-->
			                </tr>
		                </thead>
	                </table>
                </div>
            </div>
        </div>
    </div>
    <!-- 物料列表工具栏开始 -->
    <div id="tbMaterialList" style="padding:2px 5px;">
        <table>
            <tr>
                <td><a href="javascript:void(0)" class="easyui-linkbutton" onclick="CreateMaterialList()" data-options="iconCls:'icon-add',plain:true">添加</a></td>
                <td><a href="javascript:void(0)" class="easyui-linkbutton" onclick="DeleteMaterialList()" data-options="iconCls:'icon-remove',plain:true">删除</a></td>
            </tr>
        </table>      
    </div>
    <!-- 物料列表工具栏结束 -->
    <!-- 物料详细工具栏开始 -->
    <div id="tbMaterialDetail" style="padding:2px 5px;">
        <table>
            <tr>
                <td>当前：<input id="currentMaterialName" class="easyui-textbox" data-options="editable:false" style="width: 150px;" /><input id="currentMaterialKeyId" readonly="true" style="display: none;" /></td>
                <td><div class="datagrid-btn-separator"></div></td>
                <td><a href="javascript:void(0)" class="easyui-linkbutton" onclick="EditMaterialDetail()" data-options="iconCls:'icon-edit',plain:true">修改</a></td>
                <td><div class="datagrid-btn-separator"></div></td>
                <td><a href="javascript:void(0)" class="easyui-linkbutton" onclick="AddMaterialDetail()" data-options="iconCls:'icon-add',plain:true">添加</a></td>
                <td><a href="javascript:void(0)" class="easyui-linkbutton" onclick="DeleteMaterialDetail()" data-options="iconCls:'icon-remove',plain:true">删除</a></td>
            </tr>
        </table>      
    </div>
    <!-- 物料详细工具栏开结束 -->
    <!-- 物料列表添加窗口开始 -->
    <div id="dlgMaterialListEditor" class="easyui-dialog" title="物料" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false,buttons:'#bbMaterialListEditor'" style="width:400px;height:auto;padding:10px 60px 20px 60px">
	    <table>
	    	<tr>
	    		<td>名称：</td>
	    		<td><input class="easyui-textbox" type="text" id="listName" name="listName" data-options="required:true" style="width:160px" /></td>
	    	</tr>
	    </table>
    </div>
	<div id="bbMaterialListEditor">
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="MaterialListSave()">保存</a>
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#dlgMaterialListEditor').dialog('close');">取消</a>
	</div>
    <!-- 物料列表添加窗口结束 -->
    <!-- 物料详细编辑窗口开始 -->
    <div id="dlgMaterialDetailEditor" class="easyui-dialog" title="物料详细" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false,buttons:'#bbMaterialDetailEditor'" style="width:400px;height:auto;padding:10px 60px 20px 60px">
	    <table>
	    	<tr>
	    		<td>名称：</td>
	    		<td><input class="easyui-textbox" type="text" id="detailName" name="detailName" data-options="required:true" style="width:160px" /></td>
	    	</tr>
	    	<tr>
	    		<td>变量ID：</td>
	    		<td><input class="easyui-textbox" type="text" id="variableId" name="variableId" data-options="required:true" style="width:160px" /></td>
	    	</tr>
	    	<tr>
	    		<td>类型：</td>
	    		<td>
	    			<select class="easyui-combobox" id="type" name="type" data-options="panelHeight:'auto'" style="width:160px">
                        <option value="Coal">煤粉</option>
                        <option value="Clinker">熟料</option>
                        <option value="Cement">水泥</option>
	    			</select>
	    		</td>
	    	</tr>
	    	<tr>
	    		<td>单位：</td>
	    		<td><input class="easyui-textbox" id="unit" name="unit" style="width:160px" /></td>
	    	</tr>
	    	<tr>
	    		<td>ERP编码：</td>
	    		<td><input class="easyui-textbox" id="materialErpCode" name="materialErpCode" style="width:160px" /></td>
	    	</tr>
	    	<tr>
	    		<td>系数：</td>
	    		<td>
                    <input class="easyui-numberbox" data-options="min:0,max:99,precision:4" id="coefficient" name="coefficient" style="width:160px" />
	    		</td>
	    	</tr>
	    	<tr>
	    		<td>TagTableName：</td>
	    		<td>
                    <input class="easyui-textbox" id="tagTableName" name="tagTableName" style="width:160px" />
	    		</td>
	    	</tr>
	    	<tr>
	    		<td>Formula：</td>
	    		<td>
                    <input class="easyui-textbox" id="formula" name="formula" style="width:160px" />
	    		</td>
	    	</tr>
	    </table>
    </div>
	<div id="bbMaterialDetailEditor">
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="MaterialDetailSave()">保存</a>
	    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#dlgMaterialDetailEditor').dialog('close');">取消</a>
	</div>
    <!-- 物料详细编辑窗口结束 -->
    <form id="form1" runat="server"></form>
</body>
</html>
