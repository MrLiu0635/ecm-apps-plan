import * as React from 'react';
import './periodconfig.css';
import Header from "../../components/common/header/header";
import { List, Switch } from 'antd-mobile';

export default class PeriodConfig extends React.Component {
    constructor(props) {
        super(props);
        
        let stateFlag = true;
        this.state = {
            stateFlag: true,
            periodSetList: []
        }
    }
    componentWillMount() {
        let period = [];
        let periodData = [];
        let root = '/app/plan/api/v1.0/period/sets'; //获取已选择的周期集
        let addPeriodSet = <img src={require("../../images/add_white.png")}
            alt="" className="add pull-right"
            onClick={this.clickAdd.bind(this)}
        ></img>;
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
            if (resjson == "" || resjson == null || resjson == undefined || resjson == []) {
                //如果还未选择分配周期集，则默认显示自然年
                //this.setState({ noRecord: true }); return;
                return;//test
            }
            else {
                period = resjson.data;
                for (let periodIndex in period) {
                    periodData.push({ value: period[periodIndex].id, label: period[periodIndex].name });
                }
                this.setState({
                    periodSetList: periodData,
                    addPeriodSet: addPeriodSet
                })
            }
        }).catch((e) => {
            alert("查询失败，请重试");
        });
    }

    render() {
        return <div className="periodSetListPage">
            <Header name={"已启用周期集"} onLeftArrowClick={() => this.goBack()} more={this.state.addPeriodSet} />
            <div className="myContent">
                <List>
                    {this.state.periodSetList.map(i => (
                        <List.Item
                            extra={<Switch checked={this.state.stateFlag} disabled/>}
                        >{i.label}
                        </List.Item>
                    ))}
                </List>
            </div>
        </div>;
    }
    goBack() {
        console.log("periodSetList closed.");
        //清空缓存
        imp.iWindow.close();

    }
    clickAdd() {
        let periodSetList = this.state.periodSetList;
        this.props.history.push({
            pathname: '/app/plan/web/m/periodset',
            state: {
                periodSetList: periodSetList
            }
        });
    }
}