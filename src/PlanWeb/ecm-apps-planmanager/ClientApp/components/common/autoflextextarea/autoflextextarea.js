import * as React from 'react';
import './autoflextextarea.css';


/*
 * title:文本；字段名称
 * maxLength:数字；输入最大长度，默认为20.
 * type:文本（number，text）默认为text.
 * onChange(e): 方法；改变事件回调函数.
 * readonly:布尔；
 */
var countAutoFlexTextArea = 0;
export default class AutoFlexTextArea extends React.Component {
    constructor(props) {
        super(props);
    }
    render() {
        let myid = "content" + countAutoFlexTextArea++;
        let maxLength = !isNaN(this.props.maxLength) && isFinite(this.props.maxLength) ? this.props.maxLength : 20;
        let value=this.props.value;
        return (
            <div className='autoFlexTextArea'>
                <div className="titleDiv"><lable className='title' htmlFor={myid}>{this.props.title || "字段"}</lable></div>
                {
                    this.props.type == 'number' ?
                        this.props.readonly == true ?
                            <div className="contentDiv"><input className='inputContent' readOnly id={myid} placeholder='不用填写' onChange={(e) => this.onNumChange(e)} maxLength={maxLength} value={value}></input></div>
                            : <div className="contentDiv"><input className='inputContent' id={myid} placeholder='点击填写' onChange={(e) => this.onNumChange(e)} maxLength={maxLength} value={value}></input></div>
                        :this.props.readonly == true ?
                            <div className="contentDiv"><textarea className='areaContent' readOnly id={myid} ref='areaContent' placeholder='不用填写' rows="1" onChange={(e) => this.onChange(e)} maxLength={maxLength} value={value}></textarea></div>
                            : <div className="contentDiv"><textarea className='areaContent' id={myid} ref='areaContent' placeholder='点击填写' rows="1" onChange={(e) => this.onChange(e)} maxLength={maxLength} value={value}></textarea></div>
                }
            </div>
        );
    }

    onChange(e)
    {
        this.props.onChange(e);
        this.ResizeTextarea();
    }

    onNumChange(e)
    {
        this.props.onChange(e);
    }

    ResizeTextarea()
    {
        let t = this.refs.areaContent;//拿到了原生DOM 
        if (t.scrollTop == 0) t.scrollTop = 1;
        while (t.scrollTop == 0) {
            if (t.rows > 1)
                t.rows--;
            else
                break;
            t.scrollTop = 1;
            if (t.rows < 3)
        t.style.overflowY = "hidden";
            if (t.scrollTop > 0) {
            t.rows++;
        break;
            }
        }
        while (t.scrollTop > 0) {
            if (t.rows < 3) {
            t.rows++;
        if (t.scrollTop == 0) t.scrollTop = 1;
            } else {
            t.style.overflowY = "auto";
                break;
            }
        }
    }
}