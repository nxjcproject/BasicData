<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="BasicData.Web.UI_BasicData.MachineHaltReasons.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>停机原因分类</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/jquery.utility.js"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="../js/page/MachineHaltReasons/Edit.js" charset="utf-8"></script>
</head>
<body>
    <div id="wrapper" class="easyui-panel" style="width: 98%; height: auto; padding: 2px;">
        <div class="easyui-panel" style="width: 100%; padding: 5px; margin-bottom: 5px;">
            <a id="save" href="javascript:void(0)" class="easyui-linkbutton c4 easyui-tooltip tooltip-f" data-options="plain:true,iconCls:'icon-ok'" title="保存。" onclick="saveMachineHaltReasons()">保存</a>
        </div>

        <table id="tgMachineHaltReasonsEditor" class="easyui-treegrid" title="停机原因维护" style="width: 100%; height: 450px;">
        </table>
        <div class="easyui-panel" style="width: 100%; padding: 5px; margin-top: 5px;">
            <a id="addOneReason" href="javascript:void(0)" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-add'" onclick="appendRoot()">添加一级停机原因</a>
        </div>

        <!-- 右键菜单开始 -->
        <div id="mm" class="easyui-menu" style="width: 120px;">
            <div id="add" onclick="append()" data-options="iconCls:'icon-add'">添加</div>
            <div id="remove" onclick="removeIt()" data-options="iconCls:'icon-remove'">删除</div>
            <div class="menu-sep"></div>
            <div id="addReason" onclick="appendRoot()" data-options="iconCls:'icon-add'">添加一级停机原因</div>
            <div class="menu-sep"></div>
            <div onclick="collapse()">收起</div>
            <div onclick="expand()">展开</div>
        </div>
        <!-- 右键菜单结束 -->
    </div>
    <form id="form1" runat="server"></form>
</body>
</html>
