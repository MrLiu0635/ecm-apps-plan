import * as React from 'react';
import './periodconfig.css';
import Header from "../../components/common/header/header";
import { Button, List, Checkbox } from 'antd-mobile';
import { Toast, ActivityIndicator } from 'antd-mobile';
import 'antd-mobile/lib/checkbox/style/css'
import '../../css/buttons.css';
import 'antd-mobile/lib/toast/style/css';

const CheckboxItem = Checkbox.CheckboxItem;
export default class PeriodSet extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            stateFlag: false,
            periodSetList: [],
            selectedPeriod: []
        }
    }
    componentWillMount() {
        let selectedPeriod = [];
        let period = [];
        let periodData = [];
        let stateFlag = false;
        let root = '/app/plan/api/v1.0/period/sets/all'; //获取已选择的周期集
        fetch(root, {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            console.log(resjson);
            if (resjson == "" || resjson == null || resjson == undefined || resjson == []) { return;}
            else {
                period = resjson.data;
                for (let periodIndex in period) {
                    stateFlag = false;
                    this.props.location.state.periodSetList.map(item => {
                        if (period[periodIndex].id == item.value) {
                            stateFlag = true;
                            selectedPeriod.push(item.value);
                        }
                    })
                    periodData.push({ value: period[periodIndex].id, label: period[periodIndex].name, flag: stateFlag });
                }
                this.setState({
                    periodSetList: periodData,
                    selectedPeriod: selectedPeriod
                })
            }
        });
    }
    onChange(val, e) {
        let selectedPeriod = this.state.selectedPeriod;
        if (e.target.checked == true)
            selectedPeriod.push(val);
        else {
            let index = selectedPeriod.indexOf(val);
            if (!this.isEmpty(index))
                selectedPeriod.splice(index, 1);
            //this.state.selectedPeriod.map(item => {
            //    if (item == val)
            //        selectedPeriod.pop(item);               
            //})
        }
        this.setState({
            selectedPeriod: selectedPeriod
        })
    }
    render() {
        return <div className="periodSetListPage">
            <Header name={"周期集"} onLeftArrowClick={() => this.goBack()} />
            <div className="myContent">
                <List>
                    {this.state.periodSetList.map(i => 
                        (
                            <CheckboxItem key={i.value} defaultChecked={i.flag} onChange={(e) => this.onChange(i.value, e)}>
                                {i.label}
                            </CheckboxItem>
                        )
                    )}
                </List>

                <div className="periodSaveComp">
                    <button
                        className="saveButton button button-pill button-primary"
                        onClick={() => this.saveHandleClick()}>保&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;存
                    </button>
                </div>

            </div>
        </div>;
    }
    goBack() {
        console.log("periodSetList closed.");
        this.props.history.goBack();    
    }
    saveHandleClick() {
        let periodIDs = this.state.selectedPeriod;
        fetch('/app/plan/api/v1.0/period/sets',
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json, text/plain, */*',
                    'Content-Type': 'application/json',
                    // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                },
                body: JSON.stringify(periodIDs),
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
    isEmpty(value) {
        return value == null || value == undefined || value === "" || value == "undefined" || (Array.isArray(value) && value.length === 0) || (Object.prototype.isPrototypeOf(value) && Object.keys(value).length === 0);
    }
}