$(document).ready(function () {
    //LoadProductionType('first');
    //loadOrganisationTree('first');

    //$('#TextBox_OrganizationId').textbox('hide');
    SetYearValue();
    LoadEnergyConsumptionData('first');
});
function onOrganisationTreeClick(myNode) {
    //alert(myNode.text);
    //$('#TextBox_OrganizationId').attr('value', myNode.OrganizationId); 
    $('#TextBox_OrganizationLevelCode').attr('value', myNode.id);  //textbox('setText', myNode.OrganizationId);

    $('#TextBox_OrganizationText').textbox('setText', myNode.text);
    $('#TextBox_OrganizationType').textbox('setText', myNode.OrganizationType);
}
function SetYearValue() {
    var m_PlanYear = new Date().getFullYear();
    $('#numberspinner_PlanYear').numberspinner('setValue', '2014');
}
function QueryEnergyConsumptionResultInfoFun() {
    LoadEnergyConsumptionData('last');
}

function LoadEnergyConsumptionData(myLoadType) {
    var m_LevelCode = $('#TextBox_OrganizationLevelCode').val();
    var m_PlanYear = $('#numberspinner_PlanYear').numberspinner('getValue');
    //var m_OrganizationType = $('#TextBox_OrganizationType').textbox('getText');
    var m_GridCommonName = "EnergyConsumptionResultInfo";
    $.ajax({
        type: "POST",
        url: "EnergyConsumptionResult.aspx/GetEnergyConsumptionInfo",
        data: "{myLevelCode:'" + m_LevelCode + "',myPlanYear:'" + m_PlanYear + "'}",
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
        }
    });
}

//////////////////////////////////初始化基础数据//////////////////////////////////////////
function InitializeEnergyConsumptionGrid(myGridId, myData) {

    $('#grid_' + myGridId).datagrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        //loadMsg: '',   //设置本身的提示消息为空 则就不会提示了的。这个设置很关键的
        rownumbers: true,
        singleSelect: true,
        idField: 'OrganizationID',
        columns: [[{
            width: 120,
            title: '产线类型',
            field: 'Type',
            formatter: function (value, row, index) {
                if (value == 1) {
                    return "熟料";
                }
                else if (value == 2) {
                    return "水泥磨";
                }
                else if (value == 3) {
                    return "余热发电"
                }
                else {
                    return "其它";
                }
            }
        }, {
            width: 120,
            title: '指标名称',
            field: 'IndicatorName'
        }, {
            width: '120',
            title: '一月',
            field: 'January'
        }, {
            width: 120,
            title: '二月',
            field: 'February'
        }, {
            width: 150,
            title: '三月',
            field: 'March'
        }, {
            width: '120',
            title: '四月',
            field: 'April'
        }
        , {
            width: '120',
            title: '五月',
            field: 'May'
        }
        , {
            width: '120',
            title: '六月',
            field: 'June'
        }
        , {
            width: '120',
            title: '七月',
            field: 'July'
        }
        , {
            width: '120',
            title: '八月',
            field: 'August'
        }
        , {
            width: '120',
            title: '九月',
            field: 'September'
        }
        , {
            width: '120',
            title: '十月',
            field: 'October'
        }
        , {
            width: '120',
            title: '十一月',
            field: 'November'
        }, {
            width: 120,
            title: '十二月',
            field: 'December'
        }, {
            width: 60,
            title: '合计',
            field: 'Totals'
        }, {
            width: 150,
            title: '备注',
            field: 'Remarks'
        }]],
        toolbar: '#toolbar_' + myGridId
    });
}