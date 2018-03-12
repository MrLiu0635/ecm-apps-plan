import * as React from 'react';
import Header from '../../components/common/header/header';
import QueueAnim from 'rc-queue-anim';
import TweenOne from 'rc-tween-one';
import './plandefinelist.css';
import { ActivityIndicator, Modal, Toast, Tag } from 'antd-mobile'
import 'antd-mobile/lib/activity-indicator/style/css'
import 'antd-mobile/lib/tag/style/css'
import 'antd-mobile/lib/toast/style/css'
import 'antd-mobile/lib/modal/style/css'

const alert = Modal.alert;

export default class PlanDefineList extends React.Component {
    constructor(props) {
        super(props);
        this.openIndex = null;
        this.position = {};
        this.state = {
            count: 0,
            dataArray: [],
            animation: [],
            style: [],
            pageState: 0 // 0:正常1:加载中2:无数据
        };
        this.onEdit = this.onEdit.bind(this);
        this.onDelete = this.onDelete.bind(this);
        this.onTouchEnd = this.onTouchEnd.bind(this);
        this.onTouchMove = this.onTouchMove.bind(this);
        this.onTouchStart = this.onTouchStart.bind(this);
    }

    componentDidMount() {
        if (window.addEventListener) {
            window.addEventListener('touchend', this.onTouchEnd);
            window.addEventListener('mouseup', this.onTouchEnd);
        } else {
            window.attachEvent('ontouchend', this.onTouchEnd);
            window.attachEvent('onmouseup', this.onTouchEnd);
        }
        this.setState({
            pageState: 1,
            dataArray: []
        })
        // 请求计划定义列表
        let root = '/app/plan/api/v1.0/plandefine';
        fetch(root, {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            console.log(resjson);
            if(resjson.code === 200)
            {
                let data = resjson.data;
                if (data === "" || data === null || data === []) 
                {
                    this.setState({
                        pageState: 2
                    })
                }
                else
                {
                    let count = 0;
                    let dateArr = [];
                    for(let i = 0; i < data.length; i++)
                    {
                        let plandefine = data[i];
                        let imgT = require("../../images/refresh.png");
                        if (plandefine.periodType.code === "year")
                            imgT = require("../../images/year.png");
                        else if (plandefine.periodType.code === "quarter")
                            imgT = require("../../images/quarter.png");
                        else if (plandefine.periodType.code === "month")
                            imgT = require("../../images/month.png");
                        else if(plandefine.periodType.code === "week")
                            imgT = require("../../images/week.png");
                        dateArr.push({
                            img: imgT,
                            text: plandefine.name,
                            key: count++,
                            uniqueFlag: plandefine.id,
                            state: plandefine.state==1?true:false
                        });
                    }
                    this.setState({
                        dataArray: dateArr,
                        pageState: 0,
                        count: count
                    });
                }
            }
            else
            {
                this.setState({
                    pageState: 0
                })
                Toast.fail("查询失败，原因" + resjson.msg || "未知" + ",请重试");
            }
        });
    }

    componentWillUnmount() {
        if (window.addEventListener) {
            window.removeEventListener('touchend', this.onTouchEnd);
            window.removeEventListener('mouseup', this.onTouchEnd);
        } else {
            window.detachEvent('onresize', this.onTouchEnd);
            window.detachEvent('onmouseup', this.onTouchEnd);
        }
    }

    onDelete() {
        let dataArray = this.state.dataArray;
        let deleteData = dataArray.filter(item => item.key === this.openIndex)[0];
        alert('删除确认', '确定删除' + deleteData.text + '?', [
            { text: '取消', onPress: () => console.log('cancel') },
            {
                text: '确认', onPress: () => {
                    this.toDelete(deleteData);
                } 
            },
        ])
    }

    toDelete(deleteData){
        let dataArray = this.state.dataArray;
        console.log("will delete " + deleteData.uniqueFlag);
        fetch('/app/plan/api/v1.0/plandefine/' + deleteData.uniqueFlag,
            {
                method: 'DELETE',
                headers: {
                    // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                },
                credentials: 'include'
            }).then(resp => {
                return resp.json();
            }).then(response => {
                if (response.code === 200) {
                    console.log("OK!");
                    Toast.success('删除成功。', 1);
                    let i = dataArray.indexOf(deleteData);
                    dataArray.splice(i, 1);
                    delete this.state.style[this.openIndex];
                    this.openIndex = null;

                    this.setState({ dataArray });
                } else {
                    Toast.fail("删除失败，原因：" + response.msg);
                }
            }).catch((e) => {
                Toast.fail("删除失败，请重试");
            });
    };

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
        if (this.position[this.index] <= -120) {
            this.openIndex = this.index;
            animation[this.index] = { x: -120, ease: 'easeOutBack' };
        } 
        else if (this.position[this.index] >= 60)
        {
            this.openIndex = this.index;
            animation[this.index] = { x: 60, ease: 'easeOutBack' };
        }
        else {
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
        x = x > 60 ? 60 + (x - 60) * 0.2 : x;
        x = x < -120 ? -120 + (x + 120) * 0.2 : x;
        this.position[this.index] = x;
        const style = this.state.style;
        style[this.index] = { transform: `translateX(${x}px)` };
        const animation = [];
        this.setState({ style, animation });
    };

    onClickAdd()
    {
        console.log("toadd");
        this.props.history.push({
            pathname: '/app/plan/web/m/plandefine',
            state: { bizState: 2 }
        });
    }

    onClickPlanDefine(item)
    {
        console.log(item);
        this.props.history.push({
            pathname: '/app/plan/web/m/plandefine',
            state: { 
                bizState: 0,
                planDefineID: item.uniqueFlag
            }
        });
    }

    onEdit()
    {
        let dataArray = this.state.dataArray;
        let editData = dataArray.filter(item => item.key === this.openIndex)[0];

        this.props.history.push({
            pathname: '/app/plan/web/m/plandefine',
            state: {
                bizState: 1,
                planDefineID: editData.uniqueFlag
            }
        });
    }

    onStateChange(e, state)
    {
        let dataArray = this.state.dataArray;
        let data = dataArray.filter(item => item.key === this.openIndex);
        if (data == null || data.length == 0) return;
        data = data[0];

        alert("变更提醒","确定" + state == 1 ? "启用" : "停用" + "计划定义【" + data.text + "】吗", [
                { text: '取消', onPress: () => console.log('cancel') },
                {
                    text: '确认', onPress: () => {
                        this.updateState(dataArray, data, state);
                    }
                },
            ]);
    }
    
    updateState(dataArray, data, state)
    {
        fetch('/app/plan/api/v1.0/plandefine/state?plandefineid=' + data.uniqueFlag + '&state=' + state,
            {
                method: 'PUT',
                headers: {
                    'Accept': 'application/json, text/plain, */*',
                    'Content-Type': 'application/json',
                    // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                },
                credentials: 'include'
            }).then(resp => {
                return resp.json();
            }).then(response => {
                if (response.code === 200) {
                    console.log("OK!");
                    let cou = dataArray.indexOf(data);
                    dataArray[cou].state = state == 1 ? true : false

                    let animation = this.state.animation;
                    animation[this.openIndex] = { x: 0, ease: 'easeOutBack' };
                    delete this.mouseXY;
                    delete this.position[this.openIndex];
                    this.setState({ animation });

                    this.setState({
                        dataArray,
                        animation
                    })
                    if(state == 1)
                        Toast.success('启用设置成功。', 1);
                    else
                        Toast.success('停用设置完成。', 1);
                } else {
                    Toast.fail("更新失败，请重试");
                }
            }).catch((e) => {
                Toast.fail("更新失败，请重试");
            });
    }

    render() {
        const liChildren = this.state.dataArray.map((item) => {
            const { img, text, key } = item;
            return (<li
                key={key}
                onMouseMove={this.onTouchMove}
                onTouchMove={this.onTouchMove}>
                <div className="plan-definelist-delete">
                    <a onClick={(e) => { this.onDelete(e); }}>删除</a>
                </div>
                <div className="plan-definelist-edit">
                    <a onClick={(e) => { this.onEdit(e); }}>编辑</a>
                </div>
                {   item.state ?
                    <div className="plan-definelist-stop">
                        <a onClick={(e) => { this.onStateChange(e, 2); }}>停用</a>
                    </div> :
                    <div className="plan-definelist-start">
                        <a onClick={(e) => { this.onStateChange(e, 1); }}>启用</a>
                    </div>
                }
                <TweenOne
                    className="plan-definelist-content"
                    onTouchStart={e => this.onTouchStart(e, key)}
                    onMouseDown={e => this.onTouchStart(e, key)}
                    onTouchEnd={this.onTouchEnd}
                    onMouseUp={this.onTouchEnd}
                    onClick={() => this.onClickPlanDefine(item)}
                    animation={this.state.animation[key]}
                    style={this.state.style[key]}>
                    <div className="plan-definelist-img">
                        <img src={img} onDragStart={e => e.preventDefault()} />
                    </div>
                    <span>{text}</span>
                    {
                        item.state ? <Tag small className="tag tagUse">启用</Tag> : <Tag small className="tag tagStop">停用</Tag>
                    }
                </TweenOne>
            </li>);
        });

        let iconAdd = <img src={require("../../images/add_white.png")}
            alt="" className="icon-add pull-right"
            onClick={this.onClickAdd.bind(this)}
        ></img>
        let content = <ActivityIndicator text="正在加载" animating={this.state.pageState === 1} />
        if (this.state.pageState===2)
        {
            content = <div className="noRecord">暂无计划定义记录。</div>
        }
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
        return <div className="plan-definelist">
            <Header name={"计划定义"} onLeftArrowClick={() => this.goBack()} more={iconAdd}/>
            <div className="plan-definelist-body">
            <ActivityIndicator text="正在加载" animating={this.state.pageState === 1}/>
            {content}
            </div>
        </div>
    }

    goBack() {
        console.log("plandefinelist closed.");
        //清空缓存
        imp.iWindow.close();
    }
}