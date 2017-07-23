$(document).ready(function () {
    //LoadProductionType('first');
    //loadOrganisationTree('first');

    //$('#TextBox_OrganizationId').textbox('hide');
    SetYearValue();
    LoadEnergyConsumptionData('first');
});
function onOrganisationTreeClick(myNode) {
    //alert(myNode.text);
    if(myNode.id.length == 7)
    {
        $('#TextBox_OrganizationId').attr('value', myNode.OrganizationId);
        $('#TextBox_OrganizationText').textbox('setText', myNode.text);
    }
    else
    {
        alert("请选择到产线!");
    }
}
function SetYearValue() {
    var m_PlanYear = new Date().getFullYear();
    $('#numberspinner_PlanYear').numberspinner('setValue', '2014');
}
function QueryEnergyConsumptionResultInfoFun() {
    LoadEnergyConsumptionData('last');
}

function LoadEnergyConsumptionData(myLoadType) {
    $.messager.progress({
        title: 'Please waiting',
        msg: 'Loading data...'
    });

    var m_OrganizationId = $('#TextBox_OrganizationId').val();
    var m_PlanYear = $('#numberspinner_PlanYear').numberspinner('getValue');
    //var m_OrganizationType = $('#TextBox_OrganizationType').textbox('getText');
    var m_GridCommonName = "EnergyConsumptionResultInfo";
    $.ajax({
        type: "POST",
        url: "EnergyConsumptionResult.aspx/GetEnergyInfo",
        data: "{myOrganizationId:'" + m_OrganizationId + "',myPlanYear:'" + m_PlanYear + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (myLoadType == 'first') {
                InitializeEnergyConsumptionGrid(m_GridCommonName, m_MsgData);
            }
            else if (myLoadType == 'last') {
                $('#grid_' + m_GridCommonName).datagrid('loadData', m_MsgData);
            }
            if (m_MsgData != null && m_MsgData != undefined) {
                for (var i = 0; i < m_MsgData.rows.length / 2; i++) {
                    $('#grid_' + m_GridCommonName).datagrid('mergeCells', {
                        index: i * 2,
                        field: 'QuotasName',
                        rowspan: 2
                    });
                }
            }
            $.messager.progress('close');
        },
        error: function (msg) {
            $.messager.progress('close');
        }
    });
}

//////////////////////////////////初始化基础数据//////////////////////////////////////////
function InitializeEnergyConsumptionGrid(myGridId, myData) {
    var m_FrozenColumns = myData['columns'].splice(0, 4);
    m_FrozenColumns[0]["hidden"] = true;
    m_FrozenColumns[1]["hidden"] = true;
    $('#grid_' + myGridId).datagrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        idField: m_FrozenColumns[0].field,
        frozenColumns: [m_FrozenColumns],
        columns: [myData['columns']],
        //loadMsg: '',   //设置本身的提示消息为空 则就不会提示了的。这个设置很关键的
        rownumbers: true,
        //pagination: true,
        singleSelect: true,
        //idField: m_IdAndNameColumn[0].field,
        //pageSize: 20,
        //pageList: [20, 50, 100, 500],

        toolbar: '#toolbar_' + myGridId
    });
}