import * as React from 'react';
import { Route } from 'react-router-dom';
import { CSSTransitionGroup } from 'react-transition-group'

import PeriodConfig from './app/periodconfig/periodconfig';
import PeriodSet from './app/periodconfig/periodset';
import Plan from './app/plan/plan'
import PlanDefineList from './app/plandefinelist/plandefinelist'
import PlanList from './app/planlist/planlist'
import RoleSelect from './app/roleselect/roleselect'
import DynamicList from './app/plandynamiclist/plandynamiclist'
import PlanDynamicListOfAll from './app/plandynamiclist/plandynamiclistofall'
import PlanDynamic from './app/plandynamiclist/plandynamic'
import UserSelector from './components/userselector/userselector'
import PlanItem from './app/planitem/planitem'
import PlanDefine from './components/plandefine/plandefine'
import PlanListHistory from './app/planlist/planlisthistory'
import TeamPlan from './app/teamplan/teamplanlist'
import WeekPlan from './app/weekplan/weekplan'
import App from './app/app/app'

const styles = `
    .plan-enter {
        opacity: 0.01;
    }

    .plan-enter.plan-enter-active {
        opacity: 1;
        transition: opacity 500ms ease-in;
    }

    .plan-leave {
        opacity: 1;
    }
    .plan-leave.plan-leave-active {
        opacity: 0.01;
        transition: opacity 500ms ease-in;
    }
    .plan-appear {
        opacity: 0.01;
    }
    .plan-appear.plan-appear-active {
        opacity: 1;
        transition: opacity 500ms ease-in;
    }
`;

export const routes = <div>
    <style dangerouslySetInnerHTML={{ __html: styles }} />
    <CSSTransitionGroup
        transitionName="plan"
        transitionEnterTimeout={300}
        transitionLeaveTimeout={300} 
        transitionAppear={true}
        transitionAppearTimeout={500}>
        <Route key={'App'} exact path='/app/plan/web/m' component={App} />
        {/* 1.周期定义 */}
        <Route key={'PeriodConfig'} exact path='/app/plan/web/m/periodconfig' component={PeriodConfig} />
        <Route key={'PeriodSet'} exact path='/app/plan/web/m/periodset' component={PeriodSet} />
        {/* 2.计划定义 */}
        <Route key={'PlanDefine'} exact path='/app/plan/web/m/plandefine' component={PlanDefine} />
        <Route key={'PlanDefineList'} exact path='/app/plan/web/m/plandefinelist' component={PlanDefineList} />
        {/* 3.计划动态 */}
        <Route key={'PlanDynamicListOfAll'} exact path='/app/plan/web/m/allplandynamiclist' component={PlanDynamicListOfAll} />
        <Route key={'PlanDynamic'} exact path='/app/plan/web/m/plandynamic' component={PlanDynamic} />
        <Route key={'DynamicList'} exact path='/app/plan/web/m/dynamiclist' component={DynamicList} />
        {/* 4.我的计划 */}
        <Route key={'RoleSelect'} exact path='/app/plan/web/m/roleselect' component={RoleSelect} />
        <Route key={'Plan'} exact path='/app/plan/web/m/plan' component={Plan} />
        <Route key={'PlanItem'} exact path='/app/plan/web/m/planitem' component={PlanItem} />
        {/* 5.历史计划 */}
        <Route key={'PlanListHistroy'} exact path='/app/plan/web/m/planlisthistory' component={PlanListHistory}/>
        <Route key={'PlanList'} exact path='/app/plan/web/m/planlist' component={PlanList} />
        {/* 6.团队计划 */}
        <Route key={'TeamPlan'} exact path='/app/plan/web/m/teamplan' component={TeamPlan} />
        {/* 7.周计划 */}
        <Route key={'WeekPlan'} exact path='/app/plan/web/m/weekplan' component={WeekPlan} />
        {/* 8.周报 */}
        {/* <Route key={'WeeklyWorkReport'} exact path='/app/plan/web/m/weeklyworkreport' component={WeeklyWorkReport} /> */}
        {/* 9.日报 */}
        {/* <Route key={'DailyWorkReport'} exact path='/app/plan/web/m/dailyworkreport' component={DailyWorkReport} /> */}
        {/* 0.其他组件 */}
        <Route key={'UserSelector'} exact path='/app/plan/web/m/userselector' component={UserSelector} />
    </CSSTransitionGroup>
</div>;
