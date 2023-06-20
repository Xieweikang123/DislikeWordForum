<template>
  <div>
    <div style="text-align: center;">
      当前数据库:
      <el-select v-model="curSelect" placeholder="请选择">
        <el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value">
        </el-option>
      </el-select>
      <el-button type="primary" @click="dialogVisible = true">配置</el-button>
    </div>

    <el-dialog title="数据库配置" :visible.sync="dialogVisible" width="40%" :before-close="handleClose">
      <el-tabs v-model="activeTab" @tab-click="handleTabClick" tab-position="left" style="height: auto;">
        <el-tab-pane v-for="item in options" :label="item.label">
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
            <el-button type="primary" @click="testDb">测试连接</el-button>
          </el-form-item>
        </el-form>
      </el-tabs>

      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="dialogVisible = false">确 定</el-button>
      </span>
    </el-dialog>


  </div>
</template>
<script>
export default {
  data() {
    return {
      activeTab: '0',
      dbForm: {
        dbType: 'mssql',
        dbName: 'master'
      },
      dbConfigData: [],
      dialogVisible: false,
      options: [],
      curSelect: ''
    };
  },
  watch: {
    //当前选择
    curSelect() {
      console.log('watch curselect', this.curSelect)
      localStorage.setItem('curSelect', this.curSelect)
      this.resetActiveTab();
    },
    options() {
      this.resetActiveTab();

    }
  },
  mounted() {
    console.log('local storage', localStorage.getItem('curSelect'))
    this.curSelect = localStorage.getItem('curSelect')
    this.loadMyDbs()
  },

  methods: {
    resetActiveTab() {
      this.activeTab = String(this.options.findIndex(x => x.value == this.curSelect))
    },
    handleTabClick(tab) {
      // tag.index "0"
      console.log("切换到选项卡：", tab);
      var curConfig = this.dbConfigData[tab.index]
      console.log("curConfig", curConfig);
      this.dbForm = { ...curConfig }
    },

    //加载我的数据
    loadMyDbs() {
      this.$http
        .get("/api/DbManager/GetMyDbConfigs")
        .then((res) => {
          console.log('get GetMyDbConfigs', res)
          this.dbConfigData = res.data
          this.options = res.data.map(x => {
            return {
              value: x.id, label: x.connectName + `💦${x.dbName}`
            }
          })
        })
    },

    //测试连接
    testDb() {
      var that = this
      console.log('submit', that.dbForm)

      that.$http
        .post("/api/DbManager/TestConnectDb", that.dbForm)
        .then((res) => {
          console.log('TestConnectDb', res)
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

      var aa = this.$refs[formName]

      this.$refs[formName].validate((valid) => {
        if (valid) {
          console.log('submit', that.dbForm)
          that.$http
            .post("/api/DbManager/Save", that.dbForm)
            .then((res) => {
              console.log('Save', res)
              if (res.succeeded) {
                that.$message.success(res.data);
                that.loadMyDbs()
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
    resetForm(formName) {
      this.$refs[formName].resetFields();
    }
    ,
    handleClose(done) {
      done();
      // this.$confirm('确认关闭？')
      //   .then(_ => {
      //     done();
      //   })
      //   .catch(_ => { });
    }

  },
};
</script>
  
<style scoped>
.tagAllStyle {
  background-color: #447154;
  border-color: #e1f3d8;
  color: #ffffff !important;
}

.mixMode {
  /* mix-blend-mode: difference;
  color: white; */
  color: #022023;
}

.topTagContainer {
  margin-bottom: 15px;
  display: flex;
  align-items: center;
}

.contentInput img {
  width: 100px;
}

.contentInput {
  min-height: 150px;
  height: auto;
  border: 1px solid #dadada;
  padding: 2px 4px;
}

.tagItemStyle {
  color: #363f4e;
  margin: 6px 2px;
  cursor: pointer;
}

/* .leftTagContainer {
  width: 16%;
  position: fixed;
} */
.contentLine img {
  width: 50%;
}

.contentLine {
  line-height: 26px;
  white-space: pre-wrap;
}

.userHead:hover {
  opacity: 1;
}

.userHead {
  opacity: 90%;
  /* margin-top: 12px; */
  transition: opacity 0.7s;
}
</style>