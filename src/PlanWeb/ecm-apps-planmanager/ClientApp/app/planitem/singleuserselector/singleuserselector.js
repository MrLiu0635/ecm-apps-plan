import * as React from 'react';
import './singleuserselector.css';
import Header from "../../../components/common/header/header";
import { List } from 'antd-mobile';
import 'antd-mobile/lib/list/style/css';
import QueueAnim from 'rc-queue-anim';

const Item = List.Item;
const Brief = Item.Brief;
let timer = 0;
/**
 * goBack
 */
export default class SingleUserSelector extends React.Component {
    constructor(props) {
        super(props);
        this.state={
            allValue:'',
            personList:[]
        }
    }
    render() {
        return (<div> 
            <QueueAnim
                key="demo"
                type={['right']}
                ease={['easeInOutQuart']}>
                <Header key="a" name="人员选择" onLeftArrowClick={() => this.goBack()} />
                <div className='SingleUserSelectorBody' key="b">
                    <div key="c" className="list-search"><img src={require("../../../images/search.png")} alt="" />
                        <input ref="search" className="form-control" onChange={this.searchItem.bind(this)} value={this.state.allValue} placeholder="请输入姓名搜索" />
                    </div>
                    <List key="d" renderHeader={() => '人员列表（点击选择）'} className="my-list">
                    {
                        this.state.personList.map(item=>
                                <Item key={item.id} extra={item.globalName} arrow="horizontal" onClick={this.onItemClick.bind(this, item)}>{item.name}</Item>
                        )
                    }
                    </List>
                </div>
            </QueueAnim>
        </div>
        );
    }

    onItemClick(item)
    {
        this.props.onItemClick(item.id);
    }

    searchItem()
    {
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
    goBack() {
        this.props.goBack();
    }

    isEmpty(value) {
        return value == null || value == undefined || value === "" || value === "undefined" || (Array.isArray(value) && value.length === 0) || (Object.prototype.isPrototypeOf(value) && Object.keys(value).length === 0);
    }
}