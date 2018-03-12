import * as React from 'react';
import './planitemmodel.css';
import Header from '../../../components/common/header/header';
import { List } from 'antd-mobile';
import 'antd-mobile/lib/list/style/css';
import QueueAnim from 'rc-queue-anim';
import SwitchLine from '../../common/switchline/switchline'
/**
 * title
 * dataList
 * goBack
 * editable
 */
export default class PlanItemModel extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            editable: this.props.editable,
            data: this.props.dataList
        }
    }
    render() {
        return (
            <QueueAnim className="modelListSelector"
                key="demo"
                type={['right']}
                ease={['easeInOutQuart']}>
                <Header key="a" name={this.props.title} onLeftArrowClick={() => this.goBack()} />
                <div className="modelListSelectorBody" key="b">
                    {
                        this.state.data.map(item =>{
                            let title = item.name + " " + (item.type == 'label' ? '文本' : '数字');
                            return <SwitchLine onChange={(checked)=>this.onSwitchChange(item, checked)} key={item.id} defaultState={item.isEnable} title={title} disabled={!this.state.editable}></SwitchLine>
                        })
                    }
                </div>
            </QueueAnim>
        );
    }

    goBack() {
        this.props.goBack();
    }

    onSwitchChange(item, checked)
    {
        console.log(checked);
        console.log(item);
        let data = this.state.data;
        for(let i = 0; i < data.length; i++)
        {
            if(data[i].id === item.id)
            {
                data[i].isEnable = checked;
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