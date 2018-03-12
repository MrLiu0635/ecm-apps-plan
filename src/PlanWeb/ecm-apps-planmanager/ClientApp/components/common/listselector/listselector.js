import * as React from 'react';
import './listselector.css';
import Header from '../header/header';
import { List, Checkbox } from 'antd-mobile';
import 'antd-mobile/lib/list/style/css';
import 'antd-mobile/lib/checkbox/style/css';
import QueueAnim from 'rc-queue-anim';
/**
 * 1 dataList: 列表，全部展示数据，包括value, label, checked等。
 * 2 titile: 文本，标题
 * 3 goBack: 方法，返回
 * 4 more: jsx代码用于展示右侧工具，举例
 * more = <div className="more"><img src={require("../../images/more-black.png")} className="pull-right" onItemClick={(e)=>{this.}}></img></div>
 * 5 isSingle: 布尔，单选
 */
const CheckboxItem = Checkbox.CheckboxItem;
export default class ListSelector extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: this.props.dataList,
            disabled: this.props.disabled
        }
    }
    render() {
        let data = this.state.data;
        let otherStyle = {};
        let moreFuncComp = ()=><div></div>;
        if (this.props.more !== null && this.props.more !== undefined) {
            moreFuncComp = this.props.more;
        }
        return (
            <QueueAnim className="listSelector"
                key="demo"
                type={['right']}
                ease={['easeInOutQuart']}>
                <Header key="a" name={this.props.title || "选择"} onLeftArrowClick={() => this.goBack()} />
                <div className="listSelectorBody" key="b">
                        <List renderHeader={() => '选择列表'}>
                        { 
                            data.map(i => <div className="listItem" key={i.value}>
                                    <div className="checkBox" key={i.value + 'cb'}>
                                        <CheckboxItem disabled = {this.state.disabled} checked={i.checked} key={i.value} onChange={(e) => this.onChange(e, i)}>
                                            {i.label}
                                        </CheckboxItem>
                                    </div>
                                    {moreFuncComp(i)}
                                </div> )
                        }
                        </List>
                </div>
            </QueueAnim>
        );
    }

    goBack()
    {
        this.props.goBack();
    }

    onChange(e, i)
    {
        let data = this.state.data;
        if (this.props.isSingle && e.target.checked)
        {
            for (let j = 0; j < data.length; j++) 
            {
                data[j].checked = false;
            }
        }
        for(let j = 0; j < data.length; j++)
        {
            if(data[j].value === i.value)
            {
                data[j].checked = e.target.checked;
                break;
            }
        }
        this.setState({
            data
        });
    }

    isEmpty(value) {
        return value == null || value == undefined || value == "" || value == "undefined" || (Array.isArray(value) && value.length === 0) || (Object.prototype.isPrototypeOf(value) && Object.keys(value).length === 0);
    }
}