import * as React from 'react';
import './daysinweek.css';

export default class Daysinweek extends React.Component {
    constructor(props) {
        super(props);
        let currentDate = this.props.date;
        if (this.isEmpty(currentDate))
            currentDate = new Date();
        let dateArray = this.dealDate(currentDate);
        let type = this.isEmpty(this.props.type)? 0 : 1;
        this.weekShows = type == 0 ? [{ 0: "周一" }, { 1: "周二" }, { 2: "周三" }, { 3: "周四" }, { 4: "周五" }, { 5: "周六" }, { 6: "周日" }] : [{ 0: "周日" }, { 1: "周一" }, { 2: "周二" }, { 3: "周三" }, { 4: "周四" }, { 5: "周五" }, { 6: "周六" }];
        this.state=
        {
            date: currentDate,
            dateArray
        }
    }

    dealDate(date)
    {
        let week = date.getDay();
        let millisecond = 1000 * 60 * 60 * 24;
        let minusDay = week != 0 ? week - 1 : 6;
        let dayArray = [];
        for(let i = 0; i<7;i++)
        {
            dayArray.push({
                day: new Date(date.getTime() - (millisecond * (minusDay-i))).getDate(),
                checked: minusDay - i == 0? true: false,
                week: this.weekShows[i]
            })
        }
        return dayArray;
    }

    render() {
        return (<div className="daysInWeek">
            {
                dateArray.map(item=>{
                    item.checked?
                        <div className="normalDiv"><span>{item.week}</span><span>{item.day}</span></div>:
                    <div></div>
                })
            }
        </div>);
    }
    isEmpty(value) {
        return value == null || value == undefined || value === '' || value === "undefined" || (Array.isArray(value) && value.length === 0) || (Object.prototype.isPrototypeOf(value) && Object.keys(value).length === 0);
    }
    getDateStr3(date) {
        let year = "";
        let month = "";
        let day = "";
        let now = date;
        year = "" + now.getFullYear();
        if ((now.getMonth() + 1) < 10) {
            month = "0" + (now.getMonth() + 1);
        } else {
            month = "" + (now.getMonth() + 1);
        }
        if ((now.getDate()) < 10) {
            day = "0" + (now.getDate());
        } else {
            day = "" + (now.getDate());
        }
        return year + "-" + month + "-" + day;
    }
}