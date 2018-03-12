import * as React from 'react';
import './plan.css';
import Header from "../../components/common/header/header";
import 'antd-mobile/lib/image-picker/style/css';
import 'antd-mobile/lib/switch/style/css';
import 'antd-mobile/lib/action-sheet/style/css';
import fetchJsonp from 'fetch-jsonp';
import AutoFlexTextArea from '../../components/common/autoflextextarea/autoflextextarea'
import TweenOne from 'rc-tween-one';
import QueueAnim from 'rc-queue-anim';
import { ActivityIndicator, Modal, Toast } from 'antd-mobile'

const alert = Modal.alert;

export default class RoleSelect extends React.Component {
    constructor(props) {
        super(props); 
        this.position = {};
        this.openIndex = null;
        
    
        this.state={
            planDefineID:this.props.location.state.planDefineID||"",
            dynamicState:this.props.location.state.dynamicState||0,
            planDefine:{},
            planID:this.props.location.state.planID||"",
            planItemModel:{},
            mxList:[],
            ccList:[],
            animation: [],
            style:[],
            planItemList:[],
            pageState: 0,
            isCanChange:false,
            planName:"",
            role:this.props.location.state.role||"self",
            planStage:1,
            planState:1,
            period:this.props.location.state.period||{}
        }
        this.onDelete = this.onDelete.bind(this);
        this.onTouchEnd = this.onTouchEnd.bind(this);
        this.onTouchMove = this.onTouchMove.bind(this);
        this.onTouchStart = this.onTouchStart.bind(this);
    }

    componentWillMount(){
        let planDefineID=this.props.location.state.planDefineID;
        let planDefine;
        if(planDefineID!==undefined&&planDefineID!==null&&planDefineID!=="")
        {
            let planRoot;
            planRoot= "app/plan/api/v1.0/planDefine/"+planDefineID;
            fetch(planRoot, {
                method: 'GET',
                headers: {
                    // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                },
                credentials: 'include'
            }).then(resp => {
                return resp.json();
            }).then(resjson => {
                if (resjson === "" || resjson === null || resjson === undefined || resjson === []||resjson.data==null)
                   return;  
                else{
                    planDefine=resjson.data;
                    this.setState({planDefine:planDefine, planItemModel:planDefine.planModel });
                }
            });
        }
        let planID=this.props.location.state.planID;
        let plan;
        let mxListTemp = [];
        let ccListTemp = [];
        let planItemList=[];
        let planName="";
        if(planID!==undefined&&planID!==null&&planID!=="")
        {
            let planRoot;
            planRoot= "app/plan/api/v1.0/plan/"+planID;
            fetch(planRoot, {
                method: 'GET',
                headers: {
                    // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                },
                credentials: 'include'
            }).then(resp => {
                return resp.json();
            }).then(resjson => {
                if (resjson === "" || resjson === null || resjson === undefined || resjson === []||resjson.data==null)
                { 
                    if(window.localStorage.getItem("mxList")!==undefined&&window.localStorage.getItem("mxList")!==null)
                        mxListTemp = JSON.parse(window.localStorage.getItem("mxList"));
                    if(window.localStorage.getItem("ccList")!==undefined && window.localStorage.getItem("ccList")!==null)
                        ccListTemp = JSON.parse(window.localStorage.getItem("ccList"));
                    if(window.localStorage.getItem("planItemList")!==undefined&&window.localStorage.getItem("planItemList")!==null)
                        planItemList=JSON.parse(window.localStorage.getItem("planItemList"));
                    if(window.localStorage.getItem("planName")!==undefined&&window.localStorage.getItem("planName")!==null)
                        planName=JSON.parse(window.localStorage.getItem("planName"));
                    this.setState({mxList:mxListTemp,ccList:ccListTemp,planItemList:planItemList,planName:planName,planStage:1,planState:1});
                    if(this.state.dynamicState===1&&(this.props.location.state.role==undefined||this.props.location.state.role=="self"))
                        this.setState({isCanChange:true});
                }
                else {
                    plan=resjson.data;
                    if(window.localStorage.getItem("mxList")!==undefined&&window.localStorage.getItem("mxList")!==null)
                        mxListTemp = JSON.parse(window.localStorage.getItem("mxList"));
                    else if(plan!==undefined && plan!=={}&&plan.mainRecipient!==undefined && plan.mainRecipient.id!==undefined)
                        mxListTemp.push(plan.mainRecipient);
                    if(window.localStorage.getItem("ccList")!==undefined && window.localStorage.getItem("ccList")!==null)
                        ccListTemp = JSON.parse(window.localStorage.getItem("ccList"));
                    else if(plan!==undefined &&plan!=={}&&plan.carbonCopyRecipient!==undefined&&plan.carbonCopyRecipient!==null)
                        plan.carbonCopyRecipient.map(item=>{
                            if(item!==undefined&&item!==null&&item.id!==undefined)
                                ccListTemp.push(item)}
                            );
                    if(window.localStorage.getItem("planItemList")!==undefined&&window.localStorage.getItem("planItemList")!==null)
                        planItemList=JSON.parse(window.localStorage.getItem("planItemList"));
                    else if(plan!==undefined &&plan!=={}&&plan.planItems!==undefined&&plan.planItems!==[])
                        planItemList= plan.planItems;
 
                    if(window.localStorage.getItem("planName")!==undefined&&window.localStorage.getItem("planName")!==null)
                        planName=JSON.parse(window.localStorage.getItem("planName"));
                    else if(plan!==undefined && plan!=={})
                        planName= plan.name;
                    this.setState({mxList:mxListTemp,ccList:ccListTemp,planItemList:planItemList,planName:planName,planStage:plan.stage,planState:plan.state,period:plan.period});
           
                    if(this.state.dynamicState===1&&(this.state.planState===1||this.state.planState===4)
                        &&(this.props.location.state.role==undefined||this.props.location.state.role=="self"))
                        this.setState({isCanChange:true});
                }
            }).catch((e) => {
                alert("查询失败，请重试");
            });
        }
        else
        {
            if(window.localStorage.getItem("mxList")!==undefined&&window.localStorage.getItem("mxList")!==null)
                mxListTemp = JSON.parse(window.localStorage.getItem("mxList"));
            if(window.localStorage.getItem("ccList")!==undefined && window.localStorage.getItem("ccList")!==null)
                ccListTemp = JSON.parse(window.localStorage.getItem("ccList"));
            if(window.localStorage.getItem("planItemList")!==undefined&&window.localStorage.getItem("planItemList")!==null)
                planItemList=JSON.parse(window.localStorage.getItem("planItemList"));
            if(window.localStorage.getItem("planName")!==undefined&&window.localStorage.getItem("planName")!==null)
                planName=JSON.parse(window.localStorage.getItem("planName"));
            this.setState({mxList:mxListTemp,ccList:ccListTemp,planItemList:planItemList,planName:planName,planStage:1,planState:1});
            if(this.state.dynamicState===1)
                this.setState({isCanChange:true});
        }
    }
    render(){ 
        let isReadOnly=!this.state.isCanChange;
        
        let recent = this.state.mxList|| [];
        let recentNames = [];
        recent.forEach(element =>{
            if(element!==undefined&&element!==null) 
                recentNames.push(element.name); 
        });
        let recentStr = recentNames.join(',');

        let  mainRecpTextArea;
        if(isReadOnly)
            mainRecpTextArea=(<textarea  className="areaContent" readOnly placeholder="不用填写"  value={recentStr}></textarea>);
        else
           mainRecpTextArea=   (<textarea  className="areaContent" placeholder="请点击选择"  value={recentStr}
               onClick={() => this.showMainRecpUserSelector()}  onChange={(e)=>console.log(e)}></textarea>);
        let mainRecpComp = 
        <div className="paln-componentModel">
            <div className="paln-recipHeader">
                 <lable className="recipHeader">主送</lable>
            </div>
            <div className="paln-recipContent">
                {mainRecpTextArea}
            </div>
           </div>

        let ccRecent = this.state.ccList|| [];
        let recentCCNames = [];
        ccRecent.forEach(element => {
            if(element!==undefined&&element!==null)
               recentCCNames.push(element.name);
        });
        let recentCCStr = recentCCNames.join(',');

        let  ccRecpTextArea;
        if(isReadOnly)
            ccRecpTextArea= (<textarea  className="areaContent" readOnly placeholder="不用填写"  value={recentCCStr}></textarea>);
        else
           ccRecpTextArea= (<textarea  className="areaContent" placeholder="请点击选择"  value={recentCCStr}
               onClick={() => this.showCCUserSelector()}  onChange={(e)=>console.log(e)}></textarea>);

       let ccRecpComp = 
        <div className="paln-componentModel">
            <div className="paln-recipHeader">
                <lable className="recipHeader">抄送</lable>
            </div>
            <div className="paln-recipContent">
            {ccRecpTextArea}
            </div>
       </div>
            let dynamicState= this.state.dynamicState;
            let planStage=this.state.planStage;
            let planState=this.state.planState;
            let role =this.state.role;
            let stage;
            if(planStage==1)
                stage="制定中";
            if(planState==2)
                stage="已提交";
            if(planState==3)
                stage="审批通过";
            if(planState==4)
                stage="审批未通过";
            if(planStage==2)
                stage="执行中";
            if(planStage==3)
                stage="自评中";
            if(planStage==4)
                stage="上级评价中";
            if(planStage==5)
                stage="上级评价完成";
            
            let stageComp=<div className="plan-stageComp">
                 <AutoFlexTextArea title="状态" value={stage} readonly={true}></AutoFlexTextArea>
              </div>
            const liChildren = this.state.planItemList.map((item,pindex) => {
            const  text  = item.name;
            const  key  = pindex;
            return (
                <li key={key} onMouseMove={this.onTouchMove}  onTouchMove={this.onTouchMove}>
                <div className="plan-delete">
                     <a onClick={(e) => { this.onDelete(e); }}>删除</a>
                </div>
               <TweenOne
                className="plan-content"
                 onTouchStart={e => this.onTouchStart(e, key)}
                onMouseDown={e => this.onTouchStart(e, key)}
                onTouchEnd={this.onTouchEnd}
                onMouseUp={this.onTouchEnd}
                animation={this.state.animation[key]}
                onClick={e =>this.onEditItem(key)}
                style={this.state.style[key]}>
                <span>{text}</span>
                </TweenOne>
            </li>);
        });

        let content = <ActivityIndicator text="正在加载" animating={this.state.pageState === 1} />
        if (this.state.pageState===2)
            content = <div className="noRecord">暂无计划项。</div>
        else
        {
            content = <QueueAnim
            component="ul"
            animConfig={[
            { opacity: [1, 0], translateY: [0, 30] },
            { height: 0 },
            ]}
            ease={['easeOutQuart', 'easeInOutQuart']}
            duration={[550, 450]}
            interval={150}
            >{liChildren}
            </QueueAnim>
        }
        let saveComp=<div></div>
        if(((dynamicState===1&&(planState===1||planState===4)||(dynamicState===3&&planStage===3))&&role=="self")||
                (dynamicState===3&&planStage===4&&role=="superior"))
                saveComp= <div className="saveComp">
                   <button  className="plan-saveButton" onClick={() => this.saveClick()}>保&nbsp;&nbsp;&nbsp;存</button>
                   <button  className="plan-submitButton" onClick={() => this.submitClick()}>提&nbsp;&nbsp;&nbsp;交</button>
                 </div>
           if(dynamicState===1&&planState===2&&role=="superior")
                saveComp= <div className="saveComp">
                  <button  className="plan-saveButton" onClick={() => this.passSubmitClick(true)}>审批通过</button>
                  <button  className="plan-submitButton" onClick={() => this.passSubmitClick(false)}>审批不通过</button>
                </div>
        return <div className="planMakerPage">
               <Header name={"计划制定"} onLeftArrowClick={() => this.goBack()} />
               <div className="paln-componentModels">
                    {mainRecpComp}
                    {ccRecpComp}
               <div className="plan-planName">
                    <AutoFlexTextArea title="名称" value={this.state.planName} onChange={(e)=>this.onNameChange(e)} readonly={isReadOnly}></AutoFlexTextArea>
               </div>
               {stageComp}
               <div className="paln-planItem">
                    <div className="paln-planItemCategory">{"计划项"}</div>
                    <div className="paln-planItemList">
                          <ActivityIndicator text="正在加载" animating={this.state.pageState === 1}/>
                          {content}
                          <div className="paln-addPlanItem">
                              <img className="addPlanItem" src={require("../../images/add.png")} onClick={()=>this.addPlanItem()}></img>
                           </div>
                     </div>
                </div>
                </div>
               {saveComp}
         </div>
      }
         saveClick(func){
             if(this.state.dynamicState===1&&(this.state.planState===1||this.state.planState===4))
             {
                 let plan={
                     id:this.state.planID,
                     Period:this.state.period,
                     MainRecipient:this.state.mxList[0],
                     CarbonCopyRecipient:this.state.ccList,
                     Name:this.state.planName,
                     PlanItems:this.state.planItemList,
                     PlanDefineID:this.state.planDefine.id,
                     State:this.state.planState,
                     Stage:this.state.planStage
                 }
                
                 let root = "app/plan/api/v1.0/plan";
                 fetch(root, {
                     method: 'POST',
                     headers: {
                         'Accept': 'application/json, text/plain, */*',
                         'Content-Type': 'application/json',
                         // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                     },
                     body: JSON.stringify(plan),
                     credentials: 'include'
                 }).then(resp => {
                     return resp.json();
                 }).then(resjson => {
                     if (resjson === "" || resjson === null || resjson === undefined || resjson === []) { return; }
                     else {
                         if(resjson.code !== 200)
                             alert(resjson.msg); 
                         else
                         { 
                             this.setState({planID:resjson.data})
                             if(func == null || func == undefined)
                             {
                                alert("保存成功。")
                            }
                            else
                            {
                                func(resjson.data, 2);
                            }
                            }
                        }
                 });
             }
             else if(this.state.dynamicState===3&&this.state.planStage===3)
             {
                 let root = "app/plan/api/v1.0/plan/selfassessment/";
                 fetch(root, {
                     method: 'PUT',
                     headers: {
                         'Accept': 'application/json, text/plain, */*',
                         'Content-Type': 'application/json',
                         // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                     },
                     body: JSON.stringify(this.state.planItemList),
                     credentials: 'include'
                 }).then(resp => {
                     return resp.json();
                 }).then(resjson => {
                     if (resjson === "" || resjson === null || resjson === undefined || resjson === []) { return; }
                     else {
                         if(resjson.code !== 200)
                             alert(resjson.msg); 
                         else
                         { 
                             if(func == null || func == undefined)
                             {
                                 alert("保存成功。")
                             }
                             else
                             {
                                 func(resjson.data, 4);
                             }
                         }
                     }
                 });
             }
             else if(this.state.dynamicState===3&&this.state.planStage===4)
             {
                 let root = "app/plan/api/v1.0/plan/superiorassessment/";
                 fetch(root, {
                     method: 'PUT',
                     headers: {
                         'Accept': 'application/json, text/plain, */*',
                         'Content-Type': 'application/json',
                         // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                     },
                     body: JSON.stringify(this.state.planItemList),
                     credentials: 'include'
                 }).then(resp => {
                     return resp.json();
                 }).then(resjson => {
                     if (resjson === "" || resjson === null || resjson === undefined || resjson === []) { return; }
                     else {
                         if(resjson.code !== 200)
                             alert(resjson.msg); 
                         else
                         { 
                             if(func == null || func == undefined)
                             {
                                 alert("保存成功。")
                             }
                             else
                             {
                                 func(resjson.data, 5);
                             }
                         }
                     }
                 });
             }
         }
        
         toSubmit(planID, approvalState)
         { 
             //计划制定阶段计划内容提交(新增的和审批未通过的)
             if (this.state.dynamicState === 1 && (this.state.planState === 1 || this.state.planState===4))
             {
                 let root = "app/plan/api/v1.0/plan/approval/";
                 fetch(root + planID + '/' + approvalState, {
                     method: 'PUT',
                     headers: {
                         'Accept': 'application/json, text/plain, */*',
                         'Content-Type': 'application/json',
                         // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                     },
                     credentials: 'include'
                 }).then(resp => {
                     return resp.json();
                 }).then(resjson => {
                     if (resjson === "" || resjson === null || resjson === undefined || resjson === []) { return;
                     }
                     else {
                         if(resjson.code !== 200)
                             return; 
                         alert("提交成功。");
                         this.setState({planState:2});
                     }
                 });
             }
                 //自评阶段提交自评及评分和上级提交评价和评分，只是修改计划的stage
             else if((this.state.dynamicState===3&&this.state.planStage===3)
                 ||(this.state.dynamicState===3&& this.state.planStage===4&&this.state.role==="superior"))
             {
                 let root = "app/plan/api/v1.0/plan/stage/";
                 fetch(root + this.state.planID + '/' + approvalState, {
                     method: 'PUT',
                     headers: {
                         'Accept': 'application/json, text/plain, */*',
                         'Content-Type': 'application/json',
                         // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                     },
                     credentials: 'include'
                 }).then(resp => {
                     return resp.json();
                 }).then(resjson => {
                     if (resjson === "" || resjson === null || resjson === undefined || resjson === []) { return;
                     }
                     else {
                         if(resjson.code !== 200)
                             return; 
                         alert("提交成功。");
                         this.setState({planStage:approvalState});
                     }
                 });
             }
         }
         passSubmitClick(isPass){
             let planState=isPass?3:4;
             let root = "app/plan/api/v1.0/plan/approval/";
             fetch(root + this.state.planID + '/' + planState, {
                 method: 'PUT',
                 headers: {
                     'Accept': 'application/json, text/plain, */*',
                     'Content-Type': 'application/json',
                     // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                 },
                 credentials: 'include'
             }).then(resp => {
                 return resp.json();
             }).then(resjson => {
                 if (resjson === "" || resjson === null || resjson === undefined || resjson === []) { return;
                 }
                 else {
                     if(resjson.code !== 200)
                         return; 
                     this.setState({planState:planState});
                 }
             });
             if(isPass)
             {
                 let root = "app/plan/api/v1.0/plan/stage/";
                 fetch(root + this.state.planID + '/' + 3, {
                     method: 'PUT',
                     headers: {
                         'Accept': 'application/json, text/plain, */*',
                         'Content-Type': 'application/json',
                         // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                     },
                     credentials: 'include'
                 }).then(resp => {
                     return resp.json();
                 }).then(resjson => {
                     if (resjson === "" || resjson === null || resjson === undefined || resjson === []) { return;}
                     else {
                         if(resjson.code !== 200)
                         { 
                             alert(resjson.msg);
                             return; 
                         }
                         alert("提交成功。");
                         this.setState({planStage:2});
                     }
                 });
             }
             else{
                 alert("提交成功。");
             }
         }
         submitClick(){
             this.saveClick(this.toSubmit.bind(this));
         }
     onNameChange(e){
         let value = e.target.value;
         window.localStorage.setItem("planName",JSON.stringify(value));
         this.setState({planName:value});
     }
     showMainRecpUserSelector()
     {
         if(!this.state.isCanChange)
             return;
         if(this.state.mxList!==undefined&&this.state.mxList!==null)
             window.localStorage.setItem("mxList", JSON.stringify(this.state.mxList));
         this.props.history.push({
             pathname: '/app/plan/web/m/userSelector',
             state: {
                 userModel: "主送"
             },
             'callback': this.resetMainRecip.bind(this)
         });
     }
      resetMainRecip(){
        let plan;
        let mainRecipient=[];
        plan=this.state.plan;
        let ccList=window.localStorage.getItem("mxList");
        ccList.map((item,pindex)=>{
            mainRecipient.push(item); 
        });
        plan.mainRecipient=mainRecipient;
        this.setState({
            plan:plan
         })
        }
      showCCUserSelector()
      {
          if(!this.state.isCanChange)
              return;
          if(this.state.ccList!==undefined&&this.state.ccList!==null)
              window.localStorage.setItem("ccList", JSON.stringify(this.state.ccList));
          this.props.history.push({
              pathname: '/app/plan/web/m/userSelector',
              state: {
                  userModel: "抄送",
                  localStore:"ccList"
              },
              'callback': this.resetCCRecip.bind(this)
          });
      }
   resetCCRecip(){
            let plan;
            let carbonCopyRecipient=[];
            plan=this.state.plan;
            let ccList=window.localStorage.getItem("ccList");
            ccList.map((item,pindex)=>{
                carbonCopyRecipient.push(item); 
            });
            plan.carbonCopyRecipient=carbonCopyRecipient;
            this.setState({
                plan:plan
            })
        }
   addPlanItem(){
       if(!this.state.isCanChange)
           return;
        this.props.history.push({
        pathname: '/app/plan/web/m/planitem',
        state: {
             planItemModel: this.state.planItemModel,
             planItemList:this.state.planItemList,
             model:"add",
             order:this.state.planItemList.length,
             period: this.state.period
             }
           });
     }
     
    goBack() 
    {
        console.log("plan closed.");
        //清空缓存
        window.localStorage.removeItem("planItemList");
        window.localStorage.removeItem("mxList");
        window.localStorage.removeItem("ccList");
        window.localStorage.removeItem("planName");
        if (this.state.returnYunPlusFlag) 
        {
            imp.iWindow.close();
        }
        else
        {
            this.props.history.goBack();
        }
    }
    onDelete() {
        if(!this.state.isCanChange)
            return;
        let dataArray = this.state.planItemList;
        let deleteData = dataArray[this.openIndex];
        alert('删除确认', '确定删除?', [
            { text: '取消', onPress: () => console.log('cancel') },
            {text: '确认', onPress: () => this.toDelete(deleteData)}
        ]);
    }
    
    toDelete(deleteData){
        let planItemList=this.state.planItemList;
        let i = planItemList.indexOf(deleteData);
        planItemList.splice(i, 1);
        delete this.state.style[this.openIndex];
        this.openIndex = null;
        window.localStorage.setItem("planItemList",planItemList);
        this.setState({
            planItemList:planItemList
        });
    };
    onEditItem(i){
        let model;
        //计划制定阶段为可编辑，其它状态计划项内容不可编辑
        if(!this.state.isCanChange)
            model="view";
        else
            model="edit";
        //自评填写状态
        if(this.state.dynamicState===3&&this.state.planStage===3)
            model="selfassessment";
        //上级评价状态
        if(this.state.dynamicState===3&&this.state.planStage===4)
            model="superiorassessment";
        //上级评价完成状态
        if(this.state.planStage===5)
            model="compeleteselfassessment";
        let dataArray = this.state.planItemList;
        let editData = dataArray[i];
        this.props.history.push({
            pathname: '/app/plan/web/m/planitem',
            state: {
                planItemModel: this.state.planItemModel,
                planItemList:this.state.planItemList,
                planItem:editData,
                model:model,
                order:editData.Order,
                role:this.state.role
            }
        });
    }
    onTouchStart(e, i) {
        if (this.openIndex || this.openIndex === 0) {
            const animation = this.state.animation;
            animation[this.openIndex] = { x: 0, ease: 'easeOutBack' };
            this.setState({ animation }, () => {
                delete this.state.style[this.openIndex];
            });
            this.openIndex = null;
            return;
        }
        this.index = i;
        this.mouseXY = {
            startX: e.touches === undefined ? e.clientX : e.touches[0].clientX,
        };
    };

    onTouchEnd() {
        if (!this.mouseXY) {
            return;
        }
        const animation = this.state.animation;
        if (this.position[this.index] <= -60) {
            this.openIndex = this.index;
            animation[this.index] = { x: -60, ease: 'easeOutBack' };
        } else {
            animation[this.index] = { x: 0, ease: 'easeOutBack' };
        }

        delete this.mouseXY;
        delete this.position[this.index];
        this.index = null;
        this.setState({ animation });
        };

    onTouchMove(e) {
        if (!this.mouseXY) {
            return;
        }
        const currentX = e.touches === undefined ? e.clientX : e.touches[0].clientX;
        let x = currentX - this.mouseXY.startX;
        x = x > 10 ? 10 + (x - 10) * 0.2 : x;
        x = x < -60 ? -60 + (x + 60) * 0.2 : x;
        this.position[this.index] = x;
        const style = this.state.style;
        style[this.index] = { transform: `translateX(${x}px)` };
        const animation = [];
        this.setState({ style, animation });
        }
    }