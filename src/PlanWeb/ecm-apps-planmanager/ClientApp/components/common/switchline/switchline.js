import * as React from 'react';
import './switchline.css';
import { Switch } from 'antd-mobile';
import 'antd-mobile/lib/switch/style/css';

/**
 * disabled: 布尔值，是否停用，默认为不停用
 * defaultState: 布尔值，初始状态，默认为关
 * title: 文本，默认为字段
 */
export default class SwitchLine extends React.Component {
    constructor(props) {
        super(props);
        let state = this.props.defaultState == true ? true : false;
        let disabled = this.props.disabled == true ? true: false;
        this.state={
            state: state,
            disabled: disabled
        }
    }
    componentWillReceiveProps(nextProps) {
        this.setState({
            state: nextProps.defaultState,
            disabled: nextProps.disabled
        });
    }
    render() {
        return (
            <div className='switchDiv'>
                <div className="titleDiv"><lable className='title'>{this.props.title || "字段"}</lable></div>
                <div className="contentDiv">
                    {
                        <Switch 
                            disabled={this.state.disabled}
                            checked={this.state.state}
                            onClick={(checked) => this.onSwitchClick(checked)}
                        ></Switch>
                    }
                </div>
            </div>
        );
    }
    onSwitchClick(checked)
    {
        if (this.props.onChange)
            this.props.onChange(checked);
    }
}