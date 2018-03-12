import * as React from 'react';
import './plandynamiclist.css';
import DynamicCard from '../../components/dynamiccard/dynamiccard';
import Header from "../../components/common/header/header";

export default class PlanDynamicList extends React.Component {
    constructor(props) {
        super(props);
        let returnYunPlusFlag=false;
        if (this.props.location.key) {
            returnYunPlusFlag=true;
        }
        this.state={
            roleID:this.props.location.state.roleID||"",
            dynamicList:[],
            position:[],
            selectedDynamicInfo:{}
        }
    }
    componentWillMount(){
        let root = "app/plan/api/v1.0/plandynamic/";
        fetch(root+this.state.roleID, {
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
                this.setState({dynamicList:resjson.data})
            }
        }).catch((e) => {
            alert("查询失败，请重试");
        });
    }


    render() {
        let dynamicList = this.state.dynamicList;
        let dynamicDiv;
        let pos;
        pos = this.getPositions(dynamicList);
        if(this.isDynamicListNull(dynamicList)){
            dynamicDiv = <div></div>
        }
        else
        {
           dynamicDiv= pos.map((item, pindex) => {
               let dynamicListComp;
               let dynamicListInThisPosition=this.getDynamicThisPosition(pindex);
               dynamicListComp=<DynamicCard key={pindex} dynamicInfo={dynamicListInThisPosition} onCardClick={()=>this.getSelectedDynamic(dynamicListInThisPosition)} />;
              return (<div className="positionDiv" key={pindex}>
                    <div className="subList">{dynamicListComp}</div>
                    </div>);
         });
        }
        return <div>
                <Header name={"计划列表"} onLeftArrowClick={() => this.goBack()} />
               <div className="dynamicList"> 
               {dynamicDiv}
               </div>
            </div>
            }

    goBack() {
        console.log("plan closed.");
        if (this.state.returnYunPlusFlag) {
            imp.iWindow.close();
        }
        else {
            this.props.history.goBack();
        }
    }
    getDynamicThisPosition(index) {
        return this.state.dynamicList[index];
    }
   getPositions(dynamicList) {
       let pos = [];
       for(let i in dynamicList) {
           pos.push(i);
       }
       return pos;
   }
   getSelectedDynamic(dynamicInfo) {
      
       this.setState({
           selectedDynamicInfo: dynamicInfo
       });
       let planRoot;
       let plan;
       planRoot = "app/plan/api/v1.0/plan?mystatus=1&periodid=" + dynamicInfo.period.id + "&plandefines[0]=" + dynamicInfo.planDefine.id;
       fetch(planRoot, {
           method: 'GET',
           headers: {
               // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
           },
           credentials: 'include'
       }).then(resp => {
           return resp.json();
       }).then(resjson => {
           if (resjson === "" || resjson === null || resjson === undefined || resjson === []||resjson.data===null)
           {
                  this.goToPlan(dynamicInfo,"");
           }
           else {
               plan=resjson.data[0];
               this.goToPlan(dynamicInfo,plan.id);
           }
       }).catch((e) => {
           alert("查询失败，请重试");
       });
   }
   goToPlan(dynamicInfo,planID) {
       this.props.history.push({
           pathname: '/app/plan/web/m/plan',
           state: {
               planDefineID: dynamicInfo.planDefine.id,
               planID:planID,
               dynamicState:dynamicInfo.state,
               period:dynamicInfo.period
           }
       });
   }
   isDynamicListNull(dynamicList){
       for(let i in dynamicList) {
           if(i === undefined) continue;
           else {
               return false;
           }
       };
       return true;
   }
}