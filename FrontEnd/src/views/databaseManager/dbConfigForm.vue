<template>
    <el-dialog title="数据库配置" :visible.sync="dialogVisible" width="70%" :before-close="handleClose">
        <el-button type="primary" @click="addNew">新增</el-button>
        <el-tabs v-model="activeTab" tab-position="left" style="height: auto;">

            <el-tab-pane v-for="item in dbConfigData" :label="item.connectName">
            </el-tab-pane>
            <el-form :model="dbForm" ref="dbForm" label-width="100px" class="demo-ruleForm">
                <el-form-item label="类型" prop="dbType" :rules="[
                    { required: true, message: '请选择数据库类型' },
                ]">
                    <el-select v-model="dbForm.dbType" placeholder="请选择">
                        <el-option v-for="item in ['mssql', 'mysql']" :key="item" :label="item" :value="item">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="连接名" prop="connectName" :rules="[
                    { required: true, message: '连接名不能为空' },
                ]">
                    <el-input v-model="dbForm.connectName" autocomplete="off"></el-input>
                </el-form-item>
                <el-form-item label="主机Server" prop="dbServer" :rules="[
                    { required: true, message: '主机名不能为空' },
                ]">
                    <el-input v-model="dbForm.dbServer" autocomplete="off"></el-input>
                </el-form-item>
                <el-form-item label="数据库名" prop="dbName" :rules="[
                    { required: true, message: '不能为空' },
                ]">
                    <el-input v-model="dbForm.dbName" autocomplete="off"></el-input>
                </el-form-item>
                <el-form-item label="用户名" prop="dbUserId" :rules="[
                    { required: true, message: '用户名不能为空' },
                ]">
                    <el-input v-model="dbForm.dbUserId" autocomplete="off"></el-input>
                </el-form-item>
                <el-form-item label="密码" prop="dbPwd" :rules="[
                    { required: true, message: '密码不能为空' },
                ]">
                    <el-input type="password" v-model="dbForm.dbPwd" autocomplete="off"></el-input>
                </el-form-item>

                <el-form-item>
                    <el-button type="primary" @click="submitForm('dbForm')">保存</el-button>
                    <el-button type="primary" :loading="isBtnLoading" @click="testDb">测试连接</el-button>
                    <el-popconfirm @confirm="confirmDel" confirm-button-text='好的' cancel-button-text='取消'
                        icon="el-icon-info" icon-color="red" title="确定删除吗？">
                        <!-- <el-button slot="reference">删除</el-button> -->
                        <el-button style="margin-left: 10px;" slot="reference" type="danger"
                            icon="el-icon-delete"></el-button>

                    </el-popconfirm>
                </el-form-item>
            </el-form>
        </el-tabs>

        <span slot="footer" class="dialog-footer">
            <el-button @click="dialogVisible = false">取 消</el-button>
            <!-- <el-button type="primary" @click="dialogVisible = false">确 定</el-button> -->
        </span>
    </el-dialog>
</template>
<script>
export default {
    props: {
        dbConfigData: {
            type: [],
            required: true
        },
    },
    data() {
        return {
            isBtnLoading: false,
            activeTab: '-1',
            dbFormInitData: {
                dbType: 'mssql',
                dbName: 'master'
            },
            dbForm: {},
            dialogVisible: false,
        };
    },
    watch: {
        dbConfigData() {
            console.log('watch dbConfigData', this.dbConfigData)
            //数据变了，重新赋值
            // this.dbForm = JSON.parse(JSON.stringify(this.dbConfigData[this.activeTab]))
            this.resetVal()
        },
        //tab切换，form赋值
        activeTab() {
            console.log('activeTab')
            // this.dbForm = JSON.parse(JSON.stringify(this.dbConfigData[this.activeTab]))
            this.resetVal()
        }
    },
    methods: {
        //重新赋值dbForm
        resetVal() {
            if (parseInt(this.activeTab) > -1 && this.dbConfigData && this.dbConfigData.length > 0) {
                this.dbForm = JSON.parse(JSON.stringify(this.dbConfigData[this.activeTab]))

            }
        },
        confirmDel() {
            console.log('confirmDel', this.dbForm)
            //没有id直接删除
            if (!this.dbForm.id) {
                this.dbConfigData.splice(this.activeTab, 1)
            } else {
                //有id从数据库删除
                this.$http
                    .post("/api/DbManager/DelById", this.dbForm)
                    .then((res) => {
                        if (res.succeeded) {

                            this.$message.success('删除成功')
                            this.$emit('loadMyDbs')
                        }
                    });
            }
            this.activeTab = '0'
        },
        //新增
        addNew() {
            console.log('addnew')
            //是否已经有新增
            if (this.dbConfigData.some(x => !x.id)) {
                this.$message.error("有新增没有保存")
                return
            }
            this.dbForm = JSON.parse(JSON.stringify(this.dbFormInitData))
            this.dbForm.connectName = '新增'
            this.dbConfigData.push(this.dbForm)
            //定位到最后一个
            this.activeTab = String(this.dbConfigData.length - 1)
            console.log('addnew this.dbConfigData', this.dbConfigData)
        },
        show(currentSelectDbIndex) {
            this.dialogVisible = true
            //选中对应tab
            this.activeTab = String(currentSelectDbIndex)
            console.log('show currentSelectDbIndex', currentSelectDbIndex)
        },

        resetForm(formName) {
            this.$refs[formName].resetFields();
        },
        //测试连接
        testDb() {
            var that = this
            console.log('submit', that.dbForm)
            this.isBtnLoading = true
            that.$http
                .post("/api/DbManager/TestConnectDb", that.dbForm)
                .then((res) => {
                    this.isBtnLoading = false
                    if (res.succeeded) {
                        if (res.data) {
                            that.$message.success("连接成功");
                        }
                        else {
                            that.$message.error("连接失败");
                        }
                    } else {
                        that.$message.error("连接失败:" + res.errors);
                    }
                });

        },
        submitForm(formName) {
            var that = this
            console.log('sss', this)
            this.$refs[formName].validate((valid) => {
                if (valid) {
                    console.log('submit', that.dbForm)
                    that.$http
                        .post("/api/DbManager/Save", that.dbForm)
                        .then((res) => {
                            console.log('Save', res)
                            if (res.succeeded) {
                                that.$message.success(res.data);
                                // that.loadMyDbs()
                                that.$emit('loadMyDbs')
                            } else {
                                that.$message.error("失败:" + res.errors);
                            }
                        });

                    // alert('submit!');
                } else {
                    console.log('error submit!!');
                    return false;
                }
            });
        },
        handleClose(done) {
            done();
            // this.$confirm('确认关闭？')
            //   .then(_ => {
            //     done();
            //   })
            //   .catch(_ => { });
        }
    }
}
</script>