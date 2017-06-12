<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShiftArrangementEdit.aspx.cs" Inherits="BasicData.Web.UI_BasicData.ShiftArrangement.ShiftArrangementEdit" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>排班计划录入</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint.js" charset="utf-8"></script>

    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script>

    <script type="text/javascript" src="/UI_BasicData/js/page/ShiftArrangement/ShiftArrangementEdit.js" charset="utf-8"></script>
</head>
<body>
    <div id="cc" class="easyui-layout" data-options="fit:true,border:false" style="padding: 5px;">
        <div data-options="region:'west',border:false,collapsible:false" style="width: 150px;">
            <uc1:OrganisationTree runat="server" ID="OrganisationTree" />
        </div>
        <div data-options="region:'center'">
            <div id="tb" style="padding:5px;">
                <div >
                    <label>当前产线名称：</label>
                    <input id="organizationName" class="easyui-textbox" style="width: 100px;" readonly="true" />
                    <input id="organizationId" readonly="true" style="display: none;" />
                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-undo',plain:true" onclick="reject()">撤销</a>
                    <a id="id_save" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="saveShiftArrangement()">保存</a>
                </div>
            </div>
            <table id="dg" ></table>
        </div>
    </div>
</body>
</html>
