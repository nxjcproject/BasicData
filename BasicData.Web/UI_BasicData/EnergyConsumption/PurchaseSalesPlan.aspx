﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PurchaseSalesPlan.aspx.cs" Inherits="BasicData.Web.UI_BasicData.EnergyConsumption.PurchaseSalesPlan" %>
<%@ Register Src="../../UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>采购与销售计划</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/extend/editCell.js" charset="utf-8"></script>

    <script type="text/javascript" src="../js/page/EnergyConsumption/PurchaseSalesPlan.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',border:false " style="width: 150px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
        <div id="toolbar_PurchaseSalesPlanInfo" style="display: none;">
            <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td style="width: 50px; text-align: right;">选择年份</td>
                                <td>
                                    <input id="numberspinner_PlanYear" class="easyui-numberspinner" data-options="min:1900,max:2999" style="width: 70px;" />
                                </td>
                                <td style="width: 60px; text-align: right;">生产区域</td>
                                <td>
                                    <input id="TextBox_OrganizationText" class="easyui-textbox" data-options="editable:false, readonly:true" style="width: 100px;" />
                                </td>
                                <td style="width: 30px; text-align: right;">类型</td>
                                <td>
                                    <select id="drpDisplayType" class="easyui-combobox" data-options="editable:false,panelHeight:'auto'" style="width:100px;">
                                        <option value="Sales">销售计划</option>
                                        <%--<option value="Purchase">采购计划</option>--%>
                                    </select>
                                </td>
                                <td>
                                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'"
                                        onclick="QueryPurchaseSalesPlanInfoFun();">查询</a>
                                </td>
                                <td>
                                    <input id="TextBox_OrganizationId" style="width: 10px; visibility: hidden;" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td><a href="#" id="id_save" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="SavePurchaseSalesPlanFun();">保存</a>
                                </td>
                                <td>
                                    <div class="datagrid-btn-separator"></div>
                                </td>
                                <td>
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="RefreshPurchaseSalesPlanFun();">刷新</a>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'center',border:false,collapsible:false">
            <table id="grid_PurchaseSalesPlanInfo" data-options="fit:true,border:true"></table>
        </div>
    </div>

    <form id="form_PurchaseSalesPlan" runat="server">
        <div>
            <asp:HiddenField ID="HiddenField_PlanType" runat="server" />
        </div>
    </form>
</body>
</html>
