<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CoefficientUpdater.aspx.cs" Inherits="BasicData.Web.UI_BasicData.Material.CoefficientUpdater" %>
<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>物料系数修改</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/jquery.utility.js"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/UI_BasicData/js/page/Material/CoefficientUpdater.js" charset="utf-8"></script>
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
	                <table id="dgMaterial" class="easyui-datagrid" data-options="rownumbers:true,singleSelect:true,onDblClickRow:OnDgMaterialClicked" style="width:100%;height:100%;">
		                <thead>
			                <tr>
                                <th data-options="field:'KeyID',hidden:true">KeyID</th>
				                <th data-options="field:'Name',width:140">名称</th>
                                <th data-options="field:'CreatedDate',width:80">创建时间</th>
			                </tr>
		                </thead>
	                </table>
                </div>
                <div data-options="region:'center'" title="物料详细">
	                <table id="dgMaterialDetail" class="easyui-datagrid" data-options="rownumbers:true,singleSelect:true,toolbar:'#tbMaterialDetail',onClickRow:OnClickRow" style="width:100%;height:100%;">
		                <thead>
			                <tr>
                                <th data-options="field:'MaterialId',hidden:true">MaterialId</th>
				                <th data-options="field:'Name',width:140">名称</th>
                                <th data-options="field:'VariableId',width:140">变量ID</th>
                                <th data-options="field:'Unit',width:40">单位</th>
                                <th data-options="field:'Coefficient',width:80,editor:{type:'numberbox',options:{min:0,max:99,precision:4}}">系数</th>
			                </tr>
		                </thead>
	                </table>
                </div>
            </div>
        </div>
    </div>
    <!-- 物料详细工具栏开始 -->
    <div id="tbMaterialDetail" style="padding:2px 5px;">
        <table>
            <tr>
                <td>当前：<input id="currentMaterialName" class="easyui-textbox" data-options="editable:false" style="width: 150px;" /><input id="currentMaterialKeyId" readonly="true" style="display: none;" /></td>
                <td><div class="datagrid-btn-separator"></div></td>
                <td><a id="save" href="javascript:void(0)" class="easyui-linkbutton" onclick="SaveMaterialDetail()" data-options="iconCls:'icon-save',plain:true">保存</a></td>
            </tr>
        </table>      
    </div>
    <!-- 物料详细工具栏开结束 -->
    <form id="form1" runat="server"></form>
</body>
</html>
