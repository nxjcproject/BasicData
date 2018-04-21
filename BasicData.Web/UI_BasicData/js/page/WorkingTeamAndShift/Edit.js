$(function () {
    //var data = [{ "Shifts": "甲班" }, { "Shifts": "乙班" }, { "Shifts": "丙班" }];
    //shiftInitializeGrid(data);
    //var workingteamData = [{ "Name": "A组" }, { "Name": "B组" }, { "Name": "C组" }, { "Name": "D组" }];
    //workingteamInitializeGrid(workingteamData);
    InitializePage();
    initPageAuthority();
});

var publicData = {
    organizationId:"",
    editIndex:"",
    editRow: {},
    comboboxValue:[]
};
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
            var authArray = msg.d;
            //增加
            //if (authArray[1] == '0') {
            //    $("#add").linkbutton('disable');
            //}
            //修改
            if (authArray[2] == '0') {
                $("#id_shiftEdit").linkbutton('disable');
                $("#id_shiftSave").linkbutton('disable');
                $("#id_shiftReset").linkbutton('disable');
                $("#id_workingTeamEdit").linkbutton('disable');
                $("#id_workingTeamSave").linkbutton('disable');
                $("#id_workingTeamReset").linkbutton('disable');
            }
            ////删除
            //if (authArray[3] == '0') {
            //    $("#delete").linkbutton('disable');
            //}
        }
    });
}
function InitializePage() {
    loadChargeManData();
    loadShiftsData();
    loadWorkingTeamData();
}

//function onOrganisationTreeClick(node) {
//    publicData.organizationId = node.OrganizationId;
//    InitializePage();
//}
function onOrganisationTreeClick(node) {
    publicData.organizationId = node.OrganizationId;
    $('#organizationName').textbox('setText', node.text);
    if (publicData.organizationId=="" || publicData.organizationId.length==5) {
        alert('请选择分厂组织机构')
        return;
    }
    InitializePage();
}

function loadShiftsData() {
    $.ajax({
        type: "POST",
        url: "Edit.aspx/QueryShifts",
        data: "{organizationId:'" + publicData.organizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);;
            if (myData.rows.length == 0) {
                myData = { "total": 3, "rows": [{ "Shifts": "甲班" }, { "Shifts": "乙班" }, { "Shifts": "丙班" }] };
            }
            else {
                //myData = jQuery.parseJSON(msg.d);
                for (var i = myData.rows.length - 1; i >= 0; i--) {
                    myData.rows[i]["Flag"] = "True";
                }
            }
            shiftInitializeGrid(myData);
        }
    });
}
function loadWorkingTeamData() {
    $.ajax({
        type: "POST",
        url: "Edit.aspx/QueryWorkingTeam",
        data: "{organizationId:'" + publicData.organizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            if (myData.rows.length == 0) {
                myData = { "total": 4, "rows": [{ "Name": "A班" }, { "Name": "B班" }, { "Name": "C班" }, { "Name": "D班" }, { "Name": "常白"}] };
            }
            else {
                myData = jQuery.parseJSON(msg.d);
                for (var i = myData.rows.length - 1; i >= 0; i--) {
                    myData.rows[i]["Flag"] = "True";
                }
            }
            workingteamInitializeGrid(myData);
        }
    });
}

function loadChargeManData() {
    $.ajax({
        type: "POST",
        url: "Edit.aspx/GetChargeManComboboxValue",
        data: "{organizationId:'" + publicData.organizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            publicData.comboboxValue = jQuery.parseJSON(msg.d);
            for (var i = publicData.comboboxValue.length - 1; i >= 0; i--) {
                publicData.comboboxValue[i]["Name"] = publicData.comboboxValue[i]["Name"] + "(" + publicData.comboboxValue[i]["StaffInfoID"] + ")";
            }
            //InitializeChargeManCombobox(publicData.comboboxValue);
        }
    });
}

//function InitializeChargeManCombobox(mydata) {
//    $('#chargeMan').combobox({
//        data: mydata,
//        valueField: 'StaffInfoID',
//        textField: 'Name'
//    });
//}

/***********************************************************************************************************************/
function shiftReload() {
    $.messager.defaults = { ok: "是", cancel: "否" };
    $.messager.confirm('提示', '确定要重置？', function (r) {
        if (r) {
            var data = [{ "Flag": "false", "Shifts": "甲班" }, { "Flag": "false", "Shifts": "乙班" }, { "Flag": "false", "Shifts": "丙班" }];
            $('#dg_shift').datagrid('loadData', data);
        }
    });
}

function shiftInitializeGrid(myData) {
    $('#dg_shift').datagrid({
        data: myData,
        iconCls: 'icon-save', singleSelect: true, rownumbers: true, striped: true, toolbar: '#tb_shift',
        columns: [[
            {
                field: 'Flag', title: '启用标志', width: '10%', align: 'center',
                formatter: function (value) {
                    //return "<input type=\"checkbox\" disabled=\"true\" checked=\"" + "false" + "\" >";
                    if (value == true || value == "True")
                        return "<input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" />";
                    else if (value == false || value == "False")
                        return "<input type=\"checkbox\" disabled=\"disabled\" />";
                    else
                        return "<input type=\"checkbox\" disabled=\"disabled\" />";
                }
            },
            { field: 'Shifts', title: '班次名称', width: '10%', align: 'center' },
            { field: 'StartTime', title: '起始时间', width: '22%', align: 'center' },
            { field: 'EndTime', title: '终止时间', width: '22%', align: 'center' },
            { field: 'Description', title: '描述', width: '30%', align: 'center' },
            { field: 'CreatedDate', title: '创建时间', hidden:true, width: '0%', align: 'center' }
        ]]
    });
}

function shiftEditItem() {
    var row2 = $("#dg_shift").datagrid('getSelected');
    publicData.editRow = row2;
    publicData.editIndex = $("#dg_shift").datagrid('getRowIndex', row2);
    if (row2 == null) {
        alert('请选中一行数据！');
    }
    else {
        if (publicData.editIndex==2  && (Object.keys($("#dg_shift").datagrid('getRows')[1]).length==1)) {
            alert('请先编辑乙班时间');
            return;
        }
        if (publicData.editIndex == 1 && (Object.keys($("#dg_shift").datagrid('getRows')[0]).length == 1)) {
            alert('请先编辑甲班时间');
            return;
        }
        if (publicData.editIndex > 0) {
            var row = $("#dg_shift").datagrid('getRows')[publicData.editIndex - 1];
            var row1 = $("#dg_shift").datagrid('getRows')[0];
            $('#startTime').timespinner('setValue', row["EndTime"]);
            $('#startTime').timespinner('readonly', true);
            $('#endTime').timespinner('setValue', row2["EndTime"]);
            if (publicData.editIndex == 2) {
                $('#endTime').timespinner('readonly', true);
                $('#endTime').timespinner('setValue', row1["StartTime"]);

            }
            else {
                $('#endTime').timespinner('readonly', false);
            }
        }
        else {
            $('#startTime').timespinner('readonly', false);
            $('#endTime').timespinner('readonly', false);
            $('#startTime').timespinner('setValue', row2["StartTime"]);
            $('#endTime').timespinner('setValue', row2["EndTime"]);
        }
        $('#name').val(row2["Shifts"]);
        $('#shiftEditDialog').dialog('open');
    }
}
function shiftSaveEditDialog() {
    publicData.editRow["StartTime"] = $('#startTime').timespinner('getValue');
    publicData.editRow["EndTime"] = $('#endTime').timespinner('getValue');
    publicData.editRow["Description"] = $('#description').textbox('getText');
    publicData.editRow["Flag"] = $("input[name='radiobutton']:checked").val();
    $('#dg_shift').datagrid('updateRow', {
        index: publicData.editIndex,
        row: publicData.editRow
    });
    $('#shiftEditDialog').dialog('close');
}
//function shiftEditDialog() {
//    var startTime = $('#startTime').timespinner('getValue');
//    var endTime = $('#endTime').timespinner('getValue');
//    publicData.editRow["StartTime"] = startTime
//    publicData.editRow["EndTime"] = endTime
//    publicData.editRow["Description"] = $('#description').textbox('getText');
//    publicData.editRow["Flag"] = $("input[name='radiobutton']:checked").val();
//    $('#dg_shift').datagrid('updateRow', {
//        index: publicData.editIndex,
//        row: publicData.editRow
//    });
//    $('#shiftEditDialog').dialog('close');
//}

function shiftSave() {
    var rows = $("#dg_shift").datagrid('getRows');
    for (var i = rows.length-1; i >= 0; i--) {
        if (rows[i]["Flag"] != "True")
            rows.pop();
    }
    var myjson = {
        rows: rows
    };
    $.ajax({
        type: "POST",
        url: "Edit.aspx/SaveShifts",
        data: "{organizationId:'"+ publicData.organizationId +"', shifts:'" + JSON.stringify(myjson) + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == "1") {
                $('#save').dialog('close');
                alert("更新成功!");
            }
            else if(msg.d=="noright"){
                $('#save').dialog('close');
                alert("用户没有修改权限!");
            }
            else {
                $('#save').dialog('close');
                alert("更新失败!");
            }
        }
    });
}

function validateTime(startTime, endTime) {
    var start = startTime.split(':');
    var end = endTime.split(':');
    var startValue = parseInt(start[0]) * 60 + parseInt(start[1]);
    var endValue = parseInt(end[0]) * 60 + parseInt(end[1]);
    if (startValue >= endValue)
        return -1;
    else
        return 1;
}


/****************************************************************************************************************************************/

function workingteamReload() {
    $.messager.defaults = { ok: "是", cancel: "否" };
    $.messager.confirm('提示', '确定要重置？', function (r) {
        if (r) {
            var data = [{ "Name": "A班" }, { "Name": "B班" }, { "Name": "C班" }, { "Name": "D班" }, { "Name": "常白"} ];
            $('#dg_workingteam').datagrid('loadData', data);
        }
    });
}

function workingteamInitializeGrid(myData) {
    $('#dg_workingteam').datagrid({
        data: myData,
        iconCls: 'icon-save', singleSelect: true, rownumbers: true, striped: true, toolbar: '#tb_workingteam',
        columns: [[
            {
                field: 'Flag', title: '启用标志', width: '10%', align: 'center',
                formatter: function (value) {
                    //return "<input type=\"checkbox\" disabled=\"true\" checked=\"" + value + "\" >";
                    if (value == true || value == "True")
                        return "<input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" />";
                    else if (value == false || value == "False")
                        return "<input type=\"checkbox\" disabled=\"disabled\" />";
                    else
                        return "<input type=\"checkbox\" disabled=\"disabled\" />";
                }
            },
            { field: 'Name', title: '班组名称', width: '10%', align: 'center' },
            {
                field: 'ChargeManID', title: '负责人', width: '30%', align: 'center',
                formatter: function(value){
                    var result;
                    for (var i = 0; i < publicData.comboboxValue.length; i++) {
                        if (publicData.comboboxValue[i].StaffInfoID == value)
                            result = publicData.comboboxValue[i].Name;
                    }
                    return result;
                }
            },
            { field: 'Remarks', title: '备注', width: '45%', align: 'center' }
        ]]
    });
}

function shifteditItem() {
    var row = $("#dg_workingteam").datagrid('getSelected');
    publicData.editRow = row;
    publicData.editIndex = $("#dg_workingteam").datagrid('getRowIndex', row);
    if (row == null) {
        alert('请选中一行数据！');
    }
    else {
        if (publicData.editIndex == 2 && (Object.keys($("#dg_workingteam").datagrid('getRows')[1]).length == 1)) {
            alert('请先编辑B组负责人');
            return;
        }
        if (publicData.editIndex == 1 && (Object.keys($("#dg_workingteam").datagrid('getRows')[0]).length == 1)) {
            alert('请先编辑A组负责人');
            return;
        }
        if (publicData.editIndex == 3 && (Object.keys($("#dg_workingteam").datagrid('getRows')[2]).length == 1)) {
            alert('请先编辑C组负责人');
            return;
        }
        if (publicData.editIndex == 4 && (Object.keys($("#dg_workingteam").datagrid('getRows')[3]).length == 1)) {
            alert('请先编辑D组负责人');
            return;
        }
        var name = row["Name"];
        var comboboxItem = [];
        for (var i = publicData.comboboxValue.length - 1; i >= 0; i--) {
            if (publicData.comboboxValue[i]["WorkingTeamName"] == name) {               
                comboboxItem.push(publicData.comboboxValue[i]);
            }
        }
        $('#chargeMan').combobox({
            data: comboboxItem,
            valueField: 'StaffInfoID',
            textField: 'Name'
        });
        $('#workingteamName').val(name);
        $('#workingteamEditDialog').dialog('open');
    }
}

function workingteamSaveEditDialog() {
    publicData.editRow["ChargeManID"] = $('#chargeMan').combobox('getValue');
    publicData.editRow["Remarks"] = $('#remarks').val();
    publicData.editRow["Flag"] = $("input[name='radiobutton']:checked").val();
    $('#dg_workingteam').datagrid('updateRow', {
        index: publicData.editIndex,
        row: publicData.editRow
    });
    $('#workingteamEditDialog').dialog('close');
}

function workingteamSave() {
    var rows = $("#dg_workingteam").datagrid('getRows');
    for (var i = rows.length - 1; i >= 0; i--) {
        if (rows[i]["Flag"] != "True")
            rows.pop();
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "请选择一个班组！")
    }
    else {
        var myjson = {
            rows: rows
        };
        $.ajax({
            type: "POST",
            url: "Edit.aspx/SaveWorkingTeams",
            data: "{organizationId:'" + publicData.organizationId + "',workingTeams:'" + JSON.stringify(myjson) + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d == "1") {
                    $('#save').dialog('close');
                    alert("更新成功!");
                }
                else if (msg.d == "noright") {
                    $('#save').dialog('close');
                    alert("用户没有修改权限!");
                }
                else {
                    $('#save').dialog('close');
                    alert("更新失败!");
                }
            }
        });
    }   
}