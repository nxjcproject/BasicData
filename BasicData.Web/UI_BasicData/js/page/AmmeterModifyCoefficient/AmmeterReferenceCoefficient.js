$(document).ready(function () {
    initDate();
    loadDataGrid('','first');
});

function initDate() {
    var date = new Date();
    var startDate = new Date();
    startDate.setDate(startDate.getDate() - 30);
    var formateDate = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();//+ " " + date.toTimeString();
    var formateStartDate = startDate.getFullYear() + '-' + (startDate.getMonth() + 1) + '-' + startDate.getDate();//+ " " + startDate.toTimeString();
    $('#StartTime').datebox('setValue', formateStartDate);
    $('#EndTime').datebox('setValue', formateDate);
}

function loadDataGrid(data, type) {
    if (type == 'first') {
        $('#CoefficientContainer').treegrid({
            singleSelect: true,
            rownumbers: true,           
            columns: [[
                { field: 'AmmeterNumber', title: '电表编号', width: 130 },
                //{ field: 'ElectricRoom', title: '电气室', width: 120 },
                { field: 'ElectricRoomName', title: '电气室', width: 120 },
                { field: 'AmmeterName', title: '电表名称', width: 120 },
                { field: 'Remarks', title: '备注', width: 120 },
                { field: 'AmmeterAddress', title: '表地址', width: 70 },
                { field: 'PT', title: '电压变比', width: 70 },
                { field: 'CT', title: '电流变比', width: 70 },
                { field: 'Value', title: '电量', width: 100 },
                { field: 'ChildrenValue', title: '子节点电量', width: 100 },
                { field: 'DValue', title: '差值', width: 100 },
                { field: 'Ratio', title: '参考修正系数', width: 100 }
            ]],
            idField: 'id',
            treeField: 'AmmeterNumber',
        });
    }
    else {
        $('#CoefficientContainer').treegrid('loadData', data);
    }
}

function query() {
    var startDate = $('#StartTime').datebox('getValue');
    var endDate = $('#EndTime').datebox('getValue');
    var organizationId = $('#organizationId').val();
    var sendData = "{organizationId:'" + organizationId + "',startTime:'" + startDate + "',endTime:'" + endDate + "'}";
    var win=$.messager.progress({
        title:'请稍候',
        msg:'数据加载中...'
    });
    $.ajax({
        type: "POST",
        url: "AmmeterReferenceCoefficient.aspx/GetData",
        data: sendData,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var myData = JSON.parse(msg.d);
            loadDataGrid(myData,'last');
        },
        beforeSend:function(XMLHttpRequest){
            win;
        },
        error: function () {
            $.messager.progress('close');
            handleError
        }
    })
}

// 获取双击组织机构时的组织机构信息
function onOrganisationTreeClick(node) {

    // 设置组织机构ID
    // organizationId为其它任何函数提供当前选中的组织机构ID

    $('#organizationId').val(node.OrganizationId);

    // 设置组织机构名称
    // 用于呈现，在界面上显示当前的组织机构名称

    $('#txtOrganization').textbox('setText', node.text);
}