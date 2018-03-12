import React, { Component } from 'react';
import './DynamicCard.css';

export default class DynamicCard extends React.Component {
    constructor(props) {
        super(props);
        this.state = this.props;
    }
    render() {
        let state;
        if(this.props.dynamicInfo.state=="1")
           state="计划制定";
        else if(this.props.dynamicInfo.state=="2")
           state="执行中";
        else if(this.props.dynamicInfo.state=="3")
           state="计划评价";
        return (
            <div className="dynamicCard" onClick={this.goToPlan.bind(this)}>
                <div className="dynamicInfo">
                    <div className="dynamicName">
                        <span>{this.props.dynamicInfo.planDefine.name}</span>
                    </div>
                    <div className="category">
                        <span>{this.props.dynamicInfo.period.name}</span>
                        <span className="state">{state}</span>
                    </div>
                </div>
            </div>
        );
    }

    goToPlan() {
        this.props.onCardClick();
    }
}