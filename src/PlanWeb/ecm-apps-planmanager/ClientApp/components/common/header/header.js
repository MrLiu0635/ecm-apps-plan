import * as React from 'react';
import './header.css';

export default class Header extends React.Component {
    constructor(props) {
        super(props);
    }
    render() {
        let moreFuncComp = <div></div>;
        let otherStyle = {};
        let filter = <div></div>;
        if (this.props.more !== null && this.props.more !== undefined)
        {
            moreFuncComp = this.props.more;
        }
        else
        {
            otherStyle["marginRight"]="35px"
        }
        return (
            <header className="navbar text-center">
                <img id="quitOut" src={require("../../../images/arrowback-large.png")} alt="" className="pull-left arrowback"
                    onClick={this.props.onLeftArrowClick}></img>
                <span className="navbar-brand header-title-center" style={otherStyle}>{this.props.name}</span>
                {moreFuncComp}
            </header>
        );
    }
}