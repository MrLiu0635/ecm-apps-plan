import * as React from 'react';
import './planlist.css';
import Header from '../../components/common/header/header';
import{List} from 'antd-mobile';

const Item = List.Item;

export default class PlanList extends React.Component {
    constructor(props) {
        super(props);
        
        
    }

    showPlan(item)
    {
        this.props.history.push(
           {
               pathname: '/app/plan/web/m/plan',
               state:{
                   planDefineID:item.planDefineID,
                   planID:item.id
               }
           });      
    }

   /* fetchPlanInfo(planid)
    {
        let root = 'app/plan/api/v1.0/plan/' + planid;
        fetch(root, {
            method: 'GET',
            headers:{},
            credentials: 'include'
        }).then(resp =>{
            return resp.json();
        }).then(resjson =>{
            if (resjson ==='' || resjson ===null || resjson ===undefined || resjson ===[]) {return null;}
            else {
                return resjson.data;
                
            }
        });
       
    }*/

    render() {
        let listItems = this.props.location.state.planList.map((item,index) => 
            <Item arrow='horizontal' key={index} onClick={() =>this.showPlan(item)}>
                {item.name}</Item>
            );
        return<div> 
            <Header name={"计划列表"} onLeftArrowClick={()=> this.goBack()}/>
        <div className='planlist-body'>
                <List>
                    {listItems}
                </List>
            </div>
            </div>
    }

   /* showPlan(item)
    {
        let plandefineitem;
        plandefineitem = this.fetchPlanDefine(item.planDefineID);
        let planinfo = this.fetchPlanInfo(item.id);
        if (plandefineitem ==='' || plandefineitem ===null || plandefineitem ===undefined || plandefineitem ===[])
        {return;}
        if (planinfo ==='' || planinfo ===null || planinfo ===undefined || planinfo ===[])
        {return;}
        this.props.history.push(
            {
                pathname: '/app/plan/web/m/plan',
                state:{
                    plandefine:plandefineitem,
                    plan:planinfo
                }
            }
        );
    }*/

    goBack(){
        console.log('planlist is close');
        //imp.iwindow.close();
        this.props.history.goBack();
    }
    
}