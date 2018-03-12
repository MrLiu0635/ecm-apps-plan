import * as React from 'react';

export default class App extends React.Component {
    constructor(props) {
        super(props);
    }
    render() {
        return (<div>
            <ul>
                <li><a href="/app/plan/web/m/periodconfig">周期定义</a></li>
                <li><a href="/app/plan/web/m/plandefinelist">计划定义列表</a></li>
                <li><a href="/app/plan/web/m/roleselect">我的计划</a></li>
                <li><a href="/app/plan/web/m/allplandynamiclist">计划管理</a></li>
                <li><a href="/app/plan/web/m/planlisthistory">历史计划</a></li>
                <li><a href="/app/plan/web/m/teamplan">团队计划</a></li>
                <li><a href="/app/plan/web/m/weekplan">周计划</a></li>
                <li><a href="/app/plan/web/m/weeklyworkreport">周报</a></li>
                <li><a href="/app/plan/web/m/dailyworkreport">日报</a></li>
            </ul>
        </div>
        );
    }
}