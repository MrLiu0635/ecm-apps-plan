import * as React from 'react';
import './planitemselector.css';
import Header from "../../../components/common/header/header";
import QueueAnim from 'rc-queue-anim';
import { Accordion, List, Checkbox, Modal} from 'antd-mobile';
import 'antd-mobile/lib/accordion/style/css';
import 'antd-mobile/lib/list/style/css';
import 'antd-mobile/lib/checkbox/style/css'
import 'antd-mobile/lib/modal/style/css'
import AutoFlexTextArea from '../../../components/common/autoflextextarea/autoflextextarea'

const CheckboxItem = Checkbox.CheckboxItem;
const alert = Modal.alert;
/**
 * goBack
 */
export default class PlanItemSelector extends React.Component {
    constructor(props) {
        super(props);
        this.state={
            periodID: this.props.periodID || '',
            planList: [],
            userID: this.props.userID || '',
            type: this.isEmpty(this.props.userID)? 0 : 1, //0:代表自己（引用） 1:代表使用userID（参考）
            modelID: this.props.modelID,
            showPlanItemDetial: false,
            planItem:{}
        }
    }
    componentDidMount()
    {
        this.toFetchPlans(this.state.periodID);
    }

    toFetchPlans(periodid){
        let other = '';
        if (this.state.type === 0)
            other = 'state=3&stage=2&mystatus=1'; //引用自己的正在执行的计划项
        else
            other = 'senders[0]=' + this.state.userID; // 参照
        other += '&periodid=' + periodid + '&planitemmodelid='+ this.props.modelID;
        fetch('/app/plan/api/v1.0/plan?' + other, {
            method: 'GET',
            credentials: 'include'
        }).then(response => {
            return response.json();
        }).then(responseData => {
            if (responseData.code == 200)
            {
                if(this.isEmpty(responseData.data))
                {
                    if (this.state.type === 0)
                    {
                        this.toGetParentPeriodPlans(periodid);
                    }
                    else
                    {
                        alert("不存在可参考的计划。")
                    }
                }
                else
                {
                    let planList = responseData.data || [];
                    this.setState({
                        planList
                    })
                }
            }
        });
    }


    toGetParentPeriodPlans(periodID)
    {
        fetch('/app/plan/api/v1.0/period?periodid=' + periodID, {
            method: 'GET',
            credentials: 'include'
        }).then(response => {
            return response.json();
        }).then(responseData => {
            if (responseData.code == 200 && !this.isEmpty(responseData.data)) {
                if (this.isEmpty(responseData.data[0].parentID))
                {
                    alert("不存在可引用的计划。")
                }
                this.toFetchPlans(responseData.data[0].parentID);
            }
        });
    }
    onConfirm()
    {
        let planItem = {};
        let planList = this.state.planList;
        for (let m = 0; m < planList.length; m++)
            if (!this.isEmpty(planList[m].planItems))
                for (let n = 0; n < planList[m].planItems.length; n++)
                    if(planList[m].planItems[n]["checked"])
                        planItem = planList[m].planItems[n];
        if (this.isEmpty(planItem))
        {
            alert('未选择任何计划项。');
            return;
        }
        this.props.onConfirm(planItem);
        this.props.goBack();
    }
    render() {
        let moreFuncComp = (item) => <div><img src={require("../../../images/more-black.png")} className="pull-right more" onClick={() => { this.onItemClick(item) }}></img></div>;
        let more = this.state.showPlanItemDetial?<div></div>:<div className="moreHeader" onClick={()=>this.onConfirm()}>确定</div>;

        return (<div>
            <QueueAnim
                key="demo"
                className="PlanItemSelector"
                type={['right']}
                ease={['easeInOutQuart']}>
                <Header key="a" name={this.state.showPlanItemDetial ? "计划项详细" : "计划项选择"} onLeftArrowClick={() => this.goBack()} more={more}/>
                <div className='PlanItemSelectorBody' key="b">
                    {
                        this.state.showPlanItemDetial?<div>
                            <div className="plan-planitem-planItemTitle">
                                <AutoFlexTextArea title="标题" readonly={true} value={this.state.planItem.name}></AutoFlexTextArea>
                            </div>
                            <div className="planItem-timeShoose">
                                <div className="planItem-startTimeChoose" display="none">
                                    <span className="planItem-startTime">开始时间</span>
                                    <span className="timeSpan">{this.getDateStr3(new Date(this.state.planItem.startTime))}</span>
                                </div>
                                <div className="planItem-endTimeChoose" display="none">
                                    <span className="planItem-endTime">截止时间</span>
                                    <span className="timeSpan">{this.getDateStr3(new Date(this.state.planItem.endTime))}</span>
                                </div>
                            </div>
                            <div className="plan-planitem-weight">
                                <AutoFlexTextArea title="比重(%)" readonly={true} value={this.state.planItem.weight} type="number"></AutoFlexTextArea>
                            </div>
                            <div className="plan-planitem-content">
                                <label className="plan-planitem-contentLabel">计划项内容</label>
                            </div>
                            <div className="plan-planitem-componentModel">
                            {
                                this.state.planItem.planItemContent.map((item,pindex)=>{
                                    if(!this.isEmpty(item.content))
                                    {
                                        return <div className="plan-planitem" key={pindex}>
                                                        <AutoFlexTextArea
                                                            type="text"
                                                            title={item.name}
                                                            value={item.content}
                                                            readonly={true}></AutoFlexTextArea>
                                                    </div>
                                    }})
                            }
                            </div>
                        </div>:
                        <Accordion className="my-accordion" onChange={(key) => this.onChange(key)}>
                                {
                                    this.state.planList.map(item=>
                                        <Accordion.Panel header={item.name} key={item.id}>
                                            <List className="my-list">
                                                {
                                                    this.isEmpty(item.planItems)?<div>正在加载...</div>
                                                    :item.planItems.map(item1=>
                                                        <div className="listItem">
                                                            <div className="iCheckBox">
                                                            <CheckboxItem checked={item1.checked} onChange={(e) => this.onChangeState(e, item1)}>
                                                                {item1.name}
                                                            </CheckboxItem>
                                                            </div>
                                                            {moreFuncComp(item1)}
                                                        </div>
                                                    )
                                                }
                                            </List>
                                        </Accordion.Panel>)
                                }
                        </Accordion>  
                    }           
                </div>
            </QueueAnim>
        </div>
        );
    }

    onItemClick(item)
    {
        console.log(item);
        this.setState({
            showPlanItemDetial: true,
            planItem: item
        });
    }

    onChange(keys)
    {
        if(this.isEmpty(keys))
            return;
        keys.forEach(item=>{
            let planList = this.state.planList;
            for (let m = 0; m < planList.length; m++)
                if (planList[m].id === item)
                {
                    if(this.isEmpty(planList[m].planItems))
                    {
                        //"app/plan/api/v1.0/plan/"+planID
                        let root = "/app/plan/api/v1.0/plan/";
                        fetch(root + item, {
                            method: 'GET',
                            headers: {
                                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                            },
                            credentials: 'include'
                        }).then(resp => {
                            return resp.json();
                        }).then(resjson => {
                            if (resjson.code === 200 && !this.isEmpty(resjson.data)) {
                                let plan = resjson.data || [];
                                let planItems = plan.planItems;
                                for (let n = 0; n < planItems.length; n++) {
                                    planItems[n]["checked"] = false;
                                }
                                planList[m].planItems = planItems;
                                this.setState({
                                    planList
                                });
                        }});
                    }
                    break;
                }
        });
    }

    onChangeState(e, item) {
        let planList = this.state.planList;

        if(e.target.checked)
        {
            for (let m = 0; m < planList.length; m++)
                if(!this.isEmpty(planList[m].planItems))
                for (let n = 0; n < planList[m].planItems.length; n++)
                    if (item.id === planList[m].planItems[n].id)
                        planList[m].planItems[n]["checked"] = true;
                    else
                        planList[m].planItems[n]["checked"] = false;
        }
        else
            for (let m = 0; m < planList.length; m++)
                if (!this.isEmpty(planList[m].planItems))
                    for (let n = 0; n < planList[m].planItems.length; n++)
                        planList[m].planItems[n]["checked"] = false;

        this.setState({
            planList
        })
    }

    goBack() {
        if(this.state.showPlanItemDetial)
            this.setState({showPlanItemDetial:false});
        else
            this.props.goBack();
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