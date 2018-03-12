import * as React from 'react';
import './planlist.css';
import Header from '../../components/common/header/header';
import{List} from 'antd-mobile';

const Item = List.Item;

 
function getDateStr(date) {
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



export default class PlanListHistory extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            planList:[],
            planGroup:[]

        }
    }
groupPlanByPeriodTypeID(arr){
        var map = {},
        dest = [];
        for(var i = 0; i < arr.length; i++){
            var ai = arr[i];
            if(!map[ai.period.type.code]){
                dest.push({
                    periodTypeCode: ai.period.type.code,
                    name: '',
                    data: [ai]
                });
                map[ai.period.type.code] = ai;
            }else{
                for(var j = 0; j < dest.length; j++){
                    var dj = dest[j];
                    if(dj.periodTypeCode == ai.period.type.code){
                    dj.data.push(ai);
                    break;
                    } 
                }
            }
        }
        return dest;
    }

    componentWillMount(){
        //此处root应该是读取配置拼接版本信息
        const yearPeriod ='7588821c-0c0d-4229-bb0a-0fee9190c028';
        const quarterPeriod ='56acb216-7d3a-42a4-a729-fa54727cf719';
        const monthPeriod ='01f1112e-bafa-4ba6-8fb6-561ec03cbb02';
        const weekPeriod ='e1451a10-330f-4107-afe8-f7a41e4c3395';
        let root = 'app/plan/api/v1.0/plan?MyStatus=1';
        fetch(root, {
            method: 'GET',
            headers:{},
            credentials: 'include'
        }).then(resp =>{
            return resp.json();
        }).then(resjson =>{
            if (resjson ==='' || resjson ===null || resjson ===undefined || resjson ===[]) {return;}
            else {
                this.setState({planList:resjson.data});
                 //对计划按照计划定义ID分组
                this.setState({planGroup: this.groupPlanByPeriodTypeID(this.state.planList)});
            }
        });
       
     }

    render() {
        
        //let listItems = this.state.planDefineList.map((item) => <li>{item.name}</li>);
        let listItems = this.state.planGroup.map((item,index) => 
            <Item arrow='horizontal' key={index} onClick={() =>this.showPlanList(item.data)}>
                {item.data[0].period.type.name+'计划'}</Item>
            );
        return (
        <div>
            <Header name={"历史计划"} onLeftArrowClick={()=> this.goBack()} />
            <div className='planlist-history-body'>
                <List>
                    {listItems}
                </List>
            </div>
        </div>)
    }
    
    showPlanList(items)
    {
        this.props.history.push(
            {
                pathname: '/app/plan/web/m/planlist',
                state:{
                    planList:items
                }
            }
        );
    }

    goBack(){
    console.log('planhistorylist is close');
    imp.iwindow.close();
    }
    
}