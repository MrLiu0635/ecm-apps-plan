import * as React from 'react';
import './RoleSelect.css';
import Header from "../../components/common/header/header";
import { Picker, List } from 'antd-mobile';
import 'antd-mobile/lib/picker/style/css';
import 'antd-mobile/lib/list/style/css';

export default class RoleSelect extends React.Component {
    constructor(props) {
        super(props);
        let returnYunPlusFlag=true;
        if (this.props.location.key) {
            returnYunPlusFlag=false;
        }
        this.state={
            pickerValue:"",
            pickerRoleName:"",
            data:[],
            noRecord:false,
            roles:[],
            selectedRoleName:"",
            returnYunPlusFlag : returnYunPlusFlag,
            roleInfo:{}
        }
    }
    componentWillMount(){
        let roles=[];
        let roleData=[];
        let roleInfo=[];
        let root = "app/plan/api/v1.0/common/role";
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
                roles=resjson.data;
                for(let roleIndex in roles)
                {
                    roleData.push({ value:roles[roleIndex].id,label:roles[roleIndex].name});
                    roleInfo.push(roleIndex,roles[roleIndex].id)
                }
                this.setState({data:roleData,roleInfo:roleInfo})
            }
        }).catch((e) => {
            alert("查询失败，请重试");
        });
    }
    render() {
        console.log(this.state.roleNames);
        return (<div className="plan-roleselect">
            <Header name={"角色选择"} onLeftArrowClick={() => this.goBack()}/>
            <Picker data={this.state.data} cols={1} value={this.state.pickerValue} onChange={v => this.setState({ pickerValue: v })} className="forss">
          <List.Item arrow="horizontal">角色选择</List.Item>
        </Picker>
             <div className="plan-roleselect-nextComp">
                <button
                 className="plan-roleselect-nextButton"
                 onClick={() => this.nextClick()}>下&nbsp;&nbsp;&nbsp;一&nbsp;&nbsp;&nbsp;步</button>
            </div>
        </div>)
    }
    changeSelect(){

    }
    nextClick() {

        this.props.history.push({
            pathname: '/app/plan/web/m/dynamiclist',
            state: {
                roleID: this.state.pickerValue
            }
        });
    }
    goBack() {
        console.log("plan closed.");
        if (this.state.returnYunPlusFlag) {
            imp.iWindow.close();
        }
        else {
            this.props.history.goBack();
        }
    }
}