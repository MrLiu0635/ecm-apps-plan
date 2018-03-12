import React, { Component } from 'react';
import './userselector.css'
import { Modal } from 'antd-mobile';
import 'antd-mobile/lib/modal/style/css'

const alert = Modal.alert;
let timer = 0;
export default class UserSelector extends Component {
    constructor(props) {
        super(props);
        this.state = {
            personList: [],
            allValue: "",
            selectItem: [],
            userModel: this.props.location.state.userModel || "人员",
            localStore: this.props.location.state.localStore || "mxList"
        }
        // window.sessionStorage.setItem('ticketApply_notFresh', 'true');
    }
    
    componentDidMount() {
        let mxList = JSON.parse(window.localStorage.getItem(this.state.localStore));
        mxList = mxList || [];
        this.setState({
            selectItem: mxList
        })
    }
    render() {
        //生成已选人员列表
        let selectedDivs = this.state.selectItem.map((item, index) => {
            if(item!==undefined&&item!==null)
            { return (
                 <div className="selectedPsger" key={index}  >
                     <div>{item.name}</div>
                     <img className="deleteImg" src={require("../../images/delete1.png")} alt="" onClick={this.deletePsger.bind(this, item)} />
                </div>
            )}
        });

        //生成人员列表
        let personDivs = this.state.personList.map((item, index) => {
            return (
                <div className="list-group-item" key={index} onClick={this.clickItem.bind(this, item)}>
                    <img src={require("../../images/avatar-large.png")} alt="" className="avatar" />
                    <div className="info">
                        <div>{item.globalName}</div>
                        <div className="dept">{item.orgName}</div>
                    </div>
                </div>
            )
        });
        return (
            <div className="selectPsger">
                <header className="navbar text-center">
                    <img id="backImg" src={require("../../images/arrowback-large.png")} alt=""
                        className="pull-left arrowback" onClick={this.backToApply.bind(this)}></img>
                    <span className="navbar-brand header-title-center">选择{this.state.userModel}</span>
                    <span className="finish" onClick={this.finish.bind(this)}>完成</span>
                </header>
                <div className="list-search"><img src={require("../../images/search.png")} alt="" />
                    <input ref="search" className="form-control" onChange={this.searchItem.bind(this)} value={this.state.allValue} placeholder="请输入姓名搜索" />
                </div>

                <div className="selectedPanel">
                    <div className="divider">已选{this.state.userModel}</div>
                    {selectedDivs}
                </div>
                {this.state.personList.length > 0 ?
                    <div className="renyuan" >人员列表</div>
                    : ''
                }
                <div className="personList">
                    {personDivs}
                </div>
            </div>
        )
    }
    backToApply() {
        this.props.history.goBack();
    }
    /**
     * 搜索人员
     */
    searchItem(event) {
        let self = this;
        let searchValue = this.refs.search.value.trim().replace(/[\@\#\$\%\^\&\*\{\}\:\"\L\<\>\?]/);
        this.setState({
            allValue: searchValue
        });
        if (searchValue === '') {
            return;
        }
        if (timer) {
            window.clearTimeout(timer);
            timer = 0;
        }
        timer = setTimeout(function () {
            fetch('/app/plan/api/v1.0/common/' + searchValue.trim(), {
                method: 'GET',
                credentials: 'include'
            }).then(response => {
                return response.json();
            }).then(responseData => {
                self.setState({
                    personList: responseData.data || [],
                })
                self.refs.search.focus();
            }).catch(function (e) {
                self.setState({
                    personList: []
                });
                self.refs.search.focus();
            })
        }, 500);
    }
    /**
     * 选择人员
     */
    clickItem(item) {
        //去重校验
        for (let i = 0; i < this.state.selectItem.length; i++) {
            let select = this.state.selectItem[i];
            if (item.id === select.id) {
                alert(item.name + "已添加至" + this.state.userModel + "，请重新选择");
                return;
            }
        }
        this.setState({
            allValue: '',
            personList: []
        });
        this.state.selectItem.push(item);
    }
    /**
     * 点击完成
     */
    finish() {
        if (this.state.selectItem.length === 0) {
            alert('返回确认', '您尚未选择任何' + this.state.userModel + '，确认返回。', [
                { text: '取消', onPress: () => console.log('cancel'), style: 'default' },
                { text: '确定', onPress: () => { this.goBack(); }},
            ]);
        }
        else
        {
            this.goBack();
        }
    }

    goBack() {
        let mxList = this.state.selectItem;
        mxList = mxList || [];
        window.localStorage.setItem(this.state.localStore, JSON.stringify(mxList));
        this.props.history.goBack();
    }
    /**
     * 删除联系人
     */
    deletePsger(item) {
        let selectItem = this.state.selectItem;
        for (let i = 0; i < selectItem.length; i++) {
            let select = selectItem[i];
            if (item.id === select.id) {
                selectItem.splice(i, 1);
                break;
            }
        }
        this.setState({
            selectItem: selectItem
        })
    }
}