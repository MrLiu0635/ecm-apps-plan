import * as React from 'react';
import './teamplanlist.css';
import Header from '../../components/common/header/header';
import{List} from 'antd-mobile';
import { Tabs, WhiteSpace, Badge } from 'antd-mobile';
import 'antd-mobile/lib/tabs/style/css';
import DynamicCard from '../../components/dynamiccard/dynamiccard';

const Item = List.Item;
const Brief = Item.Brief;

export default class teamplanlist extends React.Component {
    constructor(props)
    {
        super(props);
        this.state={
            currentPlanList:[],
            historyPlanList:[],
            faredPlanList:[],
            planDynamicDic:[]
        }
    }
    componentWillMount()
    {
        let root = 'app/plan/api/v1.0/plan/teamhistory';
        fetch(root, {
            method: 'GET',
            headers:{},
            credentials: 'include'
        }).then(resp =>{
            return resp.json();
        }).then(resjson =>{
            if (resjson ==='' || resjson ===null || resjson ===undefined || resjson ===[]) {return;}
            else {
                this.setState({historyPlanList:resjson.data});
            }
        });
    }

    render()
    {
        let faredPlanListComp=<div></div>;
        if(this.state.faredPlanList!=undefined&&this.state.faredPlanList.length>0)
            faredPlanListComp = this.state.faredPlanList.map((item,index) => {
                let show="("+item.creator.name+")"+item.name; 
                return <Item className="teamplanlist-faredplan" arrow='horizontal' key={index} onClick={() =>this.showPlan(item,index)}>
                {show}</Item>
            });
        let historyPlanListComp=<div></div>;
        if(this.state.historyPlanList!=undefined&&this.state.historyPlanList.length>0)
            historyPlanListComp=  this.state.historyPlanList.map((item,index) => {
                let show="("+item.creator.name+")"+item.name; 
                return <Item className="teamplanlist-historyplan" arrow='horizontal' key={index} onClick={() =>this.showPlan(item,index)}>
                        {show}</Item>
            });
            
               
        let currentPlanListComp=<div></div>
        if(this.state.currentPlanList!=undefined&&this.state.currentPlanList.length>0)
            currentPlanListComp= this.state.currentPlanList.map((item,index) => {
                let stage;
                if(item.state==2)
                    stage="已提交";
                if(item.state==3)
                    stage="审批通过";
                if(item.state==4)
                    stage="审批未通过";
                if(item.stage==2)
                    stage="执行中";
                if(item.stage==3)
                    stage="自评中";
                if(item.stage==4)
                    stage="上级评价中";
                if(item.stage==5)
                    stage="上级评价完成";
                let show="("+item.creator.name+")"+item.name; 
                return <Item multipleLine extra={stage} className="teamplanlist-historyplan" arrow='horizontal' key={index} onClick={() =>this.showPlan(item,index)}>
                        {show}
                         <Brief>{item.period.name}</Brief>
                        </Item>
            });
        const tabs = [
            { title: <Badge>当前计划</Badge> },
            { title: <Badge>历史计划</Badge> },
            { title: <Badge>分享给我</Badge> },
            ];
        const tabTeamPlan = (
            <div>
                <Tabs tabs={tabs}
                    initialPage={1}
                    onChange={(tab, index) => { this.changeTab(index, tab); }}
                    onTabClick={(tab, index) => { this.onTabClick(index, tab); }}
                >
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', backgroundColor: '#fff' }}>
                     <div className="teamplanlist-currentplanList">
                    {currentPlanListComp}
                </div>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', backgroundColor: '#fff' }}>
                <List className="teamplanlist-planList">
                    {historyPlanListComp}
                </List>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', backgroundColor: '#fff' }}>
                <List className="teamplanlist-planList">
                {faredPlanListComp}
                </List>
                </div>
                </Tabs>
                <WhiteSpace />
            </div>);
                return <div>
                     <Header name={"团队计划"} onLeftArrowClick={()=> this.goBack()}/>
                         <div className="teamplanlist-content">{tabTeamPlan}</div>
                      </div>
         }
        changeTab(index, tab){
            if(index==0&&this.state.currentPlanList.length===0)
            {
                let root = 'app/plan/api/v1.0/plan/teamcurrent';
                fetch(root, {
                    method: 'GET',
                    headers:{},
                    credentials: 'include'
                }).then(resp =>{
                    return resp.json();
                }).then(resjson =>{
                    if (resjson ==='' || resjson ===null || resjson ===undefined || resjson ===[]) {return;}
                    else {
                        this.setState({currentPlanList:resjson.data});
                    }
                });
            }
            if(index==2&&this.state.faredPlanList.length===0)
            {
                let faredPlanList=[];
                let root = "app/plan/api/v1.0/plan?mystatus="+"3";
                fetch(root, {
                    method: 'GET',
                    headers: {
                        // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                    },
                    credentials: 'include'
                }).then(resp => {
                    return resp.json();
                }).then(resjson => {
                    if (resjson === "" || resjson === null || resjson === undefined || resjson === []) { return; }
                    else {
                        //考虑是否可以缓存起来？
                    return resjson.data;
                }
                }).then(resdata =>{
                    if (resdata ==='' || resdata ===null || resdata ===undefined || resdata ===[]) {return ;}
                    else {
                        resdata.map(item=>{
                            if(item.stage!==1)
                                faredPlanList.push(item);
                        })
                        this.setState({
                            faredPlanList:faredPlanList
                        });
                    }
                }).catch((e) => {
                    alert("查询失败，请重试");
                });
            }
        }
        onTabClick(index, tab){

        }
        
       
        showPlan(plan,index){
            let role;
            if(index==0||index==1)
                role="superior";
            else
                role="ccRecip";
            let root = 'app/plan/api/v1.0/plandynamic/' + plan.planDefineID+'/'+plan.period.id;
            fetch(root, {
                method: 'GET',
                headers:{},
                credentials: 'include'
            }).then(resp =>{
                return resp.json();
            }).then(resjson =>{
                if (resjson ==='' || resjson ===null || resjson ===undefined || resjson ===[]) {return null;}
                else {
                    //考虑是否可以缓存起来？
                    return resjson.data;
                }
            }).then(resdata =>{
                if (resdata ==='' || resdata ===null || resdata ===undefined || resdata ===[]) {return ;}
                else {
                    this.props.history.push(
                       {
                           pathname: '/app/plan/web/m/plan',
                           state:{
                               planDefineID:plan.planDefineID,
                               dynamicState:resdata,
                               planID:plan.id,
                               role:role
                           }
                       });
                }
            });
        }
      
        goBack(){
            console.log('planlist is close');
            //imp.iwindow.close();
            this.props.history.goBack();
        }
}
