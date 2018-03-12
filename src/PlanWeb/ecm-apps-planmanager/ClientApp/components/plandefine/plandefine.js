import * as React from 'react';
import Header from '../../components/common/header/header';
import AutoFlexTextArea from '../../components/common/autoflextextarea/autoflextextarea'
import SwitchLine from '../../components/common/switchline/switchline';
import ListSelector from '../../components/common/listselector/listselector';
import ListOrgSelector from './listorgselector/listorgselector';
import PlanItemModel from './planitemmodel/planitemmodel';
import { ActivityIndicator, Picker, List, Toast  } from 'antd-mobile';
import 'antd-mobile/lib/activity-indicator/style/css';
import 'antd-mobile/lib/picker/style/css';
import 'antd-mobile/lib/list/style/css';
import 'antd-mobile/lib/toast/style/css';
import './plandefine.css';
import '../../css/buttons.css';

export default class PlanDefine extends React.Component 
{
    constructor(props) 
    {
        super(props);
        let bizState = (this.props.location.state || {}).bizState;
        this.state=
        {
            bizState: this.isEmpty(bizState) ? 2 : bizState,//0:查看1:编辑2:新增
            pageState: 0, // 0:正常1:加载中2:无数据
            planDefineID: (this.props.location.state || {}).planDefineID,
            planDefineName: "",
            planDefineState: true, // 0:未知1:启用2:停用
            periodTypes:[],
            periodType:{},
            planItemModels:[],
            planItemModel:{},
            periodSets:[],
            periodSet:{},
            rolesSelected:[],// 结果变量
            orgSelected:[],// 结果变量
            roles:[],// 如果有值就是选择之后的值
            showRoleSelector: false,//展示角色选择
            showOrgSelector: false,//展示组织选择
            showPlanItemModelSelector: false,//展示计划项模板选择
            showModelEditor: false,
            submitLoading: false,
            planItemModelBackUp:{}
        }
    }

    componentDidMount() 
    {
        if(this.state.bizState === 0)// 查看
        {
            this.fetchPlanDefine(this.state.planDefineID);
        }
        else if(this.state.bizState === 1)//编辑
        {
            this.fetchPlanDefine(this.state.planDefineID);
            this.fetchPeriodTypes();
            this.fetchPeriodSets();
        }
         else if (this.state.bizState === 2) //新增
        {
            this.fetchPeriodTypes();
            this.fetchPeriodSets();
        }

    }

    fetchPlanDefine(planDefineID)
    {
        if (this.isEmpty(planDefineID))
            console.log("计划定义标识为空");
        let root = "/app/plan/api/v1.0/plandefine/";
        fetch(root + planDefineID, {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson.code === 200 && !this.isEmpty(resjson.data)) {
                let planDefine = resjson.data;
                let model = {
                    value: planDefine.planModel.id,
                    label: planDefine.planModel.name,
                    planItemModelContent: planDefine.planModel.planItemModelContent
                };
                this.setState({
                    planDefineName: planDefine.name,
                    planDefineState: planDefine.state === 2 ? false : true,
                    periodType:
                    {
                        value: planDefine.periodType.id,
                        label: planDefine.periodType.name
                    },
                    periodSet:
                    {
                        value: planDefine.periodSet.id,
                        label: planDefine.periodSet.name
                    },
                    planItemModel: model,
                    orgSelected: planDefine.orgList.map(item=>({
                        value: item.id,
                        label: item.name
                    })),
                    rolesSelected: planDefine.roleList.map(item=>({
                        value: item.id,
                        label: item.name
                    })),
                    planItemModelBackUp: model
                });
            }
        });
    }
    
    fetchPeriodTypes()
    {
        let self = this;
        let root = '/app/plan/api/v1.0/period';
        fetch(root + "/types", {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson.code === 200 && !this.isEmpty(resjson.data) && resjson.data.length > 0) {
                let myPeriodTypes = this.isEmpty(resjson.data) ? [] : resjson.data.map(item => {
                    return {
                        label: item.name,
                        value: item.id
                    }
                });
                let myPeriodValue = {};
                if (myPeriodTypes.length > 0) {
                    if (this.isEmpty(this.state.periodType))
                        myPeriodValue = myPeriodTypes[0];
                    else
                        myPeriodValue = this.state.periodType
                }
                this.setState({
                    periodTypes: myPeriodTypes,
                    periodType: myPeriodValue
                });
            }
        }).catch((e) => {
            Toast.fail("查询类型失败，请重试");
        });
    }

    fetchPeriodSets()
    {
        let self = this;
        let root = '/app/plan/api/v1.0/period';
        fetch(root + "/sets", {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson.code === 200 && !this.isEmpty(resjson.data) && resjson.data.length > 0) {
                let myPeriodSets = this.isEmpty(resjson.data) ? [] : resjson.data.map(item => {
                    return {
                        label: item.name,
                        value: item.id
                    }
                });
                let myPeriodSet = {};
                if (myPeriodSets.length > 0) {
                    if (this.isEmpty(this.state.periodSet))
                        myPeriodSet = myPeriodSets[0];
                    else
                        myPeriodSet = this.state.periodSet
                }
                this.setState({
                    periodSets: myPeriodSets,
                    periodSet: myPeriodSet
                });
            }
        }).catch((e) => {
            Toast.fail("查询计划项模板失败，请重试");
        });
    }

    onModelClick(item)
    {
        let planItemModelsShow = this.state.planItemModels;
        let flag = false;
        let i = 0
        for (; i < planItemModelsShow.length; i++)
        {
            if (planItemModelsShow[i].value === item.value)
            {
                if (this.isEmpty(planItemModelsShow[i].planItemModelContent))//未查询过
                    break;
                planItemModelsShow[i]["show"] = true;
                this.setState({
                    showModelEditor: true,
                    planItemModels: planItemModelsShow,
                    showPlanItemModelSelector: false
                })
                return;
            }
        }
        let self = this;
        let plandefineID = "&plandefineid="+this.state.planDefineID;
        if (this.state.bizState === 2)
            plandefineID = "";
        let root = '/app/plan/api/v1.0/planitemmodel';
        fetch(root + "?modelID=" + item.value + plandefineID, {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson.code === 200 && !this.isEmpty(resjson.data)) {
                let planItemModelShow = resjson.data;
                let old = planItemModelsShow[i];
                planItemModelsShow[i] = {
                    value: old.value, 
                    label: old.label, 
                    checked: item.checked,
                    show: true,
                    planItemModelContent: planItemModelShow.planItemModelContent
                };
                this.setState({
                    showModelEditor: true,
                    planItemModels: planItemModelsShow,
                    showPlanItemModelSelector: false 
                })
            }
        });
    }

    render()
    {
        let mainContent;
        let disabled = this.state.bizState === 0;
        if(this.state.showRoleSelector)
        {
            mainContent = <div className="plan-define">
                <ListSelector 
                    title="角色选择" 
                    disabled={disabled}
                    dataList={this.state.roles} 
                    goBack={() => this.setState({ showRoleSelector: false, rolesSelected: this.state.roles.filter(item => item.checked)})}>
                </ListSelector>
            </div>
        }
        else if(this.state.showOrgSelector)
        {
            mainContent = <div className="plan-define">
                <ListOrgSelector
                    title="组织"
                    disabled={disabled}
                    selectedValueList={this.state.orgSelected || []}
                    goBack={() => this.setState({ showOrgSelector: false })}
                    parentOrgID='root'>
                </ListOrgSelector>
            </div>
        }
        else if(this.state.showPlanItemModelSelector)
        {
            let more = (item)=>{
                return <div><img src={require("../../images/more-black.png")} className="pull-right more" onClick={() => { this.onModelClick(item) }}></img></div>
            }
            mainContent = <div className="plan-define">
                <ListSelector
                    title="计划项模板选择"
                    disabled={disabled}
                    dataList={this.state.planItemModels}
                    goBack={() => this.setState(()=>{
                        let models = this.state.planItemModels.filter(item => item.checked);
                        let modelT = {};
                        if (!this.isEmpty(models))
                            modelT = models[0];
                        return ({ showPlanItemModelSelector: false, planItemModel: modelT})})}
                    isSingle={true}
                    more={more}>
                </ListSelector>
            </div>
        }
        else if(this.state.showModelEditor)
        {
            let planItemModels = this.state.planItemModels;
            let planItemModelShow = planItemModels.find(item => item["show"]);
            mainContent = <div className="plan-define">
                <PlanItemModel
                    title={planItemModelShow.label}                    
                    dataList={planItemModelShow.planItemModelContent}
                    goBack={()=>{
                        planItemModelShow["show"] = false;
                        this.setState({
                            showModelEditor: false,
                            showPlanItemModelSelector: true,
                            planItemModels
                        })
                    }}
                    editable={planItemModelShow.checked && !disabled}>
                </PlanItemModel>
            </div>
        }
        else
        {
            let myPeriodTypes = this.state.periodTypes || [];
            let myPeriodType = this.state.periodType || {};
            let myPeriodSets = this.state.periodSets || [];
            let myPeriodSet = this.state.periodSet || {};
            if (this.isEmpty(myPeriodTypes))
                myPeriodTypes =  [myPeriodType];
            if (this.isEmpty(myPeriodSets))
                myPeriodSets = [myPeriodSet];

            let roleShow = "请选择";
            if (!this.isEmpty(this.state.rolesSelected))
                roleShow = this.state.rolesSelected.map(item => item.label.substring(0, 2)).join(',');
            let orgShow = "请选择";
            if (!this.isEmpty(this.state.orgSelected))
                orgShow = this.state.orgSelected.map(item => item.label.substring(0, 2)).join(',');

            mainContent = <div className="plan-define">
                <Header name={"计划"} onLeftArrowClick={() => this.goBack()} />
                <div className="plan-define-body">
                    <div>
                        <div className="fillScope"><span>基本信息</span></div>
                    </div>
                    <ActivityIndicator text="正在加载" animating={this.state.pageState === 1} />
                    <AutoFlexTextArea title="名称" readonly={disabled} onChange={(e) => this.onNameChange(e)} maxLength={20} value={this.state.planDefineName}></AutoFlexTextArea>
                    <div className="priodSetDiv">
                        <Picker data={myPeriodSets} disabled={disabled} cols={1} value={[myPeriodSet.value]} onChange={v => this.onSetChange(v)} className="forss2">
                            <List.Item arrow="horizontal">周期集</List.Item>
                        </Picker>
                    </div>
                    <div className="priodTypeDiv">
                        <Picker data={myPeriodTypes} cols={1} disabled={disabled} value={[myPeriodType.value]} onChange={v => this.onTypeChange(v)} className="forss1">
                            <List.Item arrow="horizontal">周期类型</List.Item>
                        </Picker>
                    </div>
                    <div className="planItemModelDiv" onClick={()=>this.onPlanItemModelClick()}>
                        <div className="am-list-item am-list-item-middle">
                            <div className="am-list-line">
                                <div className="am-list-content">计划项模板</div>
                                <div className="am-list-extra">{this.state.planItemModel.label || "请选择"}</div>
                                <div className="am-list-arrow am-list-arrow-horizontal" aria-hidden="true"></div>
                            </div>
                        </div>
                    </div>
                    <SwitchLine title="是否启用" disabled={disabled} onChange={(checked) => this.onSwitchClick(checked)} defaultState={this.state.planDefineState}></SwitchLine>
                    <div>
                        <div className="fillScope"><span>填报范围</span></div>
                    </div>
                    <div className="roleSelectorDiv" onClick={() => this.onRoleSelectorClick()}>
                        <div className="am-list-item am-list-item-middle">
                            <div className="am-list-line">
                                <div className="am-list-content">角色模板</div>
                                <div className="am-list-extra">{roleShow}</div>
                                <div className="am-list-arrow am-list-arrow-horizontal" aria-hidden="true"></div>
                            </div>
                        </div>
                    </div>
                    <div className="orgSelectorDiv" onClick={() => this.onOrgSelectorClick()}>
                        <div className="am-list-item am-list-item-middle">
                            <div className="am-list-line">
                                <div className="am-list-content">组织模板</div>
                                <div className="am-list-extra">{orgShow}</div>
                                <div className="am-list-arrow am-list-arrow-horizontal" aria-hidden="true"></div>
                            </div>
                        </div>
                    </div>
                    <ActivityIndicator
                        toast
                        text="正在提交，请稍候..."
                        animating={this.state.submitLoading}
                    />
                    {
                        disabled?
                        <div></div>:
                        <div className="submitComp">
                            <button
                                className="submitButton button button-pill button-primary"
                                onClick={() => this.submitHandleClick()}>保&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;存</button>
                        </div>
                    }
                </div>
            </div>
        }
        return mainContent;
    }

    dataValidate(planModel)
    {
        // 数据验证
        if (this.isEmpty(this.state.planDefineName))
        {
            Toast.info("请填写计划定义的名称。");
            return false;
        }
        if (this.isEmpty(this.state.periodSet) || this.isEmpty(this.state.periodSet.value))
        {
            Toast.info("请按需求选择计划定义的周期集。");
            return false;
        }
        if (this.isEmpty(this.state.periodType) || this.isEmpty(this.state.periodType.value)) {
            Toast.info("请按需求选择计划定义的周期类型。");
            return false;
        }
        if (this.isEmpty(planModel) && this.isEmpty(this.state.planItemModel))
        {
            Toast.info("请按需求选择计划定义关联的模板。");
            return false;
        }
        if (this.isEmpty(this.state.rolesSelected)) 
        {
            Toast.info("请按需求选择需制定计划的角色。");
            return false;
        }
        if (this.isEmpty(this.state.orgSelected))
        {
            Toast.info("请按需求选择需制定计划的组织。");
            return false;
        }
        return true;
    }

    submitHandleClick()
    {
        let planModel = this.state.planItemModels.find(item=>item.checked) || {};
        if (!this.dataValidate(planModel)) return;
        let state = this.state.planDefineState ? 1 : 2; // 1:启用2:停用
        let allContent = planModel.planItemModelContent || [];

        // 更新时处理计划项模板
        if (!this.isEmpty(planModel) && this.isEmpty(allContent) && planModel.value === this.state.planItemModelBackUp.value)// 选了，未点
            allContent = this.state.planItemModelBackUp.planItemModelContent
        else if (this.isEmpty(planModel) && !this.isEmpty(this.state.planItemModel))//没有选
        {
            planModel = this.state.planItemModelBackUp,
            allContent = planModel.planItemModelContent || [];
        }

        let customizedModelContent = [];
        allContent.filter(item=>!item.isEnable).forEach(item=>{
            customizedModelContent.push({
                id: item.id,
                isEnable: item.isEnable
            })
        });
        let orgList = this.state.orgSelected.map(item=> ({
            id: item.value
        }));
        let roleList = this.state.rolesSelected.map(item => ({
            id: item.value
        }));

        let planDefine = {
            id: this.state.planDefineID,
            name: this.state.planDefineName,
            state: state,
            planModel: 
            {
                    id: planModel.value,
                    //planItemModelContent: planModel.planItemModelContent
            },
            periodSet:
            {
                id:this.state.periodSet.value
            },
            periodType:
            {
                id:this.state.periodType.value
            },
            planItemCustomization: { customizedModelContent },
            orgList: orgList,
            roleList: roleList
        }

        console.log(planDefine);
        if(this.state.bizState === 2)
            this.toSavePlanDefine(planDefine);
        else if(this.state.bizState === 1)
            this.toUpdatePlanDefine(planDefine);
    }

    toUpdatePlanDefine(planDefine)
    {
        this.setState({
            submitLoading: true
        });
        fetch('/app/plan/api/v1.0/plandefine',
            {
                method: 'PUT',
                headers: {
                    'Accept': 'application/json, text/plain, */*',
                    'Content-Type': 'application/json',
                    // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                },
                body: JSON.stringify(planDefine),
                credentials: 'include'
            }).then(resp => {
                return resp.json();
            }).then(response => {
                console.log(response);
                this.setState({
                    submitLoading: false
                });
                if (response.code === 200) {
                    console.log("OK!");
                    Toast.success('更新成功。', 1);
                    this.setState({
                        bizState: 0
                    });
                } else {
                    Toast.fail("更新失败，请重试");
                }
            }).catch((e) => {
                this.setState({
                    submitLoading: false
                });
                Toast.fail("更新失败，请重试");
            });
    }

    toSavePlanDefine(planDefine){
        this.setState({
            submitLoading: true
        });
        fetch('/app/plan/api/v1.0/plandefine',
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json, text/plain, */*',
                    'Content-Type': 'application/json',
                    // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
                },
                body: JSON.stringify(planDefine),
                credentials: 'include'
            }).then(resp => {
                return resp.json();
            }).then(response => {
                console.log(response);
                this.setState({
                    submitLoading: false
                });
                if (response.code === 200) {
                    console.log("OK!");
                    Toast.success('保存成功。', 1);
                    this.setState({
                        bizState: 0
                    });
                } else {
                    Toast.fail("保存失败，请重试");
                }
            }).catch((e) => {
                this.setState({
                    submitLoading: false
                });
                Toast.fail("保存失败，请重试");
            });
    }

    onPlanItemModelClick() {
        if (!this.isEmpty(this.state.planItemModels)) // 已经打开过就不再获取了
        {
            this.setState({
                showPlanItemModelSelector: true
            });
            return;
        }
        let self = this;
        let root = '/app/plan/api/v1.0/planitemmodel';
        fetch(root + "/all", {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson.code === 200 && !this.isEmpty(resjson.data) && resjson.data.length > 0) {
                let models = resjson.data;
                models = models.map(item => {
                    let checked = false;
                    if (!this.isEmpty(this.state.planItemModel) && this.state.planItemModel.value === item.id)
                        checked = true;
                    return { value: item.id, label: item.name, checked: checked }
                });
                this.setState({
                    planItemModels: models,
                    showPlanItemModelSelector: true
                });
            }
        });
    }

    onRoleSelectorClick()
    {
        if(!this.isEmpty(this.state.roles)) // 已经打开过就不再获取了
        {
            this.setState({
                showRoleSelector: true
            });
            return;
        }
        let self = this;
        let root = '/app/plan/api/v1.0/common';
        fetch(root + "/role", {
            method: 'GET',
            headers: {
                // 'Authorization': 'bearer AT-8987991-b1zYhpOFP3rkGwf7ONsYbzaW6omayvCbvhU'
            },
            credentials: 'include'
        }).then(resp => {
            return resp.json();
        }).then(resjson => {
            if (resjson.code === 200 && !this.isEmpty(resjson.data) && resjson.data.length > 0) {
                let roles = resjson.data;
                this.setState({
                    roles:roles.map(item=>{
                        let checked = false;
                        if(!this.isEmpty(this.state.rolesSelected) && this.state.rolesSelected.filter(item2=>item2.value===item.id).length > 0)
                            checked = true;
                        return { value: item.id, label: item.name, checked: checked}
                    }),
                    showRoleSelector:true
                });
            }
        });
    }

    onOrgSelectorClick()
    {
        this.setState({
            parentOrgID: 'root',
            showOrgSelector: true
        })
    }

    onSwitchClick(checked)
    {
        let state = checked;
        this.setState({
            planDefineState: state
        });
    }

    onNameChange(e)
    {
        let name = e.target.value;
        this.setState({
            planDefineName: name
        });
    }

    onTypeChange(v)
    {
        console.log(v);
        if(this.isEmpty(v))return;
        let typeID = v[0];
        let types = this.state.periodTypes;
        let type = {};
        for (let i = 0; i < types.length; i++)
        {
            if (typeID === types[i].value)
            {
                type = {
                    value: typeID,
                    label: types[i].label
                }
                break;
            }
        }

        this.setState({
            periodType: type
        });
    }

    onSetChange(v) {
        console.log(v);
        if (this.isEmpty(v)) return;
        let setID = v[0];
        let sets = this.state.periodSets;
        let set = {};
        for (let i = 0; i < sets.length; i++) {
            if (setID === sets[i].value) {
                set = {
                    value: setID,
                    label: sets[i].label
                }
                break;
            }
        }
        this.setState({
            periodSet: set
        });
    }

    goBack()
    {
        console.log("plandefine closed.");
        this.props.history.goBack();
    }

    isEmpty(value) {
        return value == null || value == undefined || value === "" || value == "undefined" || (Array.isArray(value) && value.length === 0) || (Object.prototype.isPrototypeOf(value) && Object.keys(value).length === 0);
    }
}