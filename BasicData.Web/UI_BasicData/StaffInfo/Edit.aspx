<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="BasicData.Web.UI_BasicData.StaffInfo.Edit" %>
<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>职工信息维护</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/jquery.utility.js"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false" style="padding: 5px;">
        <!-- 左侧目录树开始 -->
        <div data-options="region:'west',border:false,collapsible:false" style="width: 150px;">
            <uc1:OrganisationTree runat="server" ID="OrganisationTree" />
        </div>
        <!-- 左侧目录树结束 -->
        <!-- 中央区域开始 -->
        <div data-options="region:'center',border:false,collapsible:false" style="padding-left:10px;">
            <!-- 工具栏开始 -->
            <div id="toolbar_StaffInfo" style="display: none;">
                <table>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>当前分厂：</td>
                                    <td><input id="organizationName" class="easyui-textbox" readonly="readonly" style="width:80px" /></td>
                                    <%--<td style="width:10px;"></td>--%>
                                    <td>姓名：</td>
                                    <td><input id="searchName" class="easyui-textbox" style="width:80px" /></td>
                                    <%--<td style="width:10px;"></td>--%>
                                    <td>工号：</td>
                                    <td><input id="searchId" class="easyui-textbox" style="width:100px" /></td>
                                    <%--<td style="width:10px;"></td>--%>
                                    <td>班组：</td>
                                    <td><input id="searchTeamName" class="easyui-textbox" style="width:80px" /></td>
                                    <td style="width:10px;"></td>
                                    <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="loadStaffInfo()">搜索</a></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="loadStaffInfo();">刷新</a></td>
                                    <td><div class="datagrid-btn-separator"></div></td>
                                    <td><a id="id_add" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="editStaffInfo(true,'');">添加新职工</a></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <!-- 工具栏结束 -->
            <table id="dgStaffInfo" class="easyui-datagrid" data-options="idField:'StaffInfoID',toolbar:'#toolbar_StaffInfo',rownumbers:true,singleSelect:true,pagination:true,pageSize:10"" title="" style="width:100%;height:100%">
		        <thead>
			        <tr>
                        <th data-options="field:'StaffInfoItemId',width:150, hidden:true">唯一标识</th>
                        <th data-options="field:'StaffInfoID',width:150">工号</th>
				        <th data-options="field:'WorkingTeamName',width:150">所属班组</th>
                        <th data-options="field:'Name',width:150">姓名</th>
				        <th data-options="field:'Sex',formatter:formatSexColumn,width:50">性别</th>
                        <th data-options="field:'PhoneNumber',width:150">电话号码</th>
                        <th data-options="field:'Enabled',hidden:true">启用标志</th>
                        <th data-options="field:'OperateColumn',formatter:formatOperateColumn,width:200">操作</th>
			        </tr>
		        </thead>
            </table>
            <!-- 编辑窗口开始 -->
            <div id="staffInfoEditor" class="easyui-window" title="职工信息" data-options="modal:true,closed:true,iconCls:'icon-edit',minimizable:false,maximizable:false,collapsible:false,resizable:false" style="width:400px;height:auto;padding:10px 60px 20px 60px">
	    	    <table>
	    		    <tr>
	    			    <td>工号：</td>
	    			    <td><input class="easyui-textbox" type="text" id="id" name="id" data-options="required:true" style="width:160px" /></td>
	    		    </tr>
	    		    <tr>
	    			    <td>姓名：</td>
	    			    <td><input class="easyui-textbox" type="text" id="name" name="name" data-options="required:true" style="width:160px" /></td>
	    		    </tr>
	    		    <tr>
	    			    <td>性别：</td>
	    			    <td>
	    				    <select class="easyui-combobox" id="sex" name="sex" style="width:160px"><option value="True">男</option><option value="False">女</option></select>
	    			    </td>
	    		    </tr>
	    		    <tr>
	    			    <td>班组：</td>
	    			    <td>
                            <select class="easyui-combobox" id="workingTeam" name="workingTeam" style="width:160px"><option value="A班">A班</option><option value="B班">B班</option><option value="C班">C班</option><option value="D班">D班</option><option value="常白">常白班</option></select>
	    			    </td>
	    		    </tr>
	    		    <tr>
	    			    <td>手机号码：</td>
	    			    <td><input class="easyui-textbox" id="phoneNumber" name="phoneNumber" data-options="" style="width:160px" /></td>
	    		    </tr>
	    		    <tr>
	    			    <td>启用：</td>
	    			    <td>
                            <select class="easyui-combobox" id="enabled" name="enabled" style="width:160px"><option value="True">启用</option><option value="False">停用</option></select>
	    			    </td>
	    		    </tr>
	    	    </table>
	            <div style="text-align:center;padding:5px;margin-left:-18px;">
	    	        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="save()">保存</a>
	    	        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="$('#staffInfoEditor').window('close');">取消</a>
	            </div>
            </div>
            <!-- 编辑窗口开始 -->
        </div>
        <!-- 中央区域结束 -->
    </div>
    <script type="text/javascript">
        var StaffInfoItemId = "";
        $(function () {
           initPageAuthority();
        })

        function formatSexColumn(val, row) {
            return val == 'True' ? "男" : "女";
        }

        function formatOperateColumn(val, row) {
            return '<a href="#" onclick="editStaffInfo(false, \'' + row.StaffInfoItemId + '\');">编辑</a>&nbsp;&nbsp;<a href="#" onclick="deleteStaffInfo(\'' + row.StaffInfoItemId + '\');">删除</a>';
        }

        // 分厂ID变量
        var organizationId = '';
        var authArray;
        //初始化页面的增删改查权限
        function initPageAuthority() {
            $.ajax({
                type: "POST",
                url: "Edit.aspx/AuthorityControl",
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,//同步执行
                success: function (msg) {
                     authArray = msg.d;
                    //增加
                    if (authArray[1] == '0') {
                        $("#id_add").linkbutton('disable');
                    }
                    //修改
                    //if (authArray[2] == '0') {
                    //    $("#edit").linkbutton('disable');
                    //}
                    ////删除
                    //if (authArray[3] == '0') {
                    //    $("#delete").linkbutton('disable');
                    //}
                }
            });
        }
        // 目录树选择事件
        function onOrganisationTreeClick(node) {
            // 仅分厂级目录有效
            if (node.id.length != 5) {
                $.messager.alert('说明', '请选择分厂级别的节点。', 'info');
                return;
            }

            // 更新当前分厂名
            $('#organizationName').textbox('setText', node.text);
            organizationId = node.OrganizationId;

            // 获取职工信息
            loadStaffInfo();
        }

        // 获取职工列表
        function loadStaffInfo() {
            var queryUrl = 'Edit.aspx/GetStaffInfoWithDataGridFormat';
            var searchName = $('#searchName').textbox('getText');
            var searchId = $('#searchId').textbox('getText');
            var searchTeamName = $('#searchTeamName').textbox('getText');
            var dataToSend = '{organizationId: "' + organizationId + '", searchName: "' + searchName + '", searchId: "' + searchId + '", searchTeamName: "' + searchTeamName + '"}';

            var win = $.messager.progress({
                title: '请稍后',
                msg: '数据载入中...'
            });

            $.ajax({
                type: "POST",
                url: queryUrl,
                data: dataToSend,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    initializeStaffInfo(jQuery.parseJSON(msg.d));
                    $.messager.progress('close');
                },
                error: function () {
                    $.messager.progress('close');
                    $.messager.alert('错误', '数据载入失败！');
                }
            });
        }

        // 初始职工列表
        function initializeStaffInfo(jsonData) {
            $('#dgStaffInfo').datagrid({
                data: jsonData,
                dataType: "json"
            });
        }

        var isStaffInfoInsert = false;
        // 编辑员工信息
        function editStaffInfo(isInsert, myStaffInfoItemId) {
            if (authArray[2] == '0') {
                $.messager.alert("提示","该用户没有编辑权限！");
                return;
            }
            if (isInsert) {
                isStaffInfoInsert = true;
                //$('#id').textbox('readonly', false);
                StaffInfoItemId = '';
                $('#id').textbox('setText', '');
                $('#name').textbox('setText', '');
                $('#sex').combobox('select', 'True');
                $('#workingTeam').combobox('select', 'A班');
                $('#phoneNumber').textbox('setText', '');
                $('#enabled').combobox('select', 'True');
            }
            else {
                isStaffInfoInsert = false;
                StaffInfoItemId = myStaffInfoItemId;
                $('#dgStaffInfo').datagrid('selectRecord', StaffInfoItemId);
                var data = $('#dgStaffInfo').datagrid('getSelected');
                //$('#id').textbox('readonly', true);
                $('#id').textbox('setText', data.StaffInfoID);
                $('#name').textbox('setText', data.Name);
                $('#sex').combobox('select', data.Sex);
                $('#workingTeam').combobox('select', data.WorkingTeamName);
                $('#phoneNumber').textbox('setText', data.PhoneNumber);
                $('#enabled').combobox('select', data.Enabled);
            }

            $('#staffInfoEditor').window('open');
        }

        function save() {
            saveStaffInfo();
        }

        // 保存职工信息
        function saveStaffInfo() {
            var queryUrl;
            if (isStaffInfoInsert)
                queryUrl = 'Edit.aspx/InsertStaffInfo';
            else
                queryUrl = 'Edit.aspx/UpdateStaffInfo';

            var id = $('#id').textbox('getText');
            var name = $('#name').textbox('getText');
            var sex = $('#sex').combobox('getValue');
            var workingTeam = $('#workingTeam').combobox('getValue');
            var phoneNumber = $('#phoneNumber').textbox('getText');
            var enabled = $('#enabled').combobox('getValue');


            var dataToSend = '{organizationId: "' + organizationId + '", staffInfoItemId: "' + StaffInfoItemId + '", staffId: "' + id + '", name: "' + name + '", sex: "' + sex + '", workingTeam: "' + workingTeam + '", phoneNumber: "' + phoneNumber + '", enabled: "' + enabled + '"}';

            $.ajax({
                type: "POST",
                url: queryUrl,
                data: dataToSend,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    $.messager.alert('消息', msg.d);
                    $('#staffInfoEditor').window('close');
                    loadStaffInfo();
                },
                error: function () {
                    $.messager.progress('close');
                    $.messager.alert('错误', '数据保存失败！');
                }
            });
        }
        function deleteStaffInfo(myStaffInfoItemId) {
            if (authArray[3] == "0") {
                $.messager.alert("提示", "该用户没有删除权限！");
                return;
            }
            parent.$.messager.confirm('询问', '您确定要删除该员工的信息?', function (r) {
                if (r) {
                    StaffInfoItemId = '';
                    queryUrl = 'Edit.aspx/DeleteStaffInfo';
                    var dataToSend = '{myStaffInfoItemId: "' + myStaffInfoItemId + '"}';
                    $.ajax({
                        type: "POST",
                        url: queryUrl,
                        data: dataToSend,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (msg) {
                            loadStaffInfo();
                        },
                        error: function () {
                            $.messager.progress('close');
                            $.messager.alert('错误', '数据保存失败！');
                        }
                    });
                }
            });
        }
    </script>

    <form id="form1" runat="server"></form>
</body>
</html>
