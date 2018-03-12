import * as React from 'react';
import Header from '../../components/common/header/header';
import './weekplan.css';
import { List } from 'antd-mobile';
import 'antd-mobile/lib/list/style/css';
import DatePicker from 'react-mobile-datepicker';

const Item = List.Item;
const periodType = 'week';
const dateFormat = ['YYYY', 'MM', 'DD'];
const showFormat = 'YYYY年MM月DD日';
export default class WeekPlan extends React.Component
{
    constructor(props)
    {
        super(props);
        this.state =
        {
            planList: [],
            date: new Date(),
            isOpen: false
        }
        this.planCount = 0;
        this.daySelected = 0;
    }

    componentWillMount() 
    {
        this.fetchPeriods();
    }

    fetchPeriods()
    {
        let time = this.formatDate(this.state.date, 'yyyy-MM-dd hh:mm:ss');
        let queryCondition = 'time=' + time + '&periodtypecode=' + periodType
        let root = 'app/plan/api/v1.0/period?' + queryCondition;
        fetch(root, {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson.code === 200)
            {
                if (this.isEmpty(resjson.data))
                {
                    alert("不存在符合要求的周期定义。");
                    return;
                }
                this.fetchPlans(resjson.data.map(item=>item.id));
            }
            else
                alert("查询失败，请重试");
        }).catch((e) => {
            alert("查询失败，请重试");
        });
    }

    fetchPlans(periodids)
    {
        let periodStr = '';
        for(let i = 0; i < periodids.length ; i++)
        {
            periodStr += '&periods[' + i + ']=' + periodids;
        }
        let queryCondition = 'mystatus=1' + periodStr
        let root = 'app/plan/api/v1.0/plan?' + queryCondition;
        fetch(root, {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson.code === 200) {
                if (this.isEmpty(resjson.data)) {
                    return;
                }
                this.planCount = resjson.data.length;
                this.fetchPlanDetail(resjson.data)
            }
            else
                alert("查询失败，请重试");
        }).catch((e) => {
            alert("查询失败，请重试");
        });
    }

    fetchPlanDetail(planList)
    {
        planList.forEach(plan=>{
            planRoot = "app/plan/api/v1.0/plan/" + plan.id;
            fetch(planRoot, {
                method: 'GET',
                headers: {
                    // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                },
                credentials: 'include'
            }).then(resp => {
                return resp.json();
            }).then(resjson => {
                if (resjson.code === 200) {
                    if (this.isEmpty(resjson.data)) {
                        return;
                    }
                    let planList = this.state.planList;
                    planList.push(resjson.data);
                    this.setState({
                        planList
                    });
                }
                else
                    alert("查询失败，请重试");
            }).catch((e) => {
                alert("查询失败，请重试");
            });
        });
    }

    render()
    {
        let planList = this.state.planList;
        if (this.planCount !== planList.length) return;

        // 日期
        let timeCompShowContent = <span className="dateSpan" onClick={() => this.handleDateClick()}>{this.getDateStr3(this.state.date)}</span>

        // 提交
        let more = <div className="moreHeader" onClick={() => this.onSubmitClick()}>提交</div>;
        
        return <div className="weekPlan">
            <Header name="周计划" onLeftArrowClick={() => this.goBack()} more={more} />
            <div className="weekPlanBody">
                <DatePicker
                    value={this.state.date}
                    isOpen={this.state.isOpen}
                    theme='android'
                    showFormat={showFormat}
                    dateFormat={dateFormat}
                    onSelect={(date) => this.handleDateSelect(date)}
                    onCancel={() => this.handleDateCancel()} />
                <div className="timeComp">
                    <img className="arrowLeft" src={require("../../images/arrowleft-large.png")} onClick={() => this.handleClickTimeLeft()}></img>
                    {timeCompShowContent}
                    <img className="arrowRight" src={require("../../images/arrowright-large.png")} onClick={() => this.handleClickTimeRight()}></img>
                </div>
                <List className="my-list">
                    <Item extra="extra content" arrow="horizontal" onClick={() => { console.log('1') }}>主送人</Item>
                    <Item extra="extra content" arrow="horizontal" onClick={() => { console.log('2') }}>抄送人</Item>
                </List>
                <div className="dayOfWeek"></div>
                <div className="planItemList">
                    <div className="planItem">
                    </div>
                </div>
            </div>
        </div>
    }

    handleDateSelect(date)
    {
        console.log(date);
        this.setState({ isOpen : false, date });
    }

    // 点击取消选择时间
    handleDateCancel()
    {
        this.setState({ isOpen: false });
    }

    // 点击日期
    handleDateClick()
    {
        console.log("点击日期");
        this.setState({ isOpen : true });
    }

    // 提交
    onSubmitClick()
    {
        console.log("提交");
    }

    // 左 时间
    handleClickTimeLeft()
    {
        let date = this.state.date;
        date = new Date(date.getTime() - 24 * 60 * 60 * 1000 * 7);
        this.setState({
            date
        });
    }

    // 右 时间
    handleClickTimeRight()
    {
        let date = this.state.date;
        date = new Date(date.getTime() + 24 * 60 * 60 * 1000 * 7);
        this.setState({
            date
        });
    }
    // --------------------工具---------------
    formatDate(date, fmt) {
        let o = {
            "M+": date.getMonth() + 1, //月份 
            "d+": date.getDate(), //日 
            "h+": date.getHours(), //小时 
            "m+": date.getMinutes(), //分 
            "s+": date.getSeconds(), //秒 
            "q+": Math.floor((date.getMonth() + 3) / 3), //季度 
            "S": date.getMilliseconds() //毫秒 
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (let k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
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

    isEmpty(value) {
        return value == null || value == undefined || value === '' || value === "undefined" || (Array.isArray(value) && value.length === 0) || (Object.prototype.isPrototypeOf(value) && Object.keys(value).length === 0);
    }
}