import * as React from 'react';
import './planitem.css';
import Header from "../../components/common/header/header";
import AutoFlexTextArea from '../../components/common/autoflextextarea/autoflextextarea'
import DatePicker from 'react-mobile-datepicker';
import { ActivityIndicator, Modal, Toast } from 'antd-mobile';
import Dropdown from 'rc-dropdown';
import 'rc-dropdown/assets/index.css';
import Menu, { Item as MenuItem, Divider } from 'rc-menu';
import SingleUserSelector from './singleuserselector/singleuserselector'
import PlanItemSelector from './planitemselector/planitemselector'

const alert = Modal.alert;

export default class PlanItem extends React.Component {
    constructor(props) {
        super(props); 
        let startTime;
        let endTime;
        let itemName;
        let itemWeight;
        if(this.props.location.state.planItem!==null&&this.props.location.state.planItem!==undefined)
        { 
            startTime=new Date(this.props.location.state.planItem.startTime);
            endTime=new Date(this.props.location.state.planItem.endTime);
            itemName=this.props.location.state.planItem.name;
            itemWeight=this.props.location.state.planItem.weight;
        }
        this.state={
            planItemName:itemName||"",
            planItemModel:this.props.location.state.planItemModel||{},
            planItem:this.props.location.state.planItem||{},
            planItemList:this.props.location.state.planItemList||[],
            compContentsDict:[],
            model:this.props.location.state.model,
            startTime: startTime || new Date(),
            endTime: this.props.endTime ||new Date(),
            isOpenStart: false,
            isOpenEnd: false,
            order:this.props.location.state.order,
            role:this.props.location.state.role||"self",
            showSingleUserSelector: false,
            showPlanItemSelector: false,
            fromUserID: "",
            itemWeight:itemWeight||0,
            period: this.props.location.state.period || {},
            quetoState: 0, //0:无1:引用2:参照
            parentPlanItemID: '',
            sourcePlanItemID: ''
        }
    }
    componentWillMount(){
        let compContentsDict=[];
        if(this.state.model=="add")
            this.state.planItemModel.planItemModelContent.map((item,pindex)=>{
                if(item.isEnable)
                   compContentsDict[item.id]={id:item.id,name:item.name,content:""};
            });
        else{
            this.state.planItem.planItemContent.map((item,pindex)=>{
                compContentsDict[item.id]=item;});
        }
        this.setState({
            compContentsDict:compContentsDict
        })
    }
    goBack() 
    {
        console.log("workReport closed.");
        this.props.history.goBack();
    }
    
    changeHandle(id, type, e)
    {
        let compContentsDictTemp = this.state.compContentsDict;
        let value = e.target.value;
        if (type == "number")
        {
            value = value.replace(/^\D*([1-9]\d*\.?\d{0,2})?.*$/, '$1');
        }
        compContentsDictTemp[id].content = value;
        this.setState({
            compContentsDict: compContentsDictTemp
        });
    }
    changeTitleHandle(e)
    {
        let value = e.target.value;
        this.setState({
            planItemName: value
        });
    }
    saveClick(){
        if(this.state.planItemName===null||this.state.planItemName==="")
        {
            alert("请先填写计划项名称。");
            return;
        }
        let planItem;
        let planItemContent=[];
        this.state.planItemModel.planItemModelContent.map((item,pindex)=>
        {
            if(item.isEnable)
               planItemContent.push(this.state.compContentsDict[item.id]);
        });

       planItem={
            id:this.state.planItem.id,
            name:this.state.planItemName,
            startTime:new Date(this.state.startTime),
            endTime:new Date(this.state.endTime),
            order:this.state.order,
            parentPlanItemID: this.state.parentPlanItemID,
            sourcePlanItemID: this.state.sourcePlanItemID,
            planItemContent:planItemContent,
            weight: this.state.itemWeight
       }

       let planItemList;
       planItemList=this.state.planItemList;

       if(this.state.model=="add")
       { 
           let planItemList;
           planItemList=this.state.planItemList;
           planItemList.push(planItem);
           
       }
       else{
           
           planItemList.map(item=>{
               if(item.id==planItem.id)
                   item=planItem;
           })
       }
       window.localStorage.setItem("planItemList", JSON.stringify(planItemList));
       this.goBack();
    }
    cancel(){
        window.localStorage.removeItem("planItemList");
    }
    // 引用或者参照
    confirmPlanItem(planItem)
    {
        if (this.isEmpty(planItem))
        {
            this.setState({
                quetoState: 0
            })
        }
        else
        {
            let parentPlanItemID = '';
            let sourcePlanItemID = '';
            if (this.state.quetoState == 1)
                parentPlanItemID = planItem.id;
            else if (this.state.quetoState == 2)
                sourcePlanItemID = planItem.id;
            let planItemContent = planItem.planItemContent;
            let planItemContentDict = this.state.compContentsDict;
            if (!this.isEmpty(planItemContent))
            {
                planItemContent.forEach(field=>{
                    if (planItemContentDict.hasOwnProperty(field.id))
                        planItemContentDict[field.id].content = field.content;
                });
            }
            alert(this.state.quetoState == 1?'引用确认':'参照确认', 
            this.state.quetoState == 1?'引用计划项将覆盖现有计划项，确定?':'参照计划项将覆盖现有计划项，确定?', [
                { text: '取消', onPress: () => console.log('cancel') },
                {
                    text: '确认', onPress: () => {
                        this.setState({
                            quetoState: 0,
                            planItemName: planItem.name,
                            startTime: new Date(planItem.startTime),
                            endTime: new Date(planItem.endTime),
                            parentPlanItemID,
                            sourcePlanItemID,
                            planItemContentDict,
                            itemWeight: planItem.weight
                        })
                    }
                },
            ]);
        }
    }
    render(){
        if (this.state.showSingleUserSelector)
        {
            return <SingleUserSelector 
                goBack={()=>this.setState({ showSingleUserSelector: false})}
                onItemClick={(userID) => { this.setState({ showSingleUserSelector: false, showPlanItemSelector: true, fromUserID: userID})}}>
                </SingleUserSelector>
        }
        else if(this.state.showPlanItemSelector)
        {
            let periodid = this.isEmpty(this.state.fromUserID) ? this.state.period.parentID : this.state.period.id;
            return <PlanItemSelector onConfirm={(item) => this.confirmPlanItem(item)} periodID={periodid} modelID={this.state.planItemModel.id} userID={this.state.fromUserID} goBack={() => this.setState({ showPlanItemSelector: false, fromUserID: ''})}></PlanItemSelector>
        }
        let isContentReadOnly;
        if(this.state.model==="add"||this.state.model==="edit")
            isContentReadOnly=false;
        else
            isContentReadOnly=true;

       let menu = (
               <Menu onClick={(e)=>this.onClickMenuItem(e)}>
                   <MenuItem key="1">引&nbsp;用</MenuItem>
                   <Divider />
                   <MenuItem key="2">参&nbsp;照</MenuItem>
               </Menu>
            );
        let dropdown =<div></div>
        if(!isContentReadOnly) 
            dropdown= <Dropdown trigger={['click']} overlay={menu} animation="slide-up">
                      <img src={require("../../images/more2x.png")} className="planItem-more"></img>
                      </Dropdown>

        let planItemComp;
        let planItemModelContent=this.state.planItemModel.planItemModelContent.map((item,pindex)=>{
            let inputStyle = null;
            if(item.isEnable)
            {
                return  <div className="plan-planitem" key={pindex}>
                        <AutoFlexTextArea 
                type={item.type}
                title={item.name} 
                value={this.state.compContentsDict[item.id].content}
                onChange={(e) => this.changeHandle(item.id, item.type, e)}
                readonly={isContentReadOnly}></AutoFlexTextArea>
            </div>
            }});
       let timeComp;
       let startTimeCompShowContent = <span className="timeSpan" onClick={() => this.handleClick("start")}>{this.getDateStr3(this.state.startTime)}</span>;
       let endTimeCompShowContent = <span className="timeSpan" onClick={() => this.handleClick("end")}>{this.getDateStr3(this.state.endTime)}</span>;
       let dateFormat = ['YYYY', 'MM', 'DD'];
       let showFormat = 'YYYY年MM月DD日';
       let senderComp = <div></div>
           timeComp=(<div className="planItem-timeShoose">
               <div className="planItem-startTimeChoose" display="none">
                 <span className="planItem-startTime">开始时间</span>
                 <span>{startTimeCompShowContent}</span>
               </div>
               <div className="startTimePicker">
               <DatePicker 
                 value={this.state.startTime}
                 isOpen={this.state.isOpenStart}
                 theme='android'
                 showFormat={showFormat}
                 dateFormat={dateFormat}
                 onSelect={(time) => this.handleSelect(time, "start")}
                 onCancel={() => this.handleCancel("start")} />
               </div>
               <div className="planItem-endTimeChoose" display="none">
               <span className="planItem-endTime">截止时间</span>
               <span>{endTimeCompShowContent}</span>
               </div>
               <div className="endTimePicker">
               <DatePicker
                 value={this.state.endTime}
                 isOpen={this.state.isOpenEnd}
                 theme='android'
                 showFormat={showFormat}
                 dateFormat={dateFormat}
                 onSelect={(time) => this.handleSelect(time, "end")}
                 onCancel={() => this.handleCancel("end")} />
               </div>
              </div> );
                 let saveButton=<div></div>;
                 if(this.state.model!=="view"&&this.state.model!=="compeleteselfassessment")
                     saveButton= <button className="plan-planitem-saveButton" onClick={() => this.saveClick()}>保&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;存</button>
    
        let selfAssessmentComp=<div></div>;
        if(this.state.model==="selfassessment"&&this.state.role==="self")
            selfAssessmentComp=<div className="plan-planitem-selfAssessment">
                <AutoFlexTextArea  title="    自    评" readonly={false} value={this.state.planItem.selfAssessment||""} onChange={(e) => this.changeSelfAssessment(e)}></AutoFlexTextArea>
                <AutoFlexTextArea  title="自 评 分" readonly={false} value={this.state.planItem.selfAssessmentScore||0} onChange={(e) => this.changeSelfAssessmentScore(e)} type="number"></AutoFlexTextArea>
                </div>
        else if(this.state.model==="superiorassessment"||this.state.model==="compeleteselfassessment")
             selfAssessmentComp=<div className="plan-planitem-selfAssessment">
                <AutoFlexTextArea  title="    自    评" readonly={true} value={this.state.planItem.selfAssessment||""} onChange={(e) => this.changeSelfAssessment(e)}></AutoFlexTextArea>
                <AutoFlexTextArea  title="  自 评 分" readonly={true} value={this.state.planItem.selfAssessmentScore||0} onChange={(e) => this.changeSelfAssessmentScore(e)} type="number"></AutoFlexTextArea>
                </div>

       let superiorAssessmentComp=<div></div>;
       if(this.state.model==="superiorassessment"&&this.state.role==="superior")
             superiorAssessmentComp=<div className="plan-planitem-selfAssessment">
                <AutoFlexTextArea  title="上级评价" readonly={false} value={this.state.planItem.assessmentOfSuperior||""} onChange={(e) => this.changeAssessmentOfSuperior(e)}></AutoFlexTextArea>
                <AutoFlexTextArea  title="上级评分" readonly={false} value={this.state.planItem.assessmentScoreOfSuperior||0} onChange={(e) => this.changeAssessmentScoreOfSuperior(e)} type="number"></AutoFlexTextArea>
                 </div>
       else if(this.state.model==="compeleteselfassessment")
            superiorAssessmentComp=<div className="plan-planitem-selfAssessment">
                      <AutoFlexTextArea  title="上级评价" readonly={true} value={this.state.planItem.assessmentOfSuperior||""} onChange={(e) => this.changeAssessmentOfSuperior(e)}></AutoFlexTextArea>
                      <AutoFlexTextArea  title="上级评分" readonly={true} value={this.state.planItem.assessmentScoreOfSuperior||0} onChange={(e) => this.changeAssessmentScoreOfSuperior(e)} type="number"></AutoFlexTextArea>
                       </div>
        return <div className="planItem-form">
          <Header name={"计划项"} onLeftArrowClick={() => this.goBack()} more={dropdown}/>
             
           <div className="plan-planitem-planItemTitle">
           <AutoFlexTextArea  title="标    题" readonly={isContentReadOnly} value={this.state.planItemName} onChange={(e) => this.changeTitleHandle(e)}></AutoFlexTextArea>
           </div>
           {timeComp}
           <div className="plan-planitem-weight">
           <AutoFlexTextArea  title="比    重(%)" readonly={isContentReadOnly} value={this.state.itemWeight} onChange={(e) => this.changeWeightHandle(e)} type="number"></AutoFlexTextArea>
           </div>
           <div className="plan-planitem-content">
           <label className="plan-planitem-contentLabel">计划项内容</label>
           </div>
           <div className="plan-planitem-componentModel">
              {planItemModelContent}
           </div>
           <div className="saveComp">
            {saveButton}
           </div>
           {selfAssessmentComp}
           {superiorAssessmentComp}
           </div>
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
handleClick(type) {
    if(this.state.model!=="add"&&this.state.model!=="edit")
        return;
    if (type == "start")
        this.setState({ isOpenStart: true });
    else if (type == "end")
        this.setState({ isOpenEnd: true });
}
handleSelect(time,type) {       
    if (type == "start") {
        this.setState({ startTime: time, isOpenStart: false });
    }
    else {
        this.setState({ endTime: time, isOpenEnd: false });
    }
}
handleCancel(type) {
    if (type == "start") {
        this.setState({ isOpenStart: false });
    }
    else {
        this.setState({ isOpenEnd: false });
    }
}
changeWeightHandle(e){
    let planItem=this.state.planItem;
    let value=e.target.value;
    value = value.replace(/^\D*([1-9]\d*\.?\d{0,2})?.*$/, '$1');
    planItem.weight=value;
    this.setState({ planItem: planItem,itemWeight:value });
}
changeSelfAssessment(e){
    let planItem=this.state.planItem;
    planItem.selfAssessment=e.target.value;
    this.setState({ planItem: planItem });
}
changeSelfAssessmentScore(e)
{
    let planItem=this.state.planItem;
    planItem.selfAssessmentScore=e.target.value.replace(/^\D*([1-9]\d*\.?\d{0,2})?.*$/, '$1');
    this.setState({ planItem: planItem });
}
changeAssessmentOfSuperior(e)
{
    let planItem=this.state.planItem;
    planItem.assessmentOfSuperior=e.target.value;
    this.setState({ planItem: planItem });
}
changeAssessmentScoreOfSuperior(e)
{
    let planItem=this.state.planItem;
    planItem.assessmentScoreOfSuperior=e.target.value.replace(/^\D*([1-9]\d*\.?\d{0,2})?.*$/, '$1');
    this.setState({ planItem: planItem });
}
onClickMenuItem(e)
{
    console.log(e);
    if(e.key==1)
    {
        // 引用 自己高层次的周期
        this.setState({
            showPlanItemSelector: true,
            quetoState: 1
        })
    }
    else if (e.key==2)
    {
        // 参照 某人的同层次周期
        this.setState({
            showSingleUserSelector: true,
            quetoState: 2
        });
    }
}

    isEmpty(value) {
        return value == null || value == undefined || value === '' || value === "undefined" || (Array.isArray(value) && value.length === 0) || (Object.prototype.isPrototypeOf(value) && Object.keys(value).length === 0);
    }
}