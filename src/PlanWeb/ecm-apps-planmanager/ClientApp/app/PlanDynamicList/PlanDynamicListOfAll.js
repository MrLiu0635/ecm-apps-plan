import * as React from 'react';
import './plandynamiclist.css';
import DynamicCard from '../../components/dynamiccard/dynamiccard';
import Header from "../../components/common/header/header";
import { List } from 'antd-mobile';
import 'antd-mobile/lib/list/style/css';

const Item = List.Item;
const Brief = Item.Brief;

export default class PlanDynamicListOfAll extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            stateFlag: "",
            addPlanDynamic: [],
            currentDynamic: "",
            dynamicList: []
        }
    }
    componentWillMount() {
        let addPlanDynamic = <img src={require("../../images/add_white.png")}
            alt="" className="addDynamic pull-right"
            onClick={this.clickAdd.bind(this)}
        ></img>;
        let root = "app/plan/api/v1.0/plandynamic/all";
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
                this.setState({
                    dynamicList: resjson.data,
                    addPlanDynamic: addPlanDynamic
                })
            }
        }).catch((e) => {
            alert("查询失败，请重试");
        });
    }

    render() {
        let dynamicList = this.state.dynamicList;
        let stateValue = "";
        let dynamicListDiv;
        if (this.isDynamicListNull(dynamicList)) {
            dynamicListDiv = <div></div>
        }
        else {
              dynamicListDiv = dynamicList.map(index => {
                switch (index.state) {
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
                return(
                        <List className="my-list-allDynamic">
                            <Item multipleLine extra={stateValue} onClick={() => this.EditSelectedDynamic(index)}>
                                {index.planDefine.name} <Brief>{index.period.name}</Brief>
                            </Item>
                        </List>
                      ) 
              }
           )
        }
        return <div className="PlanDynamicListOfAll">
            <Header name={"计划列表"} onLeftArrowClick={() => this.goBack()} more={this.state.addPlanDynamic} />
            <div className="AllDynamicList">
                {dynamicListDiv}      
            </div>
        </div>
    }
    
    goBack() {
        console.log("planDynamic closed.");
        imp.iWindow.close();
    }
   
    EditSelectedDynamic(dynamicInfo) {
        this.props.history.push({
            pathname: '/app/plan/web/m/plandynamic',
            state: {
                currentDynamic: dynamicInfo,
                stateFlag : "2"
            }
        });
    }
    clickAdd() {
        this.props.history.push({
            pathname: '/app/plan/web/m/plandynamic',
            state: {
                stateFlag : "1"
            }
        });
    }
    isDynamicListNull(dynamicList) {
        for (let i in dynamicList) {
            if (i === undefined) continue;
            else {
                return false;
            }
        };
        return true;
    }
}