$(function () {
    publicData.keyid = $.getUrlParam('keyid');
    loadGridData('first');
});
var publicData = {
    keyid: ""
};

function loadGridData(myLoadType) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    $.ajax({
        type: "POST",
        url: "Detail.aspx/GetPVFDetail",
        data: "{keyId: '" + publicData.keyid + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (myLoadType == 'first') {
                myLoadType = 'last';
                m_MsgData = jQuery.parseJSON(msg.d);
                InitializeGrid(m_MsgData);
            }
            else if (myLoadType == 'last') {
                m_MsgData = jQuery.parseJSON(msg.d);
                $('#dg').datagrid('loadData', m_MsgData);
            }
        }
    });
}
function InitializeGrid(myData) {
    $('#dg').datagrid({
        data: myData,
        iconCls: 'icon-edit', singleSelect: true, rownumbers: true, striped: true, toolbar: '#tb',
        columns: [[
            { field: 'StartTime', title: '起始时间', width: '30%', align: 'center' },
            { field: 'EndTime', title: '终止时间', width: '30%', align: 'center' },
            { field: 'Type', title: '类型', width: '20%', align: 'center' },
        ]]
    });
}