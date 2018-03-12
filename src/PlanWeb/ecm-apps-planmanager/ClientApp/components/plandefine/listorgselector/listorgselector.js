import * as React from 'react';
import './listorgselector.css';
import Header from '../../../components/common/header/header';
import { List, Checkbox, Toast } from 'antd-mobile';
import 'antd-mobile/lib/list/style/css';
import 'antd-mobile/lib/checkbox/style/css';
import 'antd-mobile/lib/toast/style/css';
import QueueAnim from 'rc-queue-anim';
/**
 * selectedValueList: 列表，已经选中的列表,包括value, label等。
 * titile: 文本，标题
 * goBack: 方法，返回
 */
const CheckboxItem = Checkbox.CheckboxItem;
export default class ListOrgSelector extends React.Component {
    constructor(props) {
        super(props);
        let selectedValueList = this.props.selectedValueList || [];
        this.state = {
            selectedValueList,
            parentOrgID: this.props.parentOrgID || 'root',
            orgs:[],
            pathList:[{value:"root", label:"TOP"}],
            disabled: this.props.disabled
        }
    }

    componentDidMount()
    {
        let self = this;
        let root = '/app/plan/api/v1.0/common';
        fetch(root + "/org?parentOrgID=" + this.state.parentOrgID, {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson.code === 200 && !this.isEmpty(resjson.data) && resjson.data.length > 0) {
                let orgs = resjson.data;
                this.setState({
                    orgs: orgs.map(item => {
                        let checked = false;
                        if (!this.isEmpty(this.state.selectedValueList) && this.state.selectedValueList.filter(item2 => item2.value === item.id).length > 0)
                            checked = true;
                        return { value: item.id, label: item.name, checked: checked }
                    })
                });
            }
        });
    }

    deletePsger(item) {
        if (this.state.disabled) return;
        let selectItem = this.state.selectedValueList;
        for (let i = 0; i < selectItem.length; i++) {
            let select = selectItem[i];
            if (item.value === select.value) {
                selectItem.splice(i, 1);
                break;
            }
        }
        let data = this.state.orgs;
        for (let j = 0; j < data.length; j++) {
            if (data[j].value === item.value) {
                data[j].checked = false;
                break;
            }
        }
        this.setState({
            orgs: data,
            selectedValueList: selectItem
        })
    }
    onBreadCrumbClick(item)
    {
        let pathlist = this.state.pathList;
        if (item.value === pathlist[pathlist.length - 1].value) return;
        for(let i = pathlist.length - 1; i > 0; i--)
        {
            let nodeTemp = pathlist[i];
            if (nodeTemp.value === item.value)
            {
                break;
            }
            pathlist.splice(i, 1);
        }
        this.toGetOrgs(item, false);
    }
    render() {
        let data = this.state.orgs;
        let selectedValueList = this.state.selectedValueList;
        //生成已选人员列表
        let selectedDivs = this.isEmpty(selectedValueList)
            ? <div className="selectedPsger" key="z">未选择</div>
            : selectedValueList.map((item, index) => {
            return (
                <div className="selectedPsger" key={index}  >
                    <div>{item.label}</div>
                    {
                        this.state.disabled?<div></div>:
                        <img className="deleteImg" src={require("../../../images/delete1.png")} alt="" onClick={this.deletePsger.bind(this, item)} />
                    }
                </div>
            )
        });
        let pathList = this.state.pathList;
        return (
            <QueueAnim className="listSelector"
                key="demo"
                type={['right','left']}
                ease={['easeOutQuart','easeInOutQuart']}>
                <Header key="a" name={this.props.title + "选择"} onLeftArrowClick={() => this.goBack()} />
                <div className="listSelectorBody" key="c">
                    <div className="selectedPanel" key="b">
                        <div className="divider">已选{this.props.title}</div>
                        {selectedDivs}
                    </div>
                    <div className="pathList" key="pathlistshow">
                        <ol>
                            {
                                pathList.map(item=>{
                                    return <li><a onClick={() => this.onBreadCrumbClick(item)}>{item.label}</a></li>
                                })
                            }
                        </ol>
                    </div>
                    <List renderHeader={() => '选择列表'}>
                        {
                            data.map(i => {
                            return <div className="listItem" key={i.value}>
                                <div className="checkBox">
                                    <CheckboxItem disabled={this.state.disabled} checked={i.checked} key={i.value} onChange={(e) => this.onChange(e, i)}>
                                        {i.label}
                                    </CheckboxItem>
                                </div>
                                <div className="next"><img src={require("../../../images/arrowright-large.png")} className="pull-right" onClick={() => { this.onItemClick(i) }}></img></div>
                            </div>
                        }
                        )}
                    </List>
                </div>
            </QueueAnim>
        );
    }

    onItemClick(i)
    {
        this.toGetOrgs(i, true);
    }

    toGetOrgs(i, flag)
    {
        let self = this;
        let root = '/app/plan/api/v1.0/common';
        fetch(root + "/org?parentOrgID=" + i.value, {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson.code === 200 && !this.isEmpty(resjson.data) && resjson.data.length > 0) {
                let orgs = resjson.data;
                let pathList = this.state.pathList;
                if (flag) pathList.push(i);
                this.setState({
                    orgs: orgs.map(item => {
                        let checked = false;
                        if (!this.isEmpty(this.state.selectedValueList) && this.state.selectedValueList.filter(item2 => item2.value === item.id).length > 0)
                            checked = true;
                        return { value: item.id, label: item.name, checked: checked }
                    }),
                    pathList: pathList
                });
            }
            else
            {
                Toast.info('已经没有下层组织了。', 1);
            }
        });
    }

    goBack()
    {
        this.props.goBack();
    }

    onChange(e, i)
    {
        console.log(e.target.checked);
        console.log(i);
        let data = this.state.orgs;
        for(let j = 0; j < data.length; j++)
        {
            if(data[j].value === i.value)
            {
                data[j].checked = e.target.checked;
                break;
            }
        }
        let selectedValueList = this.state.selectedValueList;
        if(e.target.checked)
        {
            if (selectedValueList.filter(item=>item.value===i.value).length < 1)
                selectedValueList.push(i);
        }
        else
        {
            for (let k = 0; k < selectedValueList.length; k++) {
                let select = selectedValueList[k];
                if (i.value === select.value) {
                    selectedValueList.splice(k, 1);
                    break;
                }
            }
        }
        this.setState({
            orgs: data,
            selectedValueList
        });
    }

    isEmpty(value) {
        return value == null || value == undefined || value == "" || value == "undefined" || (Array.isArray(value) && value.length === 0) || (Object.prototype.isPrototypeOf(value) && Object.keys(value).length === 0);
    }
}