import React, { Component } from 'react';
import './tabs.css';

class Tabs extends Component {
    constructor(props) {
        super(props);
        this.state = {
            tabs: this.props.tabs,
            selectedTab: this.props.queryState
        };
    }
    render() {
        let selectedTab = this.state.selectedTab;
        let tabDivs;
        tabDivs = this.state.tabs.map((item, index) => {
            //className可以根据tabs的个数自动选择，还未实现
            return (
                <div className={`twoCols ${selectedTab === item ? 'active' : ''}`}
                    key={index} onClick={this.onTabChange.bind(this, item)}>{item}
                </div>
            );
        });
        return (
            <div>
                <div className="nav-tabs">
                    {tabDivs}
                </div>
            </div>
        );
    }

    onTabChange(tabName) {
        let selectedTab = tabName;
        this.setState({
            selectedTab: tabName
        });
        this.props.callBackParent(selectedTab);

    }


}

export default Tabs