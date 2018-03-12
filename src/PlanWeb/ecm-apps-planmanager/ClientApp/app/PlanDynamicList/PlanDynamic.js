import * as React from 'react';
import './plandynamiclist.css';
import Header from "../../components/common/header/header";
import { Button, Picker, List, Steps, WingBlank, WhiteSpace } from 'antd-mobile';
import { Modal, Toast, ActivityIndicator } from 'antd-mobile';
import 'antd-mobile/lib/steps/style/css'
import '../../css/buttons.css';
import 'antd-mobile/lib/toast/style/css';
import 'antd-mobile/lib/modal/style/css';
import 'antd-mobile/lib/picker/style/css';
import 'antd-mobile/lib/list/style/css';

const Step = Steps.Step;
const alert = Modal.alert;
export default class PlanDynamic extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            disableFlag: false,
            deletePic: [],
            deleteDynamicIDs: [],
            status: "",
            stateValue: "",
            stateValueList: [],
            pickerValue: [],
            periodPickerValue: [],
            planDefines: [],
            planDefineList: [],
            periodList: []
        };
    }
    componentWillMount() {
        let status;
        let stateValue;
        let pickerValue = [];
        let periodPickerValue = [];
        let stateValueList = [];
        let planDefines = [];
        let planDefineList = [];
        let deleteDynamicIDs = [];
        let deletePic = [];
        if (this.props.location.state.stateFlag == "1") {
            status = 0;
            stateValue = "待填写";
        }
        else if (this.props.location.state.stateFlag == "2") {
            status = this.props.location.state.currentDynamic.state - 1;
            pickerValue.push(this.props.location.state.currentDynamic.planDefine.id);
            periodPickerValue.push(this.props.location.state.currentDynamic.period.id);
            deleteDynamicIDs.push(this.props.location.state.currentDynamic.id);
            deletePic = <img src={require("../../images/alter_delete.png")}
                alt="" className="delete pull-right"
                onClick={this.clickDelete.bind(this)}
            ></img>;
            switch (this.props.location.state.currentDynamic.state) {
                case 1: {
                    stateValue = "待填写";
                    break;
                }
                case 2: {
                    stateValue = "执行中";
                    break;
                }
                case 3: {
                    stateValue = "待总结";
                    break;
                }
                default: {
                    stateValue = "未知";
                    break;
                }
            }
            
        }
        stateValueList = [{
            title: '待填写',
            description: '',
        }, {
            title: '执行中',
            description: '',
        }, {
            title: '待总结',
            description: '',
        }];
        let root = "app/plan/api/v1.0/plandefine";
        fetch(root, {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson === "" || resjson === null || resjson === undefined || resjson === []) { return; }
            else {
                planDefines = resjson.data;
                for (let index in planDefines) {
                    planDefineList.push({ value: planDefines[index].id, label: planDefines[index].name });
                }
                this.setState({
                    status: status,
                    planDefines: planDefines,
                    planDefineList: planDefineList,
                    stateValue: stateValue,
                    stateValueList: stateValueList,
                    deletePic: deletePic,
                    deleteDynamicIDs: deleteDynamicIDs,
                    pickerValue: pickerValue,
                    periodPickerValue: periodPickerValue
                })
                if (this.props.location.state.stateFlag == "2") {
                    this.getPeriodSetList(pickerValue);
                }
            }
            }).catch((e) => {
                alert("查询失败，请重试");
            });
    }
    changeState(index, title) {
        //console.log(val);
        this.setState({
            status: index,
            stateValue: title
        })
    }
    render() {
        let disableFlag = this.state.disableFlag
        let steps = this.state.stateValueList.map((s, i) => <Step className="my-step-planDynamic" key={i} title={s.title} description={s.description} onClick={v => this.changeState(i, s.title)} />);   
        return <div className="planDynamic">
            <Header name={"计划动态"} onLeftArrowClick={() => this.goBack()} more={this.state.deletePic} />
            <div className="myContentOfPlanDynamic">
                <List  className="my-list">
                    <Picker data={this.state.planDefineList} cols={1} value={this.state.pickerValue} disabled={disableFlag} onChange={(v) => this.onChangeOfPlan(v)} className="forss">
                        <List.Item arrow="horizontal">计划定义选择</List.Item>
                    </Picker>
                </List>
                <List  className="my-list">
                    <Picker data={this.state.periodList} cols={1} value={this.state.periodPickerValue} disabled={disableFlag} onChange={v => this.setState({ periodPickerValue: v })} className="forss">
                        <List.Item arrow="horizontal">周期选择</List.Item>
                    </Picker>
                </List>
                <List  className="my-list">
                    <List.Item extra={this.state.stateValue}>{'当前状态'}</List.Item>
                </List>
                <List renderHeader={() => '状态流转'}  className="my-list-planDynamic">
                    <WingBlank mode={20} className="stepsExample">
                        <div className="sub-title"></div>
                        <WhiteSpace />
                        <Steps className="my-steps-planDynamic" current={this.state.status} direction="horizontal">{steps}</Steps>
                    </WingBlank>
                </List>
                <div className="planDynamicSaveComp">
                    <button disabled={disableFlag}
                        className="planDynamicSaveButton button button-pill button-primary"
                        onClick={() => this.saveHandleClick()}>保&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;存
                    </button>
                </div>
            </div>
        </div>;
    }
    saveHandleClick() {
        if (this.props.location.state.stateFlag == "1") {
            this.savePlanDynamic();
        }
        else if (this.props.location.state.stateFlag == "2") {
            this.updatePlanDynamic();
        }
    }
    savePlanDynamic() {
        let planDefine = {
            ID: this.state.pickerValue[0]
        };
        let period = {
            ID: this.state.periodPickerValue[0]
        };
        let newPlanDynamic = {
            PlanDefine: planDefine,
            Period: period,
            State: this.state.status + 1
        };

        fetch('/app/plan/api/v1.0/plandynamic',
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json, text/plain, */*',
                    'Content-Type': 'application/json',
                    // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                },
                body: JSON.stringify(newPlanDynamic),
                credentials: 'include'
            }).then(response => {
                if (response.status === 200) {
                    this.setState({
                        disableFlag: true
                    })
                    console.log("OK!");
                    Toast.success('保存成功。', 1);
                } else {
                    alert("保存失败，请重试");
                }
            }).catch((e) => {
                alert("保存失败，请重试");
            });
    }
    updatePlanDynamic() {
        let planDefine = {
            ID: this.state.pickerValue[0]
        };
        let period = {
            ID: this.state.periodPickerValue[0]
        };
        let planDynamic = {
            ID: this.props.location.state.currentDynamic.id,
            PlanDefine: planDefine,
            Period: period,
            State: this.state.status + 1
        };

        fetch('/app/plan/api/v1.0/plandynamic',
            {
                method: 'PUT',
                headers: {
                    'Accept': 'application/json, text/plain, */*',
                    'Content-Type': 'application/json',
                    // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                },
                body: JSON.stringify(planDynamic),
                credentials: 'include'
            }).then(response => {
                if (response.status === 200) {
                    console.log("OK!");
                    Toast.success('保存成功。', 1);
                } else {
                    alert("保存失败，请重试");
                }
            }).catch((e) => {
                alert("保存失败，请重试");
            });
    }
    deletePlanDynamic() {
        let deleteDynamicIDs = this.state.deleteDynamicIDs;
        fetch('/app/plan/api/v1.0/plandynamic',
            {
                method: 'DELETE',
                headers: {
                    'Accept': 'application/json, text/plain, */*',
                    'Content-Type': 'application/json',
                    // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                },
                body: JSON.stringify(deleteDynamicIDs),
                credentials: 'include'
            }).then(response => {
                if (response.status === 200) {
                    console.log("OK!");
                    Toast.success('删除成功。', 1);
                    this.goBack();
                } else {
                    alert("订单提交失败，请重试");
                }
            }).catch((e) => {
                alert("订单提交失败，请重试");
            });
        //this.goBack();
    }
    clickDelete() {
        alert('删除确认', '删除后不能恢复，请谨慎操作。', [
            { text: '取消', onPress: () => console.log('cancel'), style: 'default' },
            {
                text: '确定', onPress: () => {
                    this.deletePlanDynamic();
                }
            },
        ]);
    }
    goBack() {
        this.props.history.goBack();
    }
    getPeriodSetList(v) {
        let periodSetID;
        let periodTypeID;
        let periodList = [];
        let period = [];
        this.state.planDefines.map(item => {
            if (item.id == v) {
                periodSetID = item.periodSet.id;
                periodTypeID = item.periodType.id;
            }
        }
        )
        let root = "app/plan/api/v1.0/period?PeriodSetID=" + periodSetID + "&PeriodTypeID=" + periodTypeID;
        fetch(root, {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson === "" || resjson === null || resjson === undefined || resjson === []) { return; }
            else {
                period = resjson.data;
                for (let index in period) {
                    periodList.push({ value: period[index].id, label: period[index].name });
                }
                this.setState({
                    periodList: periodList,
                })
            }
        }).catch((e) => {
            alert("查询失败，请重试");
        });
    }
    onChangeOfPlan(v) {
        this.setState({
            pickerValue: v,
            periodPickerValue: [],
            periodList: []
        })
        let periodSetID;
        let periodTypeID;
        let periodList = [];
        let period = [];
        this.state.planDefines.map(item => {
            if (item.id == v) {
                periodSetID = item.periodSet.id;
                periodTypeID = item.periodType.id;
            }
        }
        )
        let root = "app/plan/api/v1.0/period?PeriodSetID=" + periodSetID + "&PeriodTypeID=" + periodTypeID;
        fetch(root, {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson === "" || resjson === null || resjson === undefined || resjson === []) { return; }
            else {
                period = resjson.data;
                for (let index in period) {
                    periodList.push({ value: period[index].id, label: period[index].name });
                }
                this.setState({
                    periodList: periodList,
                })
            }
        }).catch((e) => {
            alert("查询失败，请重试");
        });
    }
}